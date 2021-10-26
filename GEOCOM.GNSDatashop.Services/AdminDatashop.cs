using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.DBStore.Archive;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSDatashop.Services
{
    [ServiceBehavior(Namespace = "http://datashop.geocom.ch")]
    public class AdminDatashop : IAdminDatashop
    {
        private IMsg _log;

        public AdminDatashop()
        {
            InitLogger();
        }

        public int GetJobCount(bool showArchived, bool showNotArchived, string whereClause)
        {
            try
            {
                _log.Debug("getting Number of Jobs in DB");
                JobDetailsStore jobStore = new JobDetailsStore();
                return jobStore.GetCount(showArchived, showNotArchived, whereClause);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get amount of Jobs.", exp);
                throw;
            }
        }

        public JobLog[] GetLogsForJob(long jobId)
        {
            try
            {
                _log.DebugFormat("Getting logs for Job {0}", jobId);
                JobLogStore jobLogStore = new JobLogStore();
                var list = jobLogStore.GetLogsForJob(jobId);
                return list;
            }
            catch (Exception exp)
            {
                _log.Error("Could not get Logs for Job" + jobId, exp);
                throw;
            }
        }

        public int GetUserCount(bool showBusinessUser, bool showTempUser)
        {
            try
            {
                _log.Debug("Get amount of Users in DB.");
                UserStore userStore = new UserStore();
                return userStore.GetCount(showBusinessUser, showTempUser);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get amount of users in DB.", exp);
                throw;
            }
        }

        public int GetUserSearchCount(string adress, string firstname, string lastname, string street, string streetnr, string citycode, string city, string company, string email, string tel, string fax, string status, string roles, long? id, bool showBusinessUser, bool showTempUser)
        {
            try
            {
                _log.Debug("Getting count of UserSearchResults.");
                UserStore userStore = new UserStore();
                return userStore.GetSearchCount(adress, firstname, lastname, street, streetnr, citycode, city, company, email, tel, fax, status, roles, id, showBusinessUser, showTempUser);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get UserSearchCount.", exp);
                throw;
            }
        }

        public int GetJobSearchCount(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string whereClause)
        {
            try
            {
                _log.Debug("Getting JobSearchCount.");
                JobDetailsStore jobStore = new JobDetailsStore();
                return jobStore.GetSearchCount(
                                               jobId,
                                               createDateOld,
                                               createDateNew,
                                               stateDateOld,
                                               stateDateNew,
                                               reason,
                                               status,
                                               free1,
                                               free2,
                                               showArchived,
                                               showNotArchived,
                                               whereClause);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get JobSearchCount.", exp);
                throw;
            }
        }

        public bool ArchiveJob(long jobId)
        {
            _log.InfoFormat("Archiving for job {0} started.", jobId);

            try
            {
                var archiveJob = new JobArchiver();
                archiveJob.Archive(jobId);
                return true;
            }
            catch (Exception exp)
            {
                _log.Error("Could not archive Job " + jobId, exp);
                throw;
            }
        }

        /// <summary>
        /// Dis Method allows archiving of multiple jobs.
        /// </summary>
        /// <param name="time">
        /// A DateTime, every job older than this time will be archived 
        /// </param>
        /// <returns>
        /// For every job there is a String with the results, the last string is a summery 
        /// </returns>
        public string[] ArchiveJobsByTime(DateTime time)
        {
            _log.InfoFormat("Started archiving for all Jobs older than {0}.", time);
            JobDetails[] jobs;
            try
            {
                JobDetailsStore jobStore = new JobDetailsStore();
                jobs = jobStore.SearchJob(
                                          null,
                                          string.Empty,
                                          time.ToString(),
                                          string.Empty,
                                          string.Empty,
                                          string.Empty,
                                          string.Empty,
                                          string.Empty,
                                          string.Empty,
                                          false,
                                          true,
                                          "CreateDate",
                                          true,
                                          0,
                                          int.MaxValue);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get all Jobs for archiving.", exp);
                throw;
            }
            string[] results = new string[jobs.Length + 1];
            int failed = 0, sucsess = 0;
            for (int i = 0; i < jobs.Length; i++)
            {
                try
                {
                    if (ArchiveJob(jobs[i].JobId))
                    {
                        sucsess++;
                        results[i] = "Job " + jobs[i].JobId + " was succsesfully archived.";
                        _log.InfoFormat("Archiving for job {0} was succsesfull", jobs[i].JobId);
                    }
                    else
                    {
                        failed++;
                        results[i] = "Job " + jobs[i].JobId + " could not be archived.";
                        _log.ErrorFormat("Job {0} could not be archived", jobs[i].JobId);
                    }
                }
                catch (Exception exp)
                {
                    failed++;
                    results[i] = "Job " + jobs[i].JobId + " could not be archived. This error message may help you: " + exp.Message;
                }
            }
            results[results.Length - 1] = "Jobs:" + jobs.Length + "   Succsesfully:" + sucsess + "   Failed:" +
                                          failed;
            _log.InfoFormat("Summery of Archiving:  Jobs: " + jobs.Length + " succsesfull: " + sucsess + " failed: " + failed);
            return results;
        }

        private void InitLogger()
        {
            try
            {
                if (_log == null)
                {
                    DatashopLogInitializer.Initialize();

                    // Log4net generic logger interface
                    _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }
    }
}
