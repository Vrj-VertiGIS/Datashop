using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.ErrorHandling.Exceptions;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Common.Mail;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using GEOCOM.GNSDatashop.ServiceContracts;
using GEOCOM.GNSDatashop.Services.Config;
using GEOCOM.TDE.Enumerations;
using GEOCOM.TDE.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using OutputFormat = GEOCOM.GNSD.Common.Model.OutputFormat;

namespace GEOCOM.GNSDatashop.Services
{

    /// <summary>
    /// Offers functionality to work with Datashop DB
    /// </summary>
    [ServiceBehavior(Namespace = "http://datashop.geocom.ch")]
    public class JobManager : IJobManager
    {
        // log4net
        private IMsg _log;

        public JobManager()
        {
            InitLogger();
        }

        readonly JobDetailsStore _jobDetailsStore = new JobDetailsStore();

        /// <summary>
        /// This method first assosiates the given User with the given BizUser.
        /// Both are stored.
        /// an email is sent to the administrator.
        /// </summary>
        /// <param name="bizUser">
        /// The business bizUser to be created
        /// </param>
        /// <param name="user">
        /// The bizUser to be created
        /// </param>
        /// <returns>
        /// The ID of the User or -1 if something failed
        /// </returns>
        public long CreateBizUserAndSendAdminMail(BizUser bizUser, User user)
        {
            _log.Debug("creating a new bizUser and a bizUser");
            if (bizUser == null || user == null)
                return -1;

            try
            {
                BizUserStore bizUserStore = new BizUserStore();
                long userid = bizUserStore.Add(bizUser, user);
                if (userid > 0)
                {
                    _log.Debug("BizUser has been created");

                    // Create Dictionary for Mail                    
                    Dictionary<string, string> variables = CreateUserVariables(user);

                    // Send mail
                    try
                    {
                        MailSender mail = new MailSender();
                        mail.SendMail(
                                      "newBizUserAdmin",
                                      GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["newBizUserAdmin"].To,
                                      variables,
                                      true);
                    }
                    catch (Exception e)
                    {
                        _log.Error("Could not send email after creating new BizUser", e);
                    }
                    return userid;
                }
                _log.Warn("BizUser could not bee created");
                return userid;
            }
            catch (Exception ex)
            {
                _log.Error("CreateBizUser failed: " + ex.Message, ex);
                return -1;
            }
        }


        public BizUser[] GetAllBizUsers()
        {
            try
            {
                BizUserStore bizUserStore = new BizUserStore();
                IList<BizUser> all = bizUserStore.GetAll();
                BizUser[] result = new BizUser[all.Count];
                int i = 0;
                foreach (BizUser user in all)
                {
                    result[i++] = user;
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("GetAllBizUsers failed: " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a BizUser identified by his bizId
        /// if there are no results, null will be returned
        /// </summary>
        /// <param name="bizId">
        /// The BusinessUser Id
        /// </param>
        /// <returns>
        /// The BusinessUser corresponding to the Id
        /// </returns>
        public BizUser GetBizUserByBizId(long bizId)
        {
            if (bizId == 0) return null;
            try
            {
                _log.DebugFormat("Getting bizUser with bizId {0}", bizId);
                BizUserStore bizUserStore = new BizUserStore();
                return bizUserStore.GetByBizId(bizId);
            }
            catch (Exception ex)
            {
                _log.Error("GetBizUser failed: " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a BizUser identified by his pwdResetId
        /// if there are no results, null will be returned
        /// </summary>
        public BizUser GetBizUserByPasswordResetId(Guid pwdResetId)
        {
            try
            {
                _log.DebugFormat("Getting bizUser with pwdResetId {0}", pwdResetId);
                BizUserStore bizUserStore = new BizUserStore();
                return bizUserStore.GetByPasswordResetId(pwdResetId);
            }
            catch (Exception ex)
            {
                _log.Error("GetBizUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public User[] GetUserNotActivated()
        {
            _log.Debug("Getting not activated bizUsers");
            UserStore userStore = new UserStore();
            return userStore.GetNotActivated();
        }

        public User[] GetUserNotActivatedOrdered(string orderExpression, bool orderAscending)
        {
            _log.DebugFormat("Getting not activated bizUsers ordered. orderExpression={0} orderAscending={1}", orderExpression, orderAscending);
            UserStore userStore = new UserStore();
            return userStore.GetNotActivatedOrdered(orderExpression, orderAscending);
        }

        /// <summary>
        /// This method takes a userId (NOT a BizUserId) and activates this bizUser.
        /// A Mail will be sent to the bizUser.
        /// </summary>
        /// <param name="userId">
        /// A userId to be activated
        /// </param>
        /// <returns>
        /// True if successful, false otherwise
        /// </returns>
        public bool ActivateBizUserAndSendNotificationMail(long userId)
        {
            _log.DebugFormat("Trying to activate bizUser with ID {0}", userId);
            UserStore userStore = new UserStore();
            User user;
            try
            {
                user = userStore.GetById(userId);
            }
            catch (Exception)
            {
                _log.ErrorFormat("Could not acctivate User with ID {0}, because bizUser was not found.", userId);
                return false;
            }
            if (user == null)
            {
                _log.ErrorFormat("Could not acctivate User with ID {0}, because bizUser was not found.", userId);
                return false;
            }
            if (user.BizUser == null)
            {
                _log.ErrorFormat("Could not acctivate User with ID {0}, because no associated bizUser was found.", userId);
                return false;
            }

            user.BizUser.UserStatus = BizUserStatus.ACTIVATED;
            BizUserStore bizStore = new BizUserStore();
            if (bizStore.Update(user.BizUser))
            {
                ReassignJobs(user);

                // Create Dictionary for Mail
                Dictionary<string, string> variables = CreateUserVariables(user);

                // Send mail
                MailSender mail = new MailSender();
                mail.SendMail("bizUserActivated", user.Email, variables, true);
                _log.DebugFormat("User {0} was activated and a mail sent to {1}", userId, user.Email);
                return true;
            }
            _log.ErrorFormat("Could not activate User {0}", userId);
            return false;
        }

        /// <summary>
        /// Reassign jobs and surrogate jobs from all temp users with identical email and delete these temp users
        /// </summary>
        /// <param name="user">The user.</param>
        private void ReassignJobs(User user)
        {
            // find and delete temp users' jobs 
            var jobsForEmail = GetJobForEmail(user.Email);
            foreach (var jobDetail in jobsForEmail)
            {
                var skipThisUser = jobDetail.UserId == user.UserId;
                if (skipThisUser)
                    continue;

                var jobStore = new JobStore();
                var job = jobStore.GetById(jobDetail.JobId);
                job.UserId = user.UserId;
                if (job.SurrogateJob != null)
                    job.SurrogateJob.UserId = user.UserId;

                jobStore.Update(job);
            }

            // find and delete temp users
            UserStore userStore = new UserStore();
            var usersByEmail = GetUserByEmail(user.Email);
            foreach (var userByEmail in usersByEmail)
            {
                var skipThisUser = userByEmail.UserId == user.UserId;
                if (skipThisUser)
                    continue;

                userStore.Delete(userByEmail.UserId);
            }
        }

        public bool UpdateBizUser(BizUser bizUser)
        {
            try
            {
                BizUserStore bizUserStore = new BizUserStore();

                if (!bizUserStore.Update(bizUser))
                {
                    throw new Exception("The bizUser could not be updated");
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Update BizUser {0} failed: ", bizUser.BizUserId) + ex.Message, ex);
                throw;
            }
        }

        public bool DeleteBizUser(BizUser bizUser, bool deleteAllRelatedData, bool deleteReleatedUser)
        {
            if (bizUser == null) return false;

            try
            {
                BizUserStore bizUserStore = new BizUserStore();
                return bizUserStore.Delete(bizUser, deleteAllRelatedData, deleteReleatedUser);
            }
            catch (Exception ex)
            {
                _log.Error("DeleteBizUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public bool DeleteBizUserByUserId(long userId, bool deleteAllRelatedData, bool deleteReleatedUser)
        {
            if (userId <= 0) return false;

            try
            {
                UserStore userStore = new UserStore();
                User user = userStore.GetById(userId);
                if (user == null)
                {
                    _log.ErrorFormat("DeletBizUserByID failed. No User with userId {0} found.", userId);
                    return false;
                }
                BizUserStore bizUserStore = new BizUserStore();
                return bizUserStore.Delete(user.BizUser, deleteAllRelatedData, deleteReleatedUser);
            }
            catch (Exception ex)
            {
                _log.Error("DeleteBizUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public bool SendResetPasswordMail(User user)
        {
            try
            {
                if (user == null || user.BizUser == null)
                    return false;

                var passwordResetMins = GnsDatashopCommonConfig.Instance.PasswordReset.ValidityMinutes;
                var bizUserStore = new BizUserStore();
                var bizUser = bizUserStore.GetByBizId(user.BizUserId.Value);
                user.BizUser.PasswordResetId = bizUser.PasswordResetId = Guid.NewGuid();
                user.BizUser.PasswordResetIdValidity = bizUser.PasswordResetIdValidity = DateTime.Now.AddMinutes(passwordResetMins);
                bizUserStore.Update(bizUser);

                var variables = CreateUserVariables(user);
                variables.Add("user_reset_password_mins", passwordResetMins.ToString());
                variables.Add("user_reset_password_until_date", user.BizUser.PasswordResetIdValidity.Value.ToString("D"));
                variables.Add("user_reset_password_until_time", user.BizUser.PasswordResetIdValidity.Value.ToString("T"));

                // Send mail
                MailSender mail = new MailSender();
                mail.SendMail("resetPassword", user.Email, variables, true);
                _log.InfoFormat("User {0} had a reset password mail sent to {1}", user.UserId, user.Email);
                return true;
            }
            catch (Exception exp)
            {
                if (user != null)
                {
                    this._log.Error("Could not sent reset password to bizUser " + user.UserId, exp);
                }
                return false;
            }
        }

        public long CreateTempUser(User user)
        {

            try
            {
                UserStore userStore = new UserStore();

                var bizUserExistsWithEmail = userStore.GetByEmail(user.Email).Any(u => u.BizUserId.HasValue);

                if (bizUserExistsWithEmail)
                    throw new FaultException<ServiceFault>(new ServiceFault { LanguageCode = 2317 }, new FaultReason("User already exists"));

                return CreateUser(user);
            }
            catch (Exception ex)
            {
                var message = string.Format("CreateTempUser failed with message: {0} for user {1}.", ex.Message, user.Email);
                _log.Error(message, ex);
                throw new FaultException<ServiceFault>(new ServiceFault { LanguageCode = 2319 }, new FaultReason(ex.Message));
            }
        }

        public long CreateUser(User user)
        {
            try
            {
                UserStore userStore = new UserStore();
                long newUserId = userStore.Add(user);

                _log.Debug("User has been created");

                if (newUserId < 0)
                {
                    throw new Exception("The new bizUser could not be created");
                }
                return newUserId;
            }
            catch (Exception ex)
            {
                _log.Error("CreateUser failed: " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Delets a bizUser, only if he has no jobs
        /// </summary>
        /// <param name="userId">The userId of the bizUser to be deleted</param>
        public void DeleteUser(long userId)
        {
            try
            {
                _log.InfoFormat("User with ID {0} will be deleted.", userId);
                UserStore userStore = new UserStore();
                if (!userStore.Delete(userId))
                {
                    throw new Exception("The new bizUser could not be deleted");
                }
            }
            catch (Exception ex)
            {
                _log.Error("DeleteUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public User GetUser(long userId)
        {
            try
            {
                _log.DebugFormat("Getting User with ID {0}", userId);
                UserStore userStore = new UserStore();
                User user = userStore.GetById(userId);
                return user;
            }
            catch (Exception ex)
            {
                _log.Error("GetUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public User[] GetUserByEmail(string email)
        {
            try
            {
                _log.DebugFormat("Getting all Users for emial {0}", email);
                UserStore userStore = new UserStore();
                return userStore.GetByEmail(email);
            }
            catch (Exception ex)
            {
                _log.Error("GetUserByEmail failed: " + ex.Message, ex);
                throw;
            }
        }

        public User[] GetAllUsers()
        {
            try
            {
                UserStore userStore = new UserStore();
                return userStore.GetAll();
            }
            catch (Exception exp)
            {
                _log.Error("Could not get all Users.", exp);
                throw;
            }
        }

        public User[] GetAllUsersPaged(int skip, int take)
        {
            try
            {
                UserStore userStore = new UserStore();
                return userStore.GetAllUsersPages(skip, take);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get all Users.", exp);
                throw;
            }
        }

        /// <summary>
        /// Gets all Users in the Database ordered by Property defined in String Orderexpression
        /// </summary>
        /// <param name="orderExpression">
        /// The property to sort
        /// </param>
        /// <param name="ascending">
        /// True = asscending, false = descending
        /// </param>
        /// <param name="showBusinessUser">
        /// Indicates if BusinessUsers should be returner
        /// </param>
        /// <param name="showTempUser">
        /// Indicates if TempUsers should be returned
        /// </param>
        /// <returns>
        /// All User in DB as an Array, sorted.
        /// </returns>
        public User[] GetAllUsersSorted(string orderExpression, bool ascending, bool showBusinessUser, bool showTempUser)
        {
            try
            {
                _log.Debug("Getting all Users sorted.");
                UserStore userStore = new UserStore();
                return userStore.GetAllOrdered(orderExpression, ascending, showBusinessUser, showTempUser);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get All Users sorted.", exp);
                throw;
            }
        }

        public User[] GetSomeUsersSorted(string orderExpression, bool ascending, int index, int quantity, bool showBusinessUser, bool showTempUser)
        {
            try
            {
                _log.DebugFormat("Getting Users from {0}. to {1} sorted by {2}", index, index + quantity, orderExpression);
                UserStore userStore = new UserStore();
                return userStore.GetSomeOrdered(orderExpression, ascending, index, quantity, showBusinessUser, showTempUser);
            }
            catch (Exception exp)
            {
                _log.Error("Could not get SomeUserSorted.", exp);
                throw;
            }
        }

        /// <summary>
        /// Search in Users and get a sorted result with paging
        /// </summary>
        /// <param name="salutation">
        /// The salutation to be searched
        /// </param>
        /// <param name="firstname">
        /// The firstname to be searched
        /// </param>
        /// <param name="lastname">
        /// The lastname to be searched
        /// </param>
        /// <param name="street">
        /// The street to be searched
        /// </param>
        /// <param name="streetnr">
        /// The streetnr to be searched
        /// </param>
        /// <param name="citycode">
        /// The citycode to be searched
        /// </param>
        /// <param name="city">
        /// The city to be searched
        /// </param>
        /// <param name="company">
        /// The company to be searched
        /// </param>
        /// <param name="email">
        /// The email to be searched
        /// </param>
        /// <param name="tel">
        /// The tel to be searched
        /// </param>
        /// <param name="fax">
        /// The fax to be searched
        /// </param>
        /// <param name="status">
        /// The status to be searched
        /// </param>
        /// <param name="roles">
        /// The roles to be searched
        /// </param>
        /// <param name="userId">
        /// If userId is not empty, maximal 1 result will be returned
        /// </param>
        /// <param name="showBusinessUser">
        /// The showBusinessUser to be searched
        /// </param>
        /// <param name="showTempUser">
        /// The showTempUser to be searched
        /// </param>
        /// <param name="orderExpression">
        /// Sort Expression
        /// </param>
        /// <param name="ascending">
        /// True = asscending, false = descending
        /// </param>
        /// <param name="index">
        /// The start Index
        /// </param>
        /// <param name="quantity">
        /// Amount of results
        /// </param>
        /// <returns>
        /// This Method will return all Users that have matching substrings in all not empty fields 
        /// </returns>
        public User[] SearchUsers(string salutation, string firstname, string lastname, string street, string streetnr, string citycode, string city, string company, string email, string tel, string fax, string status, string roles, long? userId, bool showBusinessUser, bool showTempUser, string orderExpression, bool ascending, int index, int quantity)
        {
            try
            {
                _log.Debug("Searching for Users");
                UserStore userStore = new UserStore();
                return userStore.SearchUsers(salutation, firstname, lastname, street, streetnr, citycode, city, company, email, tel, fax, status, roles, userId, showBusinessUser, showTempUser, orderExpression, ascending, index, quantity);
            }
            catch (Exception exp)
            {
                _log.Error("Searching for users failed.", exp);
                throw;
            }
        }

        /// <summary>
        /// Updates a User in the Database
        /// </summary>
        /// <param name="user">The bizUser to be updated</param>
        public void UpdateUser(User user)
        {
            try
            {
                _log.DebugFormat("Updating bizUser {0}", user.UserId);
                UserStore userStore = new UserStore();
                if (!userStore.Update(user))//WTF
                {
                    throw new Exception("The new bizUser could not be updated");
                }

                if (user.BizUser != null)
                {
                    ReassignJobs(user);
                }
            }
            catch (Exception ex)
            {
                _log.Error("UpdateUser failed: " + ex.Message, ex);
                throw;
            }
        }

        public long CreateJob(Job job)
        {
            try
            {
                _log.Debug("Entered CreateJob");

                var isPdfJob = job.ProcessorClassId == WorkflowDefinitions.PlotWorkflowClassId;
                if (isPdfJob)
                {
                    var exportModel = LayoutExportModel.FromXml(job.Definition);
                    CheckPlotIntensity(job, exportModel);
                    CheckTemplatesLimits(job, exportModel);
                }

                job.CreateDate = job.LastStateChangeDate = DateTime.Now;

                JobStore jobStore = new JobStore();
                long newJobId = jobStore.Add(job);

                if (newJobId < 0)
                {
                    throw new Exception("The new job could not be created");
                }

                _log.Debug("Job has been created");
                return job.JobId;
            }
            catch (Exception ex)
            {
                _log.Error("CreateJob failed: " + ex.Message, ex);
                throw;
            }
        }

        public long CloneJob(long jobId)
        {
            try
            {
                var job = GetJobById(jobId);

                job.JobId = 0;
                job.State = (int)WorkflowStepState.Idle;
                job.DownloadCount = 0;
                job.ProcessId = null;
                job.MachineName = null;
                job.NeedsProcessing = true;
                job.JobOutput = null;
                job.IsArchived = false;

                if (job.SurrogateJob != null)
                    job.SurrogateJob.Id = 0;

                var cloneId = CreateJob(job);

                return cloneId;
            }
            catch (TemplateLimitReachedException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"CloneJob failed for jobId {jobId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the count of the plots created in last minute is below a certain limit. This prevents overloading of the system.
        /// </summary>
        /// <param name="job">The job information.</param>
        /// <param name="exportModel">Export model information</param>
        private static void CheckPlotIntensity(Job job, LayoutExportModel exportModel)
        {
            var jobDetailsStore = new JobDetailsStore();
            var userStore = new UserStore();
            var byId = userStore.GetById(job.UserId);
            var plotCountInLastMinute = jobDetailsStore.GetPlotCountInTimePeriodForUser(byId, DateTime.Now.AddMinutes(-1)) +
                                        exportModel.Perimeters.Length;
            var configuredMaxJobsRate = ServicesConfig.Instance.RestrictionInfo.MaxPlotsRate;
            var tooManyPlotsInAMinute = configuredMaxJobsRate < plotCountInLastMinute;
            if (tooManyPlotsInAMinute)
            {
                var message = string.Format("Too many plots ({0}) within a minute. The limit is {1} plots per minute.", plotCountInLastMinute,
                    configuredMaxJobsRate);
                var tooManyPlotsFault = new TooManyPlotsFault()
                {
                    ConfiguredMaxPlotsRate = configuredMaxJobsRate,
                    PlotCountInLastMinute = plotCountInLastMinute

                };
                throw new FaultException<TooManyPlotsFault>(tooManyPlotsFault, message);
            }
        }

        /// <summary>
        /// Re-checks the plot templates limits on the backend. If the limit is reached, an exception is thrown.
        /// </summary>
        /// <param name="job">The job information</param>
        /// <param name="exportModel"></param>
        private void CheckTemplatesLimits(Job job, LayoutExportModel exportModel)
        {
            var contextualUserId = job.SurrogateJob != null ? job.SurrogateJob.SurrogateUserId : job.UserId;

            //var templatesForUser = GetTemplatesForUser(job.UserId);
            var templatesForUser = GetTemplatesForUser(contextualUserId);

            var templateCounts = exportModel
                .Perimeters
                .GroupBy(perimeter => perimeter.MapExtent.PlotTemplate)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Count());
            foreach (var templateCount in templateCounts)
            {
                var templateName = templateCount.Key;
                var count = templateCount.Value;

                var template = templatesForUser.FirstOrDefault(def => def.Template.Equals(templateName, StringComparison.InvariantCultureIgnoreCase));
                var templateLimitReached = template == null || template.RemainingLimit < count;
                if (templateLimitReached)
                {
                    throw new TemplateLimitReachedException("The template limit has been reached.");
                }
            }
        }


        /// <summary>
        /// Checks whether creation of a new job is allowed for the specified user.
        /// </summary>
        /// <param name="userId">The userId of the user to be checked</param>
        /// <param name="jobCountLimit">The maximum number of jobs allwed in the time period</param>
        /// <param name="timePeriod">The time period in days</param>
        /// <returns>True if a new job may be created, False otherwise</returns>
        public bool IsJobCreationAllowed(long userId, int jobCountLimit, int timePeriod)
        {
            //NOTE: this is truly terrible. the application service needs to decide which user type has what job limits!
            //NOTE: instead of which you can pass an arbitrary job limit in and make as many as you want if you know this.
            try
            {
                _log.Debug("Entered IsJobCreationAllowed");

                if (userId < 0 || jobCountLimit < 0 || timePeriod < 0)
                    return false;

                var jobDetailsStore = new JobDetailsStore();

                var fromDate = DateTime.Now.AddDays(-timePeriod);

                var user = GetUser(userId);

                var jobCount = jobDetailsStore.GetPlotCountInTimePeriodForUser(user, fromDate);

                return jobCount < jobCountLimit;
            }
            catch (Exception ex)
            {
                _log.Error("IsJobCreationAllowed failed: " + ex.Message, ex);
                throw;
            }
        }

        public void UpdateJob(Job job)
        {
            try
            {
                JobStore jobStore = new JobStore();

                _log.Debug("UpdateJob called");

                if (!jobStore.Update(job))
                {
                    throw new Exception("The job could not be updated");
                }
            }
            catch (Exception ex)
            {
                _log.Error("UpdateJob failed: " + ex.Message, ex);
                throw;
            }
        }

        public void RestartJobFromStep(long jobId, int workflowStepIndex)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            workflow.RestartFromStep(workflowStepIndex);
        }

        public int[] GetAllRestartableStepIds(long jobId)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            return workflow.GetAllRestartableStepIds();
        }

        public WorkflowStepIdName[] GetAllStepIdNames(long jobId)
        {
            try
            {
                IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
                return workflow.GetAllStepIdNames();
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
                throw;
            }
        }


        public WorkflowStepIdName[] GetAllRestartableStepIdNames(long jobId)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            return workflow.GetAllRestartableStepIdNames();
        }

        public WorkflowStepIdName GetNextStepIdName(long jobId)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            return workflow.GetNextStepIdName();
        }

        public string GetWorkflowStepNameById(long jobId, int workflowStepId)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            return workflow.GetWorkflowStepNameById(workflowStepId);
        }

        public void ActivateJob(long jobId)
        {
            IWorkflow workflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, false);
            workflow.Activate();
        }

        /// <summary>
        /// Adds a new log in the Database. You can specify JobId, Status, SubStatus and Message.
        /// This Methode is mostly used to allow userdefined logentries for a job
        /// </summary>
        /// <param name="jobId">The jobId of the changed job</param>
        /// <param name="message">The message for the log entry</param>
        public void AddJobLogByJobId(long jobId, string message)
        {
            try
            {
                JobStore jobStore = new JobStore();
                Job job = jobStore.GetById(jobId);
                JobLogStore jobLogStoreStore = new JobLogStore();
                jobLogStoreStore.Add(job, message);
            }
            catch (Exception ex)
            {
                _log.Error("Logging in DataBase (GNSD_JOBS_LOG) failed. " + ex.Message, ex);
                throw;
            }
        }

        public void DeleteJob(long jobId)
        {
            try
            {
                _log.InfoFormat("Deleteing job {0}", jobId);
                JobStore jobStore = new JobStore();
                if (!jobStore.Delete(jobId))
                {
                    throw new Exception("The job could not be deleted");
                }
            }
            catch (Exception ex)
            {
                _log.Error("DeleteJob failed: " + ex.Message, ex);
                throw;
            }
        }

        public Job GetJobByGuid(string jobGuid)
        {
            try
            {
                _log.DebugFormat("Getting job for guid={0}.", jobGuid);
                JobGuidStore jobGuidStore = new JobGuidStore();
                JobGuid jobWithGuid = jobGuidStore.GetByGuid(jobGuid);

                if (jobGuid == null) throw new Exception(string.Format("No job with GUID {0} found", jobGuid));//WTF

                JobStore jobStore = new JobStore();
                return jobStore.GetById(jobWithGuid.JobId);
            }
            catch (Exception ex)
            {
                _log.Error("GetJobByGuid failed: " + ex.Message, ex);
                throw;
            }
        }

        public JobDetails GetJobDetailsById(long jobId)
        {
            try
            {
                _log.DebugFormat("Getting gridJob for ID {0}.", jobId);
                JobDetailsStore gridstore = new JobDetailsStore();
                return gridstore.GetByID(jobId);
            }
            catch (Exception ex)
            {
                _log.Error("GetJobByGuid failed: " + ex.Message, ex);
                throw;
            }
        }

        public Job GetJobById(long jobId)
        {
            try
            {
                _log.DebugFormat("Getting job {0}", jobId);
                JobStore jobStore = new JobStore();
                return jobStore.GetById(jobId);
            }
            catch (Exception ex)
            {
                _log.Error("GetJobByGuid failed: " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets all GNSD_gridjobs for a specific bizUser
        /// </summary>
        /// <param name="userId">
        /// The userid
        /// </param>
        /// <returns>
        /// All jobs associated with this bizUser
        /// </returns>
        public JobDetails[] GetJobForUser(long userId)
        {
            try
            {
                _log.DebugFormat("Getting jobs for User {0}", userId);
                JobDetailsStore jobStore = new JobDetailsStore();
                return jobStore.GetByUserID(userId);
            }
            catch (Exception ex)
            {
                _log.Error("GetJobForUser " + userId + " failed: " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns all Jobs that are associated with this email-adress
        /// </summary>
        /// <param name="email">The email address to be searched</param>
        /// <returns>All jobs for the specified email address</returns>
        public Job[] GetJobForEmail(string email)
        {
            try
            {
                _log.DebugFormat("Getting jobs for email {0}", email);
                JobDetailsStore jobStore = new JobDetailsStore();
                var a = jobStore.GetByEmail(email);
                return a;
            }
            catch (Exception ex)
            {
                _log.Error("GetJobForEmail " + email + " failed: " + ex.Message, ex);
                throw;
            }
        }

        public Job[] GetAllJobs()
        {
            try
            {
                _log.Debug("Getting all jobs.");
                JobStore jobStore = new JobStore();
                return jobStore.GetAll();
            }
            catch (Exception ex)
            {
                _log.Error("GetAllJob failed: " + ex.Message, ex);
                throw;
            }
        }

        public Job[] GetAllJobsSorted(string orderExpression, bool ascending)
        {
            try
            {
                _log.DebugFormat("Getting all jobs sorted by {0}", orderExpression);
                JobStore jobStore = new JobStore();
                return jobStore.GetAllOrdered(orderExpression, ascending);
            }
            catch (Exception ex)
            {
                _log.Error("GetAllJobsSorted failed: " + ex.Message, ex);
                throw;
            }
        }

        public JobDetails[] GetJobDetailsSorted(bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int startIndex, int quantity, string whereClause)
        {
            try
            {
                _log.DebugFormat("Getting from {0} to {1}. job sorted by {2}", startIndex, startIndex + quantity, orderExpression);

                JobDetailsStore jobStore = new JobDetailsStore();

                JobDetails[] jobDetails = jobStore.GetSomeSorted(showArchived, showNotArchived, orderExpression, ascending, startIndex, quantity, whereClause);

                if (jobDetails != null)
                {
                    _log.DebugFormat("Total jobs found: {0}", jobDetails.Count());
                }

                return jobDetails;
            }
            catch (Exception ex)
            {
                _log.Error("GetJobDetailsSorted failed: " + ex.Message, ex);
                throw;
            }
        }

        public JobDetails[] SearchJobs(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause)
        {
            try
            {
                _log.DebugFormat("Searching for jobs");
                JobDetailsStore jobStore = new JobDetailsStore();
                var jobs = jobStore.SearchJob(
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
                                              orderExpression,
                                              ascending,
                                              index,
                                              quantity,
                                              whereClause);
                return jobs;
            }
            catch (Exception exp)
            {
                _log.Error("Searching for Jobs failed.", exp);
                throw;
            }
        }

        public JobDetails[] SearchJobsTyped(long? jobId, DateTime? createDateOld, DateTime? createDateNew, DateTime? stateDateOld, DateTime? stateDateNew, string reason, string status, string free1, string free2, bool showArchived, bool showNotArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause)
        {
            try
            {
                _log.DebugFormat("Searching for jobs");
                JobDetailsStore jobStore = new JobDetailsStore();
                var jobs = jobStore.SearchJob(
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
                                              orderExpression,
                                              ascending,
                                              index,
                                              quantity,
                                              whereClause);
                return jobs;
            }
            catch (Exception exp)
            {
                _log.Error("Searching for Jobs failed.", exp);
                throw;
            }
        }

        public Reason[] GetReasons()
        {
            try
            {
                _log.Debug("Requesting Reasons.");
                ReasonsStore reasonsStore = new ReasonsStore();
                ICollection<Reason> reasons = reasonsStore.Load();

                Reason[] reasonArray = new Reason[reasons.Count];
                reasons.CopyTo(reasonArray, 0);

                return reasonArray;
            }
            catch (Exception ex)
            {
                _log.Error("GetReasons failed: " + ex.Message, ex);
                return null;
            }
        }

        public KeyValuesPair[] GetScales()
        {
            try
            {
                _log.Debug("GetScales");
                return ServicesConfig.Instance.RequestScales;
            }
            catch (Exception ex)
            {
                _log.Error("GetScales failed: " + ex.Message, ex);
                return null;
            }
        }

        public Plotdefinition[] GetAllTemplates()
        {
            try
            {
                _log.Debug("GetTemplates");
                PlotDefinitionStore definitionStore = new PlotDefinitionStore();
                ICollection<Plotdefinition> definitions = definitionStore.LoadForMedium(null);
                return definitions.ToArray();
            }
            catch (Exception ex)
            {
                _log.Error("GetTemplates failed: " + ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Determines the remaining limit for a template and a user
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public PlotFormatDef[] GetTemplatesForUser(long userId)
        {
            try
            {
                var user = GetUser(userId);

                var userRoles = GetRolesForUser(user);
                var globalRemaining = GetGlobalRemainingPlotsForUser(user, userRoles);

                PlotDefinitionStore definitionStore = new PlotDefinitionStore();
                ICollection<Plotdefinition> definitions = definitionStore.LoadForMedium(null);

                var formatDefs = new Dictionary<string, PlotFormatDef>();
                foreach (Plotdefinition definition in definitions)
                {
                    if (string.IsNullOrEmpty(definition.Roles) || string.IsNullOrEmpty(definition.Limits) ||
                        string.IsNullOrEmpty(definition.LimitsTimePeriods) || string.IsNullOrEmpty(userRoles))
                        continue;

                    var roles = definition.Roles.Split(',');
                    var limits = definition.Limits.Split(',');

                    // replace SESSION by -1 and so ignore the history of the jobs
                    var timePeriods = definition.LimitsTimePeriods.ToUpperInvariant().Replace("SESSION", "-1").Split(',');
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (!userRoles.ToLower().Contains(roles[i].ToLower()))
                            continue;

                        int limit = Convert.ToInt32(i < limits.Length ? limits[i] : limits.FirstOrDefault());
                        int limitTimePeriod = Convert.ToInt32(i < timePeriods.Length ? timePeriods[i] : timePeriods.FirstOrDefault());

                        var fromDate = DateTime.Now.AddDays(-limitTimePeriod);
                        var templateRemaining = limit - _jobDetailsStore.GetPlotCountInTimePeriodForUserAndTemplate(user, definition.PlotdefinitionKey.Template, fromDate);
                        var remainingResult = Math.Min(templateRemaining, globalRemaining);
                        remainingResult = remainingResult < 0 ? 0 : remainingResult;

                        PlotFormatDef plotFormatDef;

                        var templateAlreadyAvailable = formatDefs.TryGetValue(definition.PlotdefinitionKey.Template, out plotFormatDef);
                        if (templateAlreadyAvailable)
                            plotFormatDef.RemainingLimit = Math.Max(remainingResult, plotFormatDef.RemainingLimit);
                        else
                            formatDefs[definition.PlotdefinitionKey.Template] =
                                new PlotFormatDef(
                                    definition.PlotdefinitionKey.Template,
                                    definition.PlotHeightCm,
                                    definition.PlotWidthCm,
                                    definition.Description,
                                    remainingResult);
                    }
                }

                var useDefaults = formatDefs.Count == 0;
                if (useDefaults)
                {
                    foreach (Plotdefinition definition in definitions)
                    {
                        formatDefs[definition.PlotdefinitionKey.Template] =
                                new PlotFormatDef(
                                    definition.PlotdefinitionKey.Template,
                                    definition.PlotHeightCm,
                                    definition.PlotWidthCm,
                                    definition.Description,
                                    globalRemaining);
                    }
                }

                return formatDefs.Values.ToArray();
            }
            catch (Exception ex)
            {
                _log.Error("GetTemplates failed: " + ex.Message, ex);
                return new PlotFormatDef[0];
            }
        }
        /// <summary>
        /// This limit is set in the config restrictions
        /// </summary>
        /// <param name="user">User for the limit</param>
        /// <param name="userRoles">Roles to test</param>
        /// <returns>Remaining limit</returns>
        private int GetGlobalRemainingPlotsForUser(User user, string userRoles)
        {
            try
            {
                int globalRemaining = 0;
                bool globalRestrictionApplied = false;
                foreach (var restriction in ServicesConfig.Instance.RestrictionInfo.Restrictions)
                {
                    var userInRole = userRoles.ToUpperInvariant().Contains(restriction.Role.ToUpperInvariant());
                    if (userInRole)
                    {
                        var dateTime = DateTime.Now.AddDays(-restriction.TimePeriod);
                        var remainingPlotsGlobal = restriction.Limit - _jobDetailsStore.GetPlotCountInTimePeriodForUser(user, dateTime);
                        globalRemaining = Math.Max(remainingPlotsGlobal, globalRemaining);
                        globalRestrictionApplied = true;
                    }
                }
                if (!globalRestrictionApplied)
                    globalRemaining = int.MaxValue;
                return globalRemaining;
            }
            catch (Exception)
            {
                return Int32.MaxValue;
            }
        }

        public bool UpdateTemplate(Plotdefinition plotTemplate)
        {
            _log.DebugFormat("Updating plottemplate {0}", plotTemplate.PlotdefinitionKey.Template);
            var plotStore = new PlotDefinitionStore();
            return plotStore.Update(plotTemplate);
        }

        public bool DeleteTemplate(Plotdefinition plotTemplate)
        {
            _log.DebugFormat("Deleting plottemplate {0}.", plotTemplate.PlotdefinitionKey.Template);
            var plotStore = new PlotDefinitionStore();
            return plotStore.Delete(plotTemplate);
        }

        public void CreateDbSchema()
        {
            try
            {
                _log.Debug("Creating Db Schema");
                NHibernateHelper.CreateSchema(@"C:\temp\schema.ddl");
            }
            catch (Exception exp)
            {
                _log.Error("Could not create Db Schema.", exp);
                throw;
            }
        }

        public DxfExportInfo GetDxfExportInfo(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name", "name parameter cannot be null or whitespace");

            try
            {
                var directory = GnsDatashopCommonConfig.Instance.Directories.DXFDirectory;

                var path = Path.Combine(directory, "dxfconfig.xml");

                var config = ConfigReader.GetConfiguration<DxfExportConfig>(path);

                return config.DxfExports.Where(dxf => dxf.Name == name)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("GetDxfExportInfo failed", ex);

                throw new Exception("GetDxfExportInfo failed", ex);
            }
        }

        public List<DxfExportInfo> GetDxfExportInfos()
        {
            try
            {
                var directory = GnsDatashopCommonConfig.Instance.Directories.DXFDirectory;

                var path = Path.Combine(directory, "dxfconfig.xml");

                var config = ConfigReader.GetConfiguration<DxfExportConfig>(path);

                return config.DxfExports;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("GetDxfExportInfo failed", ex);

                throw new Exception("GetDxfExportInfo failed", ex);
            }
        }

        public List<ProfileInfo> GetProfileInfo()
        {
            _log.Debug("Getting profile infos");
            try
            {
                List<ProfileInfo> result = new List<ProfileInfo>();

                var profileDirectory = GnsDatashopCommonConfig.Instance.Directories.TDEDirectory;

                var profiles = ProfileSerializer.ReadAllPdesFromDirectory(profileDirectory, true);
                foreach (Pde profile in profiles)
                {
                    try
                    {
                        foreach (var extractProfile in profile.ExtractProfiles)
                        {
                            // filter all not datashop profiles
                            if ((extractProfile.SourceType & SourceType.Datashop) == 0)
                            {
                                _log.InfoFormat("Profile {0} has not Sourcetype Datashop.", extractProfile.Name);
                                continue;
                            }
                            var targetFormat = new List<OutputFormat>();
                            List<string> postProcessList = new List<string>();

                            foreach (var format in extractProfile.OutputFormats)
                            {
                                //the cast between OutputFormat enums is a work around of an issue cased by double definition of the enum OutputFormat
                                //the first in TDE.Enumeration and second GEOCOM.GNSD.Common.Model
                                targetFormat.Add((OutputFormat)format.OutputFormat);
                            }

                            foreach (PostProcessProfile postProcessProfile in extractProfile.PostProcesses)
                            {
                                postProcessList.Add(postProcessProfile.Identification);
                            }

                            result.Add(new ProfileInfo
                            {
                                Name = extractProfile.Name,
                                Description = extractProfile.Description,
                                Guid = extractProfile.Guid,
                                TargetFormat = targetFormat,
                                PostProcessList = postProcessList
                            });
                            _log.DebugFormat("Added Profile {0} to collection.", extractProfile.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error("Could not add a profile. Maybe it is not a propper profile.", e);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                _log.Error("Could not load Profiles for ProfielInfo", e);
                throw;
            }
        }

        /// <summary>
        /// Authenticates a bizUser.
        /// The bizUser is authenticated depending on the userRole.
        /// If the bizUser is authenticated, and not already existing, a new bizUser will be created.
        /// The password has to be provided for the userRoles ADMIN and BUSINESS.
        /// For the other userRoles the password is ignored.
        /// </summary>
        /// <param name="userIdentification">The bizUser identification (e.g. SessionId for a public bizUser, email for a BUSINESS bizUser)</param>
        /// <param name="userRole">The bizUser role</param>
        /// <param name="password">The password only used for userRoles ADMIN and BUSINESS</param>
        /// <returns>The UserProfile of the bizUser</returns>
        public UserProfile AuthenticateUser(string userIdentification, UserRole userRole, string password)
        {
            try
            {
                _log.DebugFormat("AuthenticateUser userIdentification={0} userRole={1}", userIdentification, userRole);

                UserProfile userProfile = new UserProfile();

                UserStore userStore = new UserStore();
                User user = userStore.GetByUserIdentification(userIdentification, userRole);
                if (user == null)
                {
                    // Create a new bizUser
                    _log.Debug("Creating a new bizUser and a bizUser");
                    user = new User();
                    BizUserStore bizUserStore = new BizUserStore();
                    BizUser businessUser = new BizUser
                    {
                        Password = userIdentification,
                        Roles = Enum.GetName(typeof(UserRole), userRole),
                        UserStatus = BizUserStatus.ACTIVATED
                    };

                    long userId = bizUserStore.Add(businessUser, user);
                    if (userId > 0)
                    {
                        _log.Debug("BizUser and User have been created");
                        userProfile.UserId = userId;
                        userProfile.RightList.Add(UserRight.CreatePdeJobs);
                    }
                    else
                    {
                        userProfile.UserId = 0;
                        _log.Warn("BizUser and User could not be created");
                    }
                }
                else
                {
                    _log.DebugFormat("User with userIdentification={0} already exists. UserId={1}", userIdentification, user.UserId);
                    userProfile.UserId = user.UserId;
                    userProfile.RightList.Add(UserRight.CreatePdeJobs);
                }
                return userProfile;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("AuthenticateUser failed. Message={0}", ex.Message);
                throw;
            }
        }

        public PlotSection[] GetAllPlotSections()
        {
            try
            {
                PlotSectionStore plotSectionStore = new PlotSectionStore();
                IList<PlotSection> plotSections = plotSectionStore.GetAllSections();
                if (plotSections == null)
                    return new PlotSection[0];
                return plotSections.ToArray();
            }
            catch (Exception e)
            {
                _log.Error("Getting all plot sections failed. Reason: " + e.Message + ".");
                throw;
            }
        }

        public WorkflowStepState GetJobsStepStateByJobId(long jobId)
        {
            JobStore jobStore = new JobStore();
            Job job = jobStore.GetById(jobId);
            return (WorkflowStepState)job.State;
        }

        /// <summary>
        /// Gets my jobs by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="isRepresentativeUser">if set to <c>true</c> [is representative user].</param>
        /// <param name="sortExpression"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public List<MyJob> GetMyJobsByUserId(long userId, bool isRepresentativeUser, string sortExpression, bool sortAscending)
        {
            try
            {
                var store = new MyJobStore();

                return store.GetMyJobsByUserId(userId, isRepresentativeUser, sortExpression, sortAscending);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("JobManager.GetMyJobsByUserId failed with parameters userId: {0} and isRepresentativeUser: {1}", ex, userId, isRepresentativeUser);

                throw;
            }
        }

        public List<MyJob> GetMyJobsByUserIdAndSearchParameters(long userId, bool isRepresentativeUser, MyJobSearchParameters parameters, string sortExpression, bool sortAscending)
        {
            try
            {
                var store = new MyJobStore();

                return store.GetMyJobsByUserIdAndSearchParameters(userId, isRepresentativeUser, parameters, sortExpression, sortAscending);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("JobManager.GetMyJobsByUserIdAndSearchParameters failed with parameters userId: {0} and isRepresentativeUser: {1}", ex, userId, isRepresentativeUser);

                throw;
            }
        }

        /// <summary>
        /// Gets all the PlacementOptions.
        /// </summary>
        /// <returns></returns>
        public List<PlacementOption> GetAllPlacementOptions()
        {
            try
            {
                var store = new PlacementOptionStore();

                return store.GetAll();
            }
            catch (Exception ex)
            {
                this._log.Error("JobManager.GetAllPlacementOptions failed", ex);

                throw;
            }
        }

        /// <summary>
        /// Gets the users by surrogate filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="skip">Skip first n elements</param>
        /// <param name="take">Take n elements</param>
        /// <returns></returns>
        public User[] GetUsersBySurrogateFilterPaged(string filter, int skip, int take)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("filter cannot be an emtpy string or whitespace", "filter");

            var realFilter = filter.Split(' ');

            try
            {
                var fields = new[] { "LastName", "FirstName", "Company", "Email", "Street", "StreetNr", "City", "CityCode" };
                var store = new UserStore();
                return store.GetUsersBySurrogateFilterPaged(realFilter, fields, skip, take).ToArray();
            }
            catch (Exception ex)
            {
                this._log.Error(string.Format("JobManager.GetUsersBySurrogateFilter({0}) failed", filter), ex);

                throw;
            }
        }


        private void InitLogger()
        {
            try
            {
                if (_log == null)
                {
                    DatashopLogInitializer.Initialize();

                    _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }

        private static Dictionary<string, string> CreateUserVariables(User user)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();

            if (user != null)
            {
                string salutation = user.Salutation ?? string.Empty;
                string firstName = user.FirstName ?? string.Empty;
                string lastName = user.LastName ?? string.Empty;
                string street = user.Street ?? string.Empty;
                string streetNr = user.StreetNr ?? string.Empty;
                string cityCode = user.CityCode ?? string.Empty;
                string city = user.City ?? string.Empty;
                string company = user.Company ?? string.Empty;
                string tel = user.Tel ?? string.Empty;
                string fax = user.Fax ?? string.Empty;
                string email = user.Email ?? string.Empty;

                variables.Add("user_id", user.UserId.ToString());
                variables.Add("user_salutation", salutation);
                variables.Add("user_fullname", string.Format("{0} {1}", firstName, lastName));
                variables.Add("user_firstname", firstName);
                variables.Add("user_lastname", lastName);
                variables.Add("user_street", street);
                variables.Add("user_streetnr", streetNr);
                variables.Add("user_citycode", cityCode);
                variables.Add("user_city", city);
                variables.Add("user_company", company);
                variables.Add("user_tel", tel.Equals(string.Empty) ? " - " : tel);
                variables.Add("user_fax", fax.Equals(string.Empty) ? " - " : fax);
                variables.Add("user_email", email);
                if (user.BizUser != null && user.BizUser.PasswordResetId != null)
                {
                    var userPasswordResetLink = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["resetPassword"].DownloadUrl + user.BizUser.PasswordResetId;
                    variables.Add("link_reset_password", $"<a href={userPasswordResetLink}>{userPasswordResetLink}</a>");

                }
            }



            string linkTag = "<a href=" +
                             GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["newBizUserAdmin"].AdminLink +
                             ">Übersicht über freizuschaltende Benutzer</a>";
            variables.Add("link_activate_user", linkTag);
            variables.Add("link_datashop", "<a href=" + GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["bizUserActivated"].DownloadUrl + ">" + GnsDatashopCommonConfig.Instance.Mail.Mailtemplate["bizUserActivated"].DownloadUrl + "</a>");

            variables.Add("date", DateTime.Now.ToString("d"));
            variables.Add("longdate", DateTime.Now.ToString("D"));
            variables.Add("time", DateTime.Now.ToString("t"));
            variables.Add("longtime", DateTime.Now.ToString("T"));

            return variables;
        }

        /// <summary>
        /// Gets the data export types.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDataExportTypes()
        {
            try
            {
                var tde = GnsDatashopCommonConfig.Instance.Directories.TDEDirectory;

                var dxf = GnsDatashopCommonConfig.Instance.Directories.DXFDirectory;

                var exportTypes = new List<string>();

                if (!string.IsNullOrWhiteSpace(tde))
                    exportTypes.Add("TDE");

                if (!string.IsNullOrWhiteSpace(dxf))
                    exportTypes.Add("DXF");

                return exportTypes;
            }
            catch (Exception ex)
            {
                this._log.Error("GetExportTypes failed", ex);

                throw new Exception("GetExportTypes failed", ex);
            }
        }

        /// <inheritdoc />
        public void SetFailedLogin(long userid)
        {
            var user = GetUser(userid);
            if (user.BizUser == null) return;

            user.BizUser.FailedLoginCount++;
            var loginAttemptLimitExceeded = GnsDatashopCommonConfig.Instance.LoginAttemptLimit.Limit <= user.BizUser.FailedLoginCount;
            if (loginAttemptLimitExceeded)
            {
                user.BizUser.BlockedUntil = DateTime.Now.AddMinutes(GnsDatashopCommonConfig.Instance.LoginAttemptLimit.TimePeriod);

                // Send mail
                Dictionary<string, string> variables = CreateUserVariables(user);
                variables.Add("user_blockeduntil", user.BizUser.BlockedUntil.ToString());
                variables.Add("user_failedlogincount", user.BizUser.FailedLoginCount.ToString());
                MailSender mail = new MailSender();
                mail.SendMail("loginAttemptLimitReached", user.Email, variables, true);
                _log.Debug($"User {user.UserId} was blocked until {user.BizUser.BlockedUntil} after {user.BizUser.FailedLoginCount} attempts.");
            }


            new BizUserStore().Update(user.BizUser);
        }

        /// <inheritdoc />
        public void ResetFailedLogin(long userid)
        {
            var user = GetUser(userid);
            if (user.BizUser == null || user.BizUser.BlockedUntil == null && user.BizUser.FailedLoginCount == 0) return;

            user.BizUser.FailedLoginCount = 0;
            user.BizUser.BlockedUntil = null;
            new BizUserStore().Update(user.BizUser);
        }

        private string GetRolesForUser(User user)
        {
            if (user == null) return null;

            var roles = user.BizUser != null ? user.BizUser.Roles : "TEMP";
            return roles;
        }

        private string GetrUserEmail(long userId)
        {
            var user = GetUser(userId);
            return user.Email;
        }


    }
}