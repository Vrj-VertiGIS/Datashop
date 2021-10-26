
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement
{
    public class LabelByPointPlacement : LabelPlacement
    {
        private IPoint _referencePoint;
        
        public LabelByPointPlacement(IPoint referencePoint, HVAlignment alignement)
            : base(alignement)
        {
            _referencePoint = referencePoint;
        }

        public override void Apply(TextSymbolInfo symbolInfo)
        {
            symbolInfo.ReferencePoint = new Vector2(_referencePoint.X, _referencePoint.Y);
        }
    }
}
