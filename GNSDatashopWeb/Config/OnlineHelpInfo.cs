using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class OnlineHelpInfo
    {
        [XmlElement("button")]
        public OnlineHelpButton[] Buttons { get; set; }
    }
}