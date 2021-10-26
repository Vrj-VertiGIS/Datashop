using System;
using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;
using GNSDatashopAdmin.Controls;

namespace GNSDatashopAdmin.Config
{
    /// <summary>
    /// Config class for the Datashop Admin web
    /// </summary>
    [XmlRoot("datashopWebAdmin")]
	public class DatashopWebAdminConfig : ConfigBase<DatashopWebAdminConfig>
	{

        /// <summary>
        /// URL and other information to the map service used for displaying job definitions
        /// </summary>
		[XmlElement("mapservice")]
		public MapServiceInfo MapService;


		/// <summary>
		/// Gets or sets the proxy.
		/// </summary>
		/// <value>
		/// The proxy.
		/// </value>
		[XmlElement("proxy")]
		public ProxyConfig Proxy { get; set; }

        [XmlElement("security")]
        public Security Security { get; set; }

        /* too dangerous - keep it in separate files
        [XmlElement("jobdetaillayout")]
        public DsDynamicReportConfig JobDetailLayout { get; set; }         
        */

        [XmlElement("joblistMain")]
        public JobList JobListMain { get; set; }

        [XmlElement("joblistUser")]
        public JobList JobListUser { get; set; }

        [XmlElement("jobdetaillayoutconfigfile")]
        public string JobDetailLayoutConfigFile { get; set; }
    }
}