var webmap = {};
webmap.item = {
};

webmap.itemData = {
    "operationalLayers": [
	/*{
       "url": "http://localhost:6080/arcgis/services/GNSD_AV_WMS/MapServer/WMSServer",
        "visibility": true,
        "visibleLayers": ["0","1"],
        "opacity": 1,
        "title": "defaultWMS",
        "type": "WMS",
        "version": "1.3.0",
        "layers": [],
	    "spatialReferences": [21781],
	    "extent": [[627000, 215000], [634000, 220000]],
        "maxWidth": 2048,
        "maxHeight": 2048,
        "format": "png24",
		"minScale": 10000,
        "maxScale": 50.497176,
        "transparentBackground": true	
	
	},*/

	// {
	    // "url": "https://wms-bgdi.prod.bgdi.ch",
	    // "id": "gebaeude",
	    // "visibility": true,
	    // "visibleLayers": "ch.bfs.gebaeude_wohnungs_register",
	    // "opacity": 1,
	    // "title": "defaultWMS",
	    // "type": "WMS",
	    // "version": "1.3.0",
	    // "spatialReferences": [21781],
	    // "extent": [[627000, 215000], [634000, 220000]],
	    // "maxWidth": 1477,
	    // "maxHeight": 573,
	    // "format": "png",
	    // "minScale": 52500.591555,
	    // "maxScale": 50.497176,
	    // "transparentBackground": true
	// },

	// {
	    // "url": "https://wms-bgdi.prod.bgdi.ch",
	    // "id": "geologie",
	    // "visibility": true,
	    // "visibleLayers": "ch.swisstopo.geologie-geotechnik-gk500-lithologie_hauptgruppen",
	    // "opacity": 1,
	    // "title": "defaultWMS",
	    // "type": "WMS",
	    // "version": "1.3.0",
	    // "spatialReferences": [21781],
	    // "extent": [[627000, 215000], [634000, 220000]],
	    // "maxWidth": 1477,
	    // "maxHeight": 573,
	    // "format": "png",
	    // "minScale": 50000.591555,
	    // "maxScale": 10000.497176,
	    // "transparentBackground": true



	// }
	/*,
	
	{
        "url": "http://localhost:6080/arcgis/rest/services/GNSD_AVonly/MapServer",
        "visibility": true,
        "visibleLayers": [],
        "opacity": 1,
        "version": "1.3.0",
        "layers": [],
	    "spatialReferences": [21781],
	    "extent": [[627000, 215000], [634000, 220000]],
        "maxWidth": 2048,
        "maxHeight": 2048,
        "format": "png24",
		"minScale": 500000.591555,
        "maxScale": 2500.497176,
        "transparentBackground": true
		
		
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
          "extent": [[627000, 215000], [634000, 220000]],
          "maxWidth": 1477,
          "maxHeight": 573,
          "format": "png",
          "transparentBackground": true
      }],
        "title": "defaultBasemap"
    },
    "version": "1.6"
};
