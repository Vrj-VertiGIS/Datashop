using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Model
{
	[XmlRoot("jobdescriptionbasemodel")]
	[XmlInclude(typeof(ExportModel))]
	[XmlInclude(typeof(LayoutExportModel))]
	[XmlInclude(typeof(TdeExportModel))]
	[XmlInclude(typeof(DxfExportModel))]
	[XmlInclude(typeof(AdminJobModel))]
	public class JobDescriptionBaseModel
	{
		public static JobDescriptionBaseModel Deserialize(string xmlString)
		{
			if (string.IsNullOrEmpty(xmlString))
				return null;
			JobDescriptionBaseModel result = null;
			XmlSerializer s = new XmlSerializer(typeof(JobDescriptionBaseModel));
			var r = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
			result = (JobDescriptionBaseModel)s.Deserialize(r);
			r.Close();
			return result;
		}

		/// <summary>
		/// Returns this Object as XML-String
		/// </summary>
		/// <returns>The object as XML-String</returns>
		public string ToXml()
		{
			XmlSerializer s = new XmlSerializer(typeof(JobDescriptionBaseModel));
			Stream stream = new MemoryStream();
			s.Serialize(stream, this);
			stream.Position = 0;
			StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		/// <summary>
		/// Returns this Object as XML-String
		/// </summary>
		/// <returns>The object as XML-String</returns>
		public static JobDescriptionBaseModel FromXml(string xml)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(JobDescriptionBaseModel));

			using (var stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream))
				{
					streamWriter.Write(xml);
					streamWriter.Flush();
					stream.Position = 0;

					var o = xmlSerializer.Deserialize(stream) as JobDescriptionBaseModel;
					return o;
				}
			}

		}

		public override string ToString()
		{
			return string.Empty;
		}
	}
}