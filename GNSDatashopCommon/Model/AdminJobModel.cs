using System;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("adminjobmodel")]
    public class AdminJobModel : JobDescriptionBaseModel
    {
        [XmlElement("action")]
        public string Action { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" Action={0}", Action);
        }
    }
}