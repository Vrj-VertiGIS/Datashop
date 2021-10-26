using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common.Logging;
using System.Text;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DBStore.Container;
using GEOCOM.GNSD.DBStore.Container.JobData;
using GEOCOM.GNSD.DBStore.Container.UserData;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.DatashopWorkflow
{

    public class AdminWorkflowDataItem : WorkflowDataItemBase
    {
        static AdminWorkflowDataItem()
        {
            DatashopLogInitializer.Initialize();
            Logger = new Msg(typeof(AdminWorkflowDataItem));
        }

        public AdminWorkflowDataItem()
        {
            CenterArea = DatashopWorkflowConfig.Instance.CenterArea;
            Export = DatashopWorkflowConfig.Instance.Export;
            ExtentDataBase = DatashopWorkflowConfig.Instance.ExtentDataBase;
            Formating = DatashopWorkflowConfig.Instance.Formating;
            LetterTemplate = DatashopWorkflowConfig.Instance.LetterTemplate;
            MunicipalityDataBase = DatashopWorkflowConfig.Instance.MunicipalityDataBase;
            NotificationDataBase = DatashopWorkflowConfig.Instance.NotificationDataBase;
        }

        public AdminWorkflowDataItem(ExtentDataBaseInfo extentDataBase, NotificationDataBaseInfo notificationDataBase, MunicipalityDataBaseInfo municipalityDataBase, CenterAreaInfo centerArea, Export export, LetterTemplate letterTemplate, Formating formating, User user, JobGuid jobGuid, string reason)
        {
            ExtentDataBase = extentDataBase;
            NotificationDataBase = notificationDataBase;
            MunicipalityDataBase = municipalityDataBase;
            CenterArea = centerArea;
            Export = export;
            LetterTemplate = letterTemplate;
            Formating = formating;
            User = user;
            JobGuid = jobGuid;
            Reason = reason;
        }

        public override void Init(WorkflowDataItemBase dataItem)
        {
            base.Init(dataItem);

            if (null == User)
            {
                UserStore userStore = new UserStore();
                User = userStore.GetById(Job.UserId);
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

            Variables = new Dictionary<string, string>();

            Variables.Add("adress", User.Address);
            Variables.Add("user_firstname", User.FirstName);
            Variables.Add("user_lastname", User.LastName);
            Variables.Add("user_email", User.Email);
            Variables.Add("user_city", User.City);
            Variables.Add("user_citycode", User.CityCode);
            Variables.Add("user_street", User.Street);
            Variables.Add("user_streetnr", User.StreetNr);
            Variables.Add("user_company", User.Company);
            Variables.Add("user_id", User.UserId.ToString());
            Variables.Add("user_tel", User.Tel);
            Variables.Add("user_fax", User.Fax);
            Variables.Add("job_comment1", Job.Free1);
            Variables.Add("job_comment2", Job.Free2);
            Variables.Add("job_reason", Reason);
            Variables.Add("job_id", Job.JobId.ToString());
            Variables.Add("job_status", Job.Step.ToString());
            string shortDateFormatting = Formating.Date.Short;
            Variables.Add("date", DateTime.Now.ToString(shortDateFormatting));
            string longDateFormatting = Formating.Date.Long;
            Variables.Add("longdate", DateTime.Now.ToString(longDateFormatting));
            Variables.Add("job_period_begin_date", Job.PeriodBeginDate.ToString());
            Variables.Add("job_period_end_date", Job.PeriodEndDate.ToString());
            Variables.Add("job_description", Job.Description);
            Variables.Add("job_parcel_number", Job.ParcelNumber);
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

        public int DownloadCount
        {
            get { return Job.DownloadCount; }
            set { Job.DownloadCount = value; }
        }

        public string Free1
        {
            get { return Job.Free1; }
            set { Job.Free1 = value; }
        }

        public string Free2
        {
            get { return Job.Free2; }
            set { Job.Free2 = value; }
        }

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
            get { return Job.CenterAreaX; }
            set { Job.CenterAreaX = value; }
        }

        #endregion

        #region properties coming from the configuration

        public ExtentDataBaseInfo ExtentDataBase { get; set; }

        public NotificationDataBaseInfo NotificationDataBase { get; set; }

        public MunicipalityDataBaseInfo MunicipalityDataBase { get; set; }

        public CenterAreaInfo CenterArea { get; set; }

        public Export Export { get; set; }

        public LetterTemplate LetterTemplate { get; set; }

        public Formating Formating { get; set; }

        #endregion

        #region Job properties
        public Dictionary<string, string> Variables { get; set; }

        public List<IGeometry> JobExtents { get; set; }

        public User User { get; set; }

        public JobGuid JobGuid { get; set; }

        public string Reason { get; set; }

        public JobDescriptionBaseModel JobDescriptionModel { get; set; }
        #endregion

        public static IMsg Logger { get; set; }
    }
}
