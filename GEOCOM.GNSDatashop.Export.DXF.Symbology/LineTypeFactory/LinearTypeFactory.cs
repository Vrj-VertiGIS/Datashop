using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.LineTypeFactory
{
    public abstract class LinearTypeFactory
    {
        protected ILineSymbol _lineSymbol;
        protected double _dotsToMeter;

        public LinearTypeFactory(ILineSymbol lineSymbol, double dotsTometer)
        {
            _lineSymbol = lineSymbol;
            _dotsToMeter = dotsTometer;
        }
    }
}
