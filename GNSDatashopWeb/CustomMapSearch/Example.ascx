<%--
This ASP.NET user control inherits directly from the System.Web.UI.UserControl and therefore is suitable mainly for JavaScript. 
If you need to execute some server side code (e.g. service or database calls):
1) create a class that inherits from System.Web.UI.UserControl 
2) make your custom map search user control to inherit from the class 
3) add the DLL with the class to the 'bin' folder of a DatashopWeb installation
User controls reference: http://msdn.microsoft.com/en-us/library/y6wb1a0e(v=vs.100).aspx

or alternatively use the <% embedded code blocks %> to write C# directly in the ASP.NET markup to execute server code.
http://msdn.microsoft.com/en-us/library/ms178135(v=vs.100).aspx

Tip1: You can add WCF web services references and configuration into the Datashop web.config.

Tip2: You can use the following code snippet to build an UpdatePanel to make asynchronous requests:
<asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        this will be called without refresh (MyEventHanlder is a method in the class that control inherits).  
        <asp:Button runat="server" OnClick="MyEventHandler" />
    </ContentTemplate>
</asp:UpdatePanel>

--%>

<%@ Control Language="C#" AutoEventWireup="true"  Inherits="System.Web.UI.UserControl" %> 
<script type="text/javascript">
function zoomToCoord() {
    var xminVal = parseInt(xmin.value);
    var yminVal = parseInt(ymin.value);
    var xmaxVal = parseInt(xmax.value);
    var ymaxVal = parseInt(ymax.value);
   
    // the function zoomToExtent zooms the map to x and y points
    zoomToExtent(xminVal, yminVal, xmaxVal, ymaxVal);
    // alternatively you can use the 'map' object and esri.map API
    console.log(map);
    // esri.map reference: http://help.arcgis.com/en/webapi/javascript/arcgis/jsapi/map.html

    // there is also dojo object available to manipulate DOM and do AJAX
    console.log(dojo);
    // dojo reference: http://dojotoolkit.org/reference-guide/1.6/dojo/
   
    // disable post on the button
    return false;
}

function showCurrentMapExtent() {
    var msg = "map extent is: xmin={0},  ymin={1}, xmax={2}, ymax={3}";
    msg = msg.replace("{0}", map.extent.xmin);
    msg = msg.replace("{1}", map.extent.ymin);
    msg = msg.replace("{2}", map.extent.xmax);
    msg = msg.replace("{3}", map.extent.ymax);

    // use the showMessage function to display Datashop-style messages
    showMessage(msg);
    
    // disable post on the button
    return false;
}
</script>
<style type="text/css">

    .custom_search {
    
    }

    .custom_search_blurb {
        line-height:25px;
    }

    .custom_search p label {
        width:40px;
        display:inline-block;
        margin-top:5px;
        margin-bottom:10px;
    }

    .custom_search p.controls {
        
        margin-top:10px;
        margin-bottom:10px;
    }

    .custom_search input {
        width:115px;
        margin-right:2px;
    }

    .custom_search button {
        width:160px;
    }

    .custom_search_subtitle {
        font-weight: bold;
        margin-top:10px;
        margin-bottom:10px;
    }

</style>
<p class="requestpage-subtitle">Example of the custom search:</p>
<p class="custom_search_blurb">This is server code in an embedded code block: <% = DateTime.Now.ToString() %></p> 

<p class="custom_search_subtitle">Enter coordinates:</p>

<p>
    <label for="xmin">xmin: </label><input type="text" id="xmin" value="450000"/>
    <label for="ymin">ymin: </label><input type="text" id="ymin" value="20000"/>
</p> 

<p>
    <label for="xmax">xmax: </label><input type="text" id="xmax" value="700000"/>
    <label for="ymax">ymax: </label><input type="text" id="ymax" value="350000"/>
</p> 

<p class="controls">
    <button onclick="return zoomToCoord();">zoom to</button>
    <button onclick="return showCurrentMapExtent();">show current map extent</button>
</p>