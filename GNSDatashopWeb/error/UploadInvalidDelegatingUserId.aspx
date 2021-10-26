<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="UploadInvalidDelegatingUserId.aspx.cs" Inherits="GEOCOM.GNSD.Web.error.UploadInvalidDelegatingUserId" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9080 The identification of the user, for which you want to submit this request, is invalid.%>" />
            </legend>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>