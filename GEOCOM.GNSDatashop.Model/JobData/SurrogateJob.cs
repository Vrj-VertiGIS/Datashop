using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
	public class SurrogateJob
	{
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual long JobId { get; set; }

        [DataMember]
        public virtual long UserId { get; set; }

        [DataMember]
        public virtual long SurrogateUserId { get; set; }

        [DataMember]
        public virtual DateTime? RequestDate { get; set; }

        [DataMember]
        public virtual string RequestType { get; set; }

        [DataMember]
        public virtual bool StopAfterProcess { get; set; }
    }
}
