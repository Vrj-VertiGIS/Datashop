using System.Diagnostics;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
{
    class EmptyWorkflow : WorkflowBase
    {
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
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
