using System;
using System.Linq;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IUniqueValueRendererExtensions
    {
        public static IEnumerable<string> UniqueValues(this IUniqueValueRenderer renderer)
        {
            if (null != renderer)
            {
                var count = renderer.ValueCount;
                for (int i = 0; i < count; i++)
                    yield return renderer.Value[i];
            }
        }

        public static IDictionary<string, ISymbol> SymbolsByValue(this IUniqueValueRenderer renderer)
        {
            return UniqueValues(renderer).ToDictionary(v => v, v => renderer.Symbol[v]);
        }

        public static IDictionary<ISymbol, string> ValuesBySymbol(this IUniqueValueRenderer renderer)
        {
            return UniqueValues(renderer).ToDictionary(v => renderer.Symbol[v], v => v);
        }

        public static ISymbol NullAwareSymbol(this IUniqueValueRenderer renderer, string value)
        {
            try
            {
                return renderer.Symbol[value];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string NullAwareReferenceValue(this IUniqueValueRenderer renderer, string value)
        {
            try
            {
                return renderer.ReferenceValue[value];
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static ISymbol GroupingAwareSymbol(this IUniqueValueRenderer renderer, string value)
        {
            var refVal = renderer.NullAwareReferenceValue(value);
            return (string.IsNullOrEmpty(refVal))
                ? renderer.NullAwareSymbol(value)
                : renderer.NullAwareSymbol(refVal);
        }

    }
}
