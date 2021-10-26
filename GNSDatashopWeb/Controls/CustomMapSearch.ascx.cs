using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.Controls
{
    public partial class CustomMapSearch : System.Web.UI.UserControl
    {
        private readonly IMsg _log = new Msg(typeof(CustomMapSearch));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DatashopWebConfig.Instance.MapSearch == null) return;
            if (DatashopWebConfig.Instance.MapSearch.CustomSearches == null) return;

            rptrCustomSearch.DataSource = DatashopWebConfig.Instance.MapSearch.CustomSearches.Where(config => config.CustomEnabled).ToList();
            rptrCustomSearch.DataBind();
        }


        protected void rptrCustomSearch_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var customMapSearchHolder = (PlaceHolder)e.Item.FindControl("customMapSearchHolder");
            try
            {
                var customSearch = (CustomSearchConfig)e.Item.DataItem;
                var customMapSearch = Page.LoadControl(customSearch.CustomMapSearchVirtualPath);
                customMapSearchHolder.Controls.Add(customMapSearch);
            }
            catch (Exception exception)
            {
                _log.Error("Error during loading of a custom map search control.", exception);
                var label = new HtmlGenericControl("span");
                label.InnerText = "Error during loading of a custom map search control: " + exception.Message;
                label.Style.Add("color", "red");
                label.Style.Add("font-weight", "bold");
                customMapSearchHolder.Controls.Add(label);
            }

        }
    }
}