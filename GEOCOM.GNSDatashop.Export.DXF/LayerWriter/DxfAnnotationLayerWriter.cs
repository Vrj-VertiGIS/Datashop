using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using log4net;
using netDxf.Entities;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfAnnotationLayerWriter : DxfLayerWriterBase
    // Cannot inherit from DxfLayerWriter - we need the annotation extension to determine ths correct
    // text dotsToMeter scaling factor which input to the constructor of dxfentityfactory...
    {
        protected AnnotationTextSymbology _symbology = null;

        protected IAnnotationClassExtension _annotationExtension = null;

        protected HashSet<int> _restrictToAnnotationClasses = null;

        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        internal DxfAnnotationLayerWriter(_IDxfWriterContext context, IFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer, HashSet<int> annotationClassesFilter)
            : base(context, esriLyr, features, dxfLayer)
        {
            _restrictToAnnotationClasses = annotationClassesFilter;

            _annotationExtension = (_esriLyr as IFeatureLayer)?.FeatureClass?.Extension as IAnnotationClassExtension;
            _symbology = new AnnotationTextSymbology(_esriLyr, _context.DxfDocument.TextStyles,
                new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, TTF.GlyphAlignment.middleCenterFull),
                DotsToMeter)
            { RegionOfInterest = _context.RegionOfInterest };
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            if (ApprovedByFilter(feature))
                WriteApprovedFeature(feature);
        }

        private void WriteApprovedFeature(IFeature feature)
        {
            var clippedFeatureShape = Clip(feature.Shape);

            if ((null != clippedFeatureShape) && (!clippedFeatureShape.IsEmpty))
            {
                WriteFeatureCore(feature);
                base.WriteFeature(feature);
            }
        }

        private void WriteFeatureCore(IFeature feature)
        {
            var symbolInfo = _symbology.CreateInfo(feature);
            try
            {
                WriteVisibleSymbolElements(symbolInfo);
            }
            finally
            {
                symbolInfo.Dispose();
            }
        }

        private void WriteVisibleSymbolElements(AnnotationTextSymbolInfo textSymbolInfo)
            => WriteSymbolElements(textSymbolInfo.SymbolInfosWithGeometries.Where(e => e.IsVisible), textSymbolInfo.Transparency);

        private void WriteSymbolElements(IEnumerable<SymbolInfoWithGeometry> symbolElements, short groupTransparency)
        {
            foreach (var symbolElement in symbolElements)
                WriteElement(symbolElement.Geometry, symbolElement.SymbolInfo, groupTransparency);
        }

        private void WriteElement(IGeometry geometry, ISymbolInfo symbolInfo, short groupTransparency)
        {
            if ((symbolInfo is TextSymbolInfo textSymbolInfo) 
                && (!string.IsNullOrEmpty(textSymbolInfo?.Text)))
                WriteEntity(EntityFactory.CreateMText(textSymbolInfo));

            else if (symbolInfo is MarkerSymbolInfo markerSymbolInfo)
            {
                EntityObject point = EntityFactory.CreateBlockInsert(
                    markerSymbolInfo.Block.Block, markerSymbolInfo.Rotation, (IPoint)geometry, groupTransparency);
                WriteEntity(point);
            }
            else if (symbolInfo is LayeredLineSymbolInfo layeredLineSymbolInfo)
            {
                var line = EntityFactory.CreatePolyline((ICurve)geometry, layeredLineSymbolInfo);
                WriteEntity(line);
            }

            else if (symbolInfo is LayeredFillSymbolInfo layeredFillSymbolInfo)
                WritePolygonGeometryLayers(layeredFillSymbolInfo, (IPolygon)geometry);

            else
                throw new InvalidOperationException($"Unexpected SymbolInfo of type \"{symbolInfo.GetType().Name}\".");
        }

        private void WritePolygonGeometryLayers(LayeredFillSymbolInfo symbolInfo, IPolygon polygon)
        {
            foreach (var symbolLayer in symbolInfo)
            {
                WritePolygonFill(symbolLayer, polygon);
                WritePolygonBoundary(symbolLayer.OutLine, polygon);
            }
        }

        private void WritePolygonFill(FillSymbolInfo symbolInfo, IPolygon polygon)
        {
            if (symbolInfo.IsVisible)
            {
                IEnumerable<EntityObject> boundary = EntityFactory.CreatePolyline(polygon, LinetypeLineSymbolInfo.Default);
                WriteEntity(EntityFactory.CreateHatch(boundary, symbolInfo));
            }
        }

        private void WritePolygonBoundary(LayeredLineSymbolInfo symbolInfo, IPolygon polygon)
        {
            foreach (IEnumerable<EntityObject> boundary in CreatePolylineEntities(symbolInfo, polygon))
            {
                WriteEntity(boundary);
            }
        }

        protected IEnumerable<IEnumerable<EntityObject>> CreatePolylineEntities(LayeredLineSymbolInfo symbolLayers, IPolycurve polyCurve)
        {
            foreach (var symbolInfoLayer in symbolLayers.LayersByVisibility.Reverse())
                yield return EntityFactory.CreatePolyline(polyCurve, symbolInfoLayer);
        }

        /// <summary>
        /// This overrides the "normal" map reference scale as there is a dedicated value supplied
        /// by the annotationextension.
        /// </summary>
        internal override double DotsToMeterScaled => 25.4 / 72000.0 * _annotationExtension.ReferenceScale;

        public sealed override double DotsToMeter => ((null != _annotationExtension) && (0 < _annotationExtension.ReferenceScale))
            ? DotsToMeterScaled
            : DotsToMeterUnscaled;


        #region Filtering by annotation class

        private bool ApprovedByFilter(IFeature feature)
        {
            var annoFeature = feature as IAnnotationFeature2;
            if (null != annoFeature)
                return ApprovedByFilter(annoFeature);
            else
                _log.Warn($"Not an annotation Feature: {(_esriLyr.FeatureClass as IDataset).Name} - OID: {feature.OID}. Geometry is of type {feature?.Shape?.GeometryType ?? esriGeometryType.esriGeometryAny}.");

            return false;             
        }

        private bool ApprovedByFilter(IAnnotationFeature2 annoFeature)
            => 0 <= (annoFeature?.AnnotationClassID ?? -1)
                ? ApprovedByFilter(annoFeature.AnnotationClassID)
                : true; // If no annotation class id in the feature - draw the annotation

        private bool ApprovedByFilter(int annotationClassId)
            => _restrictToAnnotationClasses.Contains(0 <= annotationClassId ? annotationClassId : 1);
        #endregion
    }
}
