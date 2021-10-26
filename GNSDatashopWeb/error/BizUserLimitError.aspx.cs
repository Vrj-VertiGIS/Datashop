using System;
using System.Web.UI;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.error
{
    public partial class BizUserLimitError : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty); // ensures that javascript methode  '__doPostback()' is enabled. Used for switching languages            
            // TODO fix the string.emptyies
            lblError.Text = String.Format(WebLanguage.LoadStr(9020, "Limit of {0} requests in {1} days was reached."), string.Empty, string.Empty);
        }
    }
}