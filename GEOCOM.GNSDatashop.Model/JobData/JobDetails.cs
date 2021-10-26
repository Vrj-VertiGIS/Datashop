using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
    public class JobDetails : Job
    {
        [DataMember]
        public virtual string FirstName { get; set; }

        [DataMember]
        public virtual string LastName { get; set; }

        [DataMember]
        public virtual string Email { get; set; }

        [DataMember]
        public virtual string Reason { get; set; }
    }
}
