using System;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// This is the base class for the RequestPage - there used to be two pages, Plot and Pde
    /// </summary>
    public class RequestPage : Page
    {
        #region Private variables

        protected readonly IMsg _log = new Msg(typeof (RequestPage));
        
        private bool _isAdmin = false;

        protected PageFieldInfos _pageFieldInfos;
        
        protected User user = null;
        
        protected long _userId = -1;
        
        #endregion

        public RequestPage(PageFieldInfos pageFieldInfos)
        {
            this._pageFieldInfos = pageFieldInfos;
        }

        #region Public Properties
        public long UserId
        {
            get { return this._userId; }
        }

        public bool IsAdmin
        {
            get { return this._isAdmin; }
        }

        public string Language
        {
            get
            {
                return WebLanguage.Language;
            }
        }

        public string InitialMapExtent
        {
            get
            {
                if (DatashopWebConfig.Instance.MapService.InitialExtent == null)
                    return "null";

                var extent = DatashopWebConfig.Instance.MapService.InitialExtent.ToAnonymousType();

                return string.Format(" new esri.geometry.Extent({0})", new JavaScriptSerializer().Serialize(extent));
            }
        }

        #endregion

        #region Public Methods

        public bool IsFieldVisible(string id)
        {
            return IsFieldVisible(this._pageFieldInfos, id);
        }

        public bool IsFieldVisible(PageFieldInfos fieldInfos, string id)
        {
            var fieldInfo = GetFieldInfoById(fieldInfos, id);
            return fieldInfo != null && fieldInfo.Visible;
        }

        public PageFieldInfo GetFieldInfoById(string id)
        {
            return GetFieldInfoById(this._pageFieldInfos, id);
        }

        public PageFieldInfo GetFieldInfoById(PageFieldInfos fieldInfos, string id)
        {
            if (fieldInfos == null)
            {
                return null;
            }

            PageFieldInfo pageFieldInfo = fieldInfos.GetById(id);
            return pageFieldInfo;
        }

        public void SetFieldFromConfig(Control mostTopElementPlaceHolder)
        {
            SetFieldFromConfig(this._pageFieldInfos, mostTopElementPlaceHolder);
        }

        public void SetFieldFromConfig(PageFieldInfos fieldInfos, Control mostTopElementPlaceHolder)
        {
            if (fieldInfos == null)
            {
                return;
            }

            Utils.SetFieldByPageFieldInfos(mostTopElementPlaceHolder, fieldInfos.Fields);
        }

        public string GetPlotSectionsNames()
        {

            string plotSectionNames = string.Empty;
            const string separator = ", ";
            foreach (PlotSection plotSection in DatashopService.Instance.JobService.GetAllPlotSections())
            {
                plotSectionNames += plotSection.Name + separator;
            }
            plotSectionNames = plotSectionNames.TrimEnd(separator.ToCharArray());
            return plotSectionNames;
        }

        public string LoadStr(int key, string defaultValue)
        {
            return WebLanguage.LoadStr(key, defaultValue);
        }

        public void InitPage()
        {
            try
            {
                if (this._pageFieldInfos == null)
                {
                    throw new Exception("Input parameter pageFieldInfos cannot be null.");
                }
            }
            catch (Exception e)
            {
                this.LogError("RequestPage initialization failed. Reason:", e);
                Response.RedirectSafe("error/GeneralErrorPage.aspx", false);
                return;
            }
        }

        #endregion

        #region Protected Methods

        protected void CheckUser()
        {
            this.user = null;
            try
            {
                long.TryParse(User.Identity.Name, out this._userId);

                this.user = DatashopService.Instance.JobService.GetUser(this._userId);
            }
            catch (Exception exp)
            {
                // redirect if userID is not an ID
                this.LogError("Getting User failed", exp);
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
                return;
            }

            if (this._userId <= 0 || this.user == null)
            {
                Response.RedirectSafe("error/LoginErrorPage.aspx", false);
                return;
            }

            this._isAdmin = User.IsInRole("ADMIN");

            UserRequestMetaInfo userRequestMetaInfo = GetUserRequestMetaInfo(User, user);

            if (userRequestMetaInfo.UserRequestStatus != UserRequestStatus.RequestAllowed)
            {
                Redirect(userRequestMetaInfo);
            }
        }

        protected void LogError(string message, Exception ex)
        {
            HttpContext.Current.Session.Add("lasterror", (ex == null) ? new Exception(message) : ex);
            this._log.Error(message, ex);
        }

        protected void LogDebug(string message)
        {
            this._log.Debug(message);
        }

        protected void LogInfo(string message)
        {
            this._log.Info(message);
        }

        #endregion

        #region Private methods

        private UserRequestMetaInfo GetUserRequestMetaInfo(IPrincipal principalUser, User user)
        {
            UserRequestMetaInfo userRequestMetaInfo = new UserRequestMetaInfo(UserRequestStatus.RequestAllowed);
            if (principalUser.IsInRole("BUSINESS"))
            {
                userRequestMetaInfo = CheckBusinessUserStatus(principalUser, user);
            }
       
            return userRequestMetaInfo;
        }

        private UserRequestMetaInfo CheckBusinessUserStatus(IPrincipal principal, User user)
        {
            UserRequestMetaInfo userRequestMetaInfo = new UserRequestMetaInfo(UserRequestStatus.RequestAllowed);
            if (user.BizUser.UserStatus == BizUserStatus.LOCKED)
            {
                userRequestMetaInfo = new UserRequestMetaInfo(UserRequestStatus.BizUserLocked, "error/BizUserLocked.aspx");
            }
            if (user.BizUser.UserStatus == BizUserStatus.DISABLED)
            {
                userRequestMetaInfo = new UserRequestMetaInfo(UserRequestStatus.BizUserDisabled, "error/BizUserDisabled.aspx");
            }
            return userRequestMetaInfo;
        }


        private void Redirect(UserRequestMetaInfo userRequestMetaInfo)
        {
            if ((userRequestMetaInfo.UserRequestStatus == UserRequestStatus.BizUserDisabled) ||
                (userRequestMetaInfo.UserRequestStatus == UserRequestStatus.BizUserLocked))
            {
                FormsAuthentication.SignOut();
            }
            Response.RedirectSafe(userRequestMetaInfo.RedirectPage, false);
        }

        #endregion
    }
}