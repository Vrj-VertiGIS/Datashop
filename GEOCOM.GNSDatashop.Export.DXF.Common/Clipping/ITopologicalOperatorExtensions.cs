using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Clipping
{
    public static class ITopologicalOperatorExtensions
    {
        public static IGeometry BagAwareDifference(this ITopologicalOperator geometry, IGeometry other)
        {
            if (other.GeometryType == esriGeometryType.esriGeometryBag)
                return SubtractBag(geometry, other);
            else
                return geometry.Difference(other);
        }

        public static IGeometry BagAwareUnion(this ITopologicalOperator geometry, IGeometry other)
        {
            if (other.GeometryType == esriGeometryType.esriGeometryBag)
                return UnionWithBag(geometry, other);
            else
                return geometry.Union(other);
        }

        private static IGeometry SubtractBag(ITopologicalOperator geometry, IGeometry other)
        {
            var coll = other as IGeometryCollection;
            var nGeo = coll.GeometryCount;
            for (int i = 0; i < nGeo; i++)
                geometry = geometry.Difference(other) as ITopologicalOperator;
            return geometry as IGeometry;
        }

        public static IGeometry UnionWithBag(ITopologicalOperator geometry, IGeometry other)
        {
            var sum = (geometry as IClone).Clone() as ITopologicalOperator;
            sum.ConstructUnion(other as IEnumGeometry); // Will erase the former contents of sum
            sum.Union(geometry as IGeometry);
            return sum as IGeometry;
        }
    }
}
