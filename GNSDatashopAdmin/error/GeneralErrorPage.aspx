<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true" CodeBehind="GeneralErrorPage.aspx.cs" Inherits="GNSDatashopAdmin.error.GeneralErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <p>
    <asp:Literal runat="server" Text="An error did occur." /></p>
    <div id="errormessage">
    <asp:DataList ID="errorlist" runat="Server">
   <ItemTemplate>
     <asp:Label Text='<%# ShowLastError() %>'></asp:Label>
   </ItemTemplate>
</asp:DataList></div>
<p>
    <asp:Literal runat="server" Text="Please retry and if the error persists conntact our support at <a href='mailto:support@geocom.ch'>
    support@geocom.ch</a>." /></p>
    
<p>
    <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/WelcomePage.aspx"><asp:Literal runat="server" Text="Back" /></asp:HyperLink>
</p>
</asp:Content>
