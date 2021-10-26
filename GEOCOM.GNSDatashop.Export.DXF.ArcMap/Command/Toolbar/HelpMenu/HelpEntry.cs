using System.Collections.Generic;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu
{
    [XmlRoot(ElementName = "help_entry")]
    public class HelpEntry
    {
        [XmlElement(ElementName = "online_url")]
        public string OnlineUrl { get; set; }

        [XmlElement(ElementName = "offline_path")]
        public string OfflinePath { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
    }

    [XmlRoot(ElementName = "help_config")]
    public class HelpConfigObjects
    {
        [XmlElement(ElementName = "help_entry")]
        public List<HelpEntry> HelpEntries { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        public HelpConfigObjects()  // Will use this in case of deserialization errors.
        {   
            HelpEntries = new List<HelpEntry>();
            Version = string.Empty;
        }
    }
}