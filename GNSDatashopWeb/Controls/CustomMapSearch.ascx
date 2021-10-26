<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomMapSearch.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.CustomMapSearch" %>
<asp:Repeater runat="server" ID="rptrCustomSearch" OnItemDataBound="rptrCustomSearch_OnItemDataBound">
    <ItemTemplate>
        <div class="request_panel_search custom_search">
            <asp:PlaceHolder runat="server" ID="customMapSearchHolder"></asp:PlaceHolder>
        </div>
    </ItemTemplate>
</asp:Repeater>

