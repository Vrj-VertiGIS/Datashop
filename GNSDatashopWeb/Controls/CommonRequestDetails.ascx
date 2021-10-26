<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonRequestDetails.ascx.cs"
    Inherits="GEOCOM.GNSD.Web.Controls.CommonRequestDetails" %>
<%@ Register TagPrefix="geocom" TagName="LabelAndTextBox" Src="~/Controls/LabelAndTextBox.ascx" %>
<%@ Register TagPrefix="geocom" TagName="Agb" Src="~/Controls/Agb.ascx" %>
<script type="text/javascript">
    function SetCalendarsRequired() {
        AddToElementCssClass("<% = calender1.ClientID %>", "requiredElement");
        AddToElementCssClass("<% = calender2.ClientID %>", "requiredElement");
    }

    function SetCalendarsOptional() {
        RemoveFromElementCssClass("<% = calender1.ClientID %>", "requiredElement");
        RemoveFromElementCssClass("<% = calender2.ClientID %>", "requiredElement");
    }
</script>
<div id="request_description" class="request_panel">
    <p class="requestpage-subtitle">
        <asp:Literal ID="Literal1" runat="server" Text="<%$ Txt:3909 Please fill out these informations:%>" />
    </p>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="custom1" LabelText="<%$ Txt:39161 Additional Field 1 %>" SkipIllegalCharsCheck="true"
            TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label"
            OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>"
            RegExValidationFailedText="<%$ Txt:39190 Regular Expression Validation Failed for field Custom 1 %>"
            ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="custom2" LabelText="<%$ Txt:39162 Additional Field 2  %>" SkipIllegalCharsCheck="true"
            TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label"
            OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>"
            RegExValidationFailedText="<%$ Txt:39191 Regular Expression Validation Failed for field Custom 2%>"
            ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="custom3" LabelText="<%$ Txt:39163 Additional Field 3  %>" SkipIllegalCharsCheck="true"
            TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label"
            OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>"
            RegExValidationFailedText="<%$ Txt:39192 Regular Expression Validation Failed for field Custom 3%>"
            ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="custom4" LabelText="<%$ Txt:39164 Additional Field 4 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 4" ValidationGroup="request" />
    </div>                                                                              
    <div class="requestpage-control">                                                   
        <geocom:LabelAndTextBox runat="server" ID="custom5" LabelText="<%$ Txt:39165 Additional Field 5 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 5" ValidationGroup="request" />
    </div>                                                                              
    <div class="requestpage-control">                                                   
        <geocom:LabelAndTextBox runat="server" ID="custom6" LabelText="<%$ Txt:39166 Additional Field 6 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 6" ValidationGroup="request" />
    </div>                                                                            
    <div class="requestpage-control">                                                 
        <geocom:LabelAndTextBox runat="server" ID="custom7" LabelText="<%$ Txt:39167 Additional Field 7 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 7" ValidationGroup="request" />
    </div>                                                                              
    <div class="requestpage-control">                                                   
        <geocom:LabelAndTextBox runat="server" ID="custom8" LabelText="<%$ Txt:39168 Additional Field 8 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 8" ValidationGroup="request" />
    </div>                                                                               
    <div class="requestpage-control">                                                    
        <geocom:LabelAndTextBox runat="server" ID="custom9" LabelText="<%$ Txt:39169 Additional Field 9 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 9" ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="custom10" LabelText="<%$ Txt:391610 Additional Field 10 %>" SkipIllegalCharsCheck="true" TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label" OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:3919 Yellow field can not be empty.%>" RegExValidationFailedText="Regular Expression Validation Failed for field Custom 10" ValidationGroup="request" />
    </div>
    
    <script type="text/javascript">
        function setJobDescription(jobDescription) {
            var jobDescElem = dojo.byId("<%= JobDescription.TextBoxClientId %>");
            jobDescElem.value = jobDescription;
        }

         function getJobDescription(jobDescription) {
            var jobDescElem = dojo.byId("<%= JobDescription.TextBoxClientId %>");
             return jobDescElem.value;
         }
    </script>

    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="JobDescription" LabelText="<%$ Txt:3924 Description%>" SkipIllegalCharsCheck="true" MaxFieldLength="4000"
            TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label"
            RegExValidationFailedText="<%$ Txt:39193 Regular Expression Validation Failed for field Job Description%>"
            ValidationFailedText="<%$ Txt:39241 The description must be filled out.%>" OnValidationFailed="ShowMessage"
            TextBoxRequiredCssClass="requiredElement" TextMode="MultiLine" ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <geocom:LabelAndTextBox runat="server" ID="JobParcelNumber" LabelText="<%$ Txt:3925 Parcel Number%>" SkipIllegalCharsCheck="true"
            TextBoxCssClass="requestpage-custom-edit" LabelCssClass="requestpage-custom-label"
            ValidationFailedText="<%$ Txt:39194 Regular Expression Validation Failed for field Job Parcel Number%>"
            OnValidationFailed="ShowMessage" TextBoxRequiredCssClass="requiredElement" ValidationGroup="request" />
    </div>
    <div class="requestpage-control">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="divPurpose">
                    <asp:Label ID="lblPurpose" runat="server" Text="<%$ Txt:3910 Reason for the request%>"
                        CssClass="requestpage-label"></asp:Label><asp:DropDownList ID="cboReason" runat="server"
                            OnLoad="CbSelectReasonLoad" CssClass="requestpage-combobox requiredElement" OnSelectedIndexChanged="OnCboReasonSelectedIndexChanged"
                            AutoPostBack="true" ValidationGroup="request">
                        </asp:DropDownList>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <script type="text/javascript">
        // these variables are used in calendars.js - a little totally sweet hackli ;-)
        var calender1ClientId = "<% = calender1.ClientID %>";
        var calender2ClientId = "<% = calender2.ClientID %>";
    </script>
    <div class="requestpage-control">
        <asp:Label ID="Label1" runat="server" CssClass="requestpage-custom-label" Text="<%$ Txt:39171 Time period from%>" /><input
            type="text" name="calender1" id="calender1" dojotype="dijit.form.DateTextBox"
            invalidmessage="<%$ Txt:39173 Invalid start date %>" required="false" onclick="firstCalendarOnClick"
            onchange="firstCalendarOnChange" class="requestpage-calender" validationgroup="request"
            runat="server" />
    </div>
    <div class="requestpage-control">
        <asp:Label ID="Label2" runat="server" CssClass="requestpage-custom-label" Text="<%$ Txt:39172 Time period to%>" /><input
            type="text" name="calender2" id="calender2" dojotype="dijit.form.DateTextBox"
            invalidmessage="<%$ Txt:39174 Invalid end date %>" required="false" onclick="secondCalendarOnClick"
            onchange="secondCalendarOnChange" class="requestpage-calender" validationgroup="request"
            runat="server" />
    </div>
</div>
<asp:PlaceHolder runat="server" ID="GeoAttachmentsPlaceHolder">
    <div class="request_panel">
        <div>
            <asp:CheckBox runat="server" ID="chbxGeoAttachments" Text="<%$ Txt:39150 Add geo-attachemnts%>"
                AutoPostBack="false" />
        </div>
    </div>
</asp:PlaceHolder>
<script type="text/javascript">

    function SetAcceptButtonLabel(surrogate) {
        var btn = document.getElementById('<%= btnRequest.ClientID %>');
        if (btn)
            btn.value = (surrogate) ? "<%= AcceptAsSurrogateText %>" : "<%= AcceptText %>";
    }

    <%if (UsePdeToolbar)
    {%>

    // gets the polygon definitions out of the map
    //NOTE: hacked together validation for area selections in
    function validateAndSubmit() {
        ValidateArea();
        return false;
    }
    <%}%>
    //gets the hidden field which is used for passing the plot polygons to the server
    function getPolygonInfosHidddenField() {
        var hiddenField = document.getElementById("<%= hiddenPolygonsInfo.ClientID %>");
        return hiddenField;
    }
</script>
<div class="pdeAccept">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div id="request_commit" class="request_panel">
                <div>
                    <geocom:Agb runat="server" OnValidationFailed="ShowMessage" ID="agb" AutoPostBack="true"
                        ValidationGroup="request" />
                </div>
                <div style="margin-top: 5px; margin-right: 10px; text-align: right;">
                    <asp:HiddenField ID="hiddenPolygonsInfo" runat="server" Value="" />
                    <asp:Button ID="btnRequest" runat="server" OnClientClick="return validateAndSubmit();"
                        UseSubmitBehavior="false" OnClick="BtnRequestClick" Text="<%$ Txt:3911 Submit request%>"
                        CausesValidation="false" ValidationGroup="request" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
