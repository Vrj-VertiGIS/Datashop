using System;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class MarkerFillHatchPatternFactory : HatchPatternFactory
    {
        protected IMarkerFillSymbol _markerFillSymbol;

        protected MarkerSymbology _markerSymbology;

        public MarkerFillHatchPatternFactory(string name, IMarkerFillSymbol markerFillSymbol, MarkerSymbology markerSymbology, double dotsToMeter)
            : base(name, markerFillSymbol as IFillSymbol, dotsToMeter)
        {
            _markerFillSymbol = markerFillSymbol;
            _markerSymbology = markerSymbology;
        }

        public override double PatternLineWidth => _markerFillSymbol.MarkerSymbol.Size * _dotsToMeter;

        #region protected members

        protected override void AddPatternLines(HatchPattern pattern)
        {
            var ld = new HatchPatternLineDefinition();

            var symbolInfo = _markerSymbology.CreateInfo(_markerFillSymbol.MarkerSymbol as ISymbol);
            var blackboxWidth = symbolInfo.Block.Extent.Width * _dotsToMeter;
            ld.DashPattern.Add(blackboxWidth);

            if (_markerFillSymbol is IFillProperties fp)
            {
                var xSpacing = _dotsToMeter * Math.Abs(fp.XSeparation) - blackboxWidth;
                ld.DashPattern.Add((-1) *  xSpacing);
                ld.Delta = new Vector2(fp.XSeparation, fp.YSeparation) * _dotsToMeter;
                ld.Origin = new Vector2(fp.XOffset, fp.YOffset) * _dotsToMeter;
            }
            else
                ld.DashPattern.Add((-1) * 0.5 * blackboxWidth);    // A 50% gap

            pattern.LineDefinitions.Add(ld);
        }

        #endregion

    }
}
