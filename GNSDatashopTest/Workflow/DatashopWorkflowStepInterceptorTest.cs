using System.Configuration;
using GEOCOM.GNSD.DatashopWorkflow;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Interfaces;
using Moq;
using NUnit.Framework;

namespace GNSDatashopTest.Workflow
{
	[TestFixture]
	public class DatashopWorkflowStepInterceptorTest
	{
		[Test]
		public void StopAfterStepReturnsTrue()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = "reason1", UserRole = "user1RoleA", StopAfterStepName = "step1" },
					new StopCriterion { Reason = "2", UserRole = "user2RoleA", StopAfterStepName = "step2" }, // we test step2
					new StopCriterion { Reason = "reason2", UserRole = "user3RoleA", StopAfterStepName = "step2" } // additional condition criterion for step2
				};

			// mock workflow step information 
			var workflowStep = Mock.Of<IWorkflowStep>();
			Mock.Get(workflowStep).SetupGet(step => step.Name).Returns("step2");

			// mock workflow data
			var dataItem = Mock.Of<IDatashopWorkflowDataItem>();
			Mock.Get(dataItem).SetupGet(item => item.ReasonId).Returns(2);
			Mock.Get(dataItem).SetupGet(item => item.Reason).Returns("reason2");
			Mock.Get(dataItem).SetupGet(item => item.User.BizUser.Roles).Returns("user2RoleA, user2RoleB");

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(workflowStep, dataItem);

			// assert
			Assert.True(stopAfterStep);
		}

		[Test]
		public void StopAfterStepUsingAsteriskReturnsTrue()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = "reason1", UserRole = "user1RoleA", StopAfterStepName = "Step1" },
					new StopCriterion { Reason = "*", UserRole = "*", StopAfterStepName = "Step2" } // we test Step2
				};

			// mock workflow step information 
			var workflowStep = Mock.Of<IWorkflowStep>();
			Mock.Get(workflowStep).SetupGet(step => step.Name).Returns("steP2");

			// mock workflow data
			var dataItem = Mock.Of<IDatashopWorkflowDataItem>();
			Mock.Get(dataItem).SetupGet(item => item.ReasonId).Returns(2);
			Mock.Get(dataItem).SetupGet(item => item.Reason).Returns("reason2");
			Mock.Get(dataItem).SetupGet(item => item.User.BizUser.Roles).Returns("user2RoleA, user2RoleB");

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(workflowStep, dataItem);

			// assert
			Assert.True(stopAfterStep);
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void StopAfterStepInvalidStepNameThrows()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					// invalid name of step
					new StopCriterion { Reason = "reason1", UserRole = "user1RoleA", StopAfterStepName = "step1_invalid_name" },
				};

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(Mock.Of<IWorkflowStep>(), Mock.Of<IDatashopWorkflowDataItem>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void StopAfterStepEmptyReasonThrows()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = null, UserRole = "user1RoleA", StopAfterStepName = "step1" },
				};

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(Mock.Of<IWorkflowStep>(), Mock.Of<IDatashopWorkflowDataItem>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void StopAfterStepEmptyUserRoleThrows()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = "reason1", UserRole = null, StopAfterStepName = "step1" },
				};

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(Mock.Of<IWorkflowStep>(), Mock.Of<IDatashopWorkflowDataItem>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void StopAfterStepStepCannotBeAsteriskThrows()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = "reason1", UserRole = "user1RoleA", StopAfterStepName = "*" },
				};

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(Mock.Of<IWorkflowStep>(), Mock.Of<IDatashopWorkflowDataItem>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void StopAfterStepEmptyStopAfterThrows()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = new[]
				{
					new StopCriterion { Reason = "reason1", UserRole = "user1RoleA", StopAfterStepName = null },
				};

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new[] { "step1", "step2" }, interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(Mock.Of<IWorkflowStep>(), Mock.Of<IDatashopWorkflowDataItem>());
		}

		[Test]
		public void StopAfterStepNoStopCriteriaReturnsFalse()
		{
			// settings coming from config xml - no settings
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = null;

			// mock workflow step information 
			var workflowStep = Mock.Of<IWorkflowStep>();
			Mock.Get(workflowStep).SetupGet(step => step.Name).Returns("xxx");

			// mock workflow data
			var dataItem = Mock.Of<IDatashopWorkflowDataItem>();
			Mock.Get(dataItem).SetupGet(item => item.ReasonId).Returns(2);
			Mock.Get(dataItem).SetupGet(item => item.Reason).Returns("reason2");
			Mock.Get(dataItem).SetupGet(item => item.User.BizUser.Roles).Returns("user2RoleA, user2RoleB");

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new string[0], interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(workflowStep, dataItem);

			// assert
			Assert.False(stopAfterStep);
		}

		[Test]
		public void StopAfterStepReturnsFalls()
		{
			// settings coming from config xml
			var interceptionSettings = new WorkflowInterceptionSettings();
			interceptionSettings.StopCriteria = null;

			// instantiation of the tested class
			var interceptor = new DatashopWorkflowStepInterceptor(new string[0], interceptionSettings);

			// call the tested method
			bool stopAfterStep = interceptor.StopAfterStep(null, null);

			// assert
			Assert.False(stopAfterStep);
		}
	}
}