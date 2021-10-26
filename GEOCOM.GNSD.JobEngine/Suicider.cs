using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow.Report;
using GEOCOM.GNSD.DatashopWorkflow.Utils;
using GEOCOM.GNSD.DBStore.Archive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GEOCOM.GNSD.JobEngine
{
    /// <summary>
    /// This class is a workaround for process that stay inactive and hanging. Such processes consume resource and eventually lead to their depletion.
    /// The finalizers are last to be executed in a .NET application and so are the natural fit for shutting down the app. 
    /// </summary>
    public class Suicider
    {
        public static void Commit()
        {
            // create an object on the head and wait for the finalizer thread let it swallow.
            new Suicider();
        }

        ~Suicider()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
