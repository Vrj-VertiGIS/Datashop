using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ISimpleLineSymbolExtensions
    {
        public static uint Identity(this ISimpleLineSymbol lineSymbol)
        {
            // Encode the style in bit 17 thru 19 of the identity 32-bit word. Encode
            // the line with in the low-order 17 bits (0 .. 16). Bits 20 thru 31 must be zero 
            // to indicate a simple line symbol
            return ((uint)lineSymbol.Style << 8) | (lineSymbol as ILineSymbol).IdentityCore();
        }
    }
}
