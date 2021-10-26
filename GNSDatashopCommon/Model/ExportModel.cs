using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("exportmodel")]
    public class ExportModel : JobDescriptionBaseModel
    {
        [XmlElement("perimeter")]
        public ExportPerimeter[] Perimeters { get; set; }

        public override string ToString()
        {
            string toString = base.ToString() + " ";

            for (int i = 0; i < Perimeters.Length; i++)
            {
                var perimeter = Perimeters[i];
                string elemString = (perimeter != null) ? perimeter.ToString() : "Full extent or extent undefined";
                toString += string.Format("[{0}: {1}], ", i, elemString);
            }
           
            return toString;
        }
    }
}