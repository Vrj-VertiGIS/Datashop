using System.Xml.Serialization;

namespace GNSDatashopAdmin.Config
{
    public class JobListItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("visible")]
        public bool Visible { get; set; }
    }
}