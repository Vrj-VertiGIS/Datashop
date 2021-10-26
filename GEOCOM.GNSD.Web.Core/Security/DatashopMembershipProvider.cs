using System.Collections.Specialized;
using System.Security;
using System.Web.Security;
using System;
using System.Linq;
using GEOCOM.Common.Logging;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSD.Web.Core.Security
{
    public class DatashopMembershipProvider : MembershipProvider
    {

        // log4net
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        private string _Name;
        private string _FileName;
        private string _ApplicationName;
        private bool _EnablePasswordReset;
        private bool _RequiresQuestionAndAnswer;
        private string _PasswordStrengthRegEx;
        private int _MaxInvalidPasswordAttempts;
        private int _MinRequiredNonAlphanumericChars;
        private int _MinRequiredPasswordLength;
        private MembershipPasswordFormat _PasswordFormat;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "DatashopMembershipProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Datashop Membership Provider");
            }

            // Initialize the base class
            base.Initialize(name, config);

            // Initialize default values
            _ApplicationName = "DefaultApp";
            _EnablePasswordReset = false;
            _PasswordStrengthRegEx = @"";
            _MaxInvalidPasswordAttempts = 3;
            _MinRequiredNonAlphanumericChars = 0;
            _MinRequiredPasswordLength = 3;
            _RequiresQuestionAndAnswer = false;
            _PasswordFormat = MembershipPasswordFormat.Hashed;

            // Now go through the properties and initialize custom values
            foreach (string key in config.Keys)
            {
                switch (key.ToLower())
                {
                    case "name":
                        _Name = config[key];
                        break;
                    case "applicationname":
                        _ApplicationName = config[key];
                        break;
                    case "filename":
                        _FileName = config[key];
                        break;
                    case "enablepasswordreset":
                        _EnablePasswordReset = bool.Parse(config[key]);
                        break;
                    case "passwordstrengthregex":
                        _PasswordStrengthRegEx = config[key];
                        break;
                    case "maxinvalidpasswordattempts":
                        _MaxInvalidPasswordAttempts = int.Parse(config[key]);
                        break;
                    case "minrequirednonalphanumericchars":
                        _MinRequiredNonAlphanumericChars = int.Parse(config[key]);
                        break;
                    case "minrequiredpasswordlength":
                        _MinRequiredPasswordLength = int.Parse(config[key]);
                        break;
                    case "passwordformat":
                        _PasswordFormat = (MembershipPasswordFormat)Enum.Parse(
                                                                         typeof(MembershipPasswordFormat), config[key]);
                        break;
                    case "requiresquestionandanswer":
                        _RequiresQuestionAndAnswer = bool.Parse(config[key]);
                        break;
                }
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

        public override bool EnablePasswordReset
        {
            get { return _EnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                if (this.PasswordFormat == MembershipPasswordFormat.Hashed)
                    return false;
                else
                    return true;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _MaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _MinRequiredNonAlphanumericChars; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _MinRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            // 20 minutes until resetting MaxInvalidPasswordAttempts counter
            get { return 20; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _PasswordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _PasswordStrengthRegEx; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _RequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public override MembershipUser CreateUser(string username, string password,
                                                  string email, string passwordQuestion,
                                                  string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            try
            {
                // Validate the username and email
                if (!ValidateUsername(username, email))
                {
                    status = MembershipCreateStatus.InvalidUserName;
                    return null;
                }

                // Raise the event before validating the password
                base.OnValidatingPassword(
                    new ValidatePasswordEventArgs(
                        username, password, true));

                // Validate the password
                if (!ValidatePassword(password))
                {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }

                //Everything is valid, create the user
                try
                {
                    // get a new webservice proxy to connect to the jobmanager 
                    IJobManager jobMgrService = DatashopService.Instance.JobService;
                    BizUser bizUser = new BizUser();
                    // user.Email = username;
                    bizUser.Roles = "";

                    // transform passwort to hash and add salt to db
                    string salt = bizUser.PasswordSalt;
                    bizUser.Password = TransformPassword(password, ref salt);
                    bizUser.PasswordSalt = salt;

                    // create a new user as well
                    User user = new User();
                    user.Email = username;

                    // Add the user to db
                    long id = jobMgrService.CreateBizUserAndSendAdminMail(bizUser, user);
                    if (id >= 0)
                    {
                        bool isLockedOut = true;

                        // TODO creating a new membershipuser throws an error. Since we don't use that returnvalue, we can 
                        // ignore it
                        MembershipUser membershipUser = null;
                        try
                        {
                            membershipUser = new MembershipUser("CustomMembershipProvider", username,
                                                                               username, email, passwordQuestion, "",
                                                                               isApproved, isLockedOut,
                                                                               DateTime.Now, DateTime.Now, DateTime.Now,
                                                                               DateTime.Now, DateTime.Now);
                        }
                        catch (Exception)
                        {
                        }
                        status = MembershipCreateStatus.Success;
                        return membershipUser;
                    }
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error(
                        string.Format("Create user failed for user {0:s} with error {1:s}", username, ex.Message), ex);
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }
                ;

            }
            catch (Exception ex)
            {
                _log.Error(string.Format("CreateUser {0:s}", ex.Message), ex);
                throw;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return true;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {

            return GetUser((string)providerUserKey, userIsOnline);

        }

        public override string GetUserNameByEmail(string email)
        {

            return email;

        }

        public override void UpdateUser(MembershipUser user)
        {

        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                // accept userid as number as well
                long userid;

                if (!long.TryParse(username, out userid))
                {
                    var users = DatashopService.Instance.JobService.GetUserByEmail(username);

                    var emailLoginUser = users.FirstOrDefault(u => u.BizUser != null);

                    if (emailLoginUser != null)
                        userid = emailLoginUser.UserId;
                    else
                        return false;
                }

                var user = DatashopService.Instance.JobService.GetUser(userid);

                if (user == null)
                    return false;

                if (user.BizUser == null && string.IsNullOrEmpty(password))
                    return true; //TempUserLogin

                // Biz User
                var userLoginBlocked = DateTime.Now <= (user.BizUser.BlockedUntil ?? DateTime.MinValue);
                if (userLoginBlocked)
                {
                    return false;
                }

                bool userValid = this.ValidateUserInternal(user.BizUser, password);
                if (!userValid)
                {
                    DatashopService.Instance.JobService.SetFailedLogin(userid);
                }
                else
                {
                    DatashopService.Instance.JobService.ResetFailedLogin(userid);
                }

                return userValid;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ValidateUser {0:s}", ex.Message), ex);

                throw new SecurityException("Uservalidation", ex);
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            try
            {
                throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
            }
            catch
            {
                throw;
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException("ChangePasswordQuestionAndAnswer");
        }

        public override bool UnlockUser(string userName)
        {
            // This provider doesn't support locking
            return true;
        }

        #endregion

        #region Private Helper Methods

        public string TransformPassword(string password, ref string salt)
        {
            string ret = string.Empty;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    ret = password;
                    break;

                case MembershipPasswordFormat.Hashed:

                    // Generate the salt if not passed in
                    if (string.IsNullOrEmpty(salt))
                    {
                        byte[] saltBytes = new byte[16];
                        RandomNumberGenerator rng = RandomNumberGenerator.Create();
                        rng.GetBytes(saltBytes);
                        salt = Convert.ToBase64String(saltBytes);
                    }
                    ret = FormsAuthentication.HashPasswordForStoringInConfigFile(
                                                            (salt + password), "SHA1");
                    break;

                case MembershipPasswordFormat.Encrypted:
                    byte[] ClearText = Encoding.UTF8.GetBytes(password);
                    byte[] EncryptedText = base.EncryptPassword(ClearText);
                    ret = Convert.ToBase64String(EncryptedText);
                    break;
            }

            return ret;
        }

        private bool ValidateUsername(string userName, string email)
        {
            bool IsValid = true;

            return IsValid;
        }

        private bool ValidatePassword(string password)
        {
            bool IsValid = true;
            Regex HelpExpression;

            // Validate simple properties
            IsValid = IsValid && (password.Length >= this.MinRequiredPasswordLength);

            // Validate non-alphanumeric characters
            HelpExpression = new Regex(@"\W");
            IsValid = IsValid && (HelpExpression.Matches(password).Count >= this.MinRequiredNonAlphanumericCharacters);

            // Validate regular expression
            if (!string.IsNullOrEmpty(PasswordStrengthRegularExpression))
            {
                HelpExpression = new Regex(this.PasswordStrengthRegularExpression);
                IsValid = IsValid && (HelpExpression.Matches(password).Count > 0);
            }
            return IsValid;
        }

        private bool ValidateUserInternal(BizUser user, string password)
        {
            if (user != null)
            {
                string salt = user.PasswordSalt;
                string passwordValidate = TransformPassword(password, ref salt);
                user.PasswordSalt = salt;
                if (string.Compare(passwordValidate, user.Password) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private MembershipUser CreateMembershipFromInternalUser(BizUser user)
        {
            return null;
        }

        private MembershipUserCollection CreateMembershipCollectionFromInternalList(List<BizUser> users)
        {
            MembershipUserCollection ReturnCollection = new MembershipUserCollection();

            foreach (BizUser user in users)
            {
                ReturnCollection.Add(CreateMembershipFromInternalUser(user));
            }

            return ReturnCollection;
        }

        #endregion
    }

    //    }
}