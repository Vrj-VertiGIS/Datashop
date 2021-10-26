using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("tdeexport")]
    public class TdeExportModel : ExportModel
    {
        public TdeExportModel()
        {
            DefaultOutputFormat = OutputFormat.pgdb;
        }

        /// <summary>
        /// If <see cref="OutputFormat"/> is null the <see cref="DefaultOutputFormat"/>will be used. Default is set to PersonalGDB.
        /// </summary>
        public OutputFormat DefaultOutputFormat { get; set; }

        [XmlElement("profileGUID")]
        public string ProfileGuid { get; set; }

        [XmlElement("outputFormat")]
        public OutputFormat OutputFormat { get; set; }        

        public override string ToString()
        {
            return base.ToString() + string.Format("  ProfileGuid: {0}", ProfileGuid);
        }
    }
}
