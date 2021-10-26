using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
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
}
