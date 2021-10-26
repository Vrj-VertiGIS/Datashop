using System;
using System.IO;
using System.Reflection;
using GEOCOM.Common.Logging;
using Ionic.Zip;
using System.Linq;

namespace GEOCOM.GNSD.DatashopWorkflow
{
    public class DocumentZipper
    {
        private readonly string _jobdocumentsDirectory;
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        public DocumentZipper(string jobdocumentsDirectory)
        {
            _jobdocumentsDirectory = jobdocumentsDirectory;
        }

        internal string[] ZipGeneratedDocuments(string jobOutputFileName)
        {
            try
            {
                // zip the folder. Delete old existing Zip File
                if (File.Exists(jobOutputFileName))
                {
                    File.Delete(jobOutputFileName);
                }

                string jobOutputDirectory = Path.GetDirectoryName(jobOutputFileName);
                ZipFile zipFile = new ZipFile(jobOutputFileName);
                zipFile.AddDirectory(jobOutputDirectory, "Documents");
                zipFile.AddDirectory(_jobdocumentsDirectory, "Documents");

                zipFile.Save();

                return zipFile.EntryFileNames.ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Zip process failed (Check if the ArcGISServer has write access to the datashop work directories"), ex);
                throw;
            }
        }
    }
}