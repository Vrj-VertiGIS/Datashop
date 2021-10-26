using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.UserData
{
    [DataContract]
    public enum UserRole
    {
        [EnumMember]
        Undefined = 0,
        
        [EnumMember]
        Admin = 1,

        [EnumMember]
        Business = 2,

        [EnumMember]
        Temporary = 3,

        [EnumMember]
        Internal = 4,

        [EnumMember]
        Public = 5
    }
}
