using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using GNSDatashopAdmin.Config;
using GNSDatashopAdmin.Helpers;

/*
   *  CAUTION THE ViewState of the grid is enabled !!!
   */
namespace GNSDatashopAdmin.Controls
{
    public partial class JobGridControl : System.Web.UI.UserControl
    {
        // this page should not show any admin jobs
        // this is a wia goodie : use the dash sign (#) to say "<>"
        private const string WithoutAdminJobs = null;//"ProcessorClassId#GEOCOM.GNSD.DatashopWorkflow.Admin.AdminWorkflow";

        private const int PageSizeDefault = 20;

        private const int PageIndexDefault = 0;

        private readonly string[] _primaryKeys = { "JobId" };

        private readonly Dictionary<string, WorkflowStepIdName[]> _workflowIdNamesCache = new Dictionary<string, WorkflowStepIdName[]>();

        private JobDetails[] _jobs;

        private bool IsSearching
        {
            get
            {
                return (bool)ViewState["Searching"];
            }

            set
            {
                ViewState["Searching"] = value;
            }
        }

        private bool IsSearchVisible
        {
            get
            {
                return (bool)ViewState["SearchVisible"];
            }

            set
            {
                ViewState["SearchVisible"] = value;
            }
        }

        private bool _reloadAndBindGrid;
        private bool _showArchived;
        private bool _showNotArchived;

        protected void Page_Load(object sender, EventArgs e)
        {
            javaScript.Text = string.Empty;
            JobGrid.DataKeyNames = _primaryKeys;


            // fill DropDownLists in Searchbar
            if (!Page.IsPostBack)
            {
                ddlReason.Items.Clear();

                // add empty item
                ddlReason.Items.Add(string.Empty);
                var reasons = DatashopService.Instance.JobService.GetReasons();

                foreach (Reason reason in reasons)
                {
                    ddlReason.Items.Add(reason.Description);
                }
                ddlStatus.Items.Clear();
                ddlStatus.DataSource = Enum.GetValues(typeof(WorkflowStepState));
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, string.Empty);
            }

            // writing viewState for sorting
            if (ViewState["SortExpression"] == null)
                ViewState["SortExpression"] = "JobId";
            if (ViewState["SortAscending"] == null)
                ViewState["SortAscending"] = false;

            // writing ViewState for pagging
            if (ViewState["pageIndex"] == null)
                ViewState["pageIndex"] = PageIndexDefault;
            if (ViewState["pageSize"] == null)
                ViewState["pageSize"] = PageSizeDefault;

            // write ViewState for Searching
            if (ViewState["Searching"] == null)
                ViewState["Searching"] = false;

            // Write ViewState for Showing ArchiveJobs
            if (ShowArchivedDropDown)
            {
                int showArchived = int.Parse(ddlArchive.Items[ddlArchive.SelectedIndex].Value);
                switch (showArchived)
                {
                    case 1:
                        ShowArchived = false;
                        ShowNotArchived = true;
                        break;
                    case 2:
                        ShowArchived = true;
                        ShowNotArchived = false;
                        break;
                    case 3:
                        ShowArchived = true;
                        ShowNotArchived = true;
                        break;
                }
            }

            AspNet.SetupColumnsFromConfig(DatashopWebAdminConfig.Instance.JobListMain, JobGrid);

            // get parameters for pagging
            if (Request.Params.Get("__EVENTTARGET") == "Pagging")
            {
                try
                {
                    ViewState["pageIndex"] = int.Parse(Request.Params.Get("__EVENTARGUMENT"));
                }
                catch (FormatException)
                {
                    ViewState["pageIndex"] = PageIndexDefault;
                }
                ////this.GetAndBindData(false);
                _reloadAndBindGrid = true;
            }
            else
            {
                if (!Page.IsPostBack)
                {
                    // initial load of the grid
                    _reloadAndBindGrid = true;
                }
            }

            this.PreRender += JobManagePagePreRender;
        }

        /// <summary>
        /// This event handler was introduced to reset the search filter when the panel is hidden
        /// This event occurs on the server only when the user hides the search filter
        /// </summary>
        /// <param name="sender">Net sender</param>
        /// <param name="e">Net event data</param>
        protected void CmdShowSearchClick(object sender, EventArgs e)
        {
            this.ResetSearchFilter();
            searchFields.Visible = !searchFields.Visible;
        }

        protected void DdlPageSizeIndexChange(object sender, EventArgs e)
        {
            var pageSize = PageSizeDefault;
            int.TryParse(DdlPageSize.SelectedValue, out pageSize);
            JobGrid.PageSize = pageSize;
            ViewState["pageSize"] = pageSize;
            ViewState["pageIndex"] = PageIndexDefault;
            _reloadAndBindGrid = true;
        }

        protected void JobGridSorting(object sender, GridViewSortEventArgs e)
        {
            // Change Sortdirection if same label is clicked twice
            if (ViewState["SortExpression"].ToString().Equals(e.SortExpression))
            {
                ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
            }
            ViewState["SortExpression"] = e.SortExpression;

            // this is done in preRender now
            ////this.GetAndBindData(false);
            _reloadAndBindGrid = true;
        }

        protected void ArchiveIndexChange(object sender, EventArgs e)
        {
            int showArchived = int.Parse(ddlArchive.Items[ddlArchive.SelectedIndex].Value);
            switch (showArchived)
            {
                case 1:
                    ShowArchived = false;
                    ShowNotArchived = true;
                    break;
                case 2:
                    ShowArchived = true;
                    ShowNotArchived = false;
                    break;
                case 3:
                    ShowArchived = true;
                    ShowNotArchived = true;
                    break;
            }

            // this is done in preRender now
            ////this.GetAndBindData(false);
            _reloadAndBindGrid = true;
        }

        protected void BtnSearch(object sender, EventArgs e)
        {

            // Validate the search fields
            Page.Validate("JobGridControlValidationGroup");
            if (!Page.IsValid)
                return;

            // set viewstate for searching
            if (!IsSearching)
            {
                ViewState["pageIndex"] = PageIndexDefault;
                IsSearching = true;
            }

            _reloadAndBindGrid = true;
        }

        protected void BtnSearchStopp(object sender, EventArgs e)
        {
            this.ResetSearchFilter();
        }

        protected void ResetSearchFilter()
        {
            txbCreateDateNew.Text = string.Empty;
            txbCreateDateOld.Text = string.Empty;
            txbJobId.Text = string.Empty;
            txbStateDateNew.Text = string.Empty;
            txbStateDateOld.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            ddlReason.SelectedIndex = 0;
            txbFree1.Text = string.Empty;
            txbFree2.Text = string.Empty;
            txbFree3.Text = string.Empty;
            tbxFirstName.Text = string.Empty;
            tbxLastName.Text = string.Empty;
            txbUserId.Text = string.Empty;
            txbMachineName.Text = string.Empty;

            ////ViewState["Searching"] = false;
            IsSearching = false;
            ViewState["pageIndex"] = PageIndexDefault;

            // this is done in preRender now
            ////this.GetAndBindData(false);
            _reloadAndBindGrid = true;
        }

        protected void JobManagePagePreRender(object sender, EventArgs e)
        {
            if (_reloadAndBindGrid)
            {
                if (IsSearching)
                {
                    SearchJobs();
                }
                else
                {
                    GetJobDetailsSorted();
                }
            }
        }

        #region grid data bind

        private bool ShowArchivedDropDown
        {
            get { return ddlArchive.Visible; }
        }

        public bool ShowArchived
        {
            get
            {
                if (ShowArchivedDropDown)
                    return (bool) ViewState["ShowArchived"];
                else
                    return _showArchived;
            }
            set
            {
                if (ShowArchivedDropDown)
                    ViewState["ShowArchived"] = value;
                else
                    _showArchived = value;
            }
        }

        public bool ShowNotArchived
        {
            get
            {
                if (ShowArchivedDropDown)
                    return (bool) ViewState["ShowNotArchived"];
                else
                    return _showNotArchived;
            }
            set
            {
                if (ShowArchivedDropDown)
                    ViewState["ShowNotArchived"] = value;
                else
                    _showNotArchived = value;
            }
        }

        private void GetJobDetailsSorted()
        {
            var showArchived = ShowArchived; 
            var showNotArchived = ShowNotArchived; 
            var sortExpression = ViewState["SortExpression"].ToString();
            var sortAscending = (bool)ViewState["SortAscending"];
            var startIndex = int.Parse(ViewState["pageIndex"].ToString()) * int.Parse(ViewState["pageSize"].ToString());
            var quantity = int.Parse(ViewState["pageSize"].ToString());

            // std prerender
            _jobs = DatashopService.Instance.JobService.GetJobDetailsSorted(
                showArchived,
                showNotArchived,
                sortExpression,
                sortAscending,
                startIndex,
                quantity,
                WithoutAdminJobs);

            if ((_jobs != null) && (_jobs.Length > 0))
            {
                ////IList<JobDetails> jobsWithoutAdminJob = this._jobs.Where(jobDetail => !jobDetail.ProcessorClassId.Equals(WorkflowDefinitions.AdminWorkflowClassId)).ToList();

                JobGrid.DataSource = _jobs;
                FillWorkflowIdNamesCache();
            }
            JobGrid.DataBind();
        }

        private void FillWorkflowIdNamesCache()
        {
            if (_jobs == null)
                return;

            foreach (JobDetails jobDetails in _jobs)
            {
                string processorClassId = jobDetails.ProcessorClassId;
                if (!_workflowIdNamesCache.ContainsKey(processorClassId))
                {
                    _workflowIdNamesCache[processorClassId] =
                        DatashopService.Instance.JobService.GetAllStepIdNames(jobDetails.JobId);
                }
            }
        }

        private void SearchJobs()
        {
            // get Data from Formular
            DateTime createDateOld;
            DateTime.TryParse(txbCreateDateOld.Text, out createDateOld);

            DateTime createDateNew;
            DateTime.TryParse(txbCreateDateNew.Text, out createDateNew);

            DateTime stateDateOld;
            DateTime.TryParse(txbStateDateOld.Text, out stateDateOld);

            DateTime stateDateNew;
            DateTime.TryParse(txbStateDateNew.Text, out stateDateNew);

            string reason = ddlReason.SelectedItem.Text;
            string status = ddlStatus.SelectedItem.Text.Equals(string.Empty)
                                ? string.Empty
                                : ((int)Enum.Parse(typeof(WorkflowStepState), ddlStatus.SelectedItem.Text)).ToString();
            string free1 = txbFree1.Text;
            string free2 = txbFree2.Text;
            var whereClause = ConstructWhereClause();


            long? jobId = null;
            if (!txbJobId.Text.Equals(string.Empty))
            {
                jobId = long.Parse(txbJobId.Text);
            }

            _jobs = DatashopService.Instance.JobService.SearchJobsTyped(
                jobId,
                createDateOld,
                createDateNew,
                stateDateOld,
                stateDateNew,
                reason,
                status,
                free1,
                free2,
                ShowArchived,
                ShowNotArchived,
                ViewState["SortExpression"].ToString(),
                (bool)ViewState["SortAscending"],
                int.Parse(ViewState["pageIndex"].ToString()) * int.Parse(ViewState["pageSize"].ToString()),
                int.Parse(ViewState["pageSize"].ToString()),
                whereClause
                );

            FillWorkflowIdNamesCache();
            JobGrid.DataSource = _jobs;
            JobGrid.DataBind();

        }

        private string ConstructWhereClause()
        {
            string free3 = txbFree3.Text;
            string firstName = tbxFirstName.Text;
            string lastName = tbxLastName.Text;
            string userId = txbUserId.Text;
            string machineName = txbMachineName.Text;

            string whereClause = WithoutAdminJobs;
            if (!string.IsNullOrEmpty(free3))
            {
                whereClause += string.Format(", {0} LIKE %{1}%", "Custom3", free3);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                whereClause += string.Format(", {0} LIKE %{1}%", "FirstName", firstName);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                whereClause += string.Format(", {0} LIKE %{1}%", "LastName", lastName);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                whereClause += string.Format(", {0} = {1}", "UserId", userId);
            }

            if (!string.IsNullOrEmpty(machineName))
            {
                whereClause += string.Format(", {0} LIKE %{1}%", "MachineName", machineName);
            }
            return whereClause;
        }

        #endregion //grid data bind

        #region utilities

        protected void JobGridRowCreated(object sender, GridViewRowEventArgs e)
        {
            // event fires when row is created. If it is Footerrow, pagging stuff will be added
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                this.GeneratePageLinks(e.Row.Cells);
            }
        }

        private void GeneratePageLinks(TableCellCollection cells)
        {
            // get Paging values
            int jobAmount;
            if ((bool)ViewState["Searching"])
            {
                string createDateOld = txbCreateDateOld.Text;
                string createDateNew = txbCreateDateNew.Text;
                string stateDateOld = txbStateDateOld.Text;
                string stateDateNew = txbStateDateNew.Text;
                string reason = ddlReason.SelectedItem.Text;
                string status = ddlStatus.SelectedItem.Text.Equals(string.Empty)
                                    ? string.Empty
                                    : ((int)Enum.Parse(typeof(WorkflowStepState), ddlStatus.SelectedItem.Text)).ToString();

                string free1 = txbFree1.Text;
                string free2 = txbFree2.Text;
                long? jobId = null;

                if (!txbJobId.Text.Equals(string.Empty))
                {
                    try
                    {
                        jobId = long.Parse(txbJobId.Text);
                    }
                    catch (FormatException)
                    {
                        jobId = null;
                    }

                }
                string whereClause = ConstructWhereClause();
                jobAmount = DatashopService.Instance.AdminService.GetJobSearchCount(jobId, createDateOld, createDateNew, stateDateOld, stateDateNew, reason, status, free1, free2, ShowArchived, ShowNotArchived, whereClause);
            }
            else
            {
                jobAmount = DatashopService.Instance.AdminService.GetJobCount(ShowArchived, ShowNotArchived, WithoutAdminJobs);
            }
            string pagging = string.Empty;
            double pages = jobAmount / double.Parse(ViewState["pageSize"].ToString());
            bool setBeginTag = true, setEndTag = true;
            const int amountOfLinksVisible = 15;
            int currentPage = int.Parse(ViewState["pageIndex"].ToString());

            for (int i = currentPage - (amountOfLinksVisible / 2); i < (currentPage + (amountOfLinksVisible / 2)) + pages - ((int)pages); i++)
            {
                if (i < 0) setBeginTag = false;
                else if (i >= pages)
                {
                    setEndTag = false;
                    break;
                }
                else if (i == currentPage) pagging += "<i> " + (i + 1) + " </i>";
                else pagging += "<a href=javascript:__doPostBack('Pagging','" + i + "')> " + (i + 1) + " </a>";
            }
            pagging = (setBeginTag
                          ? "<a href=javascript:__doPostBack('Pagging','" + 0 + "')> << </a>.."
                          : string.Empty) + pagging + (setEndTag
                                ? "..<a href=javascript:__doPostBack('Pagging','" + (int)(Math.Ceiling(pages) - 1) + "')> >> </a>"
                                : string.Empty);

            // get amount of cells for colspan
            int cellAmount = cells.Count;
            cells.Clear();
            cells.Add(new TableCell { Text = "<td align=center colspan=" + cellAmount + ">" + pagging + "</td>" });
        }

        protected void DateTimeValidator(object source, ServerValidateEventArgs args)
        {
            DateTime a;
            args.IsValid = DateTime.TryParse(args.Value, out a);
        }

        protected string OnPeriodDateBind(object dateTime)
        {
            if (dateTime == null) return string.Empty;
            string dateString = ((DateTime)dateTime).ToShortDateString();
            return dateString;
        }

        protected string OnStepBind(object step, object processorClassId)
        {
            if ((processorClassId == null))
                return step as string ?? string.Empty;
            if (!_workflowIdNamesCache.ContainsKey((string)processorClassId))
            {
                return string.Empty;
            }

            try
            {
                return _workflowIdNamesCache[(string)processorClassId].ToList().Find(x => (int)step == x.Id).Name;
            }
            catch (Exception)
            {
                return string.Format("Invalid step ({0})", (int)step);
            }
        }

        protected static string OnStateBind(object state)
        {
            if (state == null)
                return string.Empty;
            return Enum.GetName(typeof(WorkflowStepState), (int)state);
        }

        protected string OnProcessorClassIdBind(object processorClassId)
        {
            if (processorClassId == null) return string.Empty;
            var processorClassIdString = (string)processorClassId;
            var splits = processorClassIdString.Split(new[] { '.' });
            if (splits.Length > 0)
            {
                return splits[splits.Length - 1];
            }
            return string.Empty;
        }

        protected string OnSurrogateJobBind(object surrogateJob)
        {
            if (surrogateJob == null)
                return bool.FalseString;
            return bool.TrueString;
        }

        protected bool IsDownloadVisible(Job job)
        {
            if (job == null)
                return false;

            return !string.IsNullOrEmpty(job.JobOutput);
        }

        protected static string OnBigFieldBind(object value)
        {
            if (value == null)
                return string.Empty;

            string valueSmall = value.ToString();

            if (valueSmall.Length > 20)
            {
                valueSmall = valueSmall.Substring(0, 20) + "...";
            }
            return valueSmall;
        }

        #endregion // utilities
    }
}