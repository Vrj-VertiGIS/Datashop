using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Controls;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    public class WelcomePage : Page
    {
        #region Nonpublic properties

        protected Button btnLogin;

        protected ScriptManager scm;

        protected LinkButton cmdLoginAsTempUser;

        protected LinkButton cmdCreateBusinessUser;

        protected HtmlGenericControl divOccasionalUser;

        protected Literal litOptions;

        protected MultiView tempUserMltVw;

        #endregion

        #region Page Event Lifecycle

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.User.IsInRole("TEMP"))
            {
                FormsAuthentication.SignOut();
                // I was unable to find out how to logout an user without a trip to browser :-(
                //NOTE: you can't. think about it - FormsAuthentication.SignOut() deletes a cookie, so you have to delete the cookie in the response
                //NOTE: then you need to send a request back using the header redirect to let the site know there's no auth cookie any more. 
                Response.RedirectSafe("~/WelcomePage.aspx", false);
            }

            tempUserMltVw.ActiveViewIndex = (int)DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.DisplayMode;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.ProcessRequest();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        private void ProcessRequest()
        {
            this.HandleOccasionalUserDisabled();
        }

        /// <summary>
        /// Shows or hides the
        /// </summary>
        private void HandleOccasionalUserDisabled()
        {
            //NOTE: couldn't do this using the configuration API. It's possible to get the location node, but not the childnodes.
            var xDoc = XDocument.Load(Path.Combine(Request.PhysicalApplicationPath, "web.config"));

            var location = xDoc.Descendants()
                .Where(x => x.Name == "location")
                .Where(x => x.Attribute(XName.Get("path")).Value == "LoginTempUser.aspx")
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (location != null)
            {
                var allow = location.Descendants()
                    .Where(x => x.Name == "allow")
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();

                if (allow != null)
                {
                    var users = allow.Attribute(XName.Get("users"));

                    var occasionalUsersAllowed = (users != null && users.Value.Contains("*"));

                    litOptions.Visible = divOccasionalUser.Visible = occasionalUsersAllowed;
                }
                else
                    litOptions.Visible = divOccasionalUser.Visible = false;
            }
        }

        /// <summary>
        /// Handles the login.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        private void HandleLogin(long userID)
        {
            // clean session cookies for security reasons
            DatashopWeb.RemoveSessionCookies(Request, Response);

            var user = DatashopService.Instance.JobService.GetUser(userID);

            if (user != null && user.BizUser != null)
            {
                if (user.BizUser.UserStatus == BizUserStatus.LOCKED)
                {
                    FormsAuthentication.SignOut();
                    Response.RedirectSafe("error/BizUserLocked.aspx", true);
                    return;
                }

                if (user.BizUser.UserStatus == BizUserStatus.DISABLED)
                {
                    FormsAuthentication.SignOut();
                    Response.RedirectSafe("error/BizUserDisabled.aspx", true);
                    return;
                }

                var returnUrl = Request.QueryString["ReturnUrl"];

                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = "RequestPage.aspx";

                Response.RedirectSafe(returnUrl);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// CMDs the create business user click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void cmdCreateBusinessUserClick(object sender, EventArgs e)
        {
            var returnUrl = Request["ReturnUrl"];

            Response.RedirectSafe(string.IsNullOrEmpty(returnUrl) ? "RegisterBusinessUser.aspx" : "RegisterBusinessUser.aspx?ReturnUrl=" + returnUrl);
        }

        /// <summary>
        /// CMDs the login as temp user click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void cmdLoginAsTempUserClick(object sender, EventArgs e)
        {
            var returnUrl = Request["ReturnUrl"];

            Response.RedirectSafe(string.IsNullOrEmpty(returnUrl) ? "LoginTempUser.aspx" : "LoginTempUser.aspx?ReturnUrl=" + returnUrl);
        }

        /// <summary>
        /// Logins the mask on logged in.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LoginMaskOnLoggedIn(object sender, EventArgs e)
        {
            Login loginMask = sender as Login;

            long userID;

            if (long.TryParse(loginMask.UserName, out userID))
                this.HandleLogin(userID);
            else
            {
                var users = DatashopService.Instance.JobService.GetUserByEmail(loginMask.UserName);
                var user = users.SingleOrDefault(u => u.BizUser != null) ?? users.FirstOrDefault();

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UserId.ToString(), false); //set the auth cookie with the userId so the rest of the application still works

                    this.HandleLogin(user.UserId);
                }
                else
                    this.HandleLogin(-1); //MYP: I take full responsibility for this magic value WTF
            }
        }

        /// <summary>
        /// Logins the mask on login failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LoginMaskOnLoginFailed(object sender, EventArgs e)
        {
            var failureText = WebLanguage.LoadStr(2001, "Login failed. Please try again.");
            this.ShowMessage(failureText);

            ClearPassword(sender);
        }

        private void ClearPassword(object sender)
        {
            var login = sender as Login;
            if (login == null) return;

            var labelAndTextBox = login.FindControl("Password") as LabelAndTextBox;
            if (labelAndTextBox == null) return;

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                "clearPasswordScript",
                "document.getElementById('" + labelAndTextBox.TextBoxClientID + "').value =''",
                true);
        }

        #endregion
    }
}