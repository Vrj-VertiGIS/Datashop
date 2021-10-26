using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Security;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind class for the business user registration page
    /// </summary>
    public partial class RegisterBusinessUser : Page
    {
        #region Private members

        /// <summary>
        /// Holds the logger instance for this page
        /// </summary>
        private IMsg log = new Msg(typeof(RegisterBusinessUser));

        #endregion

        #region Page Event Lifecycle

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.EnableBtnRequestBasedOnAgb();
            }
	        captcha.Visible = !DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos.DisableCaptcha;
			captcha.Enabled = !DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos.DisableCaptcha;
            this.SetFieldFromConfig();
        } 

        #endregion

        /// <summary>
        /// Sets the field from config.
        /// </summary>
        protected void SetFieldFromConfig()
        {
            Utils.SetFieldByPageFieldInfos(MostTopElementPlaceHolder, DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos.Fields);
        }

        #region Private methods

        /// <summary>
        /// Enables the BTN request based on agb.
        /// </summary>
        private void EnableBtnRequestBasedOnAgb()
        {
            var agbFieldInfo = DatashopWebConfig.Instance.GetRegisterBusinessUserPageFieldInfoById("agb");
            var pdsFieldInfo = DatashopWebConfig.Instance.GetRegisterBusinessUserPageFieldInfoById("pds");

            var acceptedAll = (agb.Checked || (agbFieldInfo != null && !agbFieldInfo.Visible)) &&
                              (pds.Checked || (pdsFieldInfo != null && !pdsFieldInfo.Visible));

            btnAccept.Enabled = acceptedAll;
        }

        /// <summary>
        /// Matches the password.
        /// </summary>
        /// <returns></returns>
        private bool PasswordsMatch()
        {
            return password.Text.Equals(passwordRepeated.Text, StringComparison.InvariantCulture);
        }

        #endregion

        #region Event handlers

        public void UniqueUserValidator(object source, ServerValidateEventArgs args)
        {
            var users = DatashopService.Instance.JobService.GetUserByEmail(email.Text);

            var businessUserAlreadyExists = users.Any(u => u.BizUser != null);
            if (businessUserAlreadyExists)
            {
                args.IsValid = false;
                var msg = WebLanguage.LoadStr(2317, "Username with specified email already exists");
                this.ShowMessage(msg);
            }
        }

        /// <summary>
        /// Handles the OnCheckedChanged event of the agb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void agb_OnCheckedChanged(object sender, EventArgs args)
        {
            this.EnableBtnRequestBasedOnAgb();
        }

        /// <summary>
        /// BTNs the accept clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnAcceptClicked(object sender, EventArgs args)
        {
            if (!PasswordsMatch())
            {
                this.ShowMessage(WebLanguage.LoadStr(2314, "The passwords do not match."));
                return;
            }

            if (!Page.IsValid)
            {
				if (!DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos.DisableCaptcha && !this.captcha.IsValid)
                    this.ShowMessage(this.captcha.ErrorMessage);

                return;
            }

            string redirectOnError = "error/GeneralErrorPage.aspx";

            try
            {
              string salt = null;

                var sec = new DatashopMembershipProvider();
                sec.Initialize("", new NameValueCollection());

                var newBizUser = new BizUser
                                     {
                                         Password = sec.TransformPassword(password.Text, ref salt),
                                         PasswordSalt = salt,
                                         Roles = "BUSINESS",
                                         UserStatus = BizUserStatus.LOCKED
                                     };

                var newUser = new User
                                  {
                                      Salutation = salutation.SelectedItem.Text,
                                      City = city.Text,
                                      CityCode = zip.Text,
                                      Email = email.Text,
                                      LastName = lastName.Text,
                                      FirstName = firstName.Text,
                                      Street = street.Text,
                                      StreetNr = streetNumber.Text,
                                      Company = company.Text,
                                      Tel = phone.Text,
                                      Fax = fax.Text
                                  };

                var newID = DatashopService.Instance.JobService.CreateBizUserAndSendAdminMail(newBizUser, newUser);

                if (newID <= 0)
                {
                    redirectOnError = "error/LoginErrorPage.aspx";

                    throw new Exception("Creating new bizUser failed");
                }

                FormsAuthentication.SignOut();

                if (Membership.ValidateUser(newID.ToString(), password.Text))
                {
                    var ticket = FormsAuthentication.GetAuthCookie(newID.ToString(), true);
                    ticket.Expires = DateTime.Now.AddMinutes(20);
                    Response.Cookies.Add(ticket);
                }
                else
                {
                    redirectOnError = "error/LoginErrorPage.aspx";
                    throw new Exception(string.Format("Authentication failed for user {0}", newID));
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["lasterror"] = ex;
                log.Error(ex.Message, ex);
                Response.RedirectSafe(redirectOnError, false);
            }

            var returnUrl = Request["ReturnUrl"];

            Response.RedirectSafe(string.IsNullOrEmpty(returnUrl) ? DatashopWebConfig.Instance.DefaultRequestPage.PageName : returnUrl, false);
        } 

        #endregion
    }
}