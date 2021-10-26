<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="StreamProduct.aspx.cs" Inherits="GEOCOM.GNSD.Web.StreamProduct" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend><asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:3103 Download order%>" /></legend>
            <div>
                <asp:Literal ID="litExpired" runat="server" Text="<%$ Txt:3106 The file you requested has expired. Please click the link below to request another.%>" />
                <asp:Literal ID="litReadyForDownload" runat="server" Text="<%$ Txt:3100 The following file is ready for download:%>" /><br />
                <asp:HiddenField ID="hfdJobID" runat="server" /><br />
                <b><asp:LinkButton ID="lbtDownload" runat="server" onclick="OnClickLbtDownload"><asp:Literal runat="server" Text="<%$ Txt:3101 Download%>" /></asp:LinkButton></b>
                <asp:HyperLink ID="lnkReturnToApp" runat="server" NavigateUrl="~/Default.aspx" Text="<%$ Txt:3105 Return to the Datashop%>" Visible="false" />
            </div>
            <div>
                <p style="line-height: 25px">
                    <br />
                    <asp:Literal runat="server" Text="<%$ Txt:3102 Your order information:%>" /><br />
                    <b><asp:Label ID="lblUserID" runat="server" Text=".unknown."></asp:Label></b><br />
                    <b><asp:Label ID="lblJobId" runat="server" Text=".unknown."></asp:Label></b>
                </p>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftPanelContent" runat="server">
<asp:Literal runat="server" Text="<%$ Txt:3104 Here you can download your order.%>" />
</asp:Content>