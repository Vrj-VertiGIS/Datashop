//To enable the user to change the plot format and scale on all polygons added to the map,
//you must comment out the combobox disabling methods:
//  getPlotTemplateComboBox().disabled = true;
//  getScaleComboBox().disabled = true;
//These two lines are found in the method named "addNewPlotFrame"

var divMapId = "divMap";

//the active polygon on the map
var polygonGraphic = null;
//the angle of the active polygon on the map
var polygonAngle = 0;
//the center of the active polygon on the map
var polygonCenter = null;

//symbology for the polygons
var simpleFillSymbol;
var simpleFillSymbolActivePolygon;
var simpleTextFont;

//the initial index of the scale combobox
//we need this so that we can reset the value when the user clicks the cancel button
var initialScaleCboIndex = 0;

//used during the dragging of the polygon
var startPoint = null;

var isMovePlotFrameToolActive = false;

//mouse events
var mouseDragConnection = null;
var mouseDownConnection = null;
var mouseUpConnection = null;
var mouseOverGraphicConnection = null;
var mouseOutGraphicConnection = null;

var frameNumber = 1;

function initScript() {
    simpleFillSymbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([255, 0, 0]), 2), new dojo.Color([255, 230, 230, 0.8]));
    simpleFillSymbolActivePolygon = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 255, 0]), 2), new dojo.Color([230, 255, 230, 0.8]));
    simpleTextFont = new esri.symbol.Font("14pt", esri.symbol.Font.STYLE_NORMAL, esri.symbol.Font.VARIANT_NORMAL, esri.symbol.Font.WEIGHT_NORMAL, "Arial");

    var scaleCbo = getScaleComboBox();
    initialScaleCboIndex = scaleCbo.selectedIndex;

    if (map.loaded)
        onMapLoaded();
    else
        map.on("load", onMapLoaded);
    map.on("extent-change", extentChange);
    updatePlotFrameButtons();
}

function onMapLoaded(obj) {
    clearPlots(); // remove trash that is normally in the map
}

//fired when the user changes the plotting format
function changePageFormat(obj) {
    var plotTemplate = obj[obj.selectedIndex].value;
    if (polygonGraphic != null) {
        removeDatashopPolygon(polygonGraphic);
        polygonGraphic = null;
    }
    addPlotFrame(plotTemplate);
    updatePlotFrameButtons();
    onMovePlotFrame();
}

//fired when the user changes the rotation
function changeRotation(obj) {
    polygonAngle = parseFloat(obj.value);
    if (polygonGraphic != null) {
        var origin = polygonGraphic.geometry.getCenterPoint();
        map.graphics.remove(polygonGraphic);
        polygonGraphic.geometry.rotate(polygonAngle, origin);
        map.graphics.add(polygonGraphic);
    }
}

//fired when the user changes the scale
function changeScale() {

    plotScale = getSelectedScale();
    var plotTemplate = getSelectedPlotTemplate();

    if (polygonGraphic != null && plotTemplate == "") {
        removeDatashopPolygon(polygonGraphic);

        polygonGraphic.geometry.scale(plotScale);
        map.graphics.add(polygonGraphic);
        updateActivePolygonText(polygonGraphic.geometry);
        onMovePlotFrame();
    }
    else if (polygonGraphic != null) {
        removeDatashopPolygon(polygonGraphic);
        polygonGraphic = null;
        onRemovePlotFrame();
        addPlotFrame(plotTemplate);
        updatePlotFrameButtons();
        //onMovePlotFrame();
    }
}

function onZoomIn() {
    if (map.graphics == null) return;
    dijit.byId("zoomIn").setChecked(true);
    dijit.byId("zoomOut").setChecked(false);
    dijit.byId("pan").setChecked(false);
    dijit.byId("movePlotFrame").setChecked(false);
    deactivatePolygonEdit();
    navToolbar.activate(esri.toolbars.Navigation.ZOOM_IN);
}

function onZoomOut() {
    if (map.graphics == null) return;
    dijit.byId("zoomOut").setChecked(true);
    dijit.byId("zoomIn").setChecked(false);
    dijit.byId("pan").setChecked(false);
    dijit.byId("movePlotFrame").setChecked(false);
    deactivatePolygonEdit();
    navToolbar.activate(esri.toolbars.Navigation.ZOOM_OUT);
}

function onPan() {
    if (map.graphics == null) return;
    dijit.byId("pan").setChecked(true);
    dijit.byId("zoomIn").setChecked(false);
    dijit.byId("zoomOut").setChecked(false);
    dijit.byId("movePlotFrame").setChecked(false);
    deactivatePolygonEdit();
    navToolbar.activate(esri.toolbars.Navigation.PAN);
}

function onMovePlotFrame() {
    dijit.byId("movePlotFrame").setChecked(true);
    dijit.byId("zoomIn").setChecked(false);
    dijit.byId("zoomOut").setChecked(false);
    dijit.byId("pan").setChecked(false);
    activatePolygonEdit();
}

//fired when the user clicks on the center plot frame button
//moves the active polygon to the center of the map
function onCenterPlotFrame() {
    if (polygonGraphic != null) {
        var mapDiv = document.getElementById(divMapId);
        var centerPixel = getDivCenter(mapDiv);
        var centerMap = map.toMap(centerPixel);

        removeDatashopPolygon(polygonGraphic);
        polygonGraphic.geometry.centerAt(centerMap);
        map.graphics.add(polygonGraphic);
        updateActivePolygonText(polygonGraphic.geometry);
    }
    onMovePlotFrame();
}

//fired when the user clicks on the zoom plot frame button
//centers the map at the center of the active polygon
function onZoomPlotFrame() {
    if (map.graphics.graphics.length > 0) {
        var extentOfAllPlotFrames = getExtentOfAllPlotFrames();
        var extentOfCurrentPlotFrame = getExtentOfCurrentPlotFrame();
        var deltaX = (extentOfCurrentPlotFrame.xmax - extentOfCurrentPlotFrame.xmin) / 2;
        var deltaY = (extentOfCurrentPlotFrame.ymax - extentOfCurrentPlotFrame.ymin) / 2;
        zoomToExtent(extentOfAllPlotFrames.xmin - deltaX,
                     extentOfAllPlotFrames.ymin - deltaY,
                     extentOfAllPlotFrames.xmax + deltaX,
                     extentOfAllPlotFrames.ymax + deltaY);
    }
    onMovePlotFrame();
}

//fired when the user clicks on the add new plot frame button
//adds a new polygon to the map. This new polygon becomes the active polygon
function onAddNewPlotFrame() {

    if (map == null) {
        alert(mapNotReady);
        return;
    }

    if (map.graphics == null) {
        alert(mapNotReady);
        return;
    }
    var plotTemplateCbo = getPlotTemplateComboBox();
    var plotTemplate = plotTemplateCbo[plotTemplateCbo.selectedIndex].value;
    getPlotTemplateComboBox().disabled = true;
    getScaleComboBox().disabled = true;

    addPlotFrame(plotTemplate, true);

    updatePlotFrameButtons();
    onMovePlotFrame();
}


//fired when the user clicks on the remove plot frame button
//removes the map's active polygon
function onRemovePlotFrame() {
    if (polygonGraphic != null) {
        removeDatashopPolygon(polygonGraphic);
        if (map.graphics.graphics.length > 0) {
            var polygonCount = 0;
            for (var i = 0; i < map.graphics.graphics.length; i++) {
                if (map.graphics.graphics[i].geometry instanceof DatashopPolygon) {
                    polygonGraphic = map.graphics.graphics[i];
                    polygonCenter = polygonGraphic.geometry.getCenterPoint();
                    setActiveSymbology(polygonGraphic);
                    polygonCount++;
                    break;
                }
            }
            if (polygonCount == 1) {
                getPlotTemplateComboBox().disabled = false;
                getScaleComboBox().disabled = false;
                polygonAngle = polygonGraphic.geometry.rotationInDegrees;
            }
            recalcFrameNumbers();
        } else {
            clearPlots();
            onPan();
        }
    }
    isMaxPolygon = false;
    updatePlotFrameButtons();
}

//adds a polygon to the map with the passed template
//the added polygon becomes the map's active polygon
function addPlotFrame(plotTemplate, increment) {

    if (map == null) {
        alert(mapNotReady);
        return;
    }

    if (map.graphics == null) {
        alert(mapNotReady);
        return;
    }

    if (plotTemplate == "") {
        clearPlots();
        return;
    }



    var plotWidthMeters;
    var plotHeightMeters;

    // read plot definitions from array submitted by server
    for (var i in templateDimension) {
        var tInfo = templateDimension[i].split("|");
        if (tInfo[0] == plotTemplate) {
            var plotWidthCentimeter = tInfo[1];
            plotWidthMeters = convertCentimetersToMeters(plotWidthCentimeter);
            var plotHeightCentimeter = tInfo[2];
            plotHeightMeters = convertCentimetersToMeters(plotHeightCentimeter);

            var polygonLimit = tInfo[3];
            var polygonCount = getDatashopPolyCount();
            if (polygonCount < polygonLimit) {
                polygonCenter = null;
            } else {
                showMessage("Your limit of plots has been reached.");
                isMaxPolygon = true;
                return;
            }
        }
    }
    //get the center point of the map
    var mapDiv = document.getElementById(divMapId);
    var cenPixel = getDivCenter(mapDiv);
    var mapCenter = map.toMap(cenPixel);

    //get the polygon offset distances from the center point
    var offX = (plotWidthMeters) / 2;
    var offY = (plotHeightMeters) / 2;

    var polygonMap = new esri.geometry.Polygon();
    polygonMap.spatialReference = map.spatialReference;

    var anchorX = mapCenter.x - offX;
    var anchorY = mapCenter.y - offY;
    polygonMap.addRing([[anchorX, anchorY], [mapCenter.x - offX, mapCenter.y + offY], [mapCenter.x + offX, mapCenter.y + offY], [mapCenter.x + offX, mapCenter.y - offY], [mapCenter.x - offX, mapCenter.y - offY]]);

    var datashopPolygon = new DatashopPolygon(polygonMap);
    datashopPolygon.plotTemplate = plotTemplate;

    if (increment == true && getDatashopPolyCount() > 0) {
        frameNumber++;
    }

    datashopPolygon.id = frameNumber;

    datashopPolygon.centerAt(polygonCenter);

    //scale the polygon
    datashopPolygon.scale(plotScale);

    //rotate the polygon
    datashopPolygon.rotate(polygonAngle, datashopPolygon.getCenterPoint());

    // create text symbol
    var textSymbol = new esri.symbol.TextSymbol(datashopPolygon.id);
    textSymbol.setColor(new dojo.Color([0, 255, 0]));
    textSymbol.setAlign(esri.symbol.TextSymbol.ALIGN_MIDDLE);
    textSymbol.setFont(simpleTextFont);

    // create a text graphic for this datashop polygon
    datashopPolygon.textGraphic = new esri.Graphic(new esri.geometry.Point(0, 0, map.spatialReference), textSymbol);

    if (polygonGraphic != null && getDatashopPolyCount() > 0) {
        setDefaultSymbology(polygonGraphic);
    }
    polygonGraphic = new esri.Graphic(datashopPolygon, simpleFillSymbol);
    map.graphics.add(polygonGraphic);

    setActiveSymbology(polygonGraphic);

    // update the position of the text
    updateActivePolygonText(datashopPolygon);
}

// returns the center coordinates, in pixels, of the object/div
function getDivCenter(obj) {
    var pos = new esri.geometry.Point();
    pos.spatialReference = map.spatialReference;
    //    divHeight = obj.style.height;
    //    divWidth = obj.style.width;
    //    pos.y = parseInt(divHeight.substr(0, divHeight.length)) / 2;
    //    pos.x = parseInt(divWidth.substr(0, divWidth.length)) / 2;
    pos.y = obj.offsetHeight / 2;
    pos.x = obj.offsetWidth / 2;
    return pos;
}

function extentChange(ev) {
    if (map.graphics == null) return;
    var isFirst = navToolbar.isFirstExtent();
    var isLast = navToolbar.isLastExtent();
    var prevMapExtent = dijit.byId("previousMapExtent");
    var nextMapExtent = dijit.byId("nextMapExtent");
    prevMapExtent.attr("iconClass", isFirst ? "previousMapExtentIconDisabled" : "previousMapExtentIcon");
    prevMapExtent.attr("disabled", isFirst);
    nextMapExtent.attr("iconClass", isLast ? "nextMapExtentIconDisabled" : "nextMapExtentIcon");
    nextMapExtent.attr("disabled", isLast);
}

// wia check it first
function zoomToPrevExtent() {
    if (map.graphics)
        navToolbar.zoomToPrevExtent();
}

// wia check it first
function zoomToNextExtent() {
    if (map.graphics)
        navToolbar.zoomToNextExtent();
}



//fired when the user moves the mouse over a graphic, we change the cursor to crosshairs
//and add the events for mousedown, mouseup and mousedrag
function mouseOverGraphic(ev) {
    map.setMapCursor("crosshair");
}

//fired when the user moves the mouse out of a graphic, we change the cursor to the default
//and remove the events for mousedown, mouseup and mousedrag
function mouseOutGraphic(ev) {
    map.setMapCursor("default");

    //mouseDragConnection = null;
}

//fired when the user makes a mouseup, events are disconnected, and the startPoint
//of the dragging set to null
function mouseUp(ev) {
    startPoint = null;

    if (polygonGraphic != null) {
        updateActivePolygonText(polygonGraphic.geometry);
    }
}

//fired when the user makes a mousedown, change polygon symbology if necessary,
//and store the start point for polygon dragging
function mouseDown(evt) {
    var mp = evt.mapPoint;
    //check if the user clicked on a polygon
    var graphic = getPolygonGraphicByLocation(mp);
    if (graphic != null) {
        //set the existing polygon to default symbology, as it will no longer be the active polygon
        if (polygonGraphic != null) {
            setDefaultSymbology(polygonGraphic);
        }

        //set the clicked polygon as the active polygon, and update it's symbology
        polygonGraphic = graphic;
        setActiveSymbology(polygonGraphic);
        //store the active polygon's rotation
        polygonAngle = polygonGraphic.geometry.rotationInDegrees;
    }
    else {
        map.graphics.remove(polygonGraphic);
        polygonGraphic.geometry.centerAt(mp);
        map.graphics.add(polygonGraphic);
    }

    //store the start point for mouse dragging
    if (polygonGraphic != null) {
        startPoint = evt.mapPoint;
    }
}

//fired when the user drags the active polygon on the map
//moves the polygon across the map
function mouseDrag(evt) {
    if (startPoint != null) {
        var mp = evt.mapPoint;
        var deltaX = mp.x - startPoint.x;
        var deltaY = startPoint.y - mp.y;

        if (polygonGraphic != null) {
            removeDatashopPolygon(polygonGraphic);

            polygonGraphic.geometry.move(deltaX, deltaY);

            map.graphics.add(polygonGraphic);

            startPoint.x = mp.x;
            startPoint.y = mp.y;

            polygonCenter = polygonGraphic.geometry.getCenterPoint();
        }
    }
}

//clears all the polygons from the map and resets all member variables
function clearPlots() {
    map.graphics.clear();
    var plotCbo = getPlotTemplateComboBox();
    var scaleCbo = getScaleComboBox();
    plotCbo.disabled = false;
    scaleCbo.disabled = false;
    scaleCbo.selectedIndex = initialScaleCboIndex;
    plotCbo.selectedIndex = 0;

    polygonGraphic = null;
    polygonAngle = 0;
    startPoint = null;
    polygonCenter = null;
}

//creates a string which describes the map's current polygons to be sent to the server for plotting
function createPolygonInfos() {
    var hiddenfield = getPolygonInfosHidddenField();
    var polygonInfos = new Array();

    var index = 0;
    for (var i = 0; i < map.graphics.graphics.length; ++i) {
        var polygonGraphic = map.graphics.graphics[i];
        if (polygonGraphic.geometry instanceof DatashopPolygon) {
            var polygonCenter = polygonGraphic.geometry.getCenterPoint();
            var rotation = polygonGraphic.geometry.rotationInDegrees;

            var scale = polygonGraphic.geometry.plotScale;
            var plotTemplate = polygonGraphic.geometry.plotTemplate;
            var id = polygonGraphic.geometry.id;
            polygonInfos[index] = '{ "Id": ' + id + ', "X": ' + polygonCenter.x + ', "Y": ' + polygonCenter.y + ', "Scale": ' + scale + ', "Rotation": ' + rotation + ', "PlotTemplate": "' + plotTemplate + '" }';
            index++;
        }
    }

    hiddenfield.value = "[" + polygonInfos + "]";
    console.log(hiddenfield.value);
}

//what do you think this does?
function convertCentimetersToMeters(centimeters) {
    var meters = centimeters / 100;
    return meters;
}

function refreshMap() {
    map.panUp();
    map.panDown();
}

////zoom the map to the passed extent
//function zoomToExtent(xBottomLeft, yBottomLeft, xTopRight, yTopRight) {
//    var extent = new esri.geometry.Extent();
//    extent.xmax = xTopRight;
//    extent.xmin = xBottomLeft;
//    extent.ymax = yTopRight;
//    extent.ymin = yBottomLeft;
//      extent.spatialReference = map.spatialReference;
//    map.setExtent(extent, true);
//    setTimeout("zoomToCenter(" + (extent.getCenter().x + 0.5) + ", " + (extent.getCenter().y + 0.5) + ")", 1000);
//}

////zoom the map to the passed center
//function zoomToCenter(x, y) {
//    var centerPoint = new esri.geometry.Point(x, y);
//    map.centerAt(centerPoint);
//}

//looks in the graphics layer of the map for a polygon at the passed location
//if a polygon is found, it is returned.
function getPolygonGraphicByLocation(point) {
    for (i = map.graphics.graphics.length - 1; i >= 0; i--) {
        var polygonGraphic = map.graphics.graphics[i];
        if ((polygonGraphic.geometry instanceof DatashopPolygon) && polygonGraphic.geometry.contains(point)) {
            return polygonGraphic;
        }
    }
    return null;
}

//activates the map for polygon editing
//turns off the navigation tool bar
function activatePolygonEdit() {

    if (map == null) {
        alert(mapNotReady);
        return;
    }

    if (map.graphics == null) {
        alert(mapNotReady);
        return;
    }

    if (!isMovePlotFrameToolActive) {
        isMovePlotFrameToolActive = true;

        navToolbar.deactivate();
        map.graphics.enableMouseEvents();
        map.enableScrollWheelZoom();
        map.disablePan();

        mouseOverGraphicConnection = dojo.connect(map.graphics, "onMouseOver", mouseOverGraphic);
        mouseOutGraphicConnection = dojo.connect(map.graphics, "onMouseOut", mouseOutGraphic);
        // for IE 10 and IE 11 there are problems with the onMouseDrag event, there is workaround with the onMouseMove event
        if (getInternetExplorerVersion() > 9)
            mouseDownConnection = dojo.connect(map, "onMouseMove",  mouseDrag);
        else
            mouseDragConnection = dojo.connect(map, "onMouseDrag", mouseDrag);
        mouseDownConnection = dojo.connect(map, "onMouseDown", mouseDown);
        mouseUpConnection = dojo.connect(map, "onMouseUp", mouseUp);
    }
}

function getInternetExplorerVersion()
    // Returns the version of Internet Explorer or a -1
    // (indicating the use of another browser).
{
    var ua = navigator.userAgent;

    //http://stackoverflow.com/questions/21825157/internet-explorer-11-detection
    if (!!ua.match(/Trident.*rv[ :]*11\./))
        return 11;

    var edge = ua.match(/Edge\/(\d.)/);
    if (edge != null) {
        var edgeVersion = parseInt(edge[1]);
        return edgeVersion;
    }

    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

function deactivatePolygonEdit() {

    if (isMovePlotFrameToolActive) {
        isMovePlotFrameToolActive = false;

        dojo.disconnect(mouseOverGraphicConnection);
        mouseOverGraphicConnection = null;
        dojo.disconnect(mouseOutGraphicConnection);
        mouseOutGraphicConnection = null;
        dojo.disconnect(mouseDragConnection);
        mouseDragConnection = null;
        dojo.disconnect(mouseDownConnection);
        mouseDownConnection = null;
        dojo.disconnect(mouseUpConnection);
        mouseUpConnection = null;

        navToolbar.activate();
        map.graphics.disableMouseEvents();
    }
}

//set the passed graphic symbology to the "default" polygon symbology
function setDefaultSymbology(graphic) {
    map.graphics.remove(graphic);
    polygonGraphic.symbol = simpleFillSymbol;
    map.graphics.add(graphic);

    map.graphics.remove(graphic.geometry.textGraphic);
    graphic.geometry.textGraphic.symbol.setColor(new dojo.Color([255, 0, 0]));
    map.graphics.add(graphic.geometry.textGraphic);
}

//sets the passed graphic symbology to the "active" polygon symbology
function setActiveSymbology(graphic) {
    polygonGraphic.symbol = simpleFillSymbolActivePolygon;
    updateRotationSlider(graphic.geometry.rotationInDegrees);

    graphic.geometry.textGraphic.symbol.setColor(new dojo.Color([0, 255, 0]));
}

// fires map.graphics.remove(graphic);  map.graphics.add(graphic); in changeRotation
function updateRotationSlider(angleValue) {
    var rotationSlider = $find(rotationSliderBehaviourId);
    if (rotationSlider != null) {
        rotationSlider.set_Value(angleValue);
    }
}

var isMaxPolygon;
function updatePlotFrameButtons() {
    var isNoPolygon;


    if (map.graphics == null) {
        isNoPolygon = true;
        isMaxPolygon = false;
    }
    else {
        isNoPolygon = map.graphics.graphics.length == 0;
    }

    var templateCbox = getPlotTemplateComboBox();
    var cboNotSelected = true;
    if (templateCbox != null)
        cboNotSelected = getPlotTemplateComboBox().value == "";


    var disableBtns = cboNotSelected || isNoPolygon;
    var dijitAlreadyInitialized = dijit.byId("removePlotFrame") != null;
    if (!dijitAlreadyInitialized)
        return;

    dijit.byId("removePlotFrame").set("iconClass", disableBtns ? "removePlotFrameIconDisabled" : "removePlotFrameIcon");
    dijit.byId("removePlotFrame").set("disabled", disableBtns);
    dijit.byId("addPlotFrame").set("iconClass", isMaxPolygon || disableBtns ? "addPlotFrameIconDisabled" : "addPlotFrameIcon");
    dijit.byId("addPlotFrame").set("disabled", isMaxPolygon || disableBtns);
    dijit.byId("centerPlotFrame").set("iconClass", disableBtns ? "centerPlotFrameIconDisabled" : "centerPlotFrameIcon");
    dijit.byId("centerPlotFrame").set("disabled", disableBtns);
    dijit.byId("zoomPlotFrame").set("iconClass", disableBtns ? "zoomPlotFrameIconDisabled" : "zoomPlotFrameIcon");
    dijit.byId("zoomPlotFrame").set("disabled", disableBtns);
    dijit.byId("movePlotFrame").set("iconClass", disableBtns ? "movePlotFrameIconDisabled" : "movePlotFrameIcon");
    dijit.byId("movePlotFrame").set("disabled", disableBtns);
}

function getExtentOfAllPlotFrames() {
    var extent;

    for (i = 0; i < map.graphics.graphics.length; i++) {
        var dsPolygon = map.graphics.graphics[i].geometry;
        if (dsPolygon instanceof DatashopPolygon) {
            if (extent == null) {
                extent = dsPolygon.getExtent();
            }
            else {
                extent = extent.union(dsPolygon.getExtent());
            }
        }
    }
    return extent;
}



function getExtentOfCurrentPlotFrame() {
    var extent;

    if (polygonGraphic != null) {
        extent = polygonGraphic.geometry.getExtent();
    }
    return extent;
}

function updateActivePolygonText(datashopPolygon) {

    map.graphics.remove(datashopPolygon.textGraphic);

    var x = datashopPolygon.getCenterPoint().x;
    var y = datashopPolygon.getCenterPoint().y;

    datashopPolygon.textGraphic.geometry.update(x, y);
    map.graphics.add(datashopPolygon.textGraphic);
}

function removeDatashopPolygon(datashopGraphic) {
    map.graphics.remove(datashopGraphic);
    map.graphics.remove(datashopGraphic.geometry.textGraphic);
}

function recalcFrameNumbers() {

    var slots = new Array();


    for (var i = 0; i < map.graphics.graphics.length; i++) {
        datashopPolygon = map.graphics.graphics[i].geometry;
        if (datashopPolygon instanceof DatashopPolygon) {
            slots[datashopPolygon.id] = datashopPolygon;
        }
    }

    var number = 0;
    for (var i = 0; i < slots.length; i++) {
        if (slots[i] instanceof DatashopPolygon) {
            number++;
            slots[i].id = number;
            map.graphics.remove(slots[i].textGraphic);
            slots[i].textGraphic.symbol.setText(slots[i].id);
            map.graphics.add(slots[i].textGraphic);
        }
    }

    if (number > 0)
        frameNumber = number;
    else
        frameNumber = 1;
}

function getDatashopPolyCount() {
    var polygonCount = 0;
    for (var i = 0; i < map.graphics.graphics.length; i++) {
        if (map.graphics.graphics[i].geometry instanceof DatashopPolygon) {
            polygonCount++;
        }
    }
    return polygonCount;
}