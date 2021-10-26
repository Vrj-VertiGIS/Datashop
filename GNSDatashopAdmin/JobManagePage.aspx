<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="JobManagePage.aspx.cs" Inherits="GNSDatashopAdmin.JobManagePage"
    Title="Admin Interface - Manage Jobs" ValidateRequest="True" %>

<%@ Register Src="~/Controls/JobGridControl.ascx" TagPrefix="uc1" TagName="JobGridControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
      <h3>Manage jobs</h3>
    <uc1:JobGridControl runat="server" ID="JobGridControl" ShowArchived="False" ShowNotArchived="True" />
</asp:Content>
