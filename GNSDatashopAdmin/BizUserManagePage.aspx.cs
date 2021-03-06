using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;


namespace GNSDatashopAdmin
{
    public partial class BizUserManagePage : System.Web.UI.Page
    {
        private User[] _users;
        private String[] _primaryKeys = { "UserId" };
        private static int pageSizeDefault = 20, pageIndexDefault = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            javaScript.Text = "";
            UserGrid.EditIndex = -1;
            UserGrid.ShowFooter = true;
            //Hide Searchbar
            if (!Page.IsPostBack)
            {
                searchFields.Style["display"] = "none";

                //fill DropDownList for Status
                ddlStatus.Items.Clear();
                ddlStatus.DataSource = Enum.GetValues(typeof(BizUserStatus));
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, "");
            }

            //writing viewState for sorting
            if (ViewState["SortExpression"] == null)
                ViewState["SortExpression"] = "UserId";
            if (ViewState["SortAscending"] == null)
                ViewState["SortAscending"] = true;

            //writing ViewState for pagging
            if (ViewState["pageIndex"] == null)
                ViewState["pageIndex"] = pageIndexDefault;
            if (ViewState["pageSize"] == null)
                ViewState["pageSize"] = pageSizeDefault;

            //write ViewState for Searching
            if (ViewState["Searching"] == null)
                ViewState["Searching"] = false;

            //get parameters for pagging
            if (Request.Params.Get("__EVENTTARGET") == "Pagging")
            {
                try
                {
                    ViewState["pageIndex"] = int.Parse(Request.Params.Get("__EVENTARGUMENT"));
                }
                catch (FormatException)
                {
                    ViewState["pageIndex"] = pageIndexDefault;
                }
                getAndBindData(false);
            }
            else if (Request.Params.Get("__EVENTTARGET") == "PageSize")
            {
                try
                {
                    ViewState["pageSize"] = int.Parse(Request.Params.Get("__EVENTARGUMENT"));
                    ViewState["pageIndex"] = pageIndexDefault;
                }
                catch (FormatException)
                {
                    ViewState["pageIndex"] = pageIndexDefault;
                    ViewState["pageSize"] = pageSizeDefault;
                }

                getAndBindData(false);
            }
            else
            {
                if (!Page.IsPostBack)
                    getAndBindData(true);
            }
        }

        protected void UserGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Change Sortdirection if same label is clicked twice
            if (ViewState["SortExpression"].ToString().Equals(e.SortExpression))
            {
                ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
            }
            ViewState["SortExpression"] = e.SortExpression;

            getAndBindData(false);
        }

        protected void UserGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            UserGrid.EditIndex = e.NewEditIndex;
            getAndBindData(false);

        }

        protected void UserGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            UserGrid.EditIndex = -1;
            getAndBindData(false);
        }

        protected void UserGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            User user;
            long UserID = long.Parse(((HyperLink)UserGrid.Rows[e.RowIndex].Cells[1].Controls[0]).Text);
            try
            {
                user = DatashopService.Instance.JobService.GetUser(UserID) ?? new User();
            }
            catch (Exception)
            {
                user = new User();
            }

            user.Salutation = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            user.FirstName = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[3].Controls[0]).Text;
            user.LastName = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[4].Controls[0]).Text;
            user.Email = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[5].FindControl("EmailEdit")).Text;
            user.Street = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[6].Controls[0]).Text;
            user.StreetNr = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[7].Controls[0]).Text;
            user.CityCode = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[8].Controls[0]).Text;
            user.City = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[9].Controls[0]).Text;
            user.Company = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[10].Controls[0]).Text;            
            user.Tel = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[11].Controls[0]).Text;
            user.Fax = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[12].Controls[0]).Text;
            
            if(user.BizUser != null)
            {
                user.BizUser.Roles = ((TextBox)UserGrid.Rows[e.RowIndex].Cells[14].FindControl("RolesEdit")).Text;
            }


            if (user.UserId == 0)
            {
                DatashopService.Instance.JobService.CreateUser(user);
            }
            else
            {
                DatashopService.Instance.JobService.UpdateUser(user);
                if(user.BizUser != null)
                    DatashopService.Instance.JobService.UpdateBizUser(user.BizUser);
            }



            getAndBindData(false);



        }

        protected void UserGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //every time an event is generated, it will stop any editing process that might be in action.
            UserGrid.EditIndex = -1;
            UserGrid.Columns[0].Visible = true;

            if (e.CommandName.Equals("Deactivate"))
            {
                DeactivateBizUser(e);
            }

        }

        private void DeactivateBizUser(GridViewCommandEventArgs e)
        {
            long userID = long.Parse(e.CommandArgument.ToString());
            var user = DatashopService.Instance.JobService.GetUser(userID);
            if(user == null || user.BizUser == null)
            {
                javaScript.Text = "<script language=javascript> alert('The user was not found and could not be locked.')</script>";
                return;
            }
            user.BizUser.UserStatus = BizUserStatus.DISABLED;
            DatashopService.Instance.JobService.UpdateBizUser(user.BizUser);
            getAndBindData(false);
            
        }

        private void getAndBindData(bool checkIfIsPageBack)
        {
            if ((bool)ViewState["Searching"] == true)
            {
                btn_Search(null, null);
                return;
            }
            _users = DatashopService.Instance.JobService.GetSomeUsersSorted(ViewState["SortExpression"].ToString(),
                                                   (bool)ViewState["SortAscending"], int.Parse(ViewState["pageIndex"].ToString()) * int.Parse(ViewState["pageSize"].ToString()), int.Parse(ViewState["pageSize"].ToString()),true,false);
            //Test if it is necessary to bind JobGrid
            UserGrid.DataSource = _users;
            if (checkIfIsPageBack)
            {
                if (!Page.IsPostBack)
                {
                    UserGrid.DataBind();
                }
            }
            else
            {
                UserGrid.DataBind();
            }
        }

        protected void UserGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //event fires when row is created. If it is Footerrow, pagging stuff will be added
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                generatePageLinks(e.Row.Cells);

            }
        }

        private void generatePageLinks(TableCellCollection cells)
        {
            //get Paging values
            int userAmount;
            if ((bool)ViewState["Searching"])
            {
                String firstname = txbFirstname.Text;
                String lastname = txbLastname.Text;
                String street = txbStreet.Text;
                String streetnr = txbStreetnr.Text;
                String citycode = txbCitycode.Text;
                String city = txbCity.Text;
                String company = txbCompany.Text;
                String email = txbEmail.Text;
                String adress = txbAdress.Text;
                String tel = txbTel.Text;
                String fax = txbFax.Text;
                String status = ddlStatus.SelectedItem.Text.Equals("") ? "" : ((int)Enum.Parse(typeof(BizUserStatus), ddlStatus.SelectedItem.Text)).ToString(); //get numeric Value for valid Status
                String roles = txbRoles.Text;
                long? userId = null;
                if (!txbUserId.Text.Equals(""))
                {
                    userId = long.Parse(txbUserId.Text);
                }
                userAmount = DatashopService.Instance.AdminService.GetUserSearchCount(adress, firstname, lastname, street, streetnr, citycode, city,
                                                             company, email, tel, fax, status, roles, userId, true, false);
            }
            else
            {
                userAmount = DatashopService.Instance.AdminService.GetUserCount(true, false);
            }
            //create String for pagging

            //create String for pagging
            String pagging = "";
            double pages = (double)(userAmount) / double.Parse(ViewState["pageSize"].ToString());
            bool setBeginTag = true, setEndTag = true;
            const int amountOfLinksVisible = 15;
            int currentPage = int.Parse(ViewState["pageIndex"].ToString());

            for (int i = currentPage - amountOfLinksVisible / 2; i < (currentPage + amountOfLinksVisible / 2)+pages-((int)pages); i++)
            {
                if (i < 0) setBeginTag = false;
                else if (i >= pages) {setEndTag = false;
                    break;}
                else if (i == currentPage) pagging += "<i> " + (i + 1) + " </i>";
                else pagging += "<a href=javascript:__doPostBack('Pagging','" + i + "')> " + (i + 1) + " </a>";
            }
            pagging = (setBeginTag
                          ? "<a href=javascript:__doPostBack('Pagging','" + 0 + "')> << </a>.."
                          : "") + pagging + (setEndTag
                                ? "..<a href=javascript:__doPostBack('Pagging','" + (int)(Math.Ceiling(pages)-1) + "')> >> </a>"
                                : "");           


            int cellAmount = cells.Count; //get amount of cells for colspan
            cells.Clear();
            TableCell pCell = new TableCell();
            pCell.Text = "<td align=center colspan=" + cellAmount + ">" + pagging + "</td>";
            cells.Add(pCell);

        }

        protected void btn_Search(object sender, EventArgs e)
        {
            //allways show search fields when searching
            searchFields.Style["display"] = "";
            if ((bool)ViewState["Searching"] == false)
            {
                ViewState["pageIndex"] = pageIndexDefault;
                ViewState["Searching"] = true;
            }

            //get Data from Formular
            String firstname = txbFirstname.Text;
            String lastname = txbLastname.Text;
            String street = txbStreet.Text;
            String streetnr = txbStreetnr.Text;
            String citycode = txbCitycode.Text;
            String city = txbCity.Text;
            String company = txbCompany.Text;
            String email = txbEmail.Text;
            String adress = txbAdress.Text;
            String tel = txbTel.Text;
            String fax = txbFax.Text;
            String status = ddlStatus.SelectedItem.Text.Equals("") ? "" : ((int)Enum.Parse(typeof(BizUserStatus), ddlStatus.SelectedItem.Text)).ToString(); //get numeric Value for valid Status
            String roles = txbRoles.Text;
            try
            {
                long? userId = null;
                if (!txbUserId.Text.Equals(""))
                {
                    userId = long.Parse(txbUserId.Text);
                }
                _users = DatashopService.Instance.JobService.SearchUsers(adress, firstname, lastname, street, streetnr, citycode, city, company, email, tel, fax, status, roles, userId, true, false, ViewState["SortExpression"].ToString(), (bool)ViewState["SortAscending"], int.Parse(ViewState["pageIndex"].ToString()) * int.Parse(ViewState["pageSize"].ToString()), int.Parse(ViewState["pageSize"].ToString()));
                UserGrid.DataSource = _users;
                UserGrid.DataBind();
            }
            catch (FormatException)
            {
                javaScript.Text = "<script language=javascript> alert('The UserID in the search box has the wrong format.')</script>";
            }
        }

        protected void pageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            var size = pageSizeDefault;

            UserGrid.PageSize = size;

            if (int.TryParse(pageSize.SelectedValue, out size))
            {
                UserGrid.PageSize = size;
                ViewState["pageSize"] = size;
            }
            else
            {
                ViewState["pageSize"] = pageSizeDefault;
            }

            ViewState["pageIndex"] = pageIndexDefault;

            getAndBindData(false);

        }

        // ReSharper disable InconsistentNaming
        protected void btn_SearchStopp(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            txbAdress.Text = "";
            txbCity.Text = "";
            txbCitycode.Text = "";
            txbCompany.Text = "";
            txbEmail.Text = "";
            txbFax.Text = "";
            txbFirstname.Text = "";
            txbLastname.Text = "";
            txbRoles.Text = "";
            txbStreet.Text = "";
            txbStreetnr.Text = "";
            txbTel.Text = "";
            txbUserId.Text = "";
            ddlStatus.SelectedIndex = 0;
            ViewState["Searching"] = false;
            ViewState["pageIndex"] = pageIndexDefault;
            //// demand COF 2011-08-09 keep the panel open
            //// searchFields.Style["display"] = "none";
            searchFields.Style.Remove("display");
            getAndBindData(false);

        }
    }
}
