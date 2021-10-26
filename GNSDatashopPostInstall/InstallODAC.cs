using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEOCOM.GNSD.PostInstall
{
    class InstallODAC
    {
        public static void Run(string odac_home)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Please choose the Oracle Client setup folder on your install media";
            if(dialog.ShowDialog()== DialogResult.OK)
            {
                string installPath = dialog.SelectedPath;

                var exe = Path.Combine(installPath,"install.bat");
                 if (File.Exists(exe))
                {
                    try
                    {
                        ProcessStartInfo startinfo = new ProcessStartInfo(exe);
                        startinfo.Arguments = string.Format("odp.net20 \"{0}\" ODAC", odac_home);
                        startinfo.RedirectStandardOutput = false;
                        startinfo.CreateNoWindow = false;
                        startinfo.UseShellExecute = false;
                        startinfo.WorkingDirectory = installPath;

                        Process p = Process.Start(startinfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Execute Oracle Client setup failed: {0}", ex.Message), "Oracle Client setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            }
        }

    }
}
