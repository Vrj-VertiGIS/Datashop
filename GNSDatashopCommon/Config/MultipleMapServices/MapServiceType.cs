using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config.MultipleMapServices
{
    public enum MapServiceType
    {
        [XmlEnum("")]
        ArcGIS,
        [XmlEnum("")]
        OpenStreetMap,
        [XmlEnum("")]
        WebMap
    }
}