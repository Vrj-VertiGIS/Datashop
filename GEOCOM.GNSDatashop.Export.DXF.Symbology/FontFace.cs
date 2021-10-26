using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GEOCOM.GNSDatashop.Export.DXF.Common;

using log4net;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class FontFace : IDisposable
    {
        private string _familyName = string.Empty;
        private float _fontSize = 0f;
        private FontStyle _fontStyle = FontStyle.Regular;
        private readonly double _characterWidth = 100;       // Given in %

        private FontFamily _family = null;   // On-demand reference to the font's family
        private Font _font = null;           // On-demand reference to the font's Font object

        private static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        public FontFace(string name, float size, FontStyle style=FontStyle.Regular, double characterWidth=100)
        {
            _familyName = name;
            _fontSize = size;
            _fontStyle = style;
            _characterWidth = characterWidth;
        }

        public void Dispose()
            => DisposeManaged();

        private void DisposeManaged()
        {
            _family?.Dispose();
            _font?.Dispose();
        }

        public FontStyle FontStyle => _fontStyle;

        public IEnumerable<string> FontStyleStrings
        {
            get
            {
                if (_fontStyle == FontStyle.Regular)
                    // Return "Regular" if it's the only style set
                    yield return Enum.GetName(typeof(FontStyle), FontStyle.Regular);
                else
                    foreach (var style in Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>().Except(new List<FontStyle>() { FontStyle.Regular }))
                    {
                        if (style == (_fontStyle & style))
                            yield return Enum.GetName(typeof(FontStyle), style);
                    }

            }
        }

        public override string ToString()
        {
            var faceName = new StringBuilder(_familyName);

            faceName.Append("_");
            faceName.Append(_fontSize.ToString());

            faceName.Append("_");
            faceName.Append(string.Join("_", FontStyleStrings));

            faceName.Append("_");
            faceName.AppendFormat("|CW{0}|", _characterWidth);

            return faceName.ToString();
        }

        public FontFamily FontFamily
        {
            get
            {
                if (null == _family)
                    _family = new FontFamily(_familyName);

                return _family;
            }
        }

        public virtual Font Font
        {
            get
            {
                if (null == _font)
                    _font = new Font(FontFamily, (float)_fontSize, FontStyle, GraphicsUnit.Point);

                return _font;
            }
        }

    }
}