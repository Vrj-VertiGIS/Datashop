using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class HatchPatternPictureLineFactory : HatchPatternLineFactory
    {
        protected IPictureLineSymbol _pictureLineSymbol;

        private double[] _dashPattern = new double[] { 1 };

        public HatchPatternPictureLineFactory(IPictureLineSymbol symbol, ILineFillSymbol fillSymbol, double dotsToMeter)
            : base(symbol as ILineSymbol, fillSymbol, dotsToMeter)
        {
            _pictureLineSymbol = symbol;
        }

        public override IEnumerable<HatchPatternLineDefinition> ToPatternLine()
        {
            yield return CustomHatchPatternLine(_dashPattern, _fillSymbol.Separation, _fillSymbol.Angle, _pictureLineSymbol.Width);
        }


    }
}
