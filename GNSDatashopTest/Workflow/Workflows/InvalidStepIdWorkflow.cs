using System.Diagnostics;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
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
           WorkflowTracker.Instance.TrackThisMethod();
        }

        private void B()
        {
           WorkflowTracker.Instance.TrackThisMethod();
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
           WorkflowTracker.Instance.TrackThisMethod();
        }

        private void B()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }
    }
}
