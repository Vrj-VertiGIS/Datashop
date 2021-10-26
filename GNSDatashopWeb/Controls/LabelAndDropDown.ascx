<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LabelAndDropDown.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.LabelAndDropDown" %>
<asp:PlaceHolder ID="placeHolder" runat="server" Visible="true">
    <span id="wrap" runat="server"><asp:Label runat="server" ID="label" /><asp:DropDownList runat="server" ID="dropDown" /></span>
</asp:PlaceHolder>