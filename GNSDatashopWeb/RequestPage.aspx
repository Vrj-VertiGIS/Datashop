<%@ Page Title="<%$ Txt:1000 GEONIS Datashop%>" Language="C#" MasterPageFile="~/DatashopWeb.Master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="RequestPage.aspx.cs" Inherits="GEOCOM.GNSD.Web.RequestPage" %>

<%@ Register TagPrefix="geocom" TagName="Morph" Src="Controls/MorphingControl.ascx" %>
<%@ Register Src="~/Controls/ExportTypeSelector.ascx" TagPrefix="geocom" TagName="ExportTypeSelector" %>

<%@ OutputCache Duration="1" Location="None" %>
<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link href="css/dsToolbar.css" rel="stylesheet" type="text/css" />
    <script src="js/calendars.js" type="text/javascript"></script>
    <script src="js/utils.js" type="text/javascript"></script>
    <script type="text/javascript">
        djConfig = { parseOnLoad: true, locale: "<%= Language %>" };
        dojo.require("dijit.form.DateTextBox");
        dojo.require("dojo.string");
        dojo.require("dijit.form.FilteringSelect");
        dojo.require("dojo.data.ItemFileReadStore");
        dojo.require("dojo.fx");

        function ToggleElementDisplayById(elemId, chbx) {
            var fadeArgs = { node: elemId };
            if (chbx.checked) {
                dojo.fx.wipeIn(fadeArgs).play();
            } else {
                dojo.fx.wipeOut(fadeArgs).play();
            }
        }

        function refreshOnButtonBack() { //the aim of the function is to ensure complete refresh after the browser back buttton is clicked
            var hiddenField = document.getElementById("refreshed");
            if (hiddenField.value == "no")
                hiddenField.value = "yes";
            else {
                hiddenField.value = "no";
                location.reload();
            }
        }
        dojo.addOnLoad(refreshOnButtonBack);
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="leftColumn">
        <asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
            <input type="hidden" id="refreshed" value="no" />
            <div id="divRequest" class="pdeRequest">
                <div>
                    <asp:HiddenField ID="hfMapExtents" runat="server" />
                    <geocom:MapSearch  runat="server" />
                    <geocom:DefaultMapSearch  runat="server" ID="ctlSearch" />
                    <geocom:CommonActAsSurrogate ID="ctlActAsSurrogate" runat="server" />
                    <geocom:Morph ControlPath="~/Controls/PlotLayout.ascx"  runat="server" ID="ctlPlotLayout" RequestPageMode="Plot" />
                    <geocom:Morph ControlPath="~/Controls/ExportProfileSelector.ascx" runat="server" ID="ctlExportProfileSelector" RequestPageMode="Data" />
                    <geocom:ExportTypeSelector runat="server" ID="ExportTypeSelector" />
                    <geocom:CommonRequestDetails ID="ctlRequestDetails" OnRequestButtonClicked="RequestButtonClicked" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <div id="rightColumn">
        <geocom:Morph ControlPath="~/Controls/PlotModeMap.ascx" runat="server" ID="PlotModeMap" RequestPageMode="Plot"  />
        <geocom:Morph ControlPath="~/Controls/DataModeMap.ascx" runat="server" ID="DataModeMap" RequestPageMode="Data"  />
    </div>
    <asp:PlaceHolder runat="server" ID="placeHolderPlotSection">
        <div class="pdePlotSection">
            <asp:Literal runat="server" Text="<%$ Txt:3923 Map types: %>" />
            <asp:Label ID="lblAvailablePlotSection" runat="server"></asp:Label>
        </div>
    </asp:PlaceHolder>
</asp:Content>
