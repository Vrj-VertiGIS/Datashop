using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    [XmlRoot("common")]
    public class GnsDatashopCommonConfig : ConfigBase<GnsDatashopCommonConfig>
    {
        [XmlElement("directories")]
        public Directories Directories { get; set; }

        [XmlElement("mail")]
        public MailServerInfo Mail { get; set; }

        [XmlElement("loginAttemptLimit")]
        public LoginAttemptLimit LoginAttemptLimit { get; set; }
        
        [XmlElement("passwordReset")]
        public PasswordReset PasswordReset { get; set; }
    }
}