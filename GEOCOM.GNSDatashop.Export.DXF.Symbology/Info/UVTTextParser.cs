using System;
using System.Collections;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class UVTTextParser
    {
        public string Text { get; private set; }

        private UVTTokenizer _tokenizer;

        private int _position = 0;

        public UVTTextParser(string text)
        {
            Text = text;
        }

        public IEnumerable<UVTToken> Tokens
        {
            get
            {
                _tokenizer = new UVTTokenizer(Text);

                string token = string.Empty;
                _position = 0;
                while (_tokenizer.Length > _position)
                {
                    var c = Text[_position];
                    if ((c == '<') && _tokenizer.TryGetTag(ref _position, ref token))
                        yield return UVTToken.CreateTag(token);
                    else if (((c == '"') || (c == '\'')) && _tokenizer.TryGetQuotedString(ref _position, ref token))
                        yield return UVTToken.Create(token);
                    else if ((c == '\n') || (c == '\r') || (c == '\t'))  // Single-character escaping
                        yield return UVTToken.CreateEscaping(Text[_position++]);
                    else if ((c == '\\') && _tokenizer.TryGetEscapeString(ref _position, ref token))
                        yield return UVTToken.CreateEscaping(token);    // Symbolic - not yet interpreted - escaping
                    else
                    {
                        _tokenizer.GetElementString(ref _position, ref token);
                        yield return UVTToken.Create(token);
                    }
                }
            }
        }
    }

    public class UVTTokenizer
    {
        public string Text { get; private set; }
        public int Length { get; private set; } = 0;

        public UVTTokenizer(string text)
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
            int i = position;
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

    public enum UVTTokenType
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

    public enum UVTTokenClass
    {
        OpeningTag, ClosingTag,
        Escaping,
        Element
    }

    public class UVTToken
    {
        public string Text { get; protected set; }
        public UVTTokenType TokenType { get; protected set; }
        public UVTTokenClass TokenClass { get; protected set; }

        public static UVTToken Create(string token)
        {
            return new UVTToken() { Text = token, TokenType = UVTTokenType.Element, TokenClass = UVTTokenClass.Element };
        }

        public static UVTToken CreateEscaping(string token)
        {
            var lowerToken = token.ToLower();
            switch (lowerToken)
            {
                case "\n": return new UVTToken() { Text = token, TokenType = UVTTokenType.ESCNewline, TokenClass = UVTTokenClass.Escaping };
                case "\r": return new UVTToken() { Text = token, TokenType = UVTTokenType.ESCCarriageReturn, TokenClass = UVTTokenClass.Escaping };
                case "\t": return new UVTToken() { Text = token, TokenType = UVTTokenType.ESCTab, TokenClass = UVTTokenClass.Escaping };
                default:   return new UVTToken() { Text = token, TokenType = UVTTokenType.Element, TokenClass = UVTTokenClass.Element };
            }
        }
        /// <summary>
        /// Single character escaping ('\r', '\n', '\t',...)
        /// </summary>
        /// <param name="escChar"></param>
        /// <returns></returns>
        public static UVTToken CreateEscaping(char escChar)
        {
            switch (escChar)
            {
                case '\n': return new UVTToken() { Text = string.Empty, TokenType = UVTTokenType.ESCNewline, TokenClass = UVTTokenClass.Escaping };
                case '\r': return new UVTToken() { Text = string.Empty, TokenType = UVTTokenType.ESCCarriageReturn, TokenClass = UVTTokenClass.Escaping };
                case '\t': return new UVTToken() { Text = string.Empty, TokenType = UVTTokenType.ESCTab, TokenClass = UVTTokenClass.Escaping };
                default: return new UVTToken() { Text = string.Empty, TokenType = UVTTokenType.Element, TokenClass = UVTTokenClass.Element };
            }
        }

        public static UVTToken CreateTag(string token)
        {
            var lowerToken = token.ToLower();
            switch (lowerToken)
            {
                case "<bol>":
                case "<b>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagBold, TokenClass= UVTTokenClass.OpeningTag };
                case "</bol>":
                case "</b>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndBold, TokenClass = UVTTokenClass.ClosingTag };
                case "<_bol>":
                case "<_b>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagNotBold, TokenClass = UVTTokenClass.OpeningTag };
                case "</_bol>":
                case "</_b>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndNotBold, TokenClass = UVTTokenClass.ClosingTag };
                case "<i>":
                case "<ita>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagITalic, TokenClass = UVTTokenClass.OpeningTag };
                case "</i>":
                case "</ita>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndItalic, TokenClass = UVTTokenClass.ClosingTag };
                case "<_i>":
                case "<_ita>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagNotItalic, TokenClass = UVTTokenClass.OpeningTag };
                case "</_i>":
                case "</_ita>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndNotItalic, TokenClass = UVTTokenClass.ClosingTag };
                case "<u>":
                case "<und>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagUnderline, TokenClass = UVTTokenClass.OpeningTag };
                case "</u>":
                case "</und>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndUnderline, TokenClass = UVTTokenClass.ClosingTag };
                case "<_u>":
                case "<_und>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagNotUnderline, TokenClass = UVTTokenClass.OpeningTag };
                case "</_u>":
                case "</_und>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndNotUnderline, TokenClass = UVTTokenClass.ClosingTag };
                case "</fnt>": return new UVTToken() { Text = token, TokenType = UVTTokenType.TagEndFont, TokenClass = UVTTokenClass.ClosingTag };

                default:
                    if (lowerToken.StartsWith("<fnt"))
                        return new UVTFontToken(token);
                    else
                        return new UVTToken() { Text = token, TokenType = UVTTokenType.Element, TokenClass = UVTTokenClass.OpeningTag };
            }
        }
    }

    public class UVTFontToken : UVTToken
    {
        private UVTTokenizer _tokenizer;
        public UVTFontToken(string text)
        {
            TokenType = UVTTokenType.TagFont;
            Text = text;
            _tokenizer = new UVTTokenizer(Text);
        }

        /// <summary>
        /// Font name. Erroneously there is a naming of "Bold" or "Italic" when the respective font face is ment - which is
        /// controlled by the respective font settings attribute.
        /// </summary>
        public string Name => GetAttributeValue("name=").Replace(" Bold", string.Empty).Replace(" Italic", string.Empty);

        public string Size => GetAttributeValue("size=");

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
    public class UVTTokens : IEnumerator<UVTToken>
    {
        private IEnumerator<UVTToken> _tokens;
        private bool _validData = false;
        public UVTTokens(IEnumerator<UVTToken> tokens)
        {
            _tokens = tokens;
            _validData = _tokens.MoveNext();
        }

        public UVTToken Current => (_validData) ? _tokens.Current : null;

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
    }


}
