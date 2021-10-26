using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Services.Config.XmlWrapper
{
    public enum SearchControlType
    {
        [XmlEnum("combobox")]
        Combobox,
        [XmlEnum("edit")]
        Edit
    }
}
