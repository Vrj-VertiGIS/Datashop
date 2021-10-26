using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public static class ExportSelection
    {
        /// <summary>
        /// Get clip geometries selecting the elements (features) which shall be exported to DXF 
        /// </summary>
        /// <param name="layer">Layer defining selection polygons - if none - everything will be exported</param>
        /// <param name="map">Focus map </param>
        /// <param name="clipToViewExtent">Limit export to the features displayed on the user's screen</param>
        /// <returns></returns>
        public static IEnumerable<IGeometry> GetROI(ILayer layer, IMap map, bool clipToViewExtent)
        {
            var roi = GeometriesOfLayer(layer);

            return (clipToViewExtent)
                ? ClipToActiveView(roi, map as IActiveView)
                : roi;
        }

        private static IEnumerable<IGeometry> ClipToActiveView(IEnumerable<IGeometry> roi, IActiveView view)
        {
            return (null != view) 
                ? ClipToExtent(roi, view.Extent)
                : roi;
        }

        private static IEnumerable<IGeometry> ClipToExtent(IEnumerable<IGeometry> roi, IEnvelope clipExtent)
        {
            return (null != clipExtent) 
                ? (0 < roi.Count())
                    ? roi.Select(g => ((ITopologicalOperator)g).Intersect(clipExtent, esriGeometryDimension.esriGeometry2Dimension)).Where(g => !g.IsEmpty)
                    : new List<IGeometry> { Polygon(clipExtent) } // Simply narrow down to screen extent
                : roi;
        }

        /// <summary>
        /// Simple - 1 ringed - polygon from the points envelope
        /// We need to have the envelope as a geometry supporting ITopologicalOperator for later 
        /// intersection operations in the exporter.
        /// </summary>
        ///
        private static IGeometry Polygon(IEnvelope env)
        {
            var points = new List<IPoint>() { env.UpperLeft, env.UpperRight, env.LowerRight, env.LowerLeft, env.UpperLeft };

            return Polygon(points);
        }

        /// <summary>
        /// Simple - 1 ringed - polygon from the points given as argument 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static IGeometry Polygon(IEnumerable<IPoint> points)
        {
            var ring = new RingClass() as IPointCollection;
            var poly = new PolygonClass() as IGeometryCollection;

            poly.AddGeometry(ring as IGeometry);

            foreach (var point in points)
                ring.AddPoint(point);

            var polytOp = poly as ITopologicalOperator3;
            polytOp.IsKnownSimple_2 = false;
            polytOp.Simplify();           

            return poly as IGeometry;
        }


        /// <summary>
        /// Get masking geometries. Everything under a maskng polygon is exluded from the export. Elements
        /// are clipped a the edges of a masing polygon
        /// </summary>
        /// <param name="maskingLayer">Layer containing masking polygons.</param>
        /// <returns></returns>
        public static IGeometry GetMasks(ILayer maskingLayer)
        {
            var geoColl = new GeometryBag() as IGeometryCollection;
            var masks = GeometriesOfLayer(maskingLayer);
            foreach (var mask in masks)
                geoColl.AddGeometry(mask);
            return geoColl as IGeometry;
        }

        private static IEnumerable<IGeometry> GeometriesOfLayer(ILayer layer)
            => (null != layer) && (layer is IFeatureLayer fl)
                ? GeometriesOfLayerCore(fl)
                : new List<IGeometry>();

        private static IEnumerable<IGeometry> GeometriesOfLayerCore(IFeatureLayer fl)
        {
            var crs = fl.Search(null, true);
            for (var rw = crs.NextFeature(); rw != null; rw = crs.NextFeature())
                yield return rw.ShapeCopy;
        }
    }
}
