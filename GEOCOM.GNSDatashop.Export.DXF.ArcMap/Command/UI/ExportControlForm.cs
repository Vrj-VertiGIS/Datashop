using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.Common.Licensing;
using GEOCOM.GNSDatashop.Export.DXF.Eventing;
using GEOCOM.GNSDatashop.Export.DXF.Interface;
using Microsoft.Win32;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using log4net;
using log4net.Config;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI
{
    [Guid(Guid)]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId(ProgId)]
    [ComVisible(true)]
    public partial class ExportControlForm : UserControl, IDockableWindowDef, IDockableWindowInitialPlacement
    {
        public const string ProgId = "Geocom.GNSDatashop.Export.ExportControlForm";
        public const string Guid = "2A1CCEF6-3641-4E9F-BFE4-7A2D4CF0C1F9";

        private static UID Uuid => new UIDClass { Value = string.Format("{{{0}}}", Guid) };

        private StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        private const string UserSettingsKey = @"HKEY_CURRENT_USER\Software\Geocom\DXF-Export for ArcMAP\";
        private const string UserSettingsControlsStateKey = UserSettingsKey + @"ControlsState\";

        private IApplication _application;
        private IMxDocument _mxDocument;

        private IDxfWriter _dxfWriter = null;

        private ESRIMap _esriMap = null;

        private LicenseHolder _licenseHolder = new LicenseHolder(LicenseHolder.Feature_DXF_Export, "5.00");
        private IActiveViewEvents_Event _pageLayoutEvents;

        private OutcomeLogger _logger = null;

        private static log4net.ILog _log = null;    // Used to configure log4net

        #region COM Registration Function(s)

        [ComRegisterFunction()]
        [ComVisible(false)]
        private static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        private static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code

        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Register(regKey);
        }

        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Unregister(regKey);
        }

        #endregion

        #endregion

        public ExportControlForm()
        {
            InitializeComponent();

            _lng.TranslateForm(this);

            SetTooltips();

            SettingsToControls();   // Initialize controls with last-session user input

            SetupExportButton();
        }

        void IDockableWindowDef.OnCreate(object hook)
        {
            _application = hook as IApplication;
            if (_application == null)
            {
                // Should only happen in case of an ArcGIS Engine application.
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (_application is IApplicationStatusEvents_Event statusEvents)
            {
                statusEvents.Initialized += ApplicationInitializedEventHandler;
            }

            if (null == _log)
                InitLoggingConfig();

            WireEvents();
        }

        void IDockableWindowDef.OnDestroy()
        {
            if (null != _application)
            {
                UnwireDocumentEvents();

                if (null != _esriMap)
                {
                    _esriMap.OnSelectionChanged -= SetControlsEnabledState;
                    _esriMap.OnLayersChanged -= OnLayersChangedEventHandler;
                }
            }
            _application = null;
        }

        int IDockableWindowDef.ChildHWND => Handle.ToInt32();

        string IDockableWindowDef.Caption => $"{_lng.LoadStr(10100, "VertiGIS DXF Export for ArcMap")}";

        string IDockableWindowDef.Name => ProgId;

        public object UserData => this;

        public esriDockFlags DockPosition => esriDockFlags.esriDockFloat;

        public UID Neighbor => null;

        #region dockable window event handling
        private void ApplicationInitializedEventHandler()
        {
            WireEvents();
            SetupForm();
        }

        private void WireEvents()
        {
            if (_application is IApplicationStatus applicationStatus && !applicationStatus.Initialized)
            {
                // Let's wait for the "application initialized" event
                return;
            }
            // Keep a reference to the "document". This way we make sure that its RCW is not thrown away by the garbage collector.
            _mxDocument = _application.Document as IMxDocument;
            if (_mxDocument == null)
            {
                return;
            }

            _esriMap = new ESRIMap(_application, FocusMap); // Bind map to focus map (implicit)

            _esriMap.OnSelectionChanged += SetControlsEnabledState;
            _esriMap.OnLayersChanged += OnLayersChangedEventHandler;

            if (_mxDocument is IDocumentEvents_Event documentEvents)
            {
                WirePageLayoutEvents();
                documentEvents.NewDocument += NewOrOpenDocumentEventHandler;
                documentEvents.OpenDocument += NewOrOpenDocumentEventHandler;
                documentEvents.MapsChanged += MapsChangedEventHandler;
                documentEvents.CloseDocument += OnCloseDocumentEventHandler;
            }
        }

        private void UnwireDocumentEvents()
        {
            try
            {
                if (_application.Document is IDocumentEvents_Event documentEvents)
                {
                    ((IActiveViewEvents_Event)_mxDocument.PageLayout).FocusMapChanged -= FocusMapChangedEventHandler;
                    documentEvents.NewDocument -= NewOrOpenDocumentEventHandler;
                    documentEvents.OpenDocument -= NewOrOpenDocumentEventHandler;
                    documentEvents.MapsChanged -= MapsChangedEventHandler;
                    documentEvents.CloseDocument -= OnCloseDocumentEventHandler;
                }
            }
            catch
            {
                // Ignore any error
            }
        }

        private void NewOrOpenDocumentEventHandler()
        {
            WirePageLayoutEvents();
            SetupForm();
        }

        public void OnCloseDocumentEventHandler()
        {
            UnwirePageLayoutEvents();
            SetupLayerBrowserCombos(new List<ILayer>());

            (_dxfWriter as IDisposable)?.Dispose();

            _dxfWriter = null;
        }

        private void WirePageLayoutEvents()
        {
            // Keep a reference to the "PageLayout". This way we make sure that its RCW is not thrown away by the garbage collector.
            _pageLayoutEvents = ((IActiveViewEvents_Event)_mxDocument.PageLayout);
            _pageLayoutEvents.FocusMapChanged += FocusMapChangedEventHandler;
            // Yes: FocusMap is a property of the document whereas FocusMapChanged is an event thrown by the PageLayout. Who makes such decisions?!? 
        }

        private void UnwirePageLayoutEvents()
        {
            try
            {
                _pageLayoutEvents.FocusMapChanged -= FocusMapChangedEventHandler;
            }
            catch
            {
                // Ignore any error
            }
        }

        private void FocusMapChangedEventHandler()
        {
            SetupForm();
        }

        public void MapsChangedEventHandler()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            _lng.TranslateForm(this);
            _esriMap.Map = FocusMap;
            SetUpDialogUiElements();
#if DEBUG
            SetOutputDxfName();
#endif
        }

        public void OnLayersChangedEventHandler()
        {
            SetUpDialogUiElements();
        }

        private void SetUpDialogUiElements()
        {
            PopulateDxfVersionCombo();
            SetupLayerBrowserCombos(_esriMap.Layers);
            SetControlsEnabledState();
        }

        #endregion

        #region Form preparation

        private void PopulateDxfVersionCombo()
        {
            if (0 >= cmbDxfVersion.Items.Count)
            // This is static content - so do it only once.
            {
                var supportedVersions = Enum.GetValues(typeof(DxfVersion))
                    .Cast<DxfVersion>()
                    .Select(t => new CmbDxfVersionItem(t))
                    .Where(t => t.IsSupported)
                    .ToArray();

                cmbDxfVersion.Items.Clear();
                cmbDxfVersion.Items.AddRange(supportedVersions.Cast<object>().ToArray());

                var defaultSelectedDxfVersion = (_dxfWriter is IDxfWriterOutputOptions options)
                    ? options.DxfVersion
                    : DxfWriter.DxfVersionDefault;

                var selectedDxfVersion = UserPresetDxfVersion(defaultSelectedDxfVersion);

                cmbDxfVersion.SelectedItem = supportedVersions.FirstOrDefault(t => t.DxfVersion.Equals(selectedDxfVersion));
            }
        }

        private void SetupExportButton()
        {
            btnExport.FireCaption = _lng.LoadStr(10220, "E&xportieren");
            btnExport.CancelCaption = _lng.LoadStr(10222, "A&bbrechen");
        }

        private void SetTooltips()
        {
            toolTips.SetToolTip(cmbDxfVersion, _lng.LoadStr(10120, "Select the version of the AutoCad DXF format specification the exported file should adhere to.In doubt, leave the default."));
            toolTips.SetToolTip(cmbSelectionLayer, _lng.LoadStr(10214, @"This layer allows you to select the features you want to export. Only features within a polyon of the layer specified heere will be exported"));
            toolTips.SetToolTip(cmbMaskingLayer, _lng.LoadStr(10216, @"This layer allows you to mask out certain features. Features ""covered"" by a polygon of this layer will not be exported or will only partially be exported"));
            toolTips.SetToolTip(btnExport, _lng.LoadStr(10224, @"start export or abort running export. a running export can also be cancelled by hitting the keyboard's ESC-Key or by clicking the map view."));
            toolTips.SetToolTip(txtOutfileSspec, _lng.LoadStr(10226, @"Name of export file. A running 3 digit number will be appended if you specify a selection layer. When typed-in manually, no file extension (i.e. .DXF) will be automatically appended."));
            toolTips.SetToolTip(btnBrowseOutputFile, _lng.LoadStr(10228, @"Browse for a suitable output file (-location)."));
            toolTips.SetToolTip(cbBinary, _lng.LoadStr(10230, @"Write binary DXF. Approx. 25% less storage consumption, faster loading in AutoCAD (up to 5 times). Check whether your client application supports binary DXF prior to first use."));
        }

        private void SetControlsEnabledState()
        {
            if (_esriMap == null)
                return;

            _licenseHolder.CheckAvailability();

            var documentLoaded = (_esriMap.Layers.Any() /* || _esriMap.DrawingElements.Any() - has too many impacts if no layers - leave for now */)
                && ((_licenseHolder.Feature.Status == License.LicenseStatus.available) || (_licenseHolder.Feature.Status == License.LicenseStatus.checkedOut));

            cmbDxfVersion.Enabled = documentLoaded;
            cbBinary.Enabled = documentLoaded;
            txtOutfileSspec.Enabled = documentLoaded;
            btnBrowseOutputFile.Enabled = documentLoaded;

            cbRestrictToVisibleLayers.Enabled = documentLoaded;
            cbRestrictToScreenExtent.Enabled = documentLoaded;
            cbRestrictToSelection.Enabled = documentLoaded && _esriMap.SelectedLayers.Any();

            cmbMaskingLayer.Enabled = documentLoaded;
            cmbSelectionLayer.Enabled = documentLoaded;

            btnExport.Enabled = documentLoaded && ValidOutputFileSpec();
        }

        private void SetupWriter()
        {
            var display = Display;
            var focusMap = _esriMap.Map;
            if ((null != focusMap) && (0 < focusMap.LayerCount))
            {
                // Create fesh one.
                _dxfWriter = new DxfWriter(display, focusMap);

                // Output options
                if (_dxfWriter is IDxfWriterOutputOptions dxfO)
                {
                    // CancelTracker and StepProgressor: We don't link the cancel tracker directly to the step progressor
                    // as this may lead to unpredictable progressbar positioning - in our current situation.
                    dxfO.StepProgressor = StepProgressorDummy.FromApplication(_application);
                    dxfO.CancelTracker = new CancelTrackerDummy(new CancelTrackerClass() { CancelOnClick = true, CancelOnKeyPress = true });
                    dxfO.DxfVersion = GetDxfVersionFromUI();
                }

                // Events
                if (_dxfWriter is IDxfWriterEvents dxfE)
                {
                    dxfE.OnStart += dxfexport_Started;
                    dxfE.OnBeforeAbort += dxfExport_RequestCancel;
                    dxfE.OnAbort += dxfExport_Cancelled;
                    dxfE.OnNothingDone += dxfExport_NothingDone;
                    dxfE.OnUnhandledException += dxfExport_UnhandledException;
                    dxfE.OnSuccess += dxfexport_Succeeded;
                }

                // Options
                if (_dxfWriter is IDxfWriterOptions dwo)
                {
                    dwo.UseSelectionSet = cbRestrictToSelection.Checked;
                    dwo.ReferenceScale = (0 >= focusMap.ReferenceScale) ? focusMap.MapScale : focusMap.ReferenceScale;
                    dwo.CurrentScale = focusMap.MapScale;
                    dwo.BinaryDXF = cbBinary.Checked;
                }

                if (_dxfWriter is IDxfWriterRunMode drmo)
                    drmo.RunMode = DxfWriterRunMode.Interactive;
            }
        }

        private void DisposeWriter()
        {
            // Force disposing of the writer instance previously in use
            (_dxfWriter as IDisposable)?.Dispose();

            _dxfWriter = null;

            SetupWriter();

            DotNetFrameworkSupport.CollectGarbage();
        }

        private DxfVersion GetDxfVersionFromUI()
        {
            var selected = cmbDxfVersion.SelectedIndex;
            return (selected >= 0)
                ? ((CmbDxfVersionItem)cmbDxfVersion.Items[selected]).DxfVersion
                : DxfVersion.AutoCad2010;
        }

        private void SetupLayerBrowserCombos(IList<ILayer> layers)
        {
            var layerBrowserGeometryTypes = new List<esriGeometryType>() { esriGeometryType.esriGeometryPolygon };
            var layerBrowserFeatureTypes = new List<esriFeatureType>() { esriFeatureType.esriFTSimple };

            cmbMaskingLayer.SetSupportedGeometryTypes(layerBrowserGeometryTypes);
            cmbMaskingLayer.SetSupportedFeatureTypes(layerBrowserFeatureTypes);
            cmbMaskingLayer.NoLayerEntry = true;
            cmbMaskingLayer.Set_Layers(layers);
            cmbMaskingLayer.FromRegistry(UserSettingsControlsStateKey, @"MaskingLayer");

            cmbSelectionLayer.SetSupportedGeometryTypes(layerBrowserGeometryTypes);
            cmbSelectionLayer.SetSupportedFeatureTypes(layerBrowserFeatureTypes);
            cmbSelectionLayer.NoLayerEntry = true;
            cmbSelectionLayer.Set_Layers(layers);
            cmbSelectionLayer.FromRegistry(UserSettingsControlsStateKey, @"SelectionLayer");
        }

        private void btnBrowseOutputFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                ofd.DefaultExt = ".DXF";
                ofd.CheckFileExists = false;
                ofd.CheckPathExists = true;
                ofd.Filter = _lng.LoadStr(10510, "DXF Autocad Files (*.DXF)|*.DXF|All Files (*.*)|*.*");
                ofd.FilterIndex = 2;
                ofd.OverwritePrompt = false;
                ofd.Title = _lng.LoadStr(10500, "Write DXF from currently displayed map (visible layers only)");
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtOutfileSspec.Text = ofd.FileName;

                SetControlsEnabledState();
            }
        }

        private bool ValidOutputFileSpec()
        {
            return !string.IsNullOrEmpty(txtOutfileSspec.Text); // for now ... TODO: Check validity of spec
        }

        private bool ValidExportSpec()
        {
            var sl = cmbSelectionLayer.SelectedLayer;
            var ml = cmbMaskingLayer.SelectedLayer;
            if ((null != sl) && (null != ml) && (ml.Equals(sl)))
            {
                cmbSelectionLayer.Focus();
                TaskDialog.Show(Control.FromHandle(Handle),
                    _lng.LoadStr(10900, "Fehler"),
                    Product.Name,
                    _lng.LoadStr(10902, "Ein Layer kann nicht Selektions- und Maskierungslayer gleichzeitig sein"),
                    TaskDialog.Buttons.OK, TaskDialog.Icon.Error);
                return false;
            }
            return true;
        }

        private bool CheckOutLicense()
        {
            return (License.LicenseStatus.checkedOut.Equals(_licenseHolder.CheckOut()));
        }

        private void ReturnLicense()
        {
            _licenseHolder?.Return();
        }

        private void RunExport()
        {
            SetupWriter();

            try
            {
                RunExportCore();
            }
            finally
            {
                DisposeWriter();
            }
        }

        private void RunExportCore()
        {
            var maskedOutRegions = ExportSelection.GetMasks(cmbMaskingLayer.SelectedLayer);
            var regionsOfInterest = ExportSelection.GetROI(cmbSelectionLayer.SelectedLayer, _esriMap.Map, cbRestrictToScreenExtent.Checked).ToList();

            var fileSpec = txtOutfileSspec.Text;

            if (!regionsOfInterest.Any())
                _dxfWriter.WriteSingleDXF(fileSpec, cbRestrictToVisibleLayers.Checked, null, maskedOutRegions);
            else if (2 > regionsOfInterest.Count)
                _dxfWriter.WriteSingleDXF(fileSpec, cbRestrictToVisibleLayers.Checked, regionsOfInterest.ElementAt(0), maskedOutRegions);
            else
                _dxfWriter.WriteDXFByTemplate(fileSpec, cbRestrictToVisibleLayers.Checked, regionsOfInterest, maskedOutRegions);
        }

        private void RunExportWithUIFeedback()
        {
            var pointer = new MouseCursor();
            pointer.SetCursor(2);
            btnExport.OperationMode = FireButton.Mode.Cancel;
            try
            {
                try
                {
                    RunExport();
                }
                catch (OutOfMemoryException)    // Possible in 32-bit environment
                {
                }
            }
            finally
            {
                pointer.SetCursor(0);
                btnExport.OperationMode = FireButton.Mode.Fire;
            }
        }

        #endregion

        #region logging

        private OutcomeLogger StartLogging()
        {
            _logger = new OutcomeLogger(_lng);
            _logger.LogStart();
            return _logger;
        }

        private void StopLogging()
        {
            _logger.LogEnd();
            _logger = null;
        }

        private void InitLoggingConfig()
        {
            var geonisPresent = null != _application.FindExtensionByCLSID(new UID() { Value = "{C28F7F9D-B16C-40B4-BA98-CF4940F2FB38}" });
            if (!geonisPresent)
            {
                var assemblyURI = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                var assemblyPath = System.IO.Path.GetFullPath(Uri.UnescapeDataString(assemblyURI));

                var configPath = assemblyPath + ".Config";

                if (File.Exists(configPath))
                    XmlConfigurator.Configure(new FileInfo(configPath));
            }
            _log = LogManager.GetLogger("DxfWriter");
        }

        #endregion

        #region instance control

        public static IDockableWindow GetInstance(IApplication arcMap)
        {
            var dw = (arcMap is IDockableWindowManager dwm)
                ? dwm.GetDockableWindow(Uuid)
                : null;
            return dw;
        }

        #endregion

        #region arcobjects related helpers

        private IDisplay Display => (_application as IMxApplication)?.Display;

        private IMap FocusMap => (_application?.Document as IMxDocument)?.FocusMap;

        #endregion

        #region form components/controls event handlers

        private void btnExport_Click(object sender, EventArgs e)
        {
            ControlsToSettings();   // Save user control contents for a next session

            if (ValidExportSpec())
                if (CheckOutLicense())
                {
                    try
                    {
                        RunExportWithUIFeedback();
                    }
                    finally
                    {
                        ReturnLicense();
                    }
                }
                else
                    licenseHolder_InvalidLicense(this, new LicenseStatusChangedEventArgs(_licenseHolder.Feature));
        }

        private void txtOutfileSspec_TextChanged(object sender, EventArgs e)
        {
            SetControlsEnabledState();
        }

        private void btnExport_CancelClick(object sender, EventArgs e)
        {
            var dwo = _dxfWriter as IDxfWriterOutputOptions;

            if ((null != dwo) && (null != dwo.CancelTracker))
                dwo.CancelTracker.Cancel();
            btnExport.OperationMode = FireButton.Mode.Fire;
        }

        private void txtOutfileSspec_Leave(object sender, EventArgs e)
        {
            SetDefaultFileExtension(txtOutfileSspec);
        }

        private void ExportControlForm_Leave(object sender, EventArgs e)
        {
            txtOutfileSspec_Leave(sender, e);
        }

        private void SetDefaultFileExtension(TextBox txtFileName)
        {
            var fileName = txtFileName.Text;
            if ((!string.IsNullOrEmpty(fileName)) && (!string.IsNullOrWhiteSpace(fileName)))
            {
                var ext = System.IO.Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(ext))
                    txtFileName.Text = fileName + @".dxf";
            }
        }

        private void cmbDxfVersion_Enter(object sender, EventArgs e)
        {
            if (_application is IApplicationStatus status && !status.Initialized)
                return;

            // This is for Issue 23503 "Auswahlliste DXF-Version ist leer nach Erst-Installation".
            // The event handling was reworked, so this shouldn't happen anymore. But to be on the
            // save side we double-check here.

            if (0 >= cmbDxfVersion.Items.Count)
            {
                WireEvents();
                SetupForm();
            }
        }

        #endregion

        #region Dxf exporter event handlers

        private void dxfexport_Started(object sender, DxfWriterStartEventEventArgs e)
        {
            StartLogging();

            var stp = DxfWriterProgressor;

            if (null != stp)
            {
                stp.Position = stp.MinRange;
                stp.Show();
            }
        }

        private void dxfexport_Succeeded(object sender, DxfWriterSuccessEventEventArgs e)
        {
            var nFiles = e.FilesWritten?.Count() ?? 0;
            var nLayers = e.ErroneousLayers?.Count ?? 0;


            if (0 < nLayers)      // info to logfile
                _logger.LogIrregularities(null, e.FilesWritten, e.ErroneousLayers, e.ExportedLayers);
            else
                _logger.LogOutcome(e.FilesWritten, e.ExportedLayers);
            StopLogging();

            var info = new List<string>();
            if (0 == nFiles)
                info.Add(_lng.FmtLoadStr(10412, "Nothing exported - no (visible) layers present or no data in the relevant extent."));
            else if (1 == nFiles)
                info.Add(_lng.FmtLoadStr(10404, "AutoCad DXF data written to: {0}", ToMarkup(e.FilesWritten.SingleOrDefault())));
            else
                info.Add($"{_lng.FmtLoadStr(10405, "Autocad DXF Data written to ({0} files):", nFiles)}\n{LimitedInfoAboutFilesWritten(e.FilesWritten)}");

            if (0 < (e.ErroneousLayers?.Count ?? 0))
            {
                info.AddRange(InfoAboutErroneousLayers(e.ErroneousLayers));
                info.AddRange(InfoAboutLogfiles());
            }

            var icon = (0 == nLayers) ? TaskDialog.Icon.Information : TaskDialog.Icon.Warning;

            TaskDialog.Show(Control.FromHandle(Handle),
                _lng.LoadStr(10402, "Exporting to AutoCad dxf"),
                Product.Name,
                Paragraphs(2, (IEnumerable<string>)info, false),
                (1 < nFiles) ? Paragraphs(1, e.FilesWritten, true) : string.Empty,
                _lng.LoadStr(10420, "Show list of all output files"),
                _lng.LoadStr(10421, "Hide list"),
                TaskDialog.Buttons.OK, icon); ;

            DxfWriterProgressor?.Hide();
        }

        private void dxfExport_RequestCancel(object sender, DxfWriterBeforeAbortEventEventArgs e)
        {
            var reply = (MessageBox.Show(_lng.FmtLoadStr(10408, "Really abort current dxf export?", txtOutfileSspec.Text),
                _lng.LoadStr(10410, "Abort exporting to AutoCad dxf..."), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2));
            e.CancelAbort = (reply == DialogResult.No);
        }

        private void dxfExport_Cancelled(object sender, DxfWriterAbortEventEventArgs e)
        {
            var cancelInfo = _lng.LoadStr(10406, "Export to AutoCAD DXF aborted.");

            _logger.LogIrregularities(null, e.FilesWritten, e.ErroneousLayers, e.ExportedLayers, cancelInfo);
            StopLogging();

            var info = new List<string>();
            info.Add(cancelInfo);
            info.AddRange(InfoAboutFilesWrittenSoFar(e.FilesWritten));

            if (0 < (e.ErroneousLayers?.Count ?? 0))
            {
                info.AddRange(InfoAboutErroneousLayers(e.ErroneousLayers));
                info.AddRange(InfoAboutLogfiles());
            }

            TaskDialog.Show(Control.FromHandle(Handle),
                _lng.LoadStr(10410, "Abort exporting to AutoCad dxf..."),
                Product.Name,
                Paragraphs(2, info),
                (1 < e.FilesWritten.Count()) ? Paragraphs(1, e.FilesWritten, true) : string.Empty,    // Only if multiple files
                _lng.LoadStr(10420, "Show list of all output files"),
                _lng.LoadStr(10421, "Hide list"),
                TaskDialog.Buttons.OK, TaskDialog.Icon.Warning);

            DxfWriterProgressor?.Hide();
        }

        private void dxfExport_NothingDone(object sender, EventArgs e)
        {
            (_logger ?? StartLogging()).LogOutcome(null, null);   // At present - in rare cases (no visible layers due to layer visibility change) it might occur...
            StopLogging();

            MessageBox.Show(_lng.LoadStr(10412, "No data written - no (visible) layers to export data from)"),
                _lng.LoadStr(10410, "Abort exporting to AutoCad dxf..."), MessageBoxButtons.OK, MessageBoxIcon.Hand);
            DxfWriterProgressor?.Hide();
        }

        private IEnumerable<string> InfoAboutFilesWrittenSoFar(IEnumerable<string> filesWritten)
        {
            var nFiles = filesWritten.Count();
            yield return 
                (0 == nFiles)
                    ? _lng.FmtLoadStr(10412, "Nothing exported - no (visible) layers present or no data in the relevant extent.")
                    : (1 == nFiles)
                        ? $"{_lng.FmtLoadStr(10407, "{0} file(s) written so far:", nFiles)}\n{ToMarkup(filesWritten.SingleOrDefault())}"
                        : $"{_lng.FmtLoadStr(10407, "{0} file(s) written so far:", nFiles)}\n{LimitedInfoAboutFilesWritten(filesWritten)}";
         }

        private string LimitedInfoAboutFilesWritten(IEnumerable<string> filesWritten)
        {
            var info = filesWritten.Take(4).NullAwareAggregate((line, name) => line + "\n\t" + System.IO.Path.GetFileName(name));

            var restOfFiles = filesWritten.Skip(4).ToList();

            if (restOfFiles.Any())
                info += "\n\t...\n\t" + System.IO.Path.GetFileName(restOfFiles.Last());

            return info;
        }

        private IEnumerable<string> InfoAboutErroneousLayers(ErroneousLayersInfo layers)
        {
            if (0 < (layers?.Count ?? 0))
            {
                var singleLayer = (1 == layers.Count());

                var layerText = (singleLayer)
                    ? _lng.LoadStr(10910, "Unable to export listed layer")
                    : _lng.LoadStr(10911, "Unable to export listed layers");
                yield return $"{layerText}\n{layers.ToString(false, true)}";
            }
        }

        private IEnumerable<string> InfoAboutLogfiles()
        {
            var appenders = log4net.LogManager.GetAllRepositories().SelectMany(l => l.GetAppenders().Where(g => g is log4net.Appender.RollingFileAppender));
            var logFiles = string.Join("\n", appenders.Distinct().Select(a => a as log4net.Appender.RollingFileAppender).Select(a => ToMarkup(a.File)));
            if (!string.IsNullOrEmpty(logFiles))
                yield return $"{_lng.LoadStr(40920, "Please find more precise info in:")}\n{logFiles}";
        }

        private IEnumerable<string> ToMarkup(IEnumerable<string> lines)
            => lines.Select(l => ToMarkup(l));
        private string ToMarkup(string fullFilePath)
            => $"<a href=\"file://{ToURL(fullFilePath)}\">{fullFilePath}</a>";
        private string ToURL(string fullFilePath)
            => fullFilePath.Replace('\\', '/');

        private void dxfExport_UnhandledException(object sender, DxfWriterUnhandledExceptionEventArgs e)
        {

            if (e.Exception is OutOfMemoryException)
                HandleOutOfMemoryException(e);
            else
                HandleUnknownException(e);

            StopLogging();

            DxfWriterProgressor?.Hide();
        }

        private void HandleUnknownException(DxfWriterUnhandledExceptionEventArgs e)
        {
            Exception ex = e?.Exception ?? new Exception("Unknown Exception");

            _logger.LogIrregularities(ex, e?.FilesWritten, null, null);

            var nFiles = e.FilesWritten.Count();

            var unexpectedErrorParagraph = $"{_lng.LoadStr(10904, "Unexpected error.")}\n{ex.GetType().Name}: {ex.Message}";
            var filesWrittenParagraph = Paragraphs(2, InfoAboutFilesWrittenSoFar(e.FilesWritten));
            var additionalInfo = (1 < nFiles)
                ? Paragraphs(1, e.FilesWritten, true)
                : DetailedTechInfoParagraph(e.Exception);
            var expandCaption = (1 < nFiles)
                ? _lng.LoadStr(10420, "Show list of all output files")
                : _lng.LoadStr(10905, "Detailed technical info (to be forwarted to support)");
            var collapseCaption = (1 < nFiles)
                ? _lng.LoadStr(10421, "Hide list")
                : _lng.LoadStr(10906, "Hide information");


            TaskDialog.Show(Control.FromHandle(Handle),
                _lng.LoadStr(10410, "Abort exporting to AutoCad dxf..."),
                Product.Name,
                Paragraphs(2, unexpectedErrorParagraph, filesWrittenParagraph),
                additionalInfo,
                expandCaption,
                collapseCaption,
                TaskDialog.Buttons.OK, TaskDialog.Icon.Error);
        }

        private void HandleOutOfMemoryException(DxfWriterUnhandledExceptionEventArgs e)
        {
            var filesWrittenParagraph = Paragraphs(2, InfoAboutFilesWrittenSoFar(e.FilesWritten));
            var dataSetTooBigText = _lng.LoadStr(10903, "Dataset to be written is too big. Select less features, Limit # of Layers to export and use a selection layer.");

            _logger.LogIrregularities(null, e.FilesWritten, null, null, dataSetTooBigText);

            TaskDialog.Show(Control.FromHandle(Handle),
                _lng.LoadStr(10410, "Abort exporting to AutoCad dxf..."),
                Product.Name,
                Paragraphs(2, dataSetTooBigText, filesWrittenParagraph),
                (1 < e.FilesWritten.Count()) ? Paragraphs(1, e.FilesWritten, true) : string.Empty,    // Only if multiple files
                _lng.LoadStr(10420, "Show list of all output files"),
                _lng.LoadStr(10421, "Hide list"),
                TaskDialog.Buttons.OK, TaskDialog.Icon.Error);
        }

        private string DetailedTechInfoParagraph(Exception innerEx)
            => $"{_lng.LoadStr(10905, "Detailed technical info (to be forwarted to support):")}\n{innerEx}";

        private void licenseHolder_InvalidLicense(object sender, LicenseStatusChangedEventArgs e)
        {
            MessageBox.Show(_lng.LoadStr(10702, "No license to run Geocom Datashop AutoCad DXF Export"),
                _lng.LoadStr(10700, "Geocom Datashop AutoCad DXF Export licensing"),
                MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private string Paragraphs(int nNewLines, params object[] paragraph)
            => string.Join(new String('\n', nNewLines), paragraph);
        private string Paragraphs(int nNewLines, IEnumerable<string> paragraphs, bool asMarkup = false)
            => string.Join(new string('\n', nNewLines), (asMarkup) ? paragraphs.Select(p => ToMarkup(p)) : paragraphs);

        /// <summary>
        /// The ArcMap step progressor to signal progression of export
        /// </summary>
        private IStepProgressor DxfWriterProgressor
            => (_dxfWriter as IDxfWriterOutputOptions)?.StepProgressor;

        #endregion

        #region form user settings loading/saving

        private void SettingsToControls()
        {
            cbBinary.Checked = (0 != (int)GetRegistryValue(UserSettingsControlsStateKey, @"BinaryDXF", (object)0));
            cbRestrictToVisibleLayers.Checked = (0 != (int)GetRegistryValue(UserSettingsControlsStateKey, @"VisibleOnlyLayers", (object)1));
            cbRestrictToScreenExtent.Checked = (0 != (int)GetRegistryValue(UserSettingsControlsStateKey, @"LimitToScreenExtent", (object)1));
            cbRestrictToSelection.Checked = (0 != (int)GetRegistryValue(UserSettingsControlsStateKey, @"SelectedFeaturesOnly", (object)1));
            // Note: cmbMaskingLayer and cmbSelectionLayer settings cannot be read here as we will not necessarily have
            // valid combobox contents that allows setting of a current selected item. The selected item of this controls
            // will be set upon layer change in SetupLayerBrowserCombos()
        }

        private void ControlsToSettings()
        {
            if (cmbDxfVersion.SelectedItem is CmbDxfVersionItem vi)
                Registry.SetValue(UserSettingsControlsStateKey, @"DXFVersion", (object)(vi.DxfVersion), RegistryValueKind.DWord);
            Registry.SetValue(UserSettingsControlsStateKey, @"BinaryDXF", (object)cbBinary.Checked, RegistryValueKind.DWord);
            Registry.SetValue(UserSettingsControlsStateKey, @"VisibleOnlyLayers", (object)cbRestrictToVisibleLayers.Checked, RegistryValueKind.DWord);
            Registry.SetValue(UserSettingsControlsStateKey, @"LimitToScreenExtent", (object)cbRestrictToScreenExtent.Checked, RegistryValueKind.DWord);
            Registry.SetValue(UserSettingsControlsStateKey, @"SelectedFeaturesOnly", (object)cbRestrictToSelection.Checked, RegistryValueKind.DWord);

            cmbMaskingLayer.ToRegistry(UserSettingsControlsStateKey, @"MaskingLayer");
            cmbSelectionLayer.ToRegistry(UserSettingsControlsStateKey, @"SelectionLayer");
        }
        private DxfVersion UserPresetDxfVersion(DxfVersion defaultPreset)
        {
            return (DxfVersion)GetRegistryValue(UserSettingsControlsStateKey, @"DXFVersion", (object)defaultPreset);
        }

        private object GetRegistryValue(string keyName, string valueName, object defaultValue)
        {
            var value = Registry.GetValue(keyName, valueName, defaultValue);
            return (null != value)
                ? value
                : defaultValue;
        }

        #endregion

#if DEBUG
        #region development facilitation
        private void SetOutputDxfName()
        {
            txtOutfileSspec.Text = System.IO.Path.Combine(@"C:\temp\", $"{_application.Document.Title}.DXF");
            txtOutfileSspec_TextChanged(this, new EventArgs());
        }
        #endregion

        #region termination message box / task dialog tests

        /// <summary>
        /// These have been placed heere as the context (the classes / objects) required to
        /// run them are in comfortable reach heere
        /// </summary>
        void _DialogTests()
        {
            var fileList0 = new List<string>();
            var fileList1 = new List<string>() { "C:\\temp\\gaga.dxf" };
            var fileListn = new List<string>();
            fileListn.AddRange(System.IO.Directory.EnumerateFiles("C:\\temp\\", "*.*"));

            var errorLayers0 = new ProcessedLayersInfo();
            var errorLayers1 = new ProcessedLayersInfo();
            errorLayers1.ErroneousLayers.Add(new ErroneousLayerInfo(_esriMap.Layers[0]));
            var errorLayersn = new ProcessedLayersInfo();
            errorLayersn.ErroneousLayers.AddRange(_esriMap.Layers.Select(l => new ErroneousLayerInfo(l)));

            DxfWriterSuccessEventEventArgs sa;
            sa = new DxfWriterSuccessEventEventArgs(fileList0, errorLayers0);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileList1, errorLayers0);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileListn, errorLayers0);
            dxfexport_Succeeded(this, sa);

            sa = new DxfWriterSuccessEventEventArgs(fileList0, errorLayers1);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileList1, errorLayers1);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileListn, errorLayers1);
            dxfexport_Succeeded(this, sa);

            sa = new DxfWriterSuccessEventEventArgs(fileList0, errorLayersn);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileList1, errorLayersn);
            dxfexport_Succeeded(this, sa);
            sa = new DxfWriterSuccessEventEventArgs(fileListn, errorLayersn);
            dxfexport_Succeeded(this, sa);

            DxfWriterAbortEventEventArgs ca;
            ca = new DxfWriterAbortEventEventArgs(fileList0, errorLayers0);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileList1, errorLayers0);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileListn, errorLayers0);
            dxfExport_Cancelled(this, ca);

            ca = new DxfWriterAbortEventEventArgs(fileList0, errorLayers1);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileList1, errorLayers1);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileListn, errorLayers1);
            dxfExport_Cancelled(this, ca);

            ca = new DxfWriterAbortEventEventArgs(fileList0, errorLayersn);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileList1, errorLayersn);
            dxfExport_Cancelled(this, ca);
            ca = new DxfWriterAbortEventEventArgs(fileListn, errorLayersn);
            dxfExport_Cancelled(this, ca);

            DxfWriterUnhandledExceptionEventArgs ua;
            var ome = new OutOfMemoryException("Testing out-of-memory messages");
            ua = new DxfWriterUnhandledExceptionEventArgs(ome, fileList0);
            dxfExport_UnhandledException(this, ua);
            ua = new DxfWriterUnhandledExceptionEventArgs(ome, fileList1);
            dxfExport_UnhandledException(this, ua);
            ua = new DxfWriterUnhandledExceptionEventArgs(ome, fileListn);
            dxfExport_UnhandledException(this, ua);

            var are = new ArgumentException("Some unhandled exception to test the messages",
                new InvalidOperationException("InnerExpection1",
                    new InvalidOleVariantTypeException("Some inner exception 2",
                        new Exception("Innermoust exception")
                        )
                    )
                );
            ua = new DxfWriterUnhandledExceptionEventArgs(are, fileList0);
            dxfExport_UnhandledException(this, ua);
            ua = new DxfWriterUnhandledExceptionEventArgs(are, fileList1);
            dxfExport_UnhandledException(this, ua);
            ua = new DxfWriterUnhandledExceptionEventArgs(are, fileListn);
            dxfExport_UnhandledException(this, ua);

            TaskDialog.Show(Control.FromHandle(Handle), "Tests are over", "Tests");
        }
        #endregion
#endif
    }
}
