<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportProfileSelector.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.ExportProfileSelector" %>
<%@ Register TagPrefix="gnsduc" TagName="LabelAndDropDown" Src="~/Controls/LabelAndDropDown.ascx" %>
<%@ Register TagPrefix="geocom" TagName="HelpButton" Src="~/Controls/HelpButton.ascx"  %>

<div class="request_panel" id="divProfileSelector" runat="server">
    <asp:PlaceHolder runat="server" ID="placeHolderProfile">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div  class="requestpage-subtitle">
                    <asp:Label runat="server" Text="<%$ Txt:3801 Profile selection:%>" Font-Bold="true" />
                    <geocom:HelpButton ID="helpDataMode" runat="server" CssClass="profileHelp" />
                </div>
               <br />
               <div class="requestpage-control">
                     <gnsduc:LabelAndDropDown runat="server" ID="cboExportType" LabelText="<%$ Txt:3964 Export Type:%>"
                        LabelCssClass="requestpage-label" DropDownCssClass="requestpage-combobox" DropDownRequiredCssClass="requiredElement"
                        ValidationFailedText="<%$ Txt:3966 Select the export type%>" OnValidationFailed="ShowMessage"
                        ValidationGroup="request" AutoPostBack="true" OnSelectedIndexChanged="CboExportTypeSelectedIndexChanged">
                    </gnsduc:LabelAndDropDown>
                </div>
                <div class="requestpage-control">
                    <gnsduc:LabelAndDropDown runat="server" ID="cboDxfExport" LabelText="<%$ Txt:3965 Dxf Profile:%>"
                        LabelCssClass="requestpage-label" DropDownCssClass="requestpage-combobox" DropDownRequiredCssClass="requiredElement"
                        OnSelectedIndexChanged="CboDxfProfileSelectedIndexChanged" AutoPostBack="true"
                        ValidationFailedText="<%$ Txt:3803 Select the export profile%>" OnValidationFailed="ShowMessage"
                        ValidationGroup="request"  />
                </div>
                <div class="requestpage-control">
                    <gnsduc:LabelAndDropDown runat="server" ID="cboProfile" LabelText="<%$ Txt:3802 Profile:%>"
                        LabelCssClass="requestpage-label" DropDownCssClass="requestpage-combobox" DropDownRequiredCssClass="requiredElement"
                        ValidationFailedText="<%$ Txt:3803 Select the export profile%>" OnValidationFailed="ShowMessage"
                        ValidationGroup="request" AutoPostBack="true" OnSelectedIndexChanged="CboProfileSelectedIndexChanged"  />
                    <asp:Label runat="server" ID="lblProfileDetail" class="requestpage-description" Visible="false">
                    </asp:Label>
                </div>
                <div class="requestpage-control">
                    <gnsduc:LabelAndDropDown runat="server" ID="cboDataFormat" LabelText="<%$ Txt:3805 Data format:%>"
                        LabelCssClass="requestpage-label" DropDownCssClass="requestpage-combobox" DropDownRequiredCssClass="requiredElement"
                        ValidationFailedText="<%$ Txt:3806 Select the data format%>" OnValidationFailed="ShowMessage"
                        ValidationGroup="request"  />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:PlaceHolder>
</div>