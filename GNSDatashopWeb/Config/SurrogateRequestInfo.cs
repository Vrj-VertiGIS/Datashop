using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class SurrogateRequestInfo
    {
        [XmlAttribute("placement")]
        public SurrogateRequestPlacement Placement { get; set; }
    }
}