//=================== IMPORTANT ===================
//This file only extracts common javascript from the user controls PlotModeMap.ascx and DataModeMap.ascx.
//It is result of the unification of Pde and Plot Request sites and needs futher refactoring.
//vrj
//=================================================


//the esri js api map
var map;
var lods = [];

require(["dojo/ready", "esri/map", "esri/layers/agstiled", "esri/toolbars/navigation", "esri/geometry", "esri/symbol",
		"dijit/form/Button", "dijit/Toolbar", "dijit/Tooltip", "esri/dijit/InfoWindow"], function (ready) {

			ready(function () {
				if (scales != null && scales.length > 0 && ratio > 0) {
					for (var i = 0; i < scales.length; i++) {
						lods.push({ "level": i, "resolution": scales[i] / ratio, "scale": scales[i] });
					}
				}

				if (_previousMapExtent)
					InstantiateMap(_previousMapExtent);
				else
					InstantiateMap(_initialMapExtent);

				dojo.connect(window, 'resize', ResizeMap);

				dojo.connect(map, 'onLoad', function () {
					SaveMapExtent(map.extent);
				});

				dojo.connect(map, 'zoom-end', function (extent, zoomFactor, anchor, level) {
					SaveMapExtent(extent);
				});

				dojo.connect(map, 'pan-end', function (extent, zoomFactor, anchor, level) {
					SaveMapExtent(extent);
				});

				dojo.connect(map, 'extent-change', function (extent, zoomFactor, anchor, level) {
					NotifyPolygonsOutOfMapExtentBounds(extent);
				});

				//dojo.connect(window, 'onresize', function () { map.resize(); });

				//perform initialization that is speficic to the Pde or Plot map
				if (mapSpecificInit) {
					mapSpecificInit();
				}
			});


			// This shouldn't be called for webMap
			function InstantiateMap(extent) {
				var mapType = mapServiceLayer;
				//if (mapType == "ArcGISDynamicMapServiceLayer" && lods.length > 0) {
				//    map = new esri.Map("divMap", { slider: slider, extent: extent, logo: false, lods: lods });
				//}
				//else {
				//    map = new esri.Map("divMap", { slider: slider, extent: extent, logo: false });
				//}
				//NOTE: for v3.5
				if (mapType == "ArcGISDynamicMapServiceLayer" && lods.length > 0) {
					map = new esri.Map("divMap", {
						isZoomSlider: slider,
						sliderOrientation: "horizontal",
						sliderPosition: "bottom-left",
						sliderStyle: "large",
						extent: extent,
						logo: false,
						lods: lods
					});
				}
				else {
					map = new esri.Map("divMap", {
						isZoomSlider: slider,
						sliderOrientation: "horizontal",
						sliderPosition: "bottom-left",
						sliderStyle: "large",
						extent: extent,
						logo: false
					});
				}
			}

		});

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

var timResizeMap = null;

function ResizeMap() {
	clearTimeout(timResizeMap);
	timResizeMap = setTimeout(function () {
		map.resize();
		map.reposition();
	}, 250);
}
