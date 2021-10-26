using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace GEOCOM.GNSD.Web.Core.WebControls
{
    /// <summary>
    /// Custom control that renders the language selection links
    /// </summary>
    [ParseChildren(true)]
    [PersistChildren(true)]
    [ToolboxItem(true)]
    [Designer(typeof(LanguageSelectorDesigner))]
    [ToolboxData("<{0}:LanguageSelector ID=\"LanguageSelector\" runat=\"server\"></{0}:LanguageSelector>")]
    public class LanguageSelector : Control
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the language info definitions.
        /// </summary>
        /// <value>
        /// The language info definitions.
        /// </value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public List<LanguageInfo> LanguageInfoDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the language link CSS class.
        /// </summary>
        /// <value>
        /// The language link CSS class.
        /// </value>
        [Browsable(true)]
        public string LanguageLinkCssClass { get; set; }

        /// <summary>
        /// Gets or sets the language link selected CSS class.
        /// </summary>
        /// <value>
        /// The language link selected CSS class.
        /// </value>
        [Browsable(true)]
        public string LanguageLinkSelectedCssClass { get; set; }

        /// <summary>
        /// Gets or sets the language item CSS class.
        /// </summary>
        /// <value>
        /// The language item CSS class.
        /// </value>
        [Browsable(true)]
        public string LanguageItemCssClass { get; set; }

        /// <summary>
        /// Gets or sets the separator item CSS class.
        /// </summary>
        /// <value>
        /// The separator item CSS class.
        /// </value>
        [Browsable(true)]
        public string SeparatorItemCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [Browsable(true)]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        [Browsable(true)]
        public string Separator { get; set; }

        #endregion

        #region Constructor

        public LanguageSelector()
        {
            this.LanguageInfoDefinitions = new List<LanguageInfo>();
        }

        #endregion

        #region Base class overrides

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.CreateLanguageSelectorControls();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Creates the language selector controls.
        /// </summary>
        private void CreateLanguageSelectorControls()
        {
            if (this.LanguageInfoDefinitions.Count > 0)
            {
                var urlBase = this.GetUrlBase();

                this.Controls.Add(!string.IsNullOrWhiteSpace(this.CssClass)
                                      ? new LiteralControl(string.Format("<ul class=\"{0}\">", this.CssClass))
                                      : new LiteralControl("<ul>"));

                for (var i = 0; i < this.LanguageInfoDefinitions.Count; i++)
                {
                    this.Controls.Add(!string.IsNullOrWhiteSpace(this.LanguageItemCssClass)
                                          ? new LiteralControl(string.Format("<li class=\"{0}\">", this.LanguageItemCssClass))
                                          : new LiteralControl("<li>"));

                    this.CreateLanguageLink(this.LanguageInfoDefinitions[i], urlBase);

                    this.Controls.Add(new LiteralControl("</li>"));

                    if (!string.IsNullOrEmpty(this.Separator) && i < this.LanguageInfoDefinitions.Count - 1)
                    {
                        this.Controls.Add(!string.IsNullOrWhiteSpace(this.SeparatorItemCssClass)
                                              ? new LiteralControl(string.Format("<li class=\"{0}\">", this.SeparatorItemCssClass))
                                              : new LiteralControl("<li>"));

                        this.Controls.Add(new LiteralControl(string.Format("{0}</li>", this.Separator)));
                    }
                }

                this.Controls.Add(new LiteralControl("</ul>"));
            }
        }

        /// <summary>
        /// Gets the URL base.
        /// </summary>
        /// <returns></returns>
        private string GetUrlBase()
        {
            var urlParts = HttpContext.Current.Request.Url
                .ToString()
                .Split('?');

            var queryStrings = new List<string>();
            var queryStringPresent = urlParts.Length > 1;
            if ( queryStringPresent )
            {
                var queryString = urlParts[1];
                var nonLangQueryStrings =
                    queryString
                        .Split('&')
                        .Where(s => !s.StartsWith("lang", StringComparison.InvariantCultureIgnoreCase));
                queryStrings.AddRange(nonLangQueryStrings);
            }
            queryStrings.Add("lang=");
         
            return $"?{string.Join("&", queryStrings)}";
        }

        /// <summary>
        /// Creates the language link.
        /// </summary>
        /// <param name="languageInfo">The language info.</param>
        /// <param name="urlBase">The URL base.</param>
        private void CreateLanguageLink(LanguageInfo languageInfo, string urlBase)
        {
            var link = new HyperLink
                           {
                               ToolTip = languageInfo.LanguageSpecificTooltip,
                               Text = languageInfo.TwoLetterISOCode,
                               CssClass = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() ==
                                          languageInfo.TwoLetterISOCode.ToUpper()
                                              ? this.LanguageLinkSelectedCssClass
                                              : this.LanguageLinkCssClass,
                              NavigateUrl = string.Format("{0}{1}", urlBase, languageInfo.TwoLetterISOCode.ToLower())

                           };

            this.Controls.Add(link);
        }

        #endregion
    }

    /// <summary>
    /// Class that defines the text and tooltip of each link in the language selector
    /// </summary>
    public class LanguageInfo
    {
        /// <summary>
        /// Gets or sets the two letter ISO code.
        /// </summary>
        /// <value>
        /// The two letter ISO code.
        /// </value>
        [Browsable(true)]
        [XmlAttribute("code")]
        public string TwoLetterISOCode { get; set; }

        /// <summary>
        /// Gets or sets the language specific tooltip.
        /// </summary>
        /// <value>
        /// The language specific tooltip.
        /// </value>
        [Browsable(true)]
        [XmlAttribute("text")]
        public string LanguageSpecificTooltip { get; set; }
    }

    /// <summary>
    /// Designer class for the Language Selector so we have something to show in the visual designer surface
    /// </summary>
    public class LanguageSelectorDesigner : ControlDesigner
    {
    }
}