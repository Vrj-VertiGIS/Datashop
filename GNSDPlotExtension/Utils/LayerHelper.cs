using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GISClient;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.PlotExtension;

namespace GEOCOM.GNSD.PlotExtension.Utils
{
	public class LayerHelper
	{
		private static IMsg _logger = new Msg(typeof(LayerHelper));
		/// <summary>
		/// iterates the layout and returns the first mapframe found
		/// this will just work if we only have one map in the layout!!!
		/// </summary>
		/// <param name="pageLayout">The page layout</param>
		/// <returns>The first mapframe found</returns>
		public static IMapFrame GetFirstMapFrameFound(IPageLayout pageLayout)
		{
			Assert.NotNull(pageLayout, "PageLayout may not be null");
			IGraphicsContainer gc = pageLayout as IGraphicsContainer;
			gc.Reset();
			IElement element = gc.Next();
			while (element != null)
			{
				if (element is IMapFrame)
				{
					IMapFrame mapFrame = element as IMapFrame;

					return mapFrame;
				}
				element = gc.Next();
			}
			throw new Exception("No mapframe found in pagelayout");
		}

		/// <summary>
		/// iterates the layout and sets the visible group layers on the first map frame found.
		/// this will just work if we only have one map in the layout!!!
		/// </summary>
		/// <param name="pageLayout">The page layout</param>
		/// <param name="visibleGroupLayers">The visible group layers. Only the specified layers will be set to visible in the map frame.</param>
		/// <returns>The first mapframe found</returns>
		public static void SetVisibleGroupLayers(IPageLayout pageLayout, IList<string> visibleGroupLayers)
		{
			IMapFrame mapFrame = GetFirstMapFrameFound(pageLayout);
			if (mapFrame == null)
				return;

			SetVisibleGroupLayers(mapFrame.Map, visibleGroupLayers);
		}

		public static TLayer GetLayerByName<TLayer>(IPageLayout pageLayout, string layerName)
			where TLayer : class, ILayer
		{
			IMapFrame mapFrame = GetFirstMapFrameFound(pageLayout);
			if (mapFrame == null)
				return null;

			for (int i = 0; i < mapFrame.Map.LayerCount; i++)
			{
				ILayer layer = mapFrame.Map.get_Layer(i);
				if (layer is TLayer && layer.Name.Equals(layerName, StringComparison.InvariantCultureIgnoreCase))
				{
					return layer as TLayer;
				}
			}

			return null;
		}

		/// <summary>
		/// Iterates over all group layers in the passed map and sets visible those group layers whose name is found in the passed groupLayers list. 
		/// Group layers not named in the list remain hidden. 
		/// If an empty or null list is passed, no operation is performed on the map's group layers
		/// </summary>
		public static void SetVisibleGroupLayers(IMap map, IList<string> groupLayers)
		{
			if (groupLayers != null && groupLayers.Count > 0 && !String.IsNullOrEmpty(groupLayers[0]))
			{
				for (int i = 0; i < map.LayerCount; i++)
				{
					ILayer layer = map.get_Layer(i);
					if (layer is IGroupLayer)
					{
						layer.Visible = false;
					}
				}

				for (int i = 0; i < map.LayerCount; i++)
				{
					ILayer layer = map.get_Layer(i);
					if (layer is IGroupLayer)
					{
						foreach (string visibleLayerName in groupLayers)
						{
							if (layer.Name.Equals(visibleLayerName, StringComparison.OrdinalIgnoreCase))
							{
								layer.Visible = true;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Return a list of invalid layers that cannot be rendred
		/// </summary>
		public static IEnumerable<ILayer> GetInvalidLayers(IMap map)
		{
			IEnumLayer layerEnumerator = map.get_Layers();
			layerEnumerator.Reset();

			List<ILayer> invalidLayers = new List<ILayer>();
			ILayer layer = layerEnumerator.Next();
			while (layer != null)
			{
				if (!layer.Valid)
					invalidLayers.Add(layer);

				// A wms layer migth not be working even though the layer.Valid propterty 
				// returns true. So an HTTP request to a wms service must be done.
				if (layer.Valid && layer is IWMSLayer && !IsWMSLayersValid(layer as IWMSLayer))
					invalidLayers.Add(layer);

				layer = layerEnumerator.Next();
			}
			return invalidLayers;
		}

		/// <summary>
		/// Makes an HTTP request over the whole layer extent to a wms service (defined by the layer) and checks the returned image data validity.
		/// </summary>
		/// <param name="wmsLayer">The WMS layer.</param>
		/// <returns>
		///   <c>true</c> if the wms service sends a valid image back .
		/// </returns>
		public static bool IsWMSLayersValid(IWMSLayer wmsLayer)
		{
			if (wmsLayer == null)
				return false;
			try
			{
				var wmsLayerValidations = PlotExtensionConfig.Instance.MxdPath.WmsLayerValidations;
				if (wmsLayerValidations == null || !wmsLayerValidations.Any())
				{
					_logger.Warn("Skipping WMS Layer validation - no validation configuration found.");
					return true;
				}

				IWMSLayerDescription wmsLayerDescription = wmsLayer.WMSLayerDescription;
				var wmsLayerValidation = wmsLayerValidations.FirstOrDefault(
					validation => wmsLayerDescription.Title.Equals(validation.LayerName, StringComparison.InvariantCultureIgnoreCase));
				if (wmsLayerValidation == null)
				{
					_logger.WarnFormat("Skipping WMS Layer validation - layer '{0}' not found in the validation configuration.", wmsLayerDescription.Title);
					return true;
				}

				var imageFormat = GetImageFormat(wmsLayer);

				_logger.InfoFormat("the ESRI ImageRequestUrl function failed. Trying to generate alternative URL.");
				// if the ArcObject way fails try to create a URL according the WMS specification
				// reference to the WMS spec http://docs.geoserver.org/stable/en/user/services/wms/reference.html
				var baseWmsUrl = wmsLayer.WMSServiceDescription.BaseURL["GetMap", "GET"];
				var imageRequestUrl =
					!string.IsNullOrWhiteSpace(wmsLayerValidation.URL)
						? wmsLayerValidation.URL
						: string.Format(CultureInfo.InvariantCulture,
							"{0}SERVICE=WMS&VERSION={1}&REQUEST={2}&STYLES=&WIDTH={3}&HEIGHT={4}&FORMAT={5}&SRS={6}&CRS={6}&BBOX={7},{8},{9},{10}&LAYERS={11}&BGCOLOR=0x{12:X}&TRANSPARENT={13}&EXCEPTIONS={14}",
							baseWmsUrl,
							wmsLayer.WMSServiceDescription.WMSVersion,
							"GetMap",
							1000,
							1000,
							imageFormat,
							wmsLayerValidation.SpatialRef,
							wmsLayerValidation.Xmin,
							wmsLayerValidation.Ymin,
							wmsLayerValidation.Xmax,
							wmsLayerValidation.Ymax,
							wmsLayerDescription.Name,
							ColorTranslator.ToOle(Color.White),
							false.ToString().ToUpper(),
							wmsLayer.WMSLayerDescription.ExceptionFormat[0]);

				_logger.Info("WMS test image URL = " + imageRequestUrl);

				WebRequest webRequest = WebRequest.Create(imageRequestUrl);
				WebResponse webResponse = webRequest.GetResponse();
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					var bitmap = new Bitmap(responseStream);
					bool valid = CheckForNonWhitePixels(bitmap);
					return valid;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static string GetImageFormat(IWMSLayer wmsLayer)
		{
			string imageFormat = wmsLayer.WMSServiceDescription.ImageFormatCount > 0
				? wmsLayer.WMSServiceDescription.ImageFormat[0]
				: "image/jpeg";
			for (int i = 0; i < wmsLayer.WMSServiceDescription.ImageFormatCount; i++)
			{
				var format = wmsLayer.WMSServiceDescription.ImageFormat[i];
				if (format.StartsWith("image/jpeg", StringComparison.InvariantCultureIgnoreCase) || format.StartsWith("image/png", StringComparison.InvariantCultureIgnoreCase))
				{
					imageFormat = format;
					break;
				}
			}
			return imageFormat;
		}

		/// <summary>
		/// Checks for non white pixels.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <returns>True if the image contains at least one non-white and non-transparent pixel.</returns>
		public static bool CheckForNonWhitePixels(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
                    Color bitmapPixel = bitmap.GetPixel(x, y);
					if (!IsWhitePixelOrTransparent(bitmapPixel))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether [is white pixel] [the specified pixel].
		/// </summary>
		/// <param name="pixel">The pixel.</param>
		/// <returns></returns>
		private static bool IsWhitePixelOrTransparent(Color pixel)
		{
			var isWhitePixel = pixel.R == Color.White.R && pixel.G == Color.White.G && pixel.B == Color.White.B;
			var isTransparent = pixel.A == 0;
			return isWhitePixel || isTransparent;
		}
	}
}
