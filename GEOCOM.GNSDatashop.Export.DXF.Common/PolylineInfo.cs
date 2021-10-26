using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class PolyCurveInfo 
    {
        private IPolycurve _polyCurve;
        private IZAware _zAware;

        public PolyCurveInfo(IPolycurve polyCurve, double? offset = null)
        {
            _polyCurve = polyCurve;
            _zAware = polyCurve as IZAware;
            Offset = offset;
        }

        /// <summary>
        /// Polyline offset (in points)
        /// </summary>
        public double? Offset { get; private set; } = null;

        /// <summary>
        /// Is the contained geometry a closed ring
        /// </summary>
        public bool ClosedRings => (_polyCurve is IPolygon) || (_polyCurve is IRing);

        public IEnumerable<PathInfo> Paths => Geometries.Select(g => new PathInfo(g as IPath, _zAware?.ZAware ?? false));

        public IPolycurve Curve => (OffsetToApply) ? _polyCurve.Offset(Offset.Value) : _polyCurve;

        public IEnumerable<IGeometry> Geometries
        {
            get
            {
                var geometries = Curve as IGeometryCollection;
                for (int i = 0; i < geometries.GeometryCount; i++)
                    yield return geometries.Geometry[i];
            }
        }

        #region private helpers

        private bool OffsetToApply => Offset.HasValue && (1E-6 < Math.Abs(Offset.Value));

        #endregion

    }
}
