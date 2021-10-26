using System;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GNSDatashopTest.Workflow
{
    class Utilities
    {
        public static Job GetJob(Type workflowType)
        {
            Job job = new Job();
            job.IsActive = true;
            job.NeedsProcessing = false;
            job.State = 0;
            job.Step = 0;
            job.ProcessorClassId = workflowType.FullName;

            return job;
        }
    }
}
