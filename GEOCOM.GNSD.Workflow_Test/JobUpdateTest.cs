using System;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.GNSD.DBStore.Container.JobData;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow_Test.Workflows;
using NUnit.Framework;

namespace GEOCOM.GNSD.Workflow_Test
{
    [TestFixture]
    public class JobUpdateTest
    {
        public JobStoreMock JobStoreMock { get; set; }

        [SetUp]
        public void Setup()
        {
            WorkflowTracker.Instance.MethodsCalled = new List<string>();

            JobStoreMock = new JobStoreMock();
        }

        [Test]
        public void TestJobUpdateNormal()
        {
            IWorkflow normalWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(NormalWorkflow)), JobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
            normalWorkflow.Run();
            List<Job> jobUpdateList = JobStoreMock.JobUpdateList;
            Assert.AreEqual(10, jobUpdateList.Count);
            CheckJobUpdate(jobUpdateList[0], true,  false, 0, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[1], true,  false, 0, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[2], true,  false, 1, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[3], true, false, 1, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[4], true,  false, 2, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[5], true, false, 2, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[6], true,  false, 3, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[7], true, false, 3, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[8], true, false, 4, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[9], false, false, 4, (int)WorkflowStepState.Finished);
        }

        [Test]
        public void TestJobUpdateBarrier()
        {
            IWorkflow barrierWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(BarrierWorkflow)), JobStoreMock, Assembly.GetExecutingAssembly().FullName, true);
            barrierWorkflow.Run();
            List<Job> jobUpdateList = JobStoreMock.JobUpdateList;
            Assert.AreEqual(8, jobUpdateList.Count);
            CheckJobUpdate(jobUpdateList[0], true, false, 0, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[1], true, false, 0, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[2], true, false, 1, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[3], true, false, 1, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[4], true, false, 2, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[5], true, false, 2, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[6], true, false, 3, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[7], false, false, 3, (int)WorkflowStepState.Finished);

            barrierWorkflow.Activate();
            Assert.AreEqual(9, jobUpdateList.Count);
            CheckJobUpdate(jobUpdateList[8], true, true, 4, (int)WorkflowStepState.Idle);

            barrierWorkflow.Run();
            Assert.AreEqual(11, jobUpdateList.Count);
            //      NeedsProcessing will be set to false from JobEngineController
            CheckJobUpdate(jobUpdateList[9], true, true, 4, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[10], false, true, 4, (int)WorkflowStepState.Finished);
        }

        [Test]
        public void TestJobUpdateException()
        {
            IWorkflow exceptionWorkflow = WorkflowFactory.CreateWorkflowByJobAndJobStore(Utilities.GetJob(typeof(ExceptionWorkflow)), JobStoreMock, Assembly.GetExecutingAssembly().FullName, true);

            Assert.Throws<Exception>(exceptionWorkflow.Run);

            List<Job> jobUpdateList = JobStoreMock.JobUpdateList;
            Assert.AreEqual(6, jobUpdateList.Count);
            CheckJobUpdate(jobUpdateList[0], true, false, 0, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[1], true, false, 0, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[2], true, false, 1, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[3], true, false, 1, (int)WorkflowStepState.Finished);
            CheckJobUpdate(jobUpdateList[4], true, false, 2, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[5], false, false, 2, (int)WorkflowStepState.Failed);

            exceptionWorkflow.Run();
            Assert.AreEqual(6, jobUpdateList.Count);

            exceptionWorkflow.Activate();
            Assert.AreEqual(7, jobUpdateList.Count);
            CheckJobUpdate(jobUpdateList[6], true, true, 2, (int)WorkflowStepState.Idle);

            Assert.Throws<Exception>(exceptionWorkflow.Run);
            Assert.AreEqual(9, jobUpdateList.Count);
            //      NeedsProcessing will be set to false from JobEngineController
            CheckJobUpdate(jobUpdateList[7], true, true, 2, (int)WorkflowStepState.Running);
            CheckJobUpdate(jobUpdateList[8], false, true, 2, (int)WorkflowStepState.Failed);
        }

        private void CheckJobUpdate(Job job, bool isActiveExpected, bool needsProcessingExpected, int stepExpected, int stateExpected)
        {
            Assert.AreEqual(isActiveExpected, job.IsActive);
            Assert.AreEqual(needsProcessingExpected, job.NeedsProcessing);
            Assert.AreEqual(stepExpected, job.Step);
            Assert.AreEqual(stateExpected, job.State);
        }
    }
}
