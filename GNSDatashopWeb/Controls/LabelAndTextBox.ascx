<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LabelAndTextBox.ascx.cs"
    Inherits="GEOCOM.GNSD.Web.Controls.LabelAndTextBox" %>
<asp:PlaceHolder ID="placeHolder" runat="server" Visible="true">
    <span id="wrap" runat="server"><asp:Label runat="server" ID="label" /><asp:TextBox runat="server" ID="textBox" /></span>
</asp:PlaceHolder>