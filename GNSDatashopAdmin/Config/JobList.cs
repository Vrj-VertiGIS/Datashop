using System.Xml.Serialization;

namespace GNSDatashopAdmin.Config
{
    public class JobList
    {
        [XmlElement("field")]
        public JobListItem[] Fields { get; set; }       
    }
}