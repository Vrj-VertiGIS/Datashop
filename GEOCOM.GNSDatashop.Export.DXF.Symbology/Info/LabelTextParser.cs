using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LabelTextParser
    {
        public string Text { get; private set; }

        private LabelTextTokenizer _tokenizer;

        private int _position = 0;

        public LabelTextParser(string text)
        {
            Text = text;
        }

        public IEnumerable<LabelTextToken> Tokens
        {
            get
            {
                _tokenizer = new LabelTextTokenizer(Text);

                string token = string.Empty;
                _position = 0;
                while (_tokenizer.Length > _position)
                {
                    var c = Text[_position];
                    if ((c == '<') && _tokenizer.TryGetTag(ref _position, ref token))
                        yield return LabelTextToken.CreateTag(token);
                    else if (((c == '"') || (c == '\'')) && _tokenizer.TryGetQuotedString(ref _position, ref token))
                        yield return LabelTextToken.Create(token);
                    else if ((c == '\n') || (c == '\r') || (c == '\t'))  // Single-character escaping
                        yield return LabelTextToken.CreateEscaping(Text[_position++]);
                    else if ((c == '\\') && _tokenizer.TryGetEscapeString(ref _position, ref token))
                        yield return LabelTextToken.CreateEscaping(token);    // Symbolic - not yet interpreted - escaping
                    else
                    {
                        _tokenizer.GetElementString(ref _position, ref token);
                        yield return LabelTextToken.Create(token);
                    }
                }
            }
        }
    }

    public class LabelTextTokenizer
    {
        public string Text { get; private set; }
        public int Length { get; private set; } = 0;

        public LabelTextTokenizer(string text)
        {
            Text = text;
            Length = text.Length;
        }

        public bool TryGetQuotedString(ref int position, ref string value)
        {
            int i = position;
            if (TrySkipQuotedString(ref i))
            {
                value = Text.Substring(position + 1, i - position - 2);   // Omit quotes
                position = i;
                return true;
            }
            return false;
        }


        public bool TryGetTag(ref int position, ref string token)
        {
            int i = position;
            if (TrySkipTag(ref i))
            {
                token = Text.Substring(position, i - position);
                position = i;
                return true;
            }
            return false;
        }

        public void GetElementString(ref int position, ref string element)
        {
            int i = position++;
            SkipElementString(ref position);
            element = Text.Substring(i, position-i);
        }

        public bool TryGetEscapeString(ref int position, ref string escaping)
        {
            int i = position;
            if (TrySkipEscaping(ref i))
            {
                escaping = Text.Substring(position, i - position);
                position = i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip a text literal put inside quotation marks. Look for the closing opposite
        /// quoitation mark and return true if found. In thsi case return position just
        /// one character after the closing quotation mark. If not found, remain at the
        /// initial position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipQuotedString(ref int position)
        {
            var quotationMark = Text[position];
            int i;
            for (i = position + 1; (Length > i) && (Text[i] != quotationMark); i++) ;
            if (Length > i)
            {
                position = ++i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip a tag. Assume Text[position] points to an opening tag brace.
        /// Try to find a closing brace while skipping any text character and
        /// skipping text literals put inside quotation marks.
        /// if found a closing bracket, return true and place position at the 
        /// character just after the closing bracket.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipTag(ref int position)
        {
            int i = position + 1;
            while ((Length > i) && (Text[i] != '>'))
            {
                var c = Text[i];
                if ((c != '"') && (c != '\''))
                    i++;
                else if (!TrySkipQuotedString(ref i))
                    i++;
            }
            if (Length > i)
            {
                position = ++i;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip element text (text not part of a tag)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SkipElementString(ref int position)
        {
            bool atEndOfElement = false;
            while ((Length > position) && (!atEndOfElement))
            {
                var c = Text[position];
                if ((c == '\r') || (c == '\n') || (c == '\t'))
                    atEndOfElement = true;
                else if (c == '<')
                    if ((Length > position + 1) && (Text[position + 1] == '<'))
                        position += 2;  // Skip entire < pair
                    else
                        atEndOfElement = true;
                else if (c == '\\')
                    if ((Length > (position + 1)) && (Text[position + 1] == '\''))
                        position += 2; // Skip entire \ pair
                    else
                        atEndOfElement = true;
                else if ((c == '"') || (c == '\''))
                {
                    if (!(atEndOfElement = TrySkipQuotedString(ref position)))
                        position++;    // Orphaned " or ' - skip to next character anyways
                }
                else
                    position++;
            }
        }

        /// <summary>
        /// Skip character escaping
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TrySkipEscaping(ref int position)
        {
            int i = position + 1;
            if (Length > i)
                if (Text[i] == '\\')
                {
                    if ((Length <= i + 1) || (Text[i + 1] != '\\'))
                    // We're on a escaing of the escape character : \\
                    {
                        position = i + 1;
                        return true;
                    }
                }
                else if (IsAThruZ(Text[i]))
                // Escaping \n, \p, \P...
                {
                    position = i + 1;
                    return true;
                }
                else if ((Length > i + 2) && Is0Thru9(Text[i]) && Is0Thru9(Text[i + 1]) && Is0Thru9(Text[i + 2]))
                {
                    position = i + 3;
                    return true;
                }

            return false;
        }

        private bool IsAThruZ(char c) => (('a' <= c) && ('z' >= c)) || (('A' <= c) && ('Z' >= c));
        private bool Is0Thru9(char c) => ('0' <= c) && ('9' >= c);
    }

    public enum LabelTextTokenType
    {
        TagBold, TagITalic, TagUnderline,
        TagEndBold, TagEndItalic, TagEndUnderline,
        TagNotBold, TagNotItalic, TagNotUnderline,
        TagEndNotBold, TagEndNotItalic, TagEndNotUnderline,
        TagFont,
        TagEndFont,
        ESCNewline,
        ESCCarriageReturn,
        ESCTab,
        Element
    }

    public enum LabelTextTokenClass
    {
        OpeningTag, ClosingTag,
        Escaping,
        Element
    }

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
                default:   return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.Element };
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
            {
                case "<bol>":
                case "<b>": return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.TagBold, TokenClass= LabelTextTokenClass.OpeningTag };
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

                default:
                    if (lowerToken.StartsWith("<fnt"))
                        return new LabelTextFontToken(token);
                    else
                        return new LabelTextToken() { Text = token, TokenType = LabelTextTokenType.Element, TokenClass = LabelTextTokenClass.OpeningTag };
            }
        }
    }

    public class LabelTextFontToken : LabelTextToken
    {
        private LabelTextTokenizer _tokenizer;
        public LabelTextFontToken(string text)
        {
            TokenType = LabelTextTokenType.TagFont;
            Text = text;
            _tokenizer = new LabelTextTokenizer(Text);
            Size = GetAttributeValue(@"size=");
            // We also found that the font name contains the bold and italic modifier which
            // mainly represents a font face rather than an own font family
            // a bold font family is usually named "black" - i.e. "Arial Black"
            // Set Bold and Italic properties accordingly and remove modifiers from name 
            // in the hope we get the correct family name
            var fullName = GetAttributeValue(@"name=");
            var boldIndex = fullName.IndexOf(@" bold", StringComparison.OrdinalIgnoreCase);
            Bold = (0 <= boldIndex);
            var italicIndex = fullName.IndexOf(@" italic", StringComparison.OrdinalIgnoreCase);
            Italic = (0 <= italicIndex);

            Name = (Bold)  // Bold
                ? (Italic) // Bold and Italic
                    ? (boldIndex < italicIndex) ? fullName.Substring(0, boldIndex) : fullName.Substring(0, italicIndex)
                    : fullName.Substring(0, boldIndex)
                : (Italic)  // Not bold
                    ? fullName.Substring(0, italicIndex)    // but italic
                    : fullName;                             // and not talic: regular
        }

        /// <summary>
        /// Font name. Erroneously there is a naming of "Bold" or "Italic" when the respective font face is ment - which is
        /// controlled by the respective font settings attribute.
        /// </summary>
        public string Name { get; private set; }

        public string Size { get; private set; }

        public bool Bold { get; private set; }
        public bool Italic { get; private set; }

        private string GetAttributeValue(string attributeName)
        {
            int position = Text.IndexOf(attributeName, StringComparison.OrdinalIgnoreCase);
            if (0 <= position)
            {
                string value = string.Empty;
                position += attributeName.Length;
                if (_tokenizer.TryGetQuotedString(ref position, ref value))
                    return value;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Make shure the embedded enumerator's Current property returns
    /// null in case the enumerator is not on a valid poisiton 
    /// (before the first or after the last element)
    /// </summary>
    public class LabelTextTokens : IEnumerator<LabelTextToken>
    {
        private IEnumerator<LabelTextToken> _tokens;
        private bool _validData = false;
        public LabelTextTokens(IEnumerator<LabelTextToken> tokens)
        {
            _tokens = tokens;
            _validData = _tokens.MoveNext();
        }

        public LabelTextToken Current => (_validData) ? _tokens.Current : null;

        object IEnumerator.Current => (_validData) ? ((IEnumerator)_tokens).Current : null;

        public void Dispose()
        {
            _tokens.Dispose();
        }

        public bool MoveNext()
        {
            return (_validData) ? _validData = _tokens.MoveNext() : false;
        }

        public void Reset()
        {
            throw new NotImplementedException("Since this is a dynamic collection created by cield return... - Reset() is not implemented");
        }

        public string GetFormattedParagraphs(string Font, bool bold, bool italic, bool underline)
        {
            StringBuilder sb = new StringBuilder();
            while ((null != Current) && (Current.TokenClass != LabelTextTokenClass.ClosingTag))
            {
                sb.Append(GetFormattedParagraph(Font, bold, italic, underline));
                MoveNext();
            }

            return sb.ToString();
        }

        private string GetFormattedParagraph(string Font, bool bold, bool italic, bool underline)
        {
            var current = Current;
            if (null != current)
                switch (current.TokenType)
                {
                    case LabelTextTokenType.Element:
                        return current.Text;
                    case LabelTextTokenType.ESCNewline:
                        return @"\P";
                    case LabelTextTokenType.TagBold:
                        return EscFont(Font, true, italic)
                            + ((MoveNext())
                                ? GetFormattedParagraphs(Font, true, italic, underline)
                                : string.Empty)
                            + EscFont(Font, bold, italic);
                    case LabelTextTokenType.TagITalic:
                        return EscFont(Font, bold, true)
                            + ((MoveNext())
                                ? GetFormattedParagraphs(Font, bold, true, underline)
                                : string.Empty)
                            + EscFont(Font, bold, italic);
                    case LabelTextTokenType.TagUnderline:
                        return @"\L"
                            + ((MoveNext())
                                ? GetFormattedParagraphs(Font, bold, italic, true)
                                : string.Empty)
                            + ((underline) ? @"\L" : @"\l");
                    case LabelTextTokenType.TagFont:
                        var currentFont = current as LabelTextFontToken;
                        // Override the bold/italic attribute only if they are set to true. If not set, continue with
                        // the respective font attribute from before the <FNT> tag.
                        return EscFont(currentFont.Name, currentFont.Bold ? true : bold, currentFont.Italic ? true : italic)
                            + ((MoveNext())
                                ? GetFormattedParagraphs(currentFont.Name, bold, italic, underline)
                                : string.Empty)
                            + EscFont(Font, bold, italic);
                }
            return string.Empty;
        }

        private static string EscFont(string fontName, bool bold, bool italic)
        {
            var acp = WinAPI.GetACP();      // Get active code page

            return @"\f" + string.Format("{0}|b{1}|i{2}|c{3}|p{4};", fontName, (bold) ? 1 : 0, (italic) ? 1 : 0, acp, 0);
        }
        private static string Esc(bool set, string marker)
        {
            return (set) ? marker : string.Empty;
        }

    }
}
