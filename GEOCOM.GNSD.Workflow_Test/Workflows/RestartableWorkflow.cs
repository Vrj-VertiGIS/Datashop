using System.Diagnostics;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.Workflow_Test.Workflows
{
    class RestartableWorkflow : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);
            workflowDefinition.AddLast(2, B);
            workflowDefinition.AddLast(3, Barrier);
            workflowDefinition.AddLast(4, A);
        }

        private void A()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        private void B()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }
    }
}
