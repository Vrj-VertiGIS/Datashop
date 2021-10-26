using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using GEOCOM.GNS;
using GEOCOM.GNSD.DBStore;
using GEOCOM.GNSD.DBStore.Config;
using System.Diagnostics;
using System.Data.SqlClient;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.PostInstall.Config;
using Microsoft.Win32;

namespace GEOCOM.GNSD.PostInstall
{
    public partial class SetupWizardForm : Form
    {
        const string REGISTRYPATH = @"HKEY_LOCAL_MACHINE\Software\GEOCOM\Datashop";

        // default locations
        private string _defaultWorkingFolder = @"C:\arcgisserver\datashop";
        private string _defaultHost = "localhost";

        // properties from setup
        private string _workingFolder = @"C:\arcgisserver\datashop";
        private string _exportFolder = @"C:\arcgisserver\datashop\export";
        private bool _setExportFolder = true;

        // TODO Read from registry
        // TODO 25.10.2010 ERM ODAC not supported at the moment
        //private string _installfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        //                                             "GEOCOM/GEONIS/Datashop");

        private string _odacDir;

        public SetupWizardForm()
        {
            InitializeComponent();
        }


        private void SetupWizardForm_Load(object sender, EventArgs e)
        {
            try
            {
                edtArcGisServerHostname.Text = Environment.MachineName;
                edtOracleHost.Text = Environment.MachineName;

                // Get the datashop environment from the registry
                _workingFolder = GetFolderFromRegistry("WorkingFolder", true);
                _exportFolder = GetFolderFromRegistry("ExportFolder", false);

                // Set the mxd default file paths
                edtMxdPathGNSD_ALL.Text = Path.Combine(_workingFolder, @"mxd\GNSD_ALL.mxd");
                edtMxdPathGNSD_AVonly.Text = Path.Combine(_workingFolder, @"mxd\GNSD_AVonly.mxd");

                if (String.IsNullOrEmpty(_exportFolder))
                {
                    // The export folder registry entry will be deleted after PostInstall.
                    // So it will be available only during the first PostInstall!
                    _setExportFolder = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                // read to apply for testing oracle connection
                // TODO 25.10.2010 ERM ODAC not supported at the moment
                //_odacDir = PostInstallConfig.Instance.DbConnection.OraClientHome;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Load configuration file failed: {0}", ex.Message));
            }
            // load config
        }


        private void btnNext1_Click(object sender, EventArgs e)
        {
            if (chkRegisterServerExt.Checked)
            {
                tabControl.SelectedTab = tabPage2;
            }
            else if (chkCreateMapService.Checked)
            {
                tabControl.SelectedTab = tabPage3;
            }
            else if (chkSetupConfig.Checked)
            {
                tabControl.SelectedTab = tabPage4;
            }
            else if (chkSetupDB.Checked)
            {
                tabControl.SelectedTab = tabPage5;
            }
        }

        private void btnNext3_Click(object sender, EventArgs e)
        {
            try
            {
                CreateMapServices();

                if (chkSetupConfig.Checked)
                {
                    tabControl.SelectedTab = tabPage4;
                }
                else if (chkSetupDB.Checked)
                {
                    tabControl.SelectedTab = tabPage5;
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }


        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            try
            {
                // Creates the extension file in progfiles/arcgis/server/cfg...
                RegisterArcGisServerExtension();

                if (chkCreateMapService.Checked)
                {
                    tabControl.SelectedTab = tabPage3;
                }
                else if (chkSetupConfig.Checked)
                {
                    tabControl.SelectedTab = tabPage4;
                }
                else if (chkSetupDB.Checked)
                {
                    tabControl.SelectedTab = tabPage5;
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }


        }

        private void btnNext4_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdDbOracle.Checked)
                {
                    tabControl.SelectedTab = tabODAC;
                }
                else if (rdDbSqlServer.Checked)
                {
                    tabControl.SelectedTab = tabPage42;
                }
                else
                {
                    ModifyConfig();
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void ModifyConfig()
        {
            try
            {

                if (chkSetupConfig.Checked)
                {
                    SetupConfigFiles();
                }

                if (chkConfigureArcCatalog.Checked)
                {
                    SetupSearchExtensionProperties();
                }

                if (chkSetupDB.Checked)
                {
                    tabControl.SelectedTab = tabPage5;
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void btnODACNext_Click(object sender, EventArgs e)
        {
            // TODO 25.10.2010 ERM ODAC not supported at the moment
            //try
            //{
            //    tabControl.SelectedTab = tabPage41;
            //    if (chkInstallODAC.Checked)
            //    {
            //        _odacDir = Path.Combine(_installfolder, @"bin\ODAC");
            //        InstallODAC.Run(_odacDir); // install client
            //        PostInstallConfig.Instance.DbConnection.OraClientHome = _odacDir; // write back for connection test
            //        chkInstallODAC.Checked = false; // don't install again
            //    }
            //}
            //catch (Exception ex)
            //{
            //    handleError(ex);
            //}

        }


        private void btnNext5_Click(object sender, EventArgs e)
        {
            try
            {
                SetupDatabase();

                tabControl.SelectedTab = tabPage6;

            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void SetupDatabase()
        {
            if (chkHibernate.Checked)
            {
                SchemaCreation.CreateDatashopSchema();
            }

            if (chkInitData.Checked)
            {
                SchemaCreation.PopulateSchema();
            }

            if (chkArcGisSchema.Checked)
            {
                SchemaCreation.CreateArcGisSchema(_workingFolder, SDE_FILENAME);
            }
        }

        private void handleError(Exception ex)
        {
            TreeNode errorNode = tvError.Nodes.Add(ex.Message);
            Exception inner = ex.InnerException;
            while (inner != null)
            {
                errorNode = errorNode.Nodes.Add(inner.Message);
                inner = inner.InnerException;
            }

            tabControl.SelectedTab = tabPage7;
        }

        // Strings shown in Arc Catalog and ArcGisServer manager
        internal const string CLSID_SEARCH = "GNSDatashopSearchExtension.ServerObjectExtension";
        internal const string NAME_SEARCH = "GNSDatashopSearchExtension";
        internal const string DISPLAY_NAME_SEARCH = "GEONIS server Datashop SearchExtension";
        internal const string DESCRIPTION_SEARCH = "GEONIS server Datashop SearchExtension";

        private static void RegisterArcGisServerExtension()
        {
            try
            {
                ServerRegistrator.RegisterServer("unregister", "MapServer", NAME_SEARCH, DISPLAY_NAME_SEARCH, CLSID_SEARCH, DESCRIPTION_SEARCH);
                ServerRegistrator.RegisterServer("register", "MapServer", NAME_SEARCH, DISPLAY_NAME_SEARCH, CLSID_SEARCH, DESCRIPTION_SEARCH);
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering mapserver extension", ex);
            }
        }

        private const string SDE_FILENAME = "datashop.sde";
        private const string SDE_SERVICE_5151 = "5151";

        private void SetupSearchExtensionProperties()
        {
            DirectoryInfo configDir = new DirectoryInfo(Path.Combine(_workingFolder, "config"));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("ConnectionPath", Path.Combine(configDir.FullName, SDE_FILENAME));

            string host, user, password, database, version;

            if (rdDbOracle.Checked)
            {
                host = edtOracleHost.Text;
                user = edtOracleUser.Text;
                password = edtOraclePassword.Text;
                database = "";
                version = "sde.DEFAULT"; // TODO add version to gui
            }
            else if (rdDbSqlServer.Checked)
            {
                host = edtMsSqlHost.Text;
                user = edtMsSqlUser.Text;
                password = edtMsSqlPassword.Text;
                database = edtMsSqlDatabase.Text;
                version = "sde.DEFAULT"; // TODO add version to gui
            }
            else // manual
            {
                // todo from config
                host = "";
                user = "";
                password = "";
                database = "";
                version = "";
            }

            ServerRegistrator.EnableArcGISServerExtension("MapServer", "GNSD_AVonly",
                                              "GNSDatashopSearchExtension.ServerObjectExtension",
                                              properties);

            SdeConfiguration.SetupSdeConnFile(configDir, SDE_FILENAME, host, SDE_SERVICE_5151, user,
                                              password, database, version);
        }

        private const string DBSTORE_CONFIGFILENAME = "DbStoreConfig.xml";
        private const string WEBAPP_CONFIGFILENAME = "DatashopWebConfig.xml";
        private const string WEBAPPADMIN_CONFIGFILENAME = "DatashopWebAdminConfig.xml";
        private const string GNSDATASHOPCOMMON_CONFIGFILENAME = "GnsDatashopCommonConfig.xml";
        private const string JOBENGINE_CONFIGFILENAME = "JobEngineConfig.xml";
        private const string PLOTEXTENSION_CONFIGFILENAME = "PlotExtensionConfig.xml";

        private void SetupConfigFiles()
        {
            // Replace myhost with host from installdir
            FileInfo dbStoreConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + DBSTORE_CONFIGFILENAME));
            if (dbStoreConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(dbStoreConfigFile.FullName);

                    // setup db connection

                    XmlElement conNode = (XmlElement)doc.DocumentElement.SelectSingleNode("dbconnection");
                    XmlAttribute provider = conNode.Attributes["provider"];
                    XmlAttribute connstring = conNode.Attributes["connectionstring"];
                    XmlAttribute odacHome = conNode.Attributes["oraClientHome"];

                    if (rdDbOracle.Checked)
                    {
                        odacHome.Value = _odacDir;
                        connstring.Value = GetOracleConnectionString();
                        provider.Value = "Oracle.DataAccess.Client";
                    }
                    else if (rdDbSqlServer.Checked)
                    {
                        SqlConnectionStringBuilder sb = GetMsSqlConnectionString();
                        connstring.Value = sb.ConnectionString;
                        provider.Value = "System.Data.SqlClient";
                    }

                    XmlTextWriter xmlTW = new XmlTextWriter(dbStoreConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", dbStoreConfigFile.FullName));
            }

            FileInfo webApplicationConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + WEBAPP_CONFIGFILENAME));
            if (webApplicationConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(webApplicationConfigFile.FullName);

                    // setup WebApp
                    XmlElement mapserviceNode = (XmlElement)doc.DocumentElement.SelectSingleNode("mapservice");

                    XmlAttribute agbfilepath = mapserviceNode.Attributes["agbfilepath"];
                    agbfilepath.Value = ReplaceWorkingFolder(agbfilepath.Value);

                    XmlAttribute serviceurl = mapserviceNode.Attributes["serviceurl"];
                    serviceurl.Value = ReplaceHostName(serviceurl.Value, edtArcGisServerHostname.Text);

                    XmlTextWriter xmlTW = new XmlTextWriter(webApplicationConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", webApplicationConfigFile.FullName));
            }

            FileInfo webApplicationAdminConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + WEBAPPADMIN_CONFIGFILENAME));
            if (webApplicationAdminConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(webApplicationAdminConfigFile.FullName);

                    // setup WebApp
                    XmlElement mapserviceNode = (XmlElement)doc.DocumentElement.SelectSingleNode("mapservice");

                    XmlAttribute serviceurl = mapserviceNode.Attributes["serviceurl"];
                    serviceurl.Value = ReplaceHostName(serviceurl.Value, edtArcGisServerHostname.Text);

                    XmlTextWriter xmlTW = new XmlTextWriter(webApplicationAdminConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", webApplicationAdminConfigFile.FullName));
            }

            FileInfo gnsDatashopCommonConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + GNSDATASHOPCOMMON_CONFIGFILENAME));
            if (gnsDatashopCommonConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(gnsDatashopCommonConfigFile.FullName);

                    // setup diretcories
                    XmlElement directoriesNode = (XmlElement)doc.DocumentElement.SelectSingleNode("directories");

                    if (_setExportFolder)
                    {
                        XmlAttribute exportdirectory = directoriesNode.Attributes["exportdirectory"];
                        exportdirectory.Value = _exportFolder;  // Export folder can be defined during installation process
                    }

                    XmlAttribute profiledirectory = directoriesNode.Attributes["profiledirectory"];
                    profiledirectory.Value = ReplaceWorkingFolder(profiledirectory.Value);

                    XmlAttribute archivedirectory = directoriesNode.Attributes["archivedirectory"];
                    archivedirectory.Value = ReplaceWorkingFolder(archivedirectory.Value);

                    XmlAttribute logdirectory = directoriesNode.Attributes["logdirectory"];
                    logdirectory.Value = ReplaceWorkingFolder(logdirectory.Value);

                    XmlAttribute uploaddirectory = directoriesNode.Attributes["uploaddirectory"];
                    uploaddirectory.Value = ReplaceWorkingFolder(uploaddirectory.Value);

                    // setup mailserver
                    XmlElement mailNode = (XmlElement)doc.DocumentElement.SelectSingleNode("mail");
                    XmlElement serverNode = (XmlElement)mailNode.SelectSingleNode("server"); // just the first
                    XmlAttribute server = serverNode.Attributes["server"];
                    server.Value = edtMailserver.Text;

                    // TODO 2.11.2010 ERM The IIS Host and the virtual directory of the Datashop/DatashopAdmin
                    // Web applications is configurable during setup. At the moment there is no easy way to fetch
                    // this information at Post Setup. Therefore the mailtemplate URL's won't be adjusted atm.

                    //XmlNodeList nodes = mailNode.SelectNodes(@"mailtemplate/@downloadurl");
                    //foreach (XmlNode node in nodes)
                    //{
                    //    node.Value = ReplaceHostName(node.Value, edtArcGisServerHostname.Text);
                    //}

                    XmlTextWriter xmlTW = new XmlTextWriter(gnsDatashopCommonConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", gnsDatashopCommonConfigFile.FullName));
            }

            FileInfo jobEngineConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + JOBENGINE_CONFIGFILENAME));
            if (jobEngineConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(jobEngineConfigFile.FullName);

                    // upload
                    XmlElement extentDatabaseNode = (XmlElement)doc.DocumentElement.SelectSingleNode("extentdatabase");

                    XmlAttribute extentPath = extentDatabaseNode.Attributes["path"];
                    extentPath.Value = ReplaceWorkingFolder(extentPath.Value);

                    XmlElement notificationDatabaseNode = (XmlElement)doc.DocumentElement.SelectSingleNode("notificationdatabase");
                    XmlAttribute notificationPath = notificationDatabaseNode.Attributes["path"];
                    notificationPath.Value = ReplaceWorkingFolder(notificationPath.Value);

                    XmlElement exportNode = (XmlElement)doc.DocumentElement.SelectSingleNode("export");
                    XmlAttribute templateDirectory = exportNode.Attributes["templatedirectory"];
                    templateDirectory.Value = ReplaceWorkingFolder(templateDirectory.Value);

                    XmlElement lettertemplateNode = (XmlElement)doc.DocumentElement.SelectSingleNode("lettertemplate");
                    XmlAttribute file = lettertemplateNode.Attributes["file"];
                    file.Value = ReplaceWorkingFolder(file.Value);

                    XmlTextWriter xmlTW = new XmlTextWriter(jobEngineConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", jobEngineConfigFile.FullName));
            }

            FileInfo plotExtensionConfigFile = new FileInfo(Path.Combine(_workingFolder, @"config\" + PLOTEXTENSION_CONFIGFILENAME));
            if (plotExtensionConfigFile.Exists)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(plotExtensionConfigFile.FullName);

                    // upload
                    XmlElement mxdPathInfoNode = (XmlElement)doc.DocumentElement.SelectSingleNode("mxdpathinfo");

                    XmlAttribute path = mxdPathInfoNode.Attributes["path"];
                    path.Value = ReplaceWorkingFolder(path.Value);

                    XmlElement exportNode = (XmlElement)doc.DocumentElement.SelectSingleNode("export");
                    XmlAttribute plotTemplates = exportNode.Attributes["plottemplates"];
                    plotTemplates.Value = ReplaceWorkingFolder(plotTemplates.Value);

                    XmlTextWriter xmlTW = new XmlTextWriter(plotExtensionConfigFile.FullName, Encoding.UTF8);
                    xmlTW.Formatting = Formatting.Indented;
                    doc.Save(xmlTW);

                    xmlTW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                throw new Exception(String.Format("Configfile: {0:s} not found", plotExtensionConfigFile.FullName));
            }
        }

        private void CreateMapServices()
        {
            try
            {
                ServerRegistrator.CreateMapService("MapServer", "GNSD_ALL", edtMxdPathGNSD_ALL.Text);
                ServerRegistrator.CreateMapService("MapServer", "GNSD_AVonly", edtMxdPathGNSD_AVonly.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating mapserver", ex);
            }

        }

        private void btnCancel1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void btnCancel3_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void btnCancel4_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void btnCancel5_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void btnChooseMxdAll_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "GNSD_ALL.mxd";
            openFileDialog1.InitialDirectory = Path.Combine(_workingFolder, "mxd");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edtMxdPathGNSD_ALL.Text = openFileDialog1.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "GNSD_AVonly.mxd";
            openFileDialog1.InitialDirectory = Path.Combine(_workingFolder, "mxd");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edtMxdPathGNSD_AVonly.Text = openFileDialog1.FileName;
            }

        }

        private void chkUseSID_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseSID.Checked)
            {
                lblSID.Text = "SID";
            }
            else
            {
                lblSID.Text = "Servicename";
            }
        }

        private void btnTestOracleConn_Click(object sender, EventArgs e)
        {
            try
            {
                string sb = GetOracleConnectionString();
                NHibernateHelper.TestOracleConnection(sb);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed:" + ex.Message);
            }
        }

        private string GetOracleConnectionString()
        {
            string userPassword = edtOracleUser.Text == "" ? "" : string.Format("User Id={0};Password={1};", edtOracleUser.Text, edtOraclePassword.Text);
            //return String.Format("(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0:s})(PORT={1:s})))(CONNECT_DATA=({2:s}={3:s})));{4}", edtOracleHost.Text, edtOraclePort.Text, chkUseSID.Checked ? "SID" : "SERVICE_NAME", edtServiceName.Text, userPassword);
            return String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVER=DEDICATED)({2}={3})));{4}", edtOracleHost.Text, edtOraclePort.Text, chkUseSID.Checked ? "SID" : "SERVICE_NAME", edtServiceName.Text, userPassword);
        }

        private SqlConnectionStringBuilder GetMsSqlConnectionString()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();// ignore config.ConnectionString); because its an oracle connectionstring

            if (!chkUseIntegratedSecurity.Checked)
            {
                sb.UserID = edtMsSqlUser.Text;
                sb.Password = edtMsSqlPassword.Text;
                sb.IntegratedSecurity = false;
            }
            else
            {
                sb.IntegratedSecurity = true;
            }
            sb.InitialCatalog = edtMsSqlDatabase.Text;
            sb.DataSource = edtMsSqlHost.Text;
            return sb;
        }

        private void btnCancel41_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void btnNext41_Click(object sender, EventArgs e)
        {
            ModifyConfig();
        }

        private void btnNext42_Click(object sender, EventArgs e)
        {
            ModifyConfig();
        }

        private void btnTestMsSqlConnection_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnectionStringBuilder sb = GetMsSqlConnectionString();
                NHibernateHelper.TestMsSqlConnection(sb);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed:" + ex.Message);
            }

        }

        private void btnExitError_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnIgnoreError_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPage1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Try to remove the registry key for the export folder
            Registry.LocalMachine.DeleteValue(Path.Combine(REGISTRYPATH, "ExportFolder"), false);

            if (chkStartMapserver.Checked)
            {
                try
                {
                    ServerRegistrator.StartArcGisService("MapServer", "GNSD_ALL", true);
                    ServerRegistrator.StartArcGisService("MapServer", "GNSD_AVonly", true);
                }
                catch (Exception)
                { } // Ignore
            }

            if (chkShowLogin.Checked)
            {
                Process browser = new Process();
                browser.StartInfo.FileName = "http://localhost/Datashop/WelcomePage.aspx";
                browser.StartInfo.Arguments = "";
                browser.StartInfo.UseShellExecute = true;
                //browser.StartInfo.RedirectStandardOutput = true;
                browser.Start();

                //Console.WriteLine(browser.StandardOutput.ReadToEnd());

                //browser.WaitForExit();
            }

            Close();
        }

        private void chkUseIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseIntegratedSecurity.Checked)
            {
                edtMsSqlUser.Enabled = false;
                edtMsSqlPassword.Enabled = false;
            }
            else
            {
                edtMsSqlUser.Enabled = true;
                edtMsSqlPassword.Enabled = true;
            }
        }

        private string GetFolderFromRegistry(string registryKey, bool throwMissingException)
        {
            string folder = Registry.GetValue(REGISTRYPATH, registryKey, null) as string;
            if (string.IsNullOrEmpty(folder) && throwMissingException)
            {
                string msg = string.Format(@"RegistryKey={0} not found in registry at {1}", registryKey, REGISTRYPATH);
                throw new Exception(msg);
            }
            return folder;
        }

        /// <summary>
        /// Replace the original working folder with the working folder defined during the Setup
        /// </summary>
        /// <param name="originalPath">The original path</param>
        /// <returns>The new path (the working folder is replaced with the working folder defined during the Setup process)</returns>
        private string ReplaceWorkingFolder(string originalPath)
        {
            string purgedPath = originalPath.ToLower().Replace("/", @"\");
            return purgedPath.Replace(_defaultWorkingFolder.ToLower(), _workingFolder);            
        }

        /// <summary>
        /// Replace the original host name with the host name defined during the Setup
        /// </summary>
        /// <param name="originalHostUrl">The original host url</param>
        /// <param name="newHostName">The new host name</param>
        /// <returns>The new host url (the host name is replaced with the host name defined in the Post Setup)</returns>
        private string ReplaceHostName(string originalHostUrl, string newHostName)
        {
            return originalHostUrl.ToLower().Replace(_defaultHost.ToLower(), newHostName);
        }
    }
}