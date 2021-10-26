using System.ComponentModel;
using System.Configuration.Install;

namespace GEOCOM.GNSD.JobEngineController.ServiceInstaller
{
    /// <summary>
    /// The installer part of the job engine controller
    /// </summary>
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
