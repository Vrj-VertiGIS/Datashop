using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu
{
    public class HelpEntriesLoader
    {
        protected string _fromDirectory;

        private XmlSerializer _xmlSerializer = new XmlSerializer(typeof(HelpConfigObjects));

        public HelpEntriesLoader(string fromDirectory)
        {
            _fromDirectory = fromDirectory;
        }

        public IEnumerable<HelpEntry> Entries 
        {
            get
            {
                var xmlFiles = Directory.EnumerateFiles(_fromDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                var fileList = xmlFiles as IList<string> ?? xmlFiles.ToList(); // avoid multiple enumerations 

                foreach (var element in ReadHelpConfigurationFiles(fileList))
                    yield return element;
            }
        }

        private IEnumerable<HelpEntry> ReadHelpConfigurationFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
                foreach (var element in ReadHelpConfigurationFile(file))
                    yield return element;
        }

        private IEnumerable<HelpEntry> ReadHelpConfigurationFile(string file)
        {
            FileStream loadStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            HelpConfigObjects loadedObject = LoadXML(loadStream);
            loadStream.Close();

            // expand relative paths
            ExpandRelativePaths(loadedObject, _fromDirectory);

            // add elements
            foreach (var element in loadedObject.HelpEntries)
                yield return element;
        }

        private HelpConfigObjects LoadXML(FileStream stream)
        {
            try
            {
                return (HelpConfigObjects)_xmlSerializer.Deserialize(stream);
            }
            catch (System.InvalidOperationException)
            {
                return new HelpConfigObjects();
            }
        }

        private static void ExpandRelativePaths(HelpConfigObjects objects, string baseDirectory)
        {
            foreach (HelpEntry entry in objects.HelpEntries)
            {
                var path = entry.OfflinePath;
                if (!string.IsNullOrEmpty(path) && !Path.IsPathRooted(path))
                    entry.OfflinePath = Path.Combine(baseDirectory, path);
            }
        }
    }
}
