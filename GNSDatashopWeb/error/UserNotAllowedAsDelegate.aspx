<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="UserNotAllowedAsDelegate.aspx.cs" Inherits="GEOCOM.GNSD.Web.UserNotAllowedAsDelegate" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9081 You do not have the necessary permisions to request an order for another user. Please log in as an administrator.%>" />
            </legend>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>