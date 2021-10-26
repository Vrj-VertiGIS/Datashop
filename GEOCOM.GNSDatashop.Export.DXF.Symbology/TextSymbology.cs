using System.Linq;
using ESRI.ArcGIS.Carto;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using GEOCOM.GNSDatashop.TTF;
using log4net;
using netDxf.Collections;
using netDxf.Tables;


namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public abstract class TextSymbology<T> : Symbology<T> where T : TextSymbolInfo, new()
    {
        #region private class-type members
        //private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog _log = LogManager.GetLogger(typeof(TextSymbology<T>));

        #endregion

        protected TextStyles _textStyles = null;

        protected IGeoFeatureLayer _geoFeatureLayer;

        protected LineSymbology _leaderLineSymbology = null;
        protected MarkerSymbology _anchorPointSymbology = null;

        protected static readonly CSystemFonts _systemFonts = new CSystemFonts();

        protected TextSymbology(ILayer esriLayer, double dotsToMeter, TextStyles textStyles, MarkerSymbology markerSymbology)
            : base(esriLayer, dotsToMeter)
        {
            _geoFeatureLayer = esriLayer as IGeoFeatureLayer;
            _textStyles = textStyles;

            // Textsymbologies leader line symbols are (technically - by reference) distinct for each textsymbol - alltough they might denote
            // the same line symol (logically - data). The passed comparer will trigger a "by data" comparison of line symbols such that
            // - at the end - leader lines with equal line symbols will be referencing the same dxf Linetype
            _anchorPointSymbology = markerSymbology;
            _leaderLineSymbology = new LineSymbology(esriLayer, _anchorPointSymbology, _dotsToMeter);
        }

        protected TextStyle GetTextStyle(TextSymbolInfo symbolInfo)
        {
            TextStyle textStyle = TextStyle.Default;

            var textStyleName = string.Format("TS[{0}[{1}]]", _esriLayer.Name, symbolInfo.FontFace.ToString()).DxfCompatibleName();

            if (!_textStyles.TryGetValue(textStyleName, out textStyle))
            {
                // The first time the below is called will cause a delay depending on the # of installed fonts

                var fontFileNames = _systemFonts.get_FaceFilePaths(symbolInfo.TextSymbol.Font);
                var fontFileName = fontFileNames.FirstOrDefault();
                if (string.IsNullOrEmpty(fontFileName))
                    _log.Info($"No TrueType font file found for font \"{symbolInfo.FontFace.ToString()}\".");
                else
                {
                    textStyle = new TextStyle(textStyleName, fontFileName);
                    textStyle.Height = ((double)symbolInfo.Size) * _dotsToMeter * 0.73; // An arbitrary factor determined by empirical measurements
                    textStyle.WidthFactor = symbolInfo.CharacterWidth / 100;
                }
            }

            return textStyle;
        }

    }
}
