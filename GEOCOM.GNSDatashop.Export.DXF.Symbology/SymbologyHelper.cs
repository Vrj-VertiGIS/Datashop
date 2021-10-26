using netDxf;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class SymbologyHelper
    {
        public static AciColor ConvertEsriColorToAciColor(int esriRgb)
        {
            int red = (esriRgb & 0x00FF0000) >> 16;
            int green = (esriRgb & 0x0000FF00) >> 8;
            int blue = esriRgb & 0x000000FF;

            return new AciColor((byte)blue, (byte)green, (byte)red);
        }

        public static double ConvertEsriPointsToCentiMM(double esriPoints) => (esriPoints / 72) * 25.4 * 100;

        public static int ConvertEsriToDxfTransparency(byte esriTransparency) => Math.Abs(esriTransparency - 255) * (100 / 256);
    }

}
