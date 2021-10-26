using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    public class JobStore
    {
        // logging instance
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        #region IGNSDatastoreJob Members

        public long Add(Job job)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    job.Step = 0;
                    job.State = (int)WorkflowStepState.Idle;
                    job.NeedsProcessing = true;
                    job.IsActive = true;
                    job.JobId = (long)session.Save(job);
                    if (job.SurrogateJob != null)
                    {
                        job.SurrogateJob.JobId = job.JobId;
                        session.Save(job.SurrogateJob);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Adding a new job failed", ex);
                }
            }
            CreateJobLog(job, JobLogType.New);

            return job.JobId;
        }

        public virtual bool Update(Job job)
        {
            Job oldJob = null;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    oldJob = GetById(job.JobId);
                    session.Update(job);

                    if (job.SurrogateJob != null)
                        session.Update(job.SurrogateJob);

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Update Job failed", ex);
                    CreateJobLog(oldJob, JobLogType.Update, ex.Message);
                    throw;
                }
            }

            CreateJobLog(oldJob, JobLogType.Update, string.Empty);
            return true;
        }

        public bool Delete(long jobId)
        {
            Job oldJob = new Job();
            bool deleteJobSuccess;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var jobs = session.CreateCriteria(typeof(Job))
                                    .Add(Restrictions.Eq("JobId", jobId)).List<Job>();
                if (jobs.Count == 1)
                {
                    oldJob = jobs[0];
                    session.Delete(jobs[0]);
                    session.Flush();
                    transaction.Commit();

                    deleteJobSuccess = true;
                }
                else
                {
                    deleteJobSuccess = false;
                }
            }
            CreateJobLog(oldJob, JobLogType.Delete);
            return deleteJobSuccess;
        }

        public Job GetById(long id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var job = session
                     .CreateCriteria(typeof(Job))
                     .Add(Restrictions.Eq("JobId", id))
                     .List<Job>().SingleOrDefault();
                return job;

            }
        }

        /// <summary>
        /// Gets number of jobs matching the jobStepList and jobStatusList.
        /// The jobStatus/jobSubStatus are used as SQL "IN" operation.
        /// </summary>
        /// <returns>A list of pending jobs</returns>
        public IList<Job> GetPendingJobs(int quantity, LoadBalancingPreference loadBalancing, bool sortAsc)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Job));

                var order = sortAsc ? Order.Asc("JobId") : Order.Desc("JobId");
                criteria.Add(Restrictions.Eq("NeedsProcessing", true))
                        .AddOrder(order)
                        .SetFirstResult(0)
                        .SetMaxResults(quantity); // limit number of jobs

                if (loadBalancing != LoadBalancingPreference.NONE)
                {
                    var sign = loadBalancing == LoadBalancingPreference.EVEN ? "=" : "<>" ;
                    if (session.GetSessionImplementation().Factory.Dialect.GetType().Name.Contains("MsSql"))
                        criteria.Add(Expression.Sql("{alias}.JobId % 2 " + sign +" 0"));
                    else if (session.GetSessionImplementation().Factory.Dialect.GetType().Name.Contains("Oracle"))
                        criteria.Add(Expression.Sql("MOD({alias}.JobId, 2) " + sign +" 0"));
                }
                
                return criteria.List<Job>().Where(l => l.GetType() == typeof(Job)).ToList();
            }
        }

        /// <summary>
        /// Get all jobs in the state running
        /// </summary>
        /// <returns>All running jobs</returns>
        public IList<Job> GetJobsByState(int state)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Job));
                criteria.Add(Restrictions.Eq("State", state));

                return criteria.List<Job>().Where(l => l.GetType() == typeof(Job)).ToList();
            }
        }

        #endregion



        /// <summary>
        /// Gets all Jobs as Arrays
        /// </summary>
        /// <returns>All jobs in database</returns>
        public Job[] GetAll()
        {
            Job[] jobs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobsList = session.CreateCriteria(typeof(Job)).List();
                jobs = new Job[jobsList.Count];
                int i = 0;
                foreach (Job job in jobsList)
                {
                    jobs[i] = job;
                    i++;
                }
            }
            return jobs;
        }

        /// <summary>
        /// gets all jobs sortet.
        /// </summary>
        /// <param name="orderExpression">Property to sort by</param>
        /// <param name="ascending">true = ascending, fals = descending</param>
        /// <returns>all jobs in GNSD_JOBS as arrays</returns>
        public Job[] GetAllOrdered(string orderExpression, bool ascending)
        {
            Job[] jobs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobsList = session.CreateCriteria(typeof(Job)).
                    AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression)).
                    AddOrder(Order.Asc("JobId")).List();
                jobs = new Job[jobsList.Count];
                int i = 0;
                foreach (Job job in jobsList)
                {
                    jobs[i] = job;
                    i++;
                }
            }
            return jobs;
        }

        public Job[] GetByUserId(long userId)
        {
            Job[] jobs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobsList = session.CreateCriteria(typeof(Job)).Add(NHibernate.Criterion.Restrictions.Eq("UserId", userId)).List();
                jobs = new Job[jobsList.Count];
                int i = 0;
                foreach (Job job in jobsList)
                {
                    jobs[i] = job;
                    i++;
                }
            }
            return jobs;
        }

        public Job[] GetByEmail(string email)
        {
            Job[] jobs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var userList = session.CreateCriteria(typeof(User)).Add(Restrictions.Eq("Email", email)).List();

                var disjunct = new Disjunction();
                foreach (User user in userList)
                {
                    disjunct.Add(Expression.Eq("UserId", user.UserId));
                }
                var jobsList = session.CreateCriteria(typeof(Job)).Add(disjunct).List();
                jobs = new Job[jobsList.Count];
                int i = 0;
                foreach (Job job in jobsList)
                {
                    jobs[i] = job;
                    i++;
                }
            }
            return jobs;
        }

        public Job[] GetSomeOrdered(string orderExpression, bool ascending, int index, int quantity)
        {
            Job[] jobs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList jobsList;
                if (orderExpression.Equals("StateDate"))
                {
                    var projection = Projections.ProjectionList();
                    projection.Add(Projections.GroupProperty("JobId"));
                    projection.Add(Projections.Max("Timestamp"), "TimeSort");

                    var job_logs = session.CreateCriteria(typeof(JobLog)).SetProjection(projection).
                        AddOrder(ascending ? Order.Asc("TimeSort") : Order.Desc("TimeSort")).
                        SetMaxResults(quantity).SetFirstResult(index).List();

                    var disjunct = new Disjunction();
                    foreach (object[] job in job_logs)
                    {
                        disjunct.Add(Expression.Eq("JobId", job[0]));
                    }
                    jobsList = session.CreateCriteria(typeof(Job)).Add(disjunct).List();
                }
                else
                {
                    jobsList = session.CreateCriteria(typeof(Job))
                                        .SetFirstResult(index)
                                        .SetMaxResults(quantity)
                                        .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression))
                                        .AddOrder(Order.Asc("JobId")).List();
                }
                jobs = new Job[jobsList.Count];
                int i = 0;
                foreach (Job job in jobsList)
                {
                    jobs[i] = job;
                    i++;
                }
            }
            return jobs;
        }

        public int GetCount()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return (int)session.CreateCriteria(typeof(Job)).SetProjection(Projections.RowCount()).UniqueResult();
            }
        }

        public int GetSearchCount(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status)
        {
            throw new NotImplementedException("This methode has been replaced by GridJob");
        }

        public Job[] SearchJob(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string orderExpression, bool ascending, int index, int quantity)
        {
            throw new NotImplementedException("This Methode has been repleaced by Gridjob");
        }

        #region private methods
        private static string GetEnumsAsString<T>(IEnumerable<T> enumList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T enumElement in enumList)
            {
                sb.Append(Enum.GetName(typeof(T), enumList));
                sb.Append("|");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        private void CreateJobLog(Job oldJob, JobLogType jobLogType)
        {
            CreateJobLog(oldJob, jobLogType, string.Empty);
        }

        private void CreateJobLog(Job oldJob, JobLogType jobLogType, string externalMessage)
        {
            Job currentJob = GetById(oldJob.JobId);
            string message = string.Empty;

            if (string.IsNullOrEmpty(externalMessage))
            {
                if (jobLogType == JobLogType.New)
                {
                    message = "New job created";
                }
                else if (jobLogType == JobLogType.Delete)
                {
                    message = "Job deleted";
                }
                else if (jobLogType == JobLogType.Update)
                {
                    if (!currentJob.NeedsProcessing && oldJob.NeedsProcessing)
                    {
                        //message = "Job processing (re)started";
                    }
                    else if (currentJob.NeedsProcessing && !oldJob.NeedsProcessing)
                    {
                        //message = "Job activated or restarted";
                    }
                    else if (!currentJob.IsActive && oldJob.IsActive)
                    {
                        message = "Job processing stopped";
                    }
                    else
                    {
                        if (currentJob.Step != oldJob.Step)
                        {
                            //message = "Step changed, ";
                        }
                        if (currentJob.State != oldJob.State)
                        {
                            //message += "State changed, ";
                        }
                        if (currentJob.NeedsProcessing != oldJob.NeedsProcessing)
                        {
                            //message += "Needs processing changed, ";
                        }
                        if (currentJob.IsActive != oldJob.IsActive)
                        {
                            //message += "IsActive changed, ";
                        }
                        if (message.Length > 2)
                        {
                            message = message.Remove(message.Length - 2, 2);
                        }
                    }
                }
            }
            else
            {
                message = externalMessage;
            }

            JobLogStore jobLogStore = new JobLogStore();
            jobLogStore.Add(currentJob, message);
        }
        #endregion
    }
}