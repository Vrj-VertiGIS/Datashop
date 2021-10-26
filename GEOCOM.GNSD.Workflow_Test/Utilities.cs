using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEOCOM.GNSD.DBStore.Container.JobData;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow_Test.Workflows;

namespace GEOCOM.GNSD.Workflow_Test
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
