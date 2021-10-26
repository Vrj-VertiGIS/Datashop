using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.TTF;
using netDxf.Tables;
#if DEBUG
using System.Diagnostics;
#endif

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfPolygonLayerWriter : DxfLayerWriterBase
    {
        protected FillSymbology _symbology = null;

        internal DxfPolygonLayerWriter(_IDxfWriterContext context, IGeoFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            var markerSymbology = new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, GlyphAlignment.middleCenter);
            _symbology = new FillSymbology(_esriLyr, 
                markerSymbology,
                new LineSymbology(_esriLyr, markerSymbology, DotsToMeter, new EsriLineSymbolByIdentityComparer()),
                DotsToMeter);
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var symbolInfo = _symbology.CreateInfo(feature);

            var polygon = Clip(feature.Shape) as IPolygon;

            if ((null != polygon) && (!polygon.IsEmpty))
            {
                WritePolygonGeometryLayers(symbolInfo, polygon);
                base.WriteFeature(feature);
            }
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
            if (symbolInfo.FillIsVisible)
            {
                var boundary = EntityFactory.CreatePolyline(polygon as IPolycurve, LinetypeLineSymbolInfo.Default);
                WriteEntity(EntityFactory.CreateHatch(boundary, symbolInfo)); 
            }
        }

        private void WritePolygonBoundary(LayeredLineSymbolInfo symbolInfo, IPolygon polygon)
        {
            foreach (var boundary in EntityFactory.CreatePolylineGeometries(symbolInfo, polygon))
                WriteEntity(boundary);
        }

    }
}