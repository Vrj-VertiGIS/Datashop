using System.Collections.Generic;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.WorkArounds
{
    public static class WorkArounds
    {

        public static IEnumerable<IEnumerable<EntityObject>> ExplodePolyline(IEnumerable<IEnumerable<EntityObject>> boundaries)
        {
            foreach (var boundary in boundaries)
                yield return ExplodePolyline(boundary);
        }

        public static IEnumerable<EntityObject> ExplodePolyline(IEnumerable<EntityObject> boundary)
        {
            foreach (Polyline path in boundary)
            {
                foreach (Line line in path.Explode())
                {
                    line.Thickness = line.LinetypeScale;    // The Polyline type does not have a "Thickness" member...
                    line.Lineweight = Lineweight.W211;
                    line.LinetypeScale = 1.0;
                    line.Color = path.Color;
                    line.Linetype = path.Linetype;
                    yield return line;
                }
            }
        }

        public static IEnumerable<EntityObject> ExplodePolyline(EntityObject boundary)
        {
            if (boundary is Polyline pl)
            {
                foreach (Line line in pl.Explode())
                {
                    line.Thickness = line.LinetypeScale;    // The Polyline type does not have a "Thickness" member...
                    line.Lineweight = Lineweight.W211;
                    line.LinetypeScale = 1.0;
                    line.Color = pl.Color;
                    line.Linetype = pl.Linetype;
                    yield return line;
                }
            }
        }

    }
}
