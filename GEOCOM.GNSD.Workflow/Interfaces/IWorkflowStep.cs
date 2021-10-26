using GEOCOM.GNSD.Workflow.Delegates;

namespace GEOCOM.GNSD.Workflow.Interfaces
{
	/// <summary>
	/// Description of a workflow step
	/// </summary>
	public interface IWorkflowStep
	{
		/// <summary>
		/// Gets id of the workflow step.
		/// </summary>
		int StepId { get; }

		/// <summary>
		/// Gets name of the workflow step.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets delegate that is executed when the workflow step is run.
		/// </summary>
		WorkflowFunction Function { get;}

		/// <summary>
		/// Gets conditions that detetermins whether the workflow step should be skipped or run.
		/// </summary>
		WorkflowCondition SkipStepCondition { get; }
	}
}