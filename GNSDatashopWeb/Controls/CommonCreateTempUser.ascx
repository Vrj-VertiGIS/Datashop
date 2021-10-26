<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonCreateTempUser.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.CommonCreateTempUser" %>
<%@ Register TagPrefix="geocom" TagName="LabelAndDropDown" Src="~/Controls/LabelAndDropDown.ascx" %>
<%@ Register TagPrefix="geocom" TagName="LabelAndTextBox" Src="~/Controls/LabelAndTextBox.ascx" %>
<%@ Register TagPrefix="geocom" TagName="Pds" Src="~/Controls/Pds.ascx" %>
<%@ Register TagPrefix="geocom" TagName="Agb" Src="~/Controls/Agb.ascx" %>

<asp:PlaceHolder ID="MostTopElementPlaceHolder" runat="server">
	<fieldset class="createUserDialog">
		<legend>
			<asp:Literal ID="litTitle" runat="server" Text="<%$ Txt:2315 Login as occasional user%>" />
		</legend>
		<div>
			<geocom:LabelAndDropDown runat="server" ID="salutation" LabelText="<%$ Txt:2250 Title: %>" ValidationGroup="createTempUser"
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
			<geocom:LabelAndTextBox runat="server" ID="firstName" LabelText="<%$ Txt:2251 First name: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2301 Please enter your first name.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
			<geocom:LabelAndTextBox runat="server" ID="lastName" LabelText="<%$ Txt:2252 Last name: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2302 Please enter your last name.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
		</div>
		<div>
			<geocom:LabelAndTextBox runat="server" ID="street" LabelText="<%$ Txt:2253 Street:%>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2303 Please enter your street.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
			<geocom:LabelAndTextBox runat="server" ID="streetNumber" LabelText="<%$Txt:2254 Street Nr.: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$  Txt:2304 Please enter your street number.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
		</div>
		<div>
			<geocom:LabelAndTextBox runat="server" ID="zip" LabelText="<%$ Txt:2255 Zip: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2305 Please enter your postal code.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
			<geocom:LabelAndTextBox runat="server" ID="city" LabelText="<%$ Txt:2256 City: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2306 Please enter your city.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
		</div>
		<div>
			<geocom:LabelAndTextBox runat="server" ID="company" LabelText="<%$ Txt:2258 Company: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2307 Please enter your company.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
			<geocom:LabelAndTextBox runat="server" ID="email" LabelText="<%$ Txt:2257 Email: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$  Txt:2308 Please enter your email.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement"
				RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>" OnValidationFailed="ShowMessage" />
		</div>
		<div>
			<geocom:LabelAndTextBox runat="server" ID="phone" LabelText="<%$ Txt:2259 Phone Nr.: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2309 Please enter your phone number.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
			<geocom:LabelAndTextBox runat="server" ID="fax" LabelText="<%$  Txt:2260 Fax Nr.: %>" ValidationGroup="createTempUser"
				ValidationFailedText="<%$ Txt:2310 Please enter a fax number.%>" LabelCssClass="formElementLabel"
				TextBoxCssClass="registerBusinessUserPage-custom-edit" TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" />
		</div>

		<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
			<ContentTemplate>
				<div style="margin-top: 20px; margin-bottom: 20px;">
					<geocom:Pds runat="server" ID="pds" AutoPostBack="true" ValidationGroup="createTempUser" />
				</div>
				<div style="margin-top: 20px; margin-bottom: 20px;">
					<geocom:Agb runat="server" ID="agb" AutoPostBack="true" ValidationGroup="createTempUser" ForceTempUserAGB="True" />
				</div>
				<asp:Button ID="btnAccept" runat="server" Text="<%$ Txt:2036 Continue%>" OnClick="BtnAcceptClicked" ValidationGroup="createTempUser" />&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
			</ContentTemplate>
		</asp:UpdatePanel>
	</fieldset>
</asp:PlaceHolder>



