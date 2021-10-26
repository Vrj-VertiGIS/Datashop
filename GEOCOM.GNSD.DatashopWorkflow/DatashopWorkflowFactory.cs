using System.Runtime.CompilerServices;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.DatashopWorkflow
{
    using System.Diagnostics.CodeAnalysis;

    public static class DatashopWorkflowFactory
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public static IWorkflow CreateWorkflowByJobId(long jobId, bool doDataBinding)
        {
            IWorkflow workflow = WorkflowFactory.CreateWorkflowByJobId(jobId, doDataBinding);
            return workflow;
        }
    }
}