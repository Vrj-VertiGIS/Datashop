<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="GeneralErrorPage.aspx.cs" Inherits="GEOCOM.GNSD.Web.GeneralErrorPage" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9050 An error has occured while processing your request.%>" />            
            </legend>
            <p style="margin-bottom:15px;">
                <%=GetLastError() %>
            </p>
            <p style="margin-bottom:15px;">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:9051 Please repeat your request and contact the support, if you still receive the error. %>" />
            </p>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9042 « Back%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>