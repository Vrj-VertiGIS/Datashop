using System.Collections.Generic;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config.MultipleMapServices
{
    public class MapServiceDefinition
    {
        /// <summary>
        /// Gets or sets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        [XmlAttribute("type")]
        public MapServiceType Type { get; set; }

        /// <summary>
        /// Gets or sets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        [XmlAttribute("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        [XmlAttribute("useproxy")]
        public bool UseProxy { get; set; }

        /// <summary>
        /// Gets or sets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        [XmlAttribute("token")]
        public string Token { get; set; }

        [XmlElement("")]
        [XmlArray("")]
        public List<LevelOfDetailInfo> Lods { get; set; }
    }
}