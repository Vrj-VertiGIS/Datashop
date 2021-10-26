using System.Xml;
using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.AddressSearch;
using GEOCOM.GNSDatashop.Services.Config.XmlWrapper;

namespace GEOCOM.GNSDatashop.Services.Config
{
    [XmlRoot("services")]
    public class ServicesConfig : ConfigBase<ServicesConfig>
    {
        [XmlElement("scales")]
        public Scale Scale { get; set; }

        [XmlIgnore]
        public KeyValuesPair[] RequestScales
        {
            get { return Scale.RequestScales; }
            set { Scale.RequestScales = value; }
        }

        [XmlElement("searchExtension")]
        public AddressSearchConfig AddressSearch { get; set; }

		[XmlElement("restrictions")]
		public RestrictionInfo RestrictionInfo { get; set; }
    }

    public class AddressSearchConfig
    {
        [XmlElement("geofind")]
        public GeoFind GeoFind { get; set; }
    }
}