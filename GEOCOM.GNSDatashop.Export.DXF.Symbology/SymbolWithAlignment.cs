using System;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.TTF;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class SymbolWithAlignment : IEquatable<SymbolWithAlignment>
    {
        public ISymbol Symbol { get; }
        public GlyphAlignment Alignment { get; }

        public SymbolWithAlignment(ISymbol symbol, GlyphAlignment alignment)
        {
            Symbol = symbol;
            Alignment = alignment;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode() ^ (int)Alignment;
        }

        public override bool Equals(object other)
        {
            if (other is SymbolWithAlignment symbolWithAlignment)
            {
                return Equals(symbolWithAlignment);
            }
            return false;
        }

        public bool Equals(SymbolWithAlignment other)
        {
            if (other == null)
            {
                return false;
            }
            return Symbol.Equals(other.Symbol) && Alignment.Equals(other.Alignment);
        }

        public static bool operator ==(SymbolWithAlignment a, SymbolWithAlignment b)
        {
            return ReferenceEquals(a, b) || a?.Equals(b) == true;
        }

        public static bool operator !=(SymbolWithAlignment a, SymbolWithAlignment b)
        {
            return !(a == b);
        }
    }
}
