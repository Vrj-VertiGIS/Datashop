<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="ProfileLoadFailure.aspx.cs" Inherits="GEOCOM.GNSD.Web.error.ProfileLoadFailure" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:9090 An error occured during the loading of the export profiles. Please try again in a few minutes or contact the support.%>" />
            </legend>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>