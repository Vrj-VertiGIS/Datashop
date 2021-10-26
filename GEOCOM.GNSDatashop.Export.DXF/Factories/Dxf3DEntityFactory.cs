using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf;
using netDxf.Blocks;
using netDxf.Tables;
using System.Collections.Generic;
using System.Linq;
using Entities = netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{
    public class Dxf3DEntityFactory : DxfEntityFactory
    {
        public Dxf3DEntityFactory(Layer layer, double dotsToMeter)
            : base(layer, dotsToMeter)
        {
        }

        protected override Entities.Insert CreateBlockInsertCore(Block block, IPoint point)
            => new Entities.Insert(block, point.ToVector3());

        protected override Entities.EntityObject CreatePolylineCore(IEnumerable<IPointInfo> points, LinetypeLineSymbolInfo symbolInfo, bool ClosedRings)
        {
            var result = new Entities.Polyline() { Linetype = symbolInfo.LineType, Color = symbolInfo.DXFColor, Lineweight = Lineweight.W211};

            foreach (var ptInfo in points)
            {
                var vertex = new Entities.PolylineVertex(ptInfo.X, ptInfo.Y, (ptInfo.Z.HasValue) ? ptInfo.Z.Value : 0.0);
                result.Vertexes.Add(vertex);
            }

            result.IsClosed = ClosedRings;
            result.LinetypeScale = (0 < symbolInfo.WidthInDots) 
                ? _dotsToMeter * symbolInfo.WidthInDots
                : 1.0;  // abuse! - 3D Polyline will be exploded to simple lines for writing

            return result;
        }

        protected override IEnumerable<Entities.HatchBoundaryPath> CreateHatchCore(IEnumerable<Entities.EntityObject> rings)
        {
            var listOfRings = rings.Select(ring => 
            {
                var lwPl = new Entities.LwPolyline() { Elevation = 0.0 };
                foreach (var threeDVertex in (ring as Entities.Polyline).Vertexes)
                {
                    var pos = threeDVertex.Position;
                    lwPl.Vertexes.Add(new Entities.LwPolylineVertex(pos.X, pos.Y));
                    lwPl.Elevation += pos.Z;
                }
                lwPl.Elevation /= lwPl.Vertexes.Count;

                var l = new List<Entities.EntityObject>(1);
                l.Add(lwPl);
                return l;
            });

            return listOfRings.Select(ring => new Entities.HatchBoundaryPath(ring));
        }

    }
}
