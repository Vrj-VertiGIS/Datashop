using System.Xml.Serialization;
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public class Role
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("add")]
        public KeyTextPair[] KeyTextPairs { get; set; }
    }
}
