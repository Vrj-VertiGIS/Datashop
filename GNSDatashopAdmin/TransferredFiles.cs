namespace GNSDatashopAdmin
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    [XmlRoot("transferredfiles")]
    public class TransferredFiles 
    {
        [XmlIgnore]
        public const string Progid = "GeonisServer.FileUpload";

        [XmlElement("transferid")]
        public Guid TransferId { get; set; }

        [XmlArray("Files"), XmlArrayItem("File")]
        public string[] Files { get; set; }

        public static TransferredFiles GetFromXmlString(string stringAsXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransferredFiles));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(stringAsXml));
            TransferredFiles result = xmlSerializer.Deserialize(ms) as TransferredFiles;
            return result;
        }

        public override string ToString()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransferredFiles));
            StringBuilder sb = new StringBuilder();
            xmlSerializer.Serialize(new StringWriter(sb), this);
            return sb.ToString();
        }
    }
}