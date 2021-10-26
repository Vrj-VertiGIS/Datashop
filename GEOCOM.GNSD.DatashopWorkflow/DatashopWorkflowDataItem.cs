using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.DatashopWorkflow
{
	public interface IDatashopWorkflowDataItem : IWorkflowDataItem
	{
		long JobId { get; set; }
		long UserId { get; set; }
		long ReasonId { get; set; }
		string Definition { get; set; }
		string JobOutput { get; set; }
		long? ProcessId { get; set; }
		long? ProcessingUserId { get; set; }
		bool IsArchived { get; set; }
		int DownloadCount { get; set; }
		int MapExtentCount { get; set; }
		string Custom1 { get; set; }
		string Custom2 { get; set; }
		string Custom3 { get; set; }
		string Custom4 { get; set; }
		string Custom5 { get; set; }
		string Custom6 { get; set; }
		string Custom7 { get; set; }
		string Custom8 { get; set; }
		string Custom9 { get; set; }
		string Custom10 { get; set; }
		DateTime? PeriodBeginDate { get; set; }
		DateTime? PeriodEndDate { get; set; }
		string Description { get; set; }
		string ParcelNumber { get; set; }
		string Municipality { get; set; }
		double CenterAreaX { get; set; }
		double CenterAreaY { get; set; }
		SurrogateJob SurrogateJob { get; set; }
		ExtentDataBaseInfo ExtentDataBase { get; set; }
		NotificationDataBaseInfo NotificationDataBase { get; set; }
		ExtractionInfo Extraction { get; set; }
		CenterAreaInfo CenterArea { get; set; }
		string JobdocumentsDirectory { get; set; }
		LetterTemplate LetterTemplate { get; set; }
		PlotFileName PlotFileName { get; set; }
		Dictionary<string, string> Variables { get; set; }
		User User { get; set; }
		User SurrogateUser { get; set; }
		JobGuid JobGuid { get; set; }
		string Reason { get; set; }
		bool DxfExport { get; set; }
        JobDescriptionBaseModel JobDescriptionModel { get; set; }
	}

	public class DatashopWorkflowDataItem : WorkflowDataItemBase, IDatashopWorkflowDataItem
	{
	    static DatashopWorkflowDataItem()
        {
            DatashopLogInitializer.Initialize();
            Logger = new Msg(typeof(DatashopWorkflowDataItem));
        }

        public DatashopWorkflowDataItem()
        {
            CenterArea = DatashopWorkflowConfig.Instance.CenterArea;
            JobdocumentsDirectory = GnsDatashopCommonConfig.Instance.Directories.JobdocumentsDirectory;            
            ExtentDataBase = DatashopWorkflowConfig.Instance.ExtentDataBase;
            LetterTemplate = DatashopWorkflowConfig.Instance.LetterTemplate;
            Extraction = DatashopWorkflowConfig.Instance.Extraction;
            NotificationDataBase = DatashopWorkflowConfig.Instance.NotificationDataBase;
            this.PlotFileName = DatashopWorkflowConfig.Instance.PlotFileName;
            WorkflowInterceptionSettings = DatashopWorkflowConfig.Instance.WorkflowInterceptionSettings;
        }

        #region properties coming from a database

        public long JobId
        {
            get { return Job.JobId; }
            set { Job.JobId = value; }
        }

        public long UserId
        {
            get { return Job.UserId; }
            set { Job.UserId = value; }
        }

        public long ReasonId
        {
            get { return Job.ReasonId; }
            set { Job.ReasonId = value; }
        }

        public string Definition
        {
            get { return Job.Definition; }
            set { Job.Definition = value; }
        }

        public string JobOutput
        {
            get { return Job.JobOutput; }
            set { Job.JobOutput = value; }
        }

        public long? ProcessId
        {
            get { return Job.ProcessId; }
            set { Job.ProcessId = value; }
        }

        public long? ProcessingUserId
        {
            get { return Job.ProcessingUserId; }
            set { Job.ProcessingUserId = value; }
        }

        public bool IsArchived
        {
            get { return Job.IsArchived; }
            set { Job.IsArchived = value; }
        }

        public bool GeoAttachmentsEnabled
        {
            get { return Job.GeoAttachmentsEnabled; }
            set { Job.GeoAttachmentsEnabled = value; }
        }

        public int DownloadCount
        {
            get { return Job.DownloadCount; }
            set { Job.DownloadCount = value; }
        }

        public int MapExtentCount
        {
            get { return Job.MapExtentCount; }
            set { Job.MapExtentCount = value; }
        }

        public string Custom1
        {
            get { return Job.Custom1; }
            set { Job.Custom1 = value; }
        }

        public string Custom2
        {
            get { return Job.Custom2; }
            set { Job.Custom2 = value; }
        }

        public string Custom3{get { return Job.Custom3; }set { Job.Custom3 = value; }}
        public string Custom4{get { return Job.Custom4; }set { Job.Custom4 = value; }}
        public string Custom5{get { return Job.Custom5; }set { Job.Custom5 = value; }}
        public string Custom6{get { return Job.Custom6; }set { Job.Custom6 = value; }}
        public string Custom7{get { return Job.Custom7; }set { Job.Custom7 = value; }}
        public string Custom8{get { return Job.Custom8; }set { Job.Custom8 = value; }}
        public string Custom9{get { return Job.Custom9; }set { Job.Custom9 = value; }}
        public string Custom10{get { return Job.Custom10; }set { Job.Custom10 = value; }}

        public DateTime? PeriodBeginDate
        {
            get { return Job.PeriodBeginDate; }
            set { Job.PeriodBeginDate = value; }
        }

        public DateTime? PeriodEndDate
        {
            get { return Job.PeriodEndDate; }
            set { Job.PeriodEndDate = value; }
        }

        public string Description
        {
            get { return Job.Description; }
            set { Job.Description = value; }
        }

        public string ParcelNumber
        {
            get { return Job.Description; }
            set { Job.Description = value; }
        }

        public string Municipality
        {
            get { return Job.Municipality; }
            set { Job.Municipality = value; }
        }

        public double CenterAreaX
        {
            get { return Job.CenterAreaX; }
            set { Job.CenterAreaX = value; }
        }

        public double CenterAreaY
        {
            get { return Job.CenterAreaY; }
            set { Job.CenterAreaY = value; }
        }

        public SurrogateJob SurrogateJob
        {
            get { return Job.SurrogateJob; }
            set { Job.SurrogateJob = value; }
        }

	    public bool DxfExport
        {
	        get { return Job.DxfExport; }
	        set { Job.DxfExport = value; }
	    }

        #endregion

        #region properties coming from the configuration

        public ExtentDataBaseInfo ExtentDataBase { get; set; }

        public NotificationDataBaseInfo NotificationDataBase { get; set; }

        public ExtractionInfo Extraction { get; set; }

        public CenterAreaInfo CenterArea { get; set; }

        public string JobdocumentsDirectory { get; set; }

        public LetterTemplate LetterTemplate { get; set; }

        public PlotFileName PlotFileName { get; set; }

	    public WorkflowInterceptionSettings WorkflowInterceptionSettings { get; set; }

	    #endregion

        #region Job properties
        public Dictionary<string, string> Variables { get; set; }

        public User User { get; set; }

        public User SurrogateUser { get; set; }

        public JobGuid JobGuid { get; set; }

        public string Reason { get; set; }

        public JobDescriptionBaseModel JobDescriptionModel { get; set; }
        #endregion

        public static IMsg Logger { get; set; }

        public override void Init(WorkflowDataItemBase dataItem)
        {
            base.Init(dataItem);

            if (null == User)
            {
                UserStore userStore = new UserStore();
                User = userStore.GetById(Job.UserId);
            }

            if ((null == SurrogateUser) && (null != Job.SurrogateJob))
            {
                UserStore userStore = new UserStore();
                SurrogateUser = userStore.GetById(Job.SurrogateJob.SurrogateUserId);
            }

            if (null == JobGuid)
            {
                JobGuidStore jobGuidStore = new JobGuidStore();
                JobGuid = jobGuidStore.GetOrCreateByJobId(Job.JobId);
            }

            if (null == Reason)
            {
                ReasonsStore reasonStore = new ReasonsStore();
                Reason reason = reasonStore.Get(Job.ReasonId);
                if (reason != null)
                {
                    Reason = reason.Description;
                }
            }

            JobDescriptionModel = JobDescriptionBaseModel.Deserialize(Job.Definition);

            InitializeVariables();
        }

        private void InitializeVariables()
        {
            Variables = new Dictionary<string, string>
                {
                    { "date", DateTime.Now.ToString("d") },
                    { "longdate", DateTime.Now.ToString("D") },
                    { "time", DateTime.Now.ToString("t") },
                    { "longtime", DateTime.Now.ToString("T") }
                };

            InitUserVariables("user", User);

            Variables.Add("job_custom1", Job.Custom1);
            Variables.Add("job_custom2", Job.Custom2);
            Variables.Add("job_custom3", Job.Custom3);
            Variables.Add("job_custom4", Job.Custom4);
            Variables.Add("job_custom5", Job.Custom5);
            Variables.Add("job_custom6", Job.Custom6);
            Variables.Add("job_custom7", Job.Custom7);
            Variables.Add("job_custom8", Job.Custom8);
            Variables.Add("job_custom9", Job.Custom9);
            Variables.Add("job_custom10", Job.Custom10);
            Variables.Add("job_reason", Reason);
            Variables.Add("job_id", Job.JobId.ToString());
            Variables.Add("job_guid", JobGuid.Guid);
            Variables.Add("job_step", Job.Step.ToString());
            Variables.Add("job_status", Job.State.ToString());
            Variables.Add("job_period_begin_date", Utils.Utils.DateFormat(Job.PeriodBeginDate));
            Variables.Add("job_period_end_date", Utils.Utils.DateFormat(Job.PeriodEndDate));
            Variables.Add("job_description", Job.Description);
            Variables.Add("job_parcel_number", Job.ParcelNumber);
            Variables.Add("job_map_extent_count", Job.MapExtentCount.ToString());

            if (Job.SurrogateJob != null)
            {
                InitUserVariables("surrogateuser", SurrogateUser);
                Variables.Add("surrogatejob_request_date", Utils.Utils.DateFormat(Job.SurrogateJob.RequestDate));
                Variables.Add("surrogatejob_request_type", Job.SurrogateJob.RequestType);
            }
            else
            {
                InitUserVariables("surrogateuser", null);
                Variables.Add("surrogatejob_request_date", string.Empty);
                Variables.Add("surrogatejob_request_type", string.Empty);
            }
            
            Variables.Add("job_export_dxf", Job.DxfExport.ToString());
        }

        private void InitUserVariables(string prefix, User user)
        {
            if (user == null)
            {
                user = new User();
                user.UserId = -1;
            }

            Variables.Add(string.Format("{0}_fullname", prefix), string.Format("{0} {1}", user.FirstName, user.LastName));
            Variables.Add(string.Format("{0}_salutation", prefix), user.Salutation);
            Variables.Add(string.Format("{0}_firstname", prefix), user.FirstName);
            Variables.Add(string.Format("{0}_lastname", prefix), user.LastName);
            Variables.Add(string.Format("{0}_email", prefix), user.Email);
            Variables.Add(string.Format("{0}_city", prefix), user.City);
            Variables.Add(string.Format("{0}_citycode", prefix), user.CityCode);
            Variables.Add(string.Format("{0}_street", prefix), user.Street);
            Variables.Add(string.Format("{0}_streetnr", prefix), user.StreetNr);
            Variables.Add(string.Format("{0}_company", prefix), user.Company);
            string userId = user.UserId != -1 ? user.UserId.ToString() : string.Empty;
            Variables.Add(string.Format("{0}_id", prefix), userId);
            Variables.Add(string.Format("{0}_tel", prefix), user.Tel);
            Variables.Add(string.Format("{0}_fax", prefix), user.Fax);            
        }
    }
}
