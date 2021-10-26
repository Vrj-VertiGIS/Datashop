using System.Collections.Generic;
using ESRI.ArcGIS.Display;

using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class EsriLineSymbolByIdentityComparer : IEqualityComparer<ILineSymbol>
    {
        public bool Equals(ILineSymbol x, ILineSymbol y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(ILineSymbol obj)
        {
            return (int)obj.Identity();
        }
    }
}
