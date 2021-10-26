using System;
using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.DBStore.Config
{
    [XmlRoot("dbStore")]
    public class DbStoreConfig : ConfigBase<DbStoreConfig>
    {
        [XmlElement("dbconnection")]
        public DbConnectionInfo DbConnection { get; set; }
    }
}