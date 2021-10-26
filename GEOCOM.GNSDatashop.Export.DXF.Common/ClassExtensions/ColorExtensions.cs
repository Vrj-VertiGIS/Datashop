using netDxf;
using ESRI.ArcGIS.Display;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ColorExtensions
    {
        public static AciColor AciColor(this System.Drawing.Color value)
        {
            return AciColor(value.ToArgb());
        }

        public static AciColor AciColor(int rgbValue)
        {
            int red = (rgbValue & 0x00FF0000) >> 16;
            int green = (rgbValue & 0x0000FF00) >> 8;
            int blue = rgbValue & 0x000000FF;

            return new AciColor((byte)blue, (byte)green, (byte)red);
        }
    }
}
