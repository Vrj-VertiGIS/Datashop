using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf.Collections;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class DimensionTextSymbology : TextSymbologyBase<TextSymbolInfo>
    {
        public DimensionTextSymbology(ILayer esriLayer, TextStyles textStyles, MarkerSymbology markerSymbology, double dotsToMeter)
            : base(esriLayer, dotsToMeter, textStyles, markerSymbology)
        {
            var fields = (esriLayer as ILayerFields);
        }

        protected override TextSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            var tsi = new TextSymbolInfo(symbol, _esriLayer);
            tsi.TextStyle = GetTextStyle(tsi);
            return tsi;
        }
    }
}
