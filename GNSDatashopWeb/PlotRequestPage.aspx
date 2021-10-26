<%@ Page Title="" Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true"
    CodeBehind="PlotRequestPage.aspx.cs" Inherits="GEOCOM.GNSD.Web.PlotRequestPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
    <fieldset>
        <p>
            <asp:Literal runat="server" Text="<%$ Txt:4100 The requested website is not available anymore. In order to avoid this message, update your browser’s bookmarks: add a new bookmark from the Datashop Start page and delete the old bookmark. %>" />
        </p>
        <p>
            <asp:Literal runat="server" Text="<%$ Txt:4101 To create new map request and to be forwarded to the new page, %>" />
            <asp:HyperLink runat="server" NavigateUrl="~/RequestPage.aspx"><asp:Literal runat="server" Text="<%$ Txt:4102 click please here.%>"/></asp:HyperLink>
        </p>
    </fieldset>
    </div>
</asp:Content>
