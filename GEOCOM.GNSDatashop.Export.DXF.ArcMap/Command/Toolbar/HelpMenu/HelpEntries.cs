using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.Toolbar.HelpMenu
{
    internal class HelpEntries 
    {
        protected List<HelpEntry> _entries = new List<HelpEntry>();

        private void ReadHelpConfigurationFiles()
        {
            var moduleHelpDirectory = Assembly.GetExecutingAssembly().LoadedFromDirectory();

            ReadHelpConfigurationFilesFromDirectory(moduleHelpDirectory);
        }

        private void ReadHelpConfigurationFilesFromDirectory(string directory)
        {
            _entries.Clear();
            if (Directory.Exists(directory))
                _entries.AddRange(new HelpEntriesLoader(directory).Entries);
        }

        // only entries with online documents & entries matching the language code
        public IEnumerable<HelpEntry> Online
            => OfCurrentLanguage.Where(e => !string.IsNullOrEmpty(e.OnlineUrl)).OrderBy(e => e.Title);

        public IEnumerable<HelpEntry> OfflineHelp
            => OfCurrentLanguage.Where(e => !string.IsNullOrEmpty(e.OfflinePath)).OrderBy(e => e.Title);

        public IEnumerable<HelpEntry> OfCurrentLanguage
        {
            get
            {
                if (!_entries.Any()) // On-demand loading of help entries
                    ReadHelpConfigurationFiles();

                return _entries.Where(entry => entry.Language.Equals(StoLanguage.FindFavoredLanguageCode(), StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public int Count => OfCurrentLanguage.Count();
    }
}