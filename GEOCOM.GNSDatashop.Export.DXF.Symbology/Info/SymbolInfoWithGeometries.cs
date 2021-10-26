using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class SymbolInfoWithGeometries : List<SymbolInfoWithGeometry>
    {
        public void Add(IGeometry leaderGeometry, ISymbolInfo symbolInfo)
        {
            Add(new SymbolInfoWithGeometry(symbolInfo, leaderGeometry));
        }

        public byte Opacity => this.Max(e => e.SymbolInfo.Opacity);
    }

}
