using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    [DataContract]
    [DebuggerDisplay("JobId = {JobId}, NeedsProcessing = {NeedsProcessing}, Step = {Step}, State = {State}")]
	public class Job
	{
	    [XmlAttribute("JobId")]
        [DataMember]
		public virtual long JobId { get; set; }

        [DataMember]
        public virtual long UserId { get; set; }

        [DataMember]
        public virtual long ReasonId { get; set; }

        [DataMember]
        public virtual string Definition { get; set; }

        [DataMember]
        public virtual int Step { get; set; }

        [DataMember]
        public virtual int State { get; set; }

        [DataMember]
        public virtual bool NeedsProcessing { get; set; }

        [DataMember]
        public virtual bool IsActive { get; set; }
        
        [DataMember]
        public virtual bool GeoAttachmentsEnabled { get; set; }

        [DataMember]
        public virtual string JobOutput { get; set; }

        [DataMember]
        public virtual long? ProcessId { get; set; }

        [DataMember]
        public virtual string MachineName { get; set; }

        [DataMember]
        public virtual long? ProcessingUserId { get; set; }

        [DataMember]
        public virtual string ProcessorClassId { get; set; }

        [DataMember]
        public virtual bool IsArchived { get; set; }

        [DataMember]
        public virtual int DownloadCount { get; set; }

        [DataMember]
        public virtual int MapExtentCount { get; set; }

        [DataMember]
        public virtual string Custom1 { get; set; }

        [DataMember]
        public virtual string Custom2 { get; set; }

        [DataMember]
        public virtual string Custom3 { get; set; }
        [DataMember]
        public virtual string Custom4 { get; set; }
        [DataMember]
        public virtual string Custom5 { get; set; }
        [DataMember]
        public virtual string Custom6 { get; set; }
        [DataMember]
        public virtual string Custom7 { get; set; }
        [DataMember]
        public virtual string Custom8 { get; set; }
        [DataMember]
        public virtual string Custom9 { get; set; }
        [DataMember]
        public virtual string Custom10 { get; set; }

        [DataMember]
        public virtual DateTime? PeriodBeginDate { get; set; }

        [DataMember]
        public virtual DateTime? PeriodEndDate { get; set; }

        [DataMember]
        public virtual string Description { get; set; }

        [DataMember]
        public virtual string ParcelNumber { get; set; }

        [DataMember]
        public virtual string Municipality { get; set; }

        [DataMember]
        public virtual double CenterAreaX { get; set; }

        [DataMember]
        public virtual double CenterAreaY { get; set; }

        [DataMember]
        public virtual SurrogateJob SurrogateJob { get; set; }

        [DataMember]
        public virtual DateTime? LastStateChangeDate { get; set; }

        [DataMember]
        public virtual DateTime? CreateDate { get; set; }

	    [DataMember]
	    public virtual bool DxfExport { get; set; }
    }
}
