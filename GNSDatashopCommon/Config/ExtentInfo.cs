using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    /// <summary>
    /// Serializable class that represents the extents JSON object from map events, replaces the previously used InitialExtentInfo class
    /// </summary>
    public class ExtentInfo
    {
        /// <summary>
        /// Gets or sets the X min.
        /// </summary>
        /// <value>
        /// The X min.
        /// </value>
        [XmlAttribute("xmin")]
        public double XMin { get; set; }

        /// <summary>
        /// Gets or sets the X max.
        /// </summary>
        /// <value>
        /// The X max.
        /// </value>
        [XmlAttribute("xmax")]
        public double XMax { get; set; }

        /// <summary>
        /// Gets or sets the Y min.
        /// </summary>
        /// <value>
        /// The Y min.
        /// </value>
        [XmlAttribute("ymin")]
        public double YMin { get; set; }

        /// <summary>
        /// Gets or sets the Y max.
        /// </summary>
        /// <value>
        /// The Y max.
        /// </value>
        [XmlAttribute("ymax")]
        public double YMax { get; set; }

        /// <summary>
        /// Gets or sets the spatial reference.
        /// </summary>
        /// <value>
        /// The spatial reference.
        /// </value>
        [XmlElement("spatialReference")]
        public SpatialReference SpatialReference { get; set; }

        /// <summary>
        /// Converts the values of this instance to an anonymous CLR type that is compatible with JSON Serialization 
        /// for the ESRI ArcGIS Javascript framework
        /// </summary>
        /// <returns></returns>
        public object ToAnonymousType()
        {
            return new
            {
                xmin = (long)this.XMin,
                ymin = (long)this.YMin,
                xmax = (long)this.XMax,
                ymax = (long)this.YMax,
                spatialReference = new { wkid = (long)this.SpatialReference.WKID }
            };
        }
    }

    /// <summary>
    /// Nested class that holds the Well Known ID
    /// </summary>
    public class SpatialReference
    {
        /// <summary>
        /// Gets or sets the WKID.
        /// </summary>
        /// <value>
        /// The WKID.
        /// </value>
        [XmlAttribute("wkid")]
        public double WKID { get; set; }
    }
}