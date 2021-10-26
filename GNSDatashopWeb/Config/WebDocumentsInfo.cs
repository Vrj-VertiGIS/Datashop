using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class WebDocumentsInfo
    {
        [XmlElement("document")]
        public DocumentElementInfo[] Documents { get; set; }
    }
}