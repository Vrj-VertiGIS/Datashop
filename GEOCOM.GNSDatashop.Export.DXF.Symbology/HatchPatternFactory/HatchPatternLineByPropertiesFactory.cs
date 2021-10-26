using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf.Entities;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class HatchPatternLineByPropertiesFactory : HatchPatternLineFactory
    {
        protected ILineProperties _lineProps;

        public HatchPatternLineByPropertiesFactory(ILineProperties lineProperties, ILineFillSymbol fillSymbol, double dotsToMeter)
            : base(lineProperties as ILineSymbol, fillSymbol, dotsToMeter)
        {
            _lineProps = lineProperties;
        }

        public override IEnumerable<HatchPatternLineDefinition> ToPatternLine()
        {
            var lineDefinition = new HatchPatternLineDefinition();

            yield return CustomHatchPatternLine(_lineProps.Template.LinePattern(), _fillSymbol.Separation, _fillSymbol.Angle, _lineSymbol.Width);
        }

    }
}

