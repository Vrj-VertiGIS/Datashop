using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.DatashopWorkflow
{
	/// <summary>
	/// Defines xml configuration, workflow step, plot reason, and user role dependent interception for the <see cref="DatashopWorkflowBase"/>.
	/// </summary>
	public class DatashopWorkflowStepInterceptor : IWorkflowStepInterceptor
	{
		#region Private fields

		/// <summary>
		/// Interception criteria by names of workflow steps.
		/// </summary>
		private readonly Dictionary<string, List<InterceptionCriterion>> _stopAfterCriteria = new Dictionary<string, List<InterceptionCriterion>>();

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="DatashopWorkflowStepInterceptor" /> class.
		/// </summary>
		/// <param name="workflowStepsDomain">The all allowed names of workflow steps.</param>
		/// <param name="interceptionSettings">The interception settings.</param>
		public DatashopWorkflowStepInterceptor(string[] workflowStepsDomain, WorkflowInterceptionSettings interceptionSettings)
		{
			SortOutStopCriteria(workflowStepsDomain, interceptionSettings);
		}

		/// <summary>
		/// Decides whether the workflow should stop execution after the step.
		/// </summary>
		/// <param name="step">The current workflow step data.</param>
		/// <param name="dataItem">The data item of the <see cref="IDatashopWorkflowDataItem" /></param>
		/// <returns>
		/// True to stop the workflow after this step or false to continue.
		/// </returns>
		public bool StopAfterStep(IWorkflowStep step, IWorkflowDataItem dataItem)
		{
			var data = dataItem as IDatashopWorkflowDataItem;
			if (data == null || step == null)
				return false;

			var stopAfterCriteria = GetStopAfterStepCriteriaByStep(step);
			if (stopAfterCriteria == null)
				return false;

			var roles = (data.User.BizUser != null ? data.User.BizUser.Roles ?? string.Empty : "tempUser").Split(',');

			foreach (var stopAfterCriterion in stopAfterCriteria)
			{
				foreach (var role in roles)
				{
					var matches = stopAfterCriterion.Matches((int)data.ReasonId, data.Reason, role.Trim(), step.Name);
					if (matches)
						return true;
				}
			}

			return false;
		}

		#region Private methods

		private void SortOutStopCriteria(string[] workflowStepsDomain, WorkflowInterceptionSettings interceptionSettings)
		{
			if (interceptionSettings == null || interceptionSettings.StopCriteria == null)
				return;

			foreach (var stopCriterion in interceptionSettings.StopCriteria)
			{
				CheckAttributes(stopCriterion);

				CheckStepName(workflowStepsDomain, stopCriterion);

				var criterion = new InterceptionCriterion(stopCriterion.Reason, stopCriterion.UserRole, stopCriterion.StopAfterStepName);

				List<InterceptionCriterion> stopAfterCriteria;
				bool criteriaFound = _stopAfterCriteria.TryGetValue(stopCriterion.StopAfterStepName.ToLower(), out stopAfterCriteria);
				if (!criteriaFound)
				{
					stopAfterCriteria = new List<InterceptionCriterion>();
					_stopAfterCriteria.Add(stopCriterion.StopAfterStepName.ToLower(), stopAfterCriteria);
				}

				stopAfterCriteria.Add(criterion);
			}
		}

		private static void CheckStepName(string[] workflowStepsDomain, StopCriterion stopCriterion)
		{
			bool stepFromSettingDoesNotExists = !workflowStepsDomain.Any(
									step => step.Equals(stopCriterion.StopAfterStepName, StringComparison.OrdinalIgnoreCase));
			
			if (stepFromSettingDoesNotExists)
			{
				string message = string.Format(
					"Step '{0}' is not a valid workflow step name. Possible values are '{1}'.",
					stopCriterion.StopAfterStepName, 
					string.Join(", ", workflowStepsDomain));
				throw new ConfigurationErrorsException(message);
			}
		}

		private static void CheckAttributes(StopCriterion stopCriterion)
		{
			bool someAttrEmpty = string.IsNullOrEmpty(stopCriterion.Reason) ||
			                     string.IsNullOrEmpty(stopCriterion.UserRole) ||
			                     string.IsNullOrEmpty(stopCriterion.StopAfterStepName);
			if (someAttrEmpty)
			{
				string message =
					string.Format(
						"Invalid configuration of workflow interception. Some attributes are empty: reason='{0}', userrole='{1}', stopafter='{2}'.",
						stopCriterion.Reason, 
						stopCriterion.UserRole, 
						stopCriterion.StopAfterStepName);
				throw new ConfigurationErrorsException(message);
			}
		}

		private IEnumerable<InterceptionCriterion> GetStopAfterStepCriteriaByStep(IWorkflowStep step)
		{
			List<InterceptionCriterion> stopAfterCriteria;
			bool criterionFound = _stopAfterCriteria.TryGetValue(step.Name.ToLower(), out stopAfterCriteria);
			if (!criterionFound)
				return null;

			return stopAfterCriteria;
		}

		#endregion
	}
}
