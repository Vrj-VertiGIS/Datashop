using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Service;

namespace GEOCOM.GNSD.Web.Controls
{
    public delegate void RequestHandler();

    /// <summary>
    /// Contains all common rerquest fields and performs specific validation
    /// </summary>
    public partial class CommonRequestDetails : RequestUserControl
    {
        #region Public Propeties

        public event RequestHandler RequestButtonClicked;

        public bool UsePdeToolbar { get; set; }

        public bool GeoAttachmentsEnabled
        {
            get { return chbxGeoAttachments.Checked; }
        }

        public string PolygonsInfo
        {
            get { return hiddenPolygonsInfo.Value; }
        }

        public string Reason
        {
            get { return cboReason.SelectedValue; }
        }

        public string Description
        {
            get { return JobDescription.Text; }
        }

        public string ParcelNumber
        {
            get { return JobParcelNumber.Text; }
        }

        public string Custom1
        {
            get { return custom1.Text; }
        }

        public string Custom2
        {
            get { return custom2.Text; }
        }

        public string Custom3 { get { return custom3 .Text; } }
        public string Custom4 { get { return custom4 .Text; } }
        public string Custom5 { get { return custom5 .Text; } }
        public string Custom6 { get { return custom6 .Text; } }
        public string Custom7 { get { return custom7 .Text; } }
        public string Custom8 { get { return custom8 .Text; } }
        public string Custom9 { get { return custom9.Text; } }
        public string Custom10 { get { return custom10.Text; } }

        public string BtnRequestTitle
        {
            set
            {
                btnRequest.Text = value;
                UpdatePanel2.Update();
            }
        }

        public DateTime? PeriodBeginDate
        {
            get
            {
                //NOTE: dojo adds a load of new elements to the DOM so the name of the calendar input in the designer 
                //is by no means the name of the POST variable holding the calendar value. Assuming it has the same name
                //just leads to intrinsically broken code. Use Firebug or View Source Chart to actually see what the browser is rendering.
                string periodBeginDateRequest = Request["ctl00$MainPanelContent$ctlRequestDetails$calender1"];
                DateTime? periodBeginDate = Utils.ParseDate(periodBeginDateRequest);
                return periodBeginDate;
            }
        }

        public DateTime? PeriodEndDate
        {
            get
            {
                //NOTE: dojo adds a load of new elements to the DOM so the name of the calendar input in the designer 
                //is by no means the name of the POST variable holding the calendar value. Assuming it has the same name
                //just leads to intrinsically broken code. Use Firebug or View Source Chart to actually see what the browser is rendering.
                string periodEndDateRequest = Request["ctl00$MainPanelContent$ctlRequestDetails$calender2"];
                DateTime? periodEndDate = Utils.ParseDate(periodEndDateRequest);
                return periodEndDate;
            }
        }

        public string AcceptAsSurrogateText
        {
            get
            {
                return LoadStr(39111, "Create plot as surrogate");
            }
        }

        public string AcceptText
        {
            get
            {
                return LoadStr(3911, "Create plot");
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets a value indicating whether [period dates required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [period dates required]; otherwise, <c>false</c>.
        /// </value>
        private bool PeriodDatesRequired
        {
            get
            {
                return this.PeriodDateObligationByReasonId[this.ReasonId];
            }
        }

        /// <summary>
        /// Gets the reason id.
        /// </summary>
        private int ReasonId
        {
            get
            {
                return int.Parse(cboReason.SelectedItem.Value);
            }
        }

        /// <summary>
        /// Gets the period date obligation by reason id.
        /// </summary>
        private Dictionary<long, bool> PeriodDateObligationByReasonId
        {
            get
            {
                return (Dictionary<long, bool>)ViewState["PeriodDateObligationByReasonId"];
            }
        }

        #endregion

        #region Page Event Lifecycle

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SetFieldFromConfig(this);
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnRequest.Enabled = agb.Checked || !IsFieldVisible("agb");
            GeoAttachmentsPlaceHolder.Visible = DatashopWebConfig.Instance.GeoAttachments.ShowCheckbox;

            if (DatashopWebConfig.Instance.RequestPageConfig.ActiveMode == RequestPageMode.Plot)
            {
                btnRequest.OnClientClick = "createPolygonInfos();";
            }
        }

        #endregion

        #region Event Handlers

        protected void OnCboReasonSelectedIndexChanged(object sender, EventArgs e)
        {
            long reasonId = this.GetSelectedReasonId();
            bool reasonNotSelected = reasonId == -1;
            if (reasonNotSelected)
            {
                this.SetCalendarsOptional();
                return;
            }

            var periodDateObligatory = this.PeriodDateObligationByReasonId[reasonId];

            if (periodDateObligatory)
                this.SetCalendarsRequired();
            else
                this.SetCalendarsOptional();
        }

        protected void BtnRequestClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cboReason.SelectedValue))
                {
                    ShowMessage(LoadStr(3902, "Please specify a reason."));
                    return;
                }

                if (this.PeriodDatesRequired && !this.AreDatesCorrectlyFilled())
                {
                    ShowMessage(LoadStr(3922, "Dates 'From' and 'To' are required and have to be in a meaningful time span."));
                    return;
                }

                Page.Validate("request");
                if (!Page.IsValid)
                    return;

                if (this.RequestButtonClicked != null)
                {
                    this.RequestButtonClicked();
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Creating Job failed for user {0:d}", UserId), ex);
            }
        }

        protected void CbSelectReasonLoad(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LogDebug("loading reasons");
                try
                {
                    this.InitReasonDictionaryInViewState();
                    var reasons = DatashopService.Instance.JobService.GetReasons();
                    cboReason.Items.Clear();
                    cboReason.Items.Add(new ListItem());

                    foreach (var reason in reasons)
                    {
                        cboReason.Items.Add(new ListItem(reason.Description, reason.ReasonId.ToString()));
                        this.PeriodDateObligationByReasonId.Add(reason.ReasonId, reason.PeriodDateRequired);
                    }
                    cboReason.SelectedIndex = 0;
                }
                catch (Exception err)
                {
                    LogError("Could not load Reasons.", err);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inits the state of the reason dictionary in view.
        /// </summary>
        private void InitReasonDictionaryInViewState()
        {
            ViewState["PeriodDateObligationByReasonId"] = new Dictionary<long, bool>();
        }

        /// <summary>
        /// Ares the dates correctly filled.
        /// </summary>
        /// <returns></returns>
        private bool AreDatesCorrectlyFilled()
        {
            return this.PeriodBeginDate != null && this.PeriodEndDate != null && this.PeriodBeginDate <= this.PeriodEndDate;
        }

        /// <summary>
        /// Gets the selected reason id.
        /// </summary>
        /// <returns></returns>
        private long GetSelectedReasonId()
        {
            long reasonId;
            bool parsed = long.TryParse(cboReason.SelectedItem.Value, out reasonId);
            reasonId = parsed ? reasonId : -1;

            return reasonId;
        }

        /// <summary>
        /// Sets the calendars optional.
        /// </summary>
        private void SetCalendarsOptional()
        {
            ScriptManager.RegisterStartupScript(
                cboReason,
                cboReason.GetType(),
                "OnCboReasonSelectedIndexChanged",
                "SetCalendarsOptional()",
                true);
        }

        /// <summary>
        /// Sets the calendars required.
        /// </summary>
        private void SetCalendarsRequired()
        {
            ScriptManager.RegisterStartupScript(
                cboReason,
                cboReason.GetType(),
                "OnCboReasonSelectedIndexChanged",
                "SetCalendarsRequired()",
                true);
        }

        #endregion
    }
}