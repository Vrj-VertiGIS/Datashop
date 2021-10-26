using GEOCOM.GNSD.Workflow.DataObjects;

namespace GEOCOM.GNSD.Workflow.Interfaces
{
	/// <summary>
	/// Intercepts each step of a workflow influence its execution.
	/// </summary>
	public interface IWorkflowStepInterceptor
	{
		/// <summary>
		/// Decides whether the workflow should stop execution after the step.
		/// </summary>
		/// <param name="step">The current workflow step data.</param>
		/// <param name="dataItem">The data item of the <see cref="WorkflowDataItemBase"/> base type.</param>
		/// <returns>
		/// True to stop the workflow after this step or false to continue.
		/// </returns>
		bool StopAfterStep(IWorkflowStep step, IWorkflowDataItem dataItem);
	}
}