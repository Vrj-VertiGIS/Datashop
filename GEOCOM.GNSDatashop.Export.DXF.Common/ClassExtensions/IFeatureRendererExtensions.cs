using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IFeatureRendererExtensions
    {
        public static ISymbol Safe_SymbolByFeature(this IFeatureRenderer renderer, IFeature feature)
        {
            try
            {
                return renderer.SymbolByFeature[feature];
            }
            catch (COMException ex)
            {
                if (0x80004005 == (uint)ex.ErrorCode)
                    return null;
                throw;
            }
        }
    }
}
