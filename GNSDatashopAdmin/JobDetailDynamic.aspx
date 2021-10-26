<%@ Page Title="" Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true" CodeBehind="JobDetailDynamic.aspx.cs" Inherits="GNSDatashopAdmin.JobDetailDynamic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="css/DsDynamicReport.css" rel="stylesheet" type="text/css" />
    <script src="js/DsDynamicReport.js" type="text/javascript"></script>

    <link rel="stylesheet" type="text/css" href="//js.arcgis.com/3.9/js/dojo/dijit/themes/claro/claro.css" /> 
    <link rel="stylesheet" type="text/css" href="//js.arcgis.com/3.9/js/esri/css/esri.css" /> 
    <script type="text/javascript">djConfig = { parseOnLoad: true }</script>
    <script type="text/javascript" src="//js.arcgis.com/3.9/"></script>
    <script type="text/javascript" src="./js/polygonRenderer.js"></script>

    <script type="text/javascript">
        dojo.require("dojo.string");
        dojo.require("dojo.NodeList-traverse");

        dojo.require("esri.map");
        dojo.require("esri.toolbars.navigation");
        dojo.require("esri.geometry");
        dojo.require("esri.symbol");

        dojo.require("dijit.form.Button");
        dojo.require("dijit.Toolbar");
        dojo.require("dijit.Tooltip");
        dojo.require("dojo.fx.Toggler");
        dojo.require("esri.arcgis.utils");

        <%= AdminWebMapConfiguration %>

        var map, navToolbar, polygons;
        
        function init() {
            switchToLastActiveTab();
        }

        function handleMap(map, divMap) {
             dojo.connect(divMap, 'onresize', ResizeMap);
             navToolbar = new esri.toolbars.Navigation(map);
             polygons = eval("<%= PolygonsJson %>");
             removeMapLogo();
        }

        function getJobExtent(spatialRef) { 
            var jobExtent = null;
            polygons = eval("<%= PolygonsJson %>");
            
            if (polygons.length > 0) {
                for (var poly in polygons) {
                    var polygon = new esri.geometry.Polygon(polygons[poly]);
                    var tempExtent = polygon.getExtent();
                    if (poly == 0) {
                        jobExtent = tempExtent;
                    } else {
                        jobExtent = jobExtent.union(tempExtent);
                    }
                }
            }
            
            jobExtent!= null && spatialRef.length > 0 ? jobExtent.spatialReference.wkid = spatialRef[0] : null;
            return jobExtent;
        }
        

        function onFormSubmit() {
            saveLastActiveTabId();
        }

        
        function switchToLastActiveTab() {
            var lastActiveTabId = '<%= Request["lastActiveTabId"] %>';
            if (lastActiveTabId != '' && dojo.byId(lastActiveTabId) != null) {
                var javascriptToEval = dojo.byId(lastActiveTabId).href;
                eval(javascriptToEval);
            }
        }

        function removeMapLogo() {
            var logo = dojo.query(".logo-sm")[0];
            // wia 2011-07-11
            if (logo)
                dojo.removeClass(logo, "logo-sm");
        }

        function ResizeRepositionMap() {

            // compute height of the map control to take up the whole div
            var divMap = document.getElementById("map");
            var divRightColumn = document.getElementById("rightColumn");
            var posMap = dojo.position(divMap);
            var posRigCol = dojo.position(divRightColumn);
            var mapHeight = posRigCol.h - (posMap.y - posRigCol.y) - 25; //the 25 is for padding of parent element of the map
            dojo.setStyle(divMap, "height", mapHeight + "px");
            divMap.style.width = "100%";
             
            var jobExtent = getJobExtent(webmap.item.ExtentReference);
            var mapDeferred = new esri.arcgis.utils.createMap(webmap, divMap, {
                mapOptions: {
                    slider: false,
                    extent: jobExtent.expand(1.2)
                }
             });
             mapDeferred.then(function(response) {
                 map = response.map;
                 handleMap(map, divMap);
                 drawPolygons();
             }, function(error) {
                 console.log(error);
             });
        }

        // map resizer, calleac time the div containing the map is resized
        var timResizeMap = null;
        function ResizeMap() {
            clearTimeout(timResizeMap);
            timResizeMap = setTimeout(function() {
                map.resize();
                map.reposition();
            }, 0);
        }

        function ToggleCollapse(elem) {
            var isOpen = dojo.attr(elem, "src") == "images/arrow_up.png";
            if (isOpen) {
                Collapse(elem);
            } else {
                Open(elem);
            }
        }

        function Collapse(elem) {
            dojo.attr(elem, "src", "images/arrow_down.png");
            SetOpenOrCollapse(elem, false);
        }

        function Open(elem) {
            dojo.attr(elem, "src", "images/arrow_up.png");
            SetOpenOrCollapse(elem, true);
        }

        function SetOpenOrCollapse(elem, isOpen) {
            var rowElem = dojo.query(elem).parents("tr")[0];
            var group = dojo.attr(rowElem, "group");
            var groupElems = dojo.query(rowElem).siblings("[group=" + group + "]");
            groupElems.forEach(function (groupElem, index) {
                // skip last element
//                if (index == groupElems.length - 1)
//                    return;
                if (isOpen) {
                    dojo.removeClass(groupElem, "hidden");
                } else {
                    dojo.addClass(groupElem, "hidden");
                }
            });
        }

        function CollapseAll() {
            dojo.query(".toggleCollapseImg").forEach(function (elem) {
                Collapse(elem);
            });
        }

        function OpenAll() {
            dojo.query(".toggleCollapseImg").forEach(function (elem) {
                Open(elem);
            });
        }

        dojo.addOnLoad(init);

    </script>

    <script type="text/javascript">
        function checkFileSelected() {
            if (!isUploadFileSelected()) {
                alert('Please select a file to be uploaded!');
                return false;
            }
            else {
                disableStatusElements();
                enableWaitingElement();
            }
        }

        //this was rewritten by wia to use clientId
        function isUploadFileSelected() {
            return document.getElementById(fuFileUploadId).value != '';
        }

        //this was rewritten by wia to use clientId and to prevent multiple calls to document.getElementById...
        function disableStatusElements() {
            var e = document.getElementById('imgUploadOk');
            if (e) e.style.display = 'none';
            e = document.getElementById('imgUploadFailed');
            if (e) e.style.display = 'none';
            e = document.getElementById(lblUploadStatusId);
            if (e) e.style.display = 'none';
        }

        function enableWaitingElement() {
            document.getElementById('imgWaiting').style.display = '';
        }

        function saveLastActiveTabId() {
            if(DSDRTabs.ActiveTab == null)
                return;

            dojo.byId("<%= lastActiveTabId.ClientID %>").value = DSDRTabs.ActiveTab.id;
        }

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <h3>Job Details</h3>
     <asp:Button ID="btnRefreshPage" Text="Refresh page" Class="jobDetail_actionbutton"
                    runat="server" OnClick="OnClickbtnRefreshPage" ToolTip="Refresh this page" style="float:right;" />
    <asp:HiddenField runat="server" ID="lastActiveTabId" />
 
    <div id="divDSDR" runat="server">
    </div>
    
    <asp:Label ID="jobError" Text="" EnableViewState="False" runat="server" />
    <asp:Label ID="LogError" Text="" runat="server" />
    <asp:Label ID="UserError" Text="" EnableViewState="False" runat="server" />
    <asp:Label ID="javaScript" Text="" runat="server" />
</asp:Content>
