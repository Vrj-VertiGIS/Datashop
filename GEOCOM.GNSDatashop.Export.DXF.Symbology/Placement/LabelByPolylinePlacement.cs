using System;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement
{
    public class LabelByPolylinePlacement : LabelPlacement
    {
        private IPolyline _referenceLine;

        public LabelByPolylinePlacement(IPolyline referenceLine, HVAlignment alignement)
            : base(alignement)
        {
            _referenceLine = referenceLine;
        }

        public override void Apply(TextSymbolInfo symbolInfo)
        {
            switch (_alignement.HAlignment) {
                case esriTextHorizontalAlignment.esriTHALeft:
                    symbolInfo.ReferencePoint = GetLeftAlignedReferencePoint();
                    break;
                case esriTextHorizontalAlignment.esriTHARight:
                    symbolInfo.ReferencePoint = GetRightAlignedReferencePoint();
                    break;
                case esriTextHorizontalAlignment.esriTHACenter:
                    symbolInfo.ReferencePoint = GetCenterAlignedReferencePoint();
                    break;
                case esriTextHorizontalAlignment.esriTHAFull:
                    symbolInfo.ReferencePoint = GetCenterAlignedReferencePoint();
                    break;
                default:
                    symbolInfo.ReferencePoint = GetLeftAlignedReferencePoint();
                    break;
            }
            symbolInfo.Set_Alignment(_alignement);
            symbolInfo.Set_Angle(GetAngle());
            symbolInfo.Set_TextColumnWidth(_referenceLine.Length);
        }

        private Vector2 GetLeftAlignedReferencePoint()
        {
            var pt = _referenceLine.FromPoint;
            return new Vector2(pt.X, pt.Y);
        }

        private Vector2 GetRightAlignedReferencePoint()
        {
            var pt = _referenceLine.ToPoint;
            return new Vector2(pt.X, pt.Y);
        }

        private Vector2 GetCenterAlignedReferencePoint()
        {
            var ptf = _referenceLine.FromPoint;
            var ptt = _referenceLine.ToPoint;
            return new Vector2(ptf.X + (ptt.X - ptf.X) / 2, ptf.Y + (ptt.Y - ptf.Y) / 2);
        }

        private double GetAngle()
        {
            var ptf = _referenceLine.FromPoint;
            var ptt = _referenceLine.ToPoint;
            var x = ptt.X - ptf.X;
            var y = ptt.Y - ptf.Y;
            return Math.Atan2(y, x) * 180 / Math.PI;
        }
    }
}
