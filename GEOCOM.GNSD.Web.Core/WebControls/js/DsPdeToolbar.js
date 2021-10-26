
// This is the main script object.
// A pdeToolbar object is a dsToolbar (see dsToolbar.js for more).
var pdeToolbar = function(tbDefaultClass, tbDisabledClass, btnOutDefaultClass, btnInDefaultClass,
				btnOutHoverClass, btnInHoverClass, btnOutPressedClass, btnInPressedClass,
				btnOutSelectedClass, btnInSelectedClass, btnOutDisabledClass, btnInDisabledClass) {

    var host = this; // this is used to facilitate calling methods from within another method
    var tbMap = null; // a mapToolbar object, created in MapLoad() - loaded by MapLoad()
    var divMap = null; // the div containing the map - loaded by PageLoad()
    var cmdZoomIn = null; // the zoom_in button - loaded by PageLoad()
    var cmdZoomOut = null; // the zoom_out button - loaded by PageLoad()
    var cmdPan = null; // the pan button - loaded by PageLoad()
    var cmdPrevExtent = null; // the prev extent button - loaded by PageLoad()
    var cmdNextExtent = null; // the next extent button - loaded by PageLoad()
    var cmdRectangle = null; // the draw rectangle button - loaded by PageLoad()
    var cmdPolygon = null; // the draw polygon button - loaded by PageLoad()
    var cmdFreeHand = null; // the draw free-hand polygon button - loaded by PageLoad()
    var cmdMove = null; // the move polygon button - loaded by PageLoad()
    var cmdResize = null; // the resize polygon button - loaded by PageLoad()
    var cmdReshape = null; // the reshape polygon button - loaded by PageLoad()
    var cmdCenter = null; // the center polygon in map button - loaded by PageLoad()
    var cmdZoomInPolygon = null; // the zoom-into polygon button - loaded by PageLoad()
    var cmdRemove = null; // the remove this polygon button - loaded by PageLoad()
    var maxGraphic = 1; // the amount of graphics a user may create - loaded by PageLoad()

    var allSelectableButtons = null; // this array is used to deselect all selectable buttons when a new button is selected - loaded by PageLoad()

    // This must be called when the page is fully loaded
    this.PageLoad = function(divMapId, cmdZoomInId, cmdZoomOutId, cmdPanId, cmdPrevExtentid, cmdNextExtentId,
        cmdRectangleId, cmdPolygonId, cmdFreeHandId,
        cmdMoveId, cmdResizeId, cmdReshapeId, cmdCenterId, cmdZoomInPolygonId, cmdRemoveId, maxGra) {
        divMap = document.getElementById(divMapId);
        cmdZoomIn = document.getElementById(cmdZoomInId);
        cmdZoomOut = document.getElementById(cmdZoomOutId);
        cmdPan = document.getElementById(cmdPanId);
        cmdPrevExtent = document.getElementById(cmdPrevExtentid);
        cmdNextExtent = document.getElementById(cmdNextExtentId);
        cmdRectangle = document.getElementById(cmdRectangleId);
        cmdPolygon = document.getElementById(cmdPolygonId);
        cmdFreeHand = document.getElementById(cmdFreeHandId);
        cmdMove = document.getElementById(cmdMoveId);
        cmdResize = document.getElementById(cmdResizeId);
        cmdReshape = document.getElementById(cmdReshapeId);
        cmdCenter = document.getElementById(cmdCenterId);
        cmdZoomInPolygon = document.getElementById(cmdZoomInPolygonId);
        cmdRemove = document.getElementById(cmdRemoveId);
        maxGraphic = maxGra;

        allSelectableButtons = [cmdZoomIn, cmdZoomOut, cmdPan, cmdRectangle, cmdPolygon, cmdFreeHand, cmdMove, cmdResize, cmdReshape];
    }

    // this must be called when the map was loaded
    this.MapLoad = function(map) {
        tbMap = new mapToolbar(map, divMap);
        tbMap.MapLoad(map, host.EnableEdit, host.DisableEdit, host.ExtentHistoryChangeHandler, host.GraphicLayerChangeHandler);
        // wia 2011.7.08 same as RequestPage
        host.StartZoomIn();
        // to add the initial extent to the navigation history
        host.ZoomToFullExtent();
    }

    /*******************************************
    *  tbMap listeners
    *******************************************/

    // manages the prev/next extent buttons
    this.ExtentHistoryChangeHandler = function(isFirstExtent, isLastExtent) {
        if (isFirstExtent)
            host.DisableButton(cmdPrevExtent);
        else
            host.EnableButton(cmdPrevExtent);
        if (isLastExtent)
            host.DisableButton(cmdNextExtent);
        else
            host.EnableButton(cmdNextExtent);
    }

    // manages the create polygons command buttons depending on the amount of graphics
    this.GraphicLayerChangeHandler = function(graCount) {
        if (graCount >= maxGraphic) {
            host.DisableButton(cmdRectangle);
            host.DisableButton(cmdPolygon);
            host.DisableButton(cmdFreeHand);
        }
        else {
            host.EnableButton(cmdRectangle);
            host.EnableButton(cmdPolygon);
            host.EnableButton(cmdFreeHand);
        }
    }

    /*******************************************
    *  button helpers
    *******************************************/

    // unselects many buttons (passed as an array)
    this.UnselectTheseButtons = function(unselectedButtons) {
        for (var i in unselectedButtons)
            this.UnselectButton(unselectedButtons[i]);
    }
    // unselect an array of buttons and selects one
    this.SelectThisButton = function(selectedButton, unselectedButtons) {
        this.UnselectTheseButtons(unselectedButtons);
        this.SelectButton(selectedButton);
    }

    this.EnableEdit = function() {
        host.UnselectTheseButtons(allSelectableButtons);
        host.EnableButton(cmdMove);
        host.EnableButton(cmdResize);
        host.EnableButton(cmdReshape);
        host.EnableButton(cmdCenter);
        host.EnableButton(cmdZoomInPolygon);
        host.EnableButton(cmdRemove);
        host.SelectButton(cmdMove);
    }

    this.DisableEdit = function() {
        host.UnselectTheseButtons([cmdResize, cmdReshape]);
        host.DisableButton(cmdMove);
        host.DisableButton(cmdResize);
        host.DisableButton(cmdReshape);
        host.DisableButton(cmdCenter);
        host.DisableButton(cmdZoomInPolygon);
        host.DisableButton(cmdRemove);
    }


    /*******************************************
    *  button handlers
    *******************************************/

    this.StartZoomIn = function() {
        this.SelectThisButton(cmdZoomIn, allSelectableButtons);
        tbMap.StartZoomIn();
    }

    this.StartZoomOut = function() {
        this.SelectThisButton(cmdZoomOut, allSelectableButtons);
        tbMap.StartZoomOut();
    }

    this.StartPan = function() {
        this.SelectThisButton(cmdPan, allSelectableButtons);
        tbMap.StartPan();
    }

    this.PrevExtent = function() {
        this.UnselectTheseButtons(allSelectableButtons);
        tbMap.PrevExtent();
        host.StartPan();
    }

    this.NextExtent = function() {
        this.UnselectTheseButtons(allSelectableButtons);
        tbMap.NextExtent();
        host.StartPan();
    }

    this.StartRectangle = function() {
        if (tbMap.GraphicCount() >= maxGraphic) return;
        this.SelectThisButton(cmdRectangle, allSelectableButtons);
        this.DisableEdit();
        tbMap.CreateRectangle();
    }

    this.StartPolygon = function() {
        if (tbMap.GraphicCount() >= maxGraphic) return;
        this.SelectThisButton(cmdPolygon, allSelectableButtons);
        this.DisableEdit();
        tbMap.CreatePolygon();
    }

    this.StartFreeHand = function() {
        if (tbMap.GraphicCount() >= maxGraphic) return;
        this.SelectThisButton(cmdFreeHand, allSelectableButtons);
        this.DisableEdit();
        tbMap.CreateFreeHand();
    }

    this.StartMove = function() {
        if (!tbMap.PolygonSelected()) return;
        this.SelectThisButton(cmdMove, allSelectableButtons);
        tbMap.MoveGraphic();
    }

    this.StartResize = function() {
        if (!tbMap.PolygonSelected()) return;
        this.SelectThisButton(cmdResize, allSelectableButtons);
        tbMap.ResizeGraphic();
    }

    this.StartReshape = function() {
        if (!tbMap.PolygonSelected()) return;
        this.SelectThisButton(cmdReshape, allSelectableButtons);
        tbMap.ReshapeGraphic();
    }

    this.CenterPolygon = function() {
        if (!tbMap.PolygonSelected()) return;
        tbMap.CenterGraphic();
    }

    this.ZoomInPolygon = function() {
        if (!tbMap.PolygonSelected()) return;
        tbMap.ZoomInGraphic();
    }

    this.RemoveOneGraphic = function() {
        if (!tbMap.PolygonSelected()) return;
        if (!confirm("Do you really want to remove the selected polygon ?")) return;
        tbMap.RemoveOneGraphic();
        window.focus();
    }

    this.RemoveAllGraphics = function() {
        if (!confirm("Do you really want to remove all polygons ?")) return;
        tbmMap.RemoveAllGraphics();
    }

    /*
    *  helpers for the export request
    */

    // creates a json string which includes all rings of all polygons
    // we send all rings, but it seems that only ring[0] is used...
    this.GetPolygonsJson = function() {
        var polygonsJson = ""
        for (var i = 0; i < map.graphics.graphics.length; ++i) {
            var polygonGraphic = map.graphics.graphics[i];
            if (polygonsJson != "")
                polygonsJson += ",";
            var extent = polygonGraphic.geometry.getExtent();
            //var x = extent.xmax - extent.xmin;
            //var y = extent.ymax - extent.ymin;
                
                
            polygonsJson += dojo.toJson(polygonGraphic.geometry.rings);
//            polygonInfos[i] = '{ "X": ' + polygonCenter.x + ', "Y": ' + polygonCenter.y + ', "Scale": ' + scale + ', "Rotation": ' + rotation + ', "PlotTemplate": "' + plotTemplate + '" }';

        }
        return "[" + polygonsJson + "]";
    }

}
// the object instanciation code is generated by the server
