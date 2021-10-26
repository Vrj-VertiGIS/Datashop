using System;
using System.Text.RegularExpressions;
using GEOCOM.GNSDatashop.Model.JobData;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GEOCOM.GNSDatashop.Model.UserData;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.SqlCommand;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    public class JobDetailsStore
    {
        public List<JobDetails> GetJobDetailsByEmailAddressAndTimePeriod(string emailAddress, TimeSpan period)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    // var criteria = session.CreateCriteria(typeof(JobDetails));

                    var emailRestriction = Restrictions.Eq("Email", emailAddress);

                    var timespanRestriction = Restrictions.Between("CreateDate", DateTime.Now.Subtract(period), DateTime.Now);

                    return session.CreateCriteria(typeof(JobDetails))
                                  .Add(emailRestriction)
                                  .Add(timespanRestriction)
                                  .List<JobDetails>()
                                  .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetJobDetailsByEmailAddressAndTimePeriod failed for emaail: {0} and period: {1}", emailAddress, period), ex);
            }
        }

        public JobDetails GetByID(long id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobs = session.CreateCriteria(typeof(JobDetails))
                                  .Add(Restrictions.Eq("JobId", id))
                                  .List<JobDetails>();
                if (jobs.Count == 1)
                    return jobs[0];
                else
                {
                    return null;
                }
            }
        }

        public JobDetails[] GetByUserID(long id)
        {
            JobDetails[] jobsDetails;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobsList = session
                     .CreateCriteria(typeof(JobDetails))
                     .Add(NHibernate.Criterion.Restrictions.Eq("UserId", id))
                     .List<JobDetails>();

                jobsDetails = new JobDetails[jobsList.Count];
                int i = 0;
                foreach (JobDetails job in jobsList)
                {
                    jobsDetails[i] = job;
                    i++;
                }
            }
            return jobsDetails;
        }

        public JobDetails[] GetByEmail(string mail)
        {
            JobDetails[] jobsDetails;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobsList = session
                     .CreateCriteria(typeof(JobDetails))
                     .Add(NHibernate.Criterion.Restrictions.Eq("Email", mail))
                     .List<JobDetails>();

                jobsDetails = new JobDetails[jobsList.Count];
                int i = 0;
                foreach (JobDetails job in jobsList)
                {
                    jobsDetails[i] = job;
                    i++;
                }
            }

            return jobsDetails;
        }

        //TODO: see if really needed after meeting
        public IEnumerable<JobDetails> GetArchiveCandidates(DateTime archiveLimit)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session
                    .CreateCriteria(typeof(JobDetails))
                    .Add(Restrictions.Lt("CreateDate", archiveLimit))
                    .Add(Restrictions.Eq("IsArchived", false))
                    .Add(Restrictions.Eq("IsActive", false))
                    //TODO: correct state
                    .Add(Restrictions.Eq("State", 0))
                    .List<JobDetails>();
            }
        }

        public JobDetails[] GetSomeSorted(bool archived, bool notArchived, string orderExpression, bool ascending, int startIndex, int quantity)
        {
            return GetSomeSorted(archived, notArchived, orderExpression, ascending, startIndex, quantity, null);
        }

        public JobDetails[] GetSomeSorted(bool archived, bool notArchived, string orderExpression, bool ascending, int startIndex, int quantity, string whereClause)
        {
            if (!(archived || notArchived))
            {
                return Enumerable.Empty<JobDetails>().ToArray();
            }

            var restr = new Conjunction();
            if (archived && !notArchived)
            {
                restr.Add(Restrictions.Eq("IsArchived", true));
            }

            if (notArchived && !archived)
            {
                restr.Add(Restrictions.Eq("IsArchived", false));
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                CreateHibernateRestrictionsFromWhereClause(restr, whereClause);
            }
            
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobList = session.CreateCriteria(typeof(JobDetails))
                                        .Add(restr)
                                        .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression))
                                        .SetFirstResult(startIndex)
                                        .SetMaxResults(quantity)
                                        .List<JobDetails>().ToArray();
                
                return jobList;
            }
        }

        public JobDetails[] SearchJob(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool archived, bool notArchived, string orderExpression, bool ascending, int index, int quantity)
        {
            return SearchJob(jobId, createDateOld, createDateNew, stateDateOld, stateDateNew, reason, status, free1, free2, archived, notArchived, orderExpression, ascending, index, quantity, null);
        }


        public JobDetails[] SearchJob(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool archived, bool notArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause)
        {
            if (!(archived || notArchived))
                return new JobDetails[0];

            // generate restrictions
            var restr = new Conjunction();

            if (!createDateOld.Equals(string.Empty))
                restr.Add(Restrictions.Ge("CreateDate", DateTime.Parse(createDateOld)));
            if (!createDateNew.Equals(string.Empty))
                restr.Add(Restrictions.Le("CreateDate", DateTime.Parse(createDateNew)));
            if (!stateDateOld.Equals(string.Empty))
                restr.Add(Restrictions.Ge("LastStateChangeDate", DateTime.Parse(stateDateOld)));
            if (!stateDateNew.Equals(string.Empty))
                restr.Add(Restrictions.Le("LastStateChangeDate", DateTime.Parse(stateDateNew)));
            if (!reason.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Reason", reason));
            if (!status.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("State", status));
            if (!free1.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom1", "%" + free1 + "%"));
            if (!free2.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom2", "%" + free2 + "%"));
            if (archived && !notArchived)
                restr.Add(Restrictions.Eq("IsArchived", true));
            if (notArchived && !archived)
                restr.Add(Restrictions.Eq("IsArchived", false));

            if (!jobId.Equals(null))
            {
                restr.Add(Restrictions.InsensitiveLike("JobId", jobId));
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                CreateHibernateRestrictionsFromWhereClause(restr, whereClause);
            }

            JobDetails[] jobsDetails;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobList = session.CreateCriteria(typeof(JobDetails))
                                       .Add(restr)
                                       .SetFirstResult(index).SetMaxResults(quantity)
                                       .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression))
                                       .List();

                jobsDetails = new JobDetails[jobList.Count];
                int i = 0;
                foreach (JobDetails job in jobList)
                {
                    jobsDetails[i] = job;
                    i++;
                }
            }
            return jobsDetails;
        }


        public JobDetails[] SearchJob(long? jobId, DateTime? createDateOld, DateTime? createDateNew, DateTime? stateDateOld, DateTime? stateDateNew, string reason, string status, string free1, string free2, bool archived, bool notArchived, string orderExpression, bool ascending, int index, int quantity, string whereClause)
        {
            if (!(archived || notArchived))
                return new JobDetails[0];

            // generate restrictions
            var restr = new Conjunction();

            if (createDateOld.HasValue && createDateOld != DateTime.MinValue)
                restr.Add(Restrictions.Ge("CreateDate", createDateOld));
            if (createDateNew.HasValue && createDateNew != DateTime.MinValue)
                restr.Add(Restrictions.Le("CreateDate", createDateNew));
            if (stateDateOld.HasValue && stateDateOld != DateTime.MinValue)
                restr.Add(Restrictions.Ge("LastStateChangeDate", stateDateOld));
            if (stateDateNew.HasValue && stateDateNew != DateTime.MinValue)
                restr.Add(Restrictions.Le("LastStateChangeDate", stateDateNew));
            if (!reason.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Reason", reason));
            if (!status.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("State", status));
            if (!free1.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom1", "%" + free1 + "%"));
            if (!free2.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom2", "%" + free2 + "%"));
            if (archived && !notArchived)
                restr.Add(Restrictions.Eq("IsArchived", true));
            if (notArchived && !archived)
                restr.Add(Restrictions.Eq("IsArchived", false));

            if (!jobId.Equals(null))
            {
                restr.Add(Restrictions.InsensitiveLike("JobId", jobId));
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                CreateHibernateRestrictionsFromWhereClause(restr, whereClause);
            }

            JobDetails[] jobsDetails;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobList = session.CreateCriteria(typeof(JobDetails))
                                       .Add(restr)
                                       .SetFirstResult(index).SetMaxResults(quantity)
                                       .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression))
                                       .List();

                jobsDetails = new JobDetails[jobList.Count];
                int i = 0;
                foreach (JobDetails job in jobList)
                {
                    jobsDetails[i] = job;
                    i++;
                }
            }
            return jobsDetails;
        }

        public int GetSearchCount(long? jobId, string createDateOld, string createDateNew, string stateDateOld, string stateDateNew, string reason, string status, string free1, string free2, bool archived, bool notArchived, string whereClause)
        {
            if (!(archived || notArchived))
                return 0;

            // generate restrictions
            var restr = new Conjunction();

            if (!createDateOld.Equals(string.Empty))
                restr.Add(Restrictions.Ge("CreateDate", DateTime.Parse(createDateOld)));
            if (!createDateNew.Equals(string.Empty))
                restr.Add(Restrictions.Le("CreateDate", DateTime.Parse(createDateNew)));
            if (!stateDateOld.Equals(string.Empty))
                restr.Add(Restrictions.Ge("LastStateChangeDate", DateTime.Parse(stateDateOld)));
            if (!stateDateNew.Equals(string.Empty))
                restr.Add(Restrictions.Le("LastStateChangeDate", DateTime.Parse(stateDateNew)));
            if (!reason.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Reason", reason));
            if (!status.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("State", status));
            if (!free1.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom1", "%" + free1 + "%"));
            if (!free2.Equals(string.Empty))
                restr.Add(Restrictions.InsensitiveLike("Custom2", "%" + free2 + "%"));
            if (archived && !notArchived)
                restr.Add(Restrictions.Eq("IsArchived", true));
            if (notArchived && !archived)
                restr.Add(Restrictions.Eq("IsArchived", false));
            if (!jobId.Equals(null))
                restr.Add(Restrictions.InsensitiveLike("JobId", jobId));

            if (!string.IsNullOrEmpty(whereClause))
            {
                CreateHibernateRestrictionsFromWhereClause(restr, whereClause);
            }

            using (ISession session = NHibernateHelper.OpenSession())
            {
                return (int)session.CreateCriteria(typeof(JobDetails))
                                     .Add(restr)
                                     .SetProjection(Projections.RowCount())
                                     .UniqueResult();
            }
        }

        public int GetPlotCountInTimePeriodForUser(User user, DateTime fromDate)
        {
			return GetPlotCountInTimePeriodForUserAndTemplate(user, "<plottemplate>", fromDate);
        }

        public int GetPlotCountInTimePeriodForUserAndTemplate(User user, string templateName, DateTime fromDate)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var dialect = Dialect.GetDialect(NHibernateHelper.HypernateConfig.Properties);
                var isOracleProvider = dialect.GetType().Name.ToUpperInvariant().Contains("ORACLE");
                IQuery sqlQuery;
                sqlQuery = session.GetNamedQuery(isOracleProvider ? "GET_PLOT_COUNT_ORACLE" : "GET_PLOT_COUNT_MSSQL");

                sqlQuery.SetDateTime("CreateDate", fromDate)
                    .SetString("TemplateName", templateName);

                var isTempUser = user.BizUser == null;
                if (isTempUser)
                    sqlQuery.SetInt64("UserId", -1).SetString("Email", user.Email);
                else
                    sqlQuery.SetInt64("UserId", user.UserId).SetString("Email", null);

                var uniqueResult = sqlQuery.UniqueResult();
                var res = Convert.ToInt32(uniqueResult);
                return res;

            }
        }

        public int GetPlotCountInTimePeriodForUserAndTemplate_old(User user, string templateName, DateTime fromDate)
        {
            var conjunction = new Conjunction();

            var isTempUser = user.BizUser == null;
            if (isTempUser)
                conjunction.Add(Restrictions.Eq("Email", user.Email));
            else
                conjunction.Add(Restrictions.Eq("UserId", user.UserId));

            conjunction.Add(Restrictions.Ge("CreateDate", fromDate));

            using (ISession session = NHibernateHelper.OpenSession())
            {
                var jobs = session.CreateCriteria(typeof(JobDetails))
                    .Add(conjunction)
                    .List<JobDetails>();

                var jobDetailses =
                    jobs.Where(j => string.IsNullOrWhiteSpace(templateName) || j.Definition.Contains(templateName))
                        .Sum(details => Regex.Matches(details.Definition, "</mapextent>").Count);
                return jobDetailses;

            }
        }

        public int GetCount(bool archived, bool notArchived)
        {
            return GetCount(archived, notArchived, null);
        }

        public int GetCount(bool archived, bool notArchived, string whereClause)
        {
            if (!(archived || notArchived))
                return 0;

            // generate restrictions
            var restr = new Conjunction();

            if (archived && !notArchived)
                restr.Add(Restrictions.Eq("IsArchived", true));
            if (notArchived && !archived)
                restr.Add(Restrictions.Eq("IsArchived", false));

            if (!string.IsNullOrEmpty(whereClause))
            {
                CreateHibernateRestrictionsFromWhereClause(restr, whereClause);
            }

            using (ISession session = NHibernateHelper.OpenSession())
            {
                return (int)session.CreateCriteria(typeof(Job))
                                     .Add(restr)
                                     .SetProjection(Projections.RowCount())
                                     .UniqueResult();
            }
        }

        /// <summary>
        /// Adds the passed string where clause to the passed restriction. 
        /// The where clause is in form 'WhereClause := fieldName (= | # | LIKE) value'. 
        /// Note that the hash (#) represents not equal.
        /// </summary>
        static void CreateHibernateRestrictionsFromWhereClause(Conjunction restriction, string whereClause)
        {
            string[] pairParts = whereClause.Split(',');

            string fieldName;
            object value;

            foreach (string pair in pairParts)
            {
                if (pair.Contains("="))
                {
                    GetFieldValue(pair, "=", out fieldName, out value);
                    value = long.Parse(value.ToString());
                    restriction.Add(Restrictions.Eq(fieldName, value));
                }
                else if (pair.Contains("#")) // wia :-)
                {
                    // wia added this one :-)
                    GetFieldValue(pair, "#", out fieldName, out value);
                    restriction.Add(!Restrictions.Eq(fieldName, value));
                }
                else if (pair.Contains("LIKE"))
                {
                    GetFieldValue(pair, "LIKE", out fieldName, out value);
                    restriction.Add(Restrictions.InsensitiveLike(fieldName, value.ToString()));
                }
            }
        }

        /// <summary>
        /// Splits the passed pair in a field name and a value and removes the operand
        /// </summary>
        static void GetFieldValue(string pair, string operand, out string fieldName, out object value)
        {
            int p1 = pair.IndexOf(operand);
            fieldName = pair.Substring(0, p1).Trim();
            int offset = p1 + operand.Length;
            value = pair.Substring(offset, pair.Length - offset).Trim();
        }



    }
}
