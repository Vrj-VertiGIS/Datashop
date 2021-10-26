using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("dxfexport")]
    public class DxfExportModel : TdeExportModel
    {
        [XmlElement("dxfexportname")]
        public string DxfExportName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", base.ToString(), string.Format("  DxfExportName: {0}", this.DxfExportName));
        }
    }
}