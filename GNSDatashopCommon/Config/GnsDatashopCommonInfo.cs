using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    public enum Protocol
    {
        Http,
        Https
    }


    public class MailServerInfo
    {
        [XmlAttribute("enabled")]
        public bool IsEnabled { get; set; }
        
        [XmlAttribute("from")]
        public string From { get; set; }

        [XmlElement("server")]
        public List<MailServer> Servers { get; set; }

        [XmlIgnore]
        public Dictionary<string, MailTemplate> Mailtemplate { get; set; }

        [XmlElement(ElementName = "mailtemplate")]
        public MailTemplate[] MockupFieldForMailtemplate
        {
            get
            {
                return Mailtemplate.Values.ToArray();
            }

            set
            {
                if (Mailtemplate == null)
                    Mailtemplate = new Dictionary<string, MailTemplate>();
                else
                    Mailtemplate.Clear();

                for (int i = 0; i < value.Length; i++)
                {
                    Mailtemplate.Add(value[i].Name, value[i]);
                }
            }
        }

        public class MailServer
        {
            [XmlAttribute("order")]
            public int Order { get; set; }

            [XmlAttribute("retry")]
            public int Retry { get; set; }

            [XmlAttribute("server")]
            public string Server { get; set; }

            [XmlAttribute("port")]
            public int Port { get; set; }

            [XmlAttribute("smtpuser")]
            public string SmtpUser { get; set; }

            [XmlAttribute("smtppassword")]
            public string SmtpPassword { get; set; }

            [XmlAttribute("smtpdomain")]
            public string SmtpDomain { get; set; }

            [XmlAttribute("timeout")]
            public int Timeout { get; set; }

            [XmlAttribute("usessl")]
            public bool UseSSL { get; set; }
        }

        public class MailTemplate
        {
            [XmlAttribute("downloadurl")]
            public string DownloadUrl { get; set; }

            [XmlAttribute("adminLink")]
            public string AdminLink { get; set; }

            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("from")]
            public string From { get; set; }

            [XmlAttribute("to")]
            public string To { get; set; }

            [XmlAttribute("subject")]
            public string Subject { get; set; }

            [XmlArray("body")]
            [XmlArrayItem(ElementName = "line", Type = typeof(string))]
            public List<string> Body { get; set; }
        }
    }

    public class Directories
    {
        [XmlAttribute("exportdirectory")]
        public string ExportDirectory { get; set; }

        [XmlAttribute("profiledirectory")]
        public string ProfileDirectory { get; set; }

        [XmlAttribute("tdedirectory")]
        public string TDEDirectory { get; set; }

        [XmlAttribute("dxfdirectory")]
        public string DXFDirectory { get; set; }

        [XmlAttribute("archivedirectory")]
        public string ArchiveDirectory { get; set; }

        [XmlAttribute("jobdocumentsdirectory")]
        public string JobdocumentsDirectory { get; set; }
    }

    public class LoginAttemptLimit
    {
        [XmlAttribute("timeperiod")]
        public int TimePeriod { get; set; }

        [XmlAttribute("limit")]
        public int Limit { get; set; }
    }

    public class PasswordReset
    {
        [XmlAttribute("validityMinutes")]
        public int ValidityMinutes { get; set; }
    }
}