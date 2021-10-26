<%@ Page Title="" Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="GEOCOM.GNSD.Web.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <h3>
            <asp:Literal ID="litAboutTitle" runat="server" Text="<%$ Txt:6000 Download%>" />
        </h3>
        <p>
            <asp:Literal ID="litAboutText" runat="server" Text="<%$ Txt:6001 Download%>" />
        </p>
    </div>
</asp:Content>