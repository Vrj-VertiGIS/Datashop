#pragma warning disable 660,661
using GEOCOM.GNSD.Workflow.Delegates;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.Workflow.DataObjects
{
	internal class WorkflowStep : IWorkflowStep
	{
		public WorkflowFunction Function { get; set; }

		public WorkflowCondition SkipStepCondition { get; set; }

		public string Name { get; set; }

		public int StepId { get; set; }

		public WorkflowStep(int stepId, WorkflowFunction function, WorkflowCondition skipStepCondition, string name)
		{
			StepId = stepId;
			Function = function;
			SkipStepCondition = skipStepCondition;
			Name = name ?? function.Method.Name;
		}

		public static bool operator ==(WorkflowStep workflowStep, WorkflowFunction workflowFunction)
		{
			// casting to object is necessary to avoid recursive call via the != operator 
			return (object)workflowStep != null  && workflowStep.Function == workflowFunction;
		}

		public static bool operator !=(WorkflowStep workflowStep, WorkflowFunction workflowFunction)
		{
			return !(workflowStep == workflowFunction);
		}
	}
}
