using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSD.Web.Core.Security
{
    public class DatashopRoleProvider : RoleProvider
    {
        // log4net
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        private string _ApplicationName;

        private List<string> _roles = new List<string> { "GUEST", "ADMIN", "TEMP", "BUSINESS", "REPRESENTATIVE" };

        public override void Initialize(string name, NameValueCollection config)
        {
            DatashopLogInitializer.Initialize();

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "DatashopRoleProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Datashop Role Provider");
            }

            // Base initialization
            base.Initialize(name, config);

            // Initialize properties
            _ApplicationName = "GEONIS server Datashoop";
            foreach (string key in config.Keys)
            {
                if (key.ToLower().Equals("applicationname"))
                    ApplicationName = config[key];
                //else if (key.ToLower().Equals("filename"))
                //    _FileName = config[key];
            }
        }

        #region Properties

        public override string ApplicationName
        {
            get { return _ApplicationName; }
            set
            {
                _ApplicationName = value;
                //_CurrentStore = null;
            }
        }

        #endregion

        #region Methods

        public override void CreateRole(string roleName)
        {
            try
            {
                // ignore, there are only the hardcoded roles accepted
                throw new Exception("Adding new roles is not allowed");
            }
            catch
            {
                throw;
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            try
            {
                throw new Exception("Deleting roles is not allowed");
                //SimpleRole Role = CurrentStore.GetRole(roleName);
                //if (Role != null)
                //{
                //    CurrentStore.Roles.Remove(Role);
                //    CurrentStore.Save();
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch
            {
                throw;
            }
        }

        public override bool RoleExists(string roleName)
        {
            try
            {
                if (_roles.Contains(roleName))
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            //try
            //{
            //    // get a new webservice proxy to connect to the jobmanager 
            //    JobManagerSoapClient jobMgrService = DatashopService.JobManagerWebservice;

            //    // Get the roles to be modified
            //    foreach (string roleName in roleNames)
            //    {
            //        if (_roles.Contains(roleName))
            //        {
            //            foreach (string userName in usernames)
            //            {
            //                GNSD_Biz_User user = jobMgrService.GetBizUserByEmail(userName);
            //                if (user != null)
            //                {
            //                    List<string> roles = new List<string>(user.Roles.Split(','));
            //                    if (!roles.Contains(roleName))
            //                    {
            //                        user.Roles = ToCommaString(roles);
            //                        jobMgrService.UpdateBizUser(user);
            //                    }
            //                }
            //                else throw new Exception(string.Format("AddUsersToRoles did not find user {0:s}", userName));
            //            }
            //        }
            //    }

            //}
            //catch
            //{
            //    throw;
            //}
        }

        /// <summary>
        /// Makes a string to store the Roles in DB
        /// </summary>
        /// <param name="roles">List of roles</param>
        /// <returns>RoleA, RoleB, RoleC</returns>
        private static string ToCommaString(List<string> roles)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string role in roles)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(',');
                }

                sb.Append(role);
            }

            string res = sb.ToString();
            return res;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                // Get the roles to be modified
                foreach (string roleName in roleNames)
                {
                    if (_roles.Contains(roleName))
                    {
                        foreach (string userName in usernames)
                        {
                            //if (Role.AssignedUsers.Contains(userName))
                            //{
                            //    Role.AssignedUsers.Remove(userName);
                            //}
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public override string[] GetAllRoles()
        {
            try
            {
                return _roles.ToArray();
            }
            catch
            {
                throw;
            }
        }

        public override string[] GetRolesForUser(string username)
        {

            // get a new webservice proxy to connect to the jobmanager 
            IJobManager jobMgrService = DatashopService.Instance.JobService;

            long userID = 0;
            User user = null;
            if (long.TryParse(username, out userID))
            {
                user = DatashopService.Instance.JobService.GetUser(userID);
            }
            else
            {
                var users = DatashopService.Instance.JobService.GetUserByEmail(username);

                user = users.FirstOrDefault();
            }

            if (user != null)
            {
                if (user.BizUser != null)
                {
                    var users = user.BizUser.Roles.Split(',');
                    for (int i = 0; i < users.Length; i++)
                    {
                        users[i] = users[i].Trim();
                    }
                    return users;
                }
                else if (Membership.ValidateUser(username, ""))
                {
                    return new string[] { "TEMP" };
                }
            }
            else
            {
                //// hard coded guest account for WWZ removed
                //if (username.Equals(DatashopService.Config.GuestLogin.Username,
                //                    StringComparison.InvariantCultureIgnoreCase))
                //{
                //    return new string[] {"GUEST"};
                //}

                throw new SecurityException(string.Format("User {0:s} does not exist", username));
            }

            return new string[] { "" };
        }

        public override string[] GetUsersInRole(string roleName)
        {

            List<string> result = new List<string>();

            // get a new webservice proxy to connect to the jobmanager 
            IJobManager jobMgrService = DatashopService.Instance.JobService;

            foreach (User user in jobMgrService.GetAllUsers())
                if (user.BizUser != null)
                    if (user.BizUser.Roles.Contains(roleName))
                    {
                        result.Add(user.Email);
                    }
            return result.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                if (_roles.Contains(roleName))
                {
                    //// hard coded guest account for WWZ removed
                    //if (username.Equals(DatashopService.Config.GuestLogin.Username,
                    //                    StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    if (roleName.Equals("GUEST", StringComparison.InvariantCultureIgnoreCase))
                    //    {
                    //        return true;
                    //    }
                    //}

                    return new List<string>(GetRolesForUser(username)).Contains(roleName);
                }
                else
                {
                    _log.ErrorFormat("Role={0} for UserName={1} doesn't exist", roleName, username);
                    throw new ProviderException("Role does not exist!");
                }
            }
            catch
            {
                throw;
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            try
            {
                List<string> Results = new List<string>();
                Regex Expression = new Regex(usernameToMatch.Replace("%", @"\w*"));
                if (_roles.Contains(roleName))
                {
                    //foreach (string userName in Role.AssignedUsers)
                    //{
                    //    if (Expression.IsMatch(userName))
                    //        Results.Add(userName);
                    //}
                }
                else
                {
                    throw new ProviderException("Role does not exist!");
                }

                return Results.ToArray();
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}