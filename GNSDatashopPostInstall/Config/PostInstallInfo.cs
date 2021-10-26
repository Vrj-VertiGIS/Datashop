using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.PostInstall.Config
{
 
    public class DbConnection
    {
        [XmlAttribute("oraClientHome")]
        public string OraClientHome { get; set; }
    }
}