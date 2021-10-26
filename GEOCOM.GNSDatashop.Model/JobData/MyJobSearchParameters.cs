using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
    public class MyJobSearchParameters
    {
        [DataMember]
        public long? JobId { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public DateTime? CreatedDateStart { get; set; }

        [DataMember]
        public DateTime? CreatedDateEnd { get; set; }

        [DataMember]
        public bool? Downloaded { get; set; }

        public bool IsEmtpy 
        { 
            get { return this.CheckEmpty(); }
        }
        [DataMember]
        public string Custom1 { get; set; }
		
        [DataMember]
        public string Custom2 { get; set; }
		
        [DataMember]
        public string Custom3 { get; set; }
		
        [DataMember]
        public string Custom4 { get; set; }
		
        [DataMember]
        public string Custom5 { get; set; }
		
        [DataMember]
        public string Custom6 { get; set; }
		
        [DataMember]
        public string Custom7 { get; set; }
		
        [DataMember]
        public string Custom8 { get; set; }
		
        [DataMember]
        public string Custom9 { get; set; }
		
        [DataMember]
        public string Custom10 { get; set; }
		
        [DataMember]
        public string JobParcelNumber { get; set; }
		
        [DataMember]
        public long? ReasonId { get; set; }


        private bool CheckEmpty()
        {
            return this.JobId == null && string.IsNullOrWhiteSpace(this.UserId) &&
                string.IsNullOrWhiteSpace(this.FirstName) && string.IsNullOrWhiteSpace(this.LastName) &&
                string.IsNullOrWhiteSpace(this.Company) && this.CreatedDateStart == null && this.CreatedDateEnd == null &&
                this.Downloaded == null
                && string.IsNullOrWhiteSpace(Custom1)
                && string.IsNullOrWhiteSpace(Custom2)
                && string.IsNullOrWhiteSpace(Custom3)
                && string.IsNullOrWhiteSpace(Custom4)
                && string.IsNullOrWhiteSpace(Custom5)
                && string.IsNullOrWhiteSpace(Custom6)
                && string.IsNullOrWhiteSpace(Custom7)
                && string.IsNullOrWhiteSpace(Custom8)
                && string.IsNullOrWhiteSpace(Custom9)
                && string.IsNullOrWhiteSpace(Custom10)
                && string.IsNullOrWhiteSpace(JobParcelNumber)
                && ReasonId == null;
        }
    }
}