<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="LoginErrorPage.aspx.cs" Inherits="GEOCOM.GNSD.Web.LoginErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Txt:9060 Your account could not be verified.%>" />
            </legend>
            <p style="margin-bottom:15px;"> 
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9061 Please retry and if the error persists conntact our support at <a href='mailto:support@geocom.ch'>
                support@geocom.ch</a>.%>" />
            </p>
            <p>
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:9062 « Back%>" />
            </p>
        </fieldset>    
    </div>
</asp:Content>