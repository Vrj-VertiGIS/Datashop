<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchControl.ascx.cs"
    Inherits="GEOCOM.GNSD.Web.Controls.SearchControl" %>

<script language="javascript" type="text/javascript">
    //This script is used to controll the show/hide functionality of the search form
    function Button1_onclick(parentID, image) {
      
        var id = "<%= CommonSearchControlClientId %>_" + ((parentID == "") ? "searchfields" : parentID + "_searchfields");
        if (document.getElementById(id).style.display == 'none') {
            document.getElementById(id).style.display = 'block';
            image.src = 'images/nav/minimize.png';
        }
        else {
            document.getElementById(id).style.display = 'none';
            image.src = 'images/nav/maximize.png';            
        }
    }

    function HandleEnterKey(id) {
        var id = "ctl00_MainPanelContent_" + id + "_UpdatePanel";
        var updatepanel = document.getElementById(id);
        if (updatepanel != null) {
            updatepanel.update();
        }
        return true;
    }
</script>

<div id="searchControl" class="request_panel_search">
    <div style="font-weight: bold">
        <asp:Label runat="server" ID="Title" Text="Titel"></asp:Label>
        <img id="ImageButton1" alt="" onclick=" return Button1_onclick('<%=ID%>',this);"
            src="images/nav/<%=IMAGENAME%>" style="vertical-align: middle;float:right;" /></div>
    <div id="searchfields" runat="server" style="margin-top: 10px;">
        <asp:UpdatePanel ID="UpdatePanelSearch" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="clear: both;"></div>
    </div>
</div>
