using System;
using System.Reflection;
using System.Runtime.Serialization;
using GEOCOM.Common.Logging;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
	public class JobLog
	{
		// log4net
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        #region construction/destruction

        public JobLog()
        {
        }

        public JobLog(Job job, string message)
        {
            Init(job, message);
        }

	    public JobLog(Job job)
        {
            Init(job, string.Empty);
        }
        
        #endregion

        #region public auto accessors

        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual long JobId { get; set; }

        [DataMember]
        public virtual DateTime Timestamp { get; set; }

        [DataMember]
        public virtual int? Step { get; set; }

        [DataMember]
        public virtual int? State { get; set; }

        [DataMember]
        public virtual string Message { get; set; }

        [DataMember]
        public virtual bool NeedsProcessing { get; set; }

        [DataMember]
        public virtual bool IsActive { get; set; }
        #endregion

        private void Init(Job job, string message)
        {            
            this.JobId = job.JobId;
            Timestamp = DateTime.Now;
            Step = job.Step;
            State = job.State;
            IsActive = job.IsActive;
            NeedsProcessing = job.NeedsProcessing;
            Message = message;            
        }
	}
}
