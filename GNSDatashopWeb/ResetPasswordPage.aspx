<%@ Page Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true"
    CodeBehind="ResetPasswordPage.aspx.cs" Inherits="GEOCOM.GNSD.Web.ResetPasswordPage" Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
    <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:2407 We will create a new password and send it to your email address. Your old password will no longer be valid.%>" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <fieldset>
            <legend>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:2410 Reset password %>" />
            </legend>
            <div style="margin: 10px 0 5px 0;">
                <asp:Literal runat="server" Text="<%$ Txt:2400 Here you can reset your password.%>" />
            </div>
            <div>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <geocom:LabelAndTextBox ID="email" runat="server" LabelText="<%$ Txt:2257 Email:%>"
                            LabelCssClass="formElementLabel" TextBoxCssClass="welcome_page-edit" TextBoxRequiredCssClass="requiredElement"
                            Required="True" RegEx="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$" ValidationFailedText="<%$  Txt:2308 Please enter your email.%>"
                            RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email address.%>" OnValidationFailed="ShowMessage" />
                        <geocom:CaptchaControl ID="captcha" runat="server"
                            CaptchaFontWarping="None" CaptchaBackgroundNoise="Low" CaptchaLineNoise="Low" CaptchaWidth="200" CaptchaLength="5" CssClass="captcha"
                            CaptchaImageCssClass="captchaImage" InputFieldCssClass="captchaInputField" InputLabelCssClass="formElementLabel" RefreshButtonCssClass="captchaButton"
                            CaptchaExpiredText="<%$ Txt:2061 Captcha expired after {0} seconds %>" CaptchaNotReadyText="<%$ Txt:2062 Captcha not ready, wait {0} seconds %>" RefreshButtonText="<%$ Txt:2060 Refresh %>" CaptchaInputLabelText="<%$ Txt:2064 Type code here %>" CaptchaValidationFailedText="<%$ Txt:2063 Captcha invalid %>" />
                        <asp:Button ID="bntResetPassword" runat="server" Text="<%$ Txt:2401 Reset password%>" OnClick="BtnResetPasswordOnClick" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="margin: 10px 0 0px 180px;">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/WelcomePage.aspx" Text="<%$ Txt:2402 Back%>" />
            </div>
        </fieldset>
    </div>
</asp:Content>
