
namespace GEOCOM.GNSD.Web.Controls
{
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// This class is a helper that appends buttons and/or separators to a given container.
    /// It ensures that all buttons have the same layout and behave same
    /// It does not render anything.
    /// IMPORTANT: this class MUST be instanciated at page load time or later.
    /// </summary>
    public class DsToolbar
    {
        // these local variables hold the values passed to the creator
        private string tbId;
        private string btnOutDefaultClass;
        private string btnInDefaultClass;
        private string btnOutHoverClass;
        private string btnInHoverClass;
        private string btnOutPressedClass;
        private string btnInPressedClass;
        private string btnOutSelectedClass;
        private string btnInSelectedClass;
        private string btnOutDisabledClass;
        private string btnInDisabledClass;

        /// <summary>
        /// This class is a helper that appends buttons and/or separators to a given container.
        /// It does not render anything.
        /// </summary>
        /// <param name="tbId">Javascript id of the toolbar object</param>
        /// <param name="btnOutDefaultClass">Default css class name for the outer button</param>
        /// <param name="btnInDefaultClass">Default css class name for the inner button</param>
        /// <param name="btnOutHoverClass">Mouse over css class name for the outer button</param>
        /// <param name="btnInHoverClass">Mouse over css class name for the inner button</param>
        /// <param name="btnOutPressedClass">Button pressed css classname for the outer button</param>
        /// <param name="btnInPressedClass">Button pressed css classname for the inner button</param>
        /// <param name="btnOutSelectedClass">Button selected css classname for the outer button</param>
        /// <param name="btnInSelectedClass">Button selected css classname for the inner button</param>
        /// <param name="btnOutDisabledClass">Button disabled css classname for the outer button</param>
        /// <param name="btnInDisabledClass">Button disabled css classname for the inner button</param>
        public DsToolbar(
            string tbId, 
            string btnOutDefaultClass, 
            string btnInDefaultClass, 
			string btnOutHoverClass, 
            string btnInHoverClass, 
            string btnOutPressedClass, 
            string btnInPressedClass,
            string btnOutSelectedClass, 
            string btnInSelectedClass, 
            string btnOutDisabledClass, 
            string btnInDisabledClass)
        {
            this.tbId = tbId;
            this.btnOutDefaultClass = btnOutDefaultClass;
            this.btnInDefaultClass = btnInDefaultClass;
            this.btnOutHoverClass = btnOutHoverClass;
            this.btnInHoverClass = btnInHoverClass;
            this.btnOutPressedClass = btnOutPressedClass;
            this.btnInPressedClass = btnInPressedClass;
            this.btnOutSelectedClass = btnOutSelectedClass;
            this.btnInSelectedClass = btnInSelectedClass;
            this.btnOutDisabledClass = btnOutDisabledClass;
            this.btnInDisabledClass = btnInDisabledClass;
        }

        /// <summary>
        /// Adds a button to the toolbar.
        /// This wrapper handles the "selected" and "disabled" preselections
        /// See DsToolbarButton for more.
        /// </summary>
        /// <param name="toolbar">The host toolbar</param>
        /// <param name="cmdId">The ID of the new command button</param>
        /// <param name="iconClassName">The classname for the icon, when any</param>
        /// <param name="onClick">The javascript event handler for this button</param>
        /// <param name="tooltip">The tooltip text for this button</param>
        /// <param name="disabled">To disable the button at load time</param>
        /// <param name="selected">To check the button at load time</param>
        /// <param name="visible">To show/hide the button at load time (the button is loaded anyway)</param>
        /// <returns>The newly created toolbar button</returns>
        public DsToolbarButton AddButton(HtmlGenericControl toolbar, string cmdId, string iconClassName, string onClick, string tooltip, bool disabled, bool selected, bool visible)
        {
            var outClassName = this.btnOutDefaultClass;
            var inClassName = this.btnInDefaultClass + iconClassName;
            if (selected)
            {
                outClassName = this.btnOutSelectedClass;
                inClassName = this.btnInSelectedClass + iconClassName;
            }

            if (disabled)
            {
                outClassName = this.btnOutDisabledClass;
                inClassName = this.btnInDisabledClass + iconClassName;
            }

            var button = new DsToolbarButton(
                cmdId, 
                outClassName, 
                inClassName, 
                iconClassName,
                string.Format("{0}.BtnMouseOver(event)", tbId), 
                string.Format("{0}.BtnMouseOut(event)", tbId),
                string.Format("{0}.BtnMouseUp(event)", tbId), 
                string.Format("{0}.BtnMouseDown(event)", tbId), 
                onClick, 
                tooltip, 
                disabled, 
                selected, 
                visible);
            toolbar.Controls.Add(button);
            return button;
        }

        /// <summary>
        /// Adds a separator to the toolbar.
        /// See DsToolbarSeparator for more.
        /// </summary>
        /// <param name="toolbar">The toolbar that hosts the separator</param>
        /// <param name="className">The class for this separator (a span)</param>
        /// <param name="visible">To show/hide the separator</param>
        public void AddSeparator(HtmlGenericControl toolbar, string className, bool visible)
        {
            var separator = new DsToolbarSeparator(className, visible);
            toolbar.Controls.Add(separator);
        }

        /// <summary>
        /// This is not implemented - demo only !
        /// The intention was to create a zooming slider out of the map (in a toolbar)
        /// </summary>
        /// <param name="toolbar">The toolbar that hosts the separator</param>
        /// <param name="className">The class for this separator (a span)</param>
        /// <param name="visible">To show/hide the separator</param>
        public void AddSlider(HtmlGenericControl toolbar, string className, bool visible)
        {
            var separator = new DsToolbarSeparator(className, visible);
            toolbar.Controls.Add(separator);
        }
    }
}
