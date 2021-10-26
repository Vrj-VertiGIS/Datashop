using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.JobEngineController.Config;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;

namespace GEOCOM.GNSD.JobEngineController.Scheduler
{
    public class JobScheduler
    {
        private readonly IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// List of running processes started from this controller
        /// </summary>
        private readonly IList<Process> _childProcesses = new SynchronizedCollection<Process>();


        private readonly LoadBalancingPreference _balancingPref = JobEngineControllerConfig.Instance?.Scheduler?.LoadBalancing?.Preference ?? LoadBalancingPreference.NONE;
        private Task _zombieCleanTask;

        private static string JobEngineProcessName => Path.GetFileNameWithoutExtension(JobEngineControllerConfig.Instance.JobEngine.Path);


        public JobScheduler()
        {
            _log.Info($"Load balancing set to {_balancingPref}'");
        }

        #region Process and job cleaning

        /// <summary>
        /// Compare the DB jobs` state with the machine state and clean up.
        /// </summary>
        /// <remarks>
        /// This method makes a potentially longer db query and might take some time to return. 
        /// </remarks>
        public void CleanZombieJobsAndProcesses()
        {
            if (_zombieCleanTask != null && !_zombieCleanTask.IsCompleted)
            {
                _log.Debug("Previous clean-up of zombie processes and zombie jobs still in progress.");
                return;
            }

            _zombieCleanTask = Task.Run(() =>
            {
                try
                {
                    _log.Debug("Trying to clean zombie processes and zombie jobs.");

                    // get processes before the db call to overcome a situation when a process 
                    // correctly ends right during the query processing and thus are some jobs marked 
                    // as a zombie jobs since there were no corresponding processes running.
                    var currentProcesses = Process.GetProcessesByName(JobEngineProcessName);

                    // query jobs from DB in the running state 
                    var jobStore = new JobStore();
                    var runningDBJobs = jobStore.GetJobsByState((int)WorkflowStepState.Running)
                        .Where(job =>
                            Environment.MachineName.Equals(job.MachineName,
                                StringComparison.InvariantCultureIgnoreCase))
                        .Where(job => job.ProcessId != null)
                        .ToList();

                    // check if the db jobs have their corresponding machine process
                    FailZombieJobs(runningDBJobs, currentProcesses);

                    // check if the machine process have their corresponding running db jobs
                    KillZombieProcesses(runningDBJobs);
                }
                catch (Exception e)
                {
                    _log.Error($"Error during clean up - {e.Message}", e);
                }
            });
        }

        /// <summary>
        /// Fail jobs with state <see cref="WorkflowStepState.Running"/> in the db without a corresponding process. 
        /// </summary>
        /// <param name="runningDBJobs">Jobs from DB in the <see cref="WorkflowStepState.Running"/> state</param>
        /// <param name="currentProcesses">Job Engine processes running on this machine.</param>
        private void FailZombieJobs(List<Job> runningDBJobs, Process[] currentProcesses)
        {
            foreach (Job job in runningDBJobs)
            {
                var runningProcess = currentProcesses.SingleOrDefault(p => p.Id == (job.ProcessId ?? -1));
                if (runningProcess != null)
                    continue;

                IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(job.JobId, false);
                workflow.SetFailed();

                _log.WarnFormat(
                    $"A job JobId = {job.JobId} found with state 'running' in the DB but its associated process PID = {job.ProcessId} does not exists any more on the machine. Failing the job.");
            }
        }

        /// <summary>
        /// Kill JobEngine processes that keep hanging.
        /// In the DB are these processes not any more as running but they are still present on the machine and
        /// consume resources.
        /// </summary>
        /// <param name="runningDBJobs">Jobs from DB in the <see cref="WorkflowStepState.Running"/> state</param>
        private void KillZombieProcesses(List<Job> runningDBJobs)
        {
            var processIDsToKeep = runningDBJobs.Select(j => j.ProcessId ?? -1).ToArray(); // correspond to the running jobs in the db
            var ownZombieChildProcesses = _childProcesses.Where(
                p => !ProcHasExited(p)
                     && !processIDsToKeep.Contains(ProcId(p))
                     && ProcStartTime(p).AddMinutes(15) < DateTime.Now // give each process at lease 15 minutes to finish
                     ).ToArray();

            // first kill zombie child process - started by this instance
            foreach (var myZombieChildProcess in ownZombieChildProcesses)
            {
                KillZombieProcess(myZombieChildProcess);
            }

            // second find and kill other job engine process - not started by this instance
            processIDsToKeep = processIDsToKeep.Union(_childProcesses.Select(myp => (long)myp.Id)).ToArray();
            var somebodyElsesZombieProcesses =
                Process.GetProcessesByName(JobEngineProcessName)
                    .Where(p => !processIDsToKeep.Contains(p.Id))
                    .ToArray();

            foreach (var someoneElsesZombieProcess in somebodyElsesZombieProcesses)
            {
                KillZombieProcess(someoneElsesZombieProcess);
            }

            void KillZombieProcess(Process zombieProcess, int level = 0)
            {
                string indentation = String.Concat(Enumerable.Repeat(" ", level * 2));
                string processName = "unavailable"; // this might not be available due to permissions...
                try
                {
                    const int processKillWaitTime = 1_000; // wait max 1s 
                    try
                    {
                        processName = zombieProcess.ProcessName; // the name is not available after the process exits. Record it now.
                    }
                    catch (Exception e)
                    {
                        _log.Warn("Cannot get process name.", e);
                    }

                    _log.Warn($"{indentation}Killing the zombie process '{processName}', PID = {zombieProcess.Id} and its tree.");
                    // find and kill child processes
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher
                        ("Select * From Win32_Process Where ParentProcessID=" + zombieProcess.Id);
                    ManagementObjectCollection moc = searcher.Get();
                    foreach (ManagementObject mo in moc)
                    {
                        var childProcId = Convert.ToInt32(mo["ProcessID"]);
                        var childProc = Process.GetProcessById(childProcId);
                        KillZombieProcess(childProc, level + 1);
                    }

                    // first try the kernel32.TerminateProcess function - https://referencesource.microsoft.com/#System/services/monitoring/system/diagnosticts/Process.cs,4bfdd089d4e52d08
                    try
                    {
                        // attempt the classical way
                        zombieProcess.Kill();
                        zombieProcess.WaitForExit(processKillWaitTime);
                        if (!zombieProcess.HasExited) // HasExited can throw an exception - e.g. 
                            throw new ApplicationException("Process did not exit");
                    }
                    catch (Exception e)
                    {
                        _log.Warn($"An attempt to terminate the zombie process {processName}, PID={zombieProcess.Id} using the kernel32.TerminateProcess function failed.", e);
                    }


                    // https://docs.microsoft.com/en-us/windows/win32/wmisdk/wmic
                    // https://superuser.com/questions/109010/kill-a-process-which-says-access-denied/1109598#1109598

                    Process process = new Process
                    {
                        StartInfo =
                            {
                                FileName = "wmic.exe",
                                Arguments = $"process where processid='{zombieProcess.Id}' delete",
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            }
                    };

                    process.Start();
                    process.WaitForExit(processKillWaitTime);

                    try
                    {
                        if (!zombieProcess.HasExited)
                            throw new ApplicationException("Process did not exit");
                    }
                    catch (Exception e)
                    {
                        StreamReader reader = process.StandardOutput;
                        string wmicOutput = reader.ReadToEnd();
                        throw new ApplicationException(
                            $"An attempt to kill the zombie process '{processName}', PID={zombieProcess.Id} using WMIC failed. WMIC output was '{wmicOutput}'. Please terminate the process manually.", e);
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
                catch (Exception e)
                {
                    _log.Error($"Error killing the zombie process '{processName}', PID={zombieProcess.Id} - {e.Message}.", e);
                }
            }
        }


        #endregion

        #region Jobs execution

        /// <summary>
        /// All jobs selected from db with status = "new" are processed 
        /// Only jobs processable in the current context will be processed
        /// </summary>
        public int ExecutePendingJobs(bool prefereOldestJobs)
        {
            try
            {
                JobStore jobStore = new JobStore();
                // check number of running processes and , if there are already too many processes running.
                DispatchChildProcesses();
                var jobEngineProcesses = Process.GetProcessesByName(JobEngineProcessName);
                int runningProcess = Math.Max(_childProcesses.Count, jobEngineProcesses.Length); ;
                int availableCapacity =
                    JobEngineControllerConfig.Instance.Scheduler.MaxJobProcesses - runningProcess;
                if (availableCapacity < 0)
                {
                    _log.Warn($"Max number of processes are running ({runningProcess})! No new job process will be started!");
                    return 0;
                }

                _log.DebugFormat($"{runningProcess} {JobEngineProcessName} processes are running, max count is {JobEngineControllerConfig.Instance.Scheduler.MaxJobProcesses}.");

                var pendingJobs = jobStore.GetPendingJobs(quantity: availableCapacity, _balancingPref, prefereOldestJobs);
                ExecuteJobs(pendingJobs);
                return pendingJobs.Count;

            }
            catch (Exception ex)
            {
                _log.Error($"There was an error ({ex.Message}) executing pending jobs", ex);
            }

            return 0;
        }

        private void DispatchChildProcesses()
        {
            var runningChildren = _childProcesses.Where(process => !ProcHasExited(process)).ToArray();
            var exitedProcesses = _childProcesses.Where(process => ProcHasExited(process)).ToArray();
            _childProcesses.Clear();

            foreach (var runningChild in runningChildren)
            {
                _childProcesses.Add(runningChild);
            }

            foreach (var exitedProcess in exitedProcesses)
            {
                _log.Debug($"Process PID = {exitedProcess.Id} exited.");
                exitedProcess.Dispose();
            }
        }

        private void ExecuteJobs(IEnumerable<Job> jobs)
        {
            var jobStore = new JobStore();
            foreach (var job in jobs)
            {
                try
                {
                    job.NeedsProcessing = false;
                    job.MachineName = Environment.MachineName;
                    jobStore.Update(job);

                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = JobEngineControllerConfig.Instance.JobEngine.Path,
                        Arguments = job.JobId.ToString()
                    };
                    Process process = Process.Start(processStartInfo);
                    _childProcesses.Add(process);


                    _log.Info($"Start new JobEngine process PID = {process?.Id} for jobId = {job.JobId}.");

                }
                catch (Exception ex)
                {
                    _log.Error($"An error occurred when starting a new job id = {job.JobId} - {ex.Message}", ex);
                    job.State = (int)WorkflowStepState.Failed;
                    jobStore.Update(job);
                }
            }
        }

        #endregion


        #region Save Process class wrappers

        /// <summary>
        /// Wraps the <see cref="Process.HasExited" /> property and handles potential exceptions.
        /// </summary>
        private bool ProcHasExited(Process proc)
        {
            try
            {
                return proc.HasExited;
            }
            catch (Exception e)
            {
                _log.Warn(e.Message,e);
                return true;
            }
        }

        /// <summary>
        /// Wraps the <see cref="Process.Id" /> property and handles potential exceptions.
        /// </summary>
        private int ProcId(Process proc)
        {
            try
            {
                return proc.Id;
            }
            catch (Exception e)
            {
                _log.Warn(e.Message,e);
                return -1;
            }
        }

        /// <summary>
        /// Wraps the <see cref="Process.Id" /> property and handles potential exceptions.
        /// </summary>
        private DateTime ProcStartTime(Process proc)
        {
            try
            {
                return proc.StartTime;
            }
            catch (Exception e)
            {
                _log.Warn(e.Message,e);
                return DateTime.MinValue;
            }
        }

        #endregion
    }
}
