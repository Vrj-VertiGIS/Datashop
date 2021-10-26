<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true"
    CodeBehind="BizUserLocked.aspx.cs" Inherits="GEOCOM.GNSD.Web.BizUserLocked" Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:9031 Registration has not yet been confirmed.%>" />
            </legend>
            <p style="margin-bottom:15px;">
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9030 Your registration has not yet been confirmed by an administrator. Once your registration is confirmed, you will be notified by email.%>" />
            </p>
            <p>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$Txt:3011 Back to home page %>" />
            </p>
        </fieldset>
    </div>
</asp:Content>
