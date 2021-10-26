using System;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
{
    class ExceptionWorkflow : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);
            workflowDefinition.AddLast(2, B);
            workflowDefinition.AddLast(3, Barrier);
            workflowDefinition.AddLast(4, A);
        }

        protected override void OnError(Exception exception)
        {
           WorkflowTracker.Instance.TrackThisMethod();            
        }

        private void A()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }

        private void B()
        {
           WorkflowTracker.Instance.TrackThisMethod();
            throw new Exception();
        }
    }
}
