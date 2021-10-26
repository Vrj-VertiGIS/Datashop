using System;
using System.Web;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using Page = GEOCOM.GNSD.Web.Common.Page;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind class for the general error page
    /// </summary>
    public partial class GeneralErrorPage : Page
    {
        #region Protected Methods

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <returns></returns>
        protected string GetLastError()
        {
            var error = HttpContext.Current.Session["lasterror"];

            return error != null && error is Exception
                ? ((Exception)error).Message
                : WebLanguage.LoadStr(9052, "No error details available");
        } 

        #endregion
    }
}