using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace GEOCOM.GNSD.Web.Core.Security.WebCaptcha
{
    /// <summary>
    /// CAPTCHA image generation class
    /// </summary>
    /// <remarks>
    /// Adapted from the excellent code at 
    /// http://www.codeproject.com/aspnet/CaptchaImage.asp
    ///
    /// Jeff Atwood
    /// http://www.codinghorror.com/
    /// </remarks>
    public class CaptchaImage
    {
        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private int height;

        /// <summary>
        /// 
        /// </summary>
        private int width;

        /// <summary>
        /// 
        /// </summary>
        private readonly Random rand;

        /// <summary>
        /// 
        /// </summary>
        private int randomTextLength;

        /// <summary>
        /// 
        /// </summary>
        private string randomTextChars;

        /// <summary>
        /// 
        /// </summary>
        private string fontFamilyName;

        /// <summary>
        /// 
        /// </summary>
        private string[] fontFamilyNames;

        #endregion

        #region Public Enums

        /// <summary>
        /// Amount of random font warping to apply to rendered text
        /// </summary>
        public enum FontWarpFactor
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }

        /// <summary>
        /// Amount of background noise to add to rendered image
        /// </summary>
        public enum BackgroundNoiseLevel
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }

        /// <summary>
        /// Amount of curved line noise to add to rendered image
        /// </summary>
        public enum LineNoiseLevel
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a GUID that uniquely identifies this Captcha
        /// </summary>
        public string UniqueId { get; private set; }

        /// <summary>
        /// Returns the date and time this image was last rendered
        /// </summary>
        public DateTime RenderedAt { get; private set; }

        /// <summary>
        /// Font family to use when drawing the Captcha text. If no font is provided, a random font will be chosen from the font whitelist for each character.
        /// </summary>
        public string Font
        {
            get { return fontFamilyName; }
            set
            {
                try
                {
                    using (new Font(value, 12f)) 
                        fontFamilyName = value;
                }
                catch (Exception)
                {
                    fontFamilyName = FontFamily.GenericSerif.Name;
                }
            }
        }

        /// <summary>
        /// Amount of random warping to apply to the Captcha text.
        /// </summary>
        /// <value>
        /// The font warp.
        /// </value>
        public FontWarpFactor FontWarp { get; set; }

        /// <summary>
        /// Amount of background noise to apply to the Captcha image.
        /// </summary>
        /// <value>
        /// The background noise.
        /// </value>
        public BackgroundNoiseLevel BackgroundNoise { get; set; }

        /// <summary>
        /// Gets or sets the line noise.
        /// </summary>
        /// <value>
        /// The line noise.
        /// </value>
        public LineNoiseLevel LineNoise { get; set; }

        /// <summary>
        /// A semicolon-delimited list of valid fonts to use when no font is provided.
        /// </summary>
        /// <value>
        /// The font whitelist.
        /// </value>
        public string FontWhitelist { get; set; }

        /// <summary>
        /// Returns the randomly generated Captcha text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// A string of valid characters to use in the Captcha text. 
        /// A random character will be selected from this string for each character.
        /// </summary>
        public string TextChars
        {
            get { return randomTextChars; }
            set
            {
                randomTextChars = value;
                Text = GenerateRandomText();
            }
        }

        /// <summary>
        /// Number of characters to use in the Captcha text. 
        /// </summary>
        public int TextLength
        {
            get { return randomTextLength; }
            set
            {
                randomTextLength = value;
                Text = GenerateRandomText();
            }
        }

        /// <summary>
        /// Width of Captcha image to generate, in pixels 
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                if ((value <= 60))
                    throw new ArgumentOutOfRangeException("width", value, "width must be greater than 60.");

                width = value;
            }
        }

        /// <summary>
        /// Height of Captcha image to generate, in pixels 
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                if (value <= 30)
                    throw new ArgumentOutOfRangeException("height", value, "height must be greater than 30.");

                height = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaImage"/> class.
        /// </summary>
        public CaptchaImage()
        {
            this.rand = new Random();
            this.FontWarp = FontWarpFactor.Low;
            this.BackgroundNoise = BackgroundNoiseLevel.Low;
            this.LineNoise = LineNoiseLevel.None;
            this.width = 180;
            this.height = 50;
            this.randomTextLength = 5;
            this.randomTextChars = "ACDEFGHJKLNPQRTUVXYZ2346789";
            this.fontFamilyName = "";
            // -- a list of known good fonts in on both Windows XP and Windows Server 2003
            this.FontWhitelist = "arial;arial black;comic sans ms;courier new;estrangelo edessa;franklin gothic medium;" + "georgia;lucida console;lucida sans unicode;mangal;microsoft sans serif;palatino linotype;" + "sylfaen;tahoma;times new roman;trebuchet ms;verdana";
            this.Text = this.GenerateRandomText();
            this.RenderedAt = DateTime.Now;
            this.UniqueId = Guid.NewGuid().ToString();
        } 

        #endregion

        #region Public methods
        
        /// <summary>
        /// Forces a new Captcha image to be generated using current property value settings.
        /// </summary>
        public Bitmap RenderImage()
        {
            return this.GenerateImagePrivate();
        } 

        #endregion
        
        #region Private methods

        /// <summary>
        /// Returns a random font family from the font whitelist
        /// </summary>
        private string RandomFontFamily()
        {
            //-- small optimization so we don't have to split for each char
            if (this.fontFamilyNames == null)
                this.fontFamilyNames = this.FontWhitelist.Split(';');

            return this.fontFamilyNames[this.rand.Next(0, this.fontFamilyNames.Length)];
        }

        /// <summary>
        /// generate random text for the CAPTCHA
        /// </summary>
        private string GenerateRandomText()
        {
            var sb = new StringBuilder(this.randomTextLength);
            var maxLength = this.randomTextChars.Length;

            for (var n = 0; n <= this.randomTextLength - 1; n++)
                sb.Append(this.randomTextChars.Substring(rand.Next(maxLength), 1));

            return sb.ToString();
        }

        /// <summary>
        /// Returns a random point within the specified x and y ranges
        /// </summary>
        private PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF(this.rand.Next(xmin, xmax), this.rand.Next(ymin, ymax));
        }

        /// <summary>
        /// Returns a random point within the specified rectangle
        /// </summary>
        private PointF RandomPoint(Rectangle rect)
        {
            return this.RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom);
        }

        /// <summary>
        /// Returns a GraphicsPath containing the specified string and font
        /// </summary>
        private GraphicsPath TextPath(string s, Font f, Rectangle r)
        {
            var sf = new StringFormat
                         {
                             Alignment = StringAlignment.Near, 
                             LineAlignment = StringAlignment.Near
                         };

            var gp = new GraphicsPath();
            gp.AddString(s, f.FontFamily, Convert.ToInt32(f.Style), f.Size, r, sf);

            return gp;
        }

        /// <summary>
        /// Returns the CAPTCHA font in an appropriate size 
        /// </summary>
        private Font GetFont()
        {
            float fsize = 0;
            
            switch (this.FontWarp)
            {
                case FontWarpFactor.None:
                    fsize = Convert.ToInt32(height * 0.7);
                    break;
                case FontWarpFactor.Low:
                    fsize = Convert.ToInt32(height * 0.8);
                    break;
                case FontWarpFactor.Medium:
                    fsize = Convert.ToInt32(height * 0.85);
                    break;
                case FontWarpFactor.High:
                    fsize = Convert.ToInt32(height * 0.9);
                    break;
                case FontWarpFactor.Extreme:
                    fsize = Convert.ToInt32(height * 0.95);
                    break;
            }

            var fname = string.IsNullOrEmpty(this.fontFamilyName) ? this.RandomFontFamily() : this.fontFamilyName;

            return new Font(fname, fsize, FontStyle.Bold);
        }

        /// <summary>
        /// Renders the CAPTCHA image
        /// </summary>
        private Bitmap GenerateImagePrivate()
        {
            Font fnt = null;
            Rectangle rect = default(Rectangle);
            Brush br;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics gr = Graphics.FromImage(bmp);
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            //-- fill an empty white rectangle
            rect = new Rectangle(0, 0, width, height);
            br = new SolidBrush(Color.White);
            gr.FillRectangle(br, rect);

            int charOffset = 0;
            double charWidth = width / randomTextLength;
            Rectangle rectChar = default(Rectangle);

            foreach (char c in Text)
            {
                //-- establish font and draw area
                fnt = GetFont();
                rectChar = new Rectangle(Convert.ToInt32(charOffset * charWidth), 0, Convert.ToInt32(charWidth), height);

                //-- warp the character
                GraphicsPath gp = TextPath(c.ToString(), fnt, rectChar);
                WarpText(gp, rectChar);

                //-- draw the character
                br = new SolidBrush(Color.Black);
                gr.FillPath(br, gp);

                charOffset += 1;
            }

            AddNoise(gr, rect);
            AddLine(gr, rect);

            //-- clean up unmanaged resources
            fnt.Dispose();
            br.Dispose();
            gr.Dispose();

            return bmp;
        }

        /// <summary>
        /// Warp the provided text GraphicsPath by a variable amount
        /// </summary>
        private void WarpText(GraphicsPath textPath, Rectangle rect)
        {
            float warpDivisor = 0;
            float rangeModifier = 0;

            switch (FontWarp)
            {
                case FontWarpFactor.None:
                    return;
                case FontWarpFactor.Low:
                    warpDivisor = 6;
                    rangeModifier = 1;
                    break;
                case FontWarpFactor.Medium:
                    warpDivisor = 5;
                    rangeModifier = (float)1.3;
                    break;
                case FontWarpFactor.High:
                    warpDivisor = (float)4.5;
                    rangeModifier = (float)1.4;
                    break;
                case FontWarpFactor.Extreme:
                    warpDivisor = 4;
                    rangeModifier = (float)1.5;
                    break;
            }

            var rectF = new RectangleF(Convert.ToSingle(rect.Left), 0, Convert.ToSingle(rect.Width), rect.Height);

            var hrange = Convert.ToInt32(rect.Height / warpDivisor);
            var wrange = Convert.ToInt32(rect.Width / warpDivisor);
            var left = rect.Left - Convert.ToInt32(wrange * rangeModifier);
            var top = rect.Top - Convert.ToInt32(hrange * rangeModifier);
            var width = rect.Left + rect.Width + Convert.ToInt32(wrange * rangeModifier);
            var height = rect.Top + rect.Height + Convert.ToInt32(hrange * rangeModifier);

            if (left < 0)
                left = 0;
            if (top < 0)
                top = 0;
            if (width > this.Width)
                width = this.Width;
            if (height > this.Height)
                height = this.Height;

            var leftTop = this.RandomPoint(left, left + wrange, top, top + hrange);
            var rightTop = this.RandomPoint(width - wrange, width, top, top + hrange);
            var leftBottom = this.RandomPoint(left, left + wrange, height - hrange, height);
            var rightBottom = this.RandomPoint(width - wrange, width, height - hrange, height);

            var points = new[] 
                            {
                                leftTop,
                                rightTop,
                                leftBottom,
                                rightBottom
                            };

            using (var m = new Matrix())
            {
                m.Translate(0, 0);
                textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
            }
        }


        /// <summary>
        /// Add a variable level of graphic noise to the image
        /// </summary>
        private void AddNoise(Graphics graphics, Rectangle rect)
        {
            var density = 0;
            var size = 0;

            switch (this.BackgroundNoise)
            {
                case BackgroundNoiseLevel.None:
                    return;
                case BackgroundNoiseLevel.Low:
                    density = 30;
                    size = 40;
                    break;
                case BackgroundNoiseLevel.Medium:
                    density = 18;
                    size = 40;
                    break;
                case BackgroundNoiseLevel.High:
                    density = 16;
                    size = 39;
                    break;
                case BackgroundNoiseLevel.Extreme:
                    density = 12;
                    size = 38;
                    break;
            }

            using (var br = new SolidBrush(Color.Black))
            {
                var max = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / size);

                for (var i = 0; i <= Convert.ToInt32((rect.Width * rect.Height) / density); i++)
                    graphics.FillEllipse(br, this.rand.Next(rect.Width), this.rand.Next(rect.Height), this.rand.Next(max), this.rand.Next(max));
            }
        }

        /// <summary>
        /// Add variable level of curved lines to the image
        /// </summary>

        private void AddLine(Graphics graphics, Rectangle rect)
        {
            var length = 0;
            float width = 0;
            var linecount = 0;

            switch (this.LineNoise)
            {
                case LineNoiseLevel.None:
                    return;
                case LineNoiseLevel.Low:
                    length = 4;
                    width = Convert.ToSingle(this.height / 31.25);
                    // 1.6
                    linecount = 1;
                    break;
                case LineNoiseLevel.Medium:
                    length = 5;
                    width = Convert.ToSingle(this.height / 27.7777);
                    // 1.8
                    linecount = 1;
                    break;
                case LineNoiseLevel.High:
                    length = 3;
                    width = Convert.ToSingle(this.height / 25);
                    // 2.0
                    linecount = 2;
                    break;
                case LineNoiseLevel.Extreme:
                    length = 3;
                    width = Convert.ToSingle(this.height / 22.7272);
                    // 2.2
                    linecount = 3;
                    break;
            }

            using (var p = new Pen(Color.Black, width))
            {
                var pf = new PointF[length + 1];

                for (var l = 1; l <= linecount; l++)
                {
                    for (var i = 0; i <= length; i++)
                        pf[i] = this.RandomPoint(rect);

                    graphics.DrawCurve(p, pf, (float)1.75);
                }
            }
        } 

        #endregion
    }
}