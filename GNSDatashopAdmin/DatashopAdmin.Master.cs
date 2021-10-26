using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using GEOCOM.GNSD.Web.Core.ServerControls;
using GEOCOM.GNSD.Web.Core.Service;

namespace GNSDatashopAdmin
{
    public partial class DatashopAdmin : MasterPage
    {
        protected string Version
        {
            get { return this.GetVersion(); }
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var adminLoggedIn = HttpContext.Current.User.IsInRole("ADMIN");

            this.liSeparator.Visible = this.LoginStatus1.Visible = adminLoggedIn;

            if(adminLoggedIn)
                this.LoginStatus1.Attributes.Add("onClick", "return confirm('Logout?')");
        }

        protected void LoginStatus1_OnLoggedOut(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            Response.Redirect("~/WelcomePage.aspx", false);
        }

        //HACK: This is used to change the LoginName from the Userid to the email adress.
        protected void LoginName1_PreRender(object sender, EventArgs e)
        {
            var login = sender as System.Web.UI.WebControls.LoginName;
            if (login == null)
                return;

            //test if its a valid userID (a number not a name)
            long userID;
            if (long.TryParse(Page.User.Identity.Name, out userID))
            {
                try
                {
                    var user = DatashopService.Instance.JobService.GetUser(userID); //will throw nhibernateException if no user is found
                    if (user != null)
                    {
                        login.FormatString = "Logged in as " + user.Email;
                        //MainNavigation.Visible = true;
                    }
                }
                catch
                {
                    login.FormatString = "Unknown userId: " + "userId";
                }
                return;
            }
            //ID parsing failed, use default
            login.FormatString = "Logged in as {0}";
        }

        private static readonly string _version = ((AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Single()).InformationalVersion;
        private string GetVersion() => _version;
    }
}