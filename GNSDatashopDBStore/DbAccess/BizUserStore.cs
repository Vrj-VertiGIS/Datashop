using System;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using NHibernate;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    public class BizUserStore 
    {
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        public long Add(BizUser bizUser, User user)
        {
            if (bizUser == null)
                return -1;
            long res = -1;
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    var bid = session.Save(bizUser);
                    user.BizUserId = (long)bid;
                    var id = session.Save(user);
                    res = (long)id;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Adding a new bizUser failed", ex);
                    return -1;
                }
            }
            return res;
        }

        /// <summary>
        /// Gets a BizUser specified by his BizID.
        /// If no User is found, null will be returned.
        /// If multiple Users are found (shoud never happen) the first result will be returned.
        /// </summary>
        /// <param name="bizId">The Id of the business user</param>
        /// <returns>The business user depending on the bizId</returns>
        public BizUser GetByBizId(long bizId)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                try
                {
                    var bizUsers = session
                         .CreateCriteria(typeof(BizUser))
                         .Add(NHibernate.Criterion.Restrictions.Eq("BizUserId", bizId))
                         .List<BizUser>();

                    if (bizUsers.Count == 0)
                    {
                        _log.InfoFormat("No BizUser with bizId {0} found.", bizId);
                        return null;
                    }
                    _log.DebugFormat("Got bizUser with bizId {0}.", bizId);
                    return bizUsers[0];
                }
                catch (Exception exp)
                {
                    _log.Error("Could not get BizUser with bizId " + bizId, exp);
                    throw;
                }
            }
        }        

        /// <summary>
        /// Gets a BizUser specified by his pwdResetId.
        /// If no User is found, null will be returned.
        /// If multiple Users are found (shoud never happen) the first result will be returned.
        /// </summary>
        /// <param name="pwdResetId">The Id of the business user</param>
        /// <returns>The business user depending on the pwdResetId</returns>
        public BizUser GetByPasswordResetId(Guid pwdResetId)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                try
                {
                    var bizUsers = session
                        .CreateCriteria(typeof(BizUser))
                        .Add(NHibernate.Criterion.Restrictions.Eq("PasswordResetId", pwdResetId))
                        .List<BizUser>();

                    if (bizUsers.Count == 0)
                    {
                        _log.InfoFormat("No BizUser with pwdResetId {0} found.", pwdResetId);
                        return null;
                    }
                    _log.DebugFormat("Got bizUser with pwdResetId {0}.", pwdResetId);
                    return bizUsers[0];
                }
                catch (Exception exp)
                {
                    _log.Error("Could not get BizUser with pwdResetId " + pwdResetId, exp);
                    throw;
                }
            }
        }        

        /// <summary>
        /// Deletes a biz_user, but only if he has no jobs associated
        /// </summary>
        /// <param name="bizUser">The business user</param>
        /// <param name="deleteAllRelatedData">Flag id all related data should be deleted, too</param>
        /// <param name="deleteRetealedUser">if set, the coresponding GNSD_User will be deleted, too.</param>
        /// <returns>True if sucessfull</returns>
        public bool Delete(BizUser bizUser, bool deleteAllRelatedData, bool deleteRetealedUser)
        {
            try
            {
                // check if bizUsers has mad jobs
                var userStore = new UserStore();
                var users = userStore.GetByBizUserId(bizUser.BizUserId);
                if (users.Length == 0)
                {
                    // Should never reach this code if DB is healty
                    _log.WarnFormat("Found a BizUser without a corresponding User. Will delete this BizUser");
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        session.Delete(bizUser);
                        session.Flush();
                        _log.Info("BizUSer {0} has been deleted. No associated User found.");
                        return true;
                    }
                }
                else
                {
                    var jobStore = new JobStore();
                    List<Job> jobs = new List<Job>();
                    foreach (User user in users)
                    {
                        jobs.AddRange(jobStore.GetByUserId(user.UserId));
                    }
                    if (jobs.Count != 0)
                    {
                        _log.WarnFormat("Could not delet BizUSer with BizID {0}, because he has allready made some Jobs.", bizUser.BizUserId);
                        return false;
                    }
                }

                if (deleteAllRelatedData) throw new NotImplementedException("deleteAllRelatedData with BizUser");

                if (deleteRetealedUser)
                {
                    _log.InfoFormat("Trying to delet BizUser with ID {0} and his associated User", bizUser.BizUserId);
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        foreach (User user in users)
                        {
                            // Should only be one User if DB is healthy
                            session.Delete(user);
                            _log.InfoFormat("Deleted User with Email {0}.", user.Email);
                        }
                        session.Delete(bizUser);
                        session.Flush();
                        return true;
                    }
                }
                else
                {
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        session.Delete(bizUser);
                        session.Flush();
                        _log.InfoFormat("Deleted BizUser with BizID{0} without deleting associated user.");
                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                _log.Error("BizUser could not bee deleted.", exp);
                throw;                
            }
        }

        public bool Update(BizUser bizUser)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Update(bizUser);
                    transaction.Commit();
                    _log.DebugFormat("BizUser with bizID {0} was updated", bizUser.BizUserId);
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Update bizUser failed", ex);
                    return false;
                }
            }
         }

        public IList<BizUser> GetAll()
        {
           using (ISession session = NHibernateHelper.OpenSession())
           {
               try
               {
                   return session
                               .CreateCriteria(typeof(BizUser))
                               .List<BizUser>();
               }
               catch (Exception exp)
               {
                   _log.Error("Could not get all BizUsers", exp);
                   throw;
               }   
           }
        }

        public IList<BizUser> GetNotActivated()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                try
                {
                    _log.Debug("Getting all not activated BizUsers");
                    return session.CreateCriteria(typeof(BizUser)).Add(NHibernate.Criterion.Restrictions.Eq("Status", BizUserStatus.LOCKED))
                                       .List<BizUser>();
                }
                catch (Exception exp)
                {
                    _log.Error("Getting not activated BizUsers failed", exp);
                    throw;
                }                
            }
        }
    }
}
