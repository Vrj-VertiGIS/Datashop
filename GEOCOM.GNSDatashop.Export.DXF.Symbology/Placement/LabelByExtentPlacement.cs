using System;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Placement
{
    public class LabelByExtentPlacement : LabelPlacement
    {
        private IEnvelope _referenceEnvelope;

        public LabelByExtentPlacement(IEnvelope referenceEnvelope, HVAlignment alignement)
            : base(alignement)
        {
            _referenceEnvelope = referenceEnvelope;
        }

        public override void Apply(TextSymbolInfo symbolInfo)
        {
            switch (_alignement.HAlignment)
            {
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
        }

        private Vector2 GetLeftAlignedReferencePoint()
        {
            return new Vector2(_referenceEnvelope.XMin, YMiddle());
        }

        private Vector2 GetRightAlignedReferencePoint()
        {
            return new Vector2(_referenceEnvelope.XMax, YMiddle());
        }

        private Vector2 GetCenterAlignedReferencePoint()
        {
            return new Vector2(XMiddle(), XMiddle());
        }

        private double GetAngle()
        {
            var ptf = _referenceEnvelope.LowerLeft; 
            var ptt = _referenceEnvelope.LowerRight;
            var x = ptt.X - ptf.X;
            var y = ptt.Y - ptf.Y;
            return Math.Atan2(y, x);
        }

        private double XMiddle()
        {
            return _referenceEnvelope.XMin + (_referenceEnvelope.XMax - _referenceEnvelope.XMin) / 2.0;
        }

        private double YMiddle()
        {
            return _referenceEnvelope.YMin + (_referenceEnvelope.YMax - _referenceEnvelope.YMin) / 2.0;
        }
    }
}
