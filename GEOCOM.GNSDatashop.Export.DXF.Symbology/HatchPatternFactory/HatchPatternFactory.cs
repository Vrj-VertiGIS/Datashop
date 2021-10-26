using System;
using ESRI.ArcGIS.Display;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public abstract class HatchPatternFactory
    {
        protected IFillSymbol _fillSymbol;
        protected string _name;
        protected double _dotsToMeter;

        public HatchPatternFactory(string name, IFillSymbol fillSymbol, double dotsToMeter)
        {
            _fillSymbol = fillSymbol;
            _name = name;
            _dotsToMeter = dotsToMeter;
        }

        public virtual Tuple<HatchPattern, double> ToPatternInfo()
        {
            var pattern = NewHatchPattern();

            pattern.Scale = 1.0;
            pattern.Angle = 0.0;
            pattern.Origin = new Vector2(0, 0);

            AddPatternLines(pattern);

            return new Tuple<HatchPattern, double>(pattern, PatternLineWidth);
        }

        public virtual double PatternLineWidth => _fillSymbol.Outline.Width * _dotsToMeter;

        protected virtual HatchPattern NewHatchPattern()
        {
            return new HatchPattern(_name) { Type = HatchType.Custom };
        }

        protected abstract void AddPatternLines(HatchPattern pattern);
    }
}
