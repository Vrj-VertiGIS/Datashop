using System;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.DataObjects;
using Export = GEOCOM.GNSD.PlotExtension.Config.Export;

namespace GEOCOM.GNSD.DatashopWorkflow.Admin
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
            ExportConfig = PlotExtensionConfig.Instance.Export;
        }

        public AdminWorkflowDataItem(Export export)
        {
            ExportConfig = export;
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

        public string Custom3
        {
            get { return Job.Custom3; }
            set { Job.Custom3 = value; }
        }
        public string Custom4 { get { return Job.Custom4; } set { Job.Custom4 = value; } }
        public string Custom5 { get { return Job.Custom5; } set { Job.Custom5 = value; } }
        public string Custom6 { get { return Job.Custom6; } set { Job.Custom6 = value; } }
        public string Custom7 { get { return Job.Custom7; } set { Job.Custom7 = value; } }
        public string Custom8 { get { return Job.Custom8; } set { Job.Custom8 = value; } }
        public string Custom9 { get { return Job.Custom9; } set { Job.Custom9 = value; } }
        public string Custom10 { get { return Job.Custom10; } set { Job.Custom10 = value; } }

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

        #region config properties
        public Export ExportConfig { get; set; }
        #endregion

        #region Job properties
        public JobDescriptionBaseModel JobDescriptionModel { get; set; }
        #endregion

        public override void Init(WorkflowDataItemBase dataItem)
        {
            base.Init(dataItem);

            JobDescriptionModel = JobDescriptionBaseModel.Deserialize(Job.Definition);
        }

        public static IMsg Logger { get; set; }
    }
}
