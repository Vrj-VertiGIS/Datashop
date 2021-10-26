using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ILineSymbolExtensions
    {
        public static uint Identity(this ILineSymbol lineSymbol)
        {
            if (lineSymbol is ISimpleLineSymbol sls)
                return sls.Identity();
            else if (lineSymbol is ILineProperties lpr)
                return lpr.Identity();
            else
                return IdentityCore(lineSymbol);
        }

        internal static uint IdentityCore(this ILineSymbol lineSymbol)
        {
            // At this level, we only have the line with as property
            // use the low order 17 bits to encode this - cut if larger
            return 0x1FFFF & (uint)(100 * lineSymbol.Width);
        }
    }
}
