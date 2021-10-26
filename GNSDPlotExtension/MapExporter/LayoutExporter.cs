using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

using GEOCOM.Common.Logging;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.Layout;
using GEOCOM.GNSD.PlotExtension.Utils;
using System.Linq;

namespace GEOCOM.GNSD.PlotExtension.MapExporter
{

    public class LayoutExporter
    {
        // log4net
        private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        public void ExportMapToPdf(IPolygon actualPolygon, IPageLayout pageLayout, string targetDir, string filename, IList<string> visibleLayers)
        {
            _log.DebugFormat("Start exporting map to pdf ({0})", filename);

            var usedDpi = 600;
            var usedQuality = 3;

            var usedVectorOptions = esriPictureSymbolOptions.esriPSORasterizeIfRasterData;
            var usedLayerOptions = esriExportPDFLayerOptions.esriExportPDFLayerOptionsNone;

            var info = PlotExtensionConfig.Instance.Export.PdfExportInfo;

            if (info != null)
            {
                usedLayerOptions = info.Layers
                                    ? esriExportPDFLayerOptions.esriExportPDFLayerOptionsLayersOnly
                                    : esriExportPDFLayerOptions.esriExportPDFLayerOptionsNone;
                usedVectorOptions = info.Vector
                                        ? esriPictureSymbolOptions.esriPSORasterizeIfRasterData
                                        : esriPictureSymbolOptions.esriPSORasterize;
                usedQuality = info.Quality;
                usedDpi = info.Dpi;
            }

            IExport pdfExporter = new ExportPDFClass();
            (pdfExporter as IExportPDF).Compressed = true;
            (pdfExporter as IExportPDF).ImageCompression = esriExportImageCompression.esriExportImageCompressionLZW;
            (pdfExporter as IExportPDF).EmbedFonts = true;
            (pdfExporter as IExportPDF2).ExportPDFLayersAndFeatureAttributes = usedLayerOptions;
            (pdfExporter as IExportPDF2).ExportMeasureInfo = true;
            (pdfExporter as IExportVectorOptions).PolygonizeMarkers = false; // für versatel als notlösung!!!
            ////(result as IExportVectorOptions).Set_MaxVertexNumber(xx);
            (pdfExporter as IExportVectorOptionsEx).ExportPictureSymbolOptions = usedVectorOptions;
            (pdfExporter as IExportColorspaceSettings).Colorspace = esriExportColorspace.esriExportColorspaceRGB;
            (pdfExporter as IOutputRasterSettings).ResampleRatio = usedQuality;

            //(pdfExporter as IExportVectorOptionsEx).

            // Zu exportierenden View bestimmen
            LayerHelper.SetVisibleGroupLayers(pageLayout, visibleLayers);
            var activeView = pageLayout as IActiveView;

            // get pagesize from layout
            pageLayout.Page.Units = esriUnits.esriCentimeters;
            double width, height;
            pageLayout.Page.QuerySize(out width, out height);

            // set PixelBound to pExporter with the desired dpi;
            // the output uses  the same target extent as  tagRect struct 
            var pixelExtent = new tagRECT
            {
                right = 0, 
                bottom = 0
            };

            pixelExtent.right = (int)(width * usedDpi / 2.54);
            pixelExtent.bottom = (int)(height * usedDpi / 2.54);

            IEnvelope env = new EnvelopeClass();
            env.PutCoords(0, 0, pixelExtent.right, pixelExtent.bottom);
            pdfExporter.PixelBounds = env;

            // set the same resolution for the pdf exporter
            // this will calculate the correct page width and height for printing
            pdfExporter.Resolution = usedDpi;
            
            // set the outputname
            pdfExporter.ExportFileName = System.IO.Path.Combine(targetDir, filename);

            // Export starten

            // Delete old file if it exists already (should not)
            if (File.Exists(pdfExporter.ExportFileName)) File.Delete(pdfExporter.ExportFileName);

            // Ein erneutes Refresh hilft.
            _log.DebugFormat("Refreshing map after pdf export ({0})", filename);

            // Make sure, all graphic elements are painted
            activeView.Refresh();

            //Check layers' validity before the export
            CheckLayersValidityAndNotify(pageLayout, OnInvalidLayers);

            
            // get devicecontext to output the layoutview
            int hDC = pdfExporter.StartExporting();

            //Make a color to draw the polyline 
            //IRgbColor rgbColor = new RgbColorClass();
            //rgbColor.Red = 255;


            //var polygonClass = new PolygonClass();
            //polygonClass.AddPoint(activeView.Extent.UpperLeft);
            //polygonClass.AddPoint(activeView.Extent.UpperRight);
            //polygonClass.AddPoint(activeView.Extent.LowerRight);
            //polygonClass.AddPoint(activeView.FullExtent.LowerLeft);
            //polygonClass.AddPoint(activeView.FullExtent.UpperLeft);

            //PageLayoutManager.Instance.AddGraphicToMap(polygonClass,
            //    rgbColor, rgbColor);

            // output to this device context. params: hdc, dpi, sourceextent (null for full page), trackcancelobj
            activeView.Output(hDC, usedDpi, ref pixelExtent, null, null);

            // release used resources
            pdfExporter.FinishExporting();
            pdfExporter.Cleanup();

            //Check layers' validity after the export as well - minimizing the chance of not-detecting a data corruption during the rendering
            CheckLayersValidityAndNotify(pageLayout, OnInvalidLayers);

            // Export fertig
            _log.DebugFormat("Pdf done ({0})", filename);

            // TODO Resampling wieder zurücksetzen 
            //  TPrintManager.SetOutputQuality(pActiveView, oldResamplRate);
        }
		

        /// <summary>
        /// Check validity of layers in the pageLayout's first map-frame.
        /// Invokes the callback if an invalid layer is encountered.
        /// </summary>
        /// <returns>True if all layers in the map are valid, false otherwise.</returns>
        private bool CheckLayersValidityAndNotify(IPageLayout pageLayout, Action<IEnumerable<ILayer>, string, string> errorCallback)
        {
            IMap map = LayerHelper.GetFirstMapFrameFound(pageLayout).Map;

            IEnumerable<ILayer> invalidLayers = LayerHelper.GetInvalidLayers(map);

            if (invalidLayers.Any() && errorCallback != null)
            {
                errorCallback(invalidLayers, PlotExtensionConfig.Instance.MxdPath.Path, "In the map document '{0}' are following layers invalid: {1}.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// A callback for invalid layers. If invalidLayers were found, throws an exception.
        /// </summary>
        /// <exception cref="LayoutExportException"></exception>
        private void OnInvalidLayers(IEnumerable<ILayer> invalidLayers, string mapFileMxdPath, string formatMessage)
        {
            if (!invalidLayers.Any())
                return;

            var invalidLayersNames = ConvertInvalidLayersToString(invalidLayers);

            string message = string.Format(formatMessage, mapFileMxdPath, invalidLayersNames);
            _log.DebugFormat("PDF export error: ({0})", message);

            var layoutExportException = new LayoutExportException(message);
            layoutExportException.InvalidLayersNames = invalidLayersNames;
            layoutExportException.MapFileMxdPath = mapFileMxdPath;

            throw layoutExportException;
        }


        /// <summary>
        /// Converts the passed invalid layers into a comma separated string. 
        /// </summary>
        /// <param name="invalidLayers"></param>
        /// <returns></returns>
        private static string ConvertInvalidLayersToString(IEnumerable<ILayer> invalidLayers)
        {
            string layerSeparator = ", ";

            string invalidLayersNames = string.Empty;

            foreach (var invalidLayer in invalidLayers)
            {
                if (invalidLayer is IFeatureLayer)
                {
                    var invalidFeatureLayer = invalidLayer as IFeatureLayer;
                    invalidLayersNames += string.Format("{0} ({1})", invalidFeatureLayer.Name,
                                                        invalidFeatureLayer.DataSourceType);
                }
                else
                {
                    invalidLayersNames += string.Format("{0}", invalidLayer.Name);
                }
                invalidLayersNames += layerSeparator;
            }

            //trim the last separtor
            invalidLayersNames = invalidLayersNames.Remove(invalidLayersNames.Length - layerSeparator.Length);
            return invalidLayersNames;
        }
    }
}
