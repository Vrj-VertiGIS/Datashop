using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.DatashopWorkflow.Config
{
    public enum RepresentativeJobRecipient
    {
        Selected,
        Representative,
        Both
    }

    public enum CenterAreaType
    {
        CenterPoint = 1,
        LabelPoint = 2
    }


    /// <summary>
    /// Describes types of the extraction performed on intersecting objects.
    /// </summary>
    public enum IntersectionType
    {
        /// <summary>
        /// Extracts only data from an object with the biggest intersection area. Applicable only to polygons.
        /// </summary>
        MaxIntersectionArea,

        /// <summary>
        /// Extracts only data from an object with the longest intersection length. Applicable only to lines.
        /// </summary>
        MaxIntersectionLength,

        /// <summary>
        /// Extracts data from all intersected objects.
        /// </summary>
        AllIntersectingObjects,
    }

    /// <summary>
    /// Determines the way geo-attachemnts are added.
    /// </summary>
    public enum GeoAttachmentMode
    {
        /// <summary>
        /// Never add geo-attachments.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Add geo-attachments base on user input.
        /// </summary>
        User = 1,

        /// <summary>
        /// Always add geo-attachments. 
        /// </summary>
        Always = 2
    }


    public class RepresentativeJobInfo
    {
        [XmlAttribute("recipient")]
        public RepresentativeJobRecipient Recipient { get; set; }
    }

    public class MaskingDataBaseInfo
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlAttribute("featureclass")]
        public string FeatureClass { get; set; }
    }

    public class ExtentDataBaseInfo
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlElement("jobextent")]
        public JobExtentInfo JobExtentInfo { get; set; }
    }

    public class NotificationDataBaseInfo
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlElement("dataownerextent")]
        public DataOwnerExtentInfo DataOwnerExtentInfo { get; set; }

        [XmlElement("dataowner")]
        public DataOwnerInfo DataOwnerInfo { get; set; }
    }

    public class ExtractionInfo
    {
        [XmlAttribute("workspaceConnection")]
        public string WorkspaceConnection { get; set; }

        [XmlElement("extractionItem")]
        public ExtractionItemElementInfo[] ExtractionItems { get; set; }
    }

    public class JobExtentInfo
    {
        [XmlAttribute("featureclass")]
        public string FeatureClass { get; set; }

        [XmlAttribute("col_jobid")]
        public string ColJobId { get; set; }
    }

    public class ExtractionItemElementInfo
    {
        [XmlAttribute("featureClass")]
        public string FeatureClass { get; set; }

        [XmlAttribute("sourceColumn")]
        public string SourceColumn { get; set; }

        [XmlAttribute("destinationColumn")]
        public string DestinationColumn { get; set; }

        [XmlAttribute("intersectionType")]
        public IntersectionType IntersectionType { get; set; }

        [XmlAttribute("separator")]
        public string Separator { get; set; }
    }

    public class DataOwnerExtentInfo
    {
        [XmlAttribute("featureclass")]
        public string FeatureClass { get; set; }

        [XmlAttribute("col_ownerid")]
        public string ColOwnerId { get; set; }

        [XmlAttribute("col_extentdescription")]
        public string ColExtentDescription { get; set; }
    }

    public class DataOwnerInfo
    {
        [XmlAttribute("table")]
        public string Table { get; set; }

        [XmlAttribute("col_email")]
        public string ColEmail { get; set; }

        [XmlAttribute("col_description")]
        public string ColDescription { get; set; }
    }

    public class CenterAreaInfo
    {
        [XmlAttribute("type")]
        public CenterAreaType CenterAreaType { get; set; }

        [XmlAttribute("displayFormat")]
        public string DisplayFormat { get; set; }
    }

    public class LetterTemplate
    {
        [XmlAttribute("file")]
        public string File { get; set; }

        [XmlAttribute("targetfile")]
        public string TargetFile { get; set; }
    }

    public class PlotsOverview
    {
        [XmlAttribute("targetfile")]
        public string TargetFile { get; set; }

        [XmlAttribute("template")]
        public string Template { get; set; }

        [XmlAttribute("extentslayer")]
        public string ExtentsLayer { get; set; }

		  [XmlAttribute("maxscale")]
		  public string MaxScaleInternal { get; set; }

	    public int? MaxScale
	    {
		    get
		    {
			    int maxScale;
			    if (int.TryParse(MaxScaleInternal, out maxScale))
				    return maxScale;
			    else
				    return null;
		    }
	    }

	    [XmlAttribute("col_jobid")]
        public string JobIdColumn { get; set; }


        [XmlIgnore]
        public bool SelectPlotFrames
        {
            get { return !string.IsNullOrWhiteSpace(ExtentsLayer) && !string.IsNullOrWhiteSpace(JobIdColumn); }
        }
    }

    public class PlotFileName
    {
        [XmlAttribute("targetfile")]
        public string TargetFile { get; set; }

        [XmlElement("overview")]
        public PlotsOverview PlotsOverview { get; set; }

        [XmlIgnore]
        public bool Overview
        {
            get
            {
                return PlotsOverview != null && !string.IsNullOrWhiteSpace(PlotsOverview.TargetFile);
            }
        }
    }

    public class WorkflowInterceptionSettings
    {
        [XmlElement("stopcriterion")]
        public StopCriterion[] StopCriteria { get; set; }
    }

    public class StopCriterion
    {
        [XmlAttribute("reason")]
        public string Reason { get; set; }

        [XmlAttribute("userrole")]
        public string UserRole { get; set; }

        [XmlAttribute("stopafter")]
        public string StopAfterStepName { get; set; }

        [XmlAttribute("mailrecipients")]
        public string MailRecipients { get; set; }
    }

    /// <summary>
    /// The geographic-base attachments setting.
    /// </summary>
    public class GeoAttachmentsConfig
    {
        /// <summary>
        /// Gets or sets path to the sde/mxd file.
        /// </summary>
        /// <value>
        /// The sde/mxd file path.
        /// </value>
        [XmlAttribute("path")]
        public string Path { get; set; }


        /// <summary>
        /// Gets or sets the name of the feature class where attachment features and file information are stored.
        /// </summary>
        /// <value>
        /// The feature class name.
        /// </value>
        [XmlAttribute("featureclass")]
        public string FeatureClass { get; set; }

        /// <summary>
        /// Gets or sets the column name where the file path (UNC or absolute) to the actual attachment is stored. 
        /// </summary>
        /// <value>
        /// The column name.
        /// </value>
        [XmlAttribute("col_filepath")]
        public string FilePathColumn { get; set; }

        /// <summary>
        /// Determines the way geo-attachemnts are added.
        /// </summary>
        /// <value>
        /// The geo-attachment mode.
        /// </value>
        [XmlAttribute("mode")]
        public GeoAttachmentMode Mode { get; set; }


        /// <summary>
        /// Gets or sets the directory name (not path) where the geo-attachments will be stored after zipping.
        /// </summary>
        /// <value>
        /// The directory name (not path).
        /// </value>
        [XmlAttribute("directoryname")]
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed total size of all geo-attachments in megabytes (MB).
        /// </summary>
        /// <value>
        /// The max size in mega bytes.
        /// </value>
        [XmlAttribute("maxsize")]
        public string MaxSize { get; set; }

        [XmlElement("urlauthentication")]
        public UrlAuth UrlAuthentication { get; set; }

        /// <summary>
        /// URL authentication configuration 
        /// </summary>
        public class UrlAuth
        {
            /// <summary>
            /// Gets or sets the user name of an user that can access URLs file attachments
            /// </summary>
            /// <value>
            /// The user name
            /// </value>
            [XmlAttribute("username")]

            public string UserName { get; set; }

            /// <summary>
            /// Gets or sets the password of an user that can access URLs file attachments
            /// </summary>
            /// <value>
            /// The password.
            /// </value>
            [XmlAttribute("password")]
            public string Password { get; set; }

            /// <summary>
            /// Gets or sets the authentication type, e.g. BASIC, NTML.
            /// </summary>
            /// <value>
            /// The authentication type.
            /// </value>
            [XmlAttribute("type")]
            public string Type { get; set; }
        }
    }
}