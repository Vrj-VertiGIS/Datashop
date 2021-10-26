using System.Web.UI.HtmlControls;

namespace GEOCOM.GNSD.Web.Core.WebControls
{
	/// <summary>
	/// This class is a helper that appends buttons and/or separators to a given container.
	/// It ensures that all buttons have the same layout and behave same
	/// It does not render anything.
	/// IMPORTANT: this class MUST be instanciated at page load time or later.
	/// </summary>
	public class DsToolbar
	{
		// these local variables hold the values passed to the creator
		string tbId;
		string btnOutDefaultClass;
		string btnInDefaultClass;
		string btnOutHoverClass;
		string btnInHoverClass;
		string btnOutPressedClass;
		string btnInPressedClass;
		string btnOutSelectedClass;
		string btnInSelectedClass;
		string btnOutDisabledClass;
		string btnInDisabledClass;

		/// <summary>
		/// This class is a helper that appends buttons and/or separators to a given container.
		/// It does not render anything.
		/// </summary>
		/// <param name="tbId">Javascript id of the toolbar object</param>
		/// <param name="btnOutDefaultClass">default css class name for the outer button</param>
		/// <param name="btnInDefaultClass">default css class name for the inner button</param>
		/// <param name="btnOutHoverClass">mouse over css class name for the outer button</param>
		/// <param name="btnInHoverClass">mouse over css class name for the inner button</param>
		/// <param name="btnOutPressedClass">button pressed css classname for the outer button</param>
		/// <param name="btnInPressedClass">button pressed css classname for the inner button</param>
		/// <param name="btnOutSelectedClass">button selected css classname for the outer button</param>
		/// <param name="btnInSelectedClass">button selected css classname for the inner button</param>
		/// <param name="btnOutDisabledClass">button disabled css classname for the outer button</param>
		/// <param name="btnInDisabledClass">button disabled css classname for the inner button</param>
		public DsToolbar(string tbId, string btnOutDefaultClass, string btnInDefaultClass, 
				string btnOutHoverClass, string btnInHoverClass, string btnOutPressedClass, string btnInPressedClass,
				string btnOutSelectedClass, string btnInSelectedClass, string btnOutDisabledClass, string btnInDisabledClass)
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

		/// Adds a button to the toolbar.
		/// This wrapper handles the "selected" and "disabled" preselections
		/// See DsToolbarButton for more.
		public DsToolbarButton AddButton(HtmlGenericControl toolbar, string cmdId, string iconClassName, string onClick, string tooltip, bool disabled, bool selected, bool visible)
		{
			var outClassName = btnOutDefaultClass;
			var inClassName = btnInDefaultClass + iconClassName;
			if (selected)
			{
				outClassName = btnOutSelectedClass;
				inClassName = btnInSelectedClass + iconClassName;
			}
			if (disabled)
			{
				outClassName = btnOutDisabledClass;
				inClassName = btnInDisabledClass + iconClassName;
			}
			var button = new DsToolbarButton(cmdId, outClassName, inClassName, iconClassName,
				string.Format("{0}.BtnMouseOver(event)", tbId), string.Format("{0}.BtnMouseOut(event)", tbId),
				string.Format("{0}.BtnMouseUp(event)", tbId), string.Format("{0}.BtnMouseDown(event)", tbId), 
				onClick, tooltip, disabled, selected, visible);
			toolbar.Controls.Add(button);
			return button;
		}

	    /// <summary>
	    /// Adds a separator to the toolbar.
	    /// See DsToolbarSeparator for more.
	    /// </summary>
	    /// <param name="toolbar"></param>
	    /// <param name="className"></param>
	    /// <param name="visible"></param>
	    public void AddSeparator(HtmlGenericControl toolbar, string className, bool visible)
		{
			var separator = new DsToolbarSeparator(className, visible);
			toolbar.Controls.Add(separator);
		}

	    /// <summary>
	    /// This is not implemented - demo only !
	    /// The intention was to create a zooming slider out of the map (in a toolbar)
	    /// </summary>
	    /// <param name="toolbar"></param>
	    /// <param name="className"></param>
	    /// <param name="visible"></param>
	    public void AddSlider(HtmlGenericControl toolbar, string className, bool visible)
		{
			var separator = new DsToolbarSeparator(className, visible);
			toolbar.Controls.Add(separator);
		}

	}

}
