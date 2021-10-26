using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace GEOCOM.GNSD.Web.Core.Security.WebCaptcha
{
	/// <summary>
	/// Captcha server control
	/// </summary>
	[DefaultProperty("Text")]
	public partial class CaptchaControl : WebControl, INamingContainer
	{
		private CustomValidator _customValidator;

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="CaptchaControl"/> class.
		/// </summary>
		public CaptchaControl()
		{

			LayoutOrientation = Layout.Horizontal;

			CaptchaHeight = 50;
			CaptchaWidth = 180;
			CaptchaFontWarping = CaptchaImage.FontWarpFactor.Low;
			CaptchaBackgroundNoise = CaptchaImage.BackgroundNoiseLevel.Low;
			CaptchaLineNoise = CaptchaImage.LineNoiseLevel.None;
			CaptchaChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			CaptchaLength = 5;

			CaptchaMaxTimeout = 90;
			CaptchaMinTimeout = 3;

			CacheStrategy = CacheType.HttpRuntime;

			RefreshButtonText = "Refresh";
			CaptchaExpiredText = "The code you typed has expired after {0} seconds.";
			CaptchaNotReadyText = "Code was typed too quickly. Wait at least {0} seconds.";
			CaptchaValidationFailedText = "The code you typed does not match the code in the image.";
			CaptchaInputLabelText = "Type the visible code here";
		}

		#endregion

		#region Base class overrides and event lifecycle

		/// <summary>
		/// Saves any server control state changes that have occurred since the time the page was posted back to the server.
		/// </summary>
		/// <returns>
		/// Returns the server control's current state. If there is no state associated with the control, this method returns null.
		/// </returns>
		protected override object SaveControlState()
		{
			if (!Visible)
				return null;
				
			var cached = GetCachedCaptcha(captcha.UniqueId);

			return cached == null ? captcha.UniqueId : cached.UniqueId;
		}

		/// <summary>
		/// Loads the state of the control.
		/// </summary>
		/// <param name="state">The state.</param>
		protected override void LoadControlState(object state)
		{
			if (state != null)
				guidIdentifier = Convert.ToString(state);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Page.RegisterRequiresControlState(this);

			_customValidator = new CustomValidator();
			_customValidator.ServerValidate += (source, args) =>
			{
				args.IsValid =
				Validate();
			};
			_customValidator.ValidationGroup = ValidationGroup;

			Page.Validators.Add(_customValidator);


			EnsureChildControls();
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			image = new Image { CssClass = CaptchaImageCssClass };
			textbox = new TextBox { CssClass = InputFieldCssClass };
			btnRefresh = new ImageButton
								  {
									  CausesValidation = false,
									  ToolTip = RefreshButtonText,
									  ImageUrl = "~/images/refresh.png",
									  CssClass = RefreshButtonCssClass
								  };
			btnRefresh.Click += btnRefresh_Click;
			Controls.Add(image);
			Controls.Add(new LiteralControl("<div>"));
			Controls.Add(new Label
								  {
									  Text = CaptchaInputLabelText,
									  AssociatedControlID = textbox.ID,
									  CssClass = InputLabelCssClass
								  });

			Controls.Add(textbox);
			Controls.Add(btnRefresh);
			Controls.Add(new LiteralControl("</div>"));
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			if (!Visible)
				return;

			base.OnLoad(e);

			if (!Page.IsPostBack)
				GenerateNewCaptcha();

			if (Page.IsPostBack)
				captcha = GetCachedCaptcha(guidIdentifier);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (captcha == null)
			{
				Refresh();
			}
			image.ImageUrl = string.Format(Page.Server.UrlPathEncode("~/CaptchaImage.ashx?guid={0}{1}"),
				captcha.UniqueId, CacheStrategy == CacheType.Session ? "&s=1" : "");
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Unload"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			if (Page != null)
				Page.Validators.Remove(_customValidator);
		}

		/// <summary>
		/// Renders the HTML opening tag of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.Write(string.IsNullOrWhiteSpace(CssClass) ? "<div>" : string.Format("<div class=\"{0}\">", CssClass));
		}

		/// <summary>
		/// Renders the HTML closing tag of the control into the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			writer.Write("</div>");
		}

		#endregion

		#region Control Events

		/// <summary>
		/// Handles the Click event of the btnRefresh control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRefresh_Click(object sender, EventArgs e)
		{
			GenerateNewCaptcha();

			textbox.Text = string.Empty;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// generate a new captcha and store it in the ASP.NET Cache by unique GUID
		/// </summary>
		private void GenerateNewCaptcha()
		{
			if (!IsDesignMode)
			{
				captcha = new CaptchaImage
								   {
									   Height = CaptchaHeight,
									   Width = CaptchaWidth,
									   FontWarp = CaptchaFontWarping,
									   BackgroundNoise = CaptchaBackgroundNoise,
									   LineNoise = CaptchaLineNoise,
									   TextLength = CaptchaLength,
									   TextChars = CaptchaChars,
									   Font = CaptchaFont
								   };

				if (CacheStrategy == CacheType.HttpRuntime)
					HttpRuntime.Cache.Add(captcha.UniqueId, captcha, null, DateTime.Now.AddSeconds(Convert.ToDouble((CaptchaMaxTimeout == 0 ? 90 : CaptchaMaxTimeout))), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.NotRemovable, null);
				else
					HttpContext.Current.Session.Add(captcha.UniqueId, captcha);
			}
		}

		/// <summary>
		/// Gets the cached captcha.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		private CaptchaImage GetCachedCaptcha(string guid)
		{
			if (CacheStrategy == CacheType.HttpRuntime)
				return (CaptchaImage)HttpRuntime.Cache.Get(guid);

			return (CaptchaImage)HttpContext.Current.Session[guid];
		}

		/// <summary>
		/// Removes the cached captcha.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		private void RemoveCachedCaptcha(string guid)
		{
			if (CacheStrategy == CacheType.HttpRuntime)
				HttpRuntime.Cache.Remove(guid);
			else
				HttpContext.Current.Session.Remove(guid);
		}

		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		public void Refresh()
		{
			RemoveCachedCaptcha(guidIdentifier);
			textbox.Text = null;
			GenerateNewCaptcha();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Evaluates the condition it checks and updates the <see cref="P:System.Web.UI.IValidator.IsValid"/> property.
		/// </summary>
		public bool Validate()
		{
			var allOtherValidatorValid =
				Page.Validators.Cast<IValidator>().Except(new[] { _customValidator }).All(validator => validator.IsValid);

			// do the captcha validation only if everything else is valid
			if (!allOtherValidatorValid)
				return true;

			
			//pass validation
			if (!Visible | !Enabled)
			{
				return true;
			}

			//-- retrieve the previous captcha from the cache to inspect its properties
			var ci = GetCachedCaptcha(guidIdentifier);

			if (ci == null)
			{
				ErrorMessage = string.Format(CaptchaExpiredText, CaptchaMaxTimeout);

				GenerateNewCaptcha();

				return false;
			}

			//--  was it entered too quickly?
			if (CaptchaMinTimeout > 0 && (ci.RenderedAt.AddSeconds(CaptchaMinTimeout) > DateTime.Now))
			{
				ErrorMessage = string.Format(CaptchaNotReadyText, CaptchaMinTimeout);

				Refresh();

				return false;
			}


			//validate the input
			if (!ci.Text.Equals(textbox.Text, StringComparison.CurrentCultureIgnoreCase))
			{
				ErrorMessage = CaptchaValidationFailedText;

				Refresh();

				return false;
			}
			
			

			RemoveCachedCaptcha(guidIdentifier);
			return true;
		}

		#endregion
	}
}