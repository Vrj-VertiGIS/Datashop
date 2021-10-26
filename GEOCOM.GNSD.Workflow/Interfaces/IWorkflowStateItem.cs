using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSD.Workflow.Interfaces
{
	public interface IWorkflowStateItem
	{
		int WorkflowStepId { get; set; }
		bool IsActive { get; set; }
		bool NeedsProcessing { get; set; }
		long Id { get; set; }
		long ProcessId { get; set; }
		string AssemblyFullName { get; set; }
		string ClassId { get; set; }
		JobStore JobStore { get; set; }
		WorkflowStepState CurrentWorkflowStepState { get; set; }

		void Update();
	}
}