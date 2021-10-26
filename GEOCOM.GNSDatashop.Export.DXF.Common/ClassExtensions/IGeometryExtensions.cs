using ESRI.ArcGIS.Geometry;
using netDxf;
using System;
using System.Linq;
using System.Collections.Generic;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IGeometryExtensions
    {
        /// <summary>
        /// An epsilon value to compare double precision coordinates for equality
        /// It's current value is a simple estimate.
        /// </summary>
        private readonly static double EPSILON = 1E-06;

        /// <summary>
        /// We need this in certain cases (densifry tolerance).
        /// Will be update by Init() from DxfLayerWriterBase class.
        /// </summary>
        public static double DotsToMeter { get; private set; }

        /// <summary>
        /// Perform an "on-per-layer"init. Keep some values (i.e. dots to meter conversion factor)
        /// </summary>
        /// <param name="dotsToMeter"></param>
        public static void Init(double dotsToMeter)
        {
            DotsToMeter = dotsToMeter;
        }

        /// <summary>
        /// Compare 2 doubles and take into account that there are issues with precision
        /// and preassigned values with certaing meanings (infinity, -infinity, NAN,...)
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private static bool IsEqual(this double v, double w)
            => (!double.IsNaN(v)) && (!double.IsInfinity(v)) && (!double.IsNegativeInfinity(v))
                ? (!double.IsNaN(w)) && (!double.IsInfinity(w)) && (!double.IsNegativeInfinity(w))
                    ? EPSILON > Math.Abs(v - w)
                    : false                     // v a real value, w ininity, -infinity,...
                : v.Equals(w);                  // v infinity, w might be or not

        /// <summary>
        /// DotNet like IsNullOrEmpty() checking if reference passed is null or has empty set
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IGeometry geometry) => (null == geometry) || (geometry.IsEmpty);
        public static bool IsNullOrEmpty(this IPoint point) => (null == point) || (point.IsEmpty);
        public static bool HasNullXYCoordinates(this IPoint point) => MathHelper.IsZero(point.X) && MathHelper.IsZero(point.Y);

        public static Vector2 ToVector2(this IPoint point) => new Vector2(point.X, point.Y);

        public static Vector3 ToVector3(this IPoint point) => (point is IZAware za) && (za.ZAware)
            ? new Vector3(point.X, point.Y, point.Z)
            : new Vector3(point.X, point.Y, 0.0);

        /// <summary>
        /// Determine if a given point has 3D coordinates
        /// Only top-level geometries support IZAware
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Is3D(this IPoint p)
            => (p is IZAware zp) && zp.ZAware;

        public static bool Is3D(this IPolyline p)
            => (p is IZAware zp) && zp.ZAware;

        public static bool Is3D(this IPolygon p)
            => (p is IZAware zp) && zp.ZAware;

        public static bool Is3D(this IEnvelope p)
            => (p is IZAware zp) && zp.ZAware;

        public static bool Is3D(this ISegment seg)
            => (seg.FromPoint.Is3D());

        /// <summary>
        /// Point equals other. Cannot override Equals() in class extension
        /// A point is regarded to be equal to an other point if both
        /// have x/y pairs not differing more than a given epsilon (in both ordinates)
        /// and if they both have a z value (not differing more than the epsilon)
        /// or if they both are only 2D
        /// </summary>
        /// <param name="p"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsEqual(this IPoint p, IPoint other)
            => p.IsEqual2D(other) && p.IsEqualZ(other);

        public static bool IsEqual2D(this IPoint p, IPoint other)
            => p.X.IsEqual(other.X) && p.Y.IsEqual(other.Y);

        private static bool IsEqualZ(this IPoint p, IPoint other)
            => p.Is3D()
                ? other.Is3D()
                    ? p.Z.IsEqual(other.Z)
                    : false                     // p 3D, other only 2D
                : !other.Is3D();                // equal if both have no z coordinate at all

        /// <summary>
        /// Point a + point b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IPoint Add(this IPoint a, IPoint b)
        {
            var a3D = a.Is3D();
            var b3D = b.Is3D();
            var is3D = a3D || b3D;
            var pt = NewPoint(is3D);
            pt.X = a.X + b.X;
            pt.Y = a.Y + b.Y;
            if (is3D)
                pt.Z = ((a3D) ? a.Z : 0.0) + ((b3D) ? b.Z : 0.0);
            return pt;
        }

        /// <summary>
        /// Add deltas to point
        /// If target point (a) is 3D, dz is added if dz is not NAN.
        /// </summary>
        /// <param name="a">point to be added to</param>
        /// <param name="dx">x offset</param>
        /// <param name="dy">y offset</param>
        /// <param name="dz">z offset</param>
        /// <returns></returns>
        public static IPoint Add(this IPoint a, double dx, double dy, double dz=double.NaN)
        {
            var is3D = a.Is3D();
            var pt = NewPoint(is3D);
            pt.X = a.X + dx;
            pt.Y = a.Y + dy;
            if (is3D)
                pt.Z = (double.IsNaN(dz)) ? a.Z : a.Z + dz;
            return pt;
        }

        /// <summary>
        /// Point a - point b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IPoint Subtract(this IPoint a, IPoint b)
        {
            var a3D = a.Is3D();
            var b3D = b.Is3D();
            var is3D = a3D || b3D;
            var pt = NewPoint(is3D);
            pt.X = a.X - b.X;
            pt.Y = a.Y - b.Y;
            if (is3D)
                pt.Z = ((a3D) ? a.Z : 0.0) - ((b3D) ? b.Z : 0.0);
            return pt;
        }

        /// <summary>
        /// Multiply vector by scalar
        /// </summary>
        /// <param name="a"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static IPoint Multiply(this IPoint a, double factor)
        {
            var is3D = a.Is3D();
            var pt = NewPoint(is3D);
            pt.X = a.X * factor;
            pt.Y = a.Y * factor;
            if (is3D)
                pt.Z = a.Z * factor;
            return pt;
        }

        /// <summary>
        /// Scalar division of a vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static IPoint Divide(this IPoint a, double divisor) => (Math.Abs(divisor) > 1E-6) ? Multiply(a, 1 / divisor) : Multiply(a, 0);

        /// <summary>
        /// Magnitude of direction vector to point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Magnitude(this IPoint point) => point.Is3D()
            ? Math.Sqrt(Sqr(point.X) + Sqr(point.Y) + Sqr(point.Z))
            : Math.Sqrt(Sqr(point.X) + Sqr(point.Y));

        /// <summary>
        /// Create a point offset distance units from the coordinate origin in the direction
        /// given by (the non-normalized) vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IPoint Offset(this IPoint vector, double offset)
        {
            var unity = vector.Divide(vector.Magnitude());
            return unity.Multiply(offset);
        }

        /// <summary>
        /// Intersection of to simple lines (line segments). Nothing els than straight lines will work heere!
        /// (no Arc, no Beziers, no "nothing else").
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static IPoint SimpleLineLineIntersection(this ILine line1, ILine line2)
            => line1.SimpleLineLineIntersection(line2.FromPoint, line2.ToPoint);

        public static IPoint SimpleLineLineIntersection(this ILine line1, IPoint from2, IPoint to2)
            => SimpleLineLineIntersection(line1.FromPoint, line1.ToPoint, from2, to2);

        public static IPoint SimpleLineLineIntersection(IPoint from1, IPoint to1, IPoint from2, IPoint to2)
        {
            // Don't use IPoint extensions to calculate - speed (i.e. unneded 3'th dimension)
            var ra00 = to1.X - from1.X;
            var ra01 = (-1) * (to2.X - from2.X);
            var ra10 = to1.Y - from1.Y;
            var ra11 = (-1) * (to2.Y - from2.Y);
            var rc00 = from2.X - from1.X;
            var rc10 = from2.Y - from1.Y;
            return (MathSnippets.Equation(ra00, ra01, ra10, ra11, rc00, rc10, out double x00, out double x10))
                ? new Point() { X = from1.X + x00 * ra00, Y = from1.Y + x00 * ra10 }
                : new Point();
        }

        /// <summary>
        /// Auch Lotfusspunkt genannt...
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pointtoProject"></param>
        /// <returns></returns>
        public static IPoint ProjectPointOnLine(this IPoint queryPoint, IPoint lineFrom, IPoint lineTo)
        {
            var perpendicularVector = lineTo.Subtract(lineFrom).PerpendicularLeft();
            return SimpleLineLineIntersection(lineFrom, lineTo, queryPoint, queryPoint.Add(perpendicularVector));
        }

        /// <summary>
        /// Point regarded as direction vector - turn perpendicular right
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IPoint PerpendicularLeft(this IPoint point) => new Point() { X = (-1) * point.Y, Y = point.X };

        /// <summary>
        /// Point regarded as esriTransformDirection vector - turn perpendicular left
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IPoint PerpendicularRight(this IPoint point) => new Point() { X = point.Y, Y = (-1) * point.X };

        /// <summary>
        /// Mirror point by the origin
        /// 2D operation only
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IPoint Mirror(this IPoint point) => new Point() { X = (-1) * point.X, Y = (-1) * point.Y };

        /// <summary>
        /// Mirror point by a an arbitrary point
        /// 2D only.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static IPoint Mirror(this IPoint point, IPoint origin)
            => new Point() { X = point.X + 2 * (origin.X - point.X), Y = point.Y + 2 * (origin.Y - point.Y) };

        /// <summary>
        /// For handyness - return an arcObjects geometry collection as enumerable/enumerator
        /// </summary>
        /// <param name="geometries"></param>
        /// <returns></returns>
        public static IEnumerable<IGeometry> Geometries(this IGeometryCollection geometries)
        {
            var nGeometries = geometries?.GeometryCount ?? 0;
            for (int i = 0; i < nGeometries; i++)
                yield return geometries.Geometry[i];
        }

        /// <summary>
        /// For handyness - return an arcObjects segment collection as enumerable/enumerator
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static IEnumerable<ISegment> Segments(this ISegmentCollection segments)
        {
            var nSegments = segments?.SegmentCount ?? 0;
            for (int i = 0; i < nSegments; i++)
                yield return segments.Segment[i];
        }

        /// <summary>
        /// Segments to a path
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static IPath ToPath(this IEnumerable<ISegment> segments)
        {
            var path = new PathClass();
            foreach (var seg in segments)
                path.AddSegment(seg);
            return path;
        }

        /// <summary>
        /// Points to a path
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPath ToPath(this IEnumerable<IPoint> points)
        {
            var path = new PathClass();
            foreach (var pt in points)
                path.AddPoint(pt);
            return path;
        }

        /// <summary>
        /// Paths to a polyline
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IPolyline ToPolyline(this IEnumerable<IPath> paths)
        {
            var pl = new PolylineClass();
            foreach (var path in paths)
                pl.AddGeometry(path);
            return pl;
        }

        /// <summary>
        /// Segmeents to a 1-path polyline
        /// </summary>
        /// <param name="segs"></param>
        /// <returns></returns>
        public static IPolyline ToPolyline(this IEnumerable<ISegment> segs)
            => new List<IPath>() { segs.ToPath() }.ToPolyline();

        /// <summary>
        /// Points to a 1-path polyline
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPolyline ToPolyline(this IEnumerable<IPoint> points)
            => new List<IPath>() { points.Distinct(new PointEqualityComparer()).ToPath() }.ToPolyline();

        /// <summary>
        /// Densify a single segment
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        public static IEnumerable<ISegment> Densify(this ISegment seg)
        {
            var pc = new List<ISegment>() { seg }.ToPolyline() as IPolycurve;

            // Densify to below 1 Dot precision when printed/displayed at the reference scale
            pc.Densify(double.MaxValue, 0.5 * DotsToMeter);    

            foreach (var geo in ((IGeometryCollection)pc).Geometries())
                foreach (var sgmnt in ((ISegmentCollection)geo).Segments())
                    yield return sgmnt;
        }

        public static IEnumerable<Tuple<double, double, double>> Positions(this ITextPath path)
        {
            path.Reset();
            for (path.Next(out double x, out double y, out double angle); Math.Abs(x) > 1E-6 && Math.Abs(y) > 1E-6; path.Next(out x, out y, out angle))
                yield return new Tuple<double, double, double>(x, y, angle);
        }

        #region compare geometries

        public class PointEqualityComparer : IEqualityComparer<IPoint>
        {
            public bool Equals(IPoint x, IPoint y)
                => 0 == x.Compare(y);

            public int GetHashCode(IPoint obj)
                => obj.X.GetHashCode();

        }
        #endregion


        #region helpers
        private static double Sqr(double a) => a * a;

        private static double Pythagoras(IPoint p1, IPoint p2)
            => Math.Sqrt(Sqr(p2.X - p1.X) + Sqr(p2.Y - p1.Y));

        private static IPoint NewPoint(bool zAware)
        {
            var pt = new Point();
            if (pt is IZAware za)
                za.ZAware = zAware;
            return pt;
        }

        #endregion
    }
}
