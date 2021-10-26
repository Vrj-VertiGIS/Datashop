using System.Diagnostics;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.Workflow_Test.Workflows
{
    public class NormalWorkflow : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);
            workflowDefinition.AddLast(2, B);
            workflowDefinition.AddLast(3, A);
            workflowDefinition.AddLast(4, C);
        }

        [WorkflowStepName("my method")]
        private void A()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        private void B()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        private void C()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }
    }
}
