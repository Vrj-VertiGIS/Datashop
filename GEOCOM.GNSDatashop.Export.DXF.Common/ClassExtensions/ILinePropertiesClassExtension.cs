using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ILinePropertiesClassExtension
    {
        public static uint Identity(this ILineProperties lineProps)
        {
            // Encode the style in bit 17 thru 19 of the identity 32-bit word. Encode
            // the line with in the low-order 17 bits (0 .. 16). Bits 20 thru 31 encode
            // the line properties. Enshure bit 31 be set to 1 to differentiate to ISimplelineSymbols.
            //
            return 0x80000000 
                | ((null != lineProps.Template) 
                    ? (lineProps.Template.Identity() << 8) 
                    : 0)
                | (lineProps as ILineSymbol).IdentityCore();
        }
    }
}
