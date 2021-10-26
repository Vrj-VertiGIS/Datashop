using System;
using System.Linq;
using netDxf;
using netDxf.Entities;
using System.Collections.Generic;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public interface IPathInfo
    {
        IEnumerable<ISegment> Segments { get; }
        IEnumerable<IPointInfo> Points { get; }
    }

    public class PathInfo : IPathInfo
    {
        private ISegmentCollection _segments;
        private bool _is3D = false;


        public PathInfo(IPath path, bool is3D=false)
        {
            _segments = path as ISegmentCollection;
            _is3D = is3D;
        }

        public IEnumerable<ISegment> Segments
        {
            get
            {
                for (int i = 0; i < _segments.SegmentCount; i++)
                    yield return _segments.Segment[i];
            }
        }

        public IEnumerable<IPointInfo> Points
            => ToPoints(Segments);

        private IEnumerable<IPointInfo> ToPoints(IEnumerable<ISegment> segments)
        {
            IPoint segEndPoint = null; // segment endpoint - will not be set unless we're at the end of the path/ring
            foreach (var seg in segments)
            {
                if (seg.GeometryType.Equals(esriGeometryType.esriGeometryCircularArc))
                {
                    var ca = seg as ICircularArc;
                    if (ca.FromPoint.IsEqual(ca.ToPoint))   // might be seg.IsClosed in 2D, but not in 3D...
                    {
                        var op = ca.FromPoint.Mirror(ca.CenterPoint);
                        if (seg.Is3D())
                            op.Z = ca.FromPoint.Z + 0.5 * (ca.ToPoint.Z - ca.FromPoint.Z);
                        var bl = Bulge(ca.CentralAngle / 2);
                        yield return ArcPoint(seg.FromPoint, bl);
                        yield return ArcPoint(op, bl);
                    }
                    else
                        yield return ArcPoint(seg.FromPoint, Bulge(ca.CentralAngle));
                }
                else if ((seg.GeometryType == esriGeometryType.esriGeometryEllipticArc)
                    || (seg.GeometryType == esriGeometryType.esriGeometryBezier3Curve))
                {
                    foreach (var ellipsePointInfo in ToPoints(seg.Densify()))
                        yield return ellipsePointInfo;
                }

                else
                    yield return LinePoint(seg.FromPoint);

                segEndPoint = seg.ToPoint;
            }
            if (segEndPoint != null)
                yield return LinePoint(segEndPoint);
        }

        private static double Bulge(double ca) => Math.Tan(ca / 4);

        private ArcPointInfo ArcPoint (IPoint point, double bulge)
        {
            return (_is3D)
                ? new ArcPointInfo(point.X, point.Y, point.Z, bulge)
                : new ArcPointInfo(point.X, point.Y, bulge);
        }

        private PointInfo LinePoint(IPoint point)
        {
            return (_is3D)
                ? new PointInfo(point.X, point.Y, point.Z)
                : new PointInfo(point.X, point.Y);
        }
    }
}
