<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="ServiceNotFound.aspx.cs" Inherits="GEOCOM.GNSD.Web.ServiceNotFound" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
     <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9070 The service is currently unavailable.%>" />
            </legend>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>
