using System.Web.UI.HtmlControls;

namespace GEOCOM.GNSD.Web.Core.WebControls
{
	/// <summary>
	/// This class is a helper that appends the Pde buttons and/or separators to a some given container(s).
	/// It does not render anything.
	/// IMPORTANT: this class MUST be instanciated at page load time or later.
	/// This class is a DsToolbar. See dsToolbar.cs for more.
	/// The client application provides this control with up to three containers
	///  - tbFind for the Search command (optional)
	///  - tb1 for the map navigation commands
	///  - tb2 for the polygon commands (can be same as tb1, thus one single segment)
	/// TbFind may be null (no find button)
	/// Tb1 and Tb2 can be the the same container (this means : one single segment)
	/// The JsCreator expects the controlId (javascript), the tbFind, tb1 and tb2 container controls and the 10 css class names corresponding to the 5 states of the in- and out-buttons (see DsToolbar)
	/// </summary>
	public class DsPdeToolbar : DsToolbar
	{
		/// <summary>
		/// the JsCreator property returns the javascript constructor of the DsPdeToolbar object
		/// </summary>
		private readonly string jsCreator;
		public string JsCreator {get { return jsCreator; }}

		private readonly string jsPageLoad;
		public string JsPageLoad {get { return jsPageLoad; }}

		/// <summary>
		/// Thisn add the requested buttons to the respectibe toolbar(s)
		/// </summary>
		/// <param name="tbId">the id of the java script object</param>
		/// <param name="maxGraphic">the max amount of polygons a user may create (default = 1)</param>
		/// <param name="tbFind">a html generic control to contain the Search button</param>
		/// <param name="tb1">a html generic control to contain the map command buttons</param>
		/// <param name="tb2">a html generic control to contain the polygon command buttons</param>
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
		public DsPdeToolbar(string tbId, int maxGraphic, HtmlGenericControl tbFind, HtmlGenericControl tb1, HtmlGenericControl tb2, 
				string btnOutDefaultClass, string btnInDefaultClass, 
				string btnOutHoverClass, string btnInHoverClass, string btnOutPressedClass, string btnInPressedClass,
				string btnOutSelectedClass, string btnInSelectedClass, string btnOutDisabledClass, string btnInDisabledClass) 
			: base(tbId, btnOutDefaultClass, btnInDefaultClass, 
				btnOutHoverClass, btnInHoverClass, btnOutPressedClass, btnInPressedClass,
				btnOutSelectedClass, btnInSelectedClass, btnOutDisabledClass, btnInDisabledClass)
		{
			// optionaly add a search command
			if (tbFind != null)
				AddButton(tbFind, "cmdFind", "iconFind", "showDialog()", "Find an area on the map...", false, false, true);

			// add the map navigation commands
			AddButton(tb1, "cmdZoomIn", "iconZoomIn", tbId + ".StartZoomIn()", "Zoom-in the map", false, false, true);
			if (tb1 != tb2)
				AddSlider(tb1, "tbSlider", false);
			AddButton(tb1, "cmdZoomOut", "iconZoomOut", tbId + ".StartZoomOut()", "Zoom-out the map", false, false, true);
			AddButton(tb1, "cmdPan", "iconPan", tbId + ".StartPan()", "Pan the map", false, false, true);
			AddButton(tb1, "cmdPrevExtent", "iconPrev", tbId + ".PrevExtent()", "Show the previous map extent", true, false, true);
			AddButton(tb1, "cmdNextExtent", "iconNext", tbId + ".NextExtent()", "Show the next map extent", true, false, true);
			
			// add a separator if a single segmentz
			if (tb1 == tb2)
				AddSeparator(tb1, "tbSeparator", true);

			// add the create polygon commands
			AddButton(tb2, "cmdRectangle", "iconRectangle", tbId + ".StartRectangle()", "Draw a rectangle...", false, false, false);
			AddButton(tb2, "cmdPolygon", "iconPolygon", tbId + ".StartPolygon()", "Draw a polygon...", false, false, true);
			AddButton(tb2, "cmdFreeHand", "iconFreeHand", tbId + ".StartFreeHand()", "Draw a free-hand polygon...", false, false, false);
			AddSeparator(tb2, "tbSeparator", false);
			// add the edit polygon commands
			AddButton(tb2, "cmdMove", "iconMove", tbId + ".StartMove()", "Move the selected polygon", true, false, true);
			AddButton(tb2, "cmdResize", "iconResize", tbId + ".StartResize()", "Resize or rotate the selected polygon", true, false, false);
			AddButton(tb2, "cmdReshape", "iconReshape", tbId + ".StartReshape()", "Reshape the selected polygon", true, false, true);
			AddButton(tb2, "cmdCenter", "iconCenter", tbId + ".CenterPolygon()", "Center the selected polygon in the map", true, false, true);
			AddButton(tb2, "cmdZoomInPolygon", "iconZoomPolygon", tbId + ".ZoomInPolygon()", "Zoom-in the selected polygon", true, false, true);
			AddButton(tb2, "cmdRemove", "iconRemove", tbId + ".RemoveOneGraphic()", "Remove the selected polygon", true, false, true);

			// generate the javascript constructor
			jsCreator = string.Format(
					"pdeToolbar.prototype = new dsToolbar('tbDefaultClass', 'tbDisabledClass', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');\n",
					btnOutDefaultClass, btnInDefaultClass,
					btnOutHoverClass, btnInHoverClass, btnOutPressedClass, btnInPressedClass,
					btnOutSelectedClass, btnInSelectedClass, btnOutDisabledClass, btnInDisabledClass)
				+ string.Format("{0} = new pdeToolbar();\n", tbId);

			jsPageLoad = string.Format("{0}.PageLoad('divMap', 'cmdZoomIn', 'cmdZoomOut', 'cmdPan', 'cmdPrevExtent', 'cmdNextExtent', "
				+ "'cmdRectangle', 'cmdPolygon', 'cmdFreeHand','cmdMove', 'cmdResize', 'cmdReshape', 'cmdCenter', 'cmdZoomInPolygon', 'cmdRemove', {1});\n", tbId, maxGraphic);

		}
	}
}
