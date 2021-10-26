using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GEOCOM.GNSD.Common.TypeConversion
{
    /// <summary>
    /// Generic type conversion library, useful for types that don't have a .Parse() method
    /// and can convert a delimited string of values to an IEnumerable of the specified type
    /// </summary>
    public class MultiConverter
    {
        #region Single Value Methods

        /// <summary>
        /// Converts the specified value using the default typeconverter
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TValue Convert<TValue>(object value, bool throwOnNull = false)
        {
            return MultiConverter.Convert<TValue, TypeConverter>(value);
        }

        /// <summary>
        /// Converts the specified value using a specified typeconverter
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TTypeConverter">The type of the type converter.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TValue Convert<TValue, TTypeConverter>(object value, bool throwOnNull = false)
            where TTypeConverter : TypeConverter
        {
            if (value == null && throwOnNull)
                throw new ArgumentNullException("value");

            if (value == null && !throwOnNull)
                return default(TValue);

            try
            {
                var typeConverter = (TTypeConverter)TypeDescriptor.GetConverter(typeof(TValue));

                return (TValue)typeConverter.ConvertFrom(value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error converting value {0} using TypeConverter {1} to Type {2}",
                    value, typeof(TTypeConverter).Name, typeof(TValue).Name), ex);
            }
        } 

        #endregion

        #region IEnumerable Methods
        
        /// <summary>
        /// Converts the specified value using the default typeconverter.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static IEnumerable<TValue> Convert<TValue>(string value, char[] delimiter)
        {
            return MultiConverter.Convert<TValue, TypeConverter>(value, delimiter);
        }

        /// <summary>
        /// Converts the specified value using the specified typeconverter.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TTypeConverter">The type of the type converter.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static IEnumerable<TValue> Convert<TValue, TTypeConverter>(string value, char[] delimiter)
            where TTypeConverter : TypeConverter
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            if (delimiter == null)
                throw new ArgumentNullException("delimiter");

            if (delimiter.Length == 0)
                throw new ArgumentException("no delimiter specified", "delimiter");

            try
            {
                var typeConverter = (TTypeConverter)TypeDescriptor.GetConverter(typeof(TValue));

                return new List<string>(
                    value.Split(delimiter))
                    .ConvertAll(s => (TValue)typeConverter.ConvertFrom(s)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error converting string '{0}' using delimiter '{1}' and TypeConverter {2} to Type {3}",
                    value, delimiter, typeof(TTypeConverter).Name, typeof(TValue).Name), ex);
            }
        } 

        #endregion
    }
}