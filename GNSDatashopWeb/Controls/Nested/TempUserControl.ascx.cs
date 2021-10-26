using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.ErrorHandling.Exceptions;
using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Controls;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.UserData;

namespace GEOCOM.GNSD.Web
{
	public partial class TempUserControl : UserControl
	{
		#region Private properties

		/// <summary>
		/// Holds the logger instance for this page
		/// </summary>
		private readonly IMsg log = new Msg(typeof(LoginTempUser));

		#endregion

		#region Page Event lifecycle

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			btnAccept.Page.Form.DefaultButton = btnAccept.UniqueID; //to prevent the captcha reload button from being the main button (hidding enter)
			if (!this.IsPostBack)
			{
				this.EnableBtnRequestBasedOnAgb();
			}

			captcha.Visible = !DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.DisableCaptcha;
			captcha.Enabled = !DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.DisableCaptcha;

			this.SetFieldFromConfig();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the field from config.
		/// </summary>
		private void SetFieldFromConfig()
		{
			Utils.SetFieldByPageFieldInfos(Page, DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.Fields);
		}

		/// <summary>
		/// Enables the BTN request based on agb.
		/// </summary>
		private void EnableBtnRequestBasedOnAgb()
		{
			var agbFieldInfo = DatashopWebConfig.Instance.GetLoginTempUserPageFieldInfos("agb");
			var pdsFieldInfo = DatashopWebConfig.Instance.GetLoginTempUserPageFieldInfos("pds");

			var acceptedAll = (agb.Checked || (agbFieldInfo != null && !agbFieldInfo.Visible)) &&
							  (pds.Checked || (pdsFieldInfo != null && !pdsFieldInfo.Visible));

			btnAccept.Enabled = acceptedAll;
		}

		#endregion

		#region Event handlers

		public void UniqueUserValidator(object source, ServerValidateEventArgs args)
		{
			var users = DatashopService.Instance.JobService.GetUserByEmail(email.Text);

			var businessUserAlreadyExists = users.Any(u => u.BizUser != null);
			if (businessUserAlreadyExists)
			{
				args.IsValid = false;
				var msg = WebLanguage.LoadStr(2317, "Username with specified email already exists");
				Page.ShowMessage(msg);

			}
		}

		/// <summary>
		/// BTNs the accept clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void BtnAcceptClicked(object sender, EventArgs args)
		{
			if (!Page.IsValid)
			{
				if (!DatashopWebConfig.Instance.LoginTempUserPageFieldInfos.DisableCaptcha && !this.captcha.IsValid)
					Page.ShowMessage(this.captcha.ErrorMessage);

				return;
			}

			var redirectOnError = "error/GeneralErrorPage.aspx";

			try
			{


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

				var userID = DatashopService.Instance.JobService.CreateTempUser(newUser); //CreateUser(newUser);

				if (userID <= 0)
					throw new Exception(string.Format("Registration failed for user {0:s}", newUser.Email));

				FormsAuthentication.SignOut();

				if (Membership.ValidateUser(userID.ToString(), null))
				{
					var ticket = FormsAuthentication.GetAuthCookie(userID.ToString(), true);
					ticket.Expires = DateTime.Now.AddMinutes(20);
					Response.Cookies.Add(ticket);
				}
				else
				{
					redirectOnError = "error/LoginErrorPage.aspx";
					throw new Exception(string.Format("Authentication failed for user {0}", userID));
				}
			}
			catch (FaultException<TempUserTooManyRequestsFault> ex)
			{
				this.RedirectToErrorPage(string.Format(WebLanguage.LoadStr(9020, "The limit of {0} requests in the last {1} days has already been reached."), ex.Detail.Limit, ex.Detail.Period), "error/TempUserLimitError.aspx");
			}
			catch (FaultException<ServiceFault> ex)
			{
				this.RedirectToErrorPage(string.Format(WebLanguage.LoadStr(ex.Detail.LanguageCode, "Error creating user")), redirectOnError);
			}
			catch (Exception ex)
			{
				HttpContext.Current.Session["lasterror"] = ex;

				log.Error(ex.Message, ex);

				Response.RedirectSafe(redirectOnError, true);
			}

			var returnUrl = Request["ReturnUrl"];

			Response.RedirectSafe(string.IsNullOrEmpty(returnUrl) ? DatashopWebConfig.Instance.DefaultRequestPage.PageName : returnUrl, false);
		}

		/// <summary>
		/// Handles the OnCheckedChanged event of the agb control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void agb_OnCheckedChanged(object sender, EventArgs args)
		{
			this.EnableBtnRequestBasedOnAgb();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Redirects to error page.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="url">The URL.</param>
		private void RedirectToErrorPage(string message, string url)
		{
			var x = new Exception(message);

			HttpContext.Current.Session["lasterror"] = x;

			log.Info(x.Message);

			Response.RedirectSafe(url, true);
		}

		public void ShowMessage(string msg)
		{
			Page.ShowMessage(msg);
		}


		#endregion
	}
}