using System;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind class for the default page
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.RedirectSafe(DatashopWebConfig.Instance.DefaultRequestPage.PageName, false); 
        }
    }
}