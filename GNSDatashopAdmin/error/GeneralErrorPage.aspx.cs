using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace GNSDatashopAdmin.error
{
    public partial class GeneralErrorPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty); // ensures that javascript methode  '__doPostback()' is enabled. Used for switching languages
        }

        protected object ShowLastError()
        {
            object error = HttpContext.Current.Session["lasterror"];
            if(error != null && error is Exception)
            {
                Exception e = (Exception) error;
                return e.Message;
            }
            return "Keine Fehlermeldung verfügbar";
        }
    }
}