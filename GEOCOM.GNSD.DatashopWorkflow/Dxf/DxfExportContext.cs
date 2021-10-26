using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.GeoDataBase;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSDatashop.Export.DXF;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.Dxf
{
    /// <summary>
    /// Class that handles everything for the Dxf export
    /// </summary>
    public class DxfExportContext
    {
        #region Fields

        /// <summary>
        /// The map
        /// </summary>
        private readonly IMap _map;

        /// <summary>
        /// The logger
        /// </summary>
        private IMsg _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DxfExportContext"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DxfExportContext(IMsg logger)
        {
            _logger = logger;

            _map = GetMap();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the export.
        /// </summary>
        public void CreateExport(long jobId, string directoryPath, IEnumerable<IGeometry> regionsOfInterest,
            IEnumerable<PlotSection> plotSections)
        {
            string fileName = "";

            try
            {
                var display = GetDisplay();

                var maskGeometryBag = GetMaskGeometries();

                _logger.InfoFormat("Writing Dxf at {0}", directoryPath);

                var allMapLayers = new ESRILayers(_map);
                foreach (var plotSection in plotSections)
                {
                    var layersNamesForSection = plotSection.VisibleGroupLayers.Split(';');
                    
                    var layersForSection = allMapLayers.Layers.Where(l => layersNamesForSection.Contains(l.Name, StringComparer.OrdinalIgnoreCase));

                    var layersToExport = layersForSection.Any()
                        ? layersForSection
                        : allMapLayers.Layers;

                    using (var writer = new DxfWriter(display, layersToExport, _map.ReferenceScale, _map.MapScale))
                    {
                        var rois = regionsOfInterest.Count();

                        for (int index = 0; index < rois; index++)
                        {
                            fileName = Path.Combine(directoryPath, string.Format("DxfExport_{0}_{1}.dxf", jobId, index + 1));

                            _logger.InfoFormat("Writing Dxf at {0}", fileName);

                            writer.WriteSingleDXF(fileName, true, regionsOfInterest.ElementAt(index), maskGeometryBag);

                            _logger.InfoFormat("Completed writing Dxf at {0}", fileName);
                        }
                        //writer.WriteDXFByTemplate(fileNameTemplate, true, regionsOfInterest, maskGeometryBag);
                    }
                }

                _logger.InfoFormat("Completed Dxf at {0}", directoryPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CreateExport failed for {0} with {1} regionsOfInterest", fileName, regionsOfInterest.Count()), ex);
            }
        }



        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <returns></returns>
        private IMap GetMap()
        {
            try
            {
                var mxdPath = PlotExtensionConfig.Instance.DxfMxdPath == null
                    ? PlotExtensionConfig.Instance.MxdPath.Path
                    : PlotExtensionConfig.Instance.DxfMxdPath.Path;

                _logger.InfoFormat("Opening MapDocument at {0}", mxdPath);

                var document = new MapDocument();
                document.Open(mxdPath, string.Empty);

                var map = document.Map[0];

                return map;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMap failed", ex);
            }
        }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <returns></returns>
        private IDisplay GetDisplay()
        {
            try
            {
                _logger.InfoFormat("Opening Display");

                var view = (IActiveView)_map;

                return view.ScreenDisplay;
            }
            catch (Exception ex)
            {
                throw new Exception("GetDisplay failed", ex);
            }
        }

        /// <summary>
        /// Gets the mask geometries.
        /// </summary>
        /// <returns></returns>
        private IGeometry GetMaskGeometries()
        {
            try
            {

                var path = DatashopWorkflowConfig.Instance.MaskingDataBase.Path;

                _logger.InfoFormat("Loading Mask Geometries from {0}", path);

                var workspace = GeoDbOperation.OpenWorkspace(path);

                var fc = GeoDbOperation.OpenFeatureClass(workspace, DatashopWorkflowConfig.Instance.MaskingDataBase.FeatureClass);

                _logger.InfoFormat("Loading Mask Geometries from {0}.{1}", path, DatashopWorkflowConfig.Instance.MaskingDataBase.FeatureClass);

                var cursor = fc.Search(null, false);

                IFeature f = null;

                var bag = new GeometryBagClass();

                while ((f = cursor.NextFeature()) != null)
                {
                    bag.AddGeometry(f.Shape);

                    var remarkIndex = f.Fields.FindField("REMARK");

                    var remark = f.Value[remarkIndex];

                    _logger.InfoFormat("Adding masking geometry {0} with remark: {1}", f.OID, remark);
                }

                return bag;
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetMaskGeometries(), continuing without masking. Please check your configuration and your database.", ex);

                return null;
            }
        }

        #endregion
    }
}