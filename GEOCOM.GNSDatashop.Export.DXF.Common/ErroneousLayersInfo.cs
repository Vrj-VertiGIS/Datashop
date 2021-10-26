using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{

    public class ErroneousLayersInfo : List<ErroneousLayerInfo>
    {
        private string LayerList(bool extendedInfo, bool withNewLines)
        {
            Sort();
            return (10 >= Count)
                ? string.Join("\n", this.Select(l => l.ToString(extendedInfo)))
                : $"{string.Join("\n", this.Take(10).Select(l => l.ToString(extendedInfo)))}\n...";
        }

        public string ToString(bool extended, bool withNewLines)
            => LayerList(extended, withNewLines);
    }
}
