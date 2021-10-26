using System.ServiceProcess;

namespace GEOCOM.GNSDatashop.HostService
{
    /// <summary>
    /// Entry point for the Windows Service
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the service application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
                                    { 
                                        new EndpointHostService() 
                                    };

            ServiceBase.Run(servicesToRun);
        }
    }
}