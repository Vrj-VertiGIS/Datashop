using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    public class LevelOfDetailInfo
    {
        [XmlAttribute("scale")]
        public int Scale { get; set;}
    }
}