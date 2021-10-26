using System;
using System.IO;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Common.Config
{
    /// <summary>
    /// Utility class for derserializing arbitrary Xml
    /// </summary>
    public class ConfigReader
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static T GetConfiguration<T>(string fileName)
        {
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof (T));

                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error getting configuration object from filename: {0} using type: {1}",
                    fileName, typeof(T)), ex);
            }
        }
    }
}