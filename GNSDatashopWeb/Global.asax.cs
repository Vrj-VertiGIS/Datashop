using System;
using System.Reflection;
using System.Web;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;

namespace GEOCOM.GNSD.Web
{
    public class Global : HttpApplication
    {
        private IMsg _log;
        
        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            this.InitLogger();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs args)
        {
            Exception e= Server.GetLastError();
            string message =
                string.Format(
                    "Global.asax -  exception:\n type = {0};\n message = {1};\n source = {2};\n stack trace = {3};\n",
                    e.GetType(), e.Message, e.Source, e.StackTrace);
            _log.Log(MsgLevel.Error, message);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private void InitLogger()
        {
            try
            {
                if (_log == null)
                {
                    DatashopLogInitializer.Initialize();
                    _log = new Msg(typeof(Global));
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }
    }
}