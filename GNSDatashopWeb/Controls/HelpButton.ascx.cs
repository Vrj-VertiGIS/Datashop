using System;
using System.Linq;
using System.Threading;
using System.Web.UI;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// A help button can be placed anywhere on an app page.
    /// The button looks in the config file if there are some settings for him (based on pagename and buttonid.
    /// When no settings are found, the button is not visible.
    /// </summary>
    public partial class HelpButton : UserControl
    {
        public string CssClass { get; set; }

		public string ImageSrc { get; set; }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = false;

            if (DatashopWebConfig.Instance.OnlineHelpInfo.Buttons == null)
                return;

            try
            {
                var url = Request.Url.ToString().ToLower();
                if (url.Contains("returnurl"))
                    url = url.Split("?".ToCharArray())[0];

                var buttonInfo = DatashopWebConfig.Instance.OnlineHelpInfo.Buttons.SingleOrDefault(b => b.PlaceholderId.ToLower() == ID.ToLower()
                    && url.Contains(b.PageId.ToLower()));

				if (buttonInfo == null || string.IsNullOrEmpty(buttonInfo.Url))
                    return;

                var helpUrl = string.Format(buttonInfo.Url, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower());

                btn.HRef = string.Format("javascript:{0}('{1}');", buttonInfo.SameWindow ? "ShowHelpModal" : "ShowHelp", helpUrl);
                img.Alt = btn.Title = WebLanguage.LoadStr(1080, "Help");
				img.Src = ImageSrc;

                this.Visible = true;

                if (!string.IsNullOrWhiteSpace(this.CssClass))
                {
                    btn.Attributes.Remove("class");
                    btn.Attributes.Add("class", this.CssClass);
                }
            }
            catch
            {
                //NOTE: empty catch intended to suppress errors
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {   
            ScriptManager.RegisterClientScriptInclude(Page, typeof(Page), "HelpButtonScript", ResolveClientUrl("~/js/HelpButton.js"));
        }
    }
}