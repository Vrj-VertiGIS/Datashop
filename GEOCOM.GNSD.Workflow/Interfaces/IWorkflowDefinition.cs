using GEOCOM.GNSD.Workflow.Delegates;

namespace GEOCOM.GNSD.Workflow.Interfaces
{
    public interface IWorkflowDefinition
    {
        void AddLast(int stepId, WorkflowFunction function);

        void AddLast(int stepId, WorkflowFunction function, string name);

        void AddLast(int stepId, WorkflowFunction function, WorkflowCondition skipStepCondition);

        void AddLast(int stepId, WorkflowFunction function, WorkflowCondition skipStepCondition, string name);
    }
}
