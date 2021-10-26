using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class GeoAttachemntsConfig
    {
        [XmlAttribute("showcheckbox")]
        public bool ShowCheckbox { get; set; }
    }
}