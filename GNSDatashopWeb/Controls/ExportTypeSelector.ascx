<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportTypeSelector.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.ExportTypeSelector" %>
<div class="request_panel" id="divProfileSelector" runat="server">
    <p class="requestpage-subtitle">
        <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:3300 DXF Export Options: %>" />
        <div class="requestpage-control">
            <asp:Label ID="lblDxfExport" runat="server" AssociatedControlID="chkDxfExport" CssClass="requestpage-custom-label"><asp:Literal runat="server" Text="<%$ Txt:3301 Create DXF export %>"/></asp:Label>
            <asp:CheckBox ID="chkDxfExport" runat="server" Checked="false" CssClass="requestpage-custom-edit" />
        </div>
    </p>
</div>