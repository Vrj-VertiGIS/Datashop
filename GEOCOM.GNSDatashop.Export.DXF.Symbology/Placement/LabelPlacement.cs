using ESRI.ArcGIS.Geometry;

using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement
{
    public abstract class LabelPlacement
    {
        protected HVAlignment _alignement;

        public static LabelPlacement Create(TextSymbolInfo textSymbol)
        {
            return Create(textSymbol.Geometry, textSymbol.Alignment);
        }

        public static LabelPlacement Create(IGeometry referenceGeometry, HVAlignment alignement)
        {
            var referencePoint = referenceGeometry as IPoint;
            if (null != referencePoint)
                return new LabelByPointPlacement(referencePoint, alignement);
            var referenceLine = referenceGeometry as IPolyline;
            if (null != referenceLine)
                return new LabelByPolylinePlacement(referenceLine, alignement);
            var referenceArea = referenceGeometry as IArea;
            if (null != referenceArea)
                return new LabelByCentroidPlacement(referenceArea, alignement);
            var referenceEnvelope = referenceGeometry.Envelope;
            if (null != referenceEnvelope)
                return new LabelByExtentPlacement(referenceEnvelope, alignement);
            return null;
        }

        public LabelPlacement(HVAlignment alignement)
        {
            _alignement = alignement;
        }

        public abstract void Apply(TextSymbolInfo symbolInfo);
    }
}
