using System;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model.JobData;
using NHibernate;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    public class JobGuidStore 
    {
		// logging instance
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

		#region IGNSDatastoreJobGuid Members
        /// <summary>
        /// Get JobGUID relation entry from guid
        /// </summary>
        /// <param name="guid">The Guid to be fetched</param>
        /// <returns>null if no entry was found</returns>
        public JobGuid GetByGuid(string guid)
        {
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var entries = session
					 .CreateCriteria(typeof(JobGuid))
					 .Add(NHibernate.Criterion.Restrictions.Eq("Guid", guid))
					 .List<JobGuid>();

				if (entries.Count == 0) return null;
				return entries[0];
			}
        }

        /// <summary>
        /// Gets the JobGuid from Db. If no JobGuid exists, a new JobGuid is created and saved to Db.
        /// </summary>
        /// <param name="jobId">The jobId of the jobGuid to be fetched</param>
        /// <returns>The jobGuid</returns>
        public JobGuid GetOrCreateByJobId(long jobId)
        {
			using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var entries = session
                    .CreateCriteria(typeof (JobGuid))
                    .Add(NHibernate.Criterion.Restrictions.Eq("JobId", jobId))
                    .List<JobGuid>();

                if (entries.Count == 0)
                {
                    JobGuid jobGuid = new JobGuid();
                    jobGuid.Guid = Guid.NewGuid().ToString();
                    jobGuid.JobId = jobId;

                    session.Save(jobGuid);
                    transaction.Commit();

                    return jobGuid;
                }
                else
                {
                    return entries[0];
                }
            }
        }

        #endregion
    }
}
