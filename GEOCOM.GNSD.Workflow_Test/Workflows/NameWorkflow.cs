using System.Diagnostics;
using GEOCOM.GNSD.Workflow;

namespace GEOCOM.GNSD.Workflow_Test.Workflows
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
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        [WorkflowStepName(null)]
        private void B()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        [WorkflowStepName("")]
        private void C()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

        private void D()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }  
        
        private void E()
        {
            WorkflowTracker.Instance.MethodsCalled.Add((new StackTrace(true)).GetFrame(0).GetMethod().Name);
        }

       
    }
}
