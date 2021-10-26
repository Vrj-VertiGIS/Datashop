using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info.LabelTextParser;
using netDxf;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    /// <summary>
    /// Basic means to handle text symbols (annotations, GEOCOM labels)
    /// </summary>
    public class TextSymbolInfo : SymbolInfo, IDisposable
    {
        protected ISimpleTextSymbol _textSymbol = null;         // Label text symbology
        protected IFormattedTextSymbol _fmtTextSymbol = null;   // " 
        protected ILineSymbol _leaderLineSymbol = null;
        protected IMarkerSymbol _anchorPointSymbol = null;      // Annotation text anchor point symbol

        protected Vector2 _referencePoint = Vector2.NaN;

        // Private overriding fields. Some symbol based values can be overriden from 
        // the outside - due to alignment line or feature data
        protected double? _setAngle = null;
        protected esriTextHorizontalAlignment? _setHAlignment = null;
        protected esriTextVerticalAlignment? _setVAlignment = null;
        protected double? _setTextColumnWidth = null;
        protected string _setText = null;           // not set at all
        protected IGeometry _setGeometry = null;    // Text alignment geometry

        protected ISymbolSubstitution _symbolSubstitution = null;

        protected FontFace _fontFace = null;

        protected static Graphics _graphics = Graphics.FromHwnd(IntPtr.Zero);    // By now - quick'n dirty

        public TextSymbolInfo()
        {
            throw new System.Exception("Not to be invoked");
        }

        public TextSymbolInfo(ISymbol symbol, ILayer layer)
            : base()
        {
            base._esriLayer = layer;
            _symbolSubstitution = layer as ISymbolSubstitution;
            Symbol = symbol;
        }

        public override ISymbol Symbol
        {
            get => base.Symbol;
            protected set
            {
                base.Symbol = value;
                _textSymbol = value as ISimpleTextSymbol;
                _fmtTextSymbol = value as IFormattedTextSymbol;
                _fontFace = null;
            }
        }

        public static TextSymbolInfo Create(ISymbol symbol, ILayer layer) => new TextSymbolInfo(symbol, layer);

        public void Dispose()
        {
            DisposeManaged();
            GC.SuppressFinalize(this);
        }

        private void DisposeManaged()
        {
            _fontFace?.Dispose();
            _graphics?.Dispose();
        }

        public ISimpleTextSymbol TextSymbol => _textSymbol;

        public virtual double Angle => (null != _setAngle) ? _setAngle.Value : _textSymbol.Angle;
        public void Set_Angle(double value)
        {
            _setAngle = value;
        }

        public virtual char BreakCharacter => (char) _textSymbol.BreakCharacter;

        public virtual string FontName => _textSymbol.Font.Name;

        public virtual esriTextHorizontalAlignment HorizontalAlignment => (_setHAlignment != null) 
            ? _setHAlignment.Value  
            : _textSymbol.HorizontalAlignment;

        public void Set_HorizontalAlignment(esriTextHorizontalAlignment value)
        {
            _setHAlignment = value;
            _textSize = null;
        }

        public virtual esriTextVerticalAlignment VerticalAlignment => (_setVAlignment != null)
            ? _setVAlignment.Value
            : _textSymbol.VerticalAlignment;

        public void Set_VerticalAlignment(esriTextVerticalAlignment value)
        {
            _setVAlignment = value;
            _textSize = null;
        }

        public virtual float Size => (float) _textSymbol.Size;

        public virtual AciColor DXFColor
            => SymbolColor.AciColor();

        public override byte Opacity => (null != _textSymbol)
            ? SymbolColor.Transparency
            : SymbolInfosWithGeometries.Opacity;

        public virtual string Text => _setText ?? _textSymbol.Text;
        public void Set_Text(string value)
        {
            _setText = value;
            _textSize = null;   // Reset sizing info
        }

        public virtual double XOffset => _textSymbol.XOffset;

        public virtual double YOffset => _textSymbol.YOffset;

        public virtual bool Bold => _textSymbol.Font.Bold;

        public virtual bool Underline => _textSymbol.Font.Underline;

        public virtual bool Italic => _textSymbol.Font.Italic;

        public virtual bool StrikeTrough => _textSymbol.Font.Strikethrough;

        public virtual double Weight => _textSymbol.Font.Weight;

        public virtual double Leading => (null != _fmtTextSymbol) ? _fmtTextSymbol.Leading : 0.0;

        public virtual double CharacterWidth => (null != _fmtTextSymbol) ? _fmtTextSymbol.CharacterWidth : 1.0;

        public virtual double CharacterSpacing => (null != _fmtTextSymbol) ? _fmtTextSymbol.CharacterSpacing : 1.0;

        public virtual double WordSpacing => (null != _fmtTextSymbol) ? _fmtTextSymbol.WordSpacing : 1.0;

        public virtual bool Kerning => (null != _fmtTextSymbol) ? _fmtTextSymbol.Kerning : false;

        public virtual double FlipAngle => (null != _fmtTextSymbol) ? _fmtTextSymbol.FlipAngle : 0.0;

        public virtual System.Drawing.FontStyle FontStyle
        {
            get
            {
                // Don't use _fontFace.FontStyle - there are overloads of Bold, Italic, Underline,... in ancestors!
                var fStyle = System.Drawing.FontStyle.Regular;
                if (Bold) fStyle |= System.Drawing.FontStyle.Bold;
                if (Italic) fStyle |= System.Drawing.FontStyle.Italic;
                if (Underline) fStyle |= System.Drawing.FontStyle.Underline;
                if (StrikeTrough) fStyle |= System.Drawing.FontStyle.Strikeout;
                return fStyle;
            }
        }

        public virtual FontFamily FontFamily => FontFace.FontFamily;

        public virtual FontFace FontFace 
            => _fontFace
                // Use possibly overriden properties to create font face
                ?? (_fontFace = (null != _textSymbol)
                    ? new FontFace(_textSymbol.Font.Name, Size, FontStyle, CharacterWidth)
                    : null); // We have some cases where no text symbol is set (ESRI Annotations with markers/pointer lines only)

        public virtual double FontFamilyCellEMHeight => FontFamily.GetEmHeight(FontStyle);

        public virtual double FontFamilyCellAscent => FontFamily.GetCellAscent(FontStyle);

        public virtual double FontFamilyCellDescent => FontFamily.GetCellDescent(FontStyle);

        public virtual Font Font => FontFace.Font;

        public virtual double Ascent => Font.Size * FontFamilyCellAscent / FontFamilyCellEMHeight;

        public virtual double Descent => Font.Size * FontFamilyCellDescent / FontFamilyCellEMHeight;

        public TextStyle TextStyle { get; set; }

        public virtual IGeometry Geometry => (null != _setGeometry)
            ? _setGeometry
            : TextSymbol.TextPath.Geometry;

        public void Set_Geometry(IGeometry value)
        {
            _setGeometry = value;
        }

        public virtual HVAlignment Alignment
        {
            get
            {
                HVAlignments.TryGetAlignment(HorizontalAlignment, VerticalAlignment, out var al);
                return al;  // In case TryGet... returns false, the default alignment will be set in al
            }
        }
        public void Set_Alignment(HVAlignment value)
        {
            Set_HorizontalAlignment(value.HAlignment);
            Set_VerticalAlignment(value.VAlignment);
        }

        public Vector2 ReferencePoint
        {
            get { return _referencePoint; }
            set { _referencePoint = value; }
        }

        public virtual double TextColumnWidth => (null != _setTextColumnWidth) ? _setTextColumnWidth.Value : 0.0;
        public void Set_TextColumnWidth(double value)
        {
            _setTextColumnWidth = value;
        }

        public SymbolInfoWithGeometries SymbolInfosWithGeometries{ get; } = new SymbolInfoWithGeometries();
        public virtual void AddGeometryAndSymbol(IGeometry geometry, ISymbolInfo symbolInfo)
        {
            if (!geometry.IsEmpty)
                SymbolInfosWithGeometries.Add(new SymbolInfoWithGeometry(symbolInfo, geometry));
        }

        private LabelTextParser.LabelTextParser _parser = null;
        public IEnumerable<LabelTextToken> TextTokens
            => _parser?.Tokens ?? (_parser = new LabelTextParser.LabelTextParser(Text)).Tokens;

        #region dot coordinates of current text - assuming single-line, single character formatted only

        protected SizeF? _textSize = null;

        #endregion

        #region symbol color substitution

        protected bool SubstituteSymbolColor
            => (_symbolSubstitution?.SubstituteType ?? esriSymbolSubstituteType.esriSymbolSubstituteNone) == esriSymbolSubstituteType.esriSymbolSubstituteColor;

        protected IColor SubstitutedColor
            => _symbolSubstitution.MassColor;

        protected virtual IColor SymbolColor
            => SubstituteSymbolColor
                ? SubstitutedColor
                : _textSymbol.Color;

        #endregion
    }
}