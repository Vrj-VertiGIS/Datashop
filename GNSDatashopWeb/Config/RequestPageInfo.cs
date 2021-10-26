using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class RequestPageInfo
    {
        public RequestPageInfo()
        {
            // intentionnally left blank
        }

        public RequestPageInfo(string pageName, int textId, bool isDefaultPage)
        {
            this.PageName = pageName;
            this.IsDefautPage = isDefaultPage;
            this.TextId = textId;
        }

        [XmlAttribute("pageName")]
        public string PageName { get; set; }

        [XmlAttribute("textId")]
        public int TextId { get; set; }

        [XmlAttribute("isDefaultPage")]
        public bool IsDefautPage { get; set; }
    }
}