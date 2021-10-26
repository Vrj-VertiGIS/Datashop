//dojo.require("esri.geometry");

//dojo.addOnLoad(init_polygon);

//This DatashopPolygon extends the esri.geometry.Polygon allowing for
//rotating, scaling, centering, and moving the polygon.

var originalPolygon;
////// wia: do define those global variables only once (in the client aspx page).
//////var rotationInRadian = 0;
//////var rotationInDegrees = 0;
//////var plotScale = 0;
//////var plotTemplate;
var id;
var textSymbol;

require(["esri/geometry/Polygon"], function (esri_geometry_polygon) {
	DatashopPolygon.prototype = new esri_geometry_polygon;
	DatashopPolygon.prototype.constructor = DatashopPolygon;
	//Gets a new esri.geometry.Point at the center of this polygon
	DatashopPolygon.prototype.getCenterPoint = function () {

		var point = getPolygonCenterPoint(this);
		return point;
	};
	
	//Rotates this polygon about the passed origin by the passed angle.
	//The polygon is rotated by the passed angle from this polygon's original placement.
	//This means the rotation is not cumulative.
	DatashopPolygon.prototype.rotate = function (angle, origin) {


		this.rotationInDegrees = angle - this.rotationInDegrees;
		this.rotationInRadian = this.rotationInDegrees * (Math.PI / 180);

		rotatePolygon(this.rotationInRadian, origin, this);

		this.rotationInDegrees = angle;
		this.rotationInRadian = this.rotationInDegrees * (Math.PI / 180);

	}

	//Centers this polygon at the passed point.
	DatashopPolygon.prototype.centerAt = function (centerPoint) {

		if (centerPoint != null) {
			centerPolygon(centerPoint, this);
			var polygonCenter = this.getCenterPoint();
		}

	}

	//Scales this polygon by the passed scale factor.
	//The scaling is done from the original polygon dimensions.
	//This means the scaling is not cumulative.
	DatashopPolygon.prototype.scale = function (scale) {
		var polygon = copyPolygon(originalPolygon);
		var polygonCenter = this.getCenterPoint();
		centerPolygon(polygonCenter, polygon);

		//we can get the polygon width and height via the extent,
		//because we know the original polygon is not rotated and is rectangular.
		var extent = polygon.getExtent();
		var polygonWidth = Math.abs(extent.xmax - extent.xmin);
		var polygonHeight = Math.abs(extent.ymax - extent.ymin);

		var scaledWidth = polygonWidth * scale;
		var scaledHeight = polygonHeight * scale;
		var deltaX = scaledWidth / 2;
		var deltaY = scaledHeight / 2;

		for (i = 0; i < polygon.rings.length; i++) {
			var ring = polygon.rings[i];
			for (j = 0; j < ring.length; j++) {
				var point = polygon.getPoint(i, j);
				if (point.x > polygonCenter.x) {
					point.x = point.x + deltaX;
				} else {
					point.x = point.x - deltaX;
				}

				if (point.y > polygonCenter.y) {
					point.y = point.y + deltaY;
				} else {
					point.y = point.y - deltaY;
				}
				polygon.setPoint(i, j, point);
			}
		}

		//we need to rotate the polygon by the current rotation amount,
		//because the scaling was done on the original polygon which was not rotated.
		var origin = this.getCenterPoint(polygon);
		rotatePolygon(this.rotationInRadian, origin, polygon);
		this.rings = polygon.rings;
		this.plotScale = scale;

	}

	//Moves this polygon by the passed x and y amounts.
	DatashopPolygon.prototype.move = function (deltaX, deltaY) {
		for (i = 0; i < this.rings.length; i++) {
			var ring = this.rings[i];
			for (j = 0; j < ring.length; j++) {
				var mapPoint = this.getPoint(i, j);
				mapPoint.x = mapPoint.x + deltaX;
				mapPoint.y = mapPoint.y - deltaY;
				this.setPoint(i, j, mapPoint);
			}
		}
	};
});

function DatashopPolygon(json) {
	esri.geometry.Polygon.call(this, json);
	originalPolygon = new esri.geometry.Polygon(json);
	this.rotationInDegrees = 0;
	this.rotationInRadian = 0;
	this.plotScale = 0;
	this.plotTemplate = "";
	this.id = 0;
	this.textGraphic;
}


function getPolygonCenterPoint(polygon) {
	var minX;
	var minY;
	var maxX;
	var maxY;

	for (i = 0; i < polygon.rings.length; i++) {
		var ring = polygon.rings[i];
		for (j = 0; j < ring.length; j++) {
			var pt = polygon.getPoint(i, j);
			if (j == 0) {
				minX = pt.x;
				minY = pt.y;
				maxX = pt.x;
				maxY = pt.y;
			}
			var maxX = maxX < pt.x ? pt.x : maxX;
			var minX = minX > pt.x ? pt.x : minX;
			var maxY = maxY < pt.y ? pt.y : maxY;
			var minY = minY > pt.y ? pt.y : minY;
		}
	}

	var point = new esri.geometry.Point();
	point.spatialReference = map.spatialReference;
	point.x = minX + (Math.abs(minX - maxX)) / 2;
	point.y = minY + (Math.abs(minY - maxY)) / 2;
	return point;
}

function centerPolygon(centerPoint, polygon) {
	var polygonCenter = getPolygonCenterPoint(polygon);

	for (i = 0; i < polygon.rings.length; i++) {
		var ring = polygon.rings[i];
		for (j = 0; j < ring.length; j++) {
			var point = polygon.getPoint(i, j);
			var offsetX = polygonCenter.x - point.x;
			var offsetY = polygonCenter.y - point.y;
			point.x = centerPoint.x + offsetX;
			point.y = centerPoint.y + offsetY;
			polygon.setPoint(i, j, point);
		}
	}
}

function rotatePolygon(angle, origin, polygon) {
	if (angle != 0) {
		for (i = 0; i < polygon.rings.length; i++) {
			var ring = polygon.rings[i];
			for (j = 0; j < ring.length; j++) {
				var point = polygon.getPoint(i, j);
				point = rotatePoint(angle, point, origin);
				polygon.setPoint(i, j, point);
			}
		}
	}
}

function rotatePoint(angle, point, origin) {
	var x = origin.x + Math.cos(angle) * (point.x - origin.x) - Math.sin(angle) * (point.y - origin.y);
	var y = origin.y + Math.sin(angle) * (point.x - origin.x) + Math.cos(angle) * (point.y - origin.y);
	point.x = x;
	point.y = y;
	return point;
}

//creates a "new" copy of the passed polygon.

function copyPolygon(inPolygon) {
	var outPolygon = new esri.geometry.Polygon();
	outPolygon.spatialReference = map.spatialReference;

	for (i = 0; i < inPolygon.rings.length; i++) {
		var ring = inPolygon.rings[i];
		var outRing = new Array(ring.length);
		for (j = 0; j < ring.length; j++) {
			var inPoint = inPolygon.getPoint(i, j);
			var outPoint = new esri.geometry.Point();
			outPoint.spatialReference = map.spatialReference;
			outPoint.x = inPoint.x;
			outPoint.y = inPoint.y;
			outRing[j] = outPoint;
		}
		outPolygon.addRing(outRing);
	}
	return outPolygon;
}
