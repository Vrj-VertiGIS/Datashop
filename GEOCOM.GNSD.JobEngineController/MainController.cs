using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.JobEngineController.Config;
using GEOCOM.GNSD.JobEngineController.Scheduler;
using System;
using System.Reflection;
using System.Threading;

namespace GEOCOM.GNSD.JobEngineController
{
    public class MainController
    {
        /// <summary>
        /// Flag that indicates that the working thread is running.
        /// </summary>
        private bool _isDone = false;

   

        /// <summary>
        /// Log instance.
        /// </summary>
        private IMsg _log;

        static MainController()
        {
            DatashopLogInitializer.Initialize();
        }

        public MainController()
        {
            _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Initializes the service. If successful, the worker
        /// loop is started and the jobs are scheduled.
        /// </summary>
        public void Run()
        {
            var jobScheduler = new JobScheduler();
            try
            {
                _log.Info("Start job engine controller service.");
                int iterationCounter = 0; // run at the very first iteration
                int cleaningInterval = JobEngineControllerConfig.Instance.Scheduler.CleaningInterval;
                cleaningInterval = cleaningInterval <= 0 ? 10 : cleaningInterval;

                int jobBalancing = 0; // balance new and old process ids when processing - e.g. old job archiving blocked new customer job requests 
                while (!_isDone)
                {
                    var runZombieCleaning = iterationCounter++ % cleaningInterval == 0;
                    if (runZombieCleaning)
                    {
                        jobScheduler.CleanZombieJobsAndProcesses();
                        iterationCounter = 1; // not to run every iteration 
                    }

                    var iterationStartTime = DateTime.Now;
                    var jobStarted = jobScheduler.ExecutePendingJobs(prefereOldestJobs: jobBalancing >= 0);
                    if (jobBalancing >= 0)
                    {
                        jobBalancing -= jobStarted;
                    }
                    else
                    {
                        jobBalancing += jobStarted;
                    }

                    var msToWait = JobEngineControllerConfig.Instance.Scheduler.Timer.ScheduleTimer - (DateTime.Now - iterationStartTime).TotalMilliseconds;
                    if (msToWait < 0) 
                        continue;

                    _log.Debug($"Waiting {msToWait} ms for next polling.");
                    Thread.Sleep((int)msToWait);
                }
            }
            catch (Exception e)
            {
                _log.Error($"Job Engine Controller failed - {e.Message}.", e);
            }
            finally
            {
                _log.Info("Shutdown job engine controller service.");
            }
        }

        /// <summary>
        /// Sets the termination flag to terminate
        /// the worker loop and worker thread.
        /// </summary>
        public void Terminate()
        {
            _isDone = true;
        }

    }
}