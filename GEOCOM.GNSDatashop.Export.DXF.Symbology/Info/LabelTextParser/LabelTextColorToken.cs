namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser
{
    public class LabelTextColorToken : LabelTextAttributedToken
    {
        public LabelTextColorToken(string text)
            : base(text)
        {
            TokenType = LabelTextTokenType.TagColor;
            ColorSpace = TryGetRGBValue(ref _value)
                ? ColorSpace.RGB
                : TryGetCMYKValues(ref _value)
                    ? ColorSpace.CMYK
                    : ColorSpace.None;
        }

        private bool TryGetCMYKValues(ref int rgb)
        {
            int? c = null, m = null, y = null, k = null;
            var f = (c = GetIntegerAttribute("cyan")).HasValue
                && (m = GetIntegerAttribute("magenta")).HasValue
                && (y = GetIntegerAttribute("yellow")).HasValue
                && (k = GetIntegerAttribute("black")).HasValue;

            rgb = (f)
                ? RGB(c.Value, m.Value, y.Value, k.Value)
                : 0;

            return f;
        }

        private bool TryGetRGBValue(ref int rgb)
        {
            int? r = null, g = null, b = null;
            var f = (r = GetIntegerAttribute("red")).HasValue
                && (g = GetIntegerAttribute("green")).HasValue
                && (b = GetIntegerAttribute("blue")).HasValue;

            rgb = (f)
                ? RGB(r.Value, g.Value, b.Value)
                : 0;

            return f;
        }

        private int RGB(int c, int m, int y, int k)
        {
            var k1 = (100 - k) / 100;
            return RGB(255 * (100 - c) / 100 * k1,
                       255 * (100 - m) / 100 * k1,
                       255 * (100 - y) / 100 * k1);
        }

        private int RGB(int r, int g, int b)
            => (r << 16) | (g << 8) | b;

        public ColorSpace ColorSpace { get; private set; }

        private int _value;     // Allow use as ref parameter
        public int Value => _value;
    }

}
