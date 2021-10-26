using System;
using System.Collections.Generic;
using System.Linq;
using GEOCOM.GNSDatashop.Model.JobData;
using NHibernate.Criterion;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    /// <summary>
    /// Repository class for the MyJobs view
    /// </summary>
    public class MyJobStore
    {
        #region Public Methods

        /// <summary>
        /// Gets my jobs by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="isRepresentativeUser">if set to <c>true</c> [is representative user].</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
        /// <returns></returns>
        public List<MyJob> GetMyJobsByUserId(long userId, bool isRepresentativeUser, string sortExpression, bool sortAscending)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var criteria = session.CreateCriteria(typeof(MyJob));

                    var conjunction = Restrictions.Conjunction();

                    //NOTE: removing this constraint
                    //conjunction.Add(Restrictions.Eq("IsArchived", false));

                    var userCriterion = this.CreateRoleRestrictions(userId, isRepresentativeUser);

                    conjunction.Add(userCriterion);

                    criteria.Add(conjunction);

                    return criteria.AddOrder(new Order(sortExpression, sortAscending))
                        .List<MyJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("MyJobStore.GetMyJobsByUserId failed with parameters userId: {0} and isRepresentativeUser: {1}",
                    userId, isRepresentativeUser), ex);
            }
        }

        /// <summary>
        /// Gets my jobs by user id and search parameters.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="isRepresentativeUser">if set to <c>true</c> [is representative user].</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
        /// <returns></returns>
        public List<MyJob> GetMyJobsByUserIdAndSearchParameters(long userId, bool isRepresentativeUser, MyJobSearchParameters parameters, string sortExpression, bool sortAscending)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var criteria = session.CreateCriteria(typeof(MyJob));

                    var conjunction = Restrictions.Conjunction();

                    var userCriterion = this.CreateRoleRestrictions(userId, isRepresentativeUser);

                    conjunction.Add(userCriterion);

                    //NOTE: Removed this for the
                    //conjunction.Add(Restrictions.Eq("IsArchived", false));

                    if (parameters.Downloaded != null)
                        conjunction.Add(parameters.Downloaded.Value ? Restrictions.Ge("DownloadCount", 1) : Restrictions.Eq("DownloadCount", 0));

                    this.AddEqRestriction(conjunction, "JobId", parameters.JobId);

                    long userIdParameter;

                    if (long.TryParse(parameters.UserId, out userIdParameter))
                        this.AddEqRestriction(conjunction, "RepresentedUserId", userIdParameter);
                    else
                        this.AddInsensitiveLikeRestriction(conjunction, "RepresentedUserEmail", parameters.UserId);

                    this.AddInsensitiveLikeRestriction(conjunction, "RepresentedUserCompany", parameters.Company);
                    this.AddInsensitiveLikeRestriction(conjunction, "RepresentedUserFirstName", parameters.FirstName);
                    this.AddInsensitiveLikeRestriction(conjunction, "RepresentedUserLastName", parameters.LastName);

                    // set CreateDateEnd to absolute end of a day. 
                    if (parameters.CreatedDateEnd != null)
                        parameters.CreatedDateEnd = parameters.CreatedDateEnd.Value.Date.AddDays(1).AddMilliseconds(-1);

                    if (parameters.CreatedDateEnd != null && parameters.CreatedDateStart != null)
                        conjunction.Add(Restrictions.Between("CreatedDate", parameters.CreatedDateStart, parameters.CreatedDateEnd));

                    if (parameters.CreatedDateEnd == null && parameters.CreatedDateStart != null)
                        conjunction.Add(Restrictions.Ge("CreatedDate", parameters.CreatedDateStart));

                    if (parameters.CreatedDateEnd != null && parameters.CreatedDateStart == null)
                        conjunction.Add(Restrictions.Le("CreatedDate", parameters.CreatedDateEnd));

                    this.AddInsensitiveLikeRestriction(conjunction, "Custom1", parameters.Custom1);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom2", parameters.Custom2);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom3", parameters.Custom3);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom4", parameters.Custom4);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom5", parameters.Custom5);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom6", parameters.Custom6);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom7", parameters.Custom7);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom8", parameters.Custom8);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom9", parameters.Custom9);
                    this.AddInsensitiveLikeRestriction(conjunction, "Custom10", parameters.Custom10);
                    this.AddInsensitiveLikeRestriction(conjunction, "ParcelNumber", parameters.JobParcelNumber);

                    if (parameters.ReasonId != null)
                        this.AddEqRestriction(conjunction, "ReasonId", parameters.ReasonId);
                    


                    criteria.Add(conjunction);

                    return criteria.AddOrder(new Order(sortExpression, sortAscending))
                       .List<MyJob>()
                       .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("MyJobStore.GetMyJobsByUserIdAndSearchParameters failed with parameters userId: {0} and isRepresentativeUser: {1}",
                    userId, isRepresentativeUser), ex);
            }
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the role restrictions.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="isRepresentativeUser">if set to <c>true</c> [is representative user].</param>
        /// <returns></returns>
        private ICriterion CreateRoleRestrictions(long userId, bool isRepresentativeUser)
        {
            if (!isRepresentativeUser)
                return Restrictions.Eq("RepresentedUserId", userId);

            var disjunction = Restrictions.Disjunction();
            disjunction.Add(Restrictions.Eq("RepresentedUserId", userId));
            disjunction.Add(Restrictions.Eq("RepresentativeUserId", userId));

            return disjunction;
        }

        /// <summary>
        /// Adds the eq restriction.
        /// </summary>
        /// <param name="jct">The JCT.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        private void AddEqRestriction(Junction jct, string fieldName, object value)
        {
            if (value != null)
                jct.Add(Restrictions.Eq(fieldName, value));
        }

        /// <summary>
        /// Adds the insensitive like restriction.
        /// </summary>
        /// <param name="jct">The JCT.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        private void AddInsensitiveLikeRestriction(Junction jct, string fieldName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                jct.Add(Restrictions.InsensitiveLike(fieldName, value, MatchMode.Start));
        } 

        #endregion
    }
}