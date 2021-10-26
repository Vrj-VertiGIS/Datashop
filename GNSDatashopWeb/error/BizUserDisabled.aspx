<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="BizUserDisabled.aspx.cs" Inherits="GEOCOM.GNSD.Web.error.BizUserDeactivated" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:9010 Your account has been locked.%>" />
            </legend>
            <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9010 Your account has been locked.%>" />
        </fieldset>
    </div>
</asp:Content>