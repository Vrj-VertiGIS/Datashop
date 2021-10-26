using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.JobData
{
    /// <summary>
    /// Model class that holds the data for the MyJobs
    /// </summary>
    [DataContract]
    public class MyJob
    {
        #region Header Information

        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        /// <value>
        /// The job id.
        /// </value>
        [DataMember]
        public virtual long JobId { get; set; }

        /// <summary>
        /// Gets or sets the job GUID.
        /// </summary>
        /// <value>
        /// The job GUID.
        /// </value>
        [DataMember]
        public virtual Guid? JobGuid { get; set; }

        /// <summary>
        /// Gets or sets the is archived.
        /// </summary>
        /// <value>
        /// The is archived.
        /// </value>
        [DataMember]
        public virtual bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the download count.
        /// </summary>
        /// <value>
        /// The download count.
        /// </value>
        [DataMember]
        public virtual int DownloadCount { get; set; }
        
		/// <summary>
        /// Gets or sets the status of the job - check out <see cref="GEOCOM.GNSDatashop.Model.WorkflowStepState"/>.
        /// </summary>
        [DataMember]
        public virtual int Status { get; set; }

		/// <summary>
        /// Gets or sets the job step - checkout <see cref="GEOCOM.GNSDatashop.Model.DatashopWorkflow.PlotJobSteps"/>.
        /// </summary>
        [DataMember]
        public virtual int Step { get; set; }

        #endregion

        #region Represented User

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [DataMember]
        public virtual long RepresentedUserId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [DataMember]
        public virtual string RepresentedUserFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [DataMember]
        public virtual string RepresentedUserLastName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        [DataMember]
        public virtual string RepresentedUserCompany { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember]
        public virtual string RepresentedUserEmail { get; set; }

        #endregion

        #region Representative User

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [DataMember]
        public virtual long? RepresentativeUserId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [DataMember]
        public virtual string RepresentativeUserFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [DataMember]
        public virtual string RepresentativeUserLastName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        [DataMember]
        public virtual string RepresentativeUserCompany { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember]
        public virtual string RepresentativeUserEmail { get; set; }

        #endregion

        #region Additional fields
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
        public virtual string ParcelNumber { get; set; }

        [DataMember]
        public virtual long? ReasonId { get; set; }

        [DataMember]
        public virtual string Reason { get; set; }

        #endregion
    }
}