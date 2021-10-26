using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetupCommitCustomAction
{

   public class UninstallAction
    {
      
        public  void Uninstall(IDictionary savedState)
        {
            try
            {
              //  MessageBox.Show("Now attach to process...", "Debugging info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InitState(savedState);
                DeleteStartMenuLink();
            }
            catch (Exception e)
            {
              //  MessageBox.Show("Uninstalling start menu link: " + e.Message, "Exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitState(IDictionary savedState)
        {
            ShortcutPath = savedState["ShortcutPath"] as string;
            StartMenuFolderPath = savedState["StartMenuFolderPath"] as string;
        }

        public string ShortcutPath { get; set; }
        public string StartMenuFolderPath { get; set; }

        private void DeleteStartMenuLink()
        {
            File.Delete(ShortcutPath);
            DirectoryInfo startMenu = new DirectoryInfo(StartMenuFolderPath);
            bool dirEmpty = (startMenu.GetFiles().Length == 0);
            if (dirEmpty)
                startMenu.Delete();
        }
    }
}
