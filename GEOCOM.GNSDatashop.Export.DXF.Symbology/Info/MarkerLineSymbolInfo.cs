using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class MarkerLineSymbolInfo : LineSymbolInfo
    {
        private MarkerSymbolInfo _symbolInfo;

        private AOLineTemplate _lineTemplate;

        public MarkerLineSymbolInfo()
            : base() { }

        public MarkerLineSymbolInfo(ISymbol symbol, ILayer layer, MarkerSymbology symbology, LineDecorations lineDecoration)
            : base(symbol, layer, lineDecoration)
        {
            _symbolInfo = (symbol is IMarkerLineSymbol mls)
                ? symbology.CreateInfo(mls.MarkerSymbol as ISymbol)
                : null;

            _symbolInfo.Rotation = 0;   // Omit symbol's rotation - line markers are orientet only according to line angle

            _lineTemplate = new AOLineTemplate((symbol as ILineProperties).Template);
        }

        protected override byte LineOpacity => _symbolInfo.Opacity;

        public AOLineTemplate Template => _lineTemplate;

        public MarkerSymbolInfo SymbolInfo => _symbolInfo;
    }
}
