using System.ServiceProcess;
using System.Threading;

namespace GEOCOM.GNSD.JobEngineController
{
    public static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] 
            { 
                new JobEngineController() 
            };
            ServiceBase.Run(servicesToRun);

            // NOTE: swap commented out code when releasing!
            //(new Thread(new ThreadStart((new MainController()).Run))).Start();

            //while (true) Thread.Sleep(100);
        }
    }
}
