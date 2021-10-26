using ESRI.ArcGIS.Display;
using netDxf.Tables;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.LineTypeFactory
{
    public class SimpleLineTypeFactory : LinearTypeFactory
    {
        protected ISimpleLineSymbol _simpleLineSymbol;
        private string _name;

        public SimpleLineTypeFactory(string name, ISimpleLineSymbol lineSymbol, double dotsToMeter)
            : base(lineSymbol as ILineSymbol, dotsToMeter)
        {
            _simpleLineSymbol = lineSymbol;
            _name = name; 
        }

        public virtual Linetype ToLine()
        {
            return NewLineType();
        }

        protected virtual Linetype NewLineType()
        {
            var pf = new PatternFactory(_simpleLineSymbol.Width, _dotsToMeter);
            switch (_simpleLineSymbol.Style)
            {
                case esriSimpleLineStyle.esriSLSDash:
                    return new Linetype(_name, pf.DashPattern);
                case esriSimpleLineStyle.esriSLSDashDot:
                    return new Linetype(_name, pf.DashDotPattern);
                case esriSimpleLineStyle.esriSLSDashDotDot:
                    return new Linetype(_name, pf.DashDotDotPattern);
                case esriSimpleLineStyle.esriSLSDot:
                    return new Linetype(_name, pf.DotPattern);
                case esriSimpleLineStyle.esriSLSSolid:
                    return Linetype.Continuous;
                default:
                    return Linetype.Continuous;
            }
        }


        private class PatternFactory
        {
            private double _lineWidthDots;
            private double _dotsToMeter;
            public PatternFactory(double lineWidthDots, double dotsToMeter)
            {
                _lineWidthDots = lineWidthDots;
                _dotsToMeter = dotsToMeter;
            }

            public IEnumerable<double> DotPattern
                => new List<double>() { DotElement, GapElement(3) };

            public IEnumerable<double> DashPattern
                => new List<double>() { DashElement, GapElement(5) };

            public IEnumerable<double> DashDotPattern
                => new List<double>() { DashElement, GapElement(5), DotElement, GapElement(5) };

            public IEnumerable<double> DashDotDotPattern
                => new List<double>() { DashElement, GapElement(5), DotElement, GapElement(5), DotElement, GapElement(5) };

            private double DashElement
                => PatternElement(15);
            private double DotElement
                => PatternElement(3);

            private double PatternElement(double dotLength)
                => _dotsToMeter * _lineWidthDots * dotLength;
            private double GapElement(double dotLength)
                => (-1) * PatternElement(dotLength);

        }
    }
}
