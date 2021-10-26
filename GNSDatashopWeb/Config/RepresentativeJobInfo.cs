using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class RepresentativeJobInfo
    {
        [XmlAttribute("recipient")]
        public RepresentativeJobRecipient Recipient { get; set; }
    }
}