using System;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class LineHatchPatternFactory : HatchPatternFactory
    {
        protected ILineFillSymbol _lineFillSymbol;

        public LineHatchPatternFactory(string name, ILineFillSymbol lineFillSymbol, double dotsToMeter)
            : base(name, lineFillSymbol as IFillSymbol, dotsToMeter)
        {
            _lineFillSymbol = lineFillSymbol;
        }

        public override double PatternLineWidth => _fillSymbol.Outline.Width * _dotsToMeter;

        #region protected members

        protected override void AddPatternLines(HatchPattern pattern)
        {
            var patternLineFactory = HatchPatternLineFactory.CreateFactory(_lineFillSymbol.LineSymbol, _lineFillSymbol, _dotsToMeter);
            pattern.LineDefinitions.AddRange(patternLineFactory.ToPatternLine());
        }

        #endregion

        #region protected (base class) helpers

        protected Vector2 HatchLineOffset(double LineAngleDeg, double linearOffset)
        {
            var rad = LineAngleDeg / 180.0 * Math.PI;
            var dx = Math.Cos(rad);
            var dy = Math.Sin(rad);

            return new Vector2(dy * linearOffset, (-dx) + linearOffset);
        }

        #endregion
    }
}
