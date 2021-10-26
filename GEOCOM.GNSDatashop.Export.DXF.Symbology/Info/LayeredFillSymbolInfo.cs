using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class LayeredFillSymbolInfo : List<FillSymbolInfo>, ISymbolInfo
    {
        public bool IsVisible => (this.Any(l => l.IsVisible));

        public byte Opacity => this.Max(e => e.Opacity);
    }
}
