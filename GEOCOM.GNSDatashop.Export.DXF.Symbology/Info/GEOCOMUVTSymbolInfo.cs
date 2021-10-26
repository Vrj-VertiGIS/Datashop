using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    /// <summary>
    /// Handling GEOCOM unique value textrenderer symbols
    /// There are 2 symbols per feature - a text symbol representing the label to be drawn
    /// and a line symbol pointing from the label to the labelled feature (in case of larger
    /// label offsets - perhaps due to denser/crowded maps).
    /// </summary>
    public class GEOCOMUVTSymbolInfo : TextSymbolInfo
    {
        public GEOCOMUVTSymbolInfo()
        {
            throw new System.Exception("Not to be invoked");
        }

        public GEOCOMUVTSymbolInfo(ISymbol textSymbol, ILayer layer)
            : base(textSymbol, layer)
        {
        }

        public ILineSymbol PointerLineSymbol
        {
            get => base._leaderLineSymbol;
            set => base._leaderLineSymbol = value;
        }
    }
}
