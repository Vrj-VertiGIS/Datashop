using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.TypeConversion;
using GEOCOM.GNSD.Common.Utils;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.DocumentStreaming;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Codebehind class for the MyPlots page
    /// </summary>
    public partial class MyPlots : Controls.RequestPage
    {

        public MyPlots() : base(DatashopWebConfig.Instance.RequestPageConfig.PageFieldInfos)
        { }

        #region Private members

        /// <summary>
        /// Holds the logger for this instance
        /// </summary>
        private readonly IMsg log = new Msg(typeof(MyPlots));

        /// <summary>
        /// Holds the count of the records currently displayed
        /// </summary>
        private int recordCount;

        /// <summary>
        /// Gets the sort expression.
        /// </summary>
        private string SortExpression
        {
            get
            {
                return ViewState["SortExpression"].ToString();
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        private SortDirection SortDirection
        {
            get
            {
                return (SortDirection)Enum.Parse(typeof(SortDirection), ViewState["SortDirection"].ToString());
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        private MyJobSearchParameters SearchParameters
        {
            get
            {
                return Session["SearchParameters"] as MyJobSearchParameters;
            }
            set
            {
                Session["SearchParameters"] = value;
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
            if (Page.User.IsInRole("TEMP"))
            {
                Response.RedirectSafe("~");
                return;
            }
            if (!IsPostBack)
                ProcessRequest();
            SetFieldFromConfig(this);
            SetGridColumnsFromConfig();
            RegisterPostBackControls();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the request.
        /// </summary>
        private void ProcessRequest()
        {
            var itemToSelect = cboPageSize.Items.FindByValue(gvwPlots.PageSize.ToString());

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            var canPerformRepresentativeFunctions = User.IsInRole("REPRESENTATIVE") || User.IsInRole("ADMIN");

            ToggleSearchFieldVisibility(canPerformRepresentativeFunctions,
                txtUserId, txtFirstname, txtLastname, txtCompany, divRepresentativeFields, divRepresentativeFields2);

            SearchParameters = null;
            SortExpression = "CreatedDate";
            SortDirection = SortDirection.Descending;
            //NOTE: calling Sort on the grid implicitly binds the data via the sorting event handler
            gvwPlots.Sort("CreatedDate", SortDirection);
        }

        /// <summary>
        /// Toggles the search field visibility.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        /// <param name="controlsToHide">The controls to hide.</param>
        private void ToggleSearchFieldVisibility(bool visible, params Control[] controlsToHide)
        {
            controlsToHide.ToList()
                .ForEach(c => c.Visible = visible);
        }

        /// <summary>
        /// Resets the form.
        /// </summary>
        private void ResetForm()
        {
            txtCompany.Text = txtFirstname.Text = txtJobId.Text =
                txtLastname.Text = txtUserId.Text = calender1.Value =
                    calender2.Value = custom1.Text = custom2.Text =
                        custom3.Text = custom4.Text = custom5.Text =
                            custom6.Text = custom7.Text = custom8.Text =
                                custom9.Text = custom10.Text = JobParcelNumber.Text = string.Empty;

            cboReason.SelectedIndex = 0;

            SearchParameters = null;
            SortExpression = "CreatedDate";
            SortDirection = SortDirection.Descending;

            cboDownloaded.SelectedIndex = 0;

            gvwPlots.PageIndex = 0;
            gvwPlots.DataSource = GetData(SortExpression, SortDirection);
            gvwPlots.DataBind();
        }

        /// <summary>
        /// Sets the column header to display sorting.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortExpression">The sort expression.</param>
        private void SetColumnHeaderToDisplaySorting(DataControlField c, SortDirection sortDirection, string sortExpression)
        {
            c.HeaderStyle.CssClass = c.SortExpression == sortExpression
                ? string.Format("header {0}", sortDirection.ToString()
                                                          .ToLower())
                : "header";
        }

        /// <summary>
        /// Registers the post back controls.
        /// </summary>
        private void RegisterPostBackControls()
        {
            ScriptManager.GetCurrent(this)
                    .RegisterPostBackControl(btnExportAsCsv);

            gvwPlots.Rows.Cast<GridViewRow>()
                .ToList()
                .ForEach(RegisterGridRowButtonsForPostback);
        }

        /// <summary>
        /// Registers the grid row buttons for postback.
        /// </summary>
        /// <param name="row">The row.</param>
        private void RegisterGridRowButtonsForPostback(GridViewRow row)
        {
            RegisterGridViewRowButtonForPostback(row, "btnDownload");
            RegisterGridViewRowButtonForPostback(row, "btnRestart");
        }

        /// <summary>
        /// Registers the grid view row button for postback.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="id">The identifier.</param>
        private void RegisterGridViewRowButtonForPostback(GridViewRow row, string id)
        {
            var btnDownload = row.FindControl(id) as ImageButton;

            ScriptManager.GetCurrent(this)
                .RegisterPostBackControl(btnDownload);
        }

        /// <summary>
        /// Sets the grid columns visibility from the configuration of the RequestPage.
        /// </summary>
        private void SetGridColumnsFromConfig()
        {
            foreach (DataControlField gridColum in gvwPlots.Columns)
            {
                if (gridColum is BoundField boundColumn)
                {
                    var requestPageFieldInfo = _pageFieldInfos.Fields.SingleOrDefault(f =>
                        f.ServerId.Equals(boundColumn.DataField, StringComparison.InvariantCultureIgnoreCase));
                    if (requestPageFieldInfo != null)
                        gridColum.Visible = requestPageFieldInfo.IsVisible;
                }
            }
        }

        #region Data

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        private List<MyJob> GetData(string sortExpression, SortDirection sortDirection, MyJobSearchParameters searchParameters = null)
        {
            try
            {
                var data = searchParameters == null
                        ? GetDefaultData(sortExpression, sortDirection == SortDirection.Ascending)
                        : PerformSearch(searchParameters, sortExpression, sortDirection == SortDirection.Ascending);

                recordCount = data.Count();

                return data;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("GetData failed", ex);

                throw;
            }
        }

        /// <summary>
        /// Gets the default data.
        /// </summary>
        /// <returns></returns>
        private List<MyJob> GetDefaultData(string sortExpression, bool sortAscending)
        {
            var canPerformRepresentativeFunctions = User.IsInRole("REPRESENTATIVE") || User.IsInRole("ADMIN");

            long userId;

            if (long.TryParse(Page.User.Identity.Name, out userId))
                return DatashopService.Instance.JobService.GetMyJobsByUserId(userId, canPerformRepresentativeFunctions, sortExpression, sortAscending);

            throw new Exception(string.Format("Error identifying user with Id: {0}", Page.User.Identity.Name));
        }

        /// <summary>
        /// Performs the search.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <param name="sortAscending">if set to <c>true</c> [sort ascending].</param>
        /// <returns></returns>
        private List<MyJob> PerformSearch(MyJobSearchParameters parameters, string sortExpression, bool sortAscending)
        {
            SearchParameters = parameters;

            var canPerformRepresentativeFunctions = User.IsInRole("REPRESENTATIVE") || User.IsInRole("ADMIN");

            long userId;

            if (long.TryParse(Page.User.Identity.Name, out userId))
                return DatashopService.Instance.JobService.GetMyJobsByUserIdAndSearchParameters(userId, canPerformRepresentativeFunctions, parameters, sortExpression, sortAscending);

            throw new Exception(string.Format("Error identifying user with Id: {0}", Page.User.Identity.Name));
        }



        #endregion

        #endregion

        #region Event Handlers

        #region Button Events

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            long jobId;

            if (!string.IsNullOrEmpty(txtJobId.Text))
            {
                var jobIdNumeric = long.TryParse(txtJobId.Text, out jobId);

                if (!jobIdNumeric)
                {
                    this.ShowMessage(WebLanguage.LoadStr(5029, "JobId must be numeric"));
                    return;
                }
            }


            var parameters = new MyJobSearchParameters
            {
                Company = txtCompany.Text,
                CreatedDateEnd = MultiConverter.Convert<DateTime?>(calender2.Value),
                CreatedDateStart = MultiConverter.Convert<DateTime?>(calender1.Value),
                FirstName = txtFirstname.Text,
                LastName = txtLastname.Text,
                UserId = txtUserId.Text,
                JobId = MultiConverter.Convert<long?>(txtJobId.Text),
                Downloaded = MultiConverter.Convert<bool?>(cboDownloaded.SelectedValue),
                Custom1 = custom1.Text,
                Custom2 = custom2.Text,
                Custom3 = custom3.Text,
                Custom4 = custom4.Text,
                Custom5 = custom5.Text,
                Custom6 = custom6.Text,
                Custom7 = custom7.Text,
                Custom8 = custom8.Text,
                Custom9 = custom9.Text,
                Custom10 = custom10.Text,
                JobParcelNumber = JobParcelNumber.Text,
                ReasonId = long.TryParse(cboReason.SelectedValue, out var reasonId) ? reasonId : (long?)null
            };

            if (parameters.IsEmtpy)
                this.ShowMessage(WebLanguage.LoadStr(5028, "Please fill in at least one of the search fields"));
            else
            {
                gvwPlots.PageIndex = 0;
                gvwPlots.DataSource = GetData(SortExpression, SortDirection, parameters);
                gvwPlots.DataBind();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        /// <summary>
        /// Handles the Click event of the btnExportAsCsv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnExportAsCsv_Click(object sender, EventArgs e)
        {
            try
            {
                var jobs = GetData(SortExpression, SortDirection, SearchParameters);

                var columnsToExport = gvwPlots.Columns.Cast<DataControlField>()
                                                           .Where(d => d.Visible && d.HeaderText.Length > 0)
                                                           .OfType<BoundField>()
                                                           .ToList();

                var columnHeadersToExport = columnsToExport.ConvertAll(d => d.HeaderText);

                var columnNamesToExport = columnsToExport.ConvertAll(b => b.DataField);

                var output = jobs.ToCsvString(columnNamesToExport, columnHeadersToExport, ";");

                var outputBytes = Encoding.GetEncoding("iso-8859-1")
                                          .GetBytes(output);

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=MyPlots.csv");
                Response.ContentType = "text/csv";
                Response.BinaryWrite(outputBytes);
            }
            catch (Exception ex)
            {
                log.Error("Downloading csv file failed", ex);

                throw;
            }

            Response.End();
        }

        /// <summary>
        /// Handles the Click event of the btnDownload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;

            if (btn != null)
            {
                try
                {
                    var jobId = MultiConverter.Convert<long>(btn.CommandArgument);

                    if (jobId > 0)
                    {
                        var job = DatashopService.Instance.JobService.GetJobById(jobId);

                        var streamer = new DocumentStreamer(job, Response);

                        streamer.CopyDocumentToRespose();

                        Response.End();
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Downloading document failed with argument: {0}", btn.CommandArgument, ex);
                    this.ShowMessage(WebLanguage.LoadStr(9050, "An error has occurred while processing your request."));
                }
            }
        }

        protected void btnRestart_Click(object sender, ImageClickEventArgs e)
        {
            var btn = sender as ImageButton;

            if (btn != null)
            {
                try
                {
                    var jobId = MultiConverter.Convert<long>(btn.CommandArgument);

                    if (jobId > 0)
                    {
                        var cloneId = DatashopService.Instance.JobService.CloneJob(jobId);

                        gvwPlots.DataSource = GetData(SortExpression, SortDirection, SearchParameters);
                        gvwPlots.DataBind();

                        var confirmationMessage = WebLanguage.LoadStr(5030, "Copied job with id {0} to {1}");
                        this.ShowMessage(string.Format(confirmationMessage, jobId, cloneId));
                    }
                }
                catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> ex)
                {
                    if(ex.Detail.Type == typeof(TemplateLimitReachedException).FullName)
                    {
                        var msg = String.Format(WebLanguage.LoadStr(10000, "Limit of requests for templates was reached."));
                        Page.ShowMessage(msg);
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Restarting job with Id {0} failed", btn.CommandArgument, ex);

                    throw;
                }
            }
        }

        #endregion

        #region Grid Events

        /// <summary>
        /// Handles the PageIndexChanged event of the gvwPlots control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewPageEventArgs"/> instance containing the event data.</param>
        protected void gvwPlots_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            gvwPlots.PageIndex = e.NewPageIndex;

            gvwPlots.DataSource = GetData(SortExpression, SortDirection, SearchParameters);
            gvwPlots.DataBind();
        }

        /// <summary>
        /// Handles the Sorting event of the gvwPlots control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
        protected void gvwPlots_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (IsPostBack) //NOTE: check for IsPostback here because the initial loading of the data lands here without a postback as well
                SortDirection = e.SortExpression == SortExpression
                                         ? (SortDirection == SortDirection.Ascending
                                                ? SortDirection.Descending
                                                : SortDirection.Ascending)
                                         : e.SortDirection;

            SortExpression = e.SortExpression;

            gvwPlots.DataSource = GetData(SortExpression, SortDirection, SearchParameters);
            gvwPlots.DataBind();
        }

        /// <summary>
        /// Handles the RowDataBound event of the gvwPlots control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gvwPlots_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var myJob = (MyJob)e.Row.DataItem;

                HandleRestartRowButton(e, myJob);
                HandleDownloadRowButton(e, myJob);
                HandleArchivedImage(e, myJob);
            }
        }

        private void HandleArchivedImage(GridViewRowEventArgs e, MyJob myJob)
        {
            var img = e.Row.FindControl("imgArchived") as Image;

            if (img != null)
            {
                img.Visible = myJob.IsArchived;
            }
        }

        /// <summary>
        /// Handles the restart row button.
        /// </summary>
        /// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
        /// <param name="myJob">My job.</param>
        private void HandleRestartRowButton(GridViewRowEventArgs e, MyJob myJob)
        {
            var btn = e.Row.FindControl("btnRestart") as ImageButton;

            if (btn != null)
            {
                var expiryTimeSpan = new TimeSpan(DatashopWebConfig.Instance.DocumentExpiry.RestartJobExpiration, 0, 0, 0);

                var expiryDate = myJob.CreatedDate.Add(expiryTimeSpan);

                var expired = DateTime.Now > expiryDate;

                btn.Enabled = !expired;

                if (btn.Enabled)
                    RegisterGridViewRowButtonForPostback(e.Row, "btnRestart");
                else
                {
                    var img = e.Row.FindControl("imgRestartDisabled") as Image;

                    if (img != null)
                    {
                        btn.Visible = false;
                        img.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the download row button.
        /// </summary>
        /// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
        /// <param name="myJob">My job.</param>
        private void HandleDownloadRowButton(GridViewRowEventArgs e, MyJob myJob)
        {
            var btn = e.Row.FindControl("btnDownload") as ImageButton;

            if (btn != null)
            {

                var jobFinished = myJob.Step >= (int)PlotJobSteps.JobDone && myJob.Status == (int)WorkflowStepState.Finished;
                btn.Visible = myJob.JobGuid != null && !myJob.IsArchived && jobFinished;

                if (btn.Visible)
                {
                    var expiryTimeSpan = new TimeSpan(DatashopWebConfig.Instance.DocumentExpiry.ArchiveAfterDays, 0, 0, 0);

                    var expiryDate = myJob.CreatedDate.Add(expiryTimeSpan);

                    var expired = DateTime.Now > expiryDate;

                    btn.Enabled = !expired;

                    if (btn.Enabled)
                        RegisterGridViewRowButtonForPostback(e.Row, "btnDownload");
                    else
                    {
                        var img = e.Row.FindControl("imgDownloadDisabled") as Image;

                        if (img != null)
                        {
                            btn.Visible = false;
                            img.Visible = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DataBound event of the gvwPlots control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void gvwPlots_DataBound(object sender, EventArgs e)
        {
            gvwPlots.Columns.Cast<DataControlField>()
                .ToList()
                .ForEach(c => SetColumnHeaderToDisplaySorting(c, SortDirection, SortExpression));

            lblRecordCount.Text = string.Format(WebLanguage.LoadStr(5015, "Found {0} records"), recordCount);
        }

        #endregion

        #region Other Control Events

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cboPageSize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvwPlots.PageSize = int.Parse(cboPageSize.SelectedValue);
            gvwPlots.PageIndex = 0;
            gvwPlots.DataSource = GetData(SortExpression, SortDirection, SearchParameters);
            gvwPlots.DataBind();
        }

        protected void CbSelectReasonLoad(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LogDebug("loading reasons");
                try
                {
                    var reasons = DatashopService.Instance.JobService.GetReasons();
                    cboReason.Items.Clear();
                    cboReason.Items.Add(new ListItem());
                    foreach (var reason in reasons)
                    {
                        cboReason.Items.Add(new ListItem(reason.Description, reason.ReasonId.ToString()));
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

        #endregion
    }
}