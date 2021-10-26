using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GEOCOM.GNSD.Web.Core.ServerControls;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// Attention: this requires dojo.require("dijit.form.FilteringSelect"); on the page 
    /// when IsDojoFilteringSelect is true
    /// </summary>
    [PersistChildren(true)]
    public class LabelAndDropDown : ValidatedControl
    {
        protected DropDownList dropDown;

        protected Label label;

        protected PlaceHolder placeHolder;

        protected HtmlGenericControl wrap;

        private bool _visible = true;

        private string _dropDownCssClass;

        public LabelAndDropDown()
        {
            this.PreRender += LabelAndDropDownPreRender;
        }

        protected void LabelAndDropDownPreRender(object sender, EventArgs e)
        {
            foreach (string key in Attributes.Keys)
            {
                dropDown.Attributes.Add(key, Attributes[key]);
            }
        }

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

        public DropDownList DropDown
        {
            get
            {
                return dropDown;
            }
        }

        /// <summary>
        /// Expose the sutopostback property of the dropdown
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                return this.dropDown.AutoPostBack;
            }

            set
            {
                this.dropDown.AutoPostBack = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return dropDown.SelectedIndex;
            }

            set
            {
                dropDown.SelectedIndex = value;
            }
        }

        public ListItemCollection DropDownItems
        {
            get
            {
                return dropDown.Items;
            }

            set
            {
                foreach (ListItem listItem in value)
                {
                    dropDown.Items.Add(listItem);
                }
            }
        }

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

        public ListItem SelectedItem
        {
            get
            {
                return dropDown.SelectedItem;
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

        public override bool Required
        {
            get
            {
                return base.Required;
            }

            set
            {
                base.Required = value;
                SetDropDownRequired(value);
            }
        }

        public event EventHandler DropDownLoad
        {
            add
            {
                dropDown.Load += value;
            }

            remove
            {
                dropDown.Load -= value;
            }
        }

        public event EventHandler SelectedIndexChanged
        {
            add
            {
                dropDown.SelectedIndexChanged += value;
            }

            remove
            {
                dropDown.SelectedIndexChanged -= value;
            }
        }

        public string DropDownCssClass
        {
            get
            {
                return _dropDownCssClass;
            }

            set
            {
                dropDown.CssClass = _dropDownCssClass = value;
            }
        }

        public string DropDownRequiredCssClass { get; set; }

        protected void SetDropDownRequired(bool required)
        {
            dropDown.CssClass = required ? DropDownCssClass + " " + DropDownRequiredCssClass : DropDownCssClass;
        }

        public string SelectedId { get { return Page.Request.Form[dropDown.UniqueID]; }
        }

        protected override void OnValidate(ValidateEventArgs e)
        {
            var skipValidation = !Required || !Visible;
            if (skipValidation)
            {
                return;
            }

            var emptyItemSelected = string.IsNullOrEmpty(SelectedId);
            if (emptyItemSelected)
            {
                e.IsValid = false;
                e.Message = ValidationFailedText;
            }
        }
    }
}