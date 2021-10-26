using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Core.Security.WebCaptcha
{
	#region Enums

	/// <summary>
	/// Specifies the layout orientation of the control
	/// </summary>
	public enum Layout
	{
		/// <summary>
		/// Specifies a horizontal layout
		/// </summary>
		Horizontal,
		/// <summary>
		/// Specifies a vertical layout
		/// </summary>
		Vertical
	}

	/// <summary>
	/// Specifies how the Captcha image instance should be persisted across postbacks
	/// </summary>
	public enum CacheType
	{
		/// <summary>
		/// Specifies the image instance should be persisted in the ASP.net cache
		/// </summary>
		HttpRuntime,
		/// <summary>
		/// Specifies the image should be persisted in session
		/// </summary>
		Session
	}

	#endregion

	/// <summary>
	/// Partial class file for CaptchaControl, holds all the properties
	/// </summary>
	public partial class CaptchaControl
	{
		#region Private members

		/// <summary>
		/// Holds the instance of the CaptchaImage class for this control
		/// </summary>
		private CaptchaImage captcha;

		/// <summary>
		/// holds a string representation of the guid used to uniquely identify the captcha image
		/// </summary>
		private string guidIdentifier;

		/// <summary>
		/// holds the ASP.net image control for this instance
		/// </summary>
		private Image image;

		/// <summary>
		/// holds the ASP.net textbox control for this instance
		/// </summary>
		private TextBox textbox;

		/// <summary>
		/// holds the ASP.net refresh button control for this instance
		/// </summary>
		private ImageButton btnRefresh;

		/// <summary>
		/// Gets a value indicating whether this instance is in design mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is design mode; otherwise, <c>false</c>.
		/// </value>
		private bool IsDesignMode
		{
			get { return HttpContext.Current == null; }
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the captcha image CSS class.
		/// </summary>
		/// <value>
		/// The captcha image CSS class.
		/// </value>
		[Browsable(true)]
		public string CaptchaImageCssClass { get; set; }

		/// <summary>
		/// Gets or sets the refresh button CSS class.
		/// </summary>
		/// <value>
		/// The refresh button CSS class.
		/// </value>
		[Browsable(true)]
		public string RefreshButtonCssClass { get; set; }

		[Browsable(true)]
		public string ValidationGroup { get; set; }

		/// <summary>
		/// Gets or sets the input field CSS class.
		/// </summary>
		/// <value>
		/// The input field CSS class.
		/// </value>
		[Browsable(true)]
		public string InputFieldCssClass { get; set; }

		/// <summary>
		/// Gets or sets the input label CSS class.
		/// </summary>
		/// <value>
		/// The input label CSS class.
		/// </value>
		[Browsable(true)]
		public string InputLabelCssClass { get; set; }

		/// <summary>
		/// Gets or sets the refresh button text.
		/// </summary>
		/// <value>
		/// The refresh button text.
		/// </value>
		[Browsable(true)]
		[Localizable(true)]
		public string RefreshButtonText { get; set; }

		/// <summary>
		/// Gets or sets the captcha expired text.
		/// </summary>
		/// <value>
		/// The captcha expired text.
		/// </value>
		[Browsable(true)]
		[Localizable(true)]
		public string CaptchaExpiredText { get; set; }

		/// <summary>
		/// Gets or sets the captcha not ready text.
		/// </summary>
		/// <value>
		/// The captcha not ready text.
		/// </value>
		[Browsable(true)]
		[Localizable(true)]
		public string CaptchaNotReadyText { get; set; }

		/// <summary>
		/// Gets or sets the captcha validation failed text.
		/// </summary>
		/// <value>
		/// The captcha validation failed text.
		/// </value>
		[Browsable(true)]
		[Localizable(true)]
		public string CaptchaValidationFailedText { get; set; }

		/// <summary>
		/// Gets or sets the captcha input label text.
		/// </summary>
		/// <value>
		/// The captcha input label text.
		/// </value>
		[Browsable(true)]
		[Localizable(true)]
		public string CaptchaInputLabelText { get; set; }

		/// <summary>
		/// When implemented by a class, gets or sets the error message text generated when the condition being validated fails.
		/// </summary>
		/// <returns>The error message to generate.</returns>
		[Browsable(false)]
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("The text you typed does not match the text in the image.")]
		[Description("Message to display in a Validation Summary when the CAPTCHA fails to validate.")]
		public string ErrorMessage
		{
			get { return _customValidator.ErrorMessage; }
			set { _customValidator.ErrorMessage = value; }
		}

		/// <summary>
		/// When implemented by a class, gets or sets a value indicating whether the user-entered content in the specified control passes validation.
		/// </summary>
		/// <returns>true if the content is valid; otherwise, false.</returns>
		[Browsable(false)]
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Is Valid")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsValid
		{
			get { return _customValidator.IsValid; }

		}

		/// <summary>
		/// Gets or sets a value indicating whether the Web server control is enabled.
		/// </summary>
		/// <returns>true if control is enabled; otherwise, false. The default is true.</returns>
		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				// When a validator is disabled, generally, the intent is not to
				// make the page invalid for that round trip.
				//if (!value)
				//	this.UserValidated = true;
			}
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>
		/// The text.
		/// </value>
		[DefaultValue("Enter the code shown above:")]
		[Description("Instructional text displayed next to CAPTCHA image.")]
		[Category("Appearance")]
		[Browsable(true)]
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the layout style.
		/// </summary>
		/// <value>
		/// The layout style.
		/// </value>
		[DefaultValue(typeof(Layout), "Horizontal")]
		[Description("Determines if image and input area are displayed horizontally, or vertically.")]
		[Category("Captcha")]
		[Browsable(true)]
		public Layout LayoutOrientation { get; set; }

		/// <summary>
		/// Gets or sets the cache strategy.
		/// </summary>
		/// <value>
		/// The cache strategy.
		/// </value>
		[DefaultValue(typeof(CacheType), "HttpRuntime")]
		[Description("Determines if CAPTCHA codes are stored in HttpRuntime (fast, but local to current server) or Session (more portable across web farms).")]
		[Category("Captcha")]
		[Browsable(true)]
		public CacheType CacheStrategy { get; set; }

		#region Captcha behaviour settings

		/// <summary>
		/// Gets or sets the captcha min timeout.
		/// </summary>
		/// <value>
		/// The captcha min timeout.
		/// </value>
		[DefaultValue(3)]
		[Description("Minimum number of seconds CAPTCHA must be displayed before it is valid.")]
		[Category("Captcha")]
		[Browsable(true)]
		public int CaptchaMinTimeout { get; set; }

		/// <summary>
		/// Gets or sets the captcha max timeout.
		/// </summary>
		/// <value>
		/// The captcha max timeout.
		/// </value>
		[DefaultValue(90)]
		[Description("Maximum number of seconds CAPTCHA will be cached and valid.")]
		[Category("Captcha")]
		[Browsable(true)]
		public int CaptchaMaxTimeout { get; set; }

		#endregion

		#region Captcha Image Properties

		/// <summary>
		/// Gets or sets the height of the captcha.
		/// </summary>
		/// <value>
		/// The height of the captcha.
		/// </value>
		[DefaultValue(50)]
		[Description("Height of generated CAPTCHA image.")]
		[Category("Captcha")]
		[Browsable(true)]
		public int CaptchaHeight { get; set; }

		/// <summary>
		/// Gets or sets the width of the captcha.
		/// </summary>
		/// <value>
		/// The width of the captcha.
		/// </value>
		[DefaultValue(180)]
		[Description("Width of generated CAPTCHA image.")]
		[Category("Captcha")]
		[Browsable(true)]
		public int CaptchaWidth { get; set; }

		/// <summary>
		/// Gets or sets the captcha font warping.
		/// </summary>
		/// <value>
		/// The captcha font warping.
		/// </value>
		[DefaultValue(typeof(CaptchaImage.FontWarpFactor), "Low")]
		[Description("Amount of random font warping used on the CAPTCHA text")]
		[Category("Captcha")]
		[Browsable(true)]
		public CaptchaImage.FontWarpFactor CaptchaFontWarping { get; set; }

		/// <summary>
		/// Gets or sets the captcha background noise.
		/// </summary>
		/// <value>
		/// The captcha background noise.
		/// </value>
		[DefaultValue(typeof(CaptchaImage.BackgroundNoiseLevel), "Low")]
		[Description("Amount of background noise to generate in the CAPTCHA image")]
		[Category("Captcha")]
		[Browsable(true)]
		public CaptchaImage.BackgroundNoiseLevel CaptchaBackgroundNoise { get; set; }

		/// <summary>
		/// Gets or sets the captcha line noise.
		/// </summary>
		/// <value>
		/// The captcha line noise.
		/// </value>
		[DefaultValue(typeof(CaptchaImage.LineNoiseLevel), "None")]
		[Description("Add line noise to the CAPTCHA image")]
		[Category("Captcha")]
		[Browsable(true)]
		public CaptchaImage.LineNoiseLevel CaptchaLineNoise { get; set; }

		/// <summary>
		/// Gets or sets the captcha chars.
		/// </summary>
		/// <value>
		/// The captcha chars.
		/// </value>
		[DefaultValue("")]
		[Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string.")]
		[Category("Captcha")]
		public string CaptchaChars { get; set; }

		/// <summary>
		/// Gets or sets the length of the captcha.
		/// </summary>
		/// <value>
		/// The length of the captcha.
		/// </value>
		[DefaultValue(5)]
		[Description("Number of CaptchaChars used in the CAPTCHA text")]
		[Category("Captcha")]
		public int CaptchaLength { get; set; }

		/// <summary>
		/// Gets or sets the captcha font.
		/// </summary>
		/// <value>
		/// The captcha font.
		/// </value>
		[DefaultValue("")]
		[Description("Font used to render CAPTCHA text. If font name is blank, a random font will be chosen.")]
		[Category("Captcha")]
		public string CaptchaFont { get; set; }

		#endregion

		#endregion
	}
}