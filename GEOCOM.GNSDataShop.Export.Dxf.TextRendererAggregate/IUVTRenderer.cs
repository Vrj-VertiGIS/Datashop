using System.Runtime.InteropServices;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.TextRendererAggregate
{
    // note: Do not change the GUID under any circumstances!
    [Guid("AAE95FCD-F9F2-4C68-9AB2-91E70DD9BD1B")]
    [ComVisible(true)]
    public interface IUVTRenderer
    {
        ITextSymbol GetTextSymbol(string value);
        bool GetDefaultTextSymbol(ref ITextSymbol textSymbol, ref string heading);
        void SetDefaultTextSymbol(ITextSymbol textSymbol, string heading, bool hasDefaultSymbol);
        void addValue(string value, string heading, ITextSymbol textSymbol);
        string GetValue(long index);
        string GetHeading(long index);
        ITextSymbol GetTextSymbolByIndex(long index);
        string TextField { get; set; }
        string ValueField { get; set; }
        string ValueField2 { get; set; }
        string AlignField { get; set; }
        long ValueCount { get; }
        int TransparentLayerCorrection { get; set; }
        void InitTextRenderer();
    }
}
