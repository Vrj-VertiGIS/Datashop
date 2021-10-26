using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("featureshape")]
    public class FeatureShape
    {
        [XmlElement("datasource")]
        public string Datasource { get; set; }
        [XmlElement("featureclass")]
        public string FeatureClass { get; set; }
        [XmlElement("objectid")]
        public long ObjectId { get; set; }

        public override string ToString()
        {
            return string.Format("datasource={0}, featureclass={1}, objectid={2}", Datasource, FeatureClass, ObjectId);
        }
    }
}