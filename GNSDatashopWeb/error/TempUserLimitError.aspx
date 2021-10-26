<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="TempUserLimitError.aspx.cs" Inherits="GEOCOM.GNSD.Web.error.TempUserLimitError" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
               <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:9019 Too many requests%>" />
            </legend>
            <p style="margin-bottom:15px;">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </p>
            <p style="margin-bottom:15px;">
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9023 Please try again later, or register as businessuser.%>" />
            </p>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
                <asp:HyperLink ID="lnkReg" runat="server" NavigateUrl="~/RegisterBusinessUser.aspx" Text="<%$ Txt:9024 Register%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>