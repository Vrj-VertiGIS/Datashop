using System;

namespace GEOCOM.GNSD.Web
{
    public class UserRequestMetaInfo
    {
        public String RedirectPage { get; set; }
        public UserRequestStatus UserRequestStatus { get; set; }

        public UserRequestMetaInfo()
        {
        }

        public UserRequestMetaInfo(UserRequestStatus userRequestStatus)
        {
            UserRequestStatus = userRequestStatus;
        }

        public UserRequestMetaInfo(UserRequestStatus userRequestStatus, String redirectPage)
        {
            RedirectPage = redirectPage;
            UserRequestStatus = userRequestStatus;
        }
    }
}