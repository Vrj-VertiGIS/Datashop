using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
    public class CustomSearchConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the custom Datashop map search is enabled or disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the custom Datashop map search is enabled.
        /// </value>
        [XmlAttribute("enabled")]
        public bool CustomEnabled { get; set; }

        /// <summary>
        /// Gets or sets the virtual path to a custom ASP.NET user control that will be loaded 
        /// and placed on the Request page.
        /// </summary>
        /// <value>
        /// The user control virtual path. 
        /// </value>
        /// <example>~/Controls/MyMapSearch.ascx</example>
        [XmlAttribute("virtualPath")]
        public string CustomMapSearchVirtualPath { get; set; }
    }

    /// <summary>
    /// The map search configuration on the Request page.
    /// </summary>
    public class MapSearchConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the default Datashop map search is enabled or disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the default Datashop map search is enabled.
        /// </value>
        [XmlAttribute("defaultEnabled")]
        public bool DefaultEnabled { get; set; }

        [XmlElement("customSearch")]
        public CustomSearchConfig[] CustomSearches { get; set; }


    }
}