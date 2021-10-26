using GEOCOM.GNSDatashop.Model.JobData;

namespace GEOCOM.GNSD.Workflow.DataObjects
{
	public class WorkflowDataItemBase : IWorkflowDataItem
	{
        private Job _job;

        public Job Job
        {
            get { return _job; }
            set { _job = value; }
        }

        public virtual void Init(WorkflowDataItemBase dataItem)
        {
            _job = dataItem._job;
        }
    }
}