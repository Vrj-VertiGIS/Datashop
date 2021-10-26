using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.DatashopWorkflow.Config
{
    [XmlRoot("datashopWorkflow")]
    public class DatashopWorkflowConfig : ConfigBase<DatashopWorkflowConfig>
    {
        [XmlElement("extentdatabase")]
        public ExtentDataBaseInfo ExtentDataBase { get; set; }

        [XmlElement("masking")]
        public MaskingDataBaseInfo MaskingDataBase { get; set; }

        [XmlElement("notificationdatabase")]
        public NotificationDataBaseInfo NotificationDataBase { get; set; }

        [XmlElement("extraction")]
        public ExtractionInfo Extraction { get; set; }

        [XmlElement("centerArea")]
        public CenterAreaInfo CenterArea { get; set; }

        [XmlElement("lettertemplate")]
        public LetterTemplate LetterTemplate { get; set; }

        [XmlElement("plotfilename")]
        public PlotFileName PlotFileName { get; set; }

        [XmlElement("representativejob")]
        public RepresentativeJobInfo RepresentativeJob { get; set; }

		[XmlElement("jobworkflow")]
		public WorkflowInterceptionSettings WorkflowInterceptionSettings { get; set; }

		[XmlElement("geoattachments")]
		public GeoAttachmentsConfig GeoAttachmentsConfig { get; set; }
    }
}