using ESRI.ArcGIS.Display;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment
{
    public class HVAlignment
    {
        public string GEONISAlignment { get; private set; }
        public TextAlignment DxfAlignment { get; private set; }
        public esriTextHorizontalAlignment HAlignment { get; private set; }
        public esriTextVerticalAlignment VAlignment { get; private set; }
        public MTextAttachmentPoint DxfAttachementPoint { get; private set; }

        public HVAlignment(string geonisAlignment, TextAlignment dxfAlignment, MTextAttachmentPoint dxfAttachementPoint, esriTextHorizontalAlignment horizontalAlignment, esriTextVerticalAlignment verticalAlignment)
        {

            GEONISAlignment = geonisAlignment;
            DxfAlignment = dxfAlignment;
            DxfAttachementPoint = dxfAttachementPoint;
            HAlignment = horizontalAlignment;
            VAlignment = verticalAlignment;
        }
    }
}
