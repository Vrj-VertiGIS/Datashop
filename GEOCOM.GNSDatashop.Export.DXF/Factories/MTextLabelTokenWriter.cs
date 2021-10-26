using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{
    public class MTextLabelTokenWriter : MText
    {
        private CloneableMTextFormattingOptions _options;

        private static double _dotsToMeter; // yes - I know...

        public MTextLabelTokenWriter(Layer layer, TextSymbolInfo symbolInfo, double dotsToMeter)
            : base()
        {
            Layer = layer;
            base.Style = symbolInfo.TextStyle;
            _options = CreateFormattingOptions(symbolInfo);
            _dotsToMeter = dotsToMeter;
        }

        private static CloneableMTextFormattingOptions CreateFormattingOptions(TextSymbolInfo symbolInfo)
        {
            var options = new CloneableMTextFormattingOptions(symbolInfo.TextStyle);
            // Some formatting styles are not kept in the textstyle - transfer them from the symbolInfo
            options.Underline |= symbolInfo.Underline;
            options.Italic |= symbolInfo.Italic;
            options.Bold |= symbolInfo.Bold;
            options.StrikeThrough |= symbolInfo.StrikeTrough;
            return options;
        }

        #region Write to mtext - appending to the object's contents

        /// <summary>
        /// Write tokens.
        /// </summary>
        /// <param name="tokens"></param>
        public void Write(IEnumerable<LabelTextToken> tokens)
            => Write(new LabelTextTokens(tokens.GetEnumerator()), _options);

        /// <summary>
        /// Write tokens. Make shure that a mismatch between <start...> </start...>
        /// will not cause data loss. Call WriteCore() until no data 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="options"></param>
        private void Write(LabelTextTokens tokens, CloneableMTextFormattingOptions options)
        {
            for (; tokens.ValidData; tokens.MoveNext()) 
                WriteCore(tokens, options);
        }

        private void WriteCore(LabelTextTokens tokens, CloneableMTextFormattingOptions options)
        {
            // Be robust - write tags after end tags if start-end tags do not match
            while (tokens.ValidData && (tokens.Current.TokenClass != LabelTextTokenClass.ClosingTag))
            {
                WriteToken(tokens, options);
                tokens.MoveNext();
            }
        }

        private void WriteToken(LabelTextTokens tokens, CloneableMTextFormattingOptions options)
        {
            var token = tokens.Current;
            if (token.TokenClass == LabelTextTokenClass.OpeningTag)
            {
                var newOptions = ProcessTag(token, options);
                tokens.MoveNext();
                WriteCore(tokens, newOptions); 
            }
            else if (token.TokenClass == LabelTextTokenClass.ClosingTag)
                return;                               // Simply return from nested level
            else if (token.TokenClass == LabelTextTokenClass.Escaping)
                WriteEscaping(token, options);
            else
                Write(token.Text, options);
        }

        private void WriteEscaping(LabelTextToken token, CloneableMTextFormattingOptions options)
        {
            switch (token.TokenType)
            {
                case LabelTextTokenType.ESCNewline:
                    EndParagraph();
                    break;
            }
        }

        #endregion

        #region Getting formatted texts according to mtext formatting options

        public static IEnumerable<string> GetFormattedText(IEnumerable<LabelTextToken> tokens)
            => GetFormattedText(new LabelTextTokens(tokens.GetEnumerator()), new CloneableMTextFormattingOptions(TextStyle.Default));

        private static IEnumerable<string> GetFormattedText(LabelTextTokens tokens, CloneableMTextFormattingOptions options)
        {
            while (tokens.ValidData)
            {
                yield return GetFormattedTextToken(tokens, options);
                tokens.MoveNext();
            }
        }

        private static string GetFormattedTextToken(LabelTextTokens tokens, CloneableMTextFormattingOptions options)
        {
            string tokenText = string.Empty;

            var token = tokens.Current;
            if (token.TokenClass == LabelTextTokenClass.OpeningTag)
            {
                var newOptions = ProcessTag(token, options);
                tokens.MoveNext();
                tokenText = GetFormattedTextToken(tokens, newOptions); // Process nested level
            }
            else if (token.TokenClass == LabelTextTokenClass.ClosingTag)
                return tokenText;                               // Simply return from nested level
            else if (token.TokenClass == LabelTextTokenClass.Escaping)
                tokenText = GetEscaping(token, options);
            else
                tokenText = options.FormatText(token.Text);

            return tokenText;
        }

        private static string GetEscaping(LabelTextToken token, CloneableMTextFormattingOptions options)
        {
            switch (token.TokenType)
            {
                case LabelTextTokenType.ESCNewline:
                    return options.FormatText(@"\P");
            }
            return string.Empty;
        }

        #endregion

        #region helpers

        private static CloneableMTextFormattingOptions ProcessTag(LabelTextToken token, CloneableMTextFormattingOptions options)
        {
            var newOptions = options.Clone() as CloneableMTextFormattingOptions;

            switch (token.TokenType)
            {
                case LabelTextTokenType.TagBold:
                    newOptions.Bold = true;
                    break;
                case LabelTextTokenType.TagNotBold:
                    newOptions.Bold = false;
                    break;
                case LabelTextTokenType.TagColor:
                    newOptions.Color = AciColor.FromTrueColor((token as LabelTextColorToken)?.Value ?? 0);
                    break;
                case LabelTextTokenType.TagFont:
                    SetFontOptions(newOptions, token as LabelTextFontToken);
                    break;
                case LabelTextTokenType.TagITalic:
                    newOptions.Italic = true;
                    break;
                case LabelTextTokenType.TagNotItalic:
                    newOptions.Italic = false;
                    break;
                case LabelTextTokenType.TagNotUnderline:
                    newOptions.Underline = false;
                    break;
                case LabelTextTokenType.TagUnderline:
                    newOptions.Underline = true;
                    break;
                case LabelTextTokenType.TagSuperScript:
                    SetSuperscriptOptions(newOptions, token);
                    break;
                case LabelTextTokenType.TagSubScript:
                    SetSubscriptOptions(newOptions, token);
                    break;
            }
            return newOptions;
        }

        private static void SetFontOptions(CloneableMTextFormattingOptions options, LabelTextFontToken token)
        {
            if (null != token)
            {
                options.FontName = token.Name;
                options.Bold = token.Bold;
                options.Italic = token.Italic;
                if (token.Height.HasValue)
                    options.Height = token.Height.Value * _dotsToMeter * TransientValues.TextToDotsScaling;
                options.HeightFactor = (token.Scale.HasValue) ? token.Scale.Value / 100 : 1.0;
            }
        }

        private static void SetSuperscriptOptions(CloneableMTextFormattingOptions options, LabelTextToken token)
        {
            options.Superscript = true;
            options.SuperSubScriptHeightFactor = 0.75;
            options.Aligment = MTextFormattingOptions.TextAligment.Center;
        }

        private static void SetSubscriptOptions(CloneableMTextFormattingOptions options, LabelTextToken token)
        {
            options.Subscript = true;
            options.SuperSubScriptHeightFactor = 0.75;
            options.Aligment = MTextFormattingOptions.TextAligment.Center;
        }

        #endregion

        #region advanced - nev netdxf version related - methods

        /// <summary>
        /// Adds the text to the current paragraph. 
        /// Override the Write() method of the netdxf MText class as there are some important
        /// features missing
        /// </summary>
        /// <param name="txt">Text string.</param>
        /// <param name="options">Text formatting options.</param>
        private void Write(string txt, CloneableMTextFormattingOptions options)
            => base.Write(null == options ? txt : FormattedText(txt, options));

        private string FormattedText(string txt, CloneableMTextFormattingOptions options)
        {
            string formattedText = txt;
            // Simulate new fount size by applying it to the height factor
            double baseHeightFactor = options.HeightFactor * options.Height / Height;

            if (options.Superscript)
            {
                formattedText = string.Format("\\S{0}^ ;", formattedText);
                baseHeightFactor *= options.SuperSubScriptHeightFactor;
            }
            if (options.Subscript)
            {
                formattedText = string.Format("\\S^ {0};", formattedText);
                baseHeightFactor *= options.SuperSubScriptHeightFactor;
            }

            string f;
            if (string.IsNullOrEmpty(options.FontName))
                f = string.IsNullOrEmpty(this.Style.FontFamilyName) ? this.Style.FontFile : this.Style.FontFamilyName;
            else
                f = options.FontName;

            if (options.Bold && options.Italic)
                formattedText = string.Format("\\f{0}|b1|i1;{1}", f, formattedText);
            else if (options.Bold && !options.Italic)
                formattedText = string.Format("\\f{0}|b1|i0;{1}", f, formattedText);
            else if (!options.Bold && options.Italic)
                formattedText = string.Format("\\f{0}|i1|b0;{1}", f, formattedText);
            else
                formattedText = string.Format("\\f{0}|b0|i0;{1}", f, formattedText);

            if (options.Overline)
                formattedText = string.Format("\\O{0}\\o", formattedText);
            if (options.Underline)
                formattedText = string.Format("\\L{0}\\l", formattedText);
            if (options.StrikeThrough)
                formattedText = string.Format("\\K{0}\\k", formattedText);
            if (options.Color != null)
            {
                // The DXF is not consistent in the way it converts a true color to its 24-bit representation,
                // while stuff like layer colors it follows BGR order, when formatting text it uses RGB order.
                formattedText = options.Color.UseTrueColor
                    ? string.Format("\\C{0};\\c{1};{2}", options.Color.Index, BitConverter.ToInt32(new byte[] { options.Color.R, options.Color.G, options.Color.B, 0 }, 0), formattedText)
                    : string.Format("\\C{0};{1}", options.Color.Index, formattedText);
            }

            if (!MathHelper.IsOne(baseHeightFactor))
                formattedText = string.Format("\\H{0}x;{1}", baseHeightFactor.ToString(CultureInfo.InvariantCulture), formattedText);
            if (!MathHelper.IsZero(options.ObliqueAngle))
                formattedText = string.Format("\\Q{0};{1}", options.ObliqueAngle.ToString(CultureInfo.InvariantCulture), formattedText);
            if (!MathHelper.IsOne(options.CharacterSpaceFactor))
                formattedText = string.Format("\\T{0};{1}", options.CharacterSpaceFactor.ToString(CultureInfo.InvariantCulture), formattedText);
            if (!MathHelper.IsOne(options.WidthFactor))
                formattedText = string.Format("\\W{0};{1}", options.WidthFactor.ToString(CultureInfo.InvariantCulture), formattedText);

            return $"{{{formattedText}}}";
        }

        #endregion
    }
}