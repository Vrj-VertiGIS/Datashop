using System;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using GEOCOM.Common.ArcGIS;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.esriSystem;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.PostInstall
{
    public class SchemaCreation
    {

        public static void CreateDatashopUser()
        {

        }

        public static void CreateDatashopSchema()
        {
            NHibernateHelper.CreateSchemaOnDB();
        }

        public static void CreateArcGisSchema(string workingFolder, string sdeConnName)
        {
            // TODO <2010_02_24/ wif / prio1> Fallback server-editor... 

            try
            {
            using (new EsriLicenseInitializer(esriLicenseProductCode.esriLicenseProductCodeArcServer))
            {
                string sdeFileDir = Path.Combine(workingFolder, "config");
                string schemaFile = Path.Combine(workingFolder, @"mxd\GIS-SCHEMA.XML");

                //XmlImportWS("C:/arcgisserver/datashop/config", "GNSDatashoptest", @"C:\arcgisserver\mxd\GIS-SCHEMA.XML");
                XmlImportWS(sdeFileDir, sdeConnName, schemaFile);

            }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CreateArcGisSchema failed: {0}", ex.Message), ex);
            }
        }

        private static void XmlImportWS(string workspacePathname, string dbName, string inXmlFile)
        {
            // Create an Access database (dbName) at the workspacePathname location.
            //IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            //IWorkspaceName workspaceName = workspaceFactory.Create(workspacePathname,
            //  dbName, null, 0);
            // Open the geodatabase.
            //IName name = (IName)workspaceName;
            //IWorkspace workspace = (IWorkspace)name.Open(); // Explicit Cast

            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile(Path.Combine(workspacePathname, dbName),0);

            // Create a GdbImporter.
            IGdbXmlImport gdbXmlImport = new GdbImporterClass();

            // Read schema section from the XML file.
            IEnumNameMapping enumNameMapping = null;
            gdbXmlImport.GenerateNameMapping(inXmlFile, workspace, out enumNameMapping);

            // Create the workspace schema and load the data.
            gdbXmlImport.ImportWorkspace(inXmlFile, enumNameMapping, workspace, false);
        }

      
        public static void PopulateSchema()
        {
            ReasonsStore dbstore = new ReasonsStore();

            dbstore.Add(0, "Planung");
            dbstore.Add(1, "Grabarbeiten");
            dbstore.Add(2, "Reparatur");
            dbstore.Add(3, "Umlegung");
            dbstore.Add(4, "Eingabe Baugesuch");

            UserStore userstore = new UserStore();
            User admin = new User();
            admin.Email = "admin";
            admin.LastName = "Administrator";

            BizUserStore bizuserstore = new BizUserStore();
            BizUser biz_user = new BizUser();
            biz_user.Roles = "BUSINESS,ADMIN";
            biz_user.UserStatus = BizUserStatus.ACTIVATED;
            biz_user.Password = "8F014E1582D03F158C85AD5ECCC658509E4CFF7E"; // datash0p
            biz_user.PasswordSalt = "zd3qHjdp0PnIdBWDKEc8yw==";
            bizuserstore.Add(biz_user, admin);
            
            PlotDefinitionStore plotStore = new PlotDefinitionStore();
            plotStore.Add(new Plotdefinition()
            {
                Description = "A4 Hochformat",
                PlotdefinitionKey = new PlotdefinitionKey() { MediumCode = 0, Template = "a4_hoch" },
                PlotHeightCm = 21,
                PlotWidthCm = 20,
                Roles = "TEMP,BUSINESS"
            });
            plotStore.Add(new Plotdefinition()
            {
                Description = "A4 Querformat",
                PlotdefinitionKey = new PlotdefinitionKey() { MediumCode = 0, Template = "a4_quer" },
                PlotHeightCm = 21,
                PlotWidthCm = 30,
                Roles = "TEMP,BUSINESS"
            });
            plotStore.Add(new Plotdefinition()
            {
                Description = "A3 Hochformat",
                PlotdefinitionKey = new PlotdefinitionKey() { MediumCode = 0, Template = "a3_hoch" },
                PlotHeightCm = 38,
                PlotWidthCm = 28.8,
                Roles = "TEMP,BUSINESS"
            });
            plotStore.Add(new Plotdefinition()
            {
                Description = "A3 Querformat",
                PlotdefinitionKey = new PlotdefinitionKey() { MediumCode = 0, Template = "a3_quer" },
                PlotHeightCm = 21,
                PlotWidthCm = 20,
                Roles = "TEMP,BUSINESS"
            });
            plotStore.Add(new Plotdefinition()
            {
                Description = "A0 (kostenpflichtig)",
                PlotdefinitionKey = new PlotdefinitionKey() { MediumCode = 0, Template = "a0_plot" },
                PlotHeightCm = 21,
                PlotWidthCm = 20,
                Roles = "ADMIN"
            });

        }

        public static void DropDatashopSchema()
        {
             NHibernateHelper.DropSchemaOnDB();
        }
    }
}
