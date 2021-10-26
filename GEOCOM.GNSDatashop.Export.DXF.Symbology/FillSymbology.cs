using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class FillSymbology : Symbology<LayeredFillSymbolInfo>
    {
        protected Dictionary<IFillSymbol, Tuple<HatchPattern, double>> _dxfHatchPatternInfo = new Dictionary<IFillSymbol, Tuple<HatchPattern, double>>();

        protected IGeoFeatureLayer _geoFeatureLayer = null;

        protected LineSymbology _lineSymbology = null;
        protected MarkerSymbology _markerSymbology = null;

        public FillSymbology(ILayer esriLayer, MarkerSymbology markerSymbology, LineSymbology lineSymbology, double dotsToMeter)
            : base(esriLayer, dotsToMeter)
        {
            _geoFeatureLayer = _esriLayer as IGeoFeatureLayer;

            // fill symbologies outline symbols are (technically - by reference) distinct for each fill symbol - alltough they might denote
            // the same (outline) line symol (logically - by symbol data). The passed comparer will trigger a "by data" comparison of line symbols such that
            // - at the end - outlines with equal line symbols will be referencing the same dxf Linetype
            //_lineSymbology = new LineSymbology(_esriLayer, _dxfBlockFactory, _dotsToMeter, new EsriLineSymbolByIdentityComparer());
            _lineSymbology = lineSymbology;

            _markerSymbology = markerSymbology;
        }

        protected override LayeredFillSymbolInfo CreateInfoCore(ISymbol symbol)
        {
            var info = new LayeredFillSymbolInfo();

            if (symbol is IFillSymbol fs)
                PopulateSymbolLayers(fs, info);

            return info;
        }

        #region ArcGIS Symbol lookup, FillInfo creation
    
        private void PopulateSymbolLayers(IFillSymbol symbol, LayeredFillSymbolInfo info)
        {
            if (symbol is IMultiLayerFillSymbol mls)
                for (int i = mls.LayerCount - 1; 0 <= i; i--)
                    PopulateSymbolLayer(mls.Layer[i], info, i);
            else
                PopulateSymbolLayer(symbol, info, -1);
        }

        private void PopulateSymbolLayer(IFillSymbol symbol, LayeredFillSymbolInfo info, int symbolLayerIndex)
        {
            var patternInfo = LookupHatchPattern(symbol);
            if (null == patternInfo)
                patternInfo = NewHatchPattern(symbol, SymbolName.Create());

            LayeredLineSymbolInfo outline = null;
            if (null != symbol.Outline)
                outline = _lineSymbology.CreateInfo(symbol.Outline as ISymbol);

            AddHatchpatternLayer(symbol, info, patternInfo, outline);
        }

        #endregion

        #region Hatchpattern creation

        private void AddHatchpatternLayer(IFillSymbol symbol, LayeredFillSymbolInfo info, Tuple<HatchPattern, double> patternInfo, LayeredLineSymbolInfo outline)
        {
            if (null != patternInfo)
                info.Add(new FillSymbolInfo(symbol as ISymbol, _esriLayer, patternInfo, outline));
        }

        private Tuple<HatchPattern, double> LookupHatchPattern(IFillSymbol symbol)
        {
            Tuple<HatchPattern, double> searchedPatternInfo;

            if (_dxfHatchPatternInfo.TryGetValue(symbol, out searchedPatternInfo))
                return searchedPatternInfo;
            else
                return null;
        }

        private Tuple<HatchPattern, double> NewHatchPattern(IFillSymbol symbol, string symbolName)
        {
            Tuple<HatchPattern, double> patternInfo = null;

            if (symbol is IGradientFillSymbol gfs)
                patternInfo = GradientFillSymbolType(symbolName, gfs);
            else if (symbol is IMarkerFillSymbol mfs)
                patternInfo = MarkerFillSymbolType(symbolName, mfs);
            else if (symbol is ILineFillSymbol lfs)
                patternInfo = LineFillSymbolType(symbolName, lfs);
            else if (symbol is ISimpleFillSymbol sfs)
                patternInfo = SimpleFillSymbolType(symbolName, sfs);

            if (null != patternInfo)
                _dxfHatchPatternInfo.Add(symbol, patternInfo);

            return patternInfo;
        }

        #endregion

        #region Gradient fill symbol

        private Tuple<HatchPattern, double> GradientFillSymbolType(string name, IGradientFillSymbol symbol)
        {
            var patternInfo = new HatchGradientPattern();
            patternInfo.Description = name;
            patternInfo.Angle = (symbol.GradientAngle < 180)
                ? symbol.GradientAngle + 180
                : symbol.GradientAngle - 180;
            patternInfo.SingleColor = false;
            patternInfo.GradientType = GradientTypeFromSymbol(symbol);
            var colors = ToEnumerable(symbol.ColorRamp).ToList();
            patternInfo.Color1 = Color.FromArgb(colors.First().RGB).AciColor();
            patternInfo.Color2 = Color.FromArgb(colors.Last().RGB).AciColor();

            return new Tuple<HatchPattern, double>(patternInfo, 0.0);
        }

        private IEnumerable<IColor> ToEnumerable(IColorRamp ramp)
        {
            var rampEnum = ramp.Colors;
            rampEnum.Reset();
            for (var color = rampEnum.Next(); (null != color); color = rampEnum.Next())
                yield return color;
        }

        private HatchGradientPatternType GradientTypeFromSymbol(IGradientFillSymbol symbol)
        {
            switch (symbol.Style)
            {
                case esriGradientFillStyle.esriGFSBuffered:
                case esriGradientFillStyle.esriGFSLinear:
                    return HatchGradientPatternType.Linear;
                case esriGradientFillStyle.esriGFSCircular:
                    return HatchGradientPatternType.Spherical;
                case esriGradientFillStyle.esriGFSRectangular:
                    return HatchGradientPatternType.Cylinder;
                default:
                    return HatchGradientPatternType.Linear;
            }
        }

        #endregion

        #region Line fill symbol

        private Tuple<HatchPattern, double> LineFillSymbolType(string name, ILineFillSymbol symbol)
        {
            var factory = new LineHatchPatternFactory(name, symbol, _dotsToMeter);
            return factory.ToPatternInfo();
        }

        #endregion

        #region Marker fill symbol

        private Tuple<HatchPattern, double> MarkerFillSymbolType(string name, IMarkerFillSymbol symbol)
        {
            var factory = new MarkerFillHatchPatternFactory(name, symbol, _markerSymbology, _dotsToMeter);
            return factory.ToPatternInfo();
        }

        #endregion

        #region Simple fill symbol

        private Tuple<HatchPattern, double> SimpleFillSymbolType(string name, ISimpleFillSymbol symbol)
        {
            var factory = new SimpleFillHatchPatternFactory(name, symbol, _dotsToMeter);
            return factory.ToPatternInfo();
        }

        #endregion

    }
}