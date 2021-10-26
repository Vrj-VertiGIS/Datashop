<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true"
    CodeBehind="ConfirmOrder.aspx.cs" Inherits="GEOCOM.GNSD.Web.ConfirmOrder" Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:3000 Confirmation%>" />
            </legend>
            <p style="margin-top: 25px;">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:3001 Your order was received..%>" />
            </p>
            <p style="margin-top: 25px;">
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Txt:3002 Order information:%>" />
            </p>
            <p>
                <asp:Label ID="lblJobID" runat="server" Style="font-weight: bold"></asp:Label>
            </p>
            <p style="margin-top: 25px;">
                <asp:Label ID="lblUserEMail" runat="server"></asp:Label>
            </p>
            <asp:LoginView ID="LoginView1" runat="server">
                <AnonymousTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$Txt:3011 Back to home page %>" />
                    <br />
                    <br />
                </AnonymousTemplate>
            </asp:LoginView>
            <p style="margin-top: 25px;">
                <asp:HyperLink ID="hypPlotRequest" runat="server" style="margin-top: 25px;" />
                <br />
                <asp:HyperLink ID="hypNewPlotSameSettings" runat="server" NavigateUrl="~/RequestPage.aspx?useLastMapExtents=true"
                    Text="<%$Txt:3013 New Plot Request with previous settings %>" style="margin-top: 25px;" />
                <br />
                <br />
            </p>
            <p>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Txt:3005 Thank you for your order. %>" /><br />
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Txt:3006 Your Datashop%>" />
            </p>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftPanelContent" runat="server">
    <asp:Literal ID="Literal6" runat="server" Text="<%$ Txt:3010 For questions regarding your order please contact us with the information on this page at <email address> %>" />
</asp:Content>
