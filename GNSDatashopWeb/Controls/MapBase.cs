using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;

namespace GEOCOM.GNSD.Web.Controls
{
	public class MapBase : UserControl
	{
		#region Public Properties

		/// <summary>
		/// Gets type of the map layer. Either Dynamic or Tiled.
		/// </summary>
		public string MapServiceLayer
		{
			get
			{
				return DatashopWebConfig.Instance.MapService.Levels == MapServiceLevel.dynamic
						   ? "ArcGISDynamicMapServiceLayer"
						   : "ArcGISTiledMapServiceLayer";
			}
		}

		/// <summary>
		/// Gets or sets the previous map extent.
		/// </summary>
		/// <value>
		/// The previous map extent.
		/// </value>
		public string PreviousMapExtent { get; set; }

		/// <summary>
		/// Gets the slider.
		/// </summary>
		protected string Slider
		{
			get
			{
				return string.IsNullOrWhiteSpace(DatashopWebConfig.Instance.MapService.Slider)
						   ? "false"
						   : DatashopWebConfig.Instance.MapService.Slider.ToLower();
			}
		}

		protected string UseProxy
		{
			get
			{
				return string.IsNullOrWhiteSpace(DatashopWebConfig.Instance.MapService.UseProxy)
						   ? "true"
						   : DatashopWebConfig.Instance.MapService.UseProxy.ToLower();
			}
		}

		/// <summary>
		/// Gets the levels of detail.
		/// </summary>
		protected string LevelsOfDetail
		{
			get
			{
				return DatashopWebConfig.Instance.MapService.LevelsOfDetail != null
						   ? string.Join(", ", DatashopWebConfig.Instance.MapService.LevelsOfDetail.Levels.ToList()
												   .ConvertAll(l => l.Scale.ToString()))
						   : "";

			}
		}

		/// <summary>
		/// Gets the scale resolution ratio.
		/// </summary>
		protected string ScaleResolutionRatio
		{
			get
			{
				return DatashopWebConfig.Instance.MapService.LevelsOfDetail != null
						   ? DatashopWebConfig.Instance.MapService.LevelsOfDetail.ScaleResolutionRatio
						   : "0";
			}
		}

		/// <summary>
		/// Gets the notification text.
		/// </summary>
		protected string NotificationText
		{
			get { return WebLanguage.LoadStr(3943, "polygon(s) out of bounds"); }
		}

		/// <summary>
		/// Id of the hidden field for saving map extents from a previous job request.
		/// </summary>
		public string hfMapExtentsClientId { get; set; }


		/// <summary>
		/// URL of the Arc map rest map service.
		/// </summary>
		public string MapServiceURL
		{
			get { return DatashopWebConfig.Instance.MapService.ServiceUrl; }
		}

		/// <summary>
		/// Gets extent of the map that is used at initialization of the map.
		/// </summary>
		public string InitialMapExtent
		{
			get
			{
				if (DatashopWebConfig.Instance.MapService.InitialExtent == null)
					return "null";

				var extent = DatashopWebConfig.Instance.MapService.InitialExtent.ToAnonymousType();

				return GetJsExtent(extent);
			}
		}

		/// <summary>
		/// Get the upper limit on the number of polygons that can be added to the map.
		/// </summary>
		public int MaxPolygons
		{
			get
			{
				int maxPolygons = 0;
				switch (DatashopWebConfig.Instance.RequestPageConfig.ActiveMode)
				{
					case RequestPageMode.Plot:
						maxPolygons = DatashopWebConfig.Instance.RequestPageConfig.PlotMode.MaxPolygons;
						break;
					case RequestPageMode.Data:
						maxPolygons = DatashopWebConfig.Instance.RequestPageConfig.DataMode.MaxPolygons;
						break;
				}

				if (maxPolygons <= 0)
				{
					maxPolygons = 1;
				}

				return maxPolygons;
			}
		}

		/// <summary>
		/// Gets the web map configuration path.
		/// </summary>
		/// <value>
		/// The web map configuration path.
		/// </value>
		public string WebMapConfigurationPath
		{
			get { return DatashopWebConfig.Instance.MapService.WebMapConfigurationPath; }
		}

		/// <summary>
		/// Gets a value indicating whether [use web map].
		/// </summary>
		/// <value>
		///   <c>true</c> if [use web map]; otherwise, <c>false</c>.
		/// </value>
		public bool UseWebMap
		{
			get { return !string.IsNullOrWhiteSpace(WebMapConfigurationPath) && File.Exists(this.WebMapConfigurationPath); }
		}

		/// <summary>
		/// Gets the web map configuration.
		/// </summary>
		/// <value>
		/// The web map configuration.
		/// </value>
		public string WebMapConfiguration
		{
			get { return File.ReadAllText(this.WebMapConfigurationPath); }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Handles the loading of the previous map extents.
		/// </summary>
		protected void HandleLoadingPreviousMapExtents()
		{
			if (Request.QueryString["useLastMapExtents"] == "true" && Session["lastMapExtents"] != null)
			{
				var extentInfo = (ExtentInfo)Session["lastMapExtents"];
				var extent = extentInfo.ToAnonymousType();
				PreviousMapExtent = GetJsExtent(extent);
			}
			else
			{
				PreviousMapExtent = InitialMapExtent;
			}
		}

		#endregion

		private static string GetJsExtent(object extent)
		{
			return string.Format(" new esri.geometry.Extent({0})", new JavaScriptSerializer().Serialize(extent));
		}
	}
}