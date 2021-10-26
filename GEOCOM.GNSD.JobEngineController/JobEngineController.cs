using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace GEOCOM.GNSD.JobEngineController
{
    /// <summary>
    /// The job engine controller windows service implementation.
    /// </summary>
    public partial class JobEngineController : ServiceBase
    {
        private MainController mainController;
        private Thread workerThread;

        public JobEngineController()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            mainController = new MainController();
            workerThread = new Thread(new ThreadStart(mainController.Run));
            workerThread.Start();
        }

        protected override void OnStop()
        {
            mainController.Terminate();
            workerThread.Join();
        }
    }
}
