using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public class SearchControlDef
    {
        [XmlAttribute("type")]
        public SearchControlType ControlType { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("select")]
        public Select SqlQuery { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("languageid")]
        public int LanguageId { get; set; }
    }
}
