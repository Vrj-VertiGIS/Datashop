using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using System;


namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class DimensionSymbology : Symbology<DimensionSymbolInfo>
    {
        private MarkerSymbology _markerSymbology;
        private LineSymbology _lineSymbology;
        private DimensionTextSymbology _textSymbology;
        private IDimensionClassExtension _dimensionClassExtension;
        
        public DimensionSymbology(ILayer esriLayer, MarkerSymbology markerSymbology, LineSymbology lineSymbology, DimensionTextSymbology textSymbology, double dotsToMeter)
            : base(esriLayer, dotsToMeter)
        {
            _dimensionClassExtension = (_esriLayer as IFeatureLayer)?.FeatureClass?.Extension as IDimensionClassExtension;
            _markerSymbology = markerSymbology;
            _lineSymbology = lineSymbology;
            _textSymbology = textSymbology;
        }

        protected override DimensionSymbolInfo CreateInfoCore(IFeature feature, out ISymbol symbol)
        {
            base.CreateInfoCore(feature, out symbol);

            IDimensionStyle dimStyle;
            if (TryGetDimensionStyle((feature as IDimensionFeature).StyleID, out dimStyle))
            {
                var dimStyleDisplay = dimStyle as IDimensionStyleDisplay;

                var dimensionFeature = feature as IDimensionFeature;

                var markers = GetMarkerSymbol(dimensionFeature, dimStyleDisplay);
                var beginMarkerInfo = (null != markers.Item1) ? _markerSymbology.CreateInfo(markers.Item1 as ISymbol) : null;
                var endMarkerInfo = (null != markers.Item2) ? _markerSymbology.CreateInfo(markers.Item2 as ISymbol) : null;

                var extensionLines = GetExtensionLineSymbol(dimensionFeature, dimStyleDisplay);
                var beginExtensionLineInfo = (null != extensionLines.Item1) ? _lineSymbology.CreateInfo(extensionLines.Item1 as ISymbol) : null;
                var endExtensionLineInfo = (null != extensionLines.Item2) ? _lineSymbology.CreateInfo(extensionLines.Item2 as ISymbol) : null;

                var dimensionLines = GetDimensionLineSymbol(dimensionFeature, dimStyleDisplay);
                var beginDimensionLineInfo = (null != dimensionLines.Item1) ? _lineSymbology.CreateInfo(dimensionLines.Item1 as ISymbol) : null;
                var endDimensionLineInfo = (null != dimensionLines.Item2) ? _lineSymbology.CreateInfo(dimensionLines.Item2 as ISymbol) : null;

                var dimStyleText = dimStyle as IDimensionStyleText;
                var textSymbolInfo = _textSymbology.CreateInfo(dimStyleText?.TextSymbol as ISymbol);

                return new DimensionSymbolInfo(symbol, _esriLayer, dimStyle,
                    beginMarkerInfo, endMarkerInfo,
                    beginDimensionLineInfo, endDimensionLineInfo,
                    beginExtensionLineInfo, endExtensionLineInfo,
                    textSymbolInfo);
            }

            return null;
        }

        protected override DimensionSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            return null;    // Cannot create heere - not enough info at this time
        }

        private bool TryGetDimensionStyle(int styleId, out IDimensionStyle dimStyle)
        {
            dimStyle = null;
            try
            {
                dimStyle = _dimensionClassExtension.DimensionStyles.GetStyle(styleId);
                return (null != dimStyle);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region private helpers

        protected Tuple<IMarkerSymbol, IMarkerSymbol> GetMarkerSymbol(IDimensionFeature dimFeature, IDimensionStyleDisplay dimStyle)
            => new Tuple<IMarkerSymbol, IMarkerSymbol>
            (
                Begin(DDisplay(dimFeature.MarkerDisplay, dimStyle.MarkerDisplay))
                    ? dimStyle.BeginMarkerSymbol
                    : null,
                End(DDisplay(dimFeature.MarkerDisplay, dimStyle.MarkerDisplay))
                    ? dimStyle.EndMarkerSymbol
                    : null
            );

        protected Tuple<ILineSymbol, ILineSymbol> GetExtensionLineSymbol(IDimensionFeature dimFeature, IDimensionStyleDisplay dimStyle)
            => new Tuple<ILineSymbol, ILineSymbol>
            (
                Begin(DDisplay(dimFeature.ExtensionLineDisplay, dimStyle.ExtensionLineDisplay))
                    ? dimStyle.ExtensionLineSymbol
                    : null,
                End(DDisplay(dimFeature.ExtensionLineDisplay, dimStyle.ExtensionLineDisplay))
                    ? dimStyle.ExtensionLineSymbol
                    : null
            );

        protected Tuple<ILineSymbol, ILineSymbol> GetDimensionLineSymbol(IDimensionFeature dimFeature, IDimensionStyleDisplay dimStyle)
            => new Tuple<ILineSymbol, ILineSymbol>
            (
                Begin(DDisplay(dimFeature.DimensionLineDisplay, dimStyle.DimensionLineDisplay))
                    ? dimStyle.DimensionLineSymbol
                    : null,
                End(DDisplay(dimFeature.DimensionLineDisplay, dimStyle.DimensionLineDisplay))
                    ? dimStyle.DimensionLineSymbol
                    : null
            );

        private bool Begin(esriDimensionDisplay d) => (d == esriDimensionDisplay.esriDimensionDisplayBegin) || (d == esriDimensionDisplay.esriDimensionDisplayBoth);
        private bool End(esriDimensionDisplay d) => (d == esriDimensionDisplay.esriDimensionDisplayEnd) || (d == esriDimensionDisplay.esriDimensionDisplayBoth);

        private esriDimensionDisplay DDisplay(esriDimensionDisplay? fromFeature, esriDimensionDisplay? fromStyle)
            => (fromFeature.HasValue)
            ? fromFeature.Value 
            : (fromStyle.HasValue) 
                ? fromStyle.Value
                : esriDimensionDisplay.esriDimensionDisplayNone;

        #endregion

    }
}
