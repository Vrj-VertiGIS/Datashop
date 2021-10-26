using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    public class LevelsOfDetailInfo
    {
        [XmlAttribute("scaleresolutionratio")]
        public string ScaleResolutionRatio { get; set; }

        [XmlElement("lod")]
        public LevelOfDetailInfo[] Levels { get; set; }
    }
}