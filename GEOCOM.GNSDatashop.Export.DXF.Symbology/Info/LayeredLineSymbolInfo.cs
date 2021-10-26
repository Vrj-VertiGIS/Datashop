using System.Collections.Generic;
using System.Linq;

using netDxf.Objects;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LayeredLineSymbolInfo : List<LineSymbolInfo>, ILineSymbolInfo

    {
        /// <summary>
        /// A line symbol layer is regarded as visible if it is not completely covered by another line 
        /// symbol layer (boldness). Keep original layer order (where thinner lines might be 
        /// covered by bolder ones).
        /// </summary>
        public IEnumerable<LineSymbolInfo> LayersByVisibility
        {
            get
            {
                return this.Where(sl => sl.IsVisible);
            }
        }

        public bool IsVisible => (this.Any(l => l.IsVisible));

        public byte Opacity => this.Max(s => s.Opacity);

    }
}
