using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
	public class ProxyConfig
	{
		public class ProxyServiceConfig
		{
			[XmlAttribute("proxyname")]
			public string ProxyName { get; set; }

			[XmlAttribute("serverurl")]
			public string ServerUrl { get; set; }
		}

		[XmlElement("service")]
		public ProxyServiceConfig[] ProxyServices { get; set; }

		[XmlAttribute("enabled")]
		public bool UseProxy { get; set; }


	}

	public class MapServiceInfo
	{
		/// <summary>
		/// The url of the ArcGIS Rest map service, either direct or through the proxy
		/// </summary>
		[XmlAttribute("serviceurl")]
		public string ServiceUrl { get; set; }

		[XmlAttribute("useproxy")]
		public string UseProxy { get; set; }

		[XmlAttribute("webmapConfigurationPath")]
		public string WebMapConfigurationPath { get; set; }

		// 15.07.14
		[XmlAttribute("adminwebmapConfigurationPath")]
		public string AdminWebMapConfigurationPath { get; set; }

		[XmlAttribute("geometryserviceurl")]
		public string GeometryServiceUrl { get; set; }

		[XmlAttribute("maxselectablearea")]
		public float MaxSelectableArea { get; set; }

		/// <summary>
		/// Gets or sets the slider.
		/// </summary>
		/// <value>
		/// The slider.
		/// </value>
		[XmlAttribute("slider")]
		public string Slider { get; set; }

		/// <summary>
		/// The scale levels
		/// </summary>
		[XmlAttribute("levels")]
		public MapServiceLevel Levels { get; set; }


		/// <summary>
		/// Coordinates of the initial view of the map
		/// </summary>
		[XmlElement("initialExtent")]
		public ExtentInfo InitialExtent { get; set; }

		[XmlElement("maximumExtent")]
		public ExtentInfo MaximumExtent { get; set; }

		[XmlElement("minimumExtent")]
		public ExtentInfo MinimumExtent { get; set; }

		[XmlElement("lods")]
		public LevelsOfDetailInfo LevelsOfDetail { get; set; }
	}
}