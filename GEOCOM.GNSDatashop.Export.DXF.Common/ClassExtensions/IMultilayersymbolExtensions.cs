using System.Collections.Generic;

using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IMultilayersymbolExtensions
    {
        public static IEnumerable<IMarkerSymbol> LayersAsEnumerable(this IMultiLayerMarkerSymbol symbol)
        {
            for (int i = 0; i < symbol.LayerCount; i++)
                yield return symbol.Layer[i];
        }
    }
}
