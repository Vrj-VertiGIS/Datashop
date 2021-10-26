using System;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    public class Security
    {
        [XmlElement("authentication")]
        public Authentication Authentication { get; set; }
    }

    public class Authentication
    {
        [XmlAttribute("username")]
        public String Username { get; set; }
              
        [XmlAttribute("password")]
        public String Password { get; set; }
    }
}