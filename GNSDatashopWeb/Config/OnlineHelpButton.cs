using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class OnlineHelpButton
    {
     
        [XmlAttribute("page")]
        public string PageId { get; set; }

        [XmlAttribute("buttonId")]
        public string PlaceholderId { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        /// <summary>
        /// To get a modal dialog, set this one to true
        /// </summary>
        [XmlAttribute("sameWindow")]
        public bool SameWindow { get; set; }
    }
}