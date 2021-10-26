<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BizUserData.aspx.cs" Inherits="GEOCOM.GNSD.Web.BizUserData"
    MasterPageFile="~/DatashopWeb.Master" Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:2330 Here you can change your personal information and password.%>" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
            <fieldset runat="server" id="UserProfileForm">
                <legend>
                    <asp:Literal ID="Literal2" runat="server" Text="<%$ Txt:2202 Your profile%>" />
                </legend>
                <div>
                    <geocom:LabelAndDropDown runat="server" ID="salutation" LabelText="<%$ Txt:2250 Title: %>"
                        ValidationFailedText="<%$ Txt:2300 Field Title must not be empty.%>" LabelCssClass="formElementLabel"
                        DropDownRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage">
                        <dropdownitems>
                            <asp:ListItem Text="" />
                            <asp:ListItem Text="<%$ Txt:2230 Mr.%>" />
                            <asp:ListItem Text="<%$ Txt:2231 Mrs.%>" />
                      </dropdownitems>
                    </geocom:LabelAndDropDown>
                </div>
                <div>
                    <geocom:LabelAndTextBox runat="server" ID="firstName" LabelText="<%$ Txt:2251 First name: %>"
                        ValidationFailedText="<%$ Txt:2301 Please enter your first name.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                    <geocom:LabelAndTextBox runat="server" ID="lastName" LabelText="<%$ Txt:2252 Last name: %>"
                        ValidationFailedText="<%$ Txt:2302 Please enter your last name.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                </div>
                <div>
                    <geocom:LabelAndTextBox runat="server" ID="street" LabelText="<%$ Txt:2253 Street:%>"
                        ValidationFailedText="<%$ Txt:2303 Please enter your street.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                    <geocom:LabelAndTextBox runat="server" ID="streetNumber" LabelText="<%$Txt:2254 Street Nr.: %>"
                        ValidationFailedText="<%$  Txt:2304 Please enter your street number.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                </div>
                <div>
                    <geocom:LabelAndTextBox runat="server" ID="zip" LabelText="<%$ Txt:2255 Postal code: %>"
                        ValidationFailedText="<%$ Txt:2305 Please enter your postal code.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                    <geocom:LabelAndTextBox runat="server" ID="city" LabelText="<%$ Txt:2256 City: %>"
                        ValidationFailedText="<%$ Txt:2306 Please enter your city.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                </div>
                <div>
                    <geocom:LabelAndTextBox runat="server" ID="company" LabelText="<%$ Txt:2258 Company: %>"
                        ValidationFailedText="<%$ Txt:2307 Please enter your company.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                    <geocom:LabelAndTextBox runat="server" ID="email" LabelText="<%$ Txt:2257 Email: %>"
                        Enabled="False"
                        ValidationFailedText="<%$  Txt:2308 Please enter your email.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>" OnValidationFailed="ShowMessage" />
                </div>
                <div>
                    <geocom:LabelAndTextBox runat="server" ID="phone" LabelText="<%$ Txt:2259 Phone Nr.: %>"
                        ValidationFailedText="<%$ Txt:2309 Please enter your phone number.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                    <geocom:LabelAndTextBox runat="server" ID="fax" LabelText="<%$  Txt:2260 Fax Nr.: %>"
                        ValidationFailedText="<%$ Txt:2310 Please enter a fax number.%>" LabelCssClass="formElementLabel"
                        TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                        OnValidationFailed="ShowMessage" />
                </div>

                <div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:Button ID="btnSaveChanges" runat="server" Text="<%$ Txt:2210 Change personal information%>"
                                OnClick="BtnSaveChangeClicked" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </fieldset>
            <fieldset>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div>
                            <geocom:LabelAndTextBox runat="server" ID="oldPassword" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition"
                                LabelText="<%$ Txt:2344 Old password:%>" ValidationFailedText="<%$ Txt:2340 Please enter a new password.%>"
                                LabelCssClass="formElementLabel" TextBoxCssClass="registerBusinessUserPage-custom-edit"
                                TextBoxRequiredCssClass="requiredElement" Required="true" TextMode="Password"
                                OnValidationFailed="ShowMessage" Visible="true" ValidationGroup="Passwords" />
                        </div>
                        <div>
                            <geocom:LabelAndTextBox runat="server" ID="password" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition"
                                LabelText="<%$ Txt:2345 New password:%>" ValidationFailedText="<%$ Txt:2340 Please enter a new password.%>"
                                LabelCssClass="formElementLabel" TextBoxCssClass="registerBusinessUserPage-custom-edit"
                                TextBoxRequiredCssClass="requiredElement" Required="true" TextMode="Password"
                                OnValidationFailed="ShowMessage" RegEx="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                                RegExValidationFailedText="<%$ Txt:2342 Your password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter and one number or special character.%>"
                                ValidationGroup="Passwords" />
                        </div>
                        <div>
                            <geocom:LabelAndTextBox runat="server" ID="passwordRepeated" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition"
                                LabelText="<%$ Txt:2346 Repeat new password: %>" ValidationFailedText="<%$ Txt:2340 Please enter a new password.%>"
                                LabelCssClass="formElementLabel" TextBoxCssClass="registerBusinessUserPage-custom-edit"
                                TextBoxRequiredCssClass="requiredElement" Required="true" TextMode="Password"
                                OnValidationFailed="ShowMessage" Visible="true" ValidationGroup="Passwords" />
                        </div>
                        <div>

                            <div>
                                <asp:Button ID="btnChangePassword" runat="server" Text="<%$ Txt:2347 Change password%>"
                                    ValidationGroup="Passwords" OnClick="BtnChangePasswordClicked" />
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </fieldset>
        </asp:PlaceHolder>
    </div>
</asp:Content>
