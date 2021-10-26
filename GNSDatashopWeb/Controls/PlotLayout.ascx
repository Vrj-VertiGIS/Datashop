<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlotLayout.ascx.cs"
    Inherits="GEOCOM.GNSD.Web.Controls.PlotLayout" %>
<%@ Import Namespace="GEOCOM.GNSD.Web.Core.Localization.Language" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register TagPrefix="geocom" TagName="HelpButton" Src="~/Controls/HelpButton.ascx" %>

<script type="text/javascript">
    function getPlotTemplateComboBox() {
        var comboBox = document.getElementById("<%= cboTemplate.ClientID %>");
        return comboBox;
    }

    function getSelectedPlotTemplate() {
        var comboBox = getPlotTemplateComboBox();
        var plotTemplate = comboBox[comboBox.selectedIndex].value;
        return plotTemplate;
    }

    function getScaleComboBox() {
        var comboBox = document.getElementById("<%= cboScale.ClientID %>");
        return comboBox;
    }

    function getSelectedScale() {
        var comboBox = getScaleComboBox();
        var scale = parseInt(comboBox[comboBox.selectedIndex].value);
        return scale;
    }



    function DisablePlotFrameDefinition() {
        var skipDisablingIfLastMapExtentReused = <%= UseLastMapExtents.ToString().ToLower() %> === true;
        

        if (skipDisablingIfLastMapExtentReused) return;

        // make the whole thing transparent
        dojo.query("#request_layout .requestpage-control")
            .forEach(
                function (node) {
                    dojo.setStyle(node, "opacity", 0.7);
                }
            );

        // hide the slider
        var slider = dojo.byId("request_angel_slider");
        dojo.setStyle(slider, "display", "none");

        // show the warning icon
        var warning = dojo.byId("request_panel_warning");
        dojo.setStyle(warning, "display", "");
        for (var i = 0; i < 100000; i++) {

        }
        //disable input and select - wait 300ms time to let all controls to render properly otherwise the disabling would not sometimes work 
        window.setTimeout(
            function () {
                dojo.query("#request_layout .requestpage-control input, #request_layout .requestpage-control select")
                    .forEach(
                        function (node) {
                            dojo.setAttr(node, "disabled", "disabled");
                            dojo.setStyle(node, "cursor", "not-allowed");
                        }
                    );
            }
         , 500);
    }

    function EnablePlotFrameDefinition() {

        // enable input and select
        dojo
            .query("#request_layout .requestpage-control input, #request_layout .requestpage-control select, #request_layout .requestpage-control")
            .forEach(
                function (node) {
                    dojo.removeAttr(node, "disabled");
                    dojo.setStyle(node, "cursor", "");

                }
            );

        // make the whole thing opaque
        dojo.query("#request_layout .requestpage-control")
            .forEach(
                function (node) {
                    dojo.setStyle(node, "opacity", 1);
                }
            );

        // show the slider
        var slider = dojo.byId("request_angel_slider");
        dojo.setStyle(slider, "display", "");
    
        // hide the warning icon
        var warning = dojo.byId("request_panel_warning");
        dojo.setStyle(warning, "display", "none");

    }

    dojo.ready(DisablePlotFrameDefinition);
</script>

<div id="request_layout" class="request_panel">
    <p class="requestpage-subtitle">
        <asp:Label ID="lbl1" runat="server" Text="<%$ Txt:3903 Plotextend definition:%>"
            Font-Bold="true" Style="display: inline-block; margin-bottom: 20px;" />
        <geocom:HelpButton ID="helpPlotMode" runat="server" CssClass="inlineHelpBtn" ImageSrc="~/images/master/help-icon_24x24.png" />
    </p>
    <div id="request_panel_warning" style="position: relative;height: 0;top: -29px;">
        <img src="images/requestpage/warning-icon-128x128.png" style="cursor:pointer ;height: 26px; width: 26px;" onclick="showMessage('<%=WebLanguage.LoadStr(39031, "The map has to be navigated by map controls or address search to the area of your intereset before placing any plot frames.")%>')" />
        <%--<asp:Label runat="server" Text="<%$ Txt:39031 Position map to the plots  %>" CssClass="requestpage-label"></asp:Label>--%>
    </div>
    <div id="divTemplate" class="requestpage-control">
        <asp:Label ID="lblTemplate" runat="server" Text="<%$ Txt:3904 Plot format:%>" CssClass="requestpage-label"></asp:Label><asp:DropDownList
            ID="cboTemplate" class="requestpage-combobox requiredElement" runat="server"
            ToolTip="<%$ Txt:3905 Select the plot format%>" onchange="changePageFormat(this);">
        </asp:DropDownList>
    </div>
    <div id="divScale" class="requestpage-control">
        <asp:Label ID="lblScale" runat="server" Text="<%$ Txt:3906 Map Scale%>" CssClass="requestpage-label"></asp:Label><asp:DropDownList
            ID="cboScale" class="requestpage-combobox requiredElement" runat="server" AutoPostBack="false"
            ToolTip="<%$ Txt:3907 Select a map scale%>" Width="100px">
        </asp:DropDownList>
    </div>
    <div id="divRotation" class="requestpage-control">
        <asp:Label ID="lblRotationAngle" runat="server" Text="<%$ Txt:3908 Rotation angle%>"
            CssClass="requestpage-custom-label"></asp:Label><asp:TextBox ID="sliderValue_BoundControl"
                runat="server" AutoPostBack="false" onchange="changeRotation(this);" CssClass="requestpage-custom-edit">0</asp:TextBox><div
                    id="request_angel_slider" style="padding-left: 123px;">
                    <cc2:SliderExtender ID="SliderExtender1" BehaviorID="rotationSlider" runat="server"
                        Maximum="180" Minimum="-180" BoundControlID="sliderValue_BoundControl" TargetControlID="rotationSliderValue"
                        RaiseChangeOnlyOnMouseUp="false" Length="200">
                    </cc2:SliderExtender>
                    <asp:TextBox ID="rotationSliderValue" runat="server" AutoPostBack="false" onchange="changeRotation(this);">0
                    </asp:TextBox>
                </div>
    </div>
</div>