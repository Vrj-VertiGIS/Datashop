using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class IColorExtensions
    {
        public static AciColor AciColor(this ESRI.ArcGIS.Display.IColor esriColor)
        {
            return ColorExtensions.AciColor(esriColor.RGB);
        }

        public static bool IsTransparent(this ESRI.ArcGIS.Display.IColor esriColor)
        {
            return (0 >= esriColor.Transparency);
        }
    }
}
