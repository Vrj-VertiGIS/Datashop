<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonCreateBusinessUser.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.CommonCreateBusinessUser" %>
<%@ Register tagPrefix="geocom" tagName="LabelAndDropDown" src="~/Controls/LabelAndDropDown.ascx" %>
<%@ Register tagPrefix="geocom" tagName="LabelAndTextBox" src="~/Controls/LabelAndTextBox.ascx" %>
<%@ Register tagPrefix="geocom" tagName="Pds" src="~/Controls/Pds.ascx" %>
<%@ Register tagPrefix="geocom" tagName="Agb" src="~/Controls/Agb.ascx" %>

<asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
    <fieldset class="createUserDialog">
        <legend>
            <asp:Literal ID="litTitle" runat="server" Text="<%$ Txt:2316 Login as business user%>" />
        </legend>
        <div>
            <geocom:LabelAndDropDown runat="server" ID="salutation" LabelText="<%$ Txt:2250 Title: %>" ValidationGroup="createBusinessUser"
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
            <geocom:LabelAndTextBox runat="server" ID="firstName" LabelText="<%$ Txt:2251 First name: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2301 Please enter your first name.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="lastName" LabelText="<%$ Txt:2252 Last name: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2302 Please enter your last name.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="street" LabelText="<%$ Txt:2253 Street:%>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2303 Please enter your street.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="streetNumber" LabelText="<%$Txt:2254 Street Nr.: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$  Txt:2304 Please enter your street number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="zip" LabelText="<%$ Txt:2255 Zip: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2305 Please enter your postal code.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="city" LabelText="<%$ Txt:2256 City: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2306 Please enter your city.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="company" LabelText="<%$ Txt:2258 Company: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2307 Please enter your company.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
            <geocom:LabelAndTextBox runat="server" ID="email" LabelText="<%$ Txt:2257 Email: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$  Txt:2308 Please enter your email.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" 
                RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="phone" LabelText="<%$ Txt:2259 Phone Nr.: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2309 Please enter your phone number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
            <geocom:LabelAndTextBox runat="server" ID="fax" LabelText="<%$  Txt:2260 Fax Nr.: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$ Txt:2310 Please enter a fax number.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage"/>
        </div>
        <div>
            <geocom:LabelAndTextBox runat="server" ID="password" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition" LabelText="<%$ Txt:2261 Password:%>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$  Txt:2313 Please enter a password.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                Required="true" TextMode="Password" OnValidationFailed="ShowMessage"
                RegEx="(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"
                RegExValidationFailedText="<%$ Txt:2342 Your password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter and one number or special character.%>" />
            <geocom:LabelAndTextBox runat="server" ID="passwordRepeated" SkipIllegalCharsCheck="true" DataBindingPriority="ControlDefinition" LabelText="<%$ Txt:2262 Repeat Password: %>" ValidationGroup="createBusinessUser"
                ValidationFailedText="<%$  Txt:23131 Please repeat password.%>" LabelCssClass="formElementLabel"
                TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
                Required="true" TextMode="Password" OnValidationFailed="ShowMessage" Visible="true"/>
        </div>
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div style="margin-top: 20px; margin-bottom: 20px;">
                    <geocom:Pds runat="server" ID="pds" AutoPostBack="true" ValidationGroup="createBusinessUser" />
                </div>
                <div style="margin-top: 20px; margin-bottom: 20px;">
                    <geocom:Agb runat="server" ID="agb" AutoPostBack="true" ValidationGroup="createBusinessUser" />
                </div>
                <asp:Button ID="btnAccept" runat="server" Text="<%$ Txt:2036 Continue%>" OnClick="BtnAcceptClicked" ValidationGroup="createBusinessUser" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="noIndent" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
</asp:PlaceHolder>