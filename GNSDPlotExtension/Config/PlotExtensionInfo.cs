namespace GEOCOM.GNSD.PlotExtension.Config
{
	using System.Xml.Serialization;

	public class MxdPathInfo
	{
		[XmlAttribute("path")]
		public string Path { get; set; }

		[XmlElement("wmslayervalidation")]
		public WMSLayerValidation[] WmsLayerValidations { get; set; }
	}

	public class  WMSLayerValidation
	{
		[XmlAttribute("layername")]
		public string LayerName { get; set; }

		[XmlAttribute("xmin")]
		public double Xmin { get; set; }

		[XmlAttribute("ymin")]
		public double Ymin { get; set; }

		[XmlAttribute("xmax")]
		public double Xmax { get; set; }

		[XmlAttribute("ymax")]
		public double Ymax { get; set; }

		[XmlAttribute("spatialRef")]
		public string SpatialRef { get; set; }

		[XmlAttribute("url")]
		public string URL { get; set; }
	}

	public class EmptyPlotInfo
	{
		[XmlAttribute("text")]
		public string Text { get; set; }
	}

	public class LayerInfo
	{
		[XmlAttribute("background")]
		public string Background { get; set; }
	}

	public class Export
	{
		[XmlAttribute("plottemplates")]
		public string PlotTemplate { get; set; }

		[XmlElement("pdf")]
		public PdfExportInfo PdfExportInfo { get; set; }
	}

	public class PdfExportInfo
	{
		[XmlAttribute("vector")]
		public bool Vector { get; set; }
		[XmlAttribute("dpi")]
		public int Dpi { get; set; }
		[XmlAttribute("quality")]
		public int Quality { get; set; }
		[XmlAttribute("layers")]
		public bool Layers { get; set; }
	}
}