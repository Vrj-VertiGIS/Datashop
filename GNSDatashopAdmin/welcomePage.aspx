<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="WelcomePage.aspx.cs" Inherits="GNSDatashopAdmin.welcomePage" Title="Admin Interface - Welcome" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <h3>Administration of GEONIS server Datashop</h3>
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Welcomepage.aspx" VisibleWhenLoggedIn="False"
                OnLoggingIn="Login1_LoggingIn" DisplayRememberMe="False" UserNameLabelText="Email or User Id" 
                PasswordLabelText="Password:" TitleText="Log in" LoginButtonText="Log in" CssClass="login">
                <TextBoxStyle CssClass="loginTextBox" />
                <LoginButtonStyle CssClass="loginButton" />
                <TitleTextStyle CssClass="loginTitle" />
            </asp:Login>
        </AnonymousTemplate>
        <LoggedInTemplate><p>
            <p style="margin-bottom:15px;">Welcome to the administration interface of GEONIS server Datashop.</p>
            <p>Please select a task in the navigation on the left side.</p>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>