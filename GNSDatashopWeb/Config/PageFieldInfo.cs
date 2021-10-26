using System.Xml.Serialization;
using GEOCOM.GNSD.Web.Core.ServerControls;

namespace GEOCOM.GNSD.Web.Config
{
    public class PageFieldInfo : ValidatedControlDataItem
    {
        [XmlAttribute("serverId")]
        public string ServerId { get; set; }

        [XmlAttribute("required")]
        public bool IsRequired { get; set; }

        [XmlAttribute("visible")]
        public string IsVisibleString { get; set; }

        public bool IsVisible
        {
            get
            {
                return string.IsNullOrEmpty(IsVisibleString) || bool.Parse(IsVisibleString);
            }

            set
            {
                IsVisibleString = value.ToString();
            }
        }

        public override bool Required
        {
            get
            {
                return IsRequired;
            }

            set
            {
                IsRequired = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return IsVisible;
            }

            set
            {
                IsVisible = value;
            }
        }

        [XmlAttribute("regex")]
        public string RegEx { get; set; }
    }
}