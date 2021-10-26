using System;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GEONIS.GeonisCentralObjects;
using GEOCOM.GEONIS.GnDxfExportSrv;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.DataWorkflow;
using GEOCOM.GNSD.DatashopWorkflow.Utils;
using Stimulsoft.Base.Drawing;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.Dxf
{
    /// <summary>
    /// Contains the logic for the Dxf Workflow
    /// </summary>
    public class DxfWorkflow : DataWorkflowBase
    {
        #region Public Methods

        /// <summary>
        /// Processes this instance.
        /// </summary>
        protected override void Process()
        {
            var dxfExportModel = DataItem.JobDescriptionModel as DxfExportModel;

            if (dxfExportModel != null)
            {
                var config = this.GetConfiguration(dxfExportModel.DxfExportName);

                if (config != null)
                {
                    var extents = this.GetJobExtents();

                    var newLyrFilePath = this.GenerateTemporaryLyrFilePath(config.LyrFilePath);

                    this.PrepareLyrFile(config.LyrFilePath, newLyrFilePath);

                    for (var i = 0; i < extents.Count; i++)
                    {
                        DatashopWorkflowDataItem.Logger.InfoFormat("Starting extraction of dxf data for perimeter={0}", i);

                        var gnsDxfExport = new GnDxfSrvExt() as IGnDxfSrvExt;

                        this.ExportPerimeter(i, extents[i], config, newLyrFilePath, gnsDxfExport);

                        DatashopWorkflowDataItem.Logger.InfoFormat("Finished extracting dxf data for perimeter={0}", i);
                    }

                    this.CleanupExport(newLyrFilePath);

                    DatashopWorkflowDataItem.Logger.InfoFormat("Creating letter PDF file");
                    Letter.CreateLetterPdf(this.DataItem);
                }
                else
                    throw new InvalidOperationException("Configuration was not found or is invalid");
            }
            else
                throw new InvalidOperationException("DxfExportModel was not found or is invalid");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="dxfExportName">Name of the DXF export.</param>
        /// <returns></returns>
        private DxfExportInfo GetConfiguration(string dxfExportName)
        {
			Assert.True(!string.IsNullOrWhiteSpace(dxfExportName), "dxfExportName cannot be null or whitespace");

            try
            {
                var dxfConfigPath = Path.Combine(GnsDatashopCommonConfig.Instance.Directories.DXFDirectory, "dxfconfig.xml");

                var dxfConfigs = ConfigReader.GetConfiguration<DxfExportConfig>(dxfConfigPath);

                return dxfConfigs.DxfExports.Where(dxf => dxf.Name == dxfExportName)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetConfiguration failed with dxfExportName: {0}", dxfExportName), ex);
            }
        }

        /// <summary>
        /// Cleans up the export.
        /// </summary>
        private void CleanupExport(string newLyrFilePath)
        {
			Assert.True(!string.IsNullOrWhiteSpace(newLyrFilePath), "newLyrFilePath cannot be null or whitespace");

            try
            {
                File.Delete(newLyrFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CleanupExport failed deleting file with path: {0}", 
                    newLyrFilePath), ex);
            }
        }

        /// <summary>
        /// Exports the perimeter.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="config">The config.</param>
        /// <param name="newLyrFilePath">The new lyr file path.</param>
        /// <param name="gnsDxfExport">The GNS DXF export.</param>
        private void ExportPerimeter(int index, IGeometry geometry, DxfExportInfo config, string newLyrFilePath, IGnDxfSrvExt gnsDxfExport)
        {
			Assert.True(index > -1, "index must be a non-negative integer");
			Assert.True(geometry != null, "geometry cannot be null");
			Assert.True(config != null, "config cannot be null");
			Assert.True(string.IsNullOrWhiteSpace(newLyrFilePath), "newLyrFilePath cannot be null or whitespace");
			Assert.True(gnsDxfExport != null, "gnsDxfExport cannot be null");

            try
            {
                DatashopWorkflowDataItem.Logger.DebugFormat("Getting output file name for index: {0} and lyrFilePath: {1}", index, newLyrFilePath);

                var outputFile = this.GetOutputFileName(index);

                DatashopWorkflowDataItem.Logger.DebugFormat("Performing export for index: {0} and lyrFilePath: {1}", index, newLyrFilePath);

                this.PerformExport(geometry, config, newLyrFilePath, outputFile, gnsDxfExport);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ExportPerimeter failed on extent index: {0}", index), ex);
            }
        }

        /// <summary>
        /// Performs the export.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="config">The config.</param>
        /// <param name="lyrFilePath">The lyr file path.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="gnsDxfExport">The GNS DXF export.</param>
        private void PerformExport(IGeometry geometry, DxfExportInfo config, string lyrFilePath, string outputFile, IGnDxfSrvExt gnsDxfExport)
        {
            Assert.True(geometry != null, "geometry cannot be null");
            Assert.True(config != null, "config cannot be null");
            Assert.True(!string.IsNullOrWhiteSpace(lyrFilePath), "lyrFilePath cannot be null or whitespace");
            Assert.True(!string.IsNullOrWhiteSpace(outputFile), "outputFile cannot be null or whitespace");
            Assert.True(gnsDxfExport != null, "gnsDxfExport cannot be null");

            DatashopWorkflowDataItem.Logger.DebugFormat("Passed parameter validation");

            try
            {
                var visibleOnly = config.VisibleOnly;
                var saveSymbols = config.SaveSymbols;
                var symbolAsFont = config.SymbolAsFont;
                var clipHole = config.ClipHole;
                var clipBoundaries = config.ClipBoundaries;
                var refScale = config.RefScale;
                var lineSpacing = config.LineSpacing;
                var csvBlockPathList = config.CsvBlockPathList;

                var xMin = geometry.Envelope.XMin;
                var xMax = geometry.Envelope.XMax;
                var yMin = geometry.Envelope.YMin;
                var yMax = geometry.Envelope.YMax;

                DatashopWorkflowDataItem.Logger.DebugFormat("Initialising errors collection");

                var errors = new GCErrors();

                DatashopWorkflowDataItem.Logger.DebugFormat("Initialising infos collection");

                var infos = new GCErrors();

                DatashopWorkflowDataItem.Logger.DebugFormat("Initialising client");

                gnsDxfExport.InitializeClient(infos, errors, false);

                DatashopWorkflowDataItem.Logger.DebugFormat("Setting extent");

                gnsDxfExport.SetExtent(xMin, yMin, xMax, yMax);

                DatashopWorkflowDataItem.Logger.DebugFormat("Setting parameters");

                gnsDxfExport.SetParameter(visibleOnly, saveSymbols, symbolAsFont, clipHole, clipBoundaries, refScale,
                                          lineSpacing, csvBlockPathList);

                DatashopWorkflowDataItem.Logger.DebugFormat("Performing Export");

                gnsDxfExport.ExportDxf(lyrFilePath, outputFile, true, "");

                DatashopWorkflowDataItem.Logger.DebugFormat("Writing protocol");

                this.CreateExportProtocol(infos, errors);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("PerformExport failed with lyrFilePath: {0} and outputFile: {1}", 
                    lyrFilePath, outputFile), ex);
            }
        }

        /// <summary>
        /// Creates the export protocol.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="errors">The errors.</param>
        private void CreateExportProtocol(IGCErrors infos, IGCErrors errors)
        {
            Assert.True(infos != null, "infos cannot be null");
			Assert.True(errors != null, "errors cannot be null");

            DatashopWorkflowDataItem.Logger.InfoFormat("Infos:");

            this.CreateExportProtocolFromRaw(infos);

            DatashopWorkflowDataItem.Logger.InfoFormat("Errors:");

            this.CreateExportProtocolFromRaw(errors);
        }

        /// <summary>
        /// Creates the export protocol.
        /// </summary>
        /// <param name="rawProtocol">The raw protocol.</param>
        private void CreateExportProtocolFromRaw(IGCErrors rawProtocol)
        {
            Assert.True(rawProtocol != null, "newLyrFilePath cannot be null or whitespace");

            for (var i = 0; i < rawProtocol.Count; i++)
                DatashopWorkflowDataItem.Logger.InfoFormat("Code: {0}, Type: {1}, Message: {2}", 
                    rawProtocol[i].Code, rawProtocol[i].ErrorType, rawProtocol[i].Msg);
        }

        /// <summary>
        /// Prepares the lyr file.
        /// </summary>
        /// <param name="originalLyrFilePath">The original lyr file path.</param>
        /// <param name="newLyrFilePath">The new lyr file path.</param>
        private void PrepareLyrFile(string originalLyrFilePath, string newLyrFilePath)
        {
            Assert.True(!string.IsNullOrWhiteSpace(originalLyrFilePath), "originalLyrFilePath cannot be null or whitespace");
            Assert.True(!string.IsNullOrWhiteSpace(newLyrFilePath), "newLyrFilePath cannot be null or whitespace");
            
            try
            {
                //NOTE: we have to make this check as the GEOCOM namespace component doesn't report any errors whatsoever
                //NOTE: so if the original .lyr file doesn't exist, the path is wrong or something else fails it appears as if
                //NOTE: everything is fine and dandy
                this.ValidateOriginalLyrFile(originalLyrFilePath);

                var datasourceReplacer = new ReplaceArcGisDatasource(new Msg(typeof(ReplaceArcGisDatasource)));

                datasourceReplacer.ReplaceDatasourceInLyrFile(originalLyrFilePath, DataItem.Extraction.WorkspaceConnection, newLyrFilePath);

                //NOTE: we have to make this check as the GEOCOM namespace component doesn't report any errors whatsoever
                //NOTE: so if there is a problem making the temp copy or something else fails it appears as if
                //NOTE: everything is fine and dandy
                this.ValidateTemporaryLyrFile(newLyrFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("PrepareLyrFile failed with originalLyrFilePath: {0} and newLyrFilePath: {1}", 
                    originalLyrFilePath, newLyrFilePath), ex);
            }
        }

       

        /// <summary>
        /// Validates the original lyr file.
        /// </summary>
        /// <param name="originalLyrFilePath">The original lyr file path.</param>
        private void ValidateOriginalLyrFile(string originalLyrFilePath)
        {
            Assert.True(!string.IsNullOrWhiteSpace(originalLyrFilePath), "originalLyrFilePath cannot be null or whitespace");

            if (!File.Exists(originalLyrFilePath))
                throw new InvalidOperationException(string.Format("Cannot find original .lyr file at '{0}'. File does not exist.", originalLyrFilePath));
        }

        /// <summary>
        /// Validates the temporary lyr file.
        /// </summary>
        private void ValidateTemporaryLyrFile(string newLyrFilePath)
        {
            Assert.True(!string.IsNullOrWhiteSpace(newLyrFilePath), "newLyrFilePath cannot be null or whitespace");

            if (!File.Exists(newLyrFilePath))
                throw new InvalidOperationException(string.Format("Cannot find temporary .lyr file at '{0}'. File does not exist.", newLyrFilePath));
        }

        /// <summary>
        /// Generates the temporary lyr file path.
        /// </summary>
        /// <param name="lyrFilePath">The lyr file path.</param>
        /// <returns></returns>
        private string GenerateTemporaryLyrFilePath(string lyrFilePath)
        {
            Assert.True(!string.IsNullOrWhiteSpace(lyrFilePath), "lyrFilePath cannot be null or whitespace");

            try
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(lyrFilePath);

                var lyrFileDirectory = Path.GetDirectoryName(lyrFilePath);

                var temporaryFilename =  string.Format("{0}_{1}.lyr", fileNameWithoutExtension, this.DataItem.JobId);

                return Path.Combine(lyrFileDirectory, temporaryFilename);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GenerateTemporaryLyrFilePath failed with lyrFilePath: {0}", lyrFilePath), ex);
            }
        }

        #endregion
    }
}