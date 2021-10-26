using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Security;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BizUserData : Page
    {
        #region Private Members

        /// <summary>
        /// Holds the logger for this page
        /// </summary>
        private IMsg log;

        /// <summary>
        /// the user object
        /// </summary>
        private User user;

        private BizUser _bizUser;

        private Guid? PasswordResetId
        {
            get
            {
                var isPasswordReset = Guid.TryParse(Request.QueryString["PasswordResetId"], out var passwordResetId);
                return isPasswordReset ? (Guid?)passwordResetId : null;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BizUserData"/> class.
        /// </summary>
        public BizUserData()
        {
            InitLogger();
        }

        #endregion

        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SetFieldFromConfig();
            if (Page.User.Identity.IsAuthenticated)
            {
                InitUserData();
                PopulateUIElements();
            }
            else if (PasswordResetId != null)
            {
                GetBizUserAndVerifyPasswordResetId();
                UserProfileForm.Visible = false;
                oldPassword.Visible = false;
            }
            else
            {
                Response.RedirectSafe(FormsAuthentication.LoginUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        #endregion

        #region Private Methods

        private void GetBizUserAndVerifyPasswordResetId()
        {
            var passwordResetId = PasswordResetId.Value;
            try
            {
                _bizUser = DatashopService.Instance.JobService.GetBizUserByPasswordResetId(passwordResetId);

                if (_bizUser == null || _bizUser.PasswordResetIdValidity < DateTime.Now)
                {
                    throw new AuthenticationException($"No biz user found for PasswordResetId='{passwordResetId}'");
                }
            }
            catch (Exception e)
            {
                log.Error($"Could not get the biz user for password reset id '{passwordResetId}'", e);
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
            }
        }

        private void PopulateUIElements()
        {
            if (!Page.IsPostBack)
            {
                if (salutation.DropDownItems.FindByValue(user.Salutation) == null)
                {
                    salutation.DropDownItems.Insert(1, user.Salutation);
                    salutation.SelectedIndex = 1;
                }
                else
                {
                    salutation.DropDownItems.FindByText(user.Salutation).Selected = true;
                }
                firstName.Text = user.FirstName;
                lastName.Text = user.LastName;
                email.Text = user.Email;
                street.Text = user.Street;
                streetNumber.Text = user.StreetNr;
                city.Text = user.City;
                zip.Text = user.CityCode;
                company.Text = user.Company;
                phone.Text = user.Tel;
                fax.Text = user.Fax;
            }
        }

        private void InitUserData()
        {
            long userID = -1;
            long.TryParse(Page.User.Identity.Name, out userID);
            if (user != null && user.UserId != userID)
            {
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
            }

            try
            {
                user = DatashopService.Instance.JobService.GetUser(userID);
            }
            catch (Exception exp)
            {
                log.Error("Could not get the user " + userID + " for BizUserData.aspx", exp);
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
            }
            if (user == null || user.BizUser == null)
            {
                log.InfoFormat("Could not get BizUser with userID {0} for BizUserData.aspx", userID);
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
            }
        }

        private void InitLogger()
        {
            try
            {
                if (log == null)
                {
                    DatashopLogInitializer.Initialize();
                    log = new Msg(GetType());
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }

        #endregion

        protected void SetFieldFromConfig()
        {
            Utils.SetFieldByPageFieldInfos(MostTopElementPlaceHolder, DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos.Fields);
        }

        #region Event Handlers

        /// <summary>
        /// Handles the click event of the BtnSaveChange control
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnSaveChangeClicked(object sender, EventArgs args)
        {
            if (!Page.IsValid)
            {
                return;
            }

            if (user == null || user.BizUser == null)
            {
                string msg = WebLanguage.LoadStr(2323, "Your account was not found. Please log out and then log in again.");
                this.ShowMessage(msg);
                return;
            }

            var users = DatashopService.Instance.JobService.GetUserByEmail(email.Text);
            bool emailAlreadyUsed = users.Where(u => u.BizUser != null && u.UserId != user.UserId).Count() > 0;
            if (emailAlreadyUsed)
            {
                string msg = WebLanguage.LoadStr(2317, "Username with specified email already exists");
                this.ShowMessage(msg);
                return;
            }

            user.Salutation = salutation.SelectedItem.Value;
            user.City = city.Text;
            user.CityCode = zip.Text;
            user.LastName = lastName.Text;
            user.FirstName = firstName.Text;
            //user.Email = email.Text; // do not update email to prevent the user guessing attack
            user.Street = street.Text;
            user.StreetNr = streetNumber.Text;
            user.Company = company.Text;
            user.Tel = phone.Text;
            user.Fax = fax.Text;

            try
            {
                DatashopService.Instance.JobService.UpdateUser(user);

                string message = WebLanguage.LoadStr(2321, "Your data were successfully changed.");
                this.ShowMessage(message);
            }
            catch (Exception)
            {
                string msg = WebLanguage.LoadStr(2322, "Change of user's data failed. Please retry later.");
                this.ShowMessage(msg);
            }
        }

        /// <summary>
        /// Handles the click event of the BtnChangePassword Control
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnChangePasswordClicked(object sender, EventArgs args)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var sec = new DatashopMembershipProvider();
            sec.Initialize(string.Empty, new NameValueCollection());
            BizUser bizUser;

            if (PasswordResetId == null)
            {
                log.InfoFormat("User {0} is trying to change his password from IP {1}", user.UserId, Request.Params["REMOTE_ADDR"]);
                bizUser = user.BizUser;
                if (!sec.ValidateUser(user.UserId.ToString(), oldPassword.Text))
                {
                    string message = WebLanguage.LoadStr(2324, "Old password is not correct.");
                    this.ShowMessage(message);
                    return;
                }
            }
            else
            {
                bizUser = _bizUser;
            }

            if (password.Text.Equals(oldPassword.Text, StringComparison.InvariantCulture))
            {
                string message = WebLanguage.LoadStr(2328, "The new password must be different that the old one.");
                this.ShowMessage(message);
                return;
            }

            if (!password.Text.Equals(passwordRepeated.Text, StringComparison.InvariantCulture))
            {
                string message = WebLanguage.LoadStr(2325, "The passwords do not match.");
                this.ShowMessage(message);
                return;
            }

            string salt = null;
            bizUser.Password = sec.TransformPassword(password.Text, ref salt);
            bizUser.PasswordSalt = salt;
            if (PasswordResetId != null)
            {
                bizUser.PasswordResetId = null;
                bizUser.PasswordResetIdValidity = null;
            }

            try
            {
                string message;
                message = DatashopService.Instance.JobService.UpdateBizUser(bizUser)
                    ? WebLanguage.LoadStr(2326, "Your password was successfully changed.")
                    : WebLanguage.LoadStr(2327, "Your password could not be changed. Please retry in few minutes.");


                this.ShowMessage(message);


                if (PasswordResetId != null)
                {
                    Response.RedirectSafe(FormsAuthentication.LoginUrl, false);
                    return;
                }

                if (Membership.ValidateUser(user.UserId.ToString(), password.Text))
                {
                    var ticket = FormsAuthentication.GetAuthCookie(user.UserId.ToString(), true);
                    ticket.Expires = DateTime.Now.AddMinutes(20);
                    Response.Cookies.Add(ticket);
                }
                else
                {
                    throw new Exception($"Authentication failed for user {user.UserId.ToString()}");
                }
            }
            catch (Exception exp)
            {
                string message = WebLanguage.LoadStr(2327, "Your password could not be changed. Please retry in few minutes.");
                this.ShowMessage(message);
                log.Error("User could not change his password.", exp);
            }
        }


        #endregion
    }
}