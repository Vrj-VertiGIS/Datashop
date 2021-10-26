using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config.MultipleMapServices
{
    public class ServerToken
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}