using System;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LineDecorationPosition
    {
        protected ICurve _curve;
        protected double _distance;
        protected bool _isFirst;

        public LineDecorationPosition(ICurve curve, double distance, bool isFirst)
        {
            _curve = curve;
            _distance = distance;
            _isFirst = isFirst;
        }

        public bool IsFirst => _isFirst;

        public IPoint ESRIPosition
        {
            get
            {
                var decorationPosition = new Point() as IPoint;
                _curve.QueryPoint(esriSegmentExtension.esriNoExtension, _distance, false, decorationPosition);

                return decorationPosition;
            }
        }

        public double Tangent
        {
            get
            {
                var tangentLine = new Line();
                _curve.QueryTangent(esriSegmentExtension.esriNoExtension, _distance, false, 1.0, tangentLine);

                var fp = tangentLine.FromPoint;
                var tp = tangentLine.ToPoint;
                return Math.Atan2(tp.Y - fp.Y, tp.X - fp.X) * 180 / Math.PI;
            }
        }
    }
}
