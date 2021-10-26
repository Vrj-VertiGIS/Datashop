using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.Common.Clipping;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.Eventing;
using GEOCOM.GNSDatashop.Export.DXF.Factories;
using GEOCOM.GNSDatashop.Export.DXF.GraphicElementsWriter;
using GEOCOM.GNSDatashop.Export.DXF.Interface;
using GEOCOM.GNSDatashop.Export.DXF.LayerWriter;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using netDxf;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using IO = System.IO;
using Tables = netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF
{
    public class DxfWriter : IDisposable,
        IDxfWriter,
        IDxfWriterOutputOptions,
        IDxfWriterEvents,
        IDxfWriterOptions,
        IDxfWriterOutputInfo,
        IDxfWriterInfo,
        IDxfWriterRunMode,
        _IDxfWriterContext
    {
        #region private members

        // private IMap _map = null;

        private IDisplay _renderDisplay = null;

        private IEnumerable<ILayer> _layers = null;
        private IEnumerable<ILayer> _visibleOnlyLayers = null;

        private IEnumerable<IElement> _drawingLayerElements = null;

        private RegionOfInterest _regionOfInterest = null;

        private DxfDocument _dxfDocument = null;
        public const DxfVersion DxfVersionDefault = DxfVersion.AutoCad2010;
        private DxfVersion _dxfVersion = DxfVersionDefault;

        private string _dxfFileName = String.Empty;

        private double _mapReferenceScale = 0.0;
        private double _mapCurrentScale = 0.0;

        private bool _keepEmptyDxfFiles = false;

        private CancelTrackerDummy _cancelTracker = null;
        private StepProgressorDummy _stepProgressor = null;

        private bool _useFeatureSelection = true;

        private List<string> _filesWritten = new List<string>();

        private bool _binaryDXF = false;

        private IntPtr _hDC = IntPtr.Zero;      // An internal "compatible" memory display context to be used for all GDI graphics operations

        private Graphics _graphics;             // Graphics object based on _hdc used for all drawing operations to internal dc

        private ExpressionParsers _expressionParsers = new ExpressionParsers();

        private ProcessedLayersInfo _layersInfo = new ProcessedLayersInfo();

        #endregion

        #region Construction/destruction

        /// <summary>
        /// Create a dxf writer instance.
        /// </summary>
        /// <param name="renderDisplay">ArcMap display object</param>
        /// <param name="layers">The layers to be exported</param>
        /// <param name="referenceScale">Reference scale of the map object</param>
        /// <param name="currentMapScale">Current map scale of the map object</param>
        public DxfWriter(IDisplay renderDisplay, IEnumerable<ILayer> layers, double referenceScale, double currentMapScale)
        {
            _renderDisplay = renderDisplay;

            var esriLayers = new ESRILayers(layers, currentMapScale);
            _layers = esriLayers.Layers;
            _visibleOnlyLayers = esriLayers.VisibleOnlyLayers;

            // This installs a cancel tracker- and step progressor dummy/wrapper. Assign real yobjects if the functionality is requried.
            CancelTracker = new CancelTrackerDummy();
            StepProgressor = new StepProgressorDummy();

            _mapReferenceScale = referenceScale;
            _mapCurrentScale = currentMapScale;

            _hDC = (_renderDisplay is IScreenDisplay sd)
                ? WinAPI.CreateDCFromHWND(sd.hWnd)
                : WinAPI.CreateDCFromHWND(0);

            _graphics = Graphics.FromHdc(_hDC);
            _graphics.PageUnit = GraphicsUnit.Point;
            _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        /// <summary>
        /// Create a dxf writer instance
        /// </summary>
        /// <param name="renderDisplay">ArcMap display object</param>
        /// <param name="esriMap">an IMap interface of the ArcGIS map to export data from</param>
        public DxfWriter(IDisplay renderDisplay, IMap esriMap)
            : this(renderDisplay, null, esriMap.ReferenceScale, esriMap.MapScale)
        {
            var esriLayers = new ESRILayers(esriMap);

            _layers = esriLayers.Layers;
            _visibleOnlyLayers = esriLayers.VisibleOnlyLayers;

            // The arcMap document drawing layer
            _drawingLayerElements = esriLayers.DrawingElements;
        }

        /// <summary>
        /// Create a dxf writer instance - overwrite maps- and/or referencescale by specified values 
        /// </summary>
        /// <param name="renderDisplay">ArcMap display object</param>
        /// <param name="esriMap">an IMap interface of the ArcGIS map to export data from</param>
        public DxfWriter(IDisplay renderDisplay, IMap esriMap, double? mapScale, double? referenceScale)
            : this(renderDisplay, null, esriMap.ReferenceScale, esriMap.MapScale)
        {
            var esriLayers = new ESRILayers(esriMap);

            _layers = esriLayers.Layers;
            _visibleOnlyLayers = esriLayers.VisibleOnlyLayers;

            // The arcMap document drawing layer
            _drawingLayerElements = esriLayers.DrawingElements;

            // override map- and/or reference scale if specified as argument
            if (mapScale.HasValue)
                _mapCurrentScale = mapScale.Value;
            if (referenceScale.HasValue)
                _mapReferenceScale = referenceScale.Value;
        }

        #endregion

        #region IDxfWriter implementation

        /// <summary>
        /// Write multiple dxf files. One File per (Multipart-)Geometry in regionOfInterest. dxfTemplateFileName is used as a file name template.
        /// occasionally (when there is more than one geometry to export) the running file number (_0001, _0002,...) is appended.
        /// </summary>
        /// <param name="dxfTemplateFileName">A fully qualified dxf file name to be written</param>
        /// <param name="visibleOnly">Export visible layers only</param>
        /// <param name="regionOfInterest">A collection of regions (Polygon geometries) to be exported - one per dxf file.
        ///                                 an empty collection or a null reference export everything on the map</param>
        /// <param name="maskGeometries">Mask out any feature covered by one of the polygons within this.</param>
        public void WriteDXFByTemplate(string dxfTemplateFileName, bool visibleOnly, IEnumerable<IGeometry> regionOfInterest, IGeometry maskGeometries)
        {
            try
            {
                _cancelTracker.Reset();

                _layersInfo.Clear();

                var layers = LayersToProcess(visibleOnly);

                if (layers.Any() || _drawingLayerElements.Any())
                {
                    _stepProgressor.nParts = (uint)(layers.Count() + (_drawingLayerElements.Any() ? 1 : 0));

                    _filesWritten.Clear();

                    SignalStart();

                    WriteDXFByTemplateCore(dxfTemplateFileName, layers, regionOfInterest, maskGeometries);

                    SignalSuccess();
                }
                else
                    OnNothingDone?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Close(false);
                OnUnhandledException?.Invoke(this, new DxfWriterUnhandledExceptionEventArgs(ex, _filesWritten));
            }
        }

        /// <summary>
        /// Export the contents of a map. The data to be exported is specified by beeing contained within the (multipart) polygon
        /// given by regionOfInterest.
        /// </summary>
        /// <param name="dxfFileName">The fully qualified name of the output file to be written.</param>
        /// <param name="visibleOnly">Export visible layers only</param>
        /// <param name="regionOfInterest">A region (single- or multipart polygon) specifying the data to export.</param>
        /// <param name="mask">Mask out any feature covered by one of the polygons within this.</param>
        public void WriteSingleDXF(string dxfFileName, bool visibleOnly, IGeometry regionOfInterest, IGeometry mask)
        {
            try
            {
                _cancelTracker.Reset();

                _layersInfo.Clear();

                var layers = LayersToProcess(visibleOnly);

                if (layers.Any() || _drawingLayerElements.Any())
                {
                    _stepProgressor.nParts = (uint)(layers.Count() + (_drawingLayerElements.Any() ? 1 : 0));

                    _filesWritten.Clear();

                    SignalStart();

                    WriteDXF(dxfFileName, layers, _drawingLayerElements, regionOfInterest, mask);

                    SignalSuccess();
                }
                else
                    OnNothingDone?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Close(false);
                OnUnhandledException?.Invoke(this, new DxfWriterUnhandledExceptionEventArgs(ex, _filesWritten));
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            DisposeManaged();
            DisposeUnmanaged();

            DotNetFrameworkSupport.CollectGarbage();

            GC.SuppressFinalize(this);
        }

        ~DxfWriter()
            => DisposeUnmanaged();

        private void DisposeManaged()
        {
            try
            {
                Close(false);
            }
            catch (Exception ex)
            {
                OnUnhandledException?.Invoke(this, new DxfWriterUnhandledExceptionEventArgs(ex, _filesWritten));
            }
        }

        private void DisposeUnmanaged()
        {
            if (IntPtr.Zero != _hDC)
            {
                WinAPI.DeleteDC(_hDC);
                _hDC = IntPtr.Zero;
            }
        }

        #endregion

        #region export core routines

        private void WriteDXFByTemplateCore(string dxfTemplateFileName, IEnumerable<ILayer> layers, IEnumerable<IGeometry> regionOfInterest, IGeometry maskGeometries)
        {
            if ((null == regionOfInterest) || (!regionOfInterest.Any()))
                WriteDXF(string.Format(SequenceNumerEnhancedFileNameTemplate(dxfTemplateFileName), 1), layers, _drawingLayerElements, null, maskGeometries);
            else if (regionOfInterest.Count() >= 2)
            {
                _stepProgressor.nParts *= (uint)regionOfInterest.Count();

                var dxfFNT = SequenceNumerEnhancedFileNameTemplate(dxfTemplateFileName);

                for (int i = 0; i < regionOfInterest.Count() && _cancelTracker.Continue(); i++)
                    WriteDXF(string.Format(dxfFNT, i + 1), layers, _drawingLayerElements, regionOfInterest.ElementAt(i), maskGeometries);
            }
            else
                WriteDXF(dxfTemplateFileName, layers, _drawingLayerElements, regionOfInterest.ElementAt(0), maskGeometries);
        }

        /// <summary>
        /// Append a sequence number format to the main file name - be aware that the given file name
        /// may consist of a directory spec, followed by the main file name, followed by a file extension (i.e. .dxf)
        /// </summary>
        private string SequenceNumerEnhancedFileNameTemplate(string dxfTemplateFileName)
        {
            var dirName = IO.Path.GetDirectoryName(dxfTemplateFileName);
            var fileName = IO.Path.GetFileNameWithoutExtension(dxfTemplateFileName);
            var fileExt = IO.Path.GetExtension(dxfTemplateFileName);

            var sfn = ((!string.IsNullOrEmpty(fileName)) ? fileName : "DxfData")
                    + @"_{0:000}"
                    + ((!string.IsNullOrEmpty(fileExt)) ? fileExt : @".DXF");

            var targetPath = (!string.IsNullOrEmpty(dirName))
                ? dirName
                : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None);

            var result = IO.Path.Combine(targetPath, sfn);

            return result;
        }

        private void WriteDXF(string dxfFileName, IEnumerable<ILayer> layers, IEnumerable<IElement> drawingLayerElements, IGeometry regionOfInterest, IGeometry mask)
        {
            _dxfDocument = new DxfDocument(_dxfVersion);
            SymbolName.Reset();     // Reset blockname/linetype/textstyle.... unique id generator
            _dxfFileName = dxfFileName;
            _regionOfInterest = new RegionOfInterest(regionOfInterest, mask);
            var fileWiseLayersInfo = new ProcessedLayersInfo();

            try
            {
                try
                {
                    WriteESRILayers(layers, regionOfInterest, mask, fileWiseLayersInfo);

                    WriteDrawingLayer(drawingLayerElements, regionOfInterest, mask, fileWiseLayersInfo);

                    Close(_cancelTracker.Continue());
                }
                catch (Exception)
                {
                    Close(false);
                    throw;
                }
            }
            finally
            {
                _layersInfo.Append(fileWiseLayersInfo);
                DotNetFrameworkSupport.CollectGarbage();
            }
        }

        /// <summary>
        /// write the standard document layers 
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="regionOfInterest"></param>
        /// <param name="mask"></param>
        /// <param name="filewiseLayersInfo"></param>
        private void WriteESRILayers(IEnumerable<ILayer> layers, IGeometry regionOfInterest, IGeometry mask, ProcessedLayersInfo filewiseLayersInfo)
        {
            var thereIsASelection = layers.Any(t => (
                (t is IFeatureSelection fsel)
                && (null != fsel.SelectionSet)
                && (0 < (fsel.SelectionSet.Count))));

            foreach (var lyr in layers.Reverse())
                if (_cancelTracker.Continue())
                {
                    _stepProgressor.Message = lyr.Name + "...";

                    WriteESRILayer(lyr, (thereIsASelection && _useFeatureSelection), filewiseLayersInfo);
                }
                else
                    break;
        }

        /// <summary>
        /// Write the ArcMap document drawing layer.
        /// </summary>
        /// <param name="drawingElements"></param>
        /// <param name="regionOfInterest"></param>
        /// <param name="mask"></param>
        /// <param name="layersInfo"></param>
        private void WriteDrawingLayer(IEnumerable<IElement> drawingElements, IGeometry regionOfInterest, IGeometry mask, ProcessedLayersInfo layersInfo)
        {
            try
            {
                if (drawingElements.Any())
                {
                    _stepProgressor.Message = "ArcMap drawing layer...";
                    WriteDrawingLayer(drawingElements, layersInfo);
                }
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException)
                {
                    DotNetFrameworkSupport.CollectGarbage();
                    layersInfo.AddErroneousLayer(drawingElements, ex);
                    throw;
                }
                else
                    layersInfo.AddErroneousLayer(drawingElements, ex);
            }
        }

        private void WriteESRILayer(ILayer lyr, bool applySelectionSet, ProcessedLayersInfo layersInfo)
        {
            try
            {
                if (lyr.Valid)
                    WriteESRILayerCore(lyr, applySelectionSet, layersInfo);
                else
                    RegisterErroneousLayer(lyr);
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException)
                {
                    DotNetFrameworkSupport.CollectGarbage();
                    layersInfo.AddErroneousLayer(lyr, ex);
                    throw;
                }
                else
                    layersInfo.AddErroneousLayer(lyr, ex);
            }
        }

        private void WriteESRILayerCore(ILayer lyr, bool applySelectionSet, ProcessedLayersInfo layersInfo)
        {
            if (lyr is IBasemapLayer)
                RegisterUnsupportedLayer(lyr);
            else if (lyr is IRasterLayer)
                RegisterUnsupportedLayer(lyr);
            else if (lyr is IAnnotationSublayer)
                RegisterAnnotationSubLayerFilter(lyr);
            else if (lyr is IFeatureLayer flyr)
                TryWriteFeatureLayer(flyr, applySelectionSet, layersInfo);
            else if (lyr is ICompositeLayer)
                _stepProgressor.StepPart();
            else
                RegisterUnsupportedLayer(lyr);
        }

        private void TryWriteFeatureLayer(IFeatureLayer featureLayer, bool applySelectionSet, ProcessedLayersInfo layersInfo)
        {
            try
            {
                if (null != (featureLayer as IGeoFeatureLayer)?.Renderer as IRepresentationRenderer)
                    RegisterUnsupportedLayer(featureLayer);
                else
                    WriteFeatureLayer(featureLayer, applySelectionSet, layersInfo);

                DotNetFrameworkSupport.CollectGarbage();
            }
            catch (COMException ex)
            {
                string msg;
                switch ((uint)ex.HResult)
                {
                    case 0x80040207:
                        {
                            msg = (featureLayer is IFeatureLayerDefinition fld)
                                ? $"Invalid definition query on Layer {featureLayer.Name}: { fld.DefinitionExpression} "
                                : $"Invalid definition query on Layer {featureLayer.Name}";
                            // This texts will ge globalized by the erroneouslayersinfo.toString() method
                            _layersInfo.AddErroneousLayer(featureLayer, ex, msg);
                        }
                        break;
                    case 0x80040653:
                        {
                            msg = (featureLayer is IFeatureLayerDefinition fld)
                                ? $"Field not found on Layer {featureLayer.Name}: { fld.DefinitionExpression} "
                                : $"Field not found on Layer {featureLayer.Name}";
                            // This texts will ge globalized by the erroneouslayersinfo.toString() method
                            _layersInfo.AddErroneousLayer(featureLayer, ex, msg);
                        }
                        break;
                    default:
                        _layersInfo.AddErroneousLayer(featureLayer, ex);
                        throw;
                }
            }
        }

        private void WriteFeatureLayer(IFeatureLayer featureLayer, bool applySelectionSet, ProcessedLayersInfo layersInfo)
        {
            ESRIFeatureList features = new ESRIFeatureList(featureLayer, _regionOfInterest, applySelectionSet);

            var featureCount = features.Count;

            if (0 < featureCount)
            {
                _stepProgressor.PartSize = (uint)featureCount;

                var dxfLayer = DxfLayerFactory.CreateLayer(_dxfDocument.Layers, featureLayer.Name);

                var fc = featureLayer.FeatureClass;

                switch (fc.FeatureType)
                {
                    case esriFeatureType.esriFTAnnotation:
                        WriteAnnotationLayer(featureLayer, features, dxfLayer, layersInfo);
                        break;
                    case esriFeatureType.esriFTDimension:
                        WriteDimensionnLayer(featureLayer, features, dxfLayer, layersInfo);
                        break;
                    default:
                        WriteGeoFeatureLayerByShapeType(featureLayer as IGeoFeatureLayer, features, dxfLayer, layersInfo);
                        break;
                }
            }
            else
                RegisterEmptyLayer(featureLayer);
        }

        private void WriteGeoFeatureLayerByShapeType(IGeoFeatureLayer geoFeatureLayer, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            switch (geoFeatureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    WritePointLayer(geoFeatureLayer, features, dxfLayer, layersInfo);
                    break;

                case esriGeometryType.esriGeometryPolyline:
                    {
                        var renderer = geoFeatureLayer.Renderer;
                        var symb = renderer.Safe_SymbolByFeature(features.Features.ElementAt(0));
                        if ((null != symb) && (symb is ITextSymbol))
                            WriteGEOCOMUVTRendererLayer(geoFeatureLayer, features, dxfLayer, layersInfo);
                        else
                            WritePolylineLayer(geoFeatureLayer, features, dxfLayer, layersInfo);
                        break;
                    }

                case esriGeometryType.esriGeometryPolygon:
                    WritePolygonLayer(geoFeatureLayer, features, dxfLayer, layersInfo);
                    break;

                default:
                    RegisterUnsupportedLayer(geoFeatureLayer);
                    break;
            }
        }

        private void RegisterUnsupportedLayer(ILayer layer)
        {
            _layersInfo.ErroneousLayers.Add(new ErroneousLayerInfo(layer) { IsSupported = false });
            _stepProgressor.StepPart();
        }

        private void RegisterErroneousLayer(ILayer layer)
        {
            _layersInfo.ErroneousLayers.Add(new ErroneousLayerInfo(layer));
            _stepProgressor.StepPart();
        }

        private void RegisterEmptyLayer(ILayer layer)
        {
            _layersInfo.ExportedLayers.Add(new ExportedLayerInfo(layer) { FeaturesWritten = 0 });
            _stepProgressor.StepPart();
        }

        private void Close(bool writeDxfData)
        {
            if (writeDxfData)
                PersistDXF();

            _dxfDocument = null;
            DotNetFrameworkSupport.CollectGarbage();
        }

        private void PersistDXF()
        {
            if ((null == _dxfFileName) || (_dxfFileName.Length < 1)
                || (!IO.Directory.Exists(IO.Path.GetDirectoryName(_dxfFileName))))
                throw new Exception(string.Format("{0}: DXF file name not valid", this.GetType().Name));

            if ((null != _dxfDocument) && ((!_dxfDocument.IsEmpty()) || (_keepEmptyDxfFiles)))
            {
                AddHeaderData(_dxfDocument);
                _dxfDocument.Save(_dxfFileName, _binaryDXF);
                _filesWritten.Add(_dxfFileName);
            }
        }

        #endregion

        #region ArcMap document graphic slements layer
        
        private void WriteDrawingLayer(IEnumerable<IElement> elements, ProcessedLayersInfo layersInfo)
        {
            var dxfLayer = DxfLayerFactory.CreateLayer(_dxfDocument.Layers, "Graphic Elements");

            var elementWriter = new DxfGraphicElementsWriter(this, elements, dxfLayer);

            elementWriter.WriteElements();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(elements) { DotsToMeter = elementWriter.DotsToMeter, FeaturesWritten = elementWriter.FeaturesWritten, ElementsWritten = elementWriter.EntitiesWritten });

            _stepProgressor.StepPart();
        }

        #endregion

        #region export point features

        private void WritePointLayer(IGeoFeatureLayer esriLyr, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfPointLayerWriter(this, esriLyr, features, dxfLayer);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(esriLyr) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });
        }

        #endregion

        #region export polyline features

        private void WritePolylineLayer(IGeoFeatureLayer esriLyr, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfPolylineLayerWriter(this, esriLyr, features, dxfLayer);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(esriLyr) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });
        }

        #endregion

        #region export polygon features

        private void WritePolygonLayer(IGeoFeatureLayer esriLyr, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfPolygonLayerWriter(this, esriLyr, features, dxfLayer);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(esriLyr) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });
        }

        #endregion

        #region export GEOCOM TR labels

        private void WriteGEOCOMUVTRendererLayer(IGeoFeatureLayer esriLyr, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfGeocomUVTLayerWriter(this, esriLyr, features, dxfLayer);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(esriLyr) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });
        }

        #endregion

        #region ESRI annotation layer and annotation sublayer

        private HashSet<int> _restrictToAnnotationClasses = new HashSet<int>();
        private void RegisterAnnotationSubLayerFilter(ILayer layer)
        {
            if ((layer.Visible) && (layer is IAnnotationSublayer asl))
                _restrictToAnnotationClasses.Add(asl.AnnotationClassID);
        }

        private void WriteAnnotationLayer(IFeatureLayer featureLayer, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfAnnotationLayerWriter(this, featureLayer, features, dxfLayer, _restrictToAnnotationClasses);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(featureLayer) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });

            _restrictToAnnotationClasses.Clear();
        }

        #endregion

        #region ESRI dimension layer

        private void WriteDimensionnLayer(IFeatureLayer featureLayer, ESRIFeatureList features, Tables.Layer dxfLayer, ProcessedLayersInfo layersInfo)
        {
            var writer = new DxfDimensionLayerWriter(this, featureLayer, features, dxfLayer, _expressionParsers);

            writer.WriteLayer();

            layersInfo.ExportedLayers.Add(new ExportedLayerInfo(featureLayer) { DotsToMeter = writer.DotsToMeter, FeaturesWritten = writer.FeaturesWritten, ElementsWritten = writer.EntitiesWritten });
        }

        #endregion

        #region DXF document metadata

        private void AddHeaderData(DxfDocument document)
        {
            document.DrawingVariables.LastSavedBy = $"{Product.Name} Ver. {Product.Version} / {Product.BuildDate}";
        }

        #endregion

        #region event signalling

        private void SignalStart()
        {
            OnStart?.Invoke(this, new DxfWriterStartEventEventArgs());
        }

        private void SignalSuccess()
        {
            if (_cancelTracker.Continue())
            {
                if (_stepProgressor is StepProgressorDummy sd)
                    sd.ShowFullProgress();  // We want the user to see a 100% progress bar - for a short while ;-)

                // No abort - raise the success event
                OnSuccess?.Invoke(this, new DxfWriterSuccessEventEventArgs(_filesWritten, _layersInfo));
            }
        }

        private IEnumerable<ILayer> LayersToProcess(bool visibleOnly) => (visibleOnly) ? _visibleOnlyLayers : _layers;
        #endregion

        #region CancelTracker event handling/forwarding

        private void HandleCancelTrackerCancelEvent(object sender, EventArgs e)
        {
            OnAbort?.Invoke(this, new DxfWriterAbortEventEventArgs(_filesWritten, _layersInfo));
        }

        private void HandleCancelTrackerBeforeCancelEvent(object sender, CancelTrackerDummyBeforeCancelEventEventArgs e)
        {
            var eventArgs = new DxfWriterBeforeAbortEventEventArgs() { CancelAbort = e.AbortCancelRequest };

            OnBeforeAbort?.Invoke(this, eventArgs);

            e.AbortCancelRequest = eventArgs.CancelAbort;
        }

        #endregion

        #region IDxfWriterOutputOptions implementation
        public DxfVersion DxfVersion { get => _dxfVersion; set => _dxfVersion = value; }
        public bool KeepEmptyDxfFiles { get => _keepEmptyDxfFiles; set => _keepEmptyDxfFiles = value; }
        public ITrackCancel CancelTracker
        {
            get => _cancelTracker?.NativeObject;
            set
            {
                _cancelTracker = (null == value)
                    ? new CancelTrackerDummy()
                    : (value is CancelTrackerDummy cdy)
                        ? cdy
                        : throw new ArgumentException("Cannot use native cancel tracker heere");

                _cancelTracker.Reset();
                _cancelTracker.OnBeforeCancel += HandleCancelTrackerBeforeCancelEvent;
                _cancelTracker.OnCancel += HandleCancelTrackerCancelEvent;
            }
        }
        public IStepProgressor StepProgressor
        {
            get => _stepProgressor?.NativeObject;
            set
            {
                _stepProgressor = (null != value) && (value is StepProgressorDummy sdy)
                    ? sdy
                    : new StepProgressorDummy(value);
            }
        }

        #endregion

        #region IDxfWriterOpitons implementation

        public bool UseSelectionSet { get => _useFeatureSelection; set => _useFeatureSelection = value; }
        public double ReferenceScale { get => _mapReferenceScale; set => _mapReferenceScale = value; }
        public double CurrentScale { get => _mapCurrentScale; set => _mapCurrentScale = value; }
        public bool BinaryDXF { get => _binaryDXF; set => _binaryDXF = value; }

        #endregion

        #region IDxfWriterOutputInfo

        public IEnumerable<string> FilesWritten => throw new NotImplementedException();

        #endregion

        #region IDxfWriterEvents implementation

        public event EventHandler<DxfWriterStartEventEventArgs> OnStart;
        public event EventHandler<DxfWriterSuccessEventEventArgs> OnSuccess = null;
        public event EventHandler<DxfWriterAbortEventEventArgs> OnAbort = null;
        public event EventHandler<DxfWriterBeforeAbortEventEventArgs> OnBeforeAbort = null;
        public event EventHandler<EventArgs> OnNothingDone = null;
        public event EventHandler<DxfWriterUnhandledExceptionEventArgs> OnUnhandledException = null;

        #endregion

        #region dxf writer info
        public string VersionInfo
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                if (fvi.IsDebug)
                    return string.Format("{0} (debug)", fvi.FileVersion);
                else
                    return fvi.FileVersion;
            }
        }

        #endregion

        #region IDxfWriterRunMode 
        public DxfWriterRunMode RunMode { get; set; } = DxfWriterRunMode.Server;    // As we will not modify the datashop implementation right now ;-)

        #endregion

        #region IDxfWriterContext

        /// <summary>
        /// IDxfWriterContext - internal interface implementation
        /// => Simplify access of worker classes - i.e. DxfLayerWriter and descendants - to writer internals;
        /// </summary>
        IDisplay _IDxfWriterContext.RenderDisplay => _renderDisplay;
        IntPtr _IDxfWriterContext.HDC => _hDC;
        Graphics _IDxfWriterContext.Graphics => _graphics;
        IEnumerable<ILayer> _IDxfWriterContext.Layers => _layers;
        IEnumerable<ILayer> _IDxfWriterContext.VisibleOnlyLayers => _visibleOnlyLayers;
        RegionOfInterest _IDxfWriterContext.RegionOfInterest => _regionOfInterest;
        DxfDocument _IDxfWriterContext.DxfDocument => _dxfDocument;
        DxfVersion _IDxfWriterContext.DxfVersion => _dxfVersion;
        string _IDxfWriterContext.DxfFileName => _dxfFileName;
        double _IDxfWriterContext.MapReferenceScale => _mapReferenceScale;
        double _IDxfWriterContext.MapCurrentScale => _mapCurrentScale;
        CancelTrackerDummy _IDxfWriterContext.CancelTracker => _cancelTracker;
        StepProgressorDummy _IDxfWriterContext.StepProgressor => _stepProgressor;
        bool _IDxfWriterContext.UseFeatureSelection => _useFeatureSelection;
        IList<string> _IDxfWriterContext.FilesWritten => _filesWritten;

        #endregion
    }
}
