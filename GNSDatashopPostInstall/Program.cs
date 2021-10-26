using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace GEOCOM.GNSD.PostInstall
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //using (new GEOCOM.Common.ArcGIS.EsriLicenseInitializer(esriLicenseProductCode.esriLicenseProductCodeArcServer))
            {
                   Application.Run(new SetupWizardForm());
            }
        }
    }
}
