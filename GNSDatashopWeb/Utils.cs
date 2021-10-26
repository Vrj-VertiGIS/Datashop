using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.ServerControls;
using System;
using System.Web;
using System.Web.UI;

namespace GEOCOM.GNSD.Web
{
    public static class Utils
    {
        public static void SetFieldByPageFieldInfos(Control mostTopParentControl, PageFieldInfo[] fieldInfos)
        {
            if (mostTopParentControl == null || fieldInfos == null)
                return;

            foreach (PageFieldInfo pageFieldInfo in fieldInfos)
            {
                Control control = mostTopParentControl.FindControl(pageFieldInfo.ServerId);
                // wia : we allow now to splitthe controls in different user controls.
                if (control != null)
                {
                    if (control is ValidatedControl)
                    {
                        var validatedControl = control as ValidatedControl;
                        validatedControl.DataBind(pageFieldInfo);
                    }
                }
            }

            foreach (Control control in mostTopParentControl.Controls)
            {
                SetFieldByPageFieldInfos(control, fieldInfos);
            }
        }

        public static DateTime? ParseDate(string dateString)
        {
            if (String.IsNullOrEmpty(dateString))
                return null;

            DateTime date = Convert.ToDateTime(dateString);
            return date;
        }


        /// <summary>
        /// Show the message via a javascript and html dialog. The passed message is JavaScript encoded.
        /// </summary>
        public static void ShowMessage(this Page page, string message)
        {
            Page currentPage = page ?? HttpContext.Current.Handler as Page;
            if (currentPage != null)
            {
                ScriptManager.RegisterStartupScript(
                    currentPage,
                    currentPage.GetType(),
                    "msgJS",
                    "showMessage('" + HttpUtility.JavaScriptStringEncode(message) + "');",
                    true);
            }
        }

        public static void RedirectSafe(this HttpResponse response, string url, bool endResponse = false)
        {
            bool isAbsoluteUrl = url?.StartsWith("http") ?? true;
            if (isAbsoluteUrl)
            {
                response.Redirect("~", endResponse);
            }
            else
            {
                response.Redirect(url, endResponse);
            }
        }
    }
}