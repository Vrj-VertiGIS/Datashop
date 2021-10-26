using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using GEOCOM.GNSD.Web.Core.Security;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web.Controls
{
    public delegate void BusinessUserCreated(long userId);

    /// <summary>
    /// Dialog used in the CreateBusinessUser page and in the ActAsSurrogate user control.
    /// </summary>
    public partial class CommonCreateBusinessUser : CommonCreateUser 
    {
        #region Public events

        public event BusinessUserCreated UserCreated;

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
            password.Text = string.Empty;
            passwordRepeated.Text = string.Empty;
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

            if (!this.PasswordsMatch())
            {
                ShowMessage(LoadStr(2314, "The passwords do not match."));
                return;
            }

            string redirectOnError = "error/GeneralErrorPage.aspx";

            try
            {
                var users = DatashopService.Instance.JobService.GetUserByEmail(email.Text);

                bool businessUserAlreadyExists = users.Where(u => u.BizUser != null).Count() > 0;
                if (businessUserAlreadyExists)
                {
                    string msg = LoadStr(2317, "Username with specified email already exists");
                    ShowMessage(msg);
                    return;
                }

                var newUser = new User();
                var newBizUser = new BizUser();
                var sec = new DatashopMembershipProvider();
                sec.Initialize(string.Empty, new NameValueCollection());

                string salt = null;
                newBizUser.Password = sec.TransformPassword(password.Text, ref salt);
                newBizUser.PasswordSalt = salt;

                newUser.Salutation = salutation.SelectedItem.Text;
                newUser.City = city.Text;
                newUser.CityCode = zip.Text;
                newUser.Email = email.Text;
                newUser.LastName = lastName.Text;
                newUser.FirstName = firstName.Text;
                newUser.Street = street.Text;
                newUser.StreetNr = streetNumber.Text;
                newUser.Company = company.Text;
                newUser.Tel = phone.Text;
                newUser.Fax = fax.Text;

                newBizUser.Roles = "BUSINESS";
                newBizUser.UserStatus = BizUserStatus.LOCKED;

                var newId = DatashopService.Instance.JobService.CreateBizUserAndSendAdminMail(newBizUser, newUser);
                if (newId <= 0)
                {
                    redirectOnError = "error/LoginErrorPage.aspx";
                    throw new Exception("Creating new bizUser failed");
                }

                // wia: the rest is the client page's responsibility
                if (this.UserCreated != null)
                {
                    this.UserCreated(newId);
                }
                //////FormsAuthentication.SignOut();
                //////if (Membership.ValidateUser(newID.ToString(), password.Text))
                //////{
                //////    var ticket = FormsAuthentication.GetAuthCookie(newID.ToString(), true);
                //////    ticket.Expires = DateTime.Now.AddMinutes(20);
                //////    Response.Cookies.Add(ticket);
                //////}
                //////else
                //////{
                //////    redirectOnError = "error/LoginErrorPage.aspx";
                //////    throw new Exception(string.Format("Authentication failed for user {0}", newID));
                //////}
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["lasterror"] = ex;
                this.LogError(ex.Message, ex);
                Response.RedirectSafe(redirectOnError, false);
            }

            //////Response.RedirectSafe(DatashopWebConfig.Instance.DefaultRequestPage.PageName, false);
        }

        private bool PasswordsMatch()
        {
            bool match = password.Text.Equals(passwordRepeated.Text, StringComparison.InvariantCulture);
            return match;
        }
    }
}