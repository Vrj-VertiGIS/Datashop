using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class PlotMode
    {
        [XmlAttribute("defaultScale")]
        public int DefaultScale { get; set; }
        [XmlAttribute("maxPolygons")]
        public int MaxPolygons { get; set; }

        [XmlAttribute("dxfExport")]
        public bool DxfExport { get; set; }

        [XmlAttribute("limitFormat")]
        public string LimitFormat { get; set; }

    }
}