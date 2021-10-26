using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Attributes;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GNSDatashopTest.Workflow.Workflows
{
	public class TestInterceptor : IWorkflowStepInterceptor
	{
		public bool StopAfterStep(IWorkflowStep step, IWorkflowDataItem workflowDataItemBase)
		{
			return step.StepId == 3;
		}
	}

	public class InterceptedWorkflow : WorkflowBase
	{
		protected override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
		{
			workflowDefinition.AddLast(1, A);
			workflowDefinition.AddLast(2, B);
			workflowDefinition.AddLast(3, A); // TestInterceptor stops the workflow afert this step
			workflowDefinition.AddLast(4, C);
		}

		protected override IWorkflowStepInterceptor GetWorkflowInterceptor()
		{
			return new TestInterceptor(); // define the interceptor
		}

		[WorkflowStepName("my method")]
		private void A()
		{
			WorkflowTracker.Instance.TrackThisMethod();
		}

		private void B()
		{
			WorkflowTracker.Instance.TrackThisMethod();
		}

		private void C()
		{
			WorkflowTracker.Instance.TrackThisMethod();
		}
	}

	public class InterceptedWorkflowWithOnStopHandler : InterceptedWorkflow
	{
		protected override void OnStoppedAfterStep(IWorkflowStep workflowStep)
		{
			WorkflowTracker.Instance.TrackThisMethod();
		} 
	}

	public class NonInterceptedWorkflowWithOnStopHandler : InterceptedWorkflowWithOnStopHandler
	{
		protected override IWorkflowStepInterceptor GetWorkflowInterceptor()
		{
			return null;
		}
	}
}