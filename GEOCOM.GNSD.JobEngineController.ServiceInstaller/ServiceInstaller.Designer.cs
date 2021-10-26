using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GEOCOM.GNSD.JobEngineController.ServiceInstaller
{
    partial class ServiceInstaller
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            string directoryName = Path.GetDirectoryName(GetType().Assembly.Location);
            // Why this? Because the GEOCOM.GNSD.JobEngineController.exe might be 64-bit (ArcGIS 10.1 and 10.2...) 
            // but the ServiceInstaller that is run by the setup (installutils) must be 32-bit. 
            string jobEngineControllerDllPath = Path.Combine(directoryName, "GEOCOM.GNSD.JobEngineController.exe");
            this.serviceInstaller1 = new CustomDllServiceInstaller(jobEngineControllerDllPath);
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.DisplayName = "GNS Datashop Job Engine Controller Service";
            this.serviceInstaller1.ServiceName = "GNSDJobEngineController";
            this.serviceInstaller1.Description = "Initializes and controls GNSD Job Engine";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private CustomDllServiceInstaller serviceInstaller1;
    }
}