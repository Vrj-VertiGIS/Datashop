using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// This encapsulates the plot setting controls : template selector, rotation, scale, etc...
    /// </summary>
    public partial class PlotLayout : RequestUserControl
    {
        #region Public Properties


        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public double Rotation { get; set; }

        /// <summary>
        /// Gets the plot format client ID.
        /// </summary>
        public string PlotFormatClientID
        {
            get { return this.cboTemplate.ClientID; }
        }

        /// <summary>
        /// Gets the rotation slider behaviour id.
        /// </summary>
        public string RotationSliderBehaviourId
        {
            get
            {
                return SliderExtender1.BehaviorID;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                if (!this.IsTemplateCorrect())
                {
                    ShowMessage(WebLanguage.LoadStr(3901, "Please set a plotextend."));
                    return false;
                }

                if (!this.IsSelectedScaleCorrect())
                {
                    ShowMessage(WebLanguage.LoadStr(3907, "Please select a scale."));
                    return false;
                }

                return true;
            }
        }


        public bool UseLastMapExtents
        {
            get
            {
                bool result = false;
                bool.TryParse(Request.QueryString["useLastMapExtents"], out result);
                return result;
            }
        }


        #endregion

        #region Page Lifecycle Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.ProcessRequest();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        private void ProcessRequest()
        {
            this.PopulateTemplates();

            this.PopulateScale();

            this.ApplyClientSideEventHandlers();
        }

        private void ApplyClientSideEventHandlers()
        {
            this.cboScale.Attributes.Add("onChange", "changeScale();");
        }

        /// <summary>
        /// Populates the templates.
        /// </summary>
        private void PopulateTemplates()
        {
            LogDebug("loading templates");

            long userId = long.Parse(Page.User.Identity.Name);
            var userTemplates = DatashopService.Instance.JobService.GetTemplatesForUser(userId);

            var templateDimensions = new StringBuilder();
            cboTemplate.Items.Add(new ListItem());

            bool limitExceeded = !userTemplates.Any(def => def.RemainingLimit > 0);
            if (limitExceeded)
                Response.RedirectSafe(Page.User.IsInRole("BUSINESS") ? "error/TemplateLimitError.aspx" : "error/TempUserLimitError.aspx", false);

            var plotLimitFormat = DatashopWebConfig.Instance.RequestPageConfig.PlotMode.LimitFormat ?? "%(definition_name) - limit = %(definition_limit)";
            foreach (var pfd in userTemplates)
            {
                var maxpolygon = DatashopWebConfig.Instance.RequestPageConfig.PlotMode.MaxPolygons;
                int limit = maxpolygon > 0 ? Math.Min(maxpolygon, pfd.RemainingLimit) : pfd.RemainingLimit;

                var description = plotLimitFormat.Replace("%(definition_name)", pfd.Description);
                description = description.Replace("%(definition_limit)", limit.ToString());
                var listItem = new ListItem(description, pfd.Template) { Enabled = limit >= 1 };

                cboTemplate.Items.Add(listItem);

                templateDimensions.AppendFormat(
                    "'{0}|{1}|{2}|{3}',",
                    pfd.Template,
                    pfd.PlotWidthCM.ToString(CultureInfo.InvariantCulture),
                    pfd.PlotHeightCM.ToString(CultureInfo.InvariantCulture),
                    limit);
            }

            if (templateDimensions.Length > 0)
                Page.ClientScript.RegisterArrayDeclaration("templateDimension", templateDimensions.ToString().Substring(0, templateDimensions.Length - 1));
        }

        /// <summary>
        /// Populates the scale.
        /// </summary>
        private void PopulateScale()
        {
            try
            {
                LogDebug("loading scales");

                //TODO: update to WCF Service
                var allScales = DatashopService.Instance.JobService.GetScales();
                var roles = Roles.GetRolesForUser();
                var userScales = new SortedList<int, object>();

                foreach (var roleScale in allScales.Where(s => s.Value != null))
                {
                    var roleScaleIsDefault = roleScale.Key.Equals("default", StringComparison.OrdinalIgnoreCase);
                    var roleScaleMatchesUserRoles = roles.Contains(roleScale.Key, StringComparer.InvariantCultureIgnoreCase);
                    if (!roleScaleIsDefault && !roleScaleMatchesUserRoles)
                        continue;

                    foreach (var scale in roleScale.Value)
                    {
                        var scaleKey = int.Parse(scale.Key);
                        if (userScales.ContainsKey(scaleKey))
                            continue;
                        userScales.Add(scaleKey, scale.Text);
                    }
                }

                foreach (var ktp in userScales)
                    cboScale.Items.Add(new ListItem(ktp.Value.ToString(), ktp.Key.ToString()));

                cboScale.SelectedValue = DatashopWebConfig.Instance.RequestPageConfig.PlotMode.DefaultScale.ToString(CultureInfo.InvariantCulture); ;
            }
            catch (Exception ex)
            {
                LogError(string.Format("Loading scales failed. Is there a configuration error?: {0}", ex.Message), ex);

                Response.RedirectSafe("error/GeneralErrorPage.aspx", false);
            }
        }

        /// <summary>
        /// Determines whether [is template correct].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is template correct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTemplateCorrect()
        {
            var templates = DatashopService.Instance.JobService.GetAllTemplates();

            var found = false;

            foreach (var tempNames in templates.Where(tempNames => tempNames.PlotdefinitionKey.Template.Equals(cboTemplate.SelectedValue, StringComparison.CurrentCultureIgnoreCase)))
            {
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Determines whether the selected scale is correct.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is scale correct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSelectedScaleCorrect()
        {
            var roleScales = DatashopService.Instance.JobService.GetScales();
            var allScalesForUser = roleScales
                .Where(rs => rs.Value != null)
                .Where(rs =>
                    HttpContext.Current.User.IsInRole(rs.Key) ||
                    rs.Key.Equals("default", StringComparison.CurrentCultureIgnoreCase))
                .SelectMany(rs => rs.Value)
                .ToArray();
            var isCorrectScale = allScalesForUser
                                .Any(s => s.Key.Equals(cboScale.SelectedValue, StringComparison.CurrentCultureIgnoreCase));

            return isCorrectScale;

        }

        #endregion

    }
}