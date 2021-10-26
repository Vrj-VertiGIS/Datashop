using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("exportperimeter")]
    public class ExportPerimeter
    {
        [XmlAttribute("id")]
        public String Id { get; set; }

        [XmlElement("mapextent")]
        public MapExtent MapExtent { get; set; }

        [XmlElement("feature")]
        public FeatureShape FeatureShape { get; set; }

        [XmlElement("pointcollection")] 
        public CoordinatePair[] PointCollection { get; set; }

        // TODO <2010_02_22/ wif / prio3> erweitern für andere Anforderungen 
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(String.Format("Id={0}", Id));
            if (MapExtent != null) return MapExtent.ToString();
            if (FeatureShape != null) return FeatureShape.ToString();
            if (PointCollection != null)
            {
                stringBuilder.Append("Points: ");
                foreach (var pair in PointCollection)
                {
                    stringBuilder.AppendFormat("[{0},{1}] ", (int)pair.X, (int)pair.Y);
                }
            }
            else
            {
                stringBuilder.Append("empty perimeter");
            }

            return stringBuilder.ToString();
        }

        public class CoordinatePair
        {
            public CoordinatePair()
            {

            }
            // wia...
            public CoordinatePair(double x, double y)
            {
                X = x;
                Y = y;
            }

            [XmlElement("x")] 
            public double X;

            [XmlElement("y")] 
            public double Y;
        }
    }
}