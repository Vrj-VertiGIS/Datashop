using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class DimensionSymbolInfo : SymbolInfo, IDisposable
    {
        private IDimensionStyle _dimensionStyle = null;
        private IDimensionStyleDisplay _dimensionStyleDisplay = null;
        private IDimensionStyleText _dimensionStyleText = null;

        public MarkerSymbolInfo BeginMarker { get; private set; }
        public MarkerSymbolInfo EndMarker { get; private set; }
        public LayeredLineSymbolInfo BeginDimensionLine { get; private set; }
        public LayeredLineSymbolInfo EndDimensionLine { get; private set; }
        public LayeredLineSymbolInfo BeginExtensionLine { get; private set; }
        public LayeredLineSymbolInfo EndExtensionLine { get; private set; }
        public TextSymbolInfo TextSymbol { get; private set; }

        public DimensionSymbolInfo() { }

        public DimensionSymbolInfo(ISymbol symbol, ILayer layer, IDimensionStyle dimensionStyle,
            MarkerSymbolInfo beginMarker, MarkerSymbolInfo endMarker,
            LayeredLineSymbolInfo beginDimensionLine, LayeredLineSymbolInfo endDimensionLine,
            LayeredLineSymbolInfo beginExtensionLine, LayeredLineSymbolInfo endExtensionLine,
            TextSymbolInfo text
            )
            : base(symbol, layer)
        {
            _dimensionStyle = dimensionStyle;
            _dimensionStyleDisplay = dimensionStyle as IDimensionStyleDisplay;
            _dimensionStyleText = dimensionStyle as IDimensionStyleText;

            BeginMarker = beginMarker;
            EndMarker = endMarker;

            BeginDimensionLine = beginDimensionLine;
            EndDimensionLine = endDimensionLine;

            BeginExtensionLine = beginExtensionLine;
            EndExtensionLine = endExtensionLine;

            TextSymbol = text;
        }

        public override byte Opacity => 255;

        public bool DrawLineOnFit => _dimensionStyleDisplay.DrawLineOnFit;

        public esriDimensionMarkerFit MarkerFit => _dimensionStyleDisplay.MarkerFit;
        public double MarkerFitTolerance => _dimensionStyleDisplay.MarkerFitTolerance;

        public double BaselineHeight => _dimensionStyleDisplay.BaselineHeight;

        public double ExtensionLineOffset => _dimensionStyleDisplay.ExtensionLineOffset;

        public double ExtensionLineOvershot => _dimensionStyleDisplay.ExtensionLineOvershot;


        #region IDimensionStyle shortcuts
        public IDimensionStyle Style => _dimensionStyle;
        #endregion

        #region IDimensionStyleText xhortcuts

        public esriDimensionTextFit TextFit => _dimensionStyleText?.TextFit ?? esriDimensionTextFit.esriDimensionTextFitNone;
        public bool AlignToDimensionLine => _dimensionStyleText?.Align ?? false;
        public int DisplayPrecision => _dimensionStyleText?.DisplayPrecision ?? 0;
        public esriDimensionTextDisplay TextDisplay => _dimensionStyleText?.TextDisplay ?? esriDimensionTextDisplay.esriDimensionTDNone;
        public string TextPrefix => _dimensionStyleText?.Prefix ?? string.Empty;
        public string TextSuffix => _dimensionStyleText?.Suffix ?? string.Empty;
        public string TextExpression => _dimensionStyleText?.Expression ?? "{0}";
        public string ParserName => _dimensionStyleText?.ExpressionParserName ?? string.Empty;
        public bool ExpressionSimple => _dimensionStyleText?.ExpressionSimple ?? false;


        #endregion

        #region IDisposable

        public void Dispose()
        {
            DisposeManaged();
            GC.SuppressFinalize(this);
        }

        private void DisposeManaged()
        {
            TextSymbol?.Dispose();
        }

        #endregion

    }
}
