using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    /// <summary>
    /// Handle ESRI annotations (annotation features)
    /// </summary>
    public class AnnotationTextSymbolInfo : TextSymbolInfo  
    {
        public AnnotationTextSymbolInfo()
        {
            throw new InvalidOperationException("Not to be invoked");
        }

        public AnnotationTextSymbolInfo(ISymbolCollectionElement symbolCollectionElement, ILayer layer)
            : base(null, layer) 
        {
            SymbolCollectionElement = symbolCollectionElement;
        }

        private long _orProps = 0x00;

        protected ISymbolCollectionElement _symbolCollectionElement;
        public ISymbolCollectionElement SymbolCollectionElement
        {
            get => _symbolCollectionElement;
            set
            {
                _symbolCollectionElement = value;
                Symbol = GetEffectiveSymbol(value);
                _orProps = value?.OverriddenProperties ?? 0xff;
            }
        }

        private static ISymbol GetEffectiveSymbol(ISymbolCollectionElement symbolCollectionElement)
        {
            return (null != symbolCollectionElement)
                ? (0 <= symbolCollectionElement.SymbolID)
                    ? symbolCollectionElement.GetSharedSymbol()
                    : (symbolCollectionElement is ITextElement te)
                        ? te.Symbol as ISymbol
                        : null
                : null;
        }

        public override bool Bold => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideBold))
            ? _symbolCollectionElement.Bold
            : base.Bold;

        public override bool Italic => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideItalic))
            ? _symbolCollectionElement.Italic
            : base.Italic;

        public override bool Underline => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideUnderline))
            ? _symbolCollectionElement.Underline
            : base.Underline;

        public override double Angle => (_setAngle != null) 
            ? _setAngle.Value
            :  (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideFlipAngle))
                ? _symbolCollectionElement.FlipAngle
                : base.Angle;

        public override string FontName => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideFontName))
            ? _symbolCollectionElement.FontName
            : base.FontName;

        public override esriTextHorizontalAlignment HorizontalAlignment => (_setHAlignment != null)
            ? _setHAlignment.Value
            : (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideHorzAlignment))
                ? _symbolCollectionElement.HorizontalAlignment
                : base.HorizontalAlignment;

        public override esriTextVerticalAlignment VerticalAlignment => (_setVAlignment != null)
            ? _setVAlignment.Value
            : (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideVertAlignment))
                ? _symbolCollectionElement.VerticalAlignment
                : base.VerticalAlignment;

        public override float Size => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideSize))
            ? (float) _symbolCollectionElement.Size
            : base.Size;

        public override AciColor DXFColor 
            => SubstituteSymbolColor
                ? SubstitutedColor.AciColor()
                : (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideColor))
                    ? _symbolCollectionElement.Color.AciColor()
                    : base.DXFColor;

        public override string Text => (null != _setText) 
            ? _setText
            : _symbolCollectionElement.Text;

        public override double XOffset => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideXOffset))
            ? _symbolCollectionElement.XOffset
            : base.XOffset;

        public override double YOffset => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideYOffset))
            ? _symbolCollectionElement.YOffset
            : base.YOffset;

        public override IGeometry Geometry => (null != _setGeometry)
            ? _setGeometry
            : ((null != _symbolCollectionElement.Geometry) && (!_symbolCollectionElement.Geometry.IsEmpty))
                ? _symbolCollectionElement.Geometry
                : base.Geometry;

        public override double CharacterWidth => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideCharWidth))
            ? _symbolCollectionElement.CharacterWidth
            : base.CharacterWidth;

        public override double Leading => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideLeading))
            ? _symbolCollectionElement.Leading
            : base.Leading;

        public override double CharacterSpacing => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideCharSpacing))
            ? _symbolCollectionElement.CharacterSpacing
            : base.CharacterSpacing;

        public override double WordSpacing => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideWordSpacing))
            ? _symbolCollectionElement.WordSpacing
            : base.WordSpacing;

        public override double FlipAngle => (0 != (_orProps & (long)esriSymbolOverrideEnum.esriSymbolOverrideFlipAngle))
            ? _symbolCollectionElement.FlipAngle
            : base.FlipAngle;

        #region private helpers
        #endregion
    }
}