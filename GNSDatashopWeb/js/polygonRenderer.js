function drawPolygons() {
    for (var i in polygons) {
        var polygon = new esri.geometry.Polygon(polygons[i]);
        var simpleFillSymbol = new esri.symbol.SimpleFillSymbol(esri.symbol.SimpleFillSymbol.STYLE_SOLID, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([255, 0, 0]), 2), new dojo.Color([255, 255, 255, 0]));
        var polygonGraphic = new esri.Graphic(polygon, simpleFillSymbol);
        map.graphics.add(polygonGraphic);        
    }
    return polygons;    
}

function zoomToPolygons() {
    var extent;
    for (var i in polygons) {
        var polygon = new esri.geometry.Polygon(polygons[i]);
        if (extent == null) {
            extent = polygon.getExtent();
        } else {
            extent = extent.union(polygon.getExtent());
        }
    }

    if (extent != null) {
        map.setExtent(extent, true);
      }
}

function onZoomIn() {
    dijit.byId("zoomIn").setChecked(true);
    dijit.byId("zoomOut").setChecked(false);
    dijit.byId("pan").setChecked(false);
    navToolbar.activate(esri.toolbars.Navigation.ZOOM_IN);
}

function onZoomOut() {
    dijit.byId("zoomIn").setChecked(false);
    dijit.byId("zoomOut").setChecked(true);
    dijit.byId("pan").setChecked(false);
    navToolbar.activate(esri.toolbars.Navigation.ZOOM_OUT);
}

function onPan() {
    dijit.byId("zoomIn").setChecked(false);
    dijit.byId("zoomOut").setChecked(false);
    dijit.byId("pan").setChecked(true);
    navToolbar.activate(esri.toolbars.Navigation.PAN);
}