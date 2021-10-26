using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using GEOCOM.GNSD.Web.Controls;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
    /// <summary>
    /// Handler for obtaining surrogate users
    /// </summary>
    public class SurrogateUsersRestHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var name = HttpContext.Current.Request.QueryString["name"].Trim('*');
            var take = int.Parse(HttpContext.Current.Request.QueryString["count"] ?? "50");
            IEnumerable<User> users;
            users = string.IsNullOrEmpty(name) 
                ? DatashopService.Instance.JobService.GetAllUsersPaged(0, take) 
                : DatashopService.Instance.JobService.GetUsersBySurrogateFilterPaged(name, 0, take);

            var usersTransformed = users.Select(user => new
            {
                id = user.UserId,
                name = CommonActAsSurrogate.FormatUser(user)
            }).ToArray();
            var usr = new { numRows = usersTransformed.Length, items = usersTransformed, identifier = "id", label = "name" };

            var javaScriptSerializer = new JavaScriptSerializer();
            var json = javaScriptSerializer.Serialize(usr);
            context.Response.ContentType = "text/json";
            context.Response.Write(json);

        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}