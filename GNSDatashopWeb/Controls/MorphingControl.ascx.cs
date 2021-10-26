using System;
using System.Web.UI;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web.Controls
{
    /// <summary>
    /// Dynamically adds controls to a page base on the suplied RequestPageMode and ControlPath.
    /// </summary>
    public partial class MorphingControl : UserControl
    {
        private Control _control;

        /// <summary>
        /// Sets or gets the RequestPageMode for which the control will be added to the page.
        /// </summary>
        public RequestPageMode RequestPageMode { get; set; }

        /// <summary>
        /// Relative path to the added control, e.g. ~/Controls/MyVeryCoolControl.ascx.
        /// </summary>
        public string ControlPath { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (DatashopWebConfig.Instance.RequestPageConfig.ActiveMode == RequestPageMode)
            {
                _control = Page.LoadControl(ControlPath);
                controlHolder.Controls.Add(_control);
            }
        }

        /// <summary>
        /// Returns the control's instance safely casted to the supplied type.
        /// </summary>
        public T GetControl<T>() where T:Control
        {
            return _control as T;
        }
    }
}