using System.ComponentModel;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class Select
    {
        [XmlAttribute("scale")]
        public double Scale { get; set; }

        [XmlElement("sql"), DefaultValue("")]
        public string Sql { get; set; }   
    }
}