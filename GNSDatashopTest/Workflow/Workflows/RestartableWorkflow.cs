using System.Diagnostics;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
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
           WorkflowTracker.Instance.TrackThisMethod();
        }

        private void B()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }
    }
}
