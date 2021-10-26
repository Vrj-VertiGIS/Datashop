using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info;
using netDxf.Blocks;
using netDxf.Tables;
using System.Collections.Generic;
using System.Linq;
using Entities = netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{
    public class Dxf2DEntityFaxtory : DxfEntityFactory
    {
        public Dxf2DEntityFaxtory(Layer layer, double dotsToMeter)
            : base(layer, dotsToMeter)
        {
        }

        protected override Entities.Insert CreateBlockInsertCore(Block block, IPoint point)
            => new Entities.Insert(block, point.ToVector2());

        protected override Entities.EntityObject CreatePolylineCore(IEnumerable<IPointInfo> points, LinetypeLineSymbolInfo symbolInfo, bool closedRings)
        {
            var result = new Entities.LwPolyline();

            foreach (var ptInfo in points)
            {
                Entities.LwPolylineVertex vertex;

                if (ptInfo.Flags.HasFlag(PointInfoFlags.ArcPoint))
                    vertex = new Entities.LwPolylineVertex(ptInfo.X, ptInfo.Y, (ptInfo as ArcPointInfo).Bulge);
                else
                    vertex = new Entities.LwPolylineVertex(ptInfo.X, ptInfo.Y);

                if (symbolInfo.IsVisible)
                {
                    vertex.StartWidth = symbolInfo.WidthInDots * _dotsToMeter;
                    vertex.EndWidth = symbolInfo.WidthInDots * _dotsToMeter;
                }
                result.Vertexes.Add(vertex);
            }

            result.IsClosed = closedRings;

            return result;
        }

        protected override IEnumerable<Entities.HatchBoundaryPath> CreateHatchCore(IEnumerable<Entities.EntityObject> rings)
        {
            var listOfRings = rings.Select(ring => { var l = new List<Entities.LwPolyline>(1); l.Add(ring as Entities.LwPolyline); return l; });
            return listOfRings.Select(ring => new Entities.HatchBoundaryPath(ring));
        }

    }
}
