using System;
using System.Web.UI;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.error
{
    public partial class TempUserLimitError : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO fix the string.emptyies
            lblError.Text = String.Format(WebLanguage.LoadStr(9022, "Limit of {0} requests in {1} days was reached."), string.Empty, string.Empty);
        }
    }
}