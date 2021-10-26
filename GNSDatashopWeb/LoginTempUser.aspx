<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginTempUser.aspx.cs"
    Inherits="GEOCOM.GNSD.Web.LoginTempUser" MasterPageFile="~/DatashopWeb.Master" Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
    <asp:Literal runat="server" Text="<%$ Txt:2315 Login as occasional user%>" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
  <div id="fullColumn">
    <asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
	    <geocom:TempUserControl ID="tempUserControl" runat="server" />
    </asp:PlaceHolder>
    </div>
</asp:Content>
