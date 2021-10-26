using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class RequestPageConfig
    {
        /// <summary>
        /// Currently selected request page mode.
        /// </summary>
        [XmlAttribute("mode")]
        public RequestPageMode ActiveMode { get; set; }

        /// <summary>
        /// Setting relating Plot mode
        /// </summary>
        [XmlElement("plotMode")]
        public PlotMode PlotMode { get; set; }

        /// <summary>
        /// Setting relating Data mode
        /// </summary>
        [XmlElement("dataMode")]
        public DataMode DataMode { get; set; }

        [XmlElement("fields")]
        public PageFieldInfos PageFieldInfos { get; set; }
    }
}