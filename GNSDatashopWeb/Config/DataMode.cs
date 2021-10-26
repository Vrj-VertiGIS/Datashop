using System;
using System.Xml.Serialization;
using GEOCOM.GNSD.Common.Model;

namespace GEOCOM.GNSD.Web.Config
{
    public class DataMode
    {
        [XmlAttribute("showProfileSelection")]
        public bool ShowProfileSelection { get; set; }

        [XmlAttribute("defaultProfile")]
        public string DefaultProfile { get; set; }

        [XmlAttribute("defaultFormat")]
        public OutputFormat DefaultFormat { get; set; }

        [XmlAttribute("maxPolygons")]
        public int MaxPolygons { get; set; }

        [XmlAttribute("withCreateRectangle")]
        public bool WithCreateRectangle { get; set; }

        [XmlAttribute("withCreatePolygon")]
        public bool WithCreatePolygon { get; set; }

        public void CheckIt()
        {
            if (!ShowProfileSelection)
            {
                if (string.IsNullOrEmpty(DefaultProfile) || DefaultFormat == OutputFormat.None)
                    throw new Exception("Error in config file: default profile and format must be specified when selector is not visible.");
                if (!string.IsNullOrEmpty(DefaultProfile))
                {
                    try
                    {
                        // in 3.5, there is no TryParse method for GUID...
#pragma warning disable 168
                        var guid = new Guid(DefaultProfile);
#pragma warning restore 168
                    }
                    catch
                    {
                        throw new Exception("Error in config file: default profile is not a valid GUID.");
                    }
                }
            }
        }
    }
}