using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public static class ISymbolExtensions
    {
        public static bool IsEqual(this ISymbol thisSymbol, ISymbol otherSymbol)
        {
            var thisClone = thisSymbol as IClone;
            var otherClone = otherSymbol as IClone;
            try
            {
                return ((null != thisClone) && (null != otherClone))
                    ? thisClone.IsEqual(otherClone)
                    : false;
            }
            catch (System.Exception ex)
            {
                var s = ex.ToString();
                return false;
            }

        }
    }
}
