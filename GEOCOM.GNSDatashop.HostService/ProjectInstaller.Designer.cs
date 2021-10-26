namespace GEOCOM.GNSDatashop.HostService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EndpointHostServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.EndpointHostServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // EndpointHostServiceProcessInstaller
            // 
            this.EndpointHostServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.EndpointHostServiceProcessInstaller.Password = null;
            this.EndpointHostServiceProcessInstaller.Username = null;
            // 
            // EndpointHostServiceInstaller
            // 
            this.EndpointHostServiceInstaller.DisplayName = "GNS Datashop Endpoint Host Service";
            this.EndpointHostServiceInstaller.ServiceName = "GNSDEndpointHostService";
            this.EndpointHostServiceInstaller.Description = "Hosts GNS Datashop WCF services";
            this.EndpointHostServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.EndpointHostServiceProcessInstaller,
            this.EndpointHostServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller EndpointHostServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller EndpointHostServiceInstaller;
    }
}