using System;
using GEOCOM.GNSDatashop.HostService;

namespace ServiceTest
{
    /// <summary>
    /// Small console app that enables us to run the WCF services for debugging purposes without having to fully install the service
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point for the console app
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            try
            {
                var manager = new ConfigurableServiceManager();

                Console.WriteLine("Starting services");

                manager.InitializeServices();

                foreach (var s in manager.HostedServiceNames)
                    Console.WriteLine("Started service: {0}", s);

                Console.WriteLine("Successfully started services");

                Console.WriteLine("Press any key to exit");

                Console.ReadLine();

                manager.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                Console.WriteLine("Press any key to exit");

                Console.ReadLine();
            }
        }
    }
}