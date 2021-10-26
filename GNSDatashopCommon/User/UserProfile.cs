using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.Common.User
{
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

        public long UserId { get; set; }

        public List<UserRight> RightList { get; set; }
    }
}
