using System;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IEnumerableExtensions
    {
        public static T NullAwareAggregate<T>(this IEnumerable<T> source, Func<T, T, T> func) where T : class
        {
            return ((null != source) && source.Any())
                ? source.Aggregate(func)
                : default(T);
        }
    }
}
