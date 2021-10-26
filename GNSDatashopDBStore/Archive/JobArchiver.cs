using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.DBStore.Config;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.DBStore.Archive
{
    public class JobArchiver
    {
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        // used for serialization so it must be public
        public Job Job;
        public User User;
        public JobLog[] Logs;
        public Reason Reason;
        private JobStore _jobStore;

        public void Archive(DateTime createdBefore)
        {
            JobDetailsStore jobStore = new JobDetailsStore();

            JobDetails[] jobsDetails = jobStore.SearchJob(
                null,
                string.Empty,
                createdBefore.ToString(),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                false,
                true,
                "JobId",
                false,
                0,
                int.MaxValue,
                null);

            _log.Info($"Archiving {jobsDetails.Length} jobs.");
            foreach (var jobsDetail in jobsDetails)
            {
                _log.Debug($"Starting jobId = {jobsDetail.JobId}.");
                Archive(jobsDetail.JobId);
                _log.Debug($"Finished jobId = {jobsDetail.JobId}.");
            }
            _log.Info($"Finished archiving {jobsDetails.Length} jobs...");
        }

        public void Archive(long[] jobIds)
        {
          
            _log.Info($"Archiving {jobIds.Length} jobs, IDs = {string.Join(", ", jobIds)}.");
            foreach (var jobId in jobIds)
            {
                Archive(jobId);
                _log.Debug($"Finished archiving job Id = {jobId}.");
            }
            _log.Info($"Finished archiving {jobIds.Length} jobs...");
        }

        public void Archive(long jobId)
        {
            try
            {
                GetJob(jobId);
                GetReason();
                GetUser();
                GetLogs();
                DoArchive();
            }
            catch (Exception)
            {
                _log.ErrorFormat("An error occoured during archiving the job {0}.", jobId);
                throw;
            }
        }

        private void GetReason()
        {
            var reasonManager = new ReasonsStore();
            Reason = reasonManager.Get(Job.ReasonId);
        }

        private void GetJob(long jobId)
        {
            _jobStore = new JobStore();
            Job = _jobStore.GetById(jobId);
        }

        private void GetUser()
        {
            var userManager = new UserStore();
            User = userManager.GetById(Job.UserId);
        }

        private void GetLogs()
        {
            var logManager = new JobLogStore();
            Logs = logManager.GetLogsForJob(Job.JobId);
        }

        private void DoArchive()
        {
            try
            {
                EnsureConfigLoaded();

                string sourceDirPath = Path.GetDirectoryName(Job.JobOutput);
                string sourceFileName = Path.GetFileName(Job.JobOutput);
                string destPath = GnsDatashopCommonConfig.Instance.Directories.ArchiveDirectory + "\\ARCHIVED_Job_" +
                                  Job.JobId + "\\" + sourceFileName;
                string destDirPath = Path.GetDirectoryName(destPath);

                if (Directory.Exists(destDirPath))
                {
                    var message = string.Format("Archive directory '{0}' already exists.", destDirPath);
                    throw new IOException(message);
                }

                if (File.Exists(Job.JobOutput))
                {
                    Job.IsArchived = true;
                    Job.Step = (int) PlotJobSteps.Archive;
                    Directory.CreateDirectory(destDirPath);
                    File.Move(Job.JobOutput, destPath);
                    Directory.Delete(sourceDirPath, true);

                    _jobStore.Update(Job);
                    _log.InfoFormat("Moved job directory to archive for jobid = {0}", Job.JobId);
                    var logManager = new JobLogStore();
                    logManager.Add(Job, $"Moved job directory to archive '{destDirPath}'.");
                    //Job.State = (int) WorkflowStepState.Finished;

                }
                else
                {
                    _log.ErrorFormat("Archiving for job {0} failed. File {1} was not found", Job.JobId, Job.JobOutput,
                        destPath);
                    throw new IOException("Jobfolder was not found.");
                }

                XmlSerializer serializerObj = new XmlSerializer(typeof(JobArchiver));
                TextWriter writeFileStream = new StreamWriter(destDirPath + "/Info.xml");
                serializerObj.Serialize(writeFileStream, this);
                writeFileStream.Close();

                _log.DebugFormat("Info.xml for Job {0} was succsesfully written", Job.JobId);
                _log.DebugFormat("Job {0} has been archived", Job.JobId);
            }
            catch (Exception e)
            {
                _log.Error($"Error during archiving of a job id = {Job.JobId}", e);
            }
        }

        private void EnsureConfigLoaded()
        {
            DbStoreConfig config = DbStoreConfig.Instance;
            if (!config.IsInitialized)
            {
                throw new Exception("The DbStoreConfig configuration could not initialized.");
            }
        }
    }
}