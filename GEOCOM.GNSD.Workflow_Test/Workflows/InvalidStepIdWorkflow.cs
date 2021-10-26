using System;
using System.Diagnostics;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.Workflow_Test.Workflows
{
    class InvalidStepIdWorkflow1 : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(0, A);  // StepId=0 is reserved for "Start"!!
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

    class InvalidStepIdWorkflow2 : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);  // StepId=1 is not unique!!
            workflowDefinition.AddLast(1, B);
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
