using System.Linq;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Config
{
	public enum DisplayMode
	{
		DedicatedPage = 0,
		WelcomePage = 1
	}

	public class PageFieldInfos
	{
		[XmlElement("field")]
		public PageFieldInfo[] Fields { get; set; }

		[XmlAttribute("disableCaptcha")]
		public bool DisableCaptcha { get; set; }

		[XmlAttribute("displayMode")]
		public DisplayMode DisplayMode { get; set; }

		public PageFieldInfo GetById(string id)
		{
			if (Fields == null)
				return null;

			PageFieldInfo fieldInfo = Fields.Where(fi => fi.ServerId == id).SingleOrDefault();
			return fieldInfo;
		}
	}
}