using System;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public class LabelTextFontToken : LabelTextAttributedToken
    {
        public LabelTextFontToken(string text)
            : base(text)
        {
            TokenType = LabelTextTokenType.TagFont;
            _height = GetFloatAttribute(@"size");
            _scale = GetIntegerAttribute(@"scale");
            // We also found that the font name contains the bold and italic modifier which
            // mainly represents a font face rather than an own font family
            // a bold font family is usually named "black" - i.e. "Arial Black"
            // Set Bold and Italic properties accordingly and remove modifiers from name 
            // in the hope we get the correct family name
            var fullName = GetStringAttribute(@"name");
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

        private double? _height;
        public double? Height => _height;

        private int? _scale;
        public int? Scale => _scale;

        public bool Bold { get; private set; }
        public bool Italic { get; private set; }
    }
}
