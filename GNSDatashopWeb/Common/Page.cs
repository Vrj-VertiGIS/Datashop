using System;
using System.Web;

namespace GEOCOM.GNSD.Web.Common
{
    /// <summary>
    /// Another BasePage class..
    /// </summary>
    public class Page : System.Web.UI.Page
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is refreshed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is refreshed; otherwise, <c>false</c>.
        /// </value>
        public bool IsRefreshed
        {
            get
            {
                var o = HttpContext.Current.Items[RefreshAction.PAGEREFRESHENTRY];

                return o != null && (bool)o;
            }
        } 

        #endregion

        #region Page Lifecycle Events

        /// <summary>
        /// Sets the <see cref="P:System.Web.UI.Page.Culture"/> and <see cref="P:System.Web.UI.Page.UICulture"/> for the current thread of the page.
        /// </summary>
        protected override void InitializeCulture()
        {
            base.InitializeCulture();

            ClientScript.GetPostBackEventReference(this, string.Empty); // ensures that javascript methode  '__doPostback()' is enabled. Used for switching languages
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreRenderComplete"/> event after the <see cref="M:System.Web.UI.Page.OnPreRenderComplete(System.EventArgs)"/> event and before the page is rendered.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            SaveRefreshState();
        } 

        #endregion

        #region Private Methods
        
        // Das versteckte Feld zum Speichern des aktuellen Anforderungstickets erstellen
        /// <summary>
        /// Saves the state of the refresh.
        /// </summary>
        private void SaveRefreshState()
        {
            try
            {
                var ticket = (int)HttpContext.Current.Items[RefreshAction.NEXTPAGETICKETENTRY];

                ClientScript.RegisterHiddenField(RefreshAction.CURRENTREFRESHTICKETENTRY, ticket.ToString());
            }
            catch
            {
            }
        } 

        #endregion
    }
}