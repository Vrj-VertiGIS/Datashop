using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.Layout;
using GEOCOM.GNSD.PlotExtension.MapExporter;
using MapExtent = GEOCOM.GNSD.Common.Model.MapExtent;
using GEOCOM.GNSD.PlotExtension.Utils;

namespace GEOCOM.GNSD.PlotExtension.PlotExtension
{
    /// <summary>
    /// 
    /// </summary>
    public class GNSPlotExtension : IDisposable
    {
        /// <summary>
        /// Logger instance, using the log4net infrastructure
        /// </summary>
        private readonly IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        // the layoutmanager handles the PageLayout to create plots and exports
        private readonly LayoutMgr _layoutMgr;

        public GNSPlotExtension()
        {
            // Create instance for layoutmanager
            _layoutMgr = new LayoutMgr();
        }

        /// <summary>
        /// Creates a Vector or Bitmap export from layoutview
        /// </summary>
        public void CreateExport(
                                 string exportType,
                                 string exportFileName,
                                 MapExtent mapExtent,
                                 Dictionary<string, string> variables,
                                 IList<string> visibleLayers)
        {
            // Changes Layout, and sets the plot extent
            SetMapExtentFor(mapExtent);

            var actualPolygon = this.GetMapExtentFor(mapExtent);
            var actualPolygonCentroid = ((IArea)actualPolygon).Centroid;
            var format = "0.00";
            var allVariables = new Dictionary<string, string>(variables)
                {
                    {
                        "plot_frame_extent", string.Format(
                            "{0}, {1}, {2}, {3}", 
                            actualPolygon.Envelope.XMin.ToString(format)
                            , actualPolygon.Envelope.YMin.ToString(format), 
                            actualPolygon.Envelope.XMax.ToString(format), 
                            actualPolygon.Envelope.YMax.ToString(format))
                    },
                    {
                        "plot_frame_centroid_x",  actualPolygonCentroid.X.ToString(format)
                    },
                    {
                        "plot_frame_centroid_y",  actualPolygonCentroid.Y.ToString(format)
                    }
                };

            // replace text variables
            PageLayoutManager.Instance.ReplaceTextVars(allVariables);

            // export to pdf
            var exporter = new LayoutExporter();

            var exportFile = new FileInfo(exportFileName);

            var pageLayout = PageLayoutManager.Instance.CurrentPageLayout;

            _log.DebugFormat("Creating Export for file({0})", exportFileName);

            exporter.ExportMapToPdf(actualPolygon, pageLayout, exportFile.Directory.FullName, exportFile.Name, visibleLayers);
        }

	    /// <summary>
	    /// Gets the overview map extent from polygons.
	    /// </summary>
	    /// <param name="perimeters"></param>
	    /// <param name="overviewPath"></param>
	    /// <param name="maxScale"></param>
	    /// <param name="mapExtentPolygon">The extent polygons.</param>
	    /// <returns></returns>
	    public MapExtent GetOverviewMapExtentFromPolygons(ExportPerimeter[] perimeters, string overviewPath, int? maxScale)
        {
            var mapExtentPolygon = this.CalculateOverviewExtent(perimeters);

            var centerX = (mapExtentPolygon.Envelope.XMax + mapExtentPolygon.Envelope.XMin) / 2;
            var centerY = (mapExtentPolygon.Envelope.YMax + mapExtentPolygon.Envelope.YMin) / 2;

            var pageCoords = PlotTemplateHelper.GetMapExtentInPageCoords(overviewPath);

            var pageWidthInMeters = pageCoords.Width / 100;
            var pageHeightInMeters = pageCoords.Height / 100;

            var widthScale = mapExtentPolygon.Envelope.Width / pageWidthInMeters;
            var heightScale = mapExtentPolygon.Envelope.Height / pageHeightInMeters;
            var scale = Math.Max(widthScale, heightScale) * 1.05; // 1.05 - ensures extra map space around the plot frames
		        if (maxScale != null && maxScale > scale)
			          scale = maxScale.Value;
		    

            var mapExtent = new MapExtent
            {
                CenterX = centerX,
                CenterY = centerY,
                Rotation = 0.0,
                PlotTemplate = overviewPath,
                Scale = scale,
                Id = Guid.NewGuid()
                         .ToString()
            };

            return mapExtent;
        }

        /// <summary>
        /// Gets the empty plot frame text.
        /// </summary>
        /// <returns></returns>
        public string GetEmptyPlotFrameText()
        {
            Assert.NotNull(PlotExtensionConfig.Instance.EmptyPlot, "Missing or invalid configuration element 'emptyplot' in {0}", PlotExtensionConfig.Instance.ConfigFilePath);
            Assert.NotNull(PlotExtensionConfig.Instance.EmptyPlot.Text, "Missing or invalid configuration element 'emptyplot' in {0}", PlotExtensionConfig.Instance.ConfigFilePath);

            return PlotExtensionConfig.Instance.EmptyPlot.Text;
        }

        /// <summary>
        /// Gets the background plot frame text.
        /// </summary>
        /// <returns></returns>
        public string GetBackgroundPlotFrameText()
        {
            Assert.NotNull(PlotExtensionConfig.Instance.BackgroundPlot, "Missing or invalid configuration element 'backgroundplot' in {0}", PlotExtensionConfig.Instance.ConfigFilePath);
            Assert.NotNull(PlotExtensionConfig.Instance.BackgroundPlot.Text, "Missing or invalid configuration element 'backgroundplot' in {0}", PlotExtensionConfig.Instance.ConfigFilePath);

            return PlotExtensionConfig.Instance.BackgroundPlot.Text;
        }

        /// <summary>
        /// Determines whether [is plot frame empty] [the specified export perimeter].
        /// </summary>
        /// <param name="exportPerimeter">The export perimeter.</param>
        /// <param name="visibleLayers">The visible layers.</param>
        /// <returns></returns>
        public bool IsPlotFrameEmpty(ExportPerimeter exportPerimeter, string[] visibleLayers)
        {
            IPolygon mapExtent = GetMapExtentFor(exportPerimeter.MapExtent);

            IMapDocument document = new MapDocument();
            document.Open(PlotExtensionConfig.Instance.MxdPath.Path, string.Empty);
            IMap map = document.get_Map(0);

            

            LayerHelper.SetVisibleGroupLayers(map, visibleLayers);

            bool mapHasOnlyWhiteSurface = MapHasOnlyWhiteSurface(map as IActiveView, mapExtent.Envelope);
            document.Close();

            return mapHasOnlyWhiteSurface;
        }

        /// <summary>
        /// Opens the mxt/mxd document and calculates the mapextent out of scale, center, rotation and the width/heith from mxt pagelayout
        /// </summary>
        public IPolygon GetMapExtentFor(MapExtent mapExtent)
        {
            PageLayoutManager.Instance.SetPlotTemplateName(mapExtent.PlotTemplate);
            IPolygon extent = _layoutMgr.GetPlotExtent(mapExtent.CenterX, mapExtent.CenterY, mapExtent.Scale, mapExtent.Rotation);

            return extent;
        }

        /// <summary>
        /// Sets the map extent for.
        /// </summary>
        /// <param name="mapExtent">The map extent.</param>
        private void SetMapExtentFor(MapExtent mapExtent)
        {
            _layoutMgr.SetPlotExtent(mapExtent.CenterX, mapExtent.CenterY, mapExtent.Scale, mapExtent.Rotation);
        }

        /// <summary>
        /// Calculates the overview extent.
        /// </summary>
        /// <param name="perimeters">The perimeters.</param>
        /// <returns></returns>
        private IPolygon CalculateOverviewExtent(IEnumerable<ExportPerimeter> perimeters)
        {
            var plotFrameExtents = perimeters.ToList()
                                             .ConvertAll(p => this.GetMapExtentFor(p.MapExtent));
            // vrj to myp: just simplification
            var envelope = new EnvelopeClass
            {
                YMin = plotFrameExtents.Min(x => x.Envelope.YMin),
                XMin = plotFrameExtents.Min(x => x.Envelope.XMin),
                YMax = plotFrameExtents.Max(x => x.Envelope.YMax),
                XMax = plotFrameExtents.Max(x => x.Envelope.XMax)
            };

            var polygon = (ISegmentCollection)new PolygonClass();

            polygon.SetRectangle(envelope);

            return (IPolygon)polygon;
        }

        /// <summary>
        /// Maps the has only white surface.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        private bool MapHasOnlyWhiteSurface(IActiveView view, IEnvelope region)
        {
            Assert.NotNull(view, "View may not be null");

            try
            {
                const double resolution = 96;
                const double width1 = 0.3;

                double aspect = region.Width / region.Height;
                double height1 = width1 / aspect;

                tagRECT pictureSizeInPixel;
                pictureSizeInPixel.left = 0;
                pictureSizeInPixel.top = 0;
                pictureSizeInPixel.right = (int)(resolution / 0.0254 * width1);
                pictureSizeInPixel.bottom = (int)(resolution / 0.0254 * height1);

                Bitmap bitmap = ExportActiveViewToBitmap(view, region, resolution, pictureSizeInPixel);

                bool otherColorFound = LayerHelper.CheckForNonWhitePixels(bitmap);

                return !otherColorFound;
            }
            catch (Exception e)
            {
                _log.ErrorFormat("Exception occurred during empty map check. {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Exports the active view to bitmap.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="region">The region.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="pictureSizeInPixel">The picture size in pixel.</param>
        /// <returns></returns>
        private Bitmap ExportActiveViewToBitmap(IActiveView view, IEnvelope region, double resolution, tagRECT pictureSizeInPixel)
        {
            Bitmap bitmap = new Bitmap(pictureSizeInPixel.right, pictureSizeInPixel.bottom);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IntPtr hwnd = g.GetHdc();

                // Creates a 32bit PNG with alpha channel
                view.Output((int)hwnd, (int)resolution, ref pictureSizeInPixel, region, null);
                g.ReleaseHdc(hwnd);
            }
            return bitmap;
        }

	    #region IDisposable Members

        public void Dispose()
        {
            // The resources need to be released in the PlotWorkflow.cs!!
        }

        #endregion
    }
}
