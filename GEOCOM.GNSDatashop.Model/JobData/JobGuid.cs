using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
    public class JobGuid
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual long JobId { get; set; }

        [DataMember]
        public virtual string Guid { get; set; }
    }
}
