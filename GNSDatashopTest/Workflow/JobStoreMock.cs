using System.Collections.Generic;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GNSDatashopTest.Workflow
{
    public class JobStoreMock : JobStore
    {
        private readonly List<Job> _jobUpdateList = new List<Job>();

        public List<Job> JobUpdateList
        {
            get { return _jobUpdateList; }
        }


        public override bool Update(Job job)
        {
            Job jobClone = new Job();
            jobClone.IsActive = job.IsActive;
            jobClone.NeedsProcessing = job.NeedsProcessing;
            jobClone.Step = job.Step;
            jobClone.State = job.State;
            JobUpdateList.Add(jobClone);
            return true;
        }
    }
}
