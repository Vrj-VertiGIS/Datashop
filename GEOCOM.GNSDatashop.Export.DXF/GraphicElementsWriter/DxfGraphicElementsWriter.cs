using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.LayerWriter;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using log4net;
using netDxf.Entities;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.GraphicElementsWriter
{
    internal class DxfGraphicElementsWriter : DxfLayerWriterBase
    {
        private IEnumerable<IElement> _elements;

        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        protected AnnotationTextSymbology _symbology = null;

        public DxfGraphicElementsWriter(_IDxfWriterContext context, IEnumerable<IElement> elements, Layer dxfLayer) : base(context, null, null, dxfLayer)
        {
            this._elements = elements;

            _symbology = new AnnotationTextSymbology(null, _context.DxfDocument.TextStyles,
                new MarkerSymbology(null, BlockFactory, DotsToMeter, TTF.GlyphAlignment.middleCenter),
                DotsToMeter);
        }

        internal override void WriteFeature(IFeature feature)
        {
            throw new NotImplementedException();
        }

        public void WriteElements()
        {
            foreach (var element in _elements.Reverse())
                if (Continue)
                    WriteElement(element);
                else
                    break;
        }

        private void WriteElement(IElement element)
        {
            var clipped = Clip(element.Geometry);
            if (!clipped?.IsEmpty ?? false)
                WriteVisibleSymbolElements(_symbology.CreateInfo(element));
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

    }
}
