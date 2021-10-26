using GEOCOM.GNSD.Web.Config;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.ServerControls;
using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Controls
{

    public class LabelAndTextBox : ValidatedControl, IEditableTextControl, ILabelAndTextBox
    {
        private bool _visible = true;
        private string _textBoxCssClass;

        // do not know why they did remove the designer ???
        protected TextBox textBox;
        protected Label label;
        protected PlaceHolder placeHolder;
        protected HtmlGenericControl wrap;

        public TextBox TextBox { get { return textBox; } }
        public Label Label { get { return label; } }

        public override bool Visible
        {
            get
            {
                return _visible;
            }

            set
            {
                placeHolder.Visible = _visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the placeholder HTML attribute of the text box.
        /// </summary>
        /// <value>
        /// The placeholder.
        /// </value>
        public string Placeholder
        {
            get
            {
                return textBox.Attributes["placeholder"];
            }
            set
            {
                textBox.Attributes.Add("placeholder", value);
            }
        }

        // wia used for the welcome page
        public bool HasFocus { get; set; }

        public bool Enabled
        {
            get
            {
                return textBox.Enabled;
            }

            set
            {
                textBox.Enabled = value;
            }
        }

        public string RegEx { get; set; }

        public string LabelText
        {
            get
            {
                return label.Text;
            }

            set
            {
                label.Text = value;
            }
        }

        public string LabelCssClass
        {
            get
            {
                return label.CssClass;
            }

            set
            {
                label.CssClass = value;
            }
        }

        public TextBoxMode TextMode
        {
            get
            {
                return textBox.TextMode;
            }

            set
            {
                textBox.TextMode = value;
            }
        }

        public string TextBoxCssClass
        {
            get
            {
                return _textBoxCssClass;
            }

            set
            {
                _textBoxCssClass = value;
                textBox.CssClass = _textBoxCssClass;
            }
        }

        public string TextBoxRequiredCssClass { get; set; }

        public string TextBoxClientID
        {
            get { return textBox.ClientID; }
        }

        public string LabelClientID
        {
            get { return label.ClientID; }
        }

        public string Text
        {
            get
            {
                return textBox.Text;
            }

            set
            {
                textBox.Text = value;
            }
        }

        public string TextBoxClientId
        {
            get { return textBox.ClientID; }
        }

        public override bool Required
        {
            get
            {
                return base.Required;
            }

            set
            {
                base.Required = value;
                SetTextBoxRequired(value);
            }
        }

        protected void SetTextBoxRequired(bool required)
        {
            textBox.CssClass = required ? TextBoxCssClass + " " + TextBoxRequiredCssClass : TextBoxCssClass;
        }

        public bool IsDojoDateTextBox { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsDojoDateTextBox)
            {
                // get the required resources
                ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "DojoCalendar", ResolveClientUrl("~/js/DojoCalendar.js"));

                // decorate it with the attributes required by the dojo parser
                textBox.Attributes.Add("dojoType", "dijit.form.DateTextBox");

                // decorate the wrapper to enforce css-styling the inner text boxes
                this.wrap.Attributes.Add("class", Required ? "calendarWrap" : "calendarWrapRequired");
            }
        }

        public override void DataBind(ValidatedControlDataItem dataItem)
        {
            base.DataBind(dataItem);
            var pageFieldInfo = dataItem as PageFieldInfo;
            if (pageFieldInfo != null && !string.IsNullOrEmpty(pageFieldInfo.RegEx))
            {
                RegEx = pageFieldInfo.RegEx;
            }
        }

        public string RegExValidationFailedText { get; set; }

        public bool SkipIllegalCharsCheck { get; set; }

        public string IllegalCharsRegEx { get; set; } = @"[\^_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]";

        public int MaxFieldLength { get; set; } = 100;

        protected override void OnValidate(ValidateEventArgs e)
        {
            if (!Visible)
                return;

            if (Required && string.IsNullOrEmpty(textBox.Text))
            {
                e.IsValid = false;
                e.Message = ValidationFailedText;
                return;
            }

            if (!string.IsNullOrEmpty(RegEx))
            {
                Match match = Regex.Match(textBox.Text, RegEx);
                if (!match.Success)
                {
                    e.Message = RegExValidationFailedText;
                    e.IsValid = false;
                    return;
                }
            }
            else if (!SkipIllegalCharsCheck)
            {
                Match match = Regex.Match(textBox.Text, IllegalCharsRegEx, RegexOptions.Compiled);
                if (match.Success)
                {
                    var message = WebLanguage.LoadStr(2100, "The field '{0}' contains illegal characters.");
                    e.Message = string.Format(message, LabelText.Replace(":", ""));
                    e.IsValid = false;
                    return;
                }
            }

            if (textBox.Text?.Length > MaxFieldLength)
            {
                var message = WebLanguage.LoadStr(2101, "The field '{0}' is too long.");
                e.Message = string.Format(message, LabelText.Replace(":", ""));
                e.IsValid = false;
                return;
            }
        }

        public event EventHandler TextChanged
        {
            add
            {
                textBox.TextChanged += value;
            }

            remove
            {
                textBox.TextChanged -= value;
            }
        }
    }
}