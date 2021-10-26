using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// Codebehind class for the CommonActAsSurrogate usercontrol
    /// </summary>
    public partial class CommonActAsSurrogate : RequestUserControl
    {
        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="CommonActAsSurrogate"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        public bool Checked
        {
            get { return chbxSurrogate.Checked; }
        }

        /// <summary>
        /// Gets the request date.
        /// </summary>
        public DateTime? RequestDate
        {
            get { return Utils.ParseDate(surrogateCalender.Text); }
        }

        /// <summary>
        /// Gets the request way.
        /// </summary>
        public string RequestWay
        {
            get
            {
                switch (DatashopWebConfig.Instance.SurrogateRequest.Placement)
                {
                    case SurrogateRequestPlacement.Selection:
                        return this.surrogateRequestWayDropDown.SelectedItem.Text;

                    case SurrogateRequestPlacement.Text:
                        return this.surrogateRequestWay.Text;

                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the surrogate user id.
        /// </summary>
        public int SurrogateUserId
        {
            get
            {
                int userId;
                int.TryParse(surrogateDropDown.SelectedId, out userId);
                return userId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [stop afer process].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [stop afer process]; otherwise, <c>false</c>.
        /// </value>
        public bool StopAferProcess
        {
            get { return chbxStopAferProcess.Checked; }
        }

        #endregion

        #region Private variables

        /// <summary>
        /// Holds a reference to the CreateTempUSer control
        /// </summary>
        private CommonCreateTempUser _createTempUserControl;

        /// <summary>
        /// holds a reference to the CreateBusinessUser control
        /// </summary>
        private CommonCreateBusinessUser _createBusinessUserControl;

        /// <summary>
        /// Holds a reference to the created user's Id
        /// </summary>
        private long _createdUserId = -1;

        #endregion

        #region Page Lifecycle events

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            btnAddBusiness.Visible = MDCreateBusinessUser.Visible = IsFieldVisible("addBusinessUser");

            btnAddTemp.Visible = MDCreateTempUser.Visible = IsFieldVisible("addTempUser");

            MDCreateBusinessUser.DialogLoaded += this.MdCreateBusinessUserDialogLoaded;
            MDCreateTempUser.DialogLoaded += this.MdCreateTempUserDialogLoaded;

        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SetFieldFromConfig(placeHolderSurrogate);

            this.DisableStopAfterProcess();

            if (!IsPostBack)
                this.PopulateSurrogateRequestPlacementOptions();

            this.SetRequestPlacement();
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(surrogateCalender.Text))
                surrogateCalender.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Page.ClientScript.RegisterForEventValidation(surrogateDropDown.UniqueID);
            base.Render(writer);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Disables the stop after process checkbox if the user isn't an admin.
        /// </summary>
        private void DisableStopAfterProcess()
        {
            this.divStopAfterProcess.Visible = HttpContext.Current.User.IsInRole("ADMIN");
        }

        /// <summary>
        /// Sets the request placement.
        /// </summary>
        private void SetRequestPlacement()
        {
            // use the page field infos for the both ways of surrogate request
            var pageFieldInfo = DatashopWebConfig.Instance.RequestPageConfig.PageFieldInfos.GetById(surrogateRequestWay.ID);
            switch (DatashopWebConfig.Instance.SurrogateRequest.Placement)
            {
                case SurrogateRequestPlacement.Selection:
                    this.surrogateRequestWay.Required = this.surrogateRequestWay.Visible = false;
                    this.surrogateRequestWayDropDown.Visible = pageFieldInfo.IsVisible;
                    this.surrogateRequestWayDropDown.Required = pageFieldInfo.IsRequired;

                    break;

                case SurrogateRequestPlacement.Text:
                    this.surrogateRequestWayDropDown.Visible = this.surrogateRequestWayDropDown.Required = false;
                    this.surrogateRequestWay.Visible = pageFieldInfo.IsVisible;
                    this.surrogateRequestWay.Required = pageFieldInfo.IsRequired;
                    break;
            }
        }

        /// <summary>
        /// Populates the surrogate request placement options dropdown
        /// </summary>
        private void PopulateSurrogateRequestPlacementOptions()
        {
            if (DatashopWebConfig.Instance.SurrogateRequest.Placement == SurrogateRequestPlacement.Selection)
            {
                var placementOptions = DatashopService.Instance.JobService.GetAllPlacementOptions();

                this.surrogateRequestWayDropDown.DropDown.Items.Clear();

                this.surrogateRequestWayDropDown.DropDown.Items.Add(string.Empty);

                foreach (var placementOption in placementOptions)
                    this.surrogateRequestWayDropDown.DropDown.Items.Add(new ListItem
                    {
                        Text = placementOption.Text,
                        Value = placementOption.Text
                    });
            }
        }


        /// <summary>
        /// Handles the Create Business User dialog being loaded
        /// </summary>
        protected void MdCreateTempUserDialogLoaded()
        {
            this._createTempUserControl = (CommonCreateTempUser)MDCreateTempUser.FindControl("ucCreateTemp");
            if (this._createTempUserControl != null)
            {
                this._createTempUserControl.FieldInfos = DatashopWebConfig.Instance.LoginTempUserPageFieldInfos;
                this._createTempUserControl.UserCreated += this.TempUserCreated;
            }
        }

        /// <summary>
        /// Handles the Create Business User dialog being loaded
        /// </summary>
        protected void MdCreateBusinessUserDialogLoaded()
        {
            this._createBusinessUserControl = (CommonCreateBusinessUser)MDCreateBusinessUser.FindControl("ucCreateBusiness");
            if (this._createBusinessUserControl != null)
            {
                this._createBusinessUserControl.FieldInfos = DatashopWebConfig.Instance.RegisterBusinessUserPageFieldInfos;
                this._createBusinessUserControl.UserCreated += this.BusinessUserCreated;
            }
        }

        /// <summary>
        /// Handles the CheckedChange event of the ChbxSurrogate control
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChbxSurrogateCheckedChanged(object sender, EventArgs e)
        {
            surrogateDiv.Style.Add("display", chbxSurrogate.Checked ? string.Empty : "none");
        }

        /// <summary>
        /// Handles the Temp User being created
        /// </summary>
        /// <param name="newUserId">The new user id.</param>
        protected void TempUserCreated(long newUserId)
        {
            MDCreateTempUser.Hide();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "msgJS", "MDCreateTempUser.Hide();" +
                                                                               GenerateSetUserJS(newUserId), true);
        }

        private static string GenerateSetUserJS(long userId)
        {
            var user = DatashopService.Instance.JobService.GetUser(userId);
            return String.Format("setUser('{0}', {1});", FormatUser(user), user.UserId);
        }

        /// <summary>
        /// Handles the Business USer Being created
        /// </summary>
        /// <param name="newUserId">The new user id.</param>
        protected void BusinessUserCreated(long newUserId)
        {
            MDCreateBusinessUser.Hide();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "msgJS", "MDCreateBusinessUser.Hide();" + GenerateSetUserJS(newUserId), true);
        }

        #endregion

        public static string FormatUser(User user)
        {
            var userFormated = String.Format("{8} - {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", 
                user.LastName, user.FirstName, user.Email,
                user.Company, user.Street, user.StreetNr, user.CityCode, user.City, user.UserId)
                .Replace(" ,", "")
                .Trim()
                .TrimEnd(',');
            return userFormated;
        }
    }
}