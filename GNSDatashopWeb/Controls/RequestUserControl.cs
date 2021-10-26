namespace GEOCOM.GNSD.Web.Controls
{
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;
    using GEOCOM.Common.Logging;
    using GEOCOM.GNSD.Common.Logging;
    using GEOCOM.GNSD.Web.Config;

    /// <summary>
    /// This is the base class for any UserControl that lives only within a RequestPage.
    /// This exposes most of the RequestPage features.
    /// Please use "BasePage" and not "this.Page".
    /// </summary>
    public class RequestUserControl : UserControl
    {       
        #region Private variables
        private IMsg _log;
        #endregion

        public RequestUserControl()
        {
            this.InitLogger();
        }

        #region BasePage Properties

        protected long UserId
        {
            get { return this.BasePage.UserId; }
        }

        private RequestPage BasePage
        {
            get { return (RequestPage)this.Page; }
        }

        #endregion

        #region BasePage Methods

        protected bool IsFieldVisible(string id)
        {
            return this.BasePage.IsFieldVisible(id);
        }

        protected bool IsFieldVisible(PageFieldInfos fieldInfos, string id)
        {
            return this.BasePage.IsFieldVisible(fieldInfos, id);
        }

        protected PageFieldInfo GetFieldInfoById(string id)
        {
            return this.BasePage.GetFieldInfoById(id);
        }

        protected PageFieldInfo GetFieldInfoById(PageFieldInfos fieldInfos, string id)
        {
            return this.BasePage.GetFieldInfoById(fieldInfos, id);
        }

        protected void ShowMessage(string message)
        {
            this.BasePage.ShowMessage(message);
        }

        protected void SetFieldFromConfig(Control mostTopElementPlaceHolder)
        {
            this.BasePage.SetFieldFromConfig(mostTopElementPlaceHolder);
        }

        protected void SetFieldFromConfig(PageFieldInfos fieldInfos, Control mostTopElementPlaceHolder)
        {
            this.BasePage.SetFieldFromConfig(fieldInfos, mostTopElementPlaceHolder);
        }

        protected string LoadStr(int key, string defaultValue)
        {
            return this.BasePage.LoadStr(key, defaultValue);
        }

        #endregion

        #region Logger

        protected void LogError(string message, Exception ex)
        {
            HttpContext.Current.Session.Add("lasterror", (ex == null) ? new Exception(message) : ex);
            this._log.Error(message, ex);
        }

        protected void LogDebug(string message)
        {
            this._log.Debug(message);
        }

        private void InitLogger()
        {
            try
            {
                if (this._log == null)
                {
                    DatashopLogInitializer.Initialize();
                    this._log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }
        #endregion
    }
}
