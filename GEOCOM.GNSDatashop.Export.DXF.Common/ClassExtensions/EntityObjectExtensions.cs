using netDxf;
using System;
using System.Linq;
using netDxf.Entities;
using netDxf.Tables;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class EntityObjectExtensions
    {
        public static IEnumerable<string> Dump(this EntityObject eo)
        {
            switch (eo.Type)
            {
                case EntityType.Hatch: return Dump(eo as Hatch);
                case EntityType.LightWeightPolyline: return Dump(eo as LwPolyline);
                case EntityType.Insert: return Dump(eo as Insert);
                case EntityType.MText: return Dump(eo as MText);
            }
            return new List<string>() { eo.ToString() };
        }

        #region Dump hatch entity 
        public static IEnumerable<string> Dump(this Hatch ht)
        {
            foreach (var hb in ht.BoundaryPaths)
                foreach (var str in hb.Dump())
                    yield return str;
        }

        public static IEnumerable<string> Dump(this HatchBoundaryPath hbp)
        {
            var info = new List<string>() {
                $"Boundary Path - {hbp.Edges.Count}-Edges" };

            foreach (var edge in hbp.Edges)
                info.AddRange(edge.Dump());

            return info;
        }

        public static IEnumerable<string> Dump(this HatchBoundaryPath.Edge edge)
        {
            switch (edge.Type)
            {
                case HatchBoundaryPath.EdgeType.Arc: return Dump(edge as HatchBoundaryPath.Arc);
                case HatchBoundaryPath.EdgeType.Ellipse: return Dump(edge as HatchBoundaryPath.Ellipse);
                case HatchBoundaryPath.EdgeType.Line: return Dump(edge as HatchBoundaryPath.Line);
                case HatchBoundaryPath.EdgeType.Polyline: return Dump(edge as HatchBoundaryPath.Polyline);
                case HatchBoundaryPath.EdgeType.Spline: return Dump(edge as HatchBoundaryPath.Spline);
                default: return new List<string>();
            }
        }

        public static IEnumerable<string> Dump(this HatchBoundaryPath.Arc arc)
        {
            yield return $"ARC: {arc.Center.Dump("CP")} Radius:{arc.Radius} From:{arc.StartAngle} Ro:{arc.EndAngle} CCW:{arc.IsCounterclockwise}";
        }

        public static IEnumerable<string> Dump(this HatchBoundaryPath.Ellipse el)
        {
            yield return $"ELLIPSE: {el.Center.Dump("CP")} EndMAxis:{el.EndMajorAxis} From:{el.StartAngle} Ro:{el.EndAngle} CCW:{el.IsCounterclockwise} Min/Major:{el.MinorRatio}";
        }

        public static IEnumerable<string> Dump(this HatchBoundaryPath.Line ln)
        {
            yield return $"LINE: {ln.Start.Dump("From")} {ln.End.Dump("To")}";
        }
        public static IEnumerable<string> Dump(this HatchBoundaryPath.Polyline pl)
        {
            yield return $"POLYLINE - {pl.Vertexes.Count()} vertexes:{string.Join(",", pl.Vertexes.Select(v => v.Dump()))}";

        }
        public static IEnumerable<string> Dump(this HatchBoundaryPath.Spline spl)
        {
            yield return $"{Indicator(spl.IsPeriodic, "Periodic")} {Indicator(spl.IsRational, "Rational")} SPLINE. Nots:{spl.Knots.Dump()} ControlPoints: {spl.ControlPoints.Dump()}";
        }

        #endregion

        #region dump lwpolyline entity

        public static IEnumerable<string> Dump(this LwPolyline pl)
        {
            yield return $"{Indicator(pl.IsClosed, "Closed")} LWPOLYLINE: {pl.Vertexes.Count}-Vertexes, Thikness:{pl.Thickness}";
            foreach (var vt in pl.Vertexes)
                yield return Dump(vt);
        }

        public static string Dump(this LwPolylineVertex vt)
            => double.IsNaN(vt.Bulge) || (1E-6 > Math.Abs(vt.Bulge))
                ? $"{vt.Position.Dump()} Withs - from:{vt.StartWidth} to:{vt.EndWidth}"
                : $"{vt.Position.Dump()} Bulge: {vt.Bulge} Withs - from:{vt.StartWidth} to:{vt.EndWidth}";

        #endregion

        #region dump mtext entity

        public static IEnumerable<string> Dump(this MText txt)
        {
            yield return $"MTEXT: {txt.Value}, Style:{txt.Style.Dump()}";
        }

        #endregion

        #region dump textstyle table object
        public static string Dump(this TextStyle ts)
            => $"TEXTSTYLE {ts.Name} {ts.Height} - Family:{ts.FontFamilyName} (File:{ts.FontFile})";

        #endregion

        #region dump insert

        public static IEnumerable<string> Dump(this Insert ins)
        {
            yield return $"INSERT Block:{ins.Block.Name} {ins.Position.Dump("Position")} {ins.Scale.Dump("Scale")} {ins.Rotation}";
        }

        #endregion

        #region dump vector
        private static string Dump(this Vector2[] vec)
            => string.Join(",", vec.Select(v => v.Dump()));

        private static string Dump(this Vector3[] vec)
            => string.Join(",", vec.Select(v => v.Dump()));

        private static string Dump(this double[] values)
            => string.Join(",", values.Select(v => v.ToString()));

        public static string Dump(this Vector2 vec, string info = null)
            => !string.IsNullOrEmpty(info)
                ? $"{info}: X={vec.X} Y={vec.Y}"
                : $"X={vec.X} Y={vec.Y}";
        public static string Dump(this Vector3 vec, string info = null)
            => !string.IsNullOrEmpty(info)
                ? $"{info}: X={vec.X} Y={vec.Y}"
                : $"X={vec.X} Y={vec.Y}";
        #endregion

        #region dump list of values
        private static string Dump(this IEnumerable<double> val)
            => string.Join(",", val.Select(v => v.ToString()));
        #endregion

        #region dumping helpers
        private static string Indicator(bool ind, string trueLabel, string falseLabel = null)
            => ind
                ? trueLabel
                : falseLabel;

        #endregion
    }
}
