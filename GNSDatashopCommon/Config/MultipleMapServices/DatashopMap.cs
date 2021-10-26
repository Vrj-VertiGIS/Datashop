using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config.MultipleMapServices
{
    [Serializable]
    [XmlRoot("datashopmap")]
    public class DatashopMap
    {
        /// <summary>
        /// Gets or sets the geometry service URL.
        /// </summary>
        /// <value>
        /// The geometry service URL.
        /// </value>
        [XmlAttribute("geometryserviceurl")]
        public string GeometryServiceUrl { get; set; }

        /// <summary>
        /// The scale levels
        /// </summary>
        /// <value>
        /// The levels.
        /// </value>
        [XmlAttribute("levels")]
        public MapServiceLevel Levels { get; set; }

        /// <summary>
        /// Gets or sets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        [XmlAttribute("slider")]
        public bool Slider { get; set; }

        /// <summary>
        /// Gets or sets the max selectable area.
        /// </summary>
        /// <value>
        /// The max selectable area.
        /// </value>
        [XmlAttribute("maxselectablearea")]
        public float MaxSelectableArea { get; set; }

        /// <summary>
        /// Gets or sets the map services.
        /// </summary>
        /// <value>
        /// The map services.
        /// </value>
        [XmlArray("mapservices")]
        [XmlArrayItem("mapservice")]
        public List<MapServiceDefinition> MapServices { get; set; }

        /// <summary>
        /// Gets or sets the server tokens.
        /// </summary>
        /// <value>
        /// The server tokens.
        /// </value>
        [XmlArray("mapservices")]
        [XmlArrayItem("mapservice")]
        public List<ServerToken> ServerTokens { get; set; }
    }
}