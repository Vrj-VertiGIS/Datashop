using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.PostInstall.Config
{

    [XmlRoot("postInstall")]
    public class PostInstallConfig : ConfigBase<PostInstallConfig>
    {
        [XmlElement("dbconnection")]
        public DbConnection DbConnection { get; set; }
    }
}
