using System;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Exceptions;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model;
using GEOCOM.GNSDatashop.Model.JobData;
using GNSDatashopTest.Workflow.Workflows;
using NUnit.Framework;

namespace GNSDatashopTest.Workflow
{
	//TODO write tests for steps status

	[TestFixture]
	public class WorkflowTest
	{
		[SetUp]
		public void Setup()
		{
			WorkflowTracker.Instance.CalledMethods = new List<string>();
			WorkflowTracker.Instance.CalledMethods.Clear();
		}

		[Test]
		public void TestNormalWorkflow()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);
		}

		[Test]
		public void TestStopAtBarrierUntilActivate()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(BarrierWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);

			workflow.Run();
			workflow.Run();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.False(workflow.IsActive);

			workflow.Activate();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.True(workflow.IsActive);

			workflow.Run();
			Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
		}


		[Test]
		public void TestSkipSkippableBarrier()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(SkippableBarrierWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			// First barrier is skipped
			// Second (skippable) barrier stops
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.False(workflow.IsActive);

			workflow.Activate();
			workflow.Run();
			// Third (non-skippable) barrier stops
			Assert.AreEqual(6, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[4]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[5]);
			Assert.False(workflow.IsActive);

			workflow.Activate();
			workflow.Run();
			Assert.AreEqual(7, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[4]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[5]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[6]);
			Assert.False(workflow.IsActive);
		}

		[Test]
		public void TestActivatedAtStart()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			Assert.True(workflow.IsActive);
			workflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);

			Assert.IsFalse(workflow.IsActive);
		}

		[Test]
		public void TestDeactivatedAtEnd()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.IsFalse(workflow.IsActive);
		}

		[Test]
		public void TestMultipleActivate()
		{
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(BarrierWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);

			workflow.Activate();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);

			workflow.Activate();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);

			workflow.Run();
			Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
		}

		[Test]
		public void TestEmptyWorkflow()
		{
			Assert.Throws<WorkflowException>(() => new EmptyWorkflow());
		}

		[Test]
		public void TestRestart()
		{
			IWorkflow normalWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			normalWorkflow.RestartFromStep(3);
			Assert.IsTrue(normalWorkflow.IsActive);
			normalWorkflow.Run();
			Assert.AreEqual(2, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[1]);
		}

		[Test]
		public void TestRestart2()
		{
			IWorkflow normalWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			normalWorkflow.RestartFromStep(1);
			Assert.IsTrue(normalWorkflow.IsActive);
			normalWorkflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);
		}

		[Test]
		public void TestRestartBarrier()
		{
			IWorkflow barrierWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(BarrierWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			barrierWorkflow.RestartFromStep(3);
			barrierWorkflow.Run();
			Assert.AreEqual(0, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.IsFalse(barrierWorkflow.IsActive);

			barrierWorkflow.Activate();
			barrierWorkflow.Run();
			Assert.AreEqual(1, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
		}

		[Test]
		public void TestRepeatFailedStep()
		{
			IWorkflow exceptionWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(ExceptionWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			try
			{
				exceptionWorkflow.Run();
			}
			catch (Exception)
			{
				Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
				Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
				Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
				Assert.AreEqual("OnError", WorkflowTracker.Instance.CalledMethods[2]);
			}

			try
			{
				exceptionWorkflow.Activate();
				exceptionWorkflow.Run();
			}
			catch (Exception)
			{
				Assert.AreEqual(5, WorkflowTracker.Instance.CalledMethods.Count);
				Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
				Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
				Assert.AreEqual("OnError", WorkflowTracker.Instance.CalledMethods[2]);
				Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[3]);  // Step B will be run again!
				Assert.AreEqual("OnError", WorkflowTracker.Instance.CalledMethods[4]);
				Assert.False(exceptionWorkflow.IsActive);
			}
		}


		[Test]
		public void TestOnErrorAfterException()
		{
			IWorkflow exceptionWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(ExceptionWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			try
			{
				exceptionWorkflow.Run();
			}
			catch (Exception)
			{
				Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
				Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
				Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
				Assert.AreEqual("OnError", WorkflowTracker.Instance.CalledMethods[2]);
			}
		}

		[Test]
		public void TestRestartInvalidIndex()
		{
			IWorkflow normalWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			Assert.Throws<WorkflowException>(() => normalWorkflow.RestartFromStep(-1));
			Assert.Throws<WorkflowException>(() => normalWorkflow.RestartFromStep(7));
		}

		[Test]
		public void TestRestartableWorkflow()
		{
			IWorkflow restartableWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(RestartableWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			var restartableStepsIndices = restartableWorkflow.GetAllRestartableStepIds();
			Assert.AreEqual(1, restartableStepsIndices.Length);
			Assert.AreEqual(0, restartableStepsIndices[0]);
			restartableWorkflow.Run();

			restartableStepsIndices = restartableWorkflow.GetAllRestartableStepIds();
			Assert.AreEqual(4, restartableStepsIndices.Length);
			Assert.AreEqual(0, restartableStepsIndices[0]);
			Assert.AreEqual(1, restartableStepsIndices[1]);
			Assert.AreEqual(2, restartableStepsIndices[2]);
			Assert.AreEqual(3, restartableStepsIndices[3]);
		}

		[Test]
		public void TestNamedWorkflow()
		{
			IWorkflow nameWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NameWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			Assert.AreEqual("Start", nameWorkflow.GetWorkflowStepNameById(0));
			Assert.AreEqual("Step A", nameWorkflow.GetWorkflowStepNameById(1));
			Assert.AreEqual("B", nameWorkflow.GetWorkflowStepNameById(2));
			Assert.AreEqual("C", nameWorkflow.GetWorkflowStepNameById(3));
			Assert.AreEqual("Barrier", nameWorkflow.GetWorkflowStepNameById(4));
			Assert.AreEqual("D", nameWorkflow.GetWorkflowStepNameById(5));
			Assert.AreEqual(NameWorkflow.Name, nameWorkflow.GetWorkflowStepNameById(6));

			nameWorkflow.Run();
			var namesAndIndices = nameWorkflow.GetAllRestartableStepIdNames();
			Assert.AreEqual(5, namesAndIndices.Length);
			Assert.AreEqual("Start", namesAndIndices[0].Name);
			Assert.AreEqual(0, namesAndIndices[0].Id);
			Assert.AreEqual("Step A", namesAndIndices[1].Name);
			Assert.AreEqual(1, namesAndIndices[1].Id);
			Assert.AreEqual("B", namesAndIndices[2].Name);
			Assert.AreEqual(2, namesAndIndices[2].Id);
			Assert.AreEqual("C", namesAndIndices[3].Name);
			Assert.AreEqual(3, namesAndIndices[3].Id);
			Assert.AreEqual("Barrier", namesAndIndices[4].Name);
			Assert.AreEqual(4, namesAndIndices[4].Id);
		}

		[Test]
		public void TestGetAllStepIdNames()
		{
			IWorkflow nameWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NameWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			WorkflowStepIdName[] allStepsIdsNames = nameWorkflow.GetAllStepIdNames();
			Assert.AreEqual(7, allStepsIdsNames.Length);
			Assert.AreEqual(0, allStepsIdsNames[0].Id);
			Assert.AreEqual("Start", allStepsIdsNames[0].Name);
			Assert.AreEqual(1, allStepsIdsNames[1].Id);
			Assert.AreEqual("Step A", allStepsIdsNames[1].Name);
			Assert.AreEqual(2, allStepsIdsNames[2].Id);
			Assert.AreEqual("B", allStepsIdsNames[2].Name);
			Assert.AreEqual(3, allStepsIdsNames[3].Id);
			Assert.AreEqual("C", allStepsIdsNames[3].Name);
			Assert.AreEqual(4, allStepsIdsNames[4].Id);
			Assert.AreEqual("Barrier", allStepsIdsNames[4].Name);
			Assert.AreEqual(5, allStepsIdsNames[5].Id);
			Assert.AreEqual("D", allStepsIdsNames[5].Name);
			Assert.AreEqual(6, allStepsIdsNames[6].Id);
			Assert.AreEqual("name from the workflow definition", allStepsIdsNames[6].Name);
		}

		[Test]
		public void TestGetNextStepIdName()
		{
			IWorkflow nameWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NameWorkflow)), new JobStoreMock(), Assembly.GetExecutingAssembly().FullName, true);
			WorkflowStepIdName nextStepIdName = nameWorkflow.GetNextStepIdName();
			Assert.AreEqual(1, nextStepIdName.Id);
			Assert.AreEqual("Step A", nextStepIdName.Name);
			nameWorkflow.Run();
			nextStepIdName = nameWorkflow.GetNextStepIdName();
			Assert.AreEqual(5, nextStepIdName.Id);
			Assert.AreEqual("D", nextStepIdName.Name);
		}

		[Test]
		public void TestInvalidWorkflowStepId()
		{
			Assert.Throws<WorkflowException>(() => new InvalidStepIdWorkflow1());
			Assert.Throws<WorkflowException>(() => new InvalidStepIdWorkflow2());
		}

		[Test]
		public void TestSetFailed()
		{
			JobStoreMock jobStoreMock = new JobStoreMock();
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
			Assert.True(workflow.IsActive);
			workflow.SetFailed();
			Assert.False(workflow.IsActive);
			List<Job> jobUpdateList = jobStoreMock.JobUpdateList;
			Assert.AreEqual(1, jobUpdateList.Count);
			Assert.AreEqual(false, jobUpdateList[0].IsActive);
			Assert.AreEqual((int)WorkflowStepState.Failed, jobUpdateList[0].State);
		}

		[Test]
		public void TestWorkflowInterceptor()
		{
			JobStoreMock jobStoreMock = new JobStoreMock();
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(InterceptedWorkflow)), jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
			Assert.True(workflow.IsActive);
			workflow.Run();
			Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.False(workflow.IsActive);

			workflow.Activate();
			workflow.Run();
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
		}


		[Test]
		public void TestWorkflowInterceptorWorksAfterRestartingWorkflow()
		{
			JobStoreMock jobStoreMock = new JobStoreMock();
			Job job = Utilities.GetJob(typeof(InterceptedWorkflow));
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(job, jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);

			Assert.True(workflow.IsActive);
			workflow.Run();
			Assert.AreEqual(3, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.False(workflow.IsActive);
			Assert.AreEqual((int)WorkflowStepState.StoppedAfterStep, job.State);

			workflow.RestartFromStep(1);
			// create new instance of the workflow but with the the same job object - simulation of a complete switch off of the workflow
			// and keeping of the state only in the job object (i.e. in a DB)
			workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(job, jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
			workflow.Run();
			Assert.AreEqual(6, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[4]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[5]);
			Assert.False(workflow.IsActive);
			Assert.AreEqual((int)WorkflowStepState.StoppedAfterStep, job.State);

			workflow.Activate();
			workflow.Run();
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[6]);
			Assert.AreEqual(7, WorkflowTracker.Instance.CalledMethods.Count);
		}

		[Test]
		public void TestInterceptedWorkflowWithOnStopHandler()
		{
			JobStoreMock jobStoreMock = new JobStoreMock();
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(InterceptedWorkflowWithOnStopHandler)), jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
			Assert.True(workflow.IsActive);
			workflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("OnStoppedAfterStep", WorkflowTracker.Instance.CalledMethods[3]);
			Assert.False(workflow.IsActive);

			workflow.Activate();
			workflow.Run();
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[4]);
			Assert.AreEqual(5, WorkflowTracker.Instance.CalledMethods.Count);
		}

		[Test]
		public void TestNonInterceptedWorkflowWithOnStopHandler()
		{
			JobStoreMock jobStoreMock = new JobStoreMock();
			IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NonInterceptedWorkflowWithOnStopHandler)), jobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
			Assert.True(workflow.IsActive);
			workflow.Run();
			Assert.AreEqual(4, WorkflowTracker.Instance.CalledMethods.Count);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[0]);
			Assert.AreEqual("B", WorkflowTracker.Instance.CalledMethods[1]);
			Assert.AreEqual("A", WorkflowTracker.Instance.CalledMethods[2]);
			Assert.AreEqual("C", WorkflowTracker.Instance.CalledMethods[3]);
			
			// Assert that OnStoppedAfterStep was not called when there is no interception.
			Assert.False(WorkflowTracker.Instance.CalledMethods.Contains("OnStoppedAfterStep"));
		}
	}



}
