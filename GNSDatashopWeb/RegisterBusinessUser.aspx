<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegisterBusinessUser.aspx.cs"
    Inherits="GEOCOM.GNSD.Web.RegisterBusinessUser" MasterPageFile="~/DatashopWeb.Master" Title="<%$ Txt:1000 GEONIS Datashop%>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:2050 As a business user, you will be notified by email as soon as your account is activated. Please fill in the required information and confirm the terms and condistions in order to register. After your account is activated, you can start placing requests.%>" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
  <div id="fullColumn">
    <asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
        <fieldset>
          <legend><asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:2316 Login as business user%>" /></legend>
        <div>
            <geocom:LabelAndDropDown runat="server" ID="salutation" LabelText="<%$ Txt:2250 Title: %>"
                ValidationFailedText="<%$ Txt:2300 Please enter your title.%>" LabelCssClass="formElementLabel"
                DropDownRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" >
                <DropDownItems>
                    <asp:ListItem Text="" />
                    <asp:ListItem Text="<%$ Txt:2230 Mr.%>" />
                    <asp:ListItem Text="<%$ Txt:2231 Ms.%>" />
              </DropDownItems>
            </geocom:LabelAndDropDown>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="firstName" LabelText="<%$ Txt:2251 First name: %>"
                ValidationFailedText="<%$ Txt:2301 Please enter your first name.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="lastName" LabelText="<%$ Txt:2252 Last name: %>"
                ValidationFailedText="<%$ Txt:2302 Please enter your last name.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="street" LabelText="<%$ Txt:2253 Street:%>"
                ValidationFailedText="<%$ Txt:2303 Please enter your street.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="streetNumber" LabelText="<%$Txt:2254 Street Nr.: %>"
                ValidationFailedText="<%$  Txt:2304 Please enter your street number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="zip" LabelText="<%$ Txt:2255 Zip: %>"
                ValidationFailedText="<%$ Txt:2305 Please enter your postal code.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="city" LabelText="<%$ Txt:2256 City: %>"
                ValidationFailedText="<%$ Txt:2306 Please enter your city.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="company" LabelText="<%$ Txt:2258 Company: %>"
                ValidationFailedText="<%$ Txt:2307 Please enter your company.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
            <geocom:LabelAndTextBox runat="server" ID="email" LabelText="<%$ Txt:2257 Email: %>"
                ValidationFailedText="<%$  Txt:2308 Please enter your email.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" 
                RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>" OnValidationFailed="ShowMessage"/>
            <asp:CustomValidator runat="server" OnServerValidate="UniqueUserValidator" />
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="phone" LabelText="<%$ Txt:2259 Phone Nr.: %>"
                ValidationFailedText="<%$ Txt:2309 Please enter your phone number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="fax" LabelText="<%$  Txt:2260 Fax Nr.: %>"
                ValidationFailedText="<%$ Txt:2310 Please enter a fax number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="password" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition" LabelText="<%$ Txt:2261 Password:%>"
                ValidationFailedText="<%$  Txt:2313 Please enter a password.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                Required="true" TextMode="Password" OnValidationFailed="ShowMessage"
                RegEx="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                RegExValidationFailedText="<%$ Txt:2342 Your password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter and one number or special character.%>" />
            <geocom:LabelAndTextBox runat="server" ID="passwordRepeated" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition" LabelText="<%$ Txt:2262 Repeat Password: %>"
                ValidationFailedText="<%$  Txt:23131 Please repeat password.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                Required="true" TextMode="Password" OnValidationFailed="ShowMessage" Visible="true"/>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <geocom:CaptchaControl ID="captcha" runat="server" 
                        CaptchaFontWarping="None" CaptchaBackgroundNoise="Low" CaptchaLineNoise="Low" CaptchaWidth="200" CaptchaLength="5" CssClass="captcha"
                        CaptchaImageCssClass="captchaImage" InputFieldCssClass="captchaInputField" InputLabelCssClass="formElementLabel" RefreshButtonCssClass="captchaButton"
                         CaptchaExpiredText="<%$ Txt:2061 Captcha expired after {0} seconds %>" CaptchaNotReadyText="<%$ Txt:2062 Captcha not ready, wait {0} seconds %>" RefreshButtonText="<%$ Txt:2060 Refresh %>" CaptchaInputLabelText="<%$ Txt:2064 Type code here %>" CaptchaValidationFailedText="<%$ Txt:2063 Captcha invalid %>" />
                <div class="pds">
                    <geocom:Pds runat="server" ID="pds" AutoPostBack="true" OnCheckedChanged="agb_OnCheckedChanged" />
                </div>
                <div class="agbs">
                    <geocom:Agb runat="server" ID="agb" AutoPostBack="true" OnCheckedChanged="agb_OnCheckedChanged" />
                </div>
                <asp:Button ID="btnAccept" runat="server" Text="<%$ Txt:2036 Continue%>" OnClick="BtnAcceptClicked"/>
            </ContentTemplate>
        </asp:UpdatePanel>
         </fieldset>
    </asp:PlaceHolder>
    </div>
</asp:Content>