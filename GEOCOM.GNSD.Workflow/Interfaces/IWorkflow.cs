
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSD.Workflow.Interfaces
{
    public interface IWorkflow
    {
        bool IsActive { get; }

        void Run();

        void Activate();

        void SetFailed();

        string GetWorkflowStepNameById(int workflowStepId);

        int[] GetAllRestartableStepIds();

        WorkflowStepIdName[] GetAllStepIdNames();

        WorkflowStepIdName[] GetAllRestartableStepIdNames();

        WorkflowStepIdName GetNextStepIdName();

        void RestartFromStep(int workflowStepId);
    }
}
