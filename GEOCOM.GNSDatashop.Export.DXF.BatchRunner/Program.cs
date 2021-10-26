using CommandLine;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using GEOCOM.GNSDatashop.Export.DXF.BatchRunner.CmdLine;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Eventing;
using GEOCOM.GNSDatashop.Export.DXF.Interface;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GEOCOM.GNSDatashop.Export.DXF.BatchRunner
{
    class Program
    {
        private enum exitCodeEnum
        {
            success = 0,
            illegalCommandLine = 10,
            invalidDxfFileSpec = 12,
            InvalidDxfFileFormat = 13,
            invalidMxdFileSpec = 14,
            invalidLogFileSpec = 15,
            invalidSelectionLayerSpec = 16,
            invalidMaskingLayerSpec = 18,
            invalidMapScale = 20,
            noArcGISApplication = 30,
            noArcGISLicense = 32,
            errorSetupDxfWriter = 80,
            unknownError = 99
        };

        private static exitCodeEnum _errorLevel = exitCodeEnum.success;

        private static AoInitialize _aoInit = null;

        private static IDxfWriter _dxfWriter = null;

        private static StoLanguage _lng = new StoLanguage() { AppName = Product.TechnicalAppname };

        private static readonly log4net.ILog _log = LogManager.GetLogger("DxfWriter");

        private static OutcomeLogger _logger = null;

        [STAThread()]
        static void Main(string[] args)
        {
            var namex = AssemblyFileName;

            InitLoggingPrimary();

            Parser.Default.ParseArguments<CmdLineOptions>(args)
                .WithParsed(opts => Run(opts))
                .WithNotParsed(errors => HandleParserErrors(errors));

            Environment.Exit((int)_errorLevel);
        }

        private static void Run(CmdLineOptions cmdLine)
        {
            {
                // Initialize the license once at the beginning of job execution
                if (ParamOk(ValidateAndInitArcGIS()))
                try
                {
                    ValidateAndRunExport(cmdLine);
                }
                finally
                {
                    DetachFromArcGIS();
                }
            }
        }

        public static void HandleParserErrors(IEnumerable<Error> errors)
        {
            Environment.Exit((int)exitCodeEnum.illegalCommandLine);
        }

        #region Export processing

        private static void ValidateAndRunExport(CmdLineOptions cmdLineOptions)
        {

            try
            {
                if (ParamOk(ValidateAndInitLogging(cmdLineOptions.Log))
                    && ParamOk(ValidateDxfFileSpec(cmdLineOptions.Dxf))
                    && ParamOk(ValidateMXD(cmdLineOptions.Mxd, out IMapDocument mapDoc))
                    && ParamOk(ValidateDxfVersion(cmdLineOptions.DxfVerion))
                    && ParamOk(ValidateSelectionLayers(mapDoc, cmdLineOptions.SelectionLayer, cmdLineOptions.MaskingLayer,
                           out ILayer selectionLayer, out ILayer maskingLayer))
                    && ParamOk(ValidateMapScale(mapDoc, cmdLineOptions.MapScale, out double mapScale)))
                {
                    var exportedMap = mapDoc.Map[0];

                    exportedMap.MapScale = mapScale;

                    var activeDisplay = mapDoc?.ActiveView?.ScreenDisplay;

                    SetupWriter(activeDisplay, exportedMap, cmdLineOptions.DxfVerion, cmdLineOptions.Binary.HasValue ? cmdLineOptions.Binary.Value : false, out _dxfWriter);

                    RunExport(cmdLineOptions.Dxf, selectionLayer, maskingLayer, cmdLineOptions.RestricToVisibleLayers.HasValue ? cmdLineOptions.RestricToVisibleLayers.Value : false);

                    mapDoc.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorOut(exitCodeEnum.unknownError, 20299, "Unknown error\n{0}", ex.ToString());
            }
        }

        private static bool ParamOk(exitCodeEnum paramState)
            => exitCodeEnum.success == (_errorLevel = paramState);

        private static void RunExport(string dxfFilePath, ILayer selectionLayer, ILayer maskingLayer, bool restrictToVisible)
        {
            var maskedOutRegions = ExportSelection.GetMasks(maskingLayer);
            var regionsOfInterest = ExportSelection.GetROI(selectionLayer, null, false);

            var fileSpec = dxfFilePath;

            if ((null == regionsOfInterest) || (!regionsOfInterest.Any()))
                _dxfWriter.WriteSingleDXF(fileSpec, restrictToVisible, null, maskedOutRegions);
            else if (2 > regionsOfInterest.Count())
                _dxfWriter.WriteSingleDXF(fileSpec, restrictToVisible, regionsOfInterest.ElementAt(0), maskedOutRegions);
            else
                _dxfWriter.WriteDXFByTemplate(fileSpec, restrictToVisible, regionsOfInterest, maskedOutRegions);
        }

        #endregion

        #region semantic command line argument checking
        private static exitCodeEnum ValidateMXD(string mxdFileSpec, out IMapDocument mapDocument)
        {
            mapDocument = null;

            if ((string.IsNullOrEmpty(mxdFileSpec)
                || (!File.Exists(mxdFileSpec))))
                ErrorOut(20214, "No or non-existent map file (.mxd) specified: {0). Please check path.", (string.IsNullOrEmpty(mxdFileSpec)) ? "<NULL>" : mxdFileSpec);
            else
            {
                var myMapDoc = new MapDocumentClass();

                if ((!myMapDoc.IsPresent[mxdFileSpec])
                    || (!myMapDoc.IsMapDocument[mxdFileSpec]))
                    ErrorOut(20216, "Given map file ({0}) is not a valid ArcMAP map document (mxd) file", mxdFileSpec);
                else
                {
                    if (myMapDoc.IsPasswordProtected[mxdFileSpec])
                        ErrorOut(20218, "Given map file ({0}) is password protected. This is not supported", mxdFileSpec);
                    else
                    {
                        myMapDoc.Open(mxdFileSpec);

                        if (0 >= myMapDoc.Map[0].LayerCount)
                            ErrorOut(20220, "Given map file ({0}) does not contain any layer.", mxdFileSpec);
                        else
                        {
                            mapDocument = myMapDoc;
                            return exitCodeEnum.success;
                        }
                    }
                }
            }
            return exitCodeEnum.invalidMxdFileSpec;
        }

        private static exitCodeEnum ValidateDxfFileSpec(string dxfFileSpec)
        {
            try
            {
                if (String.IsNullOrEmpty(dxfFileSpec))
                    ErrorOut(20222, "No output Dxf file specified");
                else
                {
                    var fullSpec = GetFullFileName(dxfFileSpec);

                    if (File.Exists(fullSpec))
                        ErrorOut(20226, "Output Dxf file ({0}) already exists", fullSpec);
                    else
                    {
                        var filePath = Path.GetDirectoryName(fullSpec);

                        if (!Directory.Exists(filePath))
                            ErrorOut(20224, "Directory specified by output Dxf file ({0}) does not exist", dxfFileSpec);
                        else
                            return exitCodeEnum.success;
                    }
                }
            }
            catch (Exception)
            {
                ErrorOut(20213, "Crap specified as output file (dxf) - formal incorrect filespec ({0})", dxfFileSpec);
            }

            return exitCodeEnum.invalidDxfFileSpec;
        }

        private static exitCodeEnum ValidateAndInitLogging(string fileSpec)
            => !string.IsNullOrEmpty(fileSpec)
                ? ValidateAndInitLoggingToFile(fileSpec)
                : exitCodeEnum.success;

        private static exitCodeEnum ValidateAndInitLoggingToFile(string fileSpec)
        {
            try
            {
                var fullSpec = GetFullFileName(fileSpec);
                var filePath = Path.GetDirectoryName(fullSpec);

                if (Directory.Exists(filePath))
                {
                    InitLoggingToFile(GetFullFileName(fileSpec));
                    return exitCodeEnum.success;
                }
                else
                    WarningOut(30106, "Cannot write to logfile - directory specified does not exist");
            }
            catch (Exception ex)
            {
                WarningOut(30104, "Crap specified as log file - formal incorrect filespec ({0})", $"{ex.GetType()} - {ex.Message}");
            }

            return exitCodeEnum.invalidLogFileSpec;
        }

        private static exitCodeEnum ValidateMapScale(IMapDocument mapDoc, double? scaleSpec, out double mapScale)
        {
            mapScale = mapDoc.Map[0].MapScale;
            if (scaleSpec.HasValue)
                if ((1.0 <= scaleSpec.Value) && (1E9 >= scaleSpec.Value))
                {
                    mapScale = scaleSpec.Value;
                    mapDoc.Map[0].MapScale = mapScale;      // Scale the map to the desired scale
                }
                else
                {
                    ErrorOut(20228, "Illegal map scale specified. Must be numeric (floating point value) and in the range of [1.0 .. 1.0E9]");
                    return exitCodeEnum.invalidMapScale;
                }

            return exitCodeEnum.success;
        }

        private static exitCodeEnum ValidateSelectionLayers(IMapDocument mapDoc, string selectionLayerSpec, string maskingLayerSpec, out ILayer selectionLayer, out ILayer maskingLayer)
        {
            selectionLayer = null;
            maskingLayer = null;

            var esriLayers = new ESRILayers(mapDoc.Map[0]);

            if (!TryGetLayerByName(esriLayers, selectionLayerSpec, out selectionLayer))
            {
                ErrorOut(20210, "Layer {0} is not a valid Layer that can used as selction layer", selectionLayerSpec);
                return exitCodeEnum.invalidSelectionLayerSpec;
            }
            else if (!TryGetLayerByName(esriLayers, maskingLayerSpec, out maskingLayer))
            {
                ErrorOut(20212, "Layer {0} is not a valid Layer that can used as masking layer", maskingLayerSpec);
                return exitCodeEnum.invalidMaskingLayerSpec;
            }
            return exitCodeEnum.success;
        }

        private static exitCodeEnum ValidateDxfVersion(DxfVersion version)
        {
            if (version < DxfVersion.AutoCad2000)
            {
                ErrorOut(20234, "Invalid Version of DXF Format selected. Versions before AutoCad 2000 are ot supported");
                return exitCodeEnum.InvalidDxfFileFormat;
            }
            return exitCodeEnum.success;
        }

        private static bool TryGetLayerByName(ESRILayers lyrs, string layerName, out ILayer layer)
        {
            layer = null;
            if (!string.IsNullOrEmpty(layerName))
            {
                layer = lyrs.LayersByName(layerName).FirstOrDefault();
                return (layer != null);
            }
            return true;
        }

        private static string GetFullFileName(string fileSpec)
        {
            var fullSpec = (fileSpec.Contains(Path.DirectorySeparatorChar))
                ? fileSpec
                : Path.Combine("." + Path.DirectorySeparatorChar, fileSpec);
            return (Path.IsPathRooted(fullSpec))
                ? fullSpec
                : Path.Combine(Directory.GetCurrentDirectory(), fullSpec);
        }
        #endregion

        #region DXF Writer setup

        private static void SetupWriter(IScreenDisplay display, IMap map, DxfVersion version, bool binary, out IDxfWriter dxfWriter)
        {
            dxfWriter = null;
            try
            {
                dxfWriter = new DxfWriter(display, map);

                /// Output options
                if (dxfWriter is IDxfWriterOutputOptions dxfO)
                {
                    dxfO.DxfVersion = version;
                    // CancelTracker and StepProgressor: We don't link the cancel tracker directly to the step progressor
                    // as this may lead to unpredictable progressbar positioning - in our current situation.
                    // dxfO.StepProgressor = StepProgressorDummy.FromApplication(_application);
                    // dxfO.CancelTracker = CancelTrackerDummy.FromApplication(_application);
                }

                // Events
                if (dxfWriter is IDxfWriterEvents dxfE)
                {
                    dxfE.OnStart += dxfexort_Started;
                    dxfE.OnNothingDone += dxfExport_NothingDone;
                    dxfE.OnUnhandledException += dxfExport_UnhandledException;
                    dxfE.OnSuccess += dxfexport_Suceeded;
                }

                // Options
                if (dxfWriter is IDxfWriterOptions dwo)
                {
                    dwo.UseSelectionSet = false;
                    dwo.ReferenceScale = (0 >= map.ReferenceScale) ? map.MapScale : map.ReferenceScale;
                    dwo.CurrentScale = map.MapScale;
                    dwo.BinaryDXF = binary;
                }

                // Runmode - Batch
                if (dxfWriter is IDxfWriterRunMode drmo)
                    drmo.RunMode = DxfWriterRunMode.Batch;
            }
            catch (Exception ex)
            {
                ErrorOut(exitCodeEnum.errorSetupDxfWriter, 20230, "Error setting up dxfwriter component.\n{0}", ex.ToString());
            }
        }

        #endregion

        #region Log/Error output

        private static bool _pgmInfoWritten = false;
        private static void PgmInfoOut()
        {
            if (!_pgmInfoWritten)
            {
                _log.Info(PgmInfo);
                _pgmInfoWritten = true;
            }
        }

        private static void WarningOut(int msgNr, string defaultMsg, params object[] args)
        {
            PgmInfoOut();
            _log.Warn(_lng.FmtLoadStr(msgNr, defaultMsg, args));
        }

        private static void ErrorOut(int msgNr, string defaultMsg, params object[] args)
        {
            PgmInfoOut();
            _log.Error(_lng.FmtLoadStr(msgNr, defaultMsg, args));
        }

        private static void ErrorOut(exitCodeEnum exitCode, int msgNr, string defaultMsg, params object[] args)
        {
            ErrorOut(msgNr, defaultMsg, args);
            _errorLevel = exitCode;
        }

        #endregion

        #region ArcGIS Licensing and session management

        private static exitCodeEnum ValidateAndInitArcGIS()
        {
            // Initialize the license once at the beginning of job execution
            // Find an esri product to get a license for
            var arcGISProduct = FindProduct();
            if (arcGISProduct.HasValue)
                if (default(esriLicenseProductCode) != FindLicense(arcGISProduct.Value))
                    return exitCodeEnum.success;
                else
                {
                    ErrorOut(10710, "No suitable ArcGIS license (engine, desktop basic, desktop standard, desktop advanced) available - exiting...");
                    return exitCodeEnum.noArcGISLicense;
                }
            else
            {
                ErrorOut(10712, "No suitable ArcGIS Application (engine, desktop) installed on system - exiting...");
                return exitCodeEnum.noArcGISLicense;
            }
        }

        private static ProductCode? FindProduct()
            => RuntimeManager.Bind(ProductCode.Engine)
                ? ProductCode.Engine
                : RuntimeManager.Bind(ProductCode.Desktop)
                    ? ProductCode.Desktop
                    : (ProductCode?) null;

        private static esriLicenseProductCode FindLicense(ProductCode product)
            => ((esriLicenseProductCode[])Enum.GetValues(typeof(esriLicenseProductCode)))
                .FirstOrDefault(p => InitArcGISLicense(p));

        private static bool InitArcGISLicense(esriLicenseProductCode arcGISLicensedProduct)
        {
            var status = AOInit.Initialize(arcGISLicensedProduct);
            return (esriLicenseStatus.esriLicenseAlreadyInitialized == status)
                || (esriLicenseStatus.esriLicenseCheckedOut == status);
        }

        private static void DetachFromArcGIS()
            => AOInit.Shutdown();

        private static AoInitialize AOInit
            => _aoInit ?? (_aoInit = new AoInitializeClass() as AoInitialize);

        #endregion

        #region events
        private static void dxfexort_Started(object sender, DxfWriterStartEventEventArgs e)
        {
            _logger = new OutcomeLogger(_lng);
            _logger.LogStart();
        }

        #region handling successful termination
        private static void dxfexport_Suceeded(object sender, DxfWriterSuccessEventEventArgs e)
        {
            if (e?.ErroneousLayers?.Any() ?? false)
                _logger.LogIrregularities(null, e.FilesWritten, e.ErroneousLayers, e.ExportedLayers);
            else
                _logger.LogOutcome(e.FilesWritten, e.ExportedLayers);

            _logger.LogEnd();
        }

        private static void dxfExport_NothingDone(object sender, EventArgs e)
        {
            _logger.LogOutcome(null, null);

            _logger.LogEnd();
        }

        #endregion

        #region handling unexpected errors
        private static void dxfExport_UnhandledException(object sender, DxfWriterUnhandledExceptionEventArgs e)
        {

            if (e.Exception is OutOfMemoryException)
                HandleOutOfMemoryException(e);
            else
                HandleUnknownException(e);

            _logger.LogEnd();
        }

        private static void HandleUnknownException(DxfWriterUnhandledExceptionEventArgs e)
        {
            Exception ex = e?.Exception ?? new Exception("Unknown Exception");

            _logger.LogIrregularities(ex, e?.FilesWritten, null, null);
        }

        private static void HandleOutOfMemoryException(DxfWriterUnhandledExceptionEventArgs e)
        {
            _logger.LogIrregularities(null, e?.FilesWritten, null, null,
                _lng.LoadStr(10903, "Dataset to be written is too big. Select less features, Limit # of Layers to export and use a selection layer."));
        }

        #endregion

        #endregion

        #region log4net stuff

        private static void InitLoggingPrimary()
        {
            if (!TryInitLoggingConfig())
                InitLoggingToConsole();
        }
        private static void InitLoggingToConsole()
            => BasicConfigurator.Configure(ConsoleAppender());

        private static void InitLoggingToFile(string logFileName)
        {

            var la = LogFileAppender(logFileName);
            log4netHierarchy.Root.AddAppender(la);
        }

        private static IAppender LogFileAppender(string logFileName)
            => ActivatedAppender(new FileAppender()
            {
                Layout = LogfileLayout,
                File = logFileName,
                AppendToFile = true
            });

        private static IAppender ConsoleAppender()
            => ActivatedAppender(new ConsoleAppender() { Layout = ConsoleLayout, Target = log4net.Appender.ConsoleAppender.ConsoleOut });

        private static ILayout ConsoleLayout
            => ActivatedLayout(new PatternLayout()
            {
                ConversionPattern = "%message%newline"
            });

        private static ILayout LogfileLayout
            => ActivatedLayout(new PatternLayout()
            {
                ConversionPattern = "%date %-5level %message%newline"
            });

        private static ILayout ActivatedLayout(ILayout layout)
        {
            (layout as LayoutSkeleton)?.ActivateOptions();
            return layout;
        }

        private static IAppender ActivatedAppender(IAppender appender)
        {
            (appender as AppenderSkeleton)?.ActivateOptions();
            return appender;
        }

        private static Hierarchy log4netHierarchy
            => (Hierarchy)LogManager.GetRepository();

        /// <summary>
        /// Try to init the log4net logging using config files from 
        /// a -> the user's documents\VertiGIS\VertiGIS Dxf Export For ArcMap folder
        /// b -> the install folder of VertiGIS Dxf Export for ArcMap
        /// </summary>
        /// <returns></returns>
        private static bool TryInitLoggingConfig()
        {
            return TryConfigure(DxfExporterUserDocumentsPath) || TryConfigure(DxfExportInstallPath);
        }

        private static bool TryConfigure(string basePath)
        {
            var path = Path.Combine(basePath, "log4net.config");
            return File.Exists(path)
                ? TryConfigureCore(path)
                : false;
        }

        private static bool TryConfigureCore(string path)
        {
            try
            {
                XmlConfigurator.Configure(new FileInfo(path));
                return LogManager.GetRepository().Configured;
            }
            catch (Exception)
            {
                return false; // Omit
            }
        }

        private static string DxfExportInstallPath
        {
            get
            {
                var assemblyURI = (new Uri(Assembly.GetExecutingAssembly().Location)).AbsolutePath;
                return Path.GetDirectoryName(Uri.UnescapeDataString(assemblyURI));
            }
        }

        private static string DxfExporterUserDocumentsPath
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "VertiGIS",
                    Product.Name);
                
        #endregion

        #region metadata of the program to be used for logging etc.

        public static string PgmInfo
            => $"{Product.Name} Ver. {Product.Version})";

        public static string AssemblyFileName
            => Path.GetFileName(Assembly.GetEntryAssembly()?.GetName()?.Name ?? "VertiGISDxfBatch.exe");

        #endregion
    }
}
