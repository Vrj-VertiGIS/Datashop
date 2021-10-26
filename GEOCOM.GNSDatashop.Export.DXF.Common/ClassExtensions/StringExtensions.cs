using System.Text;
using System.Text.RegularExpressions;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class StringExtensions
    {
        private static Regex _tagRemovalRegEx = null;
        private static Regex _normalizationRegEx = null;

        public static string DxfCompatibleName(this string name, bool replaceWhiteSpaces=false)
        {
            var escapedStr = new StringBuilder((replaceWhiteSpaces) ? name.Trim() : name);

            foreach (var cStr in TableObject.InvalidCharacters)
                escapedStr.Replace(cStr, @"_");

            if (replaceWhiteSpaces)
                escapedStr.Replace(' ', '_');

            return escapedStr.ToString();
        }

        /// <summary>
        /// Remove HTML-/XML-Tags from String by RegEx
        /// Not a perfect solution - but probably suitable for the job
        /// as in-text braces &lt; or &gt; will corrupt the replacement.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveXmlTags(this string input)
        {
            var tagsRemoved = TagRemovalRegEx.Replace(input, string.Empty);
            var normalized = NormalizationRegEx.Replace(tagsRemoved, string.Empty);
            return normalized;
        }

        private static Regex TagRemovalRegEx
        {
            get
            {
                if (null == _tagRemovalRegEx)
                    _tagRemovalRegEx = new Regex(@"<[^>]*(>)");
                return _tagRemovalRegEx;
            }
        }

        private static Regex NormalizationRegEx
        {
            get
            {
                if (null == _normalizationRegEx)
                    // _normalizationRegEx = new Regex(@"[\s\r\n]+");
                    _normalizationRegEx = new Regex(@"[\r\n]+");
                return _normalizationRegEx;
            }
        }

    }

}
