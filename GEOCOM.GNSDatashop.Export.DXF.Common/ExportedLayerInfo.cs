using ESRI.ArcGIS.Carto;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ExportedLayerInfo : LayerInfo
    {
        public ExportedLayerInfo(ILayer layer)
            : base(layer)
        {
        }

        public ExportedLayerInfo(IEnumerable<IElement> drawingLayer)
            : base(drawingLayer)
        {
        }

        public double DotsToMeter { get; set; }

        public int FeaturesWritten { get; set; } = 0;
        public int ElementsWritten { get; set; } = 0;

        public override string ToString()
            => (0 < FeaturesWritten)
                ? $"{LayerName}\t{LayerType}/{GeometryType}\t{FeaturesWritten} Features ({ElementsWritten} DXF Entities)\tScale: {Scale} (1 Dot = {DotScale} meter)"
                : $"{LayerName}\t{LayerType}/{GeometryType}\t{FeaturesWritten} Features";

        private double Scale
            => System.Math.Round(72000.0/25.4 * DotsToMeter, 1);

        private double DotScale
            => System.Math.Round(DotsToMeter, 3);
    }
}
