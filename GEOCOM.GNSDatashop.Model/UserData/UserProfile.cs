using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model.UserData
{
    [DataContract]
    public class UserProfile
    {

        public UserProfile()
        {
            RightList = new List<UserRight>();
        }

        public UserProfile(long userId)
            : this()
        {
            UserId = userId;
        }

        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public List<UserRight> RightList { get; set; }
    }
}
