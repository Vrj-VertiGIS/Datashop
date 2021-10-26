using System;
using System.Web.UI;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.error
{
    public partial class TemplateLimitError : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty); // ensures that javascript methode  '__doPostback()' is enabled. Used for switching languages
            
            lblError.Text = String.Format(WebLanguage.LoadStr(10000, "Limit of requests for templates was reached."));
        }
    }
}