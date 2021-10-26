using System;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class LineMarkerPosition
    {
        protected ICurve _curve;
        protected double _distance;
        protected bool _isFirst;

        /// <summary>
        /// create linemaker psotion . give point at distance [m] from start of curve.
        /// remember information about whether this is the first marker on the line.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="distance"></param>
        /// <param name="isFirst"></param>
        public LineMarkerPosition(ICurve curve, double distance, bool isFirst)
        {
            _curve = curve;
            _distance = distance;
            _isFirst = isFirst;
        }


        /// <summary>
        /// create linemaker psotion . give point at distance [m] from start of curve.
        /// No information about wether this is the first marker on the line
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="distance"></param>
        public LineMarkerPosition(ICurve curve, double distance) : this(curve, distance, false) { }

        public bool IsFirst => _isFirst;    // First marker on the curve (simple flag passed from caller).

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

                return MathSnippets.NormalizeDeg(Math.Atan2(tp.Y - fp.Y, tp.X - fp.X) * 180 / Math.PI);
            }
        }

    }
}
