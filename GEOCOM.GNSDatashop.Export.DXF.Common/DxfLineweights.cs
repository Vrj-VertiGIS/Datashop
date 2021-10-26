using System;
using System.Collections.Generic;
using System.Linq;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{

    public class DxfLineweights
    {
        public static Lineweight FromMeasure(double measure)
        {
            return Instance._weights.Where(w => ((measure >= w.LowMeasure) && (measure < w.HighMeasure))).Select(w => w.Weight).Single();
        }
            
        private static DxfLineweights _instance = null;

        private static DxfLineweights Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new DxfLineweights();
                return _instance;
            }
        }

        private IEnumerable<WeightElement> _weights = Enum.GetValues(typeof(Lineweight)).Cast<Lineweight>()
            .OrderBy(w => w)
            .Where(w => ((w != Lineweight.Default) && (w != Lineweight.ByLayer) && (w != Lineweight.ByBlock)))
            .Select(w => new WeightElement() { Weight = w })
            .ToList();

        private IEnumerator<WeightElement> _it = null;

        private DxfLineweights()
        {
            _it = _weights.GetEnumerator();
            BuildLookup();
        }

        private WeightElement MoveNext()
        {
            return (_it.MoveNext())
                ? _it.Current
                : null;
        }

        private void BuildLookup()
        {
            var prev = MoveNext();
            var curr = MoveNext();

            prev.LowMeasure = double.MinValue;

            while (null != curr)
            {
                var measure = (prev.Measure + curr.Measure) / 2;
                prev.HighMeasure = measure;
                curr.LowMeasure = measure;

                prev = curr;
                curr = MoveNext();
            }

            prev.HighMeasure = double.MaxValue;
        }
    }

    internal class WeightElement
    {
        public Lineweight Weight { get; set; }
        public double Measure => ((double)Weight) / 1000; // in [m]
        public double LowMeasure { get; set; }
        public double HighMeasure { get; set; }
    }

}
