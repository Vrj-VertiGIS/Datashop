using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.HatchPatternFactory
{
    public class HatchPatternMultiLayerLineFactory : HatchPatternLineFactory
    {
        protected IMultiLayerLineSymbol _MultilayerLineSymbol;
        public HatchPatternMultiLayerLineFactory(IMultiLayerLineSymbol symbol, ILineFillSymbol fillSymbol, double dotsToMeter)
            : base(symbol as ILineSymbol, fillSymbol, dotsToMeter)
        {
            _MultilayerLineSymbol = symbol;
        }

        public override IEnumerable<HatchPatternLineDefinition> ToPatternLine()
        {
            for (int i = _MultilayerLineSymbol.LayerCount - 1; (0 <= i); i--)
            {
                if (_MultilayerLineSymbol.Layer[i] is ILineSymbol ls)   // Paranoja ...
                {   
                    var patternLineFactory = CreateFactory(ls, _fillSymbol, _dotsToMeter);
                    if (null != patternLineFactory)
                        foreach (var patternLine in patternLineFactory.ToPatternLine())
                            yield return patternLine;
                }
            }
        }
    }
}