using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.UserData
{
    [DataContract]
	public class User
	{
        [DataMember]
        public virtual long UserId { get; set; }

        [DataMember]
        public virtual string Salutation { get; set; }

        [DataMember]
        public virtual string FirstName { get; set; }

        [DataMember]
        public virtual string LastName { get; set; }

        [DataMember]
        public virtual string Email { get; set; }

        [DataMember]
        public virtual string Street { get; set; }

        [DataMember]
        public virtual string StreetNr { get; set; }

        [DataMember]
        public virtual string CityCode { get; set; }

        [DataMember]
        public virtual string City { get; set; }

        [DataMember]
        public virtual string Company { get; set; }

        [DataMember]
        public virtual string Tel { get; set; }

        [DataMember]
        public virtual string Fax { get; set; }

        [DataMember]
        public virtual long? BizUserId { get; set; }

        [DataMember]
        public virtual BizUser BizUser { get; set; }
    }
}
