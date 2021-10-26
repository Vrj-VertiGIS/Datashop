<%@ Page
	Language="C#"
	MasterPageFile="~/DatashopWeb.Master"
	AutoEventWireup="true"
	CodeBehind="WelcomePage.aspx.cs"
	Inherits="GEOCOM.GNSD.Web.WelcomePage"
	Title="<%$ Txt:1000 GEONIS Datashop%>" %>

<asp:Content
	ID="Content1"
	ContentPlaceHolderID="HeadContent"
	runat="server">
    <script type="text/javascript">
        dojo.ready(function () {
            // bug https://issuetracker02.eggits.net/browse/DATASHOP-476 - redirect to this site if it is in an iframe
            var inIframe = window.top !== window.self;
            if (inIframe) {
                // redirect the parent to this site
                var welcomePageUrl = window.location.href.split("?")[0];
                window.top.location = welcomePageUrl;
            }
        })
    </script>
</asp:Content>

<asp:Content
	ID="Content2"
	ContentPlaceHolderID="MainPanelContent"
	runat="server">
	<div
		id="fullColumn">
		<h3>
			<asp:Literal
				ID="Literal1"
				runat="server"
				Text="<%$ Txt:1510 Welcome to the Datashop Map Information System %>" />
		</h3>
		<h4>
			<asp:Literal
				runat="server"
				ID="litOptions"
				Text="<%$ Txt:1500 Select the desired option:%>" />
		</h4>
		<div
			style="width: 600px;">
			<asp:MultiView
				runat="server"
				ID="tempUserMltVw">
				<asp:View
					runat="server">
					<fieldset
						id="divOccasionalUser"
						runat="server">
						<legend>
							<asp:Literal
								ID="Literal2"
								runat="server"
								Text="<%$ Txt:1501 Request a map as <b>occasional user</b>%>" />
						</legend>
						<asp:Image
							ID="Image2"
							runat="server"
							ImageUrl="~/images/welcome/button_login2.png"
							CssClass="welcome_page_image" />
						<asp:Literal
							ID="Literal3"
							runat="server"
							Text="<%$ Txt:15031 If you order only occasionally maps, %>" />
						<asp:LinkButton
							ID="LinkButton1"
							OnClick="cmdLoginAsTempUserClick"
							runat="server"
							Text="<%$ Txt:15032 click here%>" />.
					</fieldset>
				</asp:View>
				<asp:View
					runat="server">
					<geocom:TempUserControl
						ID="tempUserControl"
						runat="server" />
				</asp:View>
			</asp:MultiView>

			<asp:LoginView 
				
				ID="LoginView1"
				runat="server">
				<AnonymousTemplate>
					<fieldset>
						<legend>
							<asp:Literal
								ID="Literal4"
								runat="server"
								Text="<%$ Txt:1502 Login for <b>business users</b>%>" />
						</legend>
						
						<asp:Login
							ID="loginMask"
							runat="server"
							VisibleWhenLoggedIn="false"
							OnLoggedIn="LoginMaskOnLoggedIn"
							OnLoginError="LoginMaskOnLoginFailed"
							CssClass="login">
							<LayoutTemplate>
								<asp:Image
									ID="Image1"
									runat="server"
									ImageUrl="~/images/welcome/button_login1.png"
									CssClass="welcome_page_image" />
								<div>
									<geocom:LabelAndTextBox
										runat="server"
										ID="UserName"
										LabelText="<%$ Txt:2003 Email or user id%>"
										HasFocus="true"
										RegEx="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$|^\d+$"
										RegExValidationFailedText="<%$ Txt:2311 Please enter a correct email.%>" 
										LabelCssClass="formElementLabel"
										TextBoxCssClass="welcome_page-edit"
										TextBoxRequiredCssClass="requiredElement"
										OnValidationFailed="ShowMessage"
										ValidationFailedText="<%$ Txt:2013 User name is required%>"
										ValidationGroup="BusinessUserGroup"
										Required="true" />
								</div>
								<div>
									<geocom:LabelAndTextBox
										runat="server"
										ID="Password"
										SkipIllegalCharsCheck="true"
										LabelText="<%$ Txt:2005 Password%>"
										LabelCssClass="formElementLabel"
										TextBoxCssClass="welcome_page-edit"
										TextBoxRequiredCssClass="requiredElement"
										ValidationFailedText="<%$ Txt:2014 Password is required%>"
										OnValidationFailed="ShowMessage"
										Required="true"
										ValidationGroup="BusinessUserGroup"
										TextMode="Password" />
								</div>
								<div
									style="margin: 10px 0 0 180px;">
									<asp:LinkButton
										ID="LinkButton2"
										runat="server"
										OnClick="cmdCreateBusinessUserClick"
										Text="<%$ Txt:2022 Register as a new business user%>" />
									<asp:HyperLink
										ID="HyperLink1"
										runat="server"
										NavigateUrl="~/ResetPasswordPage.aspx"
										Text="<%$ Txt:2023 Lost password%>" />
								</div>
								<asp:UpdatePanel
									ID="UpdatePanel1"
									runat="server">
									<ContentTemplate>
										<asp:Button
											ID="btnLogin"
											runat="server"
											Text="<%$ Txt:2002 Login%>"
											ValidationGroup="BusinessUserGroup"

											CommandName="Login" />
									</ContentTemplate>
								</asp:UpdatePanel>
							</LayoutTemplate>
						</asp:Login>
					</fieldset>
				</AnonymousTemplate>
				<RoleGroups>
					<asp:RoleGroup
						Roles="BUSINESS, ADMIN, INTERNAL">
						<ContentTemplate>
							<div>
								<asp:HyperLink
									ID="HyperLink1"
									runat="server"
									NavigateUrl="RequestPage.aspx"
									Text="<%$ Txt:2021 Here you can request a new map order.%>" />
							</div>
						</ContentTemplate>
					</asp:RoleGroup>
					<asp:RoleGroup
						Roles="REPRESENTATIVE">
						<ContentTemplate>
							<div>
								<asp:Literal
									ID="Literal4"
									runat="server"
									Text="<%$ Txt:2015 You need to be a member of the Business Users role to submit frequent plot requests. Please contact your administrator. %>" />
							</div>
						</ContentTemplate>
					</asp:RoleGroup>
				</RoleGroups>
			</asp:LoginView>


		</div>
	</div>
</asp:Content>
<asp:Content
	ID="Content3"
	ContentPlaceHolderID="LeftPanelContent"
	runat="server">
	<p>
		<asp:Literal
			runat="server"
			Text="<%$ Txt:1505 For frequent requests, register as a business user.%>" /><br />
		<asp:Literal
			runat="server"
			Text="<%$ Txt:1506 If you request information on an irregular occasion, access the system as a occasional user.%>" />
	</p>
</asp:Content>
