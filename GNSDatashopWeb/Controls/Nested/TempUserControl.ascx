<%@ Control
	Language="C#"
	AutoEventWireup="true"
	CodeBehind="TempUserControl.ascx.cs"
	Inherits="GEOCOM.GNSD.Web.TempUserControl" %>
<fieldset>
	<legend>
		<asp:Literal
			ID="Literal2"
			runat="server"
			Text="<%$ Txt:2315 Login as occasional user%>" /></legend>

	<div>
		<geocom:LabelAndDropDown
			runat="server"
			ID="salutation"
			LabelText="<%$ Txt:2250 Title: %>"
			ValidationFailedText="<%$ Txt:2300 Field Title must not be empty.%>"
			LabelCssClass="formElementLabel"
			DropDownRequiredCssClass="requiredElement"
			OnValidationFailed="ShowMessage"
			ValidationGroup="TempUserCntrl">
			<dropdownitems>
                    <asp:ListItem Text="" />
                    <asp:ListItem Text="<%$ Txt:2230 Mr.%>" />
                    <asp:ListItem Text="<%$ Txt:2231 Mrs.%>" />
              </dropdownitems>
		</geocom:LabelAndDropDown>
	</div>
	<div>
		<geocom:LabelAndTextBox
			runat="server"
			ID="firstName"
			LabelText="<%$ Txt:2251 First name: %>"
			ValidationFailedText="<%$ Txt:2301 Please enter your first name.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<geocom:LabelAndTextBox
			runat="server"
			ID="lastName"
			LabelText="<%$ Txt:2252 Last name: %>"
			ValidationFailedText="<%$ Txt:2302 Please enter your last name.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
	</div>
	<div>
		<geocom:LabelAndTextBox
			runat="server"
			ID="street"
			LabelText="<%$ Txt:2253 Street:%>"
			ValidationFailedText="<%$ Txt:2303 Please enter your street.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<geocom:LabelAndTextBox
			runat="server"
			ID="streetNumber"
			LabelText="<%$Txt:2254 Street Nr.: %>"
			ValidationFailedText="<%$  Txt:2304 Please enter your street number.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
	</div>
	<div>
		<geocom:LabelAndTextBox
			runat="server"
			ID="zip"
			LabelText="<%$ Txt:2255 Zip: %>"
			ValidationFailedText="<%$ Txt:2305 Please enter your postal code.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<geocom:LabelAndTextBox
			runat="server"
			ID="city"
			LabelText="<%$ Txt:2256 City: %>"
			ValidationFailedText="<%$ Txt:2306 Please enter your city.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
	</div>
	<div>
		<geocom:LabelAndTextBox
			runat="server"
			ID="company"
			LabelText="<%$ Txt:2258 Company: %>"
			ValidationFailedText="<%$ Txt:2307 Please enter your company.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<geocom:LabelAndTextBox
			runat="server"
			ID="email"
			LabelText="<%$ Txt:2257 Email: %>"
			ValidationFailedText="<%$  Txt:2308 Please enter your email.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<asp:CustomValidator
			runat="server"
			ValidationGroup="TempUserCntrl"
			OnServerValidate="UniqueUserValidator" />
	</div>
	<div>
		<geocom:LabelAndTextBox
			runat="server"
			ID="phone"
			LabelText="<%$ Txt:2259 Phone Nr.: %>"
			ValidationFailedText="<%$ Txt:2309 Please enter your phone number.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
		<geocom:LabelAndTextBox
			runat="server"
			ID="fax"
			LabelText="<%$  Txt:2260 Fax Nr.: %>"
			ValidationFailedText="<%$ Txt:2310 Please enter a fax number.%>"
			LabelCssClass="formElementLabel"
			TextBoxCssClass="registerBusinessUserPage-custom-edit"
			TextBoxRequiredCssClass="requiredElement"
			ValidationGroup="TempUserCntrl"
			OnValidationFailed="ShowMessage" />
	</div>
	<asp:UpdatePanel
		ID="UpdatePanel1"
		runat="server">
		<ContentTemplate>
			<geocom:CaptchaControl
				ID="captcha"
				runat="server"
				CaptchaFontWarping="Low"
				CaptchaBackgroundNoise="Low"
				CaptchaLineNoise="Low"
				CaptchaWidth="200"
				CaptchaLength="5"
				CssClass="captcha"
				CaptchaImageCssClass="captchaImage"
				InputFieldCssClass="captchaInputField"
				InputLabelCssClass="captchaInputLabel"
				RefreshButtonCssClass="captchaButton"
				CaptchaExpiredText="<%$ Txt:2061 Captcha expired after {0} seconds %>"
				CaptchaNotReadyText="<%$ Txt:2062 Captcha not ready, wait {0} seconds %>"
				RefreshButtonText="<%$ Txt:2060 Refresh %>"
				CaptchaInputLabelText="<%$ Txt:2064 Type code here %>"
				CaptchaValidationFailedText="<%$ Txt:2063 Captcha invalid %>"
				ValidationGroup="TempUserCntrl"
				 />
			<div
				class="pds">
				<geocom:Pds
					runat="server"
					ID="pds"
					OnCheckedChanged="agb_OnCheckedChanged"
					ValidationGroup="TempUserCntrl"
					AutoPostBack="true" />
			</div>
			<div
				class="agbs">
				<geocom:Agb
					runat="server"
					ID="agb"
					OnCheckedChanged="agb_OnCheckedChanged"
					ValidationGroup="TempUserCntrl"
					AutoPostBack="true" 
                    ForceTempUserAGB="True"/>
			</div>
			<asp:Button
				ID="btnAccept"
				runat="server"
				Text="<%$ Txt:2036 Continue%>"
				OnClick="BtnAcceptClicked"
				ValidationGroup="TempUserCntrl" />
		</ContentTemplate>
	</asp:UpdatePanel>
</fieldset>
