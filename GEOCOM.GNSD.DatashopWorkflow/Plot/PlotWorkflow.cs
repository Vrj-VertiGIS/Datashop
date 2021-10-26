using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.Dxf;
using GEOCOM.GNSD.DatashopWorkflow.Mailer;
using GEOCOM.GNSD.DatashopWorkflow.Utils;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.Layout;
using GEOCOM.GNSD.PlotExtension.MapExporter;
using GEOCOM.GNSD.PlotExtension.PlotExtension;
using GEOCOM.GNSD.PlotExtension.Utils;
using GEOCOM.GNSDatashop.Model;
using MapExtent = GEOCOM.GNSD.Common.Model.MapExtent;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.Plot
{
    /// <summary>
    /// Class that defines the workflow for Plot extracts
    /// </summary>
    public class PlotWorkflow : DatashopWorkflowBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the number of pages.
        /// </summary>
        /// <value>
        /// The number of pages.
        /// </value>
        public int NumberOfPages
        {
            get
            {
                return this._numberOfPages;
            }
            set
            {
                this._numberOfPages = value;
                DataItem.Variables["plot_total_pages"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the current export perimeter.
        /// </summary>
        /// <value>
        /// The current export perimeter.
        /// </value>
        public ExportPerimeter CurrentExportPerimeter
        {
            get
            {
                return this._currentExportPerimeter;
            }
            set
            {
                var plotAlreadySet = this._currentExportPerimeter == value;
                if (plotAlreadySet)
                    return;

                this._currentExportPerimeter = value;

                // To be backward compatible
                if (string.IsNullOrEmpty(value.MapExtent.Id))
                {
                    if (!DataItem.Variables.ContainsKey("plot_frame_id"))
                    {
                        DataItem.Variables["plot_frame_id"] = "1";
                    }
                    else
                    {
                        int numericId = int.Parse(DataItem.Variables["plot_frame_id"]);
                        numericId++;
                        DataItem.Variables["plot_frame_id"] = numericId.ToString();
                    }
                }
                else
                {
                    DataItem.Variables["plot_frame_id"] = value.MapExtent.Id;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current plot section.
        /// </summary>
        /// <value>
        /// The current plot section.
        /// </value>
        public PlotSection CurrentPlotSection
        {
            get
            {
                return this._currentPlotSection;
            }
            set
            {
                this._currentPlotSection = value;
                DataItem.Variables["plot_section_name"] = Path.GetFileNameWithoutExtension(value.Name);
                DataItem.Variables["plot_section_desc"] = value.Description;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get
            {
                return this._fileName;
            }
            set
            {
                this._fileName = value;
                DataItem.Variables["plot_file_name"] = Path.GetFileName(value);
            }
        }

        /// <summary>
        /// Gets or sets the current empty plot frame text.
        /// </summary>
        /// <value>
        /// The current empty plot frame text.
        /// </value>
        public string CurrentEmptyPlotFrameText
        {
            get
            {
                return this._currentEmptyPlotFrameText;
            }
            set
            {
                this._currentEmptyPlotFrameText = value;
                DataItem.Variables["plot_empty_text"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current background plot frame text.
        /// </summary>
        /// <value>
        /// The current background plot frame text.
        /// </value>
        public string CurrentBackgroundPlotFrameText
        {
            get
            {
                return this._currentBackgroundPlotFrameText;
            }
            set
            {
                this._currentBackgroundPlotFrameText = value;
                DataItem.Variables["plot_background_text"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of sections.
        /// </summary>
        /// <value>
        /// The number of sections.
        /// </value>
        public int NumberOfSections
        {
            get
            {
                return this._numberOfSections;
            }
            set
            {
                this._numberOfSections = value;
                DataItem.Variables["plot_section_total_sections"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
            set
            {
                this._currentPage = value;
                DataItem.Variables["plot_current_page"] = value.ToString();
            }
        }

        #endregion

        #region Private Members

        private int _numberOfPages;

        private int _currentPage;

        private int _numberOfSections;

        private PlotSection _currentPlotSection;

        private ExportPerimeter _currentExportPerimeter;

        private string _fileName;

        private string _currentEmptyPlotFrameText;

        private string _currentBackgroundPlotFrameText;

        private bool currentPlotSectionEmtpy;

        #endregion

        #region protected methods

        /// <summary>
        /// Gets the job extents.
        /// </summary>
        /// <returns>A list of IGeometry</returns>
        protected override List<IGeometry> GetJobExtents()
        {
            return GetExtentsFromJob();
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        protected override void Process()
        {
            DatashopWorkflowDataItem.Logger.InfoFormat("Process called for JobId={0}", DataItem.JobId);

            ExportModel exportModel = DataItem.JobDescriptionModel as ExportModel;
            IList<PlotSection> sections = GetPlotSections();

            if (exportModel != null)
            {
                InitProperties(exportModel.Perimeters.Length, sections.Count);

                DatashopWorkflowDataItem.Logger.InfoFormat("Creating letter PDF file");
                Letter.CreateLetterPdf(this.DataItem);

                ExportPlotframes(exportModel, sections);

                if (DataItem.DxfExport) 
                    ExportDxf(sections);

                var overview = DatashopWorkflowConfig.Instance.PlotFileName.Overview;
                if (overview)
                    ExportOverview(exportModel);
                else
                    DatashopWorkflowDataItem.Logger.Warn("Skipping the extents overview because of incomplete configuration");


                TryToCleanUpTempFolder();
            }
            else
            {
                throw new Exception("Invalid export model.");
            }
        }

        private void ExportDxf(IEnumerable<PlotSection> plotSections)
        {
            try
            {

                var extents = GetJobExtents();

                DatashopWorkflowDataItem.Logger.InfoFormat("Found {0} extents for Dxf Export", extents.Count);

                var directory = Path.GetDirectoryName(DataItem.JobOutput);

                DatashopWorkflowDataItem.Logger.InfoFormat("Using {0} for job output", directory);

                var ctx = new DxfExportContext(DatashopWorkflowDataItem.Logger);

                DatashopWorkflowDataItem.Logger.Info("Beginning Dxf export");

                ctx.CreateExport(DataItem.JobId, directory, extents, plotSections);

                DatashopWorkflowDataItem.Logger.Info("Finished Dxf export");
            }
            catch (Exception ex)
            {
                throw new Exception("Fatal error in ExportDxf", ex);
            }
        }

        /// <summary>
        /// Try to delete all temporary files created during plotting.
        /// </summary>
        /// <remarks>Solves issue DATASHOP-254</remarks>
        private static void TryToCleanUpTempFolder()
        {
            var currentProcessStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
            var tempPath = Path.GetTempPath();

            var tempDirectory = new DirectoryInfo(tempPath);
            var fileInfosTMP = tempDirectory.GetFiles("~DF*.TMP", SearchOption.TopDirectoryOnly); // observed in the temp directory
            var fileInfosMEM = tempDirectory.GetFiles("list*.mem", SearchOption.TopDirectoryOnly); // observed in the temp direcotry
            var tempFilesToDelete = fileInfosTMP.Union(fileInfosMEM);
            foreach (var tempFile in tempFilesToDelete)
            {
                var tempFileCreatedDuringCurrentProcessRunTime = currentProcessStartTime < tempFile.CreationTime;
                if (tempFileCreatedDuringCurrentProcessRunTime)
                {
                    try
                    {
                        tempFile.Delete();
                    }
                    catch (Exception){} 
                }
            }
        }

        private void ExportPlotframes(ExportModel exportModel, IList<PlotSection> sections)
        {
            var perimeters = exportModel.Perimeters.OrderBy(p => p.MapExtent.Id).ToArray();
            foreach (var exportPerimeter in perimeters)
            {
                foreach (var plotSection in sections)
                {
                    ExportMapPdf(exportPerimeter, plotSection, DataItem.PlotFileName.TargetFile);
                    CurrentPage++;
                }
            }
        }

        private void ExportOverview(ExportModel exportModel)
        {
            DatashopWorkflowDataItem.Logger.InfoFormat("Creating overview");

            using (GNSPlotExtension plotExtension = new GNSPlotExtension())
            {
                Assert.NotNullOrEmpty(DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.Template,
                    "The attribute template of the overview tag must not be empty.");
                Assert.NotNullOrEmpty(DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.TargetFile,
                 "The attribute targetfile of the overview tag must not be empty.");

                
                // if overview template is Mxd then use the whole mxd (both map and layout)
                // if overview template is Mxt then stay with the original map 
                var overViewIsMxd =
                    Path.GetExtension(DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.Template).
                    Equals(".mxd", StringComparison.InvariantCultureIgnoreCase);
                if (overViewIsMxd)
                {
                    // TODO refactor the way the MxdPath is set. This would required, however, pretty big efford
                    // Therefore this ugly way of setting directly the configuration property is used. 
                    PlotExtensionConfig.Instance.MxdPath.Path = DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.Template;
                }
              
                var mapExtentPolygon = plotExtension.GetOverviewMapExtentFromPolygons(exportModel.Perimeters,
					DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.Template, 
					DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.MaxScale);

                PageLayoutManager.Instance.SetPlotTemplateName(DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.Template);

                var overViewExportPerimeter = new ExportPerimeter()
                {
                    MapExtent = mapExtentPolygon
                };

                var plotSection = new PlotSection
                {
                    Name = "overview", // this value is not important 
                    VisibleGroupLayers = ""
                };

                var extentsLayer = LayerHelper.GetLayerByName<IFeatureLayer>(
                    PageLayoutManager.Instance.CurrentPageLayout,
                    DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.ExtentsLayer);

                var highlightExtents =
                        extentsLayer is IFeatureLayerDefinition &&
                        !string.IsNullOrEmpty(DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.JobIdColumn);
                if (highlightExtents)
                {
                    extentsLayer.Visible = true;
                    var jobFilter =
                        string.Format("{0} = {1}",
                        DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.JobIdColumn,
                        DataItem.JobId);
					DatashopWorkflowDataItem.Logger.DebugFormat("Filtering layer='{0}' with the filter='{1}.'", extentsLayer.Name, jobFilter);
                    (extentsLayer as IFeatureLayerDefinition).DefinitionExpression = jobFilter;
                }
                else
                {
					DatashopWorkflowDataItem.Logger.Debug("Skipping extents highlighting because of incomplete configuration");
					DatashopWorkflowDataItem.Logger.Debug("DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.JobIdColumn = " + DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.JobIdColumn);
					DatashopWorkflowDataItem.Logger.Debug("extentsLayer is IFeatureLayerDefinition = " + (extentsLayer is IFeatureLayerDefinition));
					DatashopWorkflowDataItem.Logger.Debug("PageLayoutManager.Instance.CurrentPageLayout == null = " + (PageLayoutManager.Instance.CurrentPageLayout == null));
					DatashopWorkflowDataItem.Logger.Debug("DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.ExtentsLayer = " + DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.ExtentsLayer);
                }

                ExportMapPdf(overViewExportPerimeter,
                    plotSection, DatashopWorkflowConfig.Instance.PlotFileName.PlotsOverview.TargetFile);
            }
        }

        private void ExportMapPdf(ExportPerimeter exportPerimeter, PlotSection plotSection, string targetFileNameTemplate)
        {
            CurrentExportPerimeter = exportPerimeter;
            currentPlotSectionEmtpy = false;

            DatashopWorkflowDataItem.Logger.InfoFormat("Processing plotFrame={0} and section={1}", exportPerimeter.MapExtent.Id,
                plotSection.Name);
            CurrentPlotSection = plotSection;
            FileName = GetFullQualifiedFileName(targetFileNameTemplate); ;

            DatashopWorkflowDataItem.Logger.InfoFormat("Checking if plotFrame is empty");
            CheckEmptyPlotFrame();

            DatashopWorkflowDataItem.Logger.InfoFormat("Checking if plotFrame is just background data");
            CheckBackgroundPlotFrame();

            DatashopWorkflowDataItem.Logger.InfoFormat("Creating plotFrame PDF file");
            CreateMapPdf();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Gets the plot sections.
        /// </summary>
        /// <returns>An IList of PlotSection</returns>
        private static IList<PlotSection> GetPlotSections()
        {
            PlotSectionStore sectionStore = new PlotSectionStore();
            IList<PlotSection> sections = sectionStore.GetAllSections();

            if (sections.Count == 0)
            {
                PlotSection defaultSection = new PlotSection { Name = "Default", Description = "Default", VisibleGroupLayers = string.Empty };

                sections.Add(defaultSection);
            }

            return sections;
        }

        /// <summary>
        /// Determines whether [is empty plot frame] [the specified export perimeter].
        /// </summary>
        /// <param name="exportPerimeter">The export perimeter.</param>
        /// <param name="plotSection">The plot section.</param>
        /// <returns>
        ///   <c>true</c> if [is empty plot frame] [the specified export perimeter]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmptyPlotFrame(ExportPerimeter exportPerimeter, PlotSection plotSection)
        {
            using (GNSPlotExtension plotExtension = new GNSPlotExtension())
            {
                string[] visibleGroupLayers = plotSection.VisibleGroupLayers.Split(';');
                bool isEmptyPlotFrame = plotExtension.IsPlotFrameEmpty(exportPerimeter, visibleGroupLayers);
                if (isEmptyPlotFrame)
                {
                    DatashopWorkflowDataItem.Logger.DebugFormat("Plot frame is empty for plotFrameId={0} plotSection={1}", exportPerimeter.MapExtent.Id, plotSection.Name);
                }

                return isEmptyPlotFrame;
            }
        }

        /// <summary>
        /// Checks the empty plot frame.
        /// </summary>
        private void CheckEmptyPlotFrame()
        {
            string emptyPlotFrameText = GetEmptyPlotFrameText();

            bool isEmptyPlotFrame = IsEmptyPlotFrame(CurrentExportPerimeter, CurrentPlotSection);
            this.CurrentEmptyPlotFrameText = isEmptyPlotFrame ? emptyPlotFrameText : string.Empty;

            this.currentPlotSectionEmtpy = isEmptyPlotFrame;
        }

        /// <summary>
        /// Determines whether the visible layer in the plot section, except the background, are empty in the passed perimeter.
        /// </summary>
        /// <param name="exportPerimeter">The export perimeter.</param>
        /// <param name="plotSection">The plot section.</param>
        /// <returns>
        ///   <c>true</c> if plot sections are empty and background not, otherwise, <c>false</c>.
        /// </returns>
        private bool VisibleLayersEmpty(ExportPerimeter exportPerimeter, PlotSection plotSection)
        {
            using (var plotExtension = new GNSPlotExtension())
            {
                var backgroundLayer = PlotExtension.Config.PlotExtensionConfig.Instance.Layers.Background;

                var visibleGroupLayers = plotSection.VisibleGroupLayers.Split(';');

                var backgroundLayers = visibleGroupLayers.Where(s => s == backgroundLayer).ToArray();

                var dataLayers = visibleGroupLayers.Where(s => s != backgroundLayer).ToArray();

                var isBackgroundEmpty = plotExtension.IsPlotFrameEmpty(exportPerimeter, backgroundLayers);

                var areLayersEmtpy = plotExtension.IsPlotFrameEmpty(exportPerimeter, dataLayers);

                //NOTE: background cannot be empty, but all other layers must be empty to determine if it's a background only plot
                var visibleLayersEmpty = !isBackgroundEmpty && areLayersEmtpy;

                if (visibleLayersEmpty)
                    DatashopWorkflowDataItem.Logger.DebugFormat("Plot frame is just background information for plotFrameId={0} plotSection={1}", exportPerimeter.MapExtent.Id, plotSection.Name);

                return visibleLayersEmpty;
            }
        }

        /// <summary>
        /// Checks the background plot frame.
        /// </summary>
        private void CheckBackgroundPlotFrame()
        {
            if (!this.currentPlotSectionEmtpy && !string.IsNullOrEmpty(CurrentPlotSection.VisibleGroupLayers))
            {
                var emptyBackgroundPlotText = this.GetEmptyBackgroundPlotText();

                var visibleLayersEmpty = VisibleLayersEmpty(CurrentExportPerimeter, CurrentPlotSection);
                this.CurrentBackgroundPlotFrameText = visibleLayersEmpty ? emptyBackgroundPlotText : string.Empty;
            }
            else
            { this.CurrentBackgroundPlotFrameText = string.Empty; }
        }


        /// <summary>
        /// Gets the background plot frame text.
        /// </summary>
        /// <returns>A string</returns>
        private string GetEmptyBackgroundPlotText()
        {
            using (GNSPlotExtension plotExtension = new GNSPlotExtension())
            {
                return plotExtension.GetBackgroundPlotFrameText();
            }
        }

        /// <summary>
        /// Gets the extents from job.
        /// </summary>
        /// <returns>A List of IGeometry</returns>
        private List<IGeometry> GetExtentsFromJob()
        {
            List<IGeometry> jobExtents = new List<IGeometry>();
            using (GNSPlotExtension plotExtension = new GNSPlotExtension())
            {
                ExportModel exportModel = DataItem.JobDescriptionModel as ExportModel;
                for (int perimeterIndex = 0; perimeterIndex < exportModel.Perimeters.Length; perimeterIndex++)
                {
                    MapExtent extent = exportModel.Perimeters[perimeterIndex].MapExtent;
                    jobExtents.Add(plotExtension.GetMapExtentFor(extent));
                }
            }

            return jobExtents;
        }

        /// <summary>
        /// Gets the empty plot frame text.
        /// </summary>
        /// <returns>The empty plot frame text</returns>
        private string GetEmptyPlotFrameText()
        {
            using (var plotExtension = new GNSPlotExtension())
            {
                return plotExtension.GetEmptyPlotFrameText();
            }
        }

        /// <summary>
        /// Creates the map PDF.
        /// </summary>
        private void CreateMapPdf()
        {
            using (var plotExtension = new GNSPlotExtension())
            {
                var visibleGroupLayers = CurrentPlotSection.VisibleGroupLayers.Split(';');

                try
                {
                    plotExtension.CreateExport("PDF", FileName, CurrentExportPerimeter.MapExtent, DataItem.Variables, visibleGroupLayers.ToList());
                }
                catch (LayoutExportException e)
                {
                    MailClient mailClient = new MailClient(DatashopWorkflowDataItem.Logger, DataItem.Variables);
                    mailClient.SendInvalidMapLayersMail(DataItem.JobId, e.InvalidLayersNames, e.MapFileMxdPath);
                    throw;
                }
            }
        }

        /// <summary>
        /// Inits the properties.
        /// </summary>
        /// <param name="numberOfPlotFrames">The number of plot frames.</param>
        /// <param name="numberOfSections">The number of sections.</param>
        private void InitProperties(int numberOfPlotFrames, int numberOfSections)
        {
            NumberOfPages = numberOfPlotFrames * numberOfSections;
            NumberOfSections = numberOfSections;
            CurrentPage = 1;
            CurrentEmptyPlotFrameText = GetEmptyPlotFrameText();
        }

        /// <summary>
        /// Gets the full name of the qualified file.
        /// </summary>
        /// <param name="targetFile"></param>
        /// <returns>The full qualified file name</returns>
        private string GetFullQualifiedFileName(string targetFile)
        {
            Assert.NotNull(DataItem.PlotFileName, "Missing or invalid configuration element 'plotfilename'");
            Assert.NotNullOrEmpty(targetFile, "Missing or invalid configuration element 'plotfilename'");

            string fileName = Utils.Utils.ReplaceVars(targetFile, DataItem.Variables);

            Letter.CheckFilename(fileName);

            return Path.Combine(Path.GetDirectoryName(DataItem.JobOutput), fileName);
        }

        #endregion
    }
}