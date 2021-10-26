using System;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using GNSDatashopAdmin.Config;
using GNSDatashopAdmin.Helpers;


namespace GNSDatashopAdmin
{
    using System.Collections.Generic;
    using System.Linq;

    



    public partial class UserPage : Page
    {        
        private const int PageSizeDefault = 20;
        private const int PageIndexDefault = 0;
        private readonly string[] _primKeysJob = { "JobId" };
   
        private readonly IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);       
        private string _email;
        private Job[] _jobs;
        private bool _searchByUserId = false;
        private User[] _user;
        private long _userId;

        private Dictionary<string, WorkflowStepIdName[]> _workflowIdNamesCache = new Dictionary<string, WorkflowStepIdName[]>();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                javaScript.Text = string.Empty;
                _email = Request.Params["Email"]; // is null if no Email-Param is found
                try
                {
                    string userId = Request.Params["UserId"];
                    if (userId != null)
                    {
                        this._userId = long.Parse(userId);
                        _searchByUserId = true;
                    }
                }
                catch (FormatException)
                {
                    ErrorLable.Text = "The userID has a wrong format.";
                    JobError.Text = string.Empty;
                    return;
                }

                // User
                if (_searchByUserId)
                {
                    _user = new User[1];
                    try
                    {
                        _user[0] = DatashopService.Instance.JobService.GetUser(this._userId);
                        if (_user[0] == null)
                        {
                            ErrorLable.Text = "The user " + this._userId + " was not found.";
                            JobError.Text = string.Empty;
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        ErrorLable.Text = "The user " + this._userId + " was not found.";
                        JobError.Text = string.Empty;
                        return;
                    }
                }
                else
                {
                    _user = DatashopService.Instance.JobService.GetUserByEmail(_email);
                }

                // check if a user is found
                if (_user.Length == 0)
                {
                    ErrorLable.Text = "The user " + (_searchByUserId ? this._userId.ToString() : _email) + " was not found.";
                    JobError.Text = string.Empty;
                    return;
                }

                if (_user.Length == 1)
                {
                    JobError.Text = "Jobs for this user";
                }

                if (_user.Length > 1)
                {
                    JobError.Text = "Jobs for these users";
                }

                UserGrid.DataSource = _user;
                UserGrid.DataBind();

                // Job

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

                // Write ViewState for Showing ArchiveJobs
                int showArchived = int.Parse(ddlArchive.Items[ddlArchive.SelectedIndex].Value);
                switch (showArchived)
                {
                    case 1:
                        ViewState["ShowArchived"] = false;
                        ViewState["ShowNotArchived"] = true;
                        break;
                    case 2:
                        ViewState["ShowArchived"] = true;
                        ViewState["ShowNotArchived"] = false;
                        break;
                    case 3:
                        ViewState["ShowArchived"] = true;
                        ViewState["ShowNotArchived"] = true;
                        break;
                }

                AspNet.SetupColumnsFromConfig(DatashopWebAdminConfig.Instance.JobListUser, JobGrid);
                
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
                    getAndBindData(false);
                }
                else if (Request.Params.Get("__EVENTTARGET") == "PageSize")
                {
                    try
                    {
                        ViewState["pageSize"] = int.Parse(Request.Params.Get("__EVENTARGUMENT"));
                        ViewState["pageIndex"] = PageIndexDefault;
                    }
                    catch (FormatException)
                    {
                        ViewState["pageIndex"] = PageIndexDefault;
                        ViewState["pageSize"] = PageSizeDefault;
                    }

                    getAndBindData(false);
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        JobGrid.DataKeyNames = _primKeysJob;                                                
                        getAndBindData(false);
                        
                        // formateDefinition();
                        /*JobGrid.DataSource = _jobs;
                        JobGrid.DataBind();*/
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Page_Load {0:s}", ex.Message), ex);

                ErrorLable.Text = "An error occured.";
                JobError.Text = string.Empty;
            }
        }

        private void GetZip(GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow row = ((Control)e.CommandSource).NamingContainer as GridViewRow;

                if (row == null)
                {
                    return;
                }

                int rowIndex = row.RowIndex;

                if (_jobs[rowIndex].IsArchived)
                {
                    javaScript.Text =
                        "<script language=javascript> alert('This job was archived. The map can not be downloaded anymore.')</script>";
                    return;
                }

                string attachFileName = "job_" + _jobs[rowIndex].JobId + ".zip";


                FileInfo file =
                    new FileInfo(_jobs[rowIndex].JobOutput);

                if (!file.Exists)
                {
                    // TODO errormeldung falls file nicht existiert
                    throw new FileNotFoundException();
                }
                else
                {
                    // clear the current output content from the buffer  
                    Response.Clear();

                    // add the header that specifies the default filename for the Download/SaveAs dialog  
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + attachFileName);

                    // add the header that specifies the file size, so that the browser can show the download progress  
                    Response.AddHeader("Content-Length", file.Length.ToString());

                    // specify that the response is a stream that cannot be read by the client and must be downloaded  
                    Response.ContentType = "application/octet-stream";

                    // send the file stream to the client  
                    Response.WriteFile(file.FullName);

                    Response.End();

                    // _log.Info("User " + _user.UserID + " (" + _user.Firstname + ", " + _user.Lastname + ") did download job " + _job.JobID + " to " + Request.Params["REMOTE_ADDR"]);
                }
            }
            catch (Exception)
            {
                javaScript.Text = "<script language=javascript> alert('The file was not found.')</script>";
            }
        }

        protected void JobGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // look for custom events
            if (e.CommandName.Equals("DownloadMap"))
            {
                GetZip(e);
            }
        }

        protected void JobGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Change Sortdirection if same label is clicked twice
            if (ViewState["SortExpression"].ToString().Equals(e.SortExpression))
            {
                ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
            }
            ViewState["SortExpression"] = e.SortExpression;

            getAndBindData(false);
        }

        private void getAndBindData(bool checkIfIsPageBack)
        {
            bool showArchived = (bool)ViewState["ShowArchived"];
            bool showNotArchived = (bool)ViewState["ShowNotArchived"];
            string SortExpression = ViewState["SortExpression"].ToString();
            bool sortAscending = (bool)ViewState["SortAscending"];
            int startIndex = int.Parse(ViewState["pageIndex"].ToString()) * int.Parse(ViewState["pageSize"].ToString());
            int quantity = int.Parse(ViewState["pageSize"].ToString());

            string whereClause;            

            if (_searchByUserId)
            {
                whereClause = "UserId = " + this._userId;            
                
            }
            else
            {
                whereClause = "Email LIKE " + _email;            
            }

            _jobs = DatashopService.Instance.JobService.GetJobDetailsSorted(showArchived, showNotArchived, SortExpression, sortAscending, startIndex, quantity, whereClause);
            
            if ((_jobs != null) && (_jobs.Length > 0))
            {
                IList<JobDetails> _jobsWithoutAdminJob = new List<JobDetails>();

                foreach (JobDetails jobDetail in _jobs)
                {
                    if (!jobDetail.ProcessorClassId.Equals(WorkflowDefinitions.AdminWorkflowClassId))
                    {
                        if (jobDetail.UserId == this._userId)
                        {
                            _jobsWithoutAdminJob.Add(jobDetail);   
                        }                        
                    }
                }

                JobGrid.DataSource = _jobsWithoutAdminJob;

                foreach (JobDetails jobDetails in _jobsWithoutAdminJob)
                {
                    string processorClassId = jobDetails.ProcessorClassId;
                    if (!_workflowIdNamesCache.ContainsKey(processorClassId))
                    {
                        _workflowIdNamesCache[processorClassId] = DatashopService.Instance.JobService.GetAllStepIdNames(jobDetails.JobId);
                    }
                }

                JobError.Text = string.Format("{0} job(s) for this user.", _jobs.Count());
            }
            else
            {
                JobError.Text = "No jobs found for this user.";
            }
            

            if (checkIfIsPageBack)
            {
                if (!Page.IsPostBack)
                {
                    JobGrid.DataBind();
                }
            }
            else
            {
                JobGrid.DataBind();
            }
        }

        protected string OnStepBind(Object step, Object processorClassId)
        {
            if ((step == null) || (processorClassId == null))
                return string.Empty;
            if (!_workflowIdNamesCache.ContainsKey((string)processorClassId))
            {
                return string.Empty;
            }

            try
            {
                return (_workflowIdNamesCache[(string)processorClassId].ToList().Find(x => (int)step == x.Id)).Name;
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

        protected void ArchiveIndexChange(object sender, EventArgs e)
        {
            int showArchived = int.Parse(ddlArchive.Items[ddlArchive.SelectedIndex].Value);
            switch (showArchived)
            {
                case 1:
                    ViewState["ShowArchived"] = false;
                    ViewState["ShowNotArchived"] = true;
                    break;
                case 2:
                    ViewState["ShowArchived"] = true;
                    ViewState["ShowNotArchived"] = false;
                    break;
                case 3:
                    ViewState["ShowArchived"] = true;
                    ViewState["ShowNotArchived"] = true;
                    break;
            }
            getAndBindData(false);
        }

        protected void JobGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            // event fires when row is created. If it is Footerrow, pagging stuff will be added
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                this.GeneratePageLinks(e.Row.Cells);

            }
        }
        private void GeneratePageLinks(TableCellCollection cells)
        {
            string whereClause = "UserId = " + this._userId;
            
            //get Paging values
            int jobAmount = DatashopService.Instance.AdminService.GetJobCount((bool)ViewState["ShowArchived"], (bool)ViewState["ShowNotArchived"], whereClause);

            String pagging = "";
            double pages = (double)(jobAmount) / double.Parse(ViewState["pageSize"].ToString());
            bool setBeginTag = true, setEndTag = true;
            const int amountOfLinksVisible = 15;
            int currentPage = int.Parse(ViewState["pageIndex"].ToString());

            for (int i = currentPage - amountOfLinksVisible / 2; i < (currentPage + amountOfLinksVisible / 2) + pages - ((int)pages); i++)
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
                          : "") + pagging + (setEndTag
                                ? "..<a href=javascript:__doPostBack('Pagging','" + (int)(Math.Ceiling(pages) - 1) + "')> >> </a>"
                                : "");

            int cellAmount = cells.Count;//get amount of cells for colspan
            cells.Clear();
            TableCell pCell = new TableCell();
            pCell.Text = "<td align=center colspan=" + cellAmount + ">" + pagging + "</td>";
            cells.Add(pCell);
        }

    }
} 