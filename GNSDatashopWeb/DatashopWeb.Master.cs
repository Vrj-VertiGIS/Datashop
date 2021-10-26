using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.WebControls;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind for the Datashop MasterPage
    /// </summary>
    public partial class DatashopWeb : MasterPage
    {
        protected string Protocol;

        protected string Version
        {
            get { return this.GetVersion(); }
        }

        public bool UserInBusinessRole
        {
            get { return Page.User.IsInRole("BUSINESS"); }
        }

        public bool UserAuthenticated
        {
            get { return Page.User.Identity.IsAuthenticated; }
        }

        #region Page Event Lifecycle

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            this.languageSelector.LanguageInfoDefinitions.AddRange(DatashopWebConfig.Instance.Languages.Languages
                                                                   ?? new LanguageInfo[] { });
            PerformAntiXSRFCheck();
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();
        }

        #endregion

        private static readonly string _version = ((AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Single()).InformationalVersion;
        private string GetVersion() => _version;

        /// <summary>
        /// Called when [ajax error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.AsyncPostBackErrorEventArgs"/> instance containing the event data.</param>
        protected void OnAjaxError(object sender, AsyncPostBackErrorEventArgs args)
        {
            var message = "<br /><b>An error occured</b> <br />";

            if (!string.IsNullOrEmpty(DatashopWebConfig.Instance.Debug) && DatashopWebConfig.Instance.Debug.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                message += args.Exception.Message;

            scriptManager.AsyncPostBackErrorMessage = message;
        }

        /// <summary>
        /// Called when [logged out].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnLoggedOut(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            // clean session cookies for security reasons
            RemoveSessionCookies(Request, Response);
        }

        /// <summary>
        /// Removes the session id and and xsrf cookies from the browser.
        /// </summary>
        public static void RemoveSessionCookies(HttpRequest Request, HttpResponse Response)
        {
            void RemoveCookieFromBrowser(string cookieName)
            {
                if (Request.Cookies[cookieName] == null) return;
                Response.Cookies[cookieName].Value = string.Empty;
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMonths(-20);
            }

            // default is ASP.NET_SessionId
            var aspSessionIdCookie =((SessionStateSection) ConfigurationManager.GetSection("system.web/sessionState")).CookieName;
            RemoveCookieFromBrowser(aspSessionIdCookie);
            RemoveCookieFromBrowser(AntiXsrfTokenKey);
        }

        #region Anti Cross site request forgery 

        // The code below originates from https://stackoverflow.com/a/29962060/502042

        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected void PerformAntiXSRFCheck()
        {
            //First, check for the existence of the Anti-XSS cookie
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;

            //If the CSRF cookie is found, parse the token from the cookie.
            //Then, set the global page variable and view state user
            //key. The global variable will be used to validate that it matches 
            //in the view state form field in the Page.PreLoad method.
            if (requestCookie != null
                && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                //Set the global token variable so the cookie value can be
                //validated against the value in the view state form field in
                //the Page.PreLoad method.
                _antiXsrfTokenValue = requestCookie.Value;

                //Set the view state user key, which will be validated by the
                //framework during each request
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            //If the CSRF cookie is not found, then this is a new session.
            else
            {
                //Generate a new Anti-XSRF token
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");

                //Set the view state user key, which will be validated by the
                //framework during each request
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                //Create the non-persistent CSRF cookie
                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    //Set the HttpOnly property to prevent the cookie from
                    //being accessed by client side script
                    HttpOnly = true,

                    //Add the Anti-XSRF token to the cookie value
                    Value = _antiXsrfTokenValue,

                    SameSite = SameSiteMode.Strict
                };

                //If we are using SSL, the cookie should be set to secure to
                //prevent it from being sent over HTTP connections
                if (FormsAuthentication.RequireSSL &&
                    Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }

                //Add the CSRF cookie to the response
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }
        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            //During the initial page load, add the Anti-XSRF token and user
            //name to the ViewState
            if (!IsPostBack)
            {
                //Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;

                //If a user name is assigned, set the user name
                ViewState[AntiXsrfUserNameKey] =
                    Context.User.Identity.Name ?? String.Empty;
            }
            //During all subsequent post backs to the page, the token value from
            //the cookie should be validated against the token in the view state
            //form field. Additionally user name should be compared to the
            //authenticated users name
            else
            {
                //Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] !=
                    (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of " +
                                                        "Anti-XSRF token failed.");
                }
            }
        }


        #endregion
    }
}