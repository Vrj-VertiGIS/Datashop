using System;
using System.Linq;
using System.Web;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web.Controls
{
   

    public delegate void TempUserCreated(long userId);    

    /// <summary>
    /// Dialog used in the CreateBusinessUser page and in the ActAsSurrogate user control.
    /// </summary>
    public partial class CommonCreateTempUser : CommonCreateUser
    {
        #region Public events

        public event TempUserCreated UserCreated;

        #endregion

        #region Public methods

        public void Reset()
        {
            salutation.DropDown.SelectedIndex = 0;
            firstName.Text = string.Empty;
            lastName.Text = string.Empty;
            street.Text = string.Empty;
            streetNumber.Text = string.Empty;
            zip.Text = string.Empty;
            city.Text = string.Empty;
            company.Text = string.Empty;
            email.Text = string.Empty;
            phone.Text = string.Empty;
            fax.Text = string.Empty;
            pds.Checked = false;
            agb.Checked = false;
        }

        #endregion

// ReSharper disable InconsistentNaming
        protected void Page_Load(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            this.PreRender += this.CommonCreateBusinessUserPreRender;
            SetFieldFromConfig(this.FieldInfos, MostTopElementPlaceHolder);
        }

        protected void CommonCreateBusinessUserPreRender(object sender, EventArgs e)
        {
            litTitle.Visible = this.HasTitle;
            btnCancel.Visible = this.HasCancel;
            if (this.HasCancel)
            {
                btnCancel.OnClientClick = string.Format("{0};return false;", this.ClientCancelScript);
            }

            btnAccept.Enabled = EnableBtnRequestBasedOnAgb(this.FieldInfos, agb, pds);
        }

        protected void BtnAcceptClicked(object sender, EventArgs args)
        {
            if (!Page.IsValid)
            {
                return;
            }

            const string redirectOnError = "error/GeneralErrorPage.aspx";

            try
            {
                //TODO: update to WCF Service
                var users = DatashopService.Instance.JobService.GetUserByEmail(email.Text);

                bool businessUserAlreadyExists = users.Where(u => u.BizUser != null).Count() > 0;
                if (businessUserAlreadyExists)
                {
                    string msg = LoadStr(2317, "Username with specified email already exists");
                    ShowMessage(msg);
                    return;
                }

                var newUser = new User
                    {
                        Salutation = salutation.SelectedItem.Text,
                        City = city.Text,
                        CityCode = zip.Text,
                        Email = email.Text,
                        LastName = lastName.Text,
                        FirstName = firstName.Text,
                        Street = street.Text,
                        StreetNr = streetNumber.Text,
                        Company = company.Text,
                        Tel = phone.Text,
                        Fax = fax.Text
                    };

                var userId = DatashopService.Instance.JobService.CreateUser(newUser);
                if (userId <= 0)
                {
                    throw new Exception(string.Format("Registration failed for user {0}", newUser.Email));
                }

                // wia: the rest is the client page's responsibility
                if (this.UserCreated != null)
                {
                    this.UserCreated(userId);
                }

                //////FormsAuthentication.SignOut();

                //////if (Membership.ValidateUser(userID.ToString(), null))
                //////{
                //////    var ticket = FormsAuthentication.GetAuthCookie(userID.ToString(), true);
                //////    ticket.Expires = DateTime.Now.AddMinutes(20);
                //////    Response.Cookies.Add(ticket);
                //////}
                //////else
                //////{
                //////    redirectOnError = "error/LoginErrorPage.aspx";
                //////    throw new Exception(string.Format("Authentication failed for user {0}", userID));
                //////}
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["lasterror"] = ex;
                LogError(ex.Message, ex);
                Response.RedirectSafe(redirectOnError, false);
            }

            //////Response.RedirectSafe(DatashopWebConfig.Instance.DefaultRequestPage.PageName, false);
        }
    }
}