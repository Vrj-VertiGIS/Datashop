using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public abstract class LineSymbolInfo : SymbolInfo, ILineSymbolInfo
    {
        protected LineDecorations _lineDecoration = null;

        public LineSymbolInfo() : base() 
        {
            _lineDecoration = new LineDecorations();
        }

        public LineSymbolInfo(ISymbol symbol, ILayer layer, LineDecorations lineDecoration)
            : base(symbol, layer)
        {
            _lineDecoration = lineDecoration;
        }

        protected byte LineTransparency => (byte) (255 - LineOpacity);              // Transparency of this line [0..255] where 255 is fully transparent

        public override byte Opacity => (byte)(LayerOpacity / 255 * LineOpacity);   // Overall opacity - including (inherited) layer opacity

        protected abstract byte LineOpacity { get; }                                // Opacity of this line [0..255] where 255 is fully opaque 

        public virtual LineDecorations LineDecoration => _lineDecoration;

        public double Offset => (_symbol as ILineProperties)?.Offset ?? 0.0;
    }
}
