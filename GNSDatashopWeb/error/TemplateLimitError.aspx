<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="TemplateLimitError.aspx.cs" Inherits="GEOCOM.GNSD.Web.error.TemplateLimitError" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
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
                <asp:Label ID="lblError" runat="server" Text="" />
            </p>
            <p>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:9021 Please try again later.%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>