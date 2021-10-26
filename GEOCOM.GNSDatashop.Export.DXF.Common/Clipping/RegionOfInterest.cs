using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Clipping
{
    public class RegionOfInterest
    {
        private IGeometry _region;
        private IGeometry _clipRegion;
        private IGeometry _mask;

        public IGeometry Region => _region;

        public IGeometry ClipRegion => _clipRegion;

        public IGeometry Mask
        {
            get { return _mask; }
        }

        public bool Selective
        {
            get { return ((null != _region) || (null != _mask)); }
        }

        private static readonly Dictionary<esriGeometryType, esriGeometryDimension> _clipDimension = new Dictionary<esriGeometryType, esriGeometryDimension>()
        {
            { esriGeometryType.esriGeometryPolygon, esriGeometryDimension.esriGeometry2Dimension },
            { esriGeometryType.esriGeometryPolyline, esriGeometryDimension.esriGeometry1Dimension },
            { esriGeometryType.esriGeometryPoint, esriGeometryDimension.esriGeometry0Dimension }
        };

        public RegionOfInterest(IGeometry region, IGeometry mask)
        {
            _region = region;   // Store the initial - unclipped/unprocessed - value

            _mask = (null != mask) ? (new Polygon() as ITopologicalOperator).BagAwareUnion(mask) : null;

            _clipRegion = (null != region)
                ? (null != _mask)
                    ? (region as ITopologicalOperator).BagAwareDifference(_mask)
                    : region
                : null;
        }

        public void PrepareQueryFilter(ISpatialFilter filter)
        {
            if (null != _clipRegion)
            {
                if (!_clipRegion.IsEmpty)
                {
                    filter.Geometry = _clipRegion;
                    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                }
                else if ((null != _mask) && (!_mask.IsEmpty))
                {
                    filter.Geometry = _region;  // Let the clipping routine do it's work...
                    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                }
                else
                {
                    filter.Geometry = null;
                    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelUndefined;
                }
            }
            else if (null != _mask)
            {
                filter.Geometry = _mask;
                filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                filter.SpatialRelDescription = "**T**T**T";
            }
            else
            {
                filter.Geometry = null;
                filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelUndefined;
            }
        }

        public IGeometry Clip(IGeometry geometry)
        {
            if (geometry is IGeometryBag geoBag)
                return Clip(geoBag);
            else
            {
                if (geometry is ITopologicalOperator topoOp)
                    if (null != _clipRegion)
                        if (!_clipRegion.IsEmpty)
                            return ((geometry is IRelationalOperator rop) && (rop.Within(_clipRegion))) 
                                ? geometry  // Do not clip if unnecessary - as clipping alters polygon ring point sequence
                                : topoOp.Intersect(_clipRegion, ResultDimension(geometry));
                        else  // Empty Selection Geometry - return empty geometry;
                            return topoOp.Difference(geometry); // Me minus myself
                    else if (null != _mask)
                        return ((geometry is IRelationalOperator rop) && (rop.Disjoint(_mask)))
                            ? geometry  // Do not clip if unnecessary - as clipping alters polygon ring point sequence
                            : topoOp.Difference(_mask);

                return geometry;
            }
        }

        private IGeometry Clip(IGeometryBag bag)
        {
            var geoColl = bag as IGeometryCollection;
            var newBag = new GeometryBag() as IGeometryCollection;
            for (int i = 0; (i < geoColl.GeometryCount); i++)
                newBag.AddGeometry(Clip(geoColl.Geometry[i]));
            return newBag as IGeometry;
        }

        private esriGeometryDimension ResultDimension(IGeometry geometry)
        {
            esriGeometryDimension rd;
            if (_clipDimension.TryGetValue(geometry.GeometryType, out rd))
                return rd;
            return esriGeometryDimension.esriGeometryNoDimension;
        }
    }
}
