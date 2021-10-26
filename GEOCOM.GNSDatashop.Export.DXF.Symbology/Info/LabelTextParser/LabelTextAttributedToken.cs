using System;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public abstract class LabelTextAttributedToken : LabelTextToken
    {
        private LabelTextTokenizer _tokenizer;

        protected LabelTextAttributedToken(string text)
        {
            Text = text;
            _tokenizer = new LabelTextTokenizer(Text);
        }

        protected string GetStringAttribute(string attributeName)
        {
            int position = Text.IndexOf(attributeName, StringComparison.OrdinalIgnoreCase);
            if (0 <= position)
            {
                string value = string.Empty;
                position += attributeName.Length;
                _tokenizer.TrySkipOperator('=', ref position);  // Might be name is separated by = - i.e. font = "arial" or font "arial"
                if (_tokenizer.TryGetQuotedString(ref position, ref value))
                    return value;
            }
            return string.Empty;
        }

        /// <summary>
        ///  Integer attribute - name="1" or name=1
        /// </summary>
        /// <param name="attributeName">Name of expected/requested attribute</param>
        /// <returns>Attribute value if numeric value faund, null otherwise</returns>
        protected int? GetIntegerAttribute(string attributeName)
        {
            var asString = GetStringAttribute(attributeName);
            return string.IsNullOrEmpty(asString)
                ? null
                : int.TryParse(asString, out int value)
                    ? value
                    : (int?)null;
        }

        /// <summary>
        /// Floating point attribute - Size = "12.34" or Size = 12.34
        /// </summary>
        /// <param name="attributeName">Name of expected/requested attribute</param>
        /// <returns>Attribute value if numeric null, otherwise</returns>
        protected double? GetFloatAttribute(string attributeName)
        {
            var asString = GetStringAttribute(attributeName);
            return string.IsNullOrEmpty(asString)
                ? null
                : double.TryParse(asString, out double value)
                    ? value
                    : (double?)null;

        }
    }
}
