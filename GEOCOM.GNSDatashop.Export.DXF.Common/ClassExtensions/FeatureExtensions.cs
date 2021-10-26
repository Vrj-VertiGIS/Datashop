using System;
using ESRI.ArcGIS.Geodatabase;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class FeatureExtensions
    {
        public delegate bool ValueHandler(object value);

        public static string ValueAsString(this IFeature feature, int fieldIndex, string nullValue)
        {
#if !DEBUG
            try
            {
#endif
                var val = feature.Value[fieldIndex];

                return (val != DBNull.Value) 
                    ? (val is System.String) ? (string)val : val.ToString()
                    : nullValue;
#if !DEBUG
            }
            catch (Exception)
            {
                return string.Empty;
            }
#endif
        }

        public static string ValueAsString(this IFeature feature, int fieldIndex)
        {
            return ValueAsString(feature, fieldIndex, string.Empty);
        }

        public static bool ProcessValue(this IFeature feature, int fieldIndex, ValueHandler handler)
        {
            return handler(feature.Value[fieldIndex]);
        }

    }
}
