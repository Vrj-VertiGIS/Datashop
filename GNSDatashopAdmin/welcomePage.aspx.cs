using System;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;
using System.Web.UI;

namespace GNSDatashopAdmin
{
    public partial class welcomePage : Page
    {
        protected void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
        {
            var users = DatashopService.Instance.JobService.GetUserByEmail((sender as Login).UserName);
            
            foreach (User user in users)
            {
                //If BizUser is found use this ID
                if (user.BizUser != null)
                {
                    (sender as Login).UserName = user.UserId.ToString();
                    return;
                }
            }
        }
    }
}