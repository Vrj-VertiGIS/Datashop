using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model.UserData;
using NHibernate;
using NHibernate.Criterion;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
	public class UserStore 
	{
		// logging instance
		private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

		public long Add(User user)
		{
			if (user == null)
				return -1;

			long newUserId = -1;
			_log.DebugFormat("Adding new User with email {0}", user.Email);

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				try
				{
					newUserId = (long)session.Save(user);
					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_log.Error("Adding a new user failed", ex);
					throw;
				}
			}
			return newUserId;
		}

		public bool Update(User user)
		{
			if (user == null)
				return false;

			try
			{
				using (ISession session = NHibernateHelper.OpenSession())
				{
					_log.DebugFormat("Updating user {0}", user.UserId);
					session.Update(user);
					session.Flush();
					return true;
				}
			}
			catch (Exception exp)
			{
				_log.Error("Updating user " + user.UserId + " failed", exp);
				throw;
			}
		}

		/// <summary>
		/// Delets a User from the DB with cheking if he has jobs.
		/// </summary>
		/// <param name="userId">UserId to delete</param>
		/// <returns>true if delete was succsefull. False if user didn't exist or has jobs</returns>
		public bool Delete(long userId)
		{
			// Check for jobs
			var jobStore = new JobStore();
			var jobs = jobStore.GetByUserId(userId);
			if (jobs.Length != 0)
			{
				_log.WarnFormat("Could not delet User with ID {0}, because he has allready made some Jobs.", userId);
				return false;
			}

			using (ISession session = NHibernateHelper.OpenSession())
			{
			  var users = session.CreateCriteria(typeof(User))
									.Add(NHibernate.Criterion.Restrictions.Eq("UserId", userId)).List<User>();
				if (users.Count == 1)
				{
					session.Delete(users[0]);
					session.Flush();
					_log.DebugFormat("User with ID {0} was deleted.", userId);
					return true;
				}
				else
				{
					_log.WarnFormat("Could not delete User with ID {0}, because no or multiple users with this ID found", userId);
					return false;
				}   
			}
		}

		/// <summary>
		/// Gets a specific User with the Userid = id
		/// </summary>
		/// <param name="id">The UserId to be looked after</param>
		/// <returns>User with the specified UserId. If he does not exist, an exception will be thrown.</returns>
		public User GetById(long id)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var users = session
					 .CreateCriteria(typeof(User))
					 .Add(NHibernate.Criterion.Restrictions.Eq("UserId", id))
					 .List<User>();

				if (users.Count == 0)
				{
					_log.WarnFormat("No User with ID {0} found.", id);
					return null;
				}
				_log.DebugFormat("Got User {0}", id);
				return users[0];
			}
		}

		public User[] GetByBizUserId(long bizId)
		{
			try
			{
				User[] users;
				_log.DebugFormat("Getting all Users with BizId {0}", bizId);
				using (ISession session = NHibernateHelper.OpenSession())
				{
					var usersList = session.CreateCriteria(typeof(User)).Add(Restrictions.Eq("BizUserId", bizId)).List();
					users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
				}
				return users;
			}
			catch (Exception exp)
			{
				_log.Error("Could not get Users with BizId" + bizId, exp);
				throw;
			}
		}

		/// <summary>
		/// Gets all User in the Database, not sorted
		/// </summary>
		/// <returns>all Users in DB</returns>
		public User[] GetAll()
		{
			try
			{
				_log.Debug("Getting all users.");
				
				using (ISession session = NHibernateHelper.OpenSession())
				{
					var users = session.CreateCriteria(typeof(User)).List<User>().ToArray();
                    _log.DebugFormat("Got {0} Users.", users.Length);
				    return users;
				}				
			}
			catch (Exception exp)
			{
				_log.Error("Could not get all Users", exp);
				throw;
			}
		}


        public User[] GetAllUsersPages(int skip, int take)
        {
            try
            {
                _log.Debug("Getting all users.");

                using (ISession session = NHibernateHelper.OpenSession())
                {
                    var users = session.CreateCriteria(typeof(User)).SetMaxResults(take).SetFirstResult(skip).List<User>().ToArray();
                    _log.DebugFormat("Got {0} Users.", users.Length);
                    return users;
                }
            }
            catch (Exception exp)
            {
                _log.Error("Could not get all Users", exp);
                throw;
            }
        }

        /// <summary>
        /// Gets all Users in the Database ordered by Property defined in String Orderexpression
        /// </summary>
        /// <param name="orderExpression">The property to sort</param>
        /// <param name="ascending">true = asscending, false = descending</param>
        /// <param name="showBusinessUser">indicates if businessusers are returned</param>
        /// <param name="showTempUser">indicates if tempUsers are returned</param>
        /// <returns>All User in DB as an Array, sorted.</returns>
        public User[] GetAllOrdered(string orderExpression, bool ascending, bool showBusinessUser, bool showTempUser)
		{        
			if (!(showBusinessUser || showTempUser))
				return new User[0];

			if (showTempUser == showBusinessUser)
			{
				throw new ArgumentException("showBusinessUser and showTempUser can't both be true. Sever Problems with returning both, if result is sortet after a property of BizUsers. This is mainly because an alias for the property bizUser has to be set, an then it won't find any tempusers");
			}

			try
			{
				var restr = new Conjunction();
				if (showBusinessUser && !showTempUser)
					restr.Add(Restrictions.IsNotNull("BizUserId"));
				if (!showBusinessUser && showTempUser)
					restr.Add(Restrictions.IsNull("BizUserId"));
				
				User[] users;
				_log.Debug("Getting all Users ordered");
				using (ISession session = NHibernateHelper.OpenSession())
				{                 
					var criteria = session.CreateCriteria(typeof(User));

					// An Alias is created for BizUser. this is used for Sorting after BizUser.Status and BizUser.Roles. Problem is, with alias no TempUser will be found
					if (showBusinessUser)
						criteria.CreateAlias("BizUser", "bizUserAlias");                    

                    var userCriteria = criteria.Add(restr)
                                               .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression));

                    if (!orderExpression.Equals("UserId"))
                        userCriteria = userCriteria.AddOrder(Order.Asc("UserId"));

                    var usersList = userCriteria.List();

					users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
				}
				return users;
			}
			catch (Exception exp)
			{
				_log.Error("Could not get users ordered.", exp);
				throw;
			}
		}

		public ICollection<User> GetByLastname(string name)
		{
			throw new NotImplementedException();
		}

		public User[] GetSomeOrdered(string orderExpression, bool ascending, int index, int quantity, bool showBusinessUser, bool showTempUser)
		{
			if (!(showBusinessUser || showTempUser))
				return new User[0];
			if (showTempUser == showBusinessUser)
			{
				throw new ArgumentException("showBusinessUser and showTempUser can't both be true. Sever Problems with returning both, if result is sortet after a property of BizUsers. This is mainly because an alias for the property bizUser has to be set, an then it won't find any tempusers");
			}

			try
			{
				var restr = new Conjunction();
				if (showBusinessUser && !showTempUser)
					restr.Add(Restrictions.IsNotNull("BizUserId"));
				if (!showBusinessUser && showTempUser)
					restr.Add(Restrictions.IsNull("BizUserId"));

				_log.Debug("Getting some Users ordered");
				User[] users;
				using (ISession session = NHibernateHelper.OpenSession())
				{
					var criteria = session.CreateCriteria(typeof(User));

					// An Alias is created for BizUser. this is used for Sorting after BizUser.Status and BizUser.Roles. Problem is, with alias no TempUser will be found
					if (showBusinessUser)
					{
						criteria.CreateAlias("BizUser", "bizUserAlias");
					}
				  
					var usersList = criteria.Add(restr)
											.SetFirstResult(index)
											.SetMaxResults(quantity)
											.AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression))
											.List();
					users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
				}
				return users;
			}
			catch (Exception exp)
			{
				_log.Error("Could not get some Users ordered.", exp);
				throw;
			}
		}

		public int GetCount(bool showBusinessUser, bool showTempUser)
		{
			if (!(showBusinessUser || showTempUser))
				return 0;

			try
			{
				var restr = new Conjunction();
				if (showBusinessUser && !showTempUser)
					restr.Add(Restrictions.IsNotNull("BizUserId"));
				if (!showBusinessUser && showTempUser)
					restr.Add(Restrictions.IsNull("BizUserId"));

				using (ISession session = NHibernateHelper.OpenSession())
				{
					_log.Debug("Getting count of all users in DB");
					return (int)session.CreateCriteria(typeof(User)).Add(restr).SetProjection(Projections.RowCount()).UniqueResult();
				}
			}
			catch (Exception exp)
			{
				_log.Error("Could not get count of Users in DB.", exp);
				throw;
			}
		}      

		public User[] SearchUsers(string salutation, string firstName, string lastName, string street, string streetNr, string cityCode, string city, string company, string email, string tel, string fax, string status, string roles, long? id, bool showBusinessUser, bool showTempUser, string orderExpression, bool ascending, int index, int quantity)
		{
			if (!(showBusinessUser || showTempUser))
				return new User[0];

			if (showTempUser == showBusinessUser)
			{
				throw new ArgumentException("showBusinessUser and showTempUser can't both be true. Sever Problems with returning both, if result is sortet after a property of BizUsers. This is mainly because an alias for the property bizUser has to be set, an then it won't find any tempusers");
			}

			try
			{
				var restr = GetRestriction(showBusinessUser, showTempUser, salutation, firstName, lastName, street, streetNr, cityCode, city, company, email, tel, fax, status, roles, id);

				User[] users;
				using (ISession session = NHibernateHelper.OpenSession())
				{
					var criteria = session.CreateCriteria(typeof(User));

					// An Alias is created for BizUser. this is used for Sorting after BizUser.Status and BizUser.Roles. Problem is, with alias no TempUser will be found
					if (showBusinessUser)
					{
						criteria.CreateAlias("BizUser", "bizUserAlias");
					}

                    criteria.Add(restr)
                            .SetFirstResult(index)
                            .SetMaxResults(quantity)
                            .AddOrder(ascending ? Order.Asc(orderExpression) : Order.Desc(orderExpression));

                    if (!orderExpression.Equals("UserId"))
                        criteria = criteria.AddOrder(Order.Asc("UserId"));

                    IList usersList = criteria.List();

					users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
				}
				return users;
			}
			catch (Exception exp)
			{
				_log.Error("Could not search for users.", exp);
				throw;
			}
		}

		public int GetSearchCount(string salutation, string firstName, string lastName, string street, string streetNr, string cityCode, string city, string company, string email, string tel, string fax, string status, string roles, long? id, bool showBusinessUser, bool showTempUser)
		{
			if (!(showBusinessUser || showTempUser))
				return 0;
			if (showTempUser == showBusinessUser)
			{
				throw new ArgumentException("showBusinessUser and showTempUser can't both be true. Sever Problems with returning both, if result is sortet after a property of BizUsers. This is mainly because an alias for the property bizUser has to be set, an then it won't find any tempusers");
			}
			try
			{               
				_log.Debug("Getting count for a searchResult.");

				var restr = GetRestriction(showBusinessUser, showTempUser, salutation, firstName, lastName, street,
										   streetNr, cityCode, city, company, email, tel, fax, status, roles, id);

				using (ISession session = NHibernateHelper.OpenSession())
				{
					var criteria = session.CreateCriteria(typeof(User));

					// An Alias is created for BizUser. this is used for Sorting after BizUser.Status and BizUser.Roles. Problem is, with alias no TempUser will be found
					if (showBusinessUser)
					{
						criteria.CreateAlias("BizUser", "bizUserAlias");
					}

					return (int)criteria.Add(restr).SetProjection(Projections.RowCount()).UniqueResult();
				}
			}
			catch (Exception exp)
			{
				_log.Error("Could not get count of searchResults.", exp);
				throw;
			}
		}

		public User[] GetByEmail(string email)
		{
			try
			{
				_log.DebugFormat("Getting Users for Email {0}", email);
				User[] users;
				var restr = new Conjunction();
				restr.Add(Restrictions.InsensitiveLike("Email", email));

				using (ISession session = NHibernateHelper.OpenSession())
				{
					var usersList = session.CreateCriteria(typeof(User)).Add(restr).List();
					users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
				}
				_log.DebugFormat("Found {0} users with email {1}", users.Length, email);
				return users;
			}
			catch (Exception exp)
			{
				_log.Error("Could not get user by email " + email, exp);
				throw;
			}
		}

		public User[] GetNotActivated()
		{
			return this.GetNotActivatedOrdered(String.Empty, true);
		}

		public User[] GetNotActivatedOrdered(string orderExpression, bool orderAscending)
		{
			_log.DebugFormat("Getting not activated bizUsers ordered. orderExpression={0} orderAscending={1}", orderExpression, orderAscending);

			try
			{
				using (ISession session = NHibernateHelper.OpenSession())
				{
					var criteria = session.CreateCriteria(typeof(User));
					criteria.CreateAlias("BizUser", "bu")
							.Add(Restrictions.IsNotNull("BizUserId"))
							.Add(Restrictions.Not(Restrictions.Eq("bu.UserStatus", BizUserStatus.ACTIVATED)));
					if (!String.IsNullOrEmpty(orderExpression))
					{
						criteria.AddOrder(orderAscending ? Order.Asc(orderExpression) : Order.Desc(orderExpression));
					}
					if (!orderExpression.Equals("UserId"))
					{
						criteria.AddOrder(Order.Asc("UserId"));
					}

					var usersList = criteria.List();

					User[] users = new User[usersList.Count];
					int i = 0;
					foreach (User user in usersList)
					{
						users[i] = user;
						i++;
					}
					_log.DebugFormat("Got {0} not activated Users", users.Length);
					return users;
				}
			}
			catch (Exception exp)
			{
				_log.ErrorFormat("Could not get not activated users. {0}", exp.Message);
				throw;
			}
		}

		/// <summary>
		/// Looks for the user specified by the identification and role.
		/// </summary>
		/// <param name="userIdentification">The user identification</param>
		/// <param name="userRole">The user role</param>
		/// <returns>The user corresponding to the identification and userRole. Null if no user found</returns>
		public User GetByUserIdentification(string userIdentification, UserRole userRole)
		{
			try
			{
				_log.DebugFormat("Getting Users for userIdentification={0} userRole={1}", userIdentification, userRole);
				User user = null;
				Conjunction restr = new Conjunction();
				switch (userRole)
				{
					case UserRole.Admin:
					case UserRole.Business:
					case UserRole.Temporary:
						// TODO Not supported at the moment
						_log.DebugFormat("UserRole={0} not supported", userRole);
						return null;
					case UserRole.Internal:
					case UserRole.Public:
						// Use Password field to save the userIdentification until the new Db Scheme is available
						restr.Add(Restrictions.Eq("BizUser.Password", userIdentification));
						restr.Add(Restrictions.Eq("BizUser.Roles", Enum.GetName(typeof(UserRole), userRole)));

						break;
				}

				using (ISession session = NHibernateHelper.OpenSession())
				{
					IList userList = session.CreateCriteria(typeof(User))
											.CreateCriteria("BizUser", "BizUser")
											.Add(restr).List();

					if ((userList != null) && (userList.Count == 1))
					{
						user = (User)userList[0];
						_log.DebugFormat("Found a user with userIdentification={0}", userIdentification);
					}
					else
					{
						_log.DebugFormat("Didn't find a user with userIdentification={0}", userIdentification);                        
					}
				}
				return user;
			}
			catch (Exception exp)
			{
				_log.ErrorFormat("Could not get user by userIdentification={0}", userIdentification, exp);
				throw;
			}
		}


		private Conjunction GetRestriction(bool showBusinessUser, bool showTempUser, string salutation, string firstName, string lastName, string street, string streetNr, string cityCode, string city, string company, string email, string tel, string fax, string status, string roles, long? id)
		{
			var restr = new Conjunction();
			if (showBusinessUser && !showTempUser)
				restr.Add(Restrictions.IsNotNull("BizUserId"));
			if (!showBusinessUser && showTempUser)
				restr.Add(Restrictions.IsNull("BizUserId"));
			_log.Debug("Start Searching for users.");

			// Generate restrictions
			if (!string.IsNullOrEmpty(salutation))
				restr.Add(Restrictions.InsensitiveLike("Salutation", "%" + salutation + "%"));
			if (!string.IsNullOrEmpty(firstName))
				restr.Add(Restrictions.InsensitiveLike("FirstName", "%" + firstName + "%"));
			if (!string.IsNullOrEmpty(lastName))
				restr.Add(Restrictions.InsensitiveLike("LastName", "%" + lastName + "%"));
			if (!string.IsNullOrEmpty(street))
				restr.Add(Restrictions.InsensitiveLike("Street", "%" + street + "%"));
			if (!string.IsNullOrEmpty(streetNr))
				restr.Add(Restrictions.InsensitiveLike("StreetNr", "%" + streetNr + "%"));
			if (!string.IsNullOrEmpty(cityCode))
				restr.Add(Restrictions.InsensitiveLike("CityCode", "%" + cityCode + "%"));
			if (!string.IsNullOrEmpty(city))
				restr.Add(Restrictions.InsensitiveLike("City", "%" + city + "%"));
			if (!string.IsNullOrEmpty(company))
				restr.Add(Restrictions.InsensitiveLike("Company", "%" + company + "%"));
			if (!string.IsNullOrEmpty(email))
				restr.Add(Restrictions.InsensitiveLike("Email", "%" + email + "%"));
			if (!string.IsNullOrEmpty(tel))
				restr.Add(Restrictions.InsensitiveLike("Tel", "%" + tel + "%"));
			if (!string.IsNullOrEmpty(fax))
				restr.Add(Restrictions.InsensitiveLike("Fax", "%" + fax + "%"));
			if (!string.IsNullOrEmpty(status))
				restr.Add(Restrictions.InsensitiveLike("bizUserAlias.UserStatus", status));
			if (!string.IsNullOrEmpty(roles))
				restr.Add(Restrictions.InsensitiveLike("bizUserAlias.Roles", "%" + roles + "%"));
			if (id.HasValue)
				restr.Add(Restrictions.InsensitiveLike("UserId", id));

			return restr;
		}


	    /// <summary>
	    /// Gets the users by surrogate filter.
	    /// </summary>
	    /// <param name="surrogateFilters">The surrogate filters.</param>
	    /// <param name="fields">The fields.</param>
	    /// <param name="maxCount">The maximal number of records returned.</param>
	    /// <param name="skip">Skip first n elements</param>
	    /// <param name="take">Take n elements</param>
	    /// <returns></returns>
	    public List<User> GetUsersBySurrogateFilterPaged(IEnumerable<string> surrogateFilters, IEnumerable<string> fields, int skip, int take)
	    {
            if (surrogateFilters == null)
                throw new ArgumentNullException("surrogateFilters");

            if (!surrogateFilters.Any())
                throw new ArgumentException("surrogateFilters parameter cannot be empty");

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var queuedCriterion = new List<AbstractCriterion>();

                    foreach (var filter in surrogateFilters)
                    {
                        var orDisjunction = Restrictions.Disjunction();

                        foreach (var field in fields)
                            orDisjunction.Add(Restrictions.InsensitiveLike(field, filter, MatchMode.Anywhere));

                        queuedCriterion.Add(orDisjunction);
                    }

                    var andConjunction = Restrictions.Conjunction();

                    foreach (var criterion in queuedCriterion)
                        andConjunction.Add(criterion);

                    return session.CreateCriteria(typeof(User))
                        .Add(andConjunction)
                        .SetMaxResults(take)
                        .SetFirstResult(skip)
                        .List<User>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("UserStore.GetUsersBySurrogateFilter({0}, {1}) failed",
                    string.Join(", ", surrogateFilters.ToArray()), string.Join(", ", fields.ToArray())), ex);
            }
        }
	}
}