using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class DocumentElementInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("relativeurl")]
        public string RelativeUrl { get; set; }
    }
}