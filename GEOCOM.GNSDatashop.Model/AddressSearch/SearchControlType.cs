using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public enum SearchControlType
    {
        [XmlEnum("combobox")]
        Combobox,
        [XmlEnum("edit")]
        Edit
    }
}