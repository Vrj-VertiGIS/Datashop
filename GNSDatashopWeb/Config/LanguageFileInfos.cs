using System.Xml.Serialization;
using GEOCOM.GNSD.Web.Core.WebControls;

namespace GEOCOM.GNSD.Web.Config
{
    public class LanguageFileInfos
    {
        [XmlElement(ElementName = "language", IsNullable = false)]
        public LanguageInfo[] Languages { get; set; }
    }
}