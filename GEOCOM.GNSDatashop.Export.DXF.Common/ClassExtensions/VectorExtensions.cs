using ESRI.ArcGIS.Geometry;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class VectorExtensions
    {

        public static IPoint ToAOPoint(this Vector2 vector)
        {
            var pt = NewPoint(false);
            pt.X = vector.X;
            pt.Y = vector.Y;
            return pt;
        }

        public static IPoint ToAOPoint(this Vector3 vector)
        {
            var pt = NewPoint(true);
            pt.X = vector.X;
            pt.Y = vector.Y;
            pt.Z = vector.Z;
            return pt;
        }

        public static bool LineLineIntersection(Vector2 line1From, Vector2 line1To, Vector2 line2From, Vector2 line2To, out Vector2 intersectionPoint)
        {
            var line1Vector = line1To - line1From;
            var line2Vector = (-1) * (line2To - line2From);
            var lineOffset = line2From - line1From;

            if (MathSnippets.Equation(line1Vector.X, line2Vector.X, line1Vector.Y, line2Vector.Y, lineOffset.X, lineOffset.Y, out double ipX, out double ipY))
            {
                intersectionPoint = new Vector2(line1From.X + ipX * line1Vector.X,
                                                line1From.Y + ipX * line1Vector.Y);
                return true;
            }
            else
            {
                intersectionPoint = new Vector2();
                return false;
            }
        }

        private static IPoint NewPoint(bool zAware)
        {
            var pt = new Point();
            if (pt is IZAware za)
                za.ZAware = zAware;
            return pt;
        }

    }
}
