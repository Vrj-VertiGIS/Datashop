using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using GEOCOM.Common.ArcGIS;

namespace GEOCOM.GNSD.PostInstall
{
    public class SdeConfiguration
    {
        public static void SetupSdeConnFile(DirectoryInfo path, string name, string serverName, string sdeInstance, string user, string password, string database, string version)
        {
            // TODO <2010_02_24/ wif / prio3> Fallback licence

            using (new EsriLicenseInitializer(esriLicenseProductCode.esriLicenseProductCodeArcServer))
            {
                name = Path.GetFileNameWithoutExtension(name);
                string sdeFile = Path.Combine(path.FullName, Path.ChangeExtension(name, "sde"));

              

                if (!File.Exists(sdeFile))
                {

                    IPropertySet set = new PropertySetClass();
                    set.SetProperty("Server", serverName);
                    set.SetProperty("Instance", sdeInstance);
                    set.SetProperty("Database", database);
                    set.SetProperty("user", user);
                    set.SetProperty("password", password);
                    set.SetProperty("version", version);
                    //set.SetProperty("", "");
                    //set.SetProperty("", "");
                    IWorkspaceFactory factory = new SdeWorkspaceFactoryClass();
                    //(factory as IRemoteDatabaseWorkspaceFactory).DeleteConnectionFile(sdeFile);
                    factory.Create(path.FullName, name, set, 0);
                }
                //else // immer anzeigen
                {
                    IWorkspaceFactory factory = new SdeWorkspaceFactoryClass();
                    (factory as IRemoteDatabaseWorkspaceFactory).EditConnectionFile(sdeFile, 0);
                }
            }
        }
    }
}