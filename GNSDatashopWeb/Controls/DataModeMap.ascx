<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataModeMap.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.DataModeMap" %>
<%@ Import Namespace="GEOCOM.GNSD.Web.Core.Localization.Language" %>
<%--<script src="<%=ResolveUrl("~/js/DataShopPolygon.js") %>" type="text/javascript"></script>--%>
<script src="<%=ResolveUrl("~/js/DsToolbar.js") %>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/js/DsToolbarMap.js") %>" type="text/javascript"></script>
<script src="<%=ResolveUrl("~/js/DsPdeToolbar.js") %>" type="text/javascript"></script>
<script src="<%= ResolveUrl("~/js/MapInit.js") %>" type="text/javascript"></script>
<script src="<%= ResolveUrl("~/js/MapCommon.js") %>" type="text/javascript"></script>

<% if (!UseWebMap)
   { %>

<script type="text/javascript">
    //see MapCommon.js

    // additional for dsToolbar        
    dojo.require("esri.dijit.editing.Editor-all");
    dojo.require("esri.toolbars.draw");
   
    // this is our toolbar
    var tbPde = null;
    //set the default map extent
    var _initialMapExtent = <%= InitialMapExtent %>;
    //set the previous map extent
    var _previousMapExtent = <%= PreviousMapExtent %>;
    //show the slider
    var slider = <%= Slider %>;
    //NOTE: forcing scale limits on dynamic services
    var scales = [<%= LevelsOfDetail %>];
    var ratio = <%= ScaleResolutionRatio %>;
    var mapServiceLayer =  '<%= MapServiceLayer%>';
    var hfMapExtentsClientId = "<%= hfMapExtentsClientId %>";
    //NOTE: language specific text for out of bounds notification
    var notificationText = "<%= NotificationText %>";

    var geometryService;

    var maximumAllowedSelectableArea = <%= MaxSelectableArea %>;

    var maximumAllowedSelectableAreaBreachedErrorMessage =  "<asp:Literal ID="Literal5" runat="server" Text="<%$  Txt:3808 Your selection area of {0} exceeds the maximum allowable selected area of {1} %>" />";

    <%=TbCreator %>        

    function mapSpecificInit() {
        var layer = new esri.layers.<%=MapServiceLayer%>("<%=MapServiceURL%>");
        map.addLayer(layer);
        
        <%=TbPageLoad %>
        // initializes the toolbar when all page control are loaded
        dojo.connect(map, "onLoad", tbPde.MapLoad);
        
        geometryService = new esri.tasks.GeometryService("<%= GeometryServiceUrl %>");
        dojo.connect(geometryService, "onAreasAndLengthsComplete", outputAreaAndLength);
    } 
    
    function outputAreaAndLength(result) {
        var totalArea = 0;

        for(var i = 0;  i < result.areas.length; i++) {
            totalArea += parseFloat(result.areas[i]);
        }

        totalArea = Math.round(totalArea * 100) / 100; //NOTE: rounding to two decimal places the javascript way.

        if(totalArea > maximumAllowedSelectableArea) {
            var msg = maximumAllowedSelectableAreaBreachedErrorMessage.replace("{0}", totalArea).replace("{1}", maximumAllowedSelectableArea);

            alert(msg);
        }
        else {
            SubmitForm();
        }
    }

    function ValidateArea() {
        var geometries = [];
        
        if(map.graphics.graphics != null && map.graphics.graphics.length > 0) {
            for (var i = 0; i < map.graphics.graphics.length; i++) {
                var dsPolygon = map.graphics.graphics[i].geometry;
                geometries.push(dsPolygon);
            }
            
            var areasAndLengthParams = new esri.tasks.AreasAndLengthsParameters();
            areasAndLengthParams.areaUnit = esri.tasks.GeometryService.UNIT_SQUARE_KILOMETERS;

            geometryService.simplify(geometries, function(simplifiedGeometries) {
              areasAndLengthParams.polygons = simplifiedGeometries;
              geometryService.areasAndLengths(areasAndLengthParams);
            });
        }
        else {
            SubmitForm();
        }
    }
    
    function SubmitForm() {
         var hiddenField = getPolygonInfosHidddenField();
            if (hiddenField == null) return;
            hiddenField.value = "";
            var polygons = tbPde.GetPolygonsJson();
            hiddenField.value = polygons;

        __doPostBack("ctl00$MainPanelContent$ctlRequestDetails$btnRequest", "");
    }
</script>

<% } %>

<div class="mapToolbar dijitToolbar">
    <div id="divDsToolbar" runat="server" dojotype="dijit.Toolbar">
    </div>
</div>
<div id="divMap" data-dojo-type="dijit/layout/ContentPane">
</div>