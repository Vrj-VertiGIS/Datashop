using System;
using System.Web.UI;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    ///Code behind class for the Temporary user login page
    /// </summary>
    public partial class LoginTempUser : Page
    {
	    protected override void OnPreInit(EventArgs e)
	    {
		    base.OnPreInit(e);
		    var redirect = DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.DisplayMode == DisplayMode.WelcomePage;
			if(redirect)
				Response.RedirectSafe("WelcomePage.aspx");
	    }
    }
}