using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Page = GEOCOM.GNSD.Web.Common.Page;

namespace GEOCOM.GNSD.Web
{
    public partial class ServiceNotFound : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty); // ensures that javascript methode  '__doPostback()' is enabled. Used for switching languages
        }
    }
}
