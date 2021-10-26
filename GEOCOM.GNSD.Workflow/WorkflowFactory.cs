using System.Diagnostics;
using System.Reflection;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow.DataObjects;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GEOCOM.GNSD.Workflow
{
    public static class WorkflowFactory
    {
        public static IWorkflow CreateWorkflowByJobId(long jobId, bool doDataBinding)
        {
            var jobStore = new JobStore();
            Job job = jobStore.GetById(jobId);
            string assemblyFullName = Assembly.GetCallingAssembly().GetName().FullName;
            return CreateWorkflowByJobAndJobStore(job, jobStore, assemblyFullName, doDataBinding);
        }

        public static IWorkflow CreateWorkflowByJobAndJobStore(Job job, JobStore jobStore, string assemblyFullName, bool doDataBinding)
        {
            job.ProcessId = Process.GetCurrentProcess().Id;

            var workflowStateItem = new WorkflowStateItem(job);
            workflowStateItem.JobStore = jobStore;
            workflowStateItem.AssemblyFullName = assemblyFullName;
            var workflowDataItemBase = new WorkflowDataItemBase { Job = job };
            WorkflowBase workflow = WorkflowBase.CreateWorkflowFromStateItem(workflowStateItem, workflowDataItemBase, doDataBinding);
            return workflow;
        }
    }
}
