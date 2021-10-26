using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.Common;
using GEOCOM.Common.Attributes;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DBStore.DbAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GEOCOM.GNSD.DatashopWorkflow.Utils
{
    public static class Utils
    {
        private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        public static string ReplaceVars(string value, IDictionary<string, string> variables)
        {
            return ReplaceVars(value, variables, "%(", ")");
        }

        public static string ReplaceVars(string value, IDictionary<string, string> variables, string startTag, string endTag)
        {
            foreach (KeyValuePair<string, string> var in variables)
            {
                string varName = startTag + var.Key + endTag;
                if (value.Contains(varName))
                {
                    value = value.Replace(varName, var.Value);
                }
            }

            return value;
        }

        public static void OnError(Exception e, long jobId, Type loggerType)
        {
            string message = string.Format("Error: {0} - {1}", e.GetType().Name, e.Message);
            if (loggerType != null)
            {
                var msg = new Msg(loggerType);
                msg.Error(message, e);
            }
            else
            {
                _log.Error(message, e);
            }

            JobStore jobStore = new JobStore();
            var job = jobStore.GetById(jobId);
            var jobLogStore = new JobLogStore();
            jobLogStore.Add(job, message);
        }

        public static string DateFormat(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return string.Format("{0:dd.MM.yyyy}", date);
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();

            return extensions.Aggregate(files, (current, ext) => current.Concat(dir.GetFiles(ext)));
        }


        public static readonly string MdbGeodatabaseFileEnding = ".mdb";
        public static readonly string SdeGeodatabaseFileEnding = ".sde";
        public static readonly string FileGeodatabaseDirectoryEnding = ".gdb";

        private static IWorkspaceFactory GetAccessWorkspaceFactory([NotNullOrEmpty] string location, bool mustExist)
        {
            if (mustExist)
                Assert.True((FileUtils.FileExists(location) ? 1 : 0) != 0, "Access-Workspace location [{0}] does not exist.", new object[1]
        {
          (object) location
        });
            return (IWorkspaceFactory)new AccessWorkspaceFactoryClass();
        }

        private static IWorkspaceFactory GetSdeWorkspaceFactory([NotNullOrEmpty] string location, bool mustExist)
        {
            if (mustExist)
                Assert.True((FileUtils.FileExists(location) ? 1 : 0) != 0, "SDE-Config workspace [{0}] does not exist.", new object[1]
        {
          (object) location
        });
            return (IWorkspaceFactory)new SdeWorkspaceFactoryClass();
        }

        private static IWorkspaceFactory GetFileGdbWorkspaceFactory([NotNullOrEmpty] string location, bool mustExist)
        {
            if (mustExist)
                Assert.True((FileUtils.DirectoryExists(location) ? 1 : 0) != 0, "File-Geodatabase workspace [{0}] does not exist.", new object[1]
        {
          (object) location
        });
            return (IWorkspaceFactory)new FileGDBWorkspaceFactoryClass();
        }

        private static IWorkspaceFactory GetFactoryOfLocation([NotNullOrEmpty] string location, bool mustExist)
        {
            string str = location.ToLowerInvariant();
            if (str.EndsWith(MdbGeodatabaseFileEnding, StringComparison.InvariantCulture))
                return GetAccessWorkspaceFactory(location, mustExist);
            if (str.EndsWith(SdeGeodatabaseFileEnding, StringComparison.InvariantCulture))
                return GetSdeWorkspaceFactory(location, mustExist);
            if (str.EndsWith(FileGeodatabaseDirectoryEnding, StringComparison.InvariantCulture))
                return GetFileGdbWorkspaceFactory(location, mustExist);
            throw new NotSupportedException(string.Format((IFormatProvider)CultureInfo.InvariantCulture,
                "Unsupported workspace-factory for location [{0}].", new object[1]
                {
                    (object) location
                }));
        }

        public static void RelaseAllWorkspaces()
        {
            foreach (IWorkspace obj in Workspaces)
            {
                try
                {
                    var name = obj.PathName;
                    Marshal.ReleaseComObject(obj);
                }
                catch (Exception e)
                {

                }
            }
        }

        private static List<IWorkspace> Workspaces = new List<IWorkspace>();
		
        public static IWorkspace OpenWorkspace(string location)
        {
            Assert.ArgumentNotNullOrEmpty(location, "location");
            IWorkspace workspace = GetFactoryOfLocation(location, true).OpenFromFile(location, 0);
            Assert.NotNull((object)workspace, "Workspace could not be opened from location [{0}].", new object[1]
      {
        (object) location
      });
            Workspaces.Add(workspace);
            return workspace;
        }
    }
}