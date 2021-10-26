using GEOCOM.GNSD.Web.Core.Localization.Language;

namespace GEOCOM.GNSD.Web.Controls
{
    using System.Web.UI.HtmlControls;

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
        #region private variables
        private readonly string _jsCreator;
        private readonly string _jsPageLoad;
        #endregion

        /// <summary>
        /// The JsCreator property returns the javascript constructor of the DsPdeToolbar object
        /// </summary>
        public string JsCreator
        {
            get { return _jsCreator; }
        }

        public string JsPageLoad
        {
            get { return _jsPageLoad; }
        }

        /// <summary>
        /// This adds the requested buttons to the related toolbar(s)
        /// </summary>
        /// <param name="tbId">Id of the java script object</param>
        /// <param name="maxGraphic">The max amount of polygons a user may create (default = 1)</param>
        /// <param name="tbFind">A html generic control to contain the Search button</param>
        /// <param name="tb1">A html generic control to contain the map command buttons</param>
        /// <param name="tb2">A html generic control to contain the polygon command buttons</param>
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
        /// <param name="btnRectangleVisible">Show button add a rectangle</param>
        /// <param name="btnFreehandVisible">Show button add a free-hand polygon</param>
        public DsPdeToolbar(
            string tbId, 
            int maxGraphic, 
            HtmlGenericControl tbFind, 
            HtmlGenericControl tb1, 
            HtmlGenericControl tb2, 
            string btnOutDefaultClass, 
            string btnInDefaultClass, 
			string btnOutHoverClass, 
            string btnInHoverClass, 
            string btnOutPressedClass, 
            string btnInPressedClass,
            string btnOutSelectedClass, 
            string btnInSelectedClass, 
            string btnOutDisabledClass, 
            string btnInDisabledClass, 
            bool btnRectangleVisible, 
            bool btnPolygonVisible) 
            : base(
                tbId, 
                btnOutDefaultClass, 
                btnInDefaultClass, 
				btnOutHoverClass, 
                btnInHoverClass, 
                btnOutPressedClass, 
                btnInPressedClass,
                btnOutSelectedClass, 
                btnInSelectedClass, 
                btnOutDisabledClass, 
                btnInDisabledClass)
        {
            // optionaly add a search command
            if (tbFind != null)
            {
                AddButton(
                    tbFind, "cmdFind", "iconFind", "showDialog()", "Find an area on the map...", false, false, true);
            }

            // add the map navigation commands
            AddButton(tb1, "cmdZoomIn", "iconZoomIn", tbId + ".StartZoomIn()", WebLanguage.LoadStr(3930, "Zoom in"), false, false, true);
            if (tb1 != tb2)
            {
                AddSlider(tb1, "tbSlider", false);
            }

            AddButton(tb1, "cmdZoomOut", "iconZoomOut", tbId + ".StartZoomOut()", WebLanguage.LoadStr(3931, "Zoom out"), false, false, true);
            AddButton(tb1, "cmdPan", "iconPan", tbId + ".StartPan()", WebLanguage.LoadStr(3932, "Pan"), false, false, true);
            AddButton(tb1, "cmdPrevExtent", "iconPrev", tbId + ".PrevExtent()", WebLanguage.LoadStr(3939, "Previous map extent"), true, false, true);
            AddButton(tb1, "cmdNextExtent", "iconNext", tbId + ".NextExtent()", WebLanguage.LoadStr(3940, "Next map extent"), true, false, true);
            AddButton(tb1, "initialMapExtent", "restoreInitialMapExtentIcon", "ResetToDefaultExtent();", WebLanguage.LoadStr(3942, "Default map extent"), true, false, true);
            
            // add a separator if a single segmentz
            if (tb1 == tb2)
            {
                AddSeparator(tb1, "tbSeparator", true);
            }

            // add the create polygon commands
            AddButton(tb2, "cmdRectangle", "iconRectangle", tbId + ".StartRectangle()", WebLanguage.LoadStr(3936, "Add plot frame"), false, false, btnRectangleVisible);
            AddButton(tb2, "cmdPolygon", "iconPolygon", tbId + ".StartPolygon()", WebLanguage.LoadStr(3944, "Draw a polygon"), false, false, btnPolygonVisible);
            AddSeparator(tb2, "tbSeparator", false);
            
            // add the edit polygon commands
            AddButton(tb2, "cmdMove", "iconMove", tbId + ".StartMove()", WebLanguage.LoadStr(3938, "Move Plot Frame"), true, false, true);
            AddButton(tb2, "cmdResize", "iconResize", tbId + ".StartResize()", WebLanguage.LoadStr(3945, "Resize or rotate the selected polygon"), true, false, false);
            AddButton(tb2, "cmdReshape", "iconReshape", tbId + ".StartReshape()", WebLanguage.LoadStr(3946, "Reshape the selected polygon"), true, false, btnPolygonVisible);
            AddButton(tb2, "cmdCenter", "iconCenter", tbId + ".CenterPolygon()", WebLanguage.LoadStr(3934, "Place plot frame in center of map"), true, false, true);
            AddButton(tb2, "cmdZoomInPolygon", "iconZoomPolygon", tbId + ".ZoomInPolygon()", WebLanguage.LoadStr(3935, "Zoom map on plot frame(s)"), true, false, true);
            AddButton(tb2, "cmdRemove", "iconRemove", tbId + ".RemoveOneGraphic(); NotifyOutOfBounds();", WebLanguage.LoadStr(3937, "Remove plot frame"), true, false, true);

            // generate the javascript constructor
            _jsCreator = string.Format(
                    "pdeToolbar.prototype = new dsToolbar('tbDefaultClass', 'tbDisabledClass', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');\n",
                    btnOutDefaultClass, 
                    btnInDefaultClass,
                    btnOutHoverClass, 
                    btnInHoverClass, 
                    btnOutPressedClass, 
                    btnInPressedClass,
                    btnOutSelectedClass, 
                    btnInSelectedClass, 
                    btnOutDisabledClass, 
                    btnInDisabledClass)
                + string.Format("{0} = new pdeToolbar();\n", tbId);

            _jsPageLoad = string.Format(
                "{0}.PageLoad('divMap', 'cmdZoomIn', 'cmdZoomOut', 'cmdPan', 'cmdPrevExtent', 'cmdNextExtent', " +
                "'cmdRectangle', 'cmdPolygon', 'cmdMove', 'cmdResize', 'cmdReshape', 'cmdCenter', 'cmdZoomInPolygon', 'cmdRemove', {1});\n", 
                tbId, 
                maxGraphic);
        }
    }
}
