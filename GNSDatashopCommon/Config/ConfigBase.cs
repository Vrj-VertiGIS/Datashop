using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GEOCOM.Common.Logging;

namespace GEOCOM.GNSD.Common.Config
{
    [XmlRoot("config")]
    public abstract class ConfigBase<T> where T : ConfigBase<T>, new()
    {
        private static IMsg log = new Msg(typeof(T));

        private static object lockObject = new object();
        private static T instance;

        public static T Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        var configFactory = new ConfigFactory();
                        instance = configFactory.DeserializeConfigFromXml();
                        instance.AddFileWatcher();
                        instance.IsInitialized = true;
                        instance.OnInit();
                    }
                    return instance;
                }
            }
        }

        public static string LogDirectoryPath
        {
            get
            {
                string logDirectory = ConfigFactory.GetLogDirectory();
                return logDirectory;
            }
        }

        [XmlIgnore]
        public bool IsInitialized { get; set; }

        [XmlIgnore]
        public string ConfigFilePath { get; set; }

        [XmlIgnore]
        public string ConfigDirectoryPath { get; set; }

        protected virtual void OnInit()
        {
        }

        private void AddFileWatcher()
        {
            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = Path.GetDirectoryName(ConfigFilePath);
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = Path.GetFileName(ConfigFilePath);
                watcher.Changed += ConfigFileChanged;
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception exp)
            {
                log.Error(exp.Message, exp);
            }
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            var configFactory = new ConfigFactory();
            instance = configFactory.DeserializeConfigFromXml();
            log.Info("Config file changed and has been reloaded.");
            IsInitialized = true;
        }

        public class ConfigFactory
        {
            #region Constants

            private const string ReadingConfigExceptionMessage = "A problem occurred while attempting to read the configuration file in {0}.";
            private const string ApplicationRootFolderSearchFailedExceptionMessage = "Application root folder not found in the configuration.";
            private const string RootAttributeIsMissing = "The XMLRoot attribute must be defined on the configuration class {0} and its value must be set.";
            private const string ConfigNameNotFoundExceptionMessage = "The configuration file name was within the application configuration file. Key 'ConfigFileName' in <appsettings> tag not found. ";
            #endregion

            public static string GetLogDirectory()
            {
                string workingFolder = GetApplicationRootFolder() ?? string.Empty;
                string configDirectoryPath = Path.Combine(workingFolder, "log");

                return configDirectoryPath;
            }

            public T DeserializeConfigFromXml()
            {
                string configFilePath = GetConfigFilePath();

                return this.DeserializeConfigFromXml(configFilePath);
            }

            public T DeserializeConfigFromXml(string configFilePath)
            {

                try
                {
                    string configurationXml = ReadTypesConfigurationXml(configFilePath, typeof(T));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        var streamWriter = new StreamWriter(memoryStream);
                        streamWriter.Write(configurationXml);
                        streamWriter.Flush();
                        memoryStream.Position = 0;

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                        T configFromXml = (T)xmlSerializer.Deserialize(memoryStream);
                        configFromXml.ConfigFilePath = configFilePath;
                        configFromXml.ConfigDirectoryPath = GetConfigDirectory();

                        return configFromXml;
                    }
                }
                catch (Exception e)
                {
                    string msg = string.Format(ReadingConfigExceptionMessage, configFilePath);
                    log.Fatal(msg);
                    throw new ConfigException(msg, e);
                }
            }

            private static string ReadTypesConfigurationXml(string configFilePath, Type type)
            {
                // File might be hold by other program that has just edit the file.
                const int triesCount = 5;
                Exception exception = new ConfigException("Error occured while reading the config file in " + configFilePath);
                for (int i = 0; i < triesCount; i++)
                {
                    try
                    {
                        using (StreamReader streamReader = new StreamReader(File.OpenRead(configFilePath)))
                        {
                            XmlDocument document = new XmlDocument();
                            document.Load(streamReader);

                            XmlRootAttribute rootAttribute = GetXMLRootAttributeFromType(type);
                            XmlNode rootElement = document.GetElementsByTagName(rootAttribute.ElementName)[0];

                            string configXml = rootElement.OuterXml;
                            return configXml;
                        }
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        const int waitingForFileToBeFreeMilisec = 200;
                        Thread.Sleep(waitingForFileToBeFreeMilisec);
                    }
                }
                throw exception;
             }

            private static string GetApplicationRootFolder()
            {
                const string ApplicationRootPathKey = "ApplicationRootPath";

                string applicationFolder = ConfigurationManager.AppSettings[ApplicationRootPathKey];
                if (string.IsNullOrEmpty(applicationFolder))
                    applicationFolder = GetAppSettingsValueByKey(ApplicationRootPathKey);

                if (string.IsNullOrEmpty(applicationFolder))
                    throw new ConfigurationErrorsException(ApplicationRootFolderSearchFailedExceptionMessage);

                return applicationFolder;
            }

            private static string GetAppSettingsValueByKey(string key)
            {
                string typesAppConfigPath = GetTypesAppConfigPath();
                XElement rootConfig = XElement.Load(typesAppConfigPath);
                var values =
                  from el in rootConfig.Element("appSettings").Elements("add")
                  where (string)el.Attribute("key") == key
                  select el.Attribute("value").Value;
                string value = values.SingleOrDefault();

                return value;
            }

            private static string GetTypesAppConfigPath()
            {
                string typesAppConfigPath = typeof(T).Assembly.CodeBase + ".config";
                return typesAppConfigPath;
            }

            private static XmlRootAttribute GetXMLRootAttributeFromType(Type type)
            {
                XmlRootAttribute rootAttribute = type.GetCustomAttributes(typeof(XmlRootAttribute), false).SingleOrDefault() as XmlRootAttribute;
                if (rootAttribute == null || string.IsNullOrEmpty(rootAttribute.ElementName))
                {
                    string msg = string.Format(RootAttributeIsMissing, type.FullName);
                    throw new ConfigException(msg);
                }
                return rootAttribute;
            }
            
            private string GetConfigFilePath()
            {
                string dir = GetConfigDirectory();
                string fileName = GetConfigFileName();
                var path = Path.Combine(dir, fileName);

                return path;
            }

            private string GetConfigDirectory()
            {
                string workingFolder = GetApplicationRootFolder();
                string configDirectoryPath = Path.Combine(workingFolder, "config");
                return configDirectoryPath;
            }

            private string GetConfigFileName()
            {
                const string configFileNameKey = "ConfigFileName";
                string configFileName = ConfigurationManager.AppSettings[configFileNameKey];
                if (string.IsNullOrEmpty(configFileName))
                    configFileName = GetAppSettingsValueByKey(configFileNameKey);

                if (string.IsNullOrEmpty(configFileName))
                    throw new ConfigException(ConfigNameNotFoundExceptionMessage);

                return configFileName;
            }
        }
    }

    internal class ConfigException : Exception
    {
        public ConfigException(string msg):base(msg)
        {}

        public ConfigException(string msg, Exception e):base(msg, e)
        {
            
        }
    }
}