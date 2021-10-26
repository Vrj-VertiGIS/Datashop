using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.Common.Security;
using GEOCOM.GNSD.Web.Core.DocumentStreaming;
using GEOCOM.GNSD.Web.Core.JSSerializer;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using GEOCOM.GNSDatashop.Model.Documents;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;
using GEOCOM.GNSDatashop.ServiceContracts;
using GNSDatashopAdmin.Config;
using GNSDatashopAdmin.Controls;
using GNSDatashopAdmin.Helpers;

namespace GNSDatashopAdmin
{
    public partial class JobDetailDynamic : Page
    {
        #region Private Members

        // log4net
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        private long _jobId;
        private JobDetails job;
        private string xmlJobDefinition;
        private User user;
        private WorkflowStepIdName[] _workflowStepIdNames;

        // dynamic report page variables
        private DsDynamicReportConfig config = null;
        private DsDynamicReportData formData = new DsDynamicReportData();
        private XDocument htmlLogs;
        private DsDynamicReport report;
        private Panel surrogateUserControl;
        private Panel userCommentControl;
        private Panel jobControlControl;
        private Panel fileUploadControl;
        private Panel jobDownloadControl;
        private Panel eventLogControl;
        private Panel jobMapControl;

        // controls in dynamic report
        // user comment
        private TextBox txbInfo;
        private Button btnInfo;
        private RegularExpressionValidator RegularExpressionValidator1;
        // file upload
        private FileUpload fuFileUpload;
        private Button btnUploadFile;
        private PlaceHolder uploadOk;
        private PlaceHolder uploadFail;
        private Label lblUploadStatus;
        // job control
        private Button btnActivateJob;
        private Button btnRestart;
        private DropDownList ddlRestartableSteps;
        // map
        private Label lblTemplateError;

        #endregion

        #region Public Properties

        public string MapServiceUrl
        {
            get
            {
                return DatashopWebAdminConfig.Instance.MapService.ServiceUrl;
            }
        }

        public string JobDetailLayoutConfigFile
        {
            get { return DatashopWebAdminConfig.Instance.JobDetailLayoutConfigFile; }
        }

        /// <summary>
        /// Gets the admin map configuration path.
        /// </summary>
        /// <value>
        /// The admin map configuration path.
        /// </value>
        public string AdminWebMapConfigurationPath
        {
            get { return DatashopWebAdminConfig.Instance.MapService.AdminWebMapConfigurationPath; }
        }

        /// <summary>
        /// Gets a value indicating whether [use web map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use web map]; otherwise, <c>false</c>.
        /// </value>
        public string UseAdminWebMap
        {
            get { return (!string.IsNullOrWhiteSpace(AdminWebMapConfigurationPath) && File.Exists(this.AdminWebMapConfigurationPath)).ToString(); }
        }

        /// <summary>
        /// Gets the admin map configuration.
        /// </summary>
        /// <value>
        /// The admin map configuration.
        /// </value>
        public string AdminWebMapConfiguration
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.AdminWebMapConfigurationPath))
                {
                    return File.ReadAllText(this.AdminWebMapConfigurationPath);
                }
                return null;
            }
            
        }

        public string PolygonsJson { get; set; }

        #endregion

        #region Page Event Lifecycle

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();

            javaScript.Text = "";

            try
            {
                try
                {
                    _jobId = long.Parse(Request.Params["JobId"]);
                }
                catch (FormatException)
                {
                    jobError.Text = "The jobID has the wrong format.";
                    return;
                }
                catch (ArgumentNullException)
                {
                    jobError.Text = "No jobID specified.";
                    return;
                }

                var serializer = new XmlSerializer<DsDynamicReportConfig>();
                try
                {
                    config = serializer.Deserialize(JobDetailLayoutConfigFile);
                }
                catch (Exception ex)
                {
                    jobError.Text = string.Format("Error while reading the report configuration file '{0}'. Reason: {1}", JobDetailLayoutConfigFile, ex.Message);
                    return;
                }
                serializer = null;
                //////config = JobDetailLayout;

                // get the job and user data
                GetAllData();

                // load the dynamic report
                report = DsDynamicReportFactory.CreateDynamicReport(config, formData);
                divDSDR.Controls.Add(report);

                // check the report
                try
                {
                    surrogateUserControl = (Panel)report.FindControlRecursive("SurrogateUser");

                    // if (userCommentControl == null) throw new Exception(string.Format(errorMessage, "SurrogateUser"));
                    userCommentControl = (Panel)report.FindControlRecursive("UserComment");

                    // if (userCommentControl == null) throw new Exception(string.Format(errorMessage, "UserComment"));
                    jobControlControl = (Panel)report.FindControlRecursive("JobControl");

                    // if (jobControlControl == null) throw new Exception(string.Format(errorMessage, "JobControl"));
                    fileUploadControl = (Panel)report.FindControlRecursive("FileUpload");

                    // if (fileUploadControl == null) throw new Exception(string.Format(errorMessage, "FileUpload"));
                    jobDownloadControl = (Panel)report.FindControlRecursive("JobDownload");

                    // if (jobDownloadControl == null) throw new Exception(string.Format(errorMessage, "JobDownload"));
                    eventLogControl = (Panel)report.FindControlRecursive("EventLog");

                    // if (eventLogControl == null) throw new Exception(string.Format(errorMessage, "EventLog"));
                    jobMapControl = (Panel)report.FindControlRecursive("JobMap");

                    // if (jobMapControl == null) throw new Exception(string.Format(errorMessage, "JobMap"));
                }
                catch (Exception exp2)
                {
                    jobError.Text = "Dynamic Report: " + exp2.Message;
                }

                // add logic and controls to the report
                AddControlsToReport();

                // prepare final rendering, after event processing
                //this.PreRender += new EventHandler(JobDetailDynamic_PreRender);

            }
            catch (Exception exp)
            {
                jobError.Text = "General error 1: " + exp.Message + exp.StackTrace;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (surrogateUserControl != null)
                surrogateUserControl.Visible = job.SurrogateJob != null;

            btnActivateJob.Enabled = job.IsActive == false;

            if (jobControlControl != null)
                jobControlControl.Visible = !job.IsArchived;

            if (userCommentControl != null)
                userCommentControl.Visible = !job.IsArchived;

            if (fileUploadControl != null)
            {
                PlotJobSteps step = (PlotJobSteps)job.Step;
                // PreProcess creates the job directory
                // Package zips all the file
                // It makes only sense to allow upload between these steps
                var filesCanBeUploaded = PlotJobSteps.PreProcess < step && step < PlotJobSteps.Package;
                fileUploadControl.Visible = filesCanBeUploaded && !job.IsActive;
                var declarations = string.Format("var fuFileUploadId = '{0}';\nvar lblUploadStatusId = '{1}';\n", fuFileUpload.ClientID, lblUploadStatus.ClientID);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "", declarations, true);
            }
            try
            {
                if (job.IsActive)
                {
                    btnActivateJob.Visible = false;
                }
                else
                {
                    btnActivateJob.Visible = true;
                    var jobService = DatashopService.Instance.JobService;
                    WorkflowStepIdName nextStep = jobService.GetNextStepIdName(job.JobId);

                    // this is a little hack to avoid showing "Continue to next step Waiting for additional documents" 
                    // on the activate button when it is not necessary.
                    var shouldNOTWaitForDocuments = job.SurrogateJob == null || !job.SurrogateJob.StopAfterProcess;
                    var nextStepIsWaitForDocuments = nextStep.Id == (int) PlotJobSteps.WaitForDocuments;
                    string nameOfNextStep = nextStep.Name;
                    if (nextStepIsWaitForDocuments && shouldNOTWaitForDocuments)
                    {
                        int idOfStepAfterWaitForDocuments = (int) PlotJobSteps.WaitForDocuments + 1;
                        nameOfNextStep= jobService.GetWorkflowStepNameById(job.JobId, idOfStepAfterWaitForDocuments);
                    }


                    btnActivateJob.Text = string.Format("Continue to next step '{0}'", nameOfNextStep);
                }
            }
            catch (Exception)
            {
                btnActivateJob.Text = string.Empty;
                btnActivateJob.Visible = false;
            }
        }

        #endregion

        #region Private Methods

        private void GetAllData()
        {
            job = DatashopService.Instance.JobService.GetJobDetailsById(_jobId);

            if (job != null)
            {
                xmlJobDefinition = job.Definition ?? string.Empty;
                FormateDefinition(job);

                _workflowStepIdNames = DatashopService.Instance.JobService.GetAllStepIdNames(job.JobId);

                // Job information</div>
                formData.Add("JobId", job.JobId);
                formData.Add("UserId", job.UserId);
                formData.Add("ReasonId", job.ReasonId);
                formData.Add("Reason", job.Reason ?? string.Empty);
                formData.Add("Definition", job.Definition ?? string.Empty);
                formData.Add("Description", job.Description ?? string.Empty);
                formData.Add("ParcelNumber", job.ParcelNumber ?? string.Empty);
                formData.Add("Custom1", job.Custom1 ?? string.Empty);
                formData.Add("Custom2", job.Custom2 ?? string.Empty);
                formData.Add("Custom3", job.Custom3 ?? string.Empty);
                formData.Add("Custom4", job.Custom4 ?? string.Empty);
                formData.Add("Custom5", job.Custom5 ?? string.Empty);
                formData.Add("Custom6", job.Custom6 ?? string.Empty);
                formData.Add("Custom7", job.Custom7 ?? string.Empty);
                formData.Add("Custom8", job.Custom8 ?? string.Empty);
                formData.Add("Custom9", job.Custom9 ?? string.Empty);
                formData.Add("Custom10",job.Custom10 ?? string.Empty);
                formData.Add("DxfExport",job.DxfExport ? "Yes" : "No");
                formData.Add("Municipality", job.Municipality ?? string.Empty);
                formData.Add("CenterAreaX", job.CenterAreaX);
                formData.Add("CenterAreaY", job.CenterAreaY);
                formData.Add("DownloadCount", job.DownloadCount);
                formData.Add("MapExtentCount", job.MapExtentCount);
                formData.Add("IsArchived", job.IsArchived);
                formData.Add("GeoAttachmentsEnabled", job.GeoAttachmentsEnabled);

                if (job.PeriodBeginDate != null)
                    formData.Add("PeriodBeginDate", job.PeriodBeginDate);
                else
                    formData.Add("PeriodBeginDate", string.Empty);

                if (job.PeriodEndDate != null)
                    formData.Add("PeriodEndDate", job.PeriodEndDate);
                else
                    formData.Add("PeriodEndDate", string.Empty);

                if (job.CreateDate != null)
                    formData.Add("Createdate", job.CreateDate);
                else
                    formData.Add("Createdate", string.Empty);

                if (job.LastStateChangeDate != null)
                    formData.Add("LastStateChangeDate", job.LastStateChangeDate);
                else
                    formData.Add("LastStateChangeDate", job.LastStateChangeDate);

                //    Job system information</div>
                formData.Add("Step", job.Step);
                formData.Add("StepName", _workflowStepIdNames.ToList().Find(x => job.Step == x.Id).Name);
                formData.Add("State", job.State);
                formData.Add("StateName", Enum.GetName(typeof(WorkflowStepState), job.State));
                formData.Add("IsActive", job.IsActive);
                formData.Add("NeedsProcessing", job.NeedsProcessing);
                formData.Add("JobOutput", job.JobOutput ?? string.Empty);
                formData.Add("ProcessId", job.ProcessId ?? 0);
                formData.Add("MachineName", job.MachineName);
                formData.Add("ProcessorClassId", job.ProcessorClassId ?? string.Empty);

                if (job.SurrogateJob != null)
                {
                    //    Surrogate job information</div>
                    formData.Add("SurrogateUserId", job.SurrogateJob.SurrogateUserId);
                    formData.Add("RequestType", job.SurrogateJob.RequestType ?? string.Empty);
                    formData.Add("StopAfterProcess", job.SurrogateJob.StopAfterProcess);

                    if (job.SurrogateJob != null)
                        formData.Add("RequestDate", job.SurrogateJob.RequestDate);
                    else
                        formData.Add("RequestDate", string.Empty);
                }

                //get logs
                var logs = DatashopService.Instance.AdminService.GetLogsForJob(_jobId);

                if (logs != null && logs.Length > 0)
                {
                    // VRJ: I am deeply sorry for this messy code
                    // however WIA goodies (Alain Willisau call his code this way)
                    // proofed them self as uneditable and I was forced into this:
                    // Here we make the job log table collapsable if number of 
                    // log entries belonging to a workflow step exceeds minimumCountOfSameStepKindsToEnableToggle

                    const int minimumCountOfSameStepKindsToEnableToggle = 5;

                    // tracking of lenght of log entries belonging to same step
                    JobLog lastStepKindLog = logs.First();
                    int lastStepKindLogIndex = 0;
                    XElement lastStepKindElem = null;

                    var elements = logs.Select((currentLog, currentLogIndex) =>
                         {
                             var elem = new XElement("Log",
                                    new XAttribute("Timestamp", currentLog.Timestamp.ToString()),
                                    new XAttribute("Step", (currentLog.Step == null) ? string.Empty : _workflowStepIdNames.ToList().Find(x => currentLog.Step == x.Id).Name),
                                    new XAttribute("State", (currentLog.State == null) ? string.Empty : Enum.GetName(typeof(WorkflowStepState), currentLog.State)),
                                    new XAttribute("Message", currentLog.Message ?? string.Empty),
                                    new XAttribute("NewStepKind", "false"),
                                    new XAttribute("TogglesCollapse", "false"));

                             // lastStepKindElem is null for the first element
                             lastStepKindElem = lastStepKindElem ?? elem;
                             // log entries change from a step to another step (preprocess -> process)
                             bool stepKindChanged = lastStepKindLog.Step != currentLog.Step;
                             bool firstIteration = currentLog == lastStepKindLog;
                             if (stepKindChanged || firstIteration)
                             {
                                 elem.Attribute("NewStepKind").SetValue("true");
                                 var countOfLastStepKind = currentLogIndex - lastStepKindLogIndex;
                                 var enableToggle = countOfLastStepKind > minimumCountOfSameStepKindsToEnableToggle;
                                 if (enableToggle)
                                     lastStepKindElem.Attribute("TogglesCollapse").SetValue("true");

                                 lastStepKindElem = elem;
                                 lastStepKindLog = currentLog;
                                 lastStepKindLogIndex = currentLogIndex;
                             }

                             elem.Add(new XAttribute("Group", lastStepKindLog.Id));

                             return elem;
                         }).ToArray();

                    // this is for the last step kind
                    var switchToggleForTheVeryLastStepKindOn = logs.Length - lastStepKindLogIndex > minimumCountOfSameStepKindsToEnableToggle;
                    if (switchToggleForTheVeryLastStepKindOn)
                        lastStepKindElem.Attribute("TogglesCollapse").SetValue("true");

                    var xmlLogs = new XDocument(new XElement("Logs", elements));

                    htmlLogs = new XDocument();

                    using (XmlWriter writer = htmlLogs.CreateWriter())
                    {
                        // Load the style sheet.
                        var xslt = new XslCompiledTransform();
                        xslt.Load(Server.MapPath("~/xslt/JobEventLog.xslt"));
                        // Execute the transform and output the results to a writer.
                        xslt.Transform(xmlLogs.CreateReader(), writer);
                    }
                }
                else
                    LogError.Text = "No logs found.";

                //get and bind User
                try
                {
                    user = DatashopService.Instance.JobService.GetUser(job.UserId);

                    if (user != null)
                    {
                        formData.Add("Salutation", user.Salutation ?? string.Empty);
                        formData.Add("FirstName", user.FirstName ?? string.Empty);
                        formData.Add("LastName", user.LastName ?? string.Empty);
                        formData.Add("Email", user.Email ?? string.Empty);
                        formData.Add("Street", user.Street ?? string.Empty);
                        formData.Add("StreetNr", user.StreetNr ?? string.Empty);
                        formData.Add("CityCode", user.CityCode ?? string.Empty);
                        formData.Add("City", user.City ?? string.Empty);
                        formData.Add("Company", user.Company ?? string.Empty);
                        formData.Add("Tel", user.Tel ?? string.Empty);
                        formData.Add("Fax", user.Fax ?? string.Empty);
                        formData.Add("BizUserId", user.BizUserId ?? 0);
                    }
                    else
                        UserError.Text = "The user was not found.";
                }
                catch (Exception)
                {
                    UserError.Text = "The user was not found.";
                }
            }
            else
                jobError.Text = "The job with Id " + Request.Params["JobId"] + " was not found.";
        }

        private void AddControlsToReport()
        {

            //„UserComment“
            if (userCommentControl != null)
            {
                txbInfo = new TextBox();
                txbInfo.ID = "txbInfo";
                txbInfo.TextMode = TextBoxMode.MultiLine;
                txbInfo.CssClass = "txbInfo";
                btnInfo = new Button();
                btnInfo.ID = "btnInfo";
                btnInfo.Text = "Add a comment";
                btnInfo.CssClass = "btnInfo";
                btnInfo.Click += new EventHandler(btnInfoOnClick);
                // AWI: someone never heard about TextBox.MaxLength...
                RegularExpressionValidator1 = new RegularExpressionValidator();
                RegularExpressionValidator1.ID = "RegularExpressionValidator1";
                RegularExpressionValidator1.ErrorMessage = "Maximal 500 caracters allowed.";
                RegularExpressionValidator1.ControlToValidate = txbInfo.ID;
                RegularExpressionValidator1.ValidationExpression = "^.{1,500}$";
                var div = new HtmlGenericControl("div");
                userCommentControl.Controls.Add(div);
                div.Controls.Add(txbInfo);
                div.Controls.Add(new LiteralControl("<br />"));
                div.Controls.Add(btnInfo);
                div.Controls.Add(RegularExpressionValidator1);
            }

            //„JobControl“
            if (jobControlControl != null)
            {
                WorkflowStepIdName[] indicesNames = DatashopService.Instance.JobService.GetAllRestartableStepIdNames(job.JobId);
                IList<WorkflowStepIdName> finalNames = new List<WorkflowStepIdName>();
                foreach (WorkflowStepIdName item in indicesNames)
                {
                    if ((item.Id != (int)PlotJobSteps.WaitForDocuments) && (item.Id != (int)PlotJobSteps.JobDone))
                    {
                        finalNames.Add(item);
                    }
                }

                var div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "divJobControl");
                jobControlControl.Controls.Add(div);
                btnActivateJob = new Button();
                btnActivateJob.ID = "btnActivateJob";
                btnActivateJob.Text = "Continue to next Step";
                btnActivateJob.ToolTip = "Reactivate the job. The job workflow will continue until the next barrier.";
                btnActivateJob.CssClass = "jobDetail_actionbutton";
                btnActivateJob.Click += new EventHandler(OnClickbtnActivate);
                div.Controls.Add(btnActivateJob);
                var lblRestart = new Label();
                lblRestart.Text = "Restart Step: ";
                lblRestart.CssClass = "lblRestart";
                div.Controls.Add(lblRestart);
                ddlRestartableSteps = new DropDownList();
                ddlRestartableSteps.ID = "ddlRestartableSteps";
                ddlRestartableSteps.DataTextField = "Name";
                ddlRestartableSteps.DataValueField = "Id";
                ddlRestartableSteps.ToolTip = "Select the restarting step";
                div.Controls.Add(ddlRestartableSteps);
                if (!IsPostBack)
                {
                    ddlRestartableSteps.DataSource = finalNames;
                    ddlRestartableSteps.DataBind();
                }
                btnRestart = new Button();
                btnRestart.ID = "btnRestart";
                btnRestart.Text = "Restart";
                btnRestart.ToolTip = "Restarts selected step";
                btnRestart.CssClass = "jobDetail_actionbutton btnRestart";
                btnRestart.Click += new EventHandler(OnClickbtnRestart);
                div.Controls.Add(btnRestart);
            }

            //„FileUpload“
            if (fileUploadControl != null)
            {
                var div = new HtmlGenericControl("div");
                fileUploadControl.Controls.Add(div);
                fuFileUpload = new FileUpload();
                fuFileUpload.ID = "fuFileUpload";
                fuFileUpload.ToolTip = "Select file to upload";
                div.Controls.Add(fuFileUpload);
                btnUploadFile = new Button();
                btnUploadFile.ID = "btnUploadFile";
                btnUploadFile.Text = "Upload File";
                btnUploadFile.ToolTip = "Uploads the selected file";
                btnUploadFile.CssClass = "jobDetail_actionbutton btnUploadFile";
                btnUploadFile.OnClientClick = "return checkFileSelected();";
                btnUploadFile.Click += new EventHandler(OnClickbtnUploadFile);
                div.Controls.Add(btnUploadFile);
                var img = new HtmlGenericControl("img");
                img.Attributes.Add("id", "imgWaiting");
                img.Attributes.Add("src", "images/wait.gif");
                img.Style.Add("display", "none");
                div.Controls.Add(img);
                uploadOk = new PlaceHolder();
                uploadOk.ID = "uploadOk";
                uploadOk.Visible = false;
                var imgUploadOk = new HtmlGenericControl("img");
                imgUploadOk.Attributes.Add("id", "imgUploadOk");
                imgUploadOk.Attributes.Add("src", "images/accept.png");
                uploadOk.Controls.Add(imgUploadOk);
                div.Controls.Add(uploadOk);
                uploadFail = new PlaceHolder();
                uploadFail.ID = "uploadFail";
                uploadFail.Visible = false;
                var imgUploadFailed = new HtmlGenericControl("img");
                imgUploadFailed.Attributes.Add("id", "imgUploadFailed");
                imgUploadFailed.Attributes.Add("src", "images/cancel.gif");
                uploadFail.Controls.Add(imgUploadFailed);
                lblUploadStatus = new Label();
                lblUploadStatus.ID = "lblUploadStatus";
                uploadFail.Controls.Add(lblUploadStatus);
                div.Controls.Add(uploadFail);
            }

            //„JobDownload“
            if (jobDownloadControl != null)
            {
                var div = new HtmlGenericControl("div");
                jobDownloadControl.Controls.Add(div);
                var lnkbtnMap = new ImageButton();
                lnkbtnMap.ID = "lnkbtnMap";
                lnkbtnMap.CommandName = "DownloadMap";

                lnkbtnMap.Click += OnLinkButtonClick;
                lnkbtnMap.Enabled = job.Step > (int) PlotJobSteps.Package; // this allows to download the file even if the send step fails
                lnkbtnMap.ImageUrl = lnkbtnMap.Enabled ? "~/images/download_16.gif" : "~/images/download_16_bw.gif";
                lnkbtnMap.ToolTip =  lnkbtnMap.Enabled ? "Download zip-file" : "Download not yet available";
                div.Controls.Add(lnkbtnMap);
            }


            var collapseAll = new HyperLink();
            collapseAll.Text = "Collapse all";
            collapseAll.Attributes.Add("onclick", "CollapseAll()");
            collapseAll.Style.Add("cursor", "pointer");

            var openAll = new HyperLink();
            openAll.Text = "Open all";
            openAll.Attributes.Add("onclick", "OpenAll()");
            openAll.Style.Add("cursor", "pointer");

            eventLogControl.Controls.Add(collapseAll);
            var delimiter = new Literal();
            delimiter.Text = " | ";

            eventLogControl.Controls.Add(delimiter);
            eventLogControl.Controls.Add(openAll);
            eventLogControl.Controls.Add(new HtmlGenericControl("br"));

            //„EventLog“
            if (eventLogControl != null)
            {
                if (htmlLogs != null)
                {
                    var lit = new Literal();
                    lit.Text = htmlLogs.ToString();
                    eventLogControl.Controls.Add(lit);
                }
            }

            //„JobMap“
            if (jobMapControl != null)
            {
                ExportModel model = GetModelFromXmlDefinition(xmlJobDefinition);
                bool showMap = model != null;
                if (showMap)
                {
                    //plcHldMap.Visible = true;
                    PolygonsJson = ExportModelJSSerializer.SerializeExtendModel(model);
                    PolygonsJson = PolygonsJson.Replace("\"", "'");
                }
                var div = new HtmlGenericControl("div");
                jobMapControl.Controls.Add(div);
                var litContent = "<div id=\"navToolbar\" dojotype=\"dijit.Toolbar\" class=\"divMapToolbar\">"
                        + "<div dojotype=\"dijit.form.ToggleButton\" id=\"zoomIn\" iconclass=\"zoomInIcon\" onclick=\"onZoomIn()\">"
                        + "</div>"
                        + "<span id=\"zoomin_tt\" dojotype=\"dijit.Tooltip\" connectid=\"zoomIn\" position=\"above\">Zoom In</span>"
                        + "<div dojotype=\"dijit.form.ToggleButton\" id=\"zoomOut\" iconclass=\"zoomOutIcon\" onclick=\"onZoomOut()\">"
                        + "</div>"
                        + "<span id=\"zoomout_tt\" dojotype=\"dijit.Tooltip\" connectid=\"zoomOut\" position=\"above\">Zoom Out</span>"
                        + "<div dojotype=\"dijit.form.ToggleButton\" id=\"pan\" iconclass=\"panIcon\" onclick=\"onPan()\">"
                        + "</div>"
                        + "<span id=\"pan_tt\" dojotype=\"dijit.Tooltip\" connectid=\"pan\" position=\"above\">Pan</span>"
                    + "</div>"
                    + "<div id=\"map\" class=\"divMap\">"
                    + "</div>";
                var lit = new Literal();
                lit.Text = litContent;
                jobMapControl.Controls.Add(lit);
                lblTemplateError = new Label();
                lblTemplateError.ID = "lblTemplateError";
                lblTemplateError.CssClass = "lblTemplateError";
                jobMapControl.Controls.Add(lblTemplateError);
            }
        }

        private void FormateDefinition(Job job)
        {
            try
            {
                try
                {
                    var model = JobDescriptionBaseModel.Deserialize(job.Definition);

                    if (model != null)
                        job.Definition = model.ToString();
                    else
                        job.Definition = "(null)"; // TODO
                }
                catch (Exception exp)
                {
                    job.Definition = exp.Message;
                }
            }
            catch (Exception exp)
            {
                exp.ToString();
                job.Definition = "Could not read definition.";
            }
        }

        private ExportModel GetModelFromXmlDefinition(string exportDefinitionXML)
        {
            return JobDescriptionBaseModel.Deserialize(exportDefinitionXML) as ExportModel;
        }

        private bool UploadFile()
        {
            bool isUploadSuccessful = false;
            using (Stream inputStream = fuFileUpload.FileContent)
            {
                DocumentUpload documentUpload = new DocumentUpload();
                documentUpload.Contents = inputStream;
                documentUpload.FileName = fuFileUpload.FileName;
                documentUpload.Length = inputStream.Length;
                documentUpload.JobId = job.JobId;
                documentUpload.UserId = user.UserId;

                try
                {
                    DatashopService.Instance.DocumentService.SaveDocument(documentUpload);
                    uploadOk.Visible = true;
                    uploadFail.Visible = false;
                    isUploadSuccessful = true;
                    lblUploadStatus.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    uploadOk.Visible = false;
                    uploadFail.Visible = true;
                    lblUploadStatus.Text = ex.Message;
                }
            }

            return isUploadSuccessful;
        }

        private void UpdateJobInfoAndLog(bool isUploadSuccessful)
        {
            String fileName = fuFileUpload.FileName;
            String logMsg;
            IJobManager jobService = DatashopService.Instance.JobService;

            if (isUploadSuccessful)
            {
                logMsg = String.Format("Uploaded file={0} userId={1}", fileName, job.UserId);
            }
            else
            {
                logMsg = String.Format("Upload file={0} userId={1} failed", fileName, job.UserId);
            }
            jobService.AddJobLogByJobId(job.JobId, logMsg);
            _log.Debug(logMsg);
        }

        private void DownloadZip()
        {
            try
            {
                if (job.IsArchived)
                {
                    javaScript.Text = "<script language=javascript> alert('This job was archived. The map can not be downloaded anymore.')</script>";
                    return;
                }

                var documentStreamer = new DocumentStreamer(job, Response);
                documentStreamer.CopyDocumentToRespose();

            }
            catch (Exception exp)
            {
                if (!(exp is System.Threading.ThreadAbortException)) // exp comes from Response.End();
                    javaScript.Text = "<script language=javascript> alert('Downlaod error: " + HttpUtility.JavaScriptStringEncode(exp.Message) + "')</script>";
            }
        }

        private void ReloadPage()
        {
            string url;
            if (string.IsNullOrEmpty(lastActiveTabId.Value))
                url = string.Format("JobDetailDynamic.aspx?JobID={0}", _jobId);
            else
                url = string.Format("JobDetailDynamic.aspx?JobID={0}&lastActiveTabId={1}", _jobId, lastActiveTabId.Value);
            Response.Redirect(url, false);
        }

        #endregion

        #region Event Handlers

        protected void OnClickbtnRefreshPage(object sender, EventArgs e)
        {
            ReloadPage();
        }

        protected void btnInfoOnClick(object sender, Object e)
        {
            if (txbInfo.Text.Equals(""))
            {
                javaScript.Text = "<script language=javascript> alert('Empty comments are not allowed.')</script>";
                return;
            }
            DatashopService.Instance.JobService.AddJobLogByJobId(_jobId, txbInfo.Text);
            ReloadPage();
        }

        protected void OnClickbtnUploadFile(object sender, EventArgs e)
        {
            if (fuFileUpload.HasFile)
            {
                bool isUploadSuccessful = UploadFile();
                UpdateJobInfoAndLog(isUploadSuccessful);
            }
        }

        protected void OnClickbtnActivate(object sender, EventArgs e)
        {
            DatashopService.Instance.JobService.ActivateJob(_jobId);
            ReloadPage();
        }

        protected void OnClickbtnRestart(object sender, EventArgs e)
        {
            var step = ddlRestartableSteps.SelectedItem.Value;
            DatashopService.Instance.JobService.RestartJobFromStep(_jobId, int.Parse(step));
            ReloadPage();
        }

        protected void OnLinkButtonClick(object sender, EventArgs e)
        {
            DownloadZip();
        }

        #endregion
    }
}