using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class HatchPatternSimpleLineFactory : HatchPatternLineFactory
    {
        protected ISimpleLineSymbol _simpleLineSymbol;

        protected static readonly IDictionary<esriSimpleLineStyle, double[]> _simpleLineDashPattern
            = new Dictionary<esriSimpleLineStyle, double[]>()
            {
                // Dots are - unscaled - 1 dot (1/72 inch) wide, dashes accordingly
                { esriSimpleLineStyle.esriSLSDash, new double[] {10, -5} },
                { esriSimpleLineStyle.esriSLSDashDot, new double[] { 10, -5, 1, -5 } },
                { esriSimpleLineStyle.esriSLSDashDotDot, new double[] { 10, -5, 1, -5, 1, -5 } },
                { esriSimpleLineStyle.esriSLSDot, new double[] { 2, -5 } },
                { esriSimpleLineStyle.esriSLSSolid, new double [] { 1 } }
            };


        public HatchPatternSimpleLineFactory(ISimpleLineSymbol symbol, ILineFillSymbol fillSymbol, double dotsToMeter)
            : base(symbol as ILineSymbol, fillSymbol, dotsToMeter)
        {
            _simpleLineSymbol = symbol;
        }

        public override IEnumerable<HatchPatternLineDefinition> ToPatternLine()
        {
            var lineDefinition = new HatchPatternLineDefinition();

            if (_simpleLineDashPattern.TryGetValue(_simpleLineSymbol.Style, out double[] dashPattern))
                yield return CustomHatchPatternLine(dashPattern, _fillSymbol.Separation, _fillSymbol.Angle, _simpleLineSymbol.Width);
            else
                yield return CustomHatchPatternLine(new double[] { 1 }, _fillSymbol.Separation, _fillSymbol.Angle, _simpleLineSymbol.Width);
        }

    }
}
