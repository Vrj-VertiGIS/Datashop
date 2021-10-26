using System;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model.JobData;
using NHibernate;
using NHibernate.Criterion;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
	public class JobLogStore 
	{
		// logging instance
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

		#region IGNSDatastoreJobLog Members


        /// <summary>
        /// Adds the formatted message to the log of the specified job. 
        /// The resulting message might be truncated to the maximal allowed
        /// database size.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="messageFormat">The formatted message.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns></returns>
	    public bool Add(Job job, string messageFormat, params object[] args)
	    {
	        const int maxAllowedLengthInDb = 255;
            
	        var message = string.Format(messageFormat, args);
	        message = message.Substring(0, Math.Min(message.Length, maxAllowedLengthInDb));
            var jobLog = new JobLog(job, message);
            bool success = Add(jobLog);
	        return success;
	    }

	    public bool Add(JobLog jobLog)
		{
			bool success = false;

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				try
				{
					session.Save(jobLog);
					transaction.Commit();
					success = true;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_log.Error("Adding a new joblog failed", ex);
				}
			}
			return success;			
		}

	    /// <summary>
        /// Gets all logs for a JobId
        /// </summary>
        /// <param name="jobId">The desired jobId</param>
        /// <returns>The log entries for the desired job</returns>
        public JobLog[] GetLogsForJob(long jobId)
        {
            JobLog[] logs;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var logsList = session.CreateCriteria(typeof(JobLog)).
                    AddOrder(Order.Asc("Id")).
                    Add(NHibernate.Criterion.Restrictions.Eq("JobId", jobId)).List();
                logs = new JobLog[logsList.Count];
                int i = 0;
                foreach (JobLog log in logsList)
                {
                    logs[i] = log;
                    i++;
                }
            }
            return logs;
        }

		#endregion

        public Dictionary<long, string> GetCreateDates(Dictionary<long, string> jobs)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                // Initialise Querry
                var criteria = session.CreateCriteria(typeof(JobLog));
                var disjunct = new Disjunction();
                foreach (KeyValuePair<long, string> pair in jobs)
                {
                    disjunct.Add(Expression.Eq("JobId", pair.Key));
                }
                var projection = Projections.ProjectionList();
                projection.Add(Projections.GroupProperty("JobId"));
                projection.Add(Projections.Min("Timestamp"));
                var job_logs = criteria.Add(disjunct).SetProjection(projection).List();

                // Get Dates         
                foreach (object[] job_log in job_logs)
                {
                    jobs[(long)job_log[0]] = ((System.DateTime)job_log[1]).ToString();
                }
            }
            return jobs;
        }

	    public Dictionary<long, string> GetStateDate(Dictionary<long, string> jobs)
	    {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                // Initialise Querry
                var criteria = session.CreateCriteria(typeof(JobLog));
                var disjunct = new Disjunction();
                foreach (KeyValuePair<long, string> pair in jobs)
                {
                    disjunct.Add(Expression.Eq("JobId", pair.Key));
                }
                var projection = Projections.ProjectionList();
                projection.Add(Projections.GroupProperty("JobId"));
                projection.Add(Projections.Max("Timestamp"));

                var job_logs = criteria.Add(disjunct).SetProjection(projection).List();

                // Get Dates
                foreach (object[] job_log in job_logs)
                {
                    jobs[(long)job_log[0]] = ((System.DateTime)job_log[1]).ToString();
                }
            }
            return jobs;
	    }
	}
}
