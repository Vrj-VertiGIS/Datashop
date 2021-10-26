using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement
{
    public class LabelByCentroidPlacement : LabelPlacement
    {
        private IArea _referenceArea;

        public LabelByCentroidPlacement(IArea referenceAea, HVAlignment alignement)
            : base(alignement)
        {
            _referenceArea = referenceAea;
        }

        public override void Apply(TextSymbolInfo symbolInfo)
        {
            var labelPoint = _referenceArea.LabelPoint;
            symbolInfo.ReferencePoint = new Vector2(labelPoint.X, labelPoint.Y);
            symbolInfo.Set_Alignment(_alignement);
            symbolInfo.Set_Angle(0.0);
        }
    }
}
