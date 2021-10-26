using System.Web;

namespace GEOCOM.GNSD.Web.Common
{
    public class RefreshModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += new System.EventHandler(OnAcquireRequestState);
        }

        public void Dispose()
        { 
        }

        void OnAcquireRequestState(object sender, System.EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext ctx = app.Context;
            RefreshAction.Check(ctx);
            return;
        }
       

    }
}