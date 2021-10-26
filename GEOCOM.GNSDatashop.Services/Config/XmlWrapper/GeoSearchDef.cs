using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public class GeoSearchDef
    {
        [XmlAttribute("title")]
        public string Title { get; set; }  
       
        [XmlAttribute("languageid")]
        public int LanguageID { get; set; }  

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("control")]
        public SearchControlDef[] Controls { get; set; }  

        [XmlElement("select")]
        public Select Select { get; set; }  
        
        [XmlAttribute("dbconnection")]
        public string DbConnection { get; set; }  
    }
}