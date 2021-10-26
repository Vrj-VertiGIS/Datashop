 var webmap = {};
  webmap.item = {
		"ExtentReference": [21781]
  }; 
  
  webmap.itemData = {
      "operationalLayers": [
	  /*{
          "id": "MapServer",
          "opacity": 1,
          "visibility": true,
          "url": "http://localhost:6080/arcgis/rest/services/GNSD_All/MapServer"
      }*/
	  ],
      "baseMap": {
          "baseMapLayers": [
 	  {
          "url": "https://wms-bgdi.prod.bgdi.ch",
          "id": "landsat25",
          "visibility": true,
          "visibleLayers": "ch.swisstopo.images-landsat25",
          "opacity": 0.573,
          "title": "defaultWMS",
          "type": "WMS",
          "version": "1.3.0",
          "spatialReferences": [21781],
          "extent": [[627000, 215000], [627000, 215000]],
          "maxWidth": 1477,
          "maxHeight": 573,
          "format": "png",
          "transparentBackground": true
      }],
          "title": "defaultBasemap"
      },
      "version": "1.6"
  };