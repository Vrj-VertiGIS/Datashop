using netDxf.Entities;
using netDxf.Tables;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{

    /// <summary>
    /// Enable cloning of <see cref="MTextFormattingOptions"> as we need
    /// this to convert text containing markung in-line formatting
    /// </summary>
    public class CloneableMTextFormattingOptions : MTextFormattingOptions, ICloneable
    {
        private TextStyle _textStyle;
        public CloneableMTextFormattingOptions(TextStyle textStyle)
            : base(textStyle)
        {
            _textStyle = textStyle;
            Height = _textStyle.Height;
        }

        // Additional option - put text to superscript 
        public bool Superscript { get; set; } = false;

        // Additional option - put text to subscript
        public bool Subscript { get; set; } = false;

        // Factor for sup/superscript
        public double SuperSubScriptHeightFactor { get; set; } = 1.0;

        // Font Height (dots). This will be applied as Height factor
        // when writing since setting the correct height in dots would
        // mean to create a new text style.
        public double Height { get; set; }

        public object Clone()
        {
            return new CloneableMTextFormattingOptions(_textStyle)
            {
                Bold = this.Bold,
                Italic = this.Italic,
                Overline = this.Overline,
                Underline = this.Underline,
                StrikeThrough = this.StrikeThrough,
                Color = this.Color,
                FontName = this.FontName,
                Aligment = this.Aligment,
                HeightFactor = this.HeightFactor,
                ObliqueAngle = this.ObliqueAngle,
                CharacterSpaceFactor = this.CharacterSpaceFactor,
                WidthFactor = this.WidthFactor,
                Superscript = this.Superscript,
                Subscript = this.Subscript,
                SuperSubScriptHeightFactor = this.SuperSubScriptHeightFactor,
                Height = this.Height
            };
        }
    }
}
