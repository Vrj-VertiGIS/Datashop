using System;
using System.Linq;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.Carto;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ILayerFieldsExtensions
    {
        public static int FindQualifiedField(this ILayerFields fields, string unQualifiedFieldName)
        {
            if (null == fields) throw new ArgumentNullException();

            return FindQualifiedFieldCore(fields, unQualifiedFieldName);
        }

        private static int FindQualifiedFieldCore(this ILayerFields fields, string unQualifiedFieldName)
            => FieldList(fields).FirstOrDefault(f => IsSearchedField(f.Item2, unQualifiedFieldName))?.Item1 ?? -1;

        private static bool IsSearchedField(IField field, string unQualifiedName)
            => field.Name.Equals(unQualifiedName, StringComparison.OrdinalIgnoreCase)
            || field.Name.EndsWith(QualifiedFieldName(unQualifiedName), StringComparison.OrdinalIgnoreCase);

        private static string QualifiedFieldName(string unQualifiedFieldName)
            => $".{unQualifiedFieldName}";

        /// <summary>
        /// Enable using Linq - we need an enumerable of the COM exposed items
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static IEnumerable<Tuple<int, IField>> FieldList(this ILayerFields fields)
        {
            for (int i = 0; (i < fields.FieldCount); i++)
                yield return new Tuple<int, IField>(i, fields.Field[i]);
        }
    }
}
