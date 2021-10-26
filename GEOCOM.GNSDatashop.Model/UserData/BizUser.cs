using System;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.UserData
{
    [DataContract]
    public class BizUser
    {
        [DataMember]
        public virtual string Password { get; set; }

        [DataMember]
        public virtual string PasswordSalt { get; set; }

        [DataMember]
        public virtual string Roles { get; set; }

        [DataMember]
        public virtual BizUserStatus UserStatus { get; set; }

        [DataMember]
        public virtual long BizUserId { get; set; }

        [DataMember]
        public virtual int FailedLoginCount { get; set; }

        [DataMember]
        public virtual DateTime? BlockedUntil { get; set; }
        
        [DataMember]
        public virtual Guid? PasswordResetId { get; set; }  

        [DataMember]
        public virtual DateTime? PasswordResetIdValidity { get; set; }

    }
}