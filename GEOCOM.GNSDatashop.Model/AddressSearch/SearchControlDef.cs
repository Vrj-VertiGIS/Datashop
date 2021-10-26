using System.ComponentModel;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class SearchControlDef
    {
        [XmlAttribute("type")]
        public SearchControlType ControlType;

        [XmlAttribute("name"), DefaultValue("")]
        public string Name;

        [XmlElement("select")]
        public Select SqlQuery;

        [XmlAttribute("title"), DefaultValue("")]
        public string Title;

        [XmlAttribute("languageid")]
        public int LanguageID;
    }
}