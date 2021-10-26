using System;

namespace GEOCOM.GNSD.DatashopWorkflow
{
	/// <summary>
	/// Represents an interception criterion of a workflow
	/// </summary>
	public class InterceptionCriterion
	{
		private int ReasonId { get; set; } 

		private string Reason { get; set; }

		private string UserRole { get; set; }

		private string StepName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptionCriterion" /> class.
		/// </summary>
		/// <param name="reason">The job reason (either id or name - in DB table gnsd_reasons) for which a step in the parameter <param name="stepName"></param> should be intercepted. May be '*' match all values.</param>
		/// <param name="userRole">The user role for which a step in the parameter <param name="stepName"></param> should be intercepted. May be '*' match all values.</param>
		/// <param name="stepName">Name of the step that should be intercepted.</param>
		public InterceptionCriterion(string reason, string userRole, string stepName)
		{
			int resultId;
			bool reasonIdParsed = int.TryParse(reason, out resultId);
			ReasonId = reasonIdParsed ? resultId : -1;
			Reason = reason.ToLower();
			UserRole = userRole.ToLower();
			StepName = stepName.ToLower();
		}
		
		/// <summary>
		/// Matches the input parameters against the values set in the constructor
		/// </summary>
		/// <param name="reasonId">The reason id.</param>
		/// <param name="reason">The reason.</param>
		/// <param name="userRole">The user role.</param>
		/// <param name="stepName">Name of the step.</param>
		/// <returns>True if they match otherwise false.</returns>
		public bool Matches(int reasonId, string reason, string userRole, string stepName)
		{
			bool reasonMatches = ReasonId == reasonId || Reason.Equals(reason, StringComparison.OrdinalIgnoreCase) || Reason == "*";
			bool userRoleMatches = UserRole.Equals(userRole,StringComparison.OrdinalIgnoreCase) || UserRole == "*";
			bool stepNameMatches = StepName.Equals(stepName, StringComparison.OrdinalIgnoreCase) || StepName == "*";
			bool matches = reasonMatches && userRoleMatches && stepNameMatches;

			return matches;
		}
	}
}