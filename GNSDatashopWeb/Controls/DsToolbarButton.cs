namespace GEOCOM.GNSD.Web.Controls
{
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// This class renders a span (outer button that contains another span (inner button)).
    /// IMPORTANT: this class MUST be instanciated at page page load time or later.
    /// All javascript references are always made to the inner button. So it is easier and safer to reference de outer button (inner.parent).
    /// The inner span eventually holds three non html attributes:
    /// - _selected (used by most event handlers)
    /// - _disabled (used by all event handelrs)
    /// - _iconClass (used by the renderer to combine button state and button icon, when icon not contained in state class </summary>
    public class DsToolbarButton : HtmlGenericControl
    {

        /// <summary>
        /// Make this control an outer button and adds the inner button
        /// </summary>
        /// <param name="cmdId">The DsToolbar maps the html objects using this id</param>
        /// <param name="outClassName">The out css class used at page load time</param>
        /// <param name="inClassName">The in css class used at page load time</param>
        /// <param name="iconClassName">The icon css class used at page load time</param>
        /// <param name="onMouseOver">The client mouseover event handler</param>
        /// <param name="onMouseOut">The client mouseout event handler</param>
        /// <param name="onMouseUp">The client mouseup event handler</param>
        /// <param name="onMouseDown">The mousedown client event handler</param>
        /// <param name="onClick">The click client event handler</param>
        /// <param name="tooltip">The title attribute of the span</param>
        /// <param name="disabled">The disabled state at page load time</param>
        /// <param name="selected">The selected state at page load time</param>
        /// <param name="visible">The element will be rendered with display:none</param>
        public DsToolbarButton(string cmdId, string outClassName, string inClassName, string iconClassName, string onMouseOver, string onMouseOut, string onMouseUp, string onMouseDown, string onClick, string tooltip, bool disabled, bool selected, bool visible)
        {
            // a rendered button looks like this:
            // <span class="btnOut btnGrad">
            //    <span id="cmdZoomIn" class="btnIn btnUnpressed iconCut" 
            //      onmouseover="tbPde.BtnMouseOver(event)" onmouseout="tbPde.BtnMouseOut(event)" 
            //      onmouseup="tbPde.BtnMouseUp(event)" onmousedown="tbPde.BtnMouseDown(event)" onclick="tbPde.StartZoomIn()" 
            //      _iconClass=" iconCut" title="Zoom-in the map">
            //    &nbsp;
            //    </span>
            // </span>

            // make this control an outer button
            this.TagName = "span";
            this.Attributes.Add("class", outClassName);
            
            // create the inner button
            var btnIn = new HtmlGenericControl();
// ReSharper disable DoNotCallOverridableMethodsInConstructor - wia this object is created from a page_load method
            this.Controls.Add(btnIn); // this is why it must be instanciated at page load time or later
// ReSharper restore DoNotCallOverridableMethodsInConstructor
            
            // configure the inner button
            btnIn.TagName = "span";
            btnIn.Style.Add("display", "inline-block"); // do not put this in the css file, please...
            btnIn.Attributes.Add("id", cmdId);
            btnIn.Attributes.Add("class", inClassName);
            btnIn.Attributes.Add("onmouseover", onMouseOver);
            btnIn.Attributes.Add("onmouseout", onMouseOut);
            btnIn.Attributes.Add("onmouseup", onMouseUp);
            btnIn.Attributes.Add("onmousedown", onMouseDown);
            btnIn.Attributes.Add("onclick", onClick);
            btnIn.Attributes.Add("title", tooltip);

            // ds specific, non-html attributes
            btnIn.Attributes.Add("_iconClass", iconClassName);
            if (selected)
                btnIn.Attributes.Add("_selected", "true");
            if (disabled)
                btnIn.Attributes.Add("_disabled", "true");
            if (visible)
            {
                btnIn.Attributes.Add("_visible", "true");
                this.Style.Add("display", "inline-block"); // do tnor put this in the css file, please... 
            }
            else
            {
                btnIn.Attributes.Add("_visible", "false");
                this.Style.Add("display", "none");
            }
        }
    }
}
