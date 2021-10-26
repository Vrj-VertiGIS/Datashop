namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public class LabelTextToken
    {
        public string Text { get; protected set; }
        public LabelTextTokenType TokenType { get; protected set; }
        public LabelTextTokenClass TokenClass { get; protected set; }

        public static LabelTextToken Create(string token)
        {
            return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.Element };
        }

        public static LabelTextToken CreateEscaping(string token)
        {
            var lowerToken = token.ToLower();
            switch (lowerToken)
            {
                case "\n": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.ESCNewline, TokenClass = LabelTextTokenClass.Escaping };
                case "\r": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.ESCCarriageReturn, TokenClass = LabelTextTokenClass.Escaping };
                case "\t": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.ESCTab, TokenClass = LabelTextTokenClass.Escaping };
                default: return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.Element };
            }
        }
        /// <summary>
        /// Single character escaping ('\r', '\n', '\t',...)
        /// </summary>
        /// <param name="escChar"></param>
        /// <returns></returns>
        public static LabelTextToken CreateEscaping(char escChar)
        {
            switch (escChar)
            {
                case '\n': return new LabelTextToken() { Text = string.Empty, TokenType = LabelTextTokenType.ESCNewline, TokenClass = LabelTextTokenClass.Escaping };
                case '\r': return new LabelTextToken() { Text = string.Empty, TokenType = LabelTextTokenType.ESCCarriageReturn, TokenClass = LabelTextTokenClass.Escaping };
                case '\t': return new LabelTextToken() { Text = string.Empty, TokenType = LabelTextTokenType.ESCTab, TokenClass = LabelTextTokenClass.Escaping };
                default: return new LabelTextToken() { Text = string.Empty, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.Element };
            }
        }

        public static LabelTextToken CreateTag(string token)
        {
            var lowerToken = token.ToLower();
            switch (lowerToken)
            {   // Simple tokens - precise match
                case "<bol>":
                case "<b>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagBold, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</bol>":
                case "</b>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndBold, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<_bol>":
                case "<_b>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagNotBold, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</_bol>":
                case "</_b>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndNotBold, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<i>":
                case "<ita>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagITalic, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</i>":
                case "</ita>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndItalic, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<_i>":
                case "<_ita>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagNotItalic, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</_i>":
                case "</_ita>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndNotItalic, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<u>":
                case "<und>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagUnderline, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</u>":
                case "</und>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndUnderline, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<_u>":
                case "<_und>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagNotUnderline, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</_u>":
                case "</_und>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndNotUnderline, TokenClass = LabelTextTokenClass.ClosingTag };
                case "</fnt>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndFont, TokenClass = LabelTextTokenClass.ClosingTag };
                case "</clr>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndColor, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<sup>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagSuperScript, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</sup>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndSuperScript, TokenClass = LabelTextTokenClass.ClosingTag };
                case "<sub>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagSubScript, TokenClass = LabelTextTokenClass.OpeningTag };
                case "</sub>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagEndSubScript, TokenClass = LabelTextTokenClass.ClosingTag };

                default:
                    // Complex tokens - i.e. tokens with attributes
                    if (lowerToken.StartsWith("<fnt"))
                        return new LabelTextFontToken(token);
                    else if (lowerToken.StartsWith("<clr"))
                        return new LabelTextColorToken(token);
                    else
                        return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.OpeningTag };
            }
        }
    }
}
