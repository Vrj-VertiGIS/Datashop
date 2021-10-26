using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.Common.Utils
{
    /// <summary>
    /// Extension methods to convert an IEnumerable<typeparamref name="T"/> to a Csv string
    /// </summary>
    public static class CsvWriter
    {
        /// <summary>
        /// Toes the CSV string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="fieldHeaders">The field headers.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public static string ToCsvString<T>(this IEnumerable<T> data, IEnumerable<string> fieldNames, IEnumerable<string> fieldHeaders = null, string separator = ",")
        {
            var sb = new StringBuilder();

            if (fieldHeaders != null)
            {
                sb.Append(string.Join(separator, fieldHeaders));
                sb.Append(Environment.NewLine);
            }

            foreach (var d in data)
            {
                var t = d.GetType();

                var stringValues = fieldNames.Select(t.GetProperty)
                    .ToList()
                    .ConvertAll(p => p.GetValue(d, null) ?? "")
                    .ConvertAll(v => v.ToString()
                                      .EscapeValue());

                sb.Append(string.Join(separator, stringValues));
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes the value.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string EscapeValue(this string s)
        {
            var quote = "\"";
            var escapedQuote = "\"\"";
            var mustQuoteChars = new[] { ',', '"', '\n' };

            if (s.Contains(quote))
                s = s.Replace(quote, escapedQuote);

            if (s.IndexOfAny(mustQuoteChars) > -1)
                s = string.Format("{0}{1}{0}", quote, s);

            // page 6 of https://wiki.mozilla.org/images/6/6f/Phpmyadmin-report.pdf
            // to suppress potential harmful excel formulas add a leading space to the formulas chars
            s = s.Replace("=", " =")
                .Replace("-", " -")
                .Replace("\"", " \"")
                .Replace("@", " @")
                .Replace("+", " +")
                .Replace("\t", " ");
            return s;
        }
    }
}
