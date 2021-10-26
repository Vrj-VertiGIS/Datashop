using ESRI.ArcGIS.Geometry;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IPolyCurveExtensions
    {
        public static IPolycurve Offset(this IPolycurve curve, double offset)
        {
            return (1E-6 < Math.Abs(offset))
                ? ApplyOffset(curve, offset)
                : curve;
        }

        private static IPolycurve ApplyOffset(IPolycurve polyCurve, double offset)
        {
            object how = esriConstructOffsetEnum.esriConstructOffsetSimple;
            object missing = Type.Missing;

            var offsetCurve = (polyCurve is IPolygon)
                ? new PolygonClass() as IConstructCurve
                : new PolylineClass() as IConstructCurve;

            offsetCurve.ConstructOffset(polyCurve, (-1) * offset, ref how, ref missing);

            return offsetCurve as IPolycurve;
        }
    }
}
