using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public class Select
    {
        [XmlAttribute("scale")]
        public double Scale { get; set; }

        [XmlElement("sql")]
        public string Sql { get; set; }
    }
}
