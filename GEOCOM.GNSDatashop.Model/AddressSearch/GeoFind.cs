using System.Collections.Generic;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class GeoFind
    {
        [XmlElement("search")]
        public List<GeoSearchDef> Searches { get; set; }
    }
}