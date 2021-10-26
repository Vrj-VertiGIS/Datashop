using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GEOCOM.GNSD.DatashopWorkflow;
using NUnit.Framework;

namespace GNSDatashopTest.Workflow
{
	[TestFixture]
	public class InterceptionCriterionTest
	{
		[Test]
		public void ReasonStringAndMatches()
		{
			string reason = "ReAsOn";
			string userRole = "UsErRoLe";
			string stepName = "sTePnAmE";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.True(matches);
		}

		[Test]
		public void ReasonStringAndDoNotMatch()
		{
			string reason = "xxx";
			string userRole = "UsErRoLe";
			string stepName = "sTePnAmE";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.False(matches);
		}

		[Test]
		public void ReasonIntAndMatches()
		{
			string reason = "1";
			string userRole = "UsErRoLe";
			string stepName = "sTePnAmE";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.True(matches);
		}

		[Test]
		public void ReasonIntAndDoNotMatch()
		{
			string reason = "2";
			string userRole = "UsErRoLe";
			string stepName = "sTePnAmE";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.False(matches);
		}

		[Test]
		public void UserRoleDoNotMatch()
		{
			string reason = "1";
			string userRole = "xxx";
			string stepName = "sTePnAmE";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.False(matches);
		}

		[Test]
		public void StepNameDoNotMatch()
		{
			string reason = "1";
			string userRole = "UsErRoLe";
			string stepName = "xxxx";
			var interceptionCriterion = new InterceptionCriterion(reason, userRole, stepName);
			var matches = interceptionCriterion.Matches(1, "Reason", "userRole", "stepName");
			Assert.False(matches);
		}
	}
}
