<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="JobArchiveWizzard.aspx.cs" Inherits="GNSDatashopAdmin.JobArchiveWizzard"
    Title="Admin Interface - Archive Jobs" %>

<%@ Register Src="~/Controls/JobGridControl.ascx" TagPrefix="uc1" TagName="JobGridControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <h3>Archive jobs</h3>
    <p style="margin-bottom:15px;">Please specify the closing date.</p>
    <p style="margin-bottom:15px;"> For all the jobs that are older, the information (Users, Logs) is collected and written to the directory of this job. After that, the folder will be prefixed with 'ARCHIVED_'.</p>
    <asp:CustomValidator ID="CustomValidator" ControlToValidate="txtDate" runat="server" ValidationGroup="ArchiveWizardGroup"
        ErrorMessage="'Closing date' is invalid. (Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33')"
        OnServerValidate="DateTimeValidator" Display="None" Visible="true" ValidateEmptyText="True" />
    <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="ArchiveWizardGroup" />
    
    <p style="margin-bottom:15px;">
        <asp:Label ID="Label2" runat="server" Text="Closing date:"></asp:Label>
        <asp:TextBox ID="txtDate" runat="server" Text="" ToolTip="'23.04.09' or 'Aug 09' or '12.10.2009 12:33'">
        </asp:TextBox>&nbsp;&nbsp;Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33' 
    </p>
    <p style="margin-bottom:15px;">
        <asp:Button ID="btnGetCount" runat="server" Text="Show number of jobs" OnClick="Count" ValidationGroup="ArchiveWizardGroup" />
        <asp:Label ID="lblCountResult" runat="server" Text=""></asp:Label>  
    </p>
    <p style="margin-bottom:15px;">
        <asp:Button ID="Button2" runat="server" Text="Archive jobs" OnClick="Archive" OnClientClick="return confirm('Are you sure you want to archive the jobs?');"/>
        <asp:Label ID="lblArchSum" runat="server" Text=""></asp:Label>  
    </p>
    
    <asp:BulletedList ID="ResultList" runat="server">
    </asp:BulletedList>
      <h3>Manage archived jobs</h3>
    <uc1:JobGridControl runat="server" ID="JobGridControl" ShowArchived="True" ShowNotArchived="False"/>
</asp:Content>