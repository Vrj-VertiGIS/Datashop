// this class handles all map events
// it communicates with the other components through the listeners (call-backs)
// The effective constructor is MapLoad(). It MUST be called upon successfull map load completion
var mapToolbar = function(map, divMap) {
    var host = this; // this is used to facilitate calling methods from within another method
    var tbNav = null; // map navigation toolbar - created by MapLoad()
    var tbDraw = null; // polygon drawig toolbar - created by MapLoad()
    var tbEdit = null; // polygon editing toolbar - created by MapLoad()
    var graphicType = 0; // the type of polygon that is currently being vreated
    var activeGra = null; // the active polygon (after creation or after manual selection)
    var moveMode = 0; // used by the edit toolbar - loaded by MapLoad()
    var resizeMode = 0; // used by the edit toolbar - loaded by MapLoad()
    var reshapeMode = 0; // used by the edit toolbar - loaded by MapLoad()
    var editMode = 0;  // holds either moveMode or resizeMode or reshapeMode
    var defaultSymbol = null; // grey filling and red outline - loaded by MapLoad()
    var highlightSymbol = null; // grey filling and green outline - loaded by MapLoad()

    this.OnEditEnabled = null; // event listener, call-back set by the client application
    this.OnEditDisabled = null; // event listener, call-back set by the client application
    this.OnExtentHistoryChanged = null; // event listener, call-back set by the client application
    this.OnGraphicLayerChanged = null; // event listener, call-back set by the client application

    // to be called everytime the navigation is deactivated
    this.EnableNavigation = function(mode) {
        tbDraw.deactivate();
        host.DisableEdit();
        tbNav.activate(mode);
        map.graphics.enableMouseEvents();
    }

    // resets the symbol of the active graphic, if anyy
    this.UnlightActiveGra = function() {
        if (activeGra)
            activeGra.setSymbol(defaultSymbol);
    }

    // to be called everytime the editor is enabled
    this.EnableEdit = function(gra, mode) {
        tbNav.deactivate();
        tbDraw.deactivate();
        host.UnlightActiveGra();
        activeGra = gra;
        activeGra.setSymbol(highlightSymbol);
        editMode = mode;
        tbEdit.activate(editMode, activeGra);
        map.disableDoubleClickZoom();
        map.disableShiftDoubleClickZoom();
        if (host.OnEditEnabled)
            host.OnEditEnabled();
    }

    // to be called everytime the editor is disabled
    this.DisableEdit = function() {
        tbEdit.deactivate();
        host.UnlightActiveGra();
        activeGra = null;
        editMode = 0;
        if (host.OnEditDisabled)
            host.OnEditDisabled();
    }

    // ************************************
    // client listeners, call-back methods
    // ************************************

    // listener to refresh the prev/next extent history buttons in the application toolbar
    this.ExtentHistoryChangeHandler = function() {
        if (host.OnExtentHistoryChanged) {
            host.OnExtentHistoryChanged(tbNav.isFirstExtent(), tbNav.isLastExtent());
        }
    }

    // the amount of graphics defined by the user exposed to the rest of the application
    this.GraphicCount = function() {
        return map.graphics.graphics.length;
    }

    // listener called each time a graphic is added or removed
    this.GraphicLayerChanged = function() {
        if (host.OnGraphicLayerChanged)
            host.OnGraphicLayerChanged(host.GraphicCount());
    }

    // map resizer, calleac time the div containing the map is resized
    var timResizeMap = null;
    this.ResizeMap = function() {
        clearTimeout(timResizeMap);
        timResizeMap = setTimeout(function() {
            map.resize();
            map.reposition();
        }, 500);
    }

    // this is the effective constructor.
    // please no other construction stuff outside of this
    // This is called upon map load
    this.MapLoad = function(map, OnEditEnabled, OnEditDisabled, OnExtentHistoryChanged, OnGraphicLayerChanged) {
        dojo.connect(divMap, 'onresize', host.ResizeMap);
        // disable standard map navigation
        //map.disableMapNavigation();
        //map.disableMapNavigation();
        map.disableClickRecenter();
        map.disableKeyboardNavigation();
        map.disablePan();
        map.disableRubberBandZoom();
        map.disableScrollWheelZoom();
        map.disableDoubleClickZoom();
        map.disableShiftDoubleClickZoom();
        // enable controlled map navigation
        tbNav = new esri.toolbars.Navigation(map);
        dojo.connect(tbNav, "onExtentHistoryChange", host.ExtentHistoryChangeHandler);
        // enable polygon drawing
        tbDraw = new esri.toolbars.Draw(map);
        dojo.connect(tbDraw, "onDrawEnd", this.CompletePolygon);
        // enable edit, move, rotate polygons
        tbEdit = new esri.toolbars.Edit(map);
        moveMode = esri.toolbars.Edit.MOVE;
        resizeMode = esri.toolbars.Edit.SCALE | esri.toolbars.Edit.ROTATE;
        reshapeMode = esri.toolbars.Edit.EDIT_VERTICES;
        defaultSymbol = new esri.symbol.SimpleFillSymbol().setOutline(new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([255, 0, 0]), 2));
        highlightSymbol = new esri.symbol.SimpleFillSymbol().setOutline(new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 255, 0]), 2));
        editMode = resizeMode;
        // connect map events
        dojo.connect(map, "onClick", this.lostClick); // when the user clicks anywhere on the map
        dojo.connect(map.graphics, "onClick", this.clickGraphic); // when the user clicks a graphic
        // connect client listeners
        host.OnEditEnabled = OnEditEnabled;
        host.OnEditDisabled = OnEditDisabled;
        host.OnExtentHistoryChanged = OnExtentHistoryChanged;
        host.OnGraphicLayerChanged = OnGraphicLayerChanged;
    }

    // ****************************
    // Response to client commands
    // ****************************

    this.ZoomToFullExtent = function() {
        tbNav.zoomToFullExtent();
    }

    this.StartZoomIn = function() {
        host.EnableNavigation(esri.toolbars.Navigation.ZOOM_IN);
    }

    this.StartZoomOut = function() {
        host.EnableNavigation(esri.toolbars.Navigation.ZOOM_OUT);
    }

    this.StartPan = function() {
        host.EnableNavigation(esri.toolbars.Navigation.PAN);
    }

    this.PrevExtent = function() {
        tbNav.zoomToPrevExtent();
    }

    this.NextExtent = function() {
        tbNav.zoomToNextExtent();
    }

    this.PolygonSelected = function() {
        return (null != activeGra);
    }

    // not used
    // start drawing a circle using the DRAW LINE function : click at the center and drag to the edge (a line is shown)
    this.CreateCircle = function() {
        tbNav.deactivate();
        tbEdit.deactivate();
        host.UnlightActiveGra();
        tbDraw.activate(esri.toolbars.Draw.LINE);
        graphicType = 1;
    }

    // start drawing a rectangle using the DRAW EXTENT command : click at the upper left corner and drag to lower right corner)
    this.CreateRectangle = function() {
        tbNav.deactivate();
        tbEdit.deactivate();
        host.UnlightActiveGra();
        tbDraw.activate(esri.toolbars.Draw.EXTENT);
        graphicType = 2;
    }

    // start drawing a polygon (click on the first point, click the second point and so on. Double-click to complete)
    this.CreatePolygon = function() {
        tbNav.deactivate();
        tbEdit.deactivate();
        host.UnlightActiveGra();
        tbDraw.activate(esri.toolbars.Draw.POLYGON);
        graphicType = 3;
    }

    // start drawing a free hand curve (mouse down and drag along. Double-click to complete)
    this.CreateFreeHand = function() {
        tbNav.deactivate();
        tbEdit.deactivate();
        host.UnlightActiveGra();
        tbDraw.activate(esri.toolbars.Draw.FREEHAND_POLYGON);
        graphicType = 4;
    }

    // this is the event handler for tbDraw.onDrawEnd. 
    this.CompletePolygon = function(geometry) {

        var extent = geometry.getExtent();

        var gra;
        switch (graphicType) {
            case 1: // circle
                var pt = new esri.geometry.Point(extent.xmax, extent.ymax, map.spatialReference);
                var x = extent.xmax - extent.xmin;
                var y = extent.ymax - extent.ymin;
                var r = Math.sqrt(x * x + y * y);
                var pt1 = geometry.getPoint(0, 0);
                var circle = host.CreateCircleGeometry(map, pt1, r);
                gra = new esri.Graphic(circle, tbDraw.fillSymbol);
                break;
            case 2: // rectangle
                var rectangle = host.CreateRectangleGeometry(map, extent);
                gra = new esri.Graphic(rectangle, highlightSymbol);
                break;
            case 3: // polygon
            case 4: // free hand
                gra = new esri.Graphic(geometry, highlightSymbol);
                break;
            default:
                break;
        }
        if (gra) {
            map.graphics.add(gra);
            host.EnableEdit(gra, moveMode);
            host.GraphicLayerChanged();
        }
        else {
            tbDraw.deactivate();
            host.DisableEdit();
        }
    }

    // a cheap way to build a circle. A better alternative would be to use four segments with bezier.
    this.CreateCircleGeometry = function(map, pt, radius) {
        var polygon = new esri.geometry.Polygon();

        var points = [];
        for (var i = 0; i <= 360; i += 10) {
            var radian = i * (Math.PI / 180.0);
            var x = pt.x + radius * Math.cos(radian);
            var y = pt.y + radius * Math.sin(radian);
            points.push(new esri.geometry.Point(x, y));
        }

        polygon.addRing(points);
        polygon.spatialReference = map.spatialReference;

        return polygon;
    }

    this.CreateRectangleGeometry = function(map, extent) {
        var polygon = new esri.geometry.Polygon();

        var points = [];
        points.push(new esri.geometry.Point(extent.xmax, extent.ymax));
        points.push(new esri.geometry.Point(extent.xmax, extent.ymin));
        points.push(new esri.geometry.Point(extent.xmin, extent.ymin));
        points.push(new esri.geometry.Point(extent.xmin, extent.ymax));
        points.push(new esri.geometry.Point(extent.xmax, extent.ymax));

        polygon.addRing(points);
        polygon.spatialReference = map.spatialReference;

        return polygon;
    }

    // start moving the selected graphic
    this.MoveGraphic = function() {
        if (!this.PolygonSelected()) return;
        editMode = moveMode;
        tbEdit.activate(editMode, activeGra);
    }

    // start resizing and rotating the selected graphic
    this.ResizeGraphic = function() {
        if (!this.PolygonSelected()) return;
        editMode = resizeMode;
        tbEdit.activate(editMode, activeGra);
    }

    // start reshaping the selected graphic
    this.ReshapeGraphic = function() {
        if (!this.PolygonSelected()) return;
        editMode = reshapeMode;
        tbEdit.activate(editMode, activeGra);
    }

    // to be clarified - does this make sense
    this.CenterGraphic = function() {
        if (!this.PolygonSelected()) return;
        try {
            var center = activeGra.geometry.getExtent().getCenter();
            map.centerAt(center);
        }
        catch (ex) {
            var s = ex;
        }
    }

    // tbd
    this.ZoomInGraphic = function() {
        if (!this.PolygonSelected()) return;
        try {
            var extentTarget = activeGra.geometry.getExtent();
            var extent = new esri.geometry.Extent();
            var deltax = (extentTarget.xmax - extentTarget.xmin) / 10;
            var deltay = (extentTarget.ymax - extentTarget.ymin) / 10;
            extent.xmax = extentTarget.xmax + deltax;
            extent.xmin = extentTarget.xmin - deltax;
            extent.ymax = extentTarget.ymax + deltay;
            extent.ymin = extentTarget.ymin - deltay;
            extent.spatialReference = map.spatialReference;
            map.setExtent(extent);
        }
        catch (ex) {
            var s = ex;
        }
    }

    // removes the currently selected graphic, if any (after prompting the user)
    this.RemoveOneGraphic = function() {
        if (!this.PolygonSelected()) return;
        map.graphics.remove(activeGra);
        host.DisableEdit();
        host.GraphicLayerChanged();
    }

    // removes all graphics (after prompting the user)
    this.RemoveAllGraphics = function() {
        map.graphics.clear();
        host.DisableEdit();
        host.GraphicLayerChanged();
    }

    // response to a click on the map
    // the isLostClick value is set by the clickGraphic event that comes first
    var isLostClick = true;
    this.lostClick = function(evt) {
        if (isLostClick)
            host.DisableEdit();
        isLostClick = true;
    }

    // response to a click on a graphic. When allowed, disables the isLostClick event handler
    this.clickGraphic = function(evt) {
        if (evt.graphic != activeGra) {
            host.EnableEdit(evt.graphic, moveMode);
            isLostClick = false;
        }
        else {
            if (editMode == 0) {
                host.EnableEdit(evt.graphic, moveMode);
                isLostClick = false;
            }
        }
        return false;
    }

}
