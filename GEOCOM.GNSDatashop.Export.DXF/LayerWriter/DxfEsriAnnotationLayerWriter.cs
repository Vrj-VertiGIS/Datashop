using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    internal class DxfEsriAnnotationLayerWriter : DxfLayerWriterBase
    // Cannot inherit from DxfLayerWriter - we need the annotation extension to determine ths correct
    // text dotsToMeter scaling factor which input to the constructor of dxfentityfactory...
    {
        protected AnnotationTextSymbology _symbology = null;

        protected IAnnotationClassExtension _annotationExtension = null;

        internal DxfEsriAnnotationLayerWriter(_IDxfWriterContext context, IFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            _annotationExtension = (_esriLyr as IFeatureLayer)?.FeatureClass?.Extension as IAnnotationClassExtension; 
            _symbology = new AnnotationTextSymbology(_esriLyr, _context.DxfDocument.TextStyles, 
                new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter, TTF.GlyphAlignment.middleCenter),
                DotsToMeter);
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var labelline = Clip(feature.Shape);

            if ((null != labelline) && (!labelline.IsEmpty))
            {
                var symbolInfo = _symbology.CreateInfo(feature);

                if ((null != symbolInfo.TextSymbol) 
                    && (!string.IsNullOrEmpty(symbolInfo.LabelText))
                    && (symbolInfo.IsVisible))
                        WriteEntity(EntityFactory.CreateMText(symbolInfo));

                foreach (var leaderSymbolWithGeometry in symbolInfo.LeaderGeometries)
                    foreach (var layerSymbolInfo in leaderSymbolWithGeometry.SymbolInfo as LayeredLineSymbolInfo)
                        if (layerSymbolInfo.IsVisible)
                        {
                            var line = EntityFactory.CreatePolyline(leaderSymbolWithGeometry.Geometry as ICurve, layerSymbolInfo);
                            WriteEntity(line);
                        }

                foreach (var markerSymbolWithGeometry in symbolInfo.MarkerSymbols)
                {
                    var markerSymbolInfo = markerSymbolWithGeometry.SymbolInfo as MarkerSymbolInfo;
                    if (markerSymbolInfo.IsVisible)
                    {
                        var point = EntityFactory.CreateBlockInsert(markerSymbolInfo.Block.Block, markerSymbolInfo.Rotation, markerSymbolWithGeometry.Geometry as IPoint, symbolInfo);
                        WriteEntity(point);
                    }
                }
            }
        }

        internal override double DotsToMeterScaled => 25.4 / 72000.0 * _annotationExtension.ReferenceScale;

        internal override double DotsToMeter => ((null != _annotationExtension) && (0 < _annotationExtension.ReferenceScale))
            ? DotsToMeterScaled
            : DotsToMeterUnscaled;
    }

}
