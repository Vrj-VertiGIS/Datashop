using System.Linq;
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
    internal class DxfGeocomUVTLayerWriter : DxfLayerWriterBase
    {
        protected GEOCOMUVTSymbology _symbology = null;

        internal DxfGeocomUVTLayerWriter(_IDxfWriterContext context, IGeoFeatureLayer esriLyr, ESRIFeatureList features, Layer dxfLayer)
            : base(context, esriLyr, features, dxfLayer)
        {
            _symbology = new GEOCOMUVTSymbology(esriLyr, DotsToMeter, _context.DxfDocument.TextStyles, 
                new MarkerSymbology(_esriLyr, BlockFactory, DotsToMeter));
        }

        internal override void WriteFeature(IFeature feature)
        {
            Step();

            var labelline = Clip(feature.Shape);

            if ((null != labelline) && (!labelline.IsEmpty))
            {
                var symbolInfo = _symbology.CreateInfo(feature);

                try
                {
                    NewMethod(symbolInfo);
                }
                finally
                {
                    symbolInfo.Dispose();
                }

                base.WriteFeature(feature);
            }
        }

        private void NewMethod(GEOCOMUVTSymbolInfo symbolInfo)
        {
            if (symbolInfo.IsVisible && !string.IsNullOrEmpty(symbolInfo.Text))
            {
                var text = EntityFactory.CreateMText(symbolInfo);
                WriteEntity(text);

                foreach (var leaderSymbolWithGeometry in symbolInfo.SymbolInfosWithGeometries.Where(entry => entry.SymbolInfo is LayeredLineSymbolInfo))
                    foreach (var layerSymbolInfo in (LayeredLineSymbolInfo)leaderSymbolWithGeometry.SymbolInfo)
                        if (layerSymbolInfo.IsVisible)
                        {
                            var line = EntityFactory.CreatePolyline(leaderSymbolWithGeometry.Geometry as ICurve, layerSymbolInfo);
                            WriteEntity(line);
                        }

                foreach (var markerSymbolWithGeometry in symbolInfo.SymbolInfosWithGeometries.Where(entry => entry.SymbolInfo is MarkerSymbolInfo))
                {
                    var markerSymbolInfo = (MarkerSymbolInfo)markerSymbolWithGeometry.SymbolInfo;
                    // if present, geocom uvt anchor points are always non transparent (although the color states full transparency)
                    var point = EntityFactory.CreateBlockInsert(markerSymbolInfo.Block.Block, 0.0, markerSymbolWithGeometry.Geometry as IPoint, symbolInfo);
                    WriteEntity(point);
                }
            }
        }
    }
}
