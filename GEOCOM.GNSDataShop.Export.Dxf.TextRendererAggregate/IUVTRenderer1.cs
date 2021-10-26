using System.Runtime.InteropServices;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace GEOCOM.GNSDatashop.Export.DXF.TextRendererAggregate
{
    // note: Do not change the GUID under any circumstances!
    [Guid("E28227A0-588E-4D38-9B98-58169D58F1E0")]
    [ComVisible(true)]
    public interface IUVTRenderer1
    {
        bool setPointerLineSymb(bool showPointerLine, ISymbol defaultLineSymbol);
        bool getPointerLineSymb(ref ISymbol defaultLineSymbol);
        ISymbol AnchorPointSymbol { get; set; }
        bool ShowAnchorPoint { get; set; }
        bool setPointerLineSymbByVal(string value, ISymbol pointerLineSymbol);
        bool getPointerLineSymbByVal(string value, ref ISymbol pointerLineSymbol);
        void getSymbolsByFeature(IFeature textFeature, ref ISymbol fixTextSymbol, ref ISymbol pointerLineSymbol);
    }
}
