namespace GEOCOM.GNSD.PlotExtension.Config
{
    using System.Xml.Serialization;

    using GEOCOM.GNSD.Common.Config;

    [XmlRoot("plotExtension")]
    public class PlotExtensionConfig : ConfigBase<PlotExtensionConfig>
    {
        [XmlElement("mxdpathinfo")]
        public MxdPathInfo MxdPath { get; set; }

        [XmlElement("dxfmxdpathinfo")]
        public MxdPathInfo DxfMxdPath { get; set; }

        [XmlElement("emptyplot")]
        public EmptyPlotInfo EmptyPlot { get; set; }

        [XmlElement("backgroundplot")]
        public EmptyPlotInfo BackgroundPlot { get; set; }

        [XmlElement("layers")]
        public LayerInfo Layers { get; set; }

        [XmlElement("export")]
        public Export Export { get; set; }
    }
}