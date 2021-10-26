using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class ExportedLayersInfo : List<ExportedLayerInfo>
    {
        public override string ToString()
            => string.Join("\n", LayerInfo);

        public IEnumerable<string> LayerInfo
            => this.Select(l => l.ToString());
    }
}
