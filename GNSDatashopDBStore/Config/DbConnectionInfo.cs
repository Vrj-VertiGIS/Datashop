using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.DBStore.Config
{
	public class DbConnectionInfo
	{
	    [XmlElement("authentification")]
	    public AuthenticationInfo Authentification { get; set; }
		
        [XmlAttribute("provider")]
        public string Provider { get; set; }

		[XmlAttribute("connectionstring")]
        public string Connectionstring { get; set; }

        [XmlAttribute("timeoutsec")]
        public string ConnectionTimeoutSec { get; set; }
	}
}
