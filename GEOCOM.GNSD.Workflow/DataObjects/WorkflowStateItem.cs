using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GEOCOM.GNSD.Workflow.DataObjects
{
	public sealed class WorkflowStateItem : IWorkflowStateItem
	{
        private Job _job;

        public JobStore JobStore { get; set; }

        public WorkflowStateItem(Job job)
        {
            _job = job;
        }

        public string AssemblyFullName { get; set; }

        public string ClassId
        {
            get { return _job.ProcessorClassId; }
            set { _job.ProcessorClassId = value; }
        }

        public WorkflowStepState CurrentWorkflowStepState
        {
            get { return (WorkflowStepState)_job.State; }
            set { _job.State = (int)value; }
        }

        public long Id
        {
            get { return _job.JobId; }
            set { _job.JobId = value; }
        }

        public int WorkflowStepId
        {
            get { return _job.Step; }
            set { _job.Step = value; }
        }

        public bool IsActive
        {
            get { return _job.IsActive; }
            set { _job.IsActive = value; }
        }

        public bool NeedsProcessing
        {
            get { return _job.NeedsProcessing; }
            set { _job.NeedsProcessing = value; }
        }

        public long ProcessId
        {
            get { return _job.ProcessId ?? 0; }
            set { _job.ProcessId = value; }
        }

        public void Update()
        {
            if (JobStore == null)
            {
                JobStore = new JobStore();
            }
            JobStore.Update(_job);
        }
    }
}