using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using System.Collections.Generic;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class AOLineTemplate : LineTemplate
    {
        public AOLineTemplate(ITemplate template) : base(template.LinePattern()) { }

        public IEnumerable<LineMarkerPosition> GetMarks(ICurve curve, double dotsToMeter)
        {
            var linearMarks = base.GetMarks(curve.Length, dotsToMeter);

            foreach (var mark in linearMarks)
                yield return new LineMarkerPosition(curve, mark.Position + mark.Length / 2);
        }
    }
}
