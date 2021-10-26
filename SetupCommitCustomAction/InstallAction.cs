using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;

namespace SetupCommitCustomAction
{
    public class InstallAction
    {
        private InstallContext installContext;
       
        private IDictionary stateSaver;
       
        private RegistryKey webAppKey;

        public InstallAction(InstallContext context, IDictionary stateSaver)
        {
            installContext = context;
            this.stateSaver = stateSaver;
        }

        public string InstallPath
        {
            get
            {
                string value = (string)WebAppKey.GetValue("InstallPath");
                return value;
            }
        }

        public string IISSite
        {
            get
            {
                string value = (string)WebAppKey.GetValue("IISSite");
                return value;
            }
        }

        public string VirtualDirectory
        {
            get
            {
                string value = (string)WebAppKey.GetValue("VirtualDirectory");
                return value;
            }
        }

        public string ProductName
        {
            get
            {
                string productName = WebAppKey.GetValue("ProductName") as string;
                return productName;
            }
        }

        public string StartMenuFolderPath
        {
            get
            {
                StringBuilder allUsersProgramFiles = new StringBuilder(500);
                SHGetFolderPath(IntPtr.Zero, 0x17, IntPtr.Zero, 0, allUsersProgramFiles);

                string startMenuFolderPath = allUsersProgramFiles.ToString();
                if (string.IsNullOrEmpty(startMenuFolderPath))
                    startMenuFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                startMenuFolderPath = Path.Combine(startMenuFolderPath, @"GEOCOM\GEONIS server Datashop");
                return startMenuFolderPath;
            }
        }

        public string ShortcutPath
        {
            get
            {
                string path = Path.Combine(StartMenuFolderPath, ProductName + ".lnk");
                return path;
            }
        }

        private RegistryKey WebAppKey
        {
            get
            {
                if (webAppKey == null)
                {
                    string currentInstallationRegistryKey = installContext.Parameters["CurrentInstallationRegistryKey"];
                    webAppKey = Registry.LocalMachine.OpenSubKey(currentInstallationRegistryKey, true);
                }
                return webAppKey;
            }
        }

        public static DirectoryEntry GetIISDirectoryEntry(string metabaseValue)
        {
            string directoryPath = metabaseValue.Replace("/LM", "localhost");
            DirectoryEntry entry = new DirectoryEntry("IIS://" + directoryPath);
            return entry;
        }

        public void Install()
        {

            try
            {
                EditLinkFile();
                WriteApplicationURLToRegistry();
                CreateStartMenuLink();
                SaveState(stateSaver);
            }
            catch (Exception e)
            {
            //    MessageBox.Show("Creating start menu link: " + e.Message, "Exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetApplicationURL()
        {
            int port = GetPort();
            string url = string.Format("http://localhost:{0}/{1}", port, VirtualDirectory);
            return url;
        }

        public int GetPort()
        {
            string metabaseValue = IISSite;
            DirectoryEntry entry = GetIISDirectoryEntry(metabaseValue);
            string portString = entry.Properties["ServerBindings"].Value.ToString().Replace(":", string.Empty);
            int port = Convert.ToInt32(portString);
            return port;
        }

        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        internal static extern int SHGetFolderPath(IntPtr hwndOwner, int numFolder, IntPtr handleToken, int dwordFlags, StringBuilder lpszPath);

        private void CreateStartMenuLink()
        {
            if (!Directory.Exists(StartMenuFolderPath))
                Directory.CreateDirectory(StartMenuFolderPath);

            WshShellClass shellClass = new WshShellClass();
            IWshShortcut wshShortcut = (IWshShortcut)shellClass.CreateShortcut(ShortcutPath);
            wshShortcut.TargetPath = Path.Combine(InstallPath, "weblink.url");
            wshShortcut.IconLocation = Path.Combine(InstallPath, "linkicon.ico");
            wshShortcut.Description = ProductName;

            wshShortcut.Save();
        }

        private void SaveState(IDictionary stateSaver)
        {
            stateSaver["ShortcutPath"] = ShortcutPath;
            stateSaver["StartMenuFolderPath"] = StartMenuFolderPath;
        }

        private void EditLinkFile()
        {
            string url = GetApplicationURL();
            string fileContent = GetLinkFileContent();
            fileContent = fileContent.Replace("%url%", url);
            RewriteLinkFile(fileContent);
        }

        private void WriteApplicationURLToRegistry()
        {
            string url = GetApplicationURL();
            WebAppKey.SetValue("URL", url);
        }

        private string GetLinkFileContent()
        {
            using (Stream linkFileStream = OpenLinkFile())
            {
                StreamReader reader = new StreamReader(linkFileStream);
                string fileContent = reader.ReadToEnd();
                return fileContent;
            }
        }

        private FileStream OpenLinkFile()
        {
            string linkPath = Path.Combine(InstallPath, "weblink.url");
            FileStream stream = File.Open(linkPath, FileMode.Open, FileAccess.ReadWrite);
            return stream;
        }

        private void RewriteLinkFile(string fileContent)
        {
            using (Stream linkFileStream = OpenLinkFile())
            {
                StreamWriter writer = new StreamWriter(linkFileStream);
                writer.Write(fileContent);
                writer.Dispose();
            }
        }
    }
}
