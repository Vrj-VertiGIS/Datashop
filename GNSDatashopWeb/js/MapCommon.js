//=================== IMPORTANT ===================
//This file only extracts common javascript from the user controls PlotModeMap.ascx and DataModeMap.ascx.
//It is result of the unification of Pde and Plot Request sites and needs futher refactoring.
//vrj
//=================================================


var timResizeMap = null;

function ResizeMap() {
	clearTimeout(timResizeMap);
	timResizeMap = setTimeout(function () {
		map.resize();
		map.reposition();
	}, 250);
}

function SaveMapExtent(extent) {

	//console.debug("SaveMapExtent");
	//alert(extent);

    var previousExtentsField = document.getElementById(hfMapExtentsClientId);
    previousExtentsField.value = dojo.toJson(extent);
}

function ResetToDefaultExtent() {
    map.setExtent(_initialMapExtent);
}

//zoom the map to the passed extent
function zoomToExtent(xmin, ymin, xmax, ymax) {
    var extent = new esri.geometry.Extent();
    extent.xmax = xmax;
    extent.xmin = xmin;
    extent.ymax = ymax;
    extent.ymin = ymin;
	extent.spatialReference = map.spatialReference;
    map.setExtent(extent, true);
    setTimeout("zoomToCenter(" + (extent.getCenter().x + 0.5) + ", " + (extent.getCenter().y + 0.5) + ")", 1000);
}

//zoom the map to the passed center
function zoomToCenter(x, y) {
	var centerPoint = new esri.geometry.Point(x, y);
	centerPoint.spatialReference = map.spatialReference;
    map.centerAt(centerPoint);
}

function NotifyOutOfBounds() {
    NotifyPolygonsOutOfMapExtentBounds(map.extent);
}

function NotifyPolygonsOutOfMapExtentBounds(extent) {

    var outofBoundCount = 0;

    for (i = 0; i < map.graphics.graphics.length; i++) {
        var dsPolygon = map.graphics.graphics[i].geometry;
        if (dsPolygon instanceof esri.geometry.Polygon) {
            if (DetectPolygonOutOfExtent(extent, dsPolygon)) {
                outofBoundCount++;
            }
        }
    }

    if (outofBoundCount > 0) {
        ShowPolygonOutOfExtentNotification(outofBoundCount);
    }
    else {
        HidePolygonOutOfExtentNotification();
    }
}

function DetectPolygonOutOfExtent(extent, polygon) {

    var polygonExtent = polygon.getExtent();

    return extent.xmax < polygonExtent.xmax || extent.xmin > polygonExtent.xmin || extent.ymax < polygonExtent.ymax || extent.ymin > polygonExtent.ymin;
}

function ShowPolygonOutOfExtentNotification(outofBoundCount) {

    var mapNode = document.getElementById("divMap");

    var notifyNode = document.getElementById("notification");

    if (!notifyNode) {
        notifyNode = document.createElement("div");
        notifyNode.setAttribute("id", "notification");
        mapNode.parentNode.appendChild(notifyNode);
    }

    notifyNode.innerHTML = "" + outofBoundCount + " " + notificationText;

    mapNode.setAttribute("style", "top:205px;");
}

function HidePolygonOutOfExtentNotification() {

    var notifyNode = document.getElementById("notification");

    if (notifyNode) {
        var mapNode = document.getElementById("divMap");
        mapNode.parentNode.removeChild(notifyNode);
        mapNode.removeAttribute("style", "top:180px;");
    }
}