using System.Xml;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("layoutexport")]
    public class LayoutExportModel : ExportModel 
    {
        #region Layout
     
        [XmlAttribute("layout")]
        public string LayoutName { get; set; }

	    public new static LayoutExportModel FromXml(string xml)
	    {
		    return JobDescriptionBaseModel.FromXml(xml) as LayoutExportModel;
	    }

        #endregion
    }
}