using System;
using System.Configuration;

namespace GNSDatashopAdmin
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            Server.Transfer("WelcomePage.aspx");
        }
    }
}
