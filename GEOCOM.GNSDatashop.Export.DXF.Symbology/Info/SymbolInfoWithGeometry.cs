using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    public class SymbolInfoWithGeometry : ISymbolInfo
    {
        public ISymbolInfo SymbolInfo { get; set; }
        public IGeometry Geometry { get; set; }

        public bool IsVisible => SymbolInfo?.IsVisible ?? false;

        public byte Opacity => SymbolInfo?.Opacity ?? 0;

        public SymbolInfoWithGeometry(ISymbolInfo symbolInfo, IGeometry geometry)
        {
            SymbolInfo = symbolInfo;
            Geometry = geometry;
        }
    }
}
