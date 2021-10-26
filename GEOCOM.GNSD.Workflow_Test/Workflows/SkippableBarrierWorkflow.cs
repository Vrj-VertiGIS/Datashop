using System.Diagnostics;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.Workflow_Test.Workflows
{
    class SkippableBarrierWorkflow : WorkflowBase
    {
    
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);
            workflowDefinition.AddLast(2, B);
            workflowDefinition.AddLast(3, Barrier, SkipBarrier);
            workflowDefinition.AddLast(4, A);
            workflowDefinition.AddLast(5, B);
            workflowDefinition.AddLast(6, Barrier, SkipBarrier2);
            workflowDefinition.AddLast(7, A);
            workflowDefinition.AddLast(8, B);
            workflowDefinition.AddLast(9, Barrier);
            workflowDefinition.AddLast(10, A);
        }

        protected bool SkipBarrier()
        {
            return true;
        }

        protected bool SkipBarrier2()
        {
            return false;
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
