using System.Diagnostics;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Attributes;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
{
    class NameWorkflow : WorkflowBase
    {
        public const string Name =  "name from the workflow definition";
        protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            workflowDefinition.AddLast(1, A);
            workflowDefinition.AddLast(2, B);
            workflowDefinition.AddLast(3, C);
            workflowDefinition.AddLast(4, Barrier);
            workflowDefinition.AddLast(5, D);
            workflowDefinition.AddLast(6, E, Name);
        }

        [WorkflowStepName("Step A")]
        private void A()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }

        [WorkflowStepName(null)]
        private void B()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }

        [WorkflowStepName("")]
        private void C()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }

        private void D()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }  
        
        private void E()
        {
           WorkflowTracker.Instance.TrackThisMethod();
        }

       
    }
}
