<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlotModeMap.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.PlotModeMap" %>
<script type="text/javascript">
    var plotScale = getSelectedScale();
	var mapNotReady = '<asp:Literal runat="server" Text="<%$ Txt:3941 Map not ready. Operation cancelled. %>" />';
</script>
<script src="<%= ResolveUrl("~/js/polygonPlotter.js") %>" type="text/javascript"></script>
<script src="<%= ResolveUrl("~/js/DatashopPolygon.js") %>" type="text/javascript"></script>

<script type="text/javascript">
	var rotationSliderBehaviourId = '<%= RotationSliderBehaviourId%>';
	//the esri js api navigation toolbar
	var navToolbar;
    
	var hfMapExtentsClientId = "<%= hfMapExtentsClientId%>";
</script>

<div class="mapToolbar dijitToolbar">
	<div dojotype="dijit.Toolbar" data-dojo-type="dijit/Toolbar" id="mapDijitToolbar">
		<div dojotype="dijit.form.ToggleButton" id="zoomIn" iconclass="zoomInIcon" onclick="onZoomIn()">
		</div>
		<span id="zoomin_tt" dojotype="dijit.Tooltip" connectid="zoomIn" position="above">
			<asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:3930 Zoom in%>" />
		</span>
		<div dojotype="dijit.form.ToggleButton" id="zoomOut" iconclass="zoomOutIcon" onclick="onZoomOut()">
		</div>
		<span id="zoomout_tt" dojotype="dijit.Tooltip" connectid="zoomOut" position="above">
			<asp:Literal ID="Literal3" runat="server" Text="<%$  Txt:3931 Zoom out%>" />
		</span>
		<div dojotype="dijit.form.ToggleButton" id="pan" iconclass="panIcon" onclick="onPan()">
		</div>
		<span id="pan_tt" dojotype="dijit.Tooltip" connectid="pan" position="above">
			<asp:Literal ID="Literal4" runat="server" Text="<%$  Txt:3932 Pan%>" />
		</span>
		<div dojotype="dijit.form.Button" id="previousMapExtent" iconclass="previousMapExtentIcon"
			onclick="zoomToPrevExtent();">
		</div>
		<span id="previousMapExtent_tt" dojotype="dijit.Tooltip" connectid="previousMapExtent"
			position="above">
			<asp:Literal ID="Literal5" runat="server" Text="<%$  Txt:3939 Previous Map Extent%>" />
		</span>
		<div dojotype="dijit.form.Button" id="nextMapExtent" iconclass="nextMapExtentIcon"
			onclick="zoomToNextExtent();">
		</div>
		<span id="nextMapExtent_tt" dojotype="dijit.Tooltip" connectid="nextMapExtent" position="above">
			<asp:Literal ID="Literal12" runat="server" Text="<%$  Txt:3940 Next Map Extent%>" />
		</span>
		<div dojotype="dijit.form.Button" id="initialMapExtent" iconclass="restoreInitialMapExtentIcon"
			onclick="ResetToDefaultExtent();">
		</div>
		<span id="Span1" dojotype="dijit.Tooltip" connectid="initialMapExtent" position="above">
			<asp:Literal ID="Literal1" runat="server" Text="<%$  Txt:3942 Next Map Extent%>" />
		</span>
		<span dojotype="dijit.ToolbarSeparator" data-dojo-type="dijit/ToolbarSeparator" style="height: 30px;"></span>
		<div dojotype="dijit.form.ToggleButton" id="movePlotFrame" iconclass="movePlotFrameIconDisabled"
			disabled="disabled"
			onclick="onMovePlotFrame()">
		</div>
		<span id="movepolygons_tt" dojotype="dijit.Tooltip" connectid="movePlotFrame" position="above">
			<asp:Literal ID="Literal11" runat="server" Text="<%$  Txt:3938 Move Plot Frame %>" />
		</span>
		<div dojotype="dijit.form.Button" id="centerPlotFrame" iconclass="centerPlotFrameIconDisabled"
			disabled="disabled"
			onclick="onCenterPlotFrame();">
		</div>
		<span id="centerplotframe_tt" dojotype="dijit.Tooltip" connectid="centerPlotFrame"
			position="above">
			<asp:Literal ID="Literal13" runat="server" Text="<%$  Txt:3934 Place plot frame in center of map%>" />
		</span>
		<div dojotype="dijit.form.Button" id="zoomPlotFrame" iconclass="zoomPlotFrameIconDisabled"
			disabled="disabled"
			onclick="onZoomPlotFrame();">
		</div>
		<span id="zoomplotframe_tt" dojotype="dijit.Tooltip" connectid="zoomPlotFrame" position="above">
			<asp:Literal ID="Literal14" runat="server" Text="<%$  Txt:3935 Zoom map on plot frame(s)%>" />
		</span>
		<div dojotype="dijit.form.Button" id="addPlotFrame" iconclass="addPlotFrameIconDisabled"
			disabled="disabled"
			onclick="onAddNewPlotFrame();">
		</div>
		<span id="addplotframe_tt" dojotype="dijit.Tooltip" connectid="addPlotFrame" position="above">
			<asp:Literal ID="Literal15" runat="server" Text="<%$  Txt:3936 Add plot frame %> " />
		</span>
		<div dojotype="dijit.form.Button" id="removePlotFrame" iconclass="removePlotFrameIconDisabled"
			disabled="disabled"
			onclick="onRemovePlotFrame();NotifyOutOfBounds();">
		</div>
		<span id="removeplotframe_tt" dojotype="dijit.Tooltip" connectid="removePlotFrame"
			position="above">
			<asp:Literal ID="Literal16" runat="server" Text="<%$  Txt:3937 Remove plot frame %>" />
		</span>
	</div>
</div>

<script type="text/javascript">
	//see MapCommon.js

	//the previously selected plotExtents
	//var polygons;
	
	var _initialMapExtent;
	var _previousMapExtent;
	var slider;
	var scales;
	var ratio;
	var mapServiceLayer;
	var notificationText = "<%= NotificationText %>";
	
	require(["esri/geometry"], function(geometry) {
		//the initial extens map extent
		_initialMapExtent = <%= InitialMapExtent%>;
		//set the previous map extent
		_previousMapExtent = <%= PreviousMapExtent%>;
		//show the slider
		slider = <%= Slider%>;
	});
	
</script>


<script type="text/javascript">

    <%= WebMapConfiguration %>
	
	esriConfig.defaults.io.proxyUrl = "./Proxy/proxy.ashx";
	esriConfig.defaults.io.alwaysUseProxy = <%=UseProxy %>;

	require(["esri/arcgis/utils"], function createMap(arcgisUtils, retryCount) {
		if (retryCount == null)
			retryCount = 0;
		//load after dijit/Toolbar is initialized

		var mapDeferred = arcgisUtils.createMap(webmap, "divMap", {
			mapOptions: {
				slider: <%= Slider %>,
				extent:  _previousMapExtent || _initialMapExtent
			}
		});

	    function DetectAnyPolygons(){
	        for (i = 0; i < map.graphics.graphics.length; i++) {
	            var dsPolygon = map.graphics.graphics[i].geometry;
	            if (dsPolygon instanceof esri.geometry.Polygon) {
	                return true;
	            }
	        }

	        return false;
	    }

		mapDeferred.then(function(response) {
			// Here the webMap  is initialized
			map = response.map;
			require(["esri/toolbars/navigation"], function(Navigation) {
				navToolbar = new Navigation(map);
			});
            
			/*dojo.connect(map, 'onLoad', function() {
        		SaveMapExtent(map.extent);
        	});
			*/
			
			initScript();
			

			/*if (_previousMapExtent)
				InstantiateMap(_previousMapExtent);
			else
				InstantiateMap(_initialMapExtent);
			*/
			
			dojo.connect(window, 'resize', ResizeMap);

			if (map.loaded) {
				SaveMapExtent(map.extent);
			}
			else
			{
				map.on('load', function () {
					SaveMapExtent(map.extent);
				});
			}

			map.on('zoom-end', function (extent, zoomFactor, anchor, level) {
				SaveMapExtent(extent.extent);
				if(!DetectAnyPolygons())
				{
				    EnablePlotFrameDefinition();
				}
			});

			map.on('pan-end', function (extent, zoomFactor, anchor, level) {
			    SaveMapExtent(extent.extent);
			    if(!DetectAnyPolygons())
			    {
			        EnablePlotFrameDefinition();
			    }
			});

			map.on('extent-change', function (extent, zoomFactor, anchor, level) {
				NotifyPolygonsOutOfMapExtentBounds(extent.extent);
			});
			
		}, 
		 function (error) {
		 	if (retryCount < 10) {
		 		// try to reinvoke this function on a separate stack - this was the only solution vrj found
		 		// for the MS Edge problem (https://issuetracker02.eggits.net/browse/DATASHOP-492)
		 		window.setTimeout(function() {
		 			createMap(arcgisUtils, ++retryCount);
		 		}, 0);
		 	}else{
		 		console.error("Error loading map: ", error.message);
		 		console.error(error);
		 		mapDeferred.cancel();
		 	}
		 });
	});

	function mapSpecificInit() {
	}

</script>

<script src="<%= ResolveUrl("~/js/MapCommon.js") %>" type="text/javascript">
</script>

<div id="divMap" data-dojo-type="dijit/layout/ContentPane">
</div>
