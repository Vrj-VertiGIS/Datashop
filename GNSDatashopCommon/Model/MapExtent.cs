using System;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
    [XmlRoot("mapextent")]
    public class MapExtent
    {
        [XmlAttribute("id")]
        public String Id { get; set; }

        [XmlElement("centerx")]
        public double CenterX { get; set; }

        [XmlElement("centery")]
        public double CenterY { get; set; }

        [XmlElement("scale")]
        public double Scale { get; set; }

        [XmlElement("rotation")]
        public double Rotation { get; set; }

        [XmlElement("plottemplate")]
        public string PlotTemplate { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0} Position: {1:F0}//{2:F0}  Rotation: {3}°\nScale 1:{4}\nPlotTemplate: {5}", Id, CenterX, CenterY, Rotation, Scale, PlotTemplate);
        }
    }
}