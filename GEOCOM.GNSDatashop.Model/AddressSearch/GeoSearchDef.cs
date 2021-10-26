using System.ComponentModel;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class GeoSearchDef
    {
        [XmlAttribute("title"), DefaultValue("")]
        public string Title { get; set; }

        [XmlAttribute("languageid"), DefaultValue(0)]
        public int LanguageID { get; set; }

        [XmlAttribute("name"), DefaultValue("")]
        public string Name { get; set; }

        [XmlElement("control")]
        public SearchControlDef[] Controls { get; set; }

        [XmlElement("select")]
        public Select Select { get; set; }

        [XmlAttribute("dbconnection"), DefaultValue("")]
        public string DbConnection { get; set; }

        [XmlAttribute("showExpanded"), DefaultValue(false)]
        public bool ShowExpanded { get; set; }  
    }
}