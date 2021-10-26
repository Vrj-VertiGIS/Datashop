using System.Web;
using System.Drawing.Imaging;
using System.Web.SessionState;

namespace GEOCOM.GNSD.Web.Core.Security.WebCaptcha
{
    /// <summary>
    /// Captcha image stream HttpModule. Retrieves CAPTCHA objects from cache, renders them to memory,
    /// and streams them to the browser.
    /// </summary>
    /// <remarks>
    /// You *MUST* enable this HttpHandler in your web.config, like so:
    /// &lt;httpHandlers&gt;
    /// &lt;add verb="GET" path="CaptchaImage.aspx" type="WebControlCaptcha.CaptchaImageHandler, WebControlCaptcha" /&gt;
    /// &lt;/httpHandlers&gt;
    /// Jeff Atwood
    /// http://www.codinghorror.com/
    /// </remarks>
    public class CaptchaImageHandler : IHttpHandler, IReadOnlySessionState
    {
        #region Public properties

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return true; }
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            //-- get the unique GUID of the captcha; this must be passed in via the querystring
            var guid = context.ApplicationInstance.Request.QueryString["guid"];

            CaptchaImage ci = null;

            if (!string.IsNullOrEmpty(guid))
            {
                if (string.IsNullOrEmpty(context.ApplicationInstance.Request.QueryString["s"]))
                    ci = (CaptchaImage)HttpRuntime.Cache.Get(guid);
                else
                    ci = (CaptchaImage)HttpContext.Current.Session[guid];
            }

            if (ci == null)
            {
                context.ApplicationInstance.Response.StatusCode = 404;
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            //-- write the image to the HTTP output stream as an array of bytes
            using (var b = ci.RenderImage())
                b.Save(context.Response.OutputStream, ImageFormat.Jpeg);

            context.ApplicationInstance.Response.ContentType = "image/jpeg";
            context.ApplicationInstance.Response.StatusCode = 200;
            context.ApplicationInstance.CompleteRequest();
        } 

        #endregion
    }
}