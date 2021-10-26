<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonActAsSurrogate.ascx.cs"
    Inherits="GEOCOM.GNSD.Web.Controls.CommonActAsSurrogate" %>
<%@ Register TagPrefix="gnsduc" TagName="LabelAndDropDown" Src="~/Controls/LabelAndDropDown.ascx" %>
<%@ Register TagPrefix="gnsduc" TagName="LabelAndTextBox" Src="~/Controls/LabelAndTextBox.ascx" %>
<%@ Register TagPrefix="gnsduc" TagName="CreateBusinessUser" Src="~/Controls/CommonCreateBusinessUser.ascx" %>
<%@ Register TagPrefix="gnsduc" TagName="CreateTempUser" Src="~/Controls/CommonCreateTempUser.ascx" %>
<%@ Register TagPrefix="gnsdwc" Namespace="GEOCOM.GNSD.Web.Core.WebControls" Assembly="GEOCOM.GNSD.Web.Core" %>
<script type="text/javascript">
    dojo.require("dojox.timing._base");
    dojo.require("dijit.TooltipDialog");
    dojo.require("dojox.data.QueryReadStore");
    function ToggleSurrogateDiv(chkbox) {
        if (SetAcceptButtonLabel) // this is defined by the CommonRequestDetails User Control
            SetAcceptButtonLabel(chkbox.checked);
        ToggleElementDisplayById('<%= surrogateDiv.ClientID %>', chkbox);
    }

    function pageLoad(sender, args) {
        ToggleSurrogateDiv(dojo.byId('<%= chbxSurrogate.ClientID%>'));
    }

    function setUser(userName, userId) {
        var usersSelect = dijit.byId('<%= surrogateDropDown.DropDown.ClientID%>');
        // set dijit filteringSelect value
        usersSelect.value = userId;
        // set dijit filteringSelect text
        usersSelect.textbox.value = userName;
        // set asp.net dropdown hidden form field
        dojo.query("input[name=<%=surrogateDropDown.DropDown.UniqueID%>]")[0].value = userId;
    }

    function cleanUsers() {
        setUser("", "");
    }

</script>
<div class="request_panel">
    <asp:PlaceHolder runat="server" ID="placeHolderSurrogate">
        <div>
            <asp:CheckBox runat="server" ID="chbxSurrogate" Text="<%$ Txt:3990 Create a map as representative%>"
                AutoPostBack="false" onclick="ToggleSurrogateDiv(this)" />
        </div>
        <div runat="server" id="surrogateDiv" style="display: none;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                  <div style="height: 0; width: 100%; text-align: right">
                        <asp:Image  runat="server" ToolTip="<%$ Txt:39991 Clear Filter %>"
                        ImageUrl="~/images/nav/close_button_16x16.png" OnClick="cleanUsers()" style="position: relative; left: 9px; top: -2px"
                        CssClass="requestpage-inlinebuttons"   />
                  </div>
                    <div id="divSelectSurrogateUser" class="requestpage-control">
                        <div data-dojo-type="dojox.data.QueryReadStore" data-dojo-props="url:'SurrogateUsersRestHandler.ashx'" data-dojo-id="SurrogateUsersStore"></div>
                        <gnsduc:LabelAndDropDown runat="server" ID="surrogateDropDown" LabelText="<%$ Txt:3991 User%>"
                            LabelCssClass="requestpage-label" DropDownCssClass="requestpage-combobox" DropDownRequiredCssClass="requiredElement"
                            ValidationFailedText="<%$ Txt:39911 You must select a user%>" OnValidationFailed="ShowMessage"
                            ValidationGroup="surrogate" data-dojo-type="dijit.form.FilteringSelect"
                            data-dojo-props="store:SurrogateUsersStore, searchAttr:'name', pageSize:40, highlightMatch:'all',autoComplete:false, queryExpr:'*${0}*', searchDelay:666 " />

                    </div>

                    <div class="divAddUser">
                        <asp:Button ID="btnAddBusiness" runat="server" OnClientClick="MDCreateBusinessUser.Show(); return false;"
                            CssClass="btnAddUser" Text="<%$ Txt:3995 Add a new business user%>" Visible="false" />
                        <asp:Button ID="btnAddTemp" runat="server" OnClientClick="MDCreateTempUser.Show(); return false;"
                            class="btnAddUser" Text="<%$ Txt:3996 Add a new temp user%>" Visible="false" />
                    </div>
                    <div class="requestpage-control">
                        <gnsduc:LabelAndTextBox ID="surrogateCalender" runat="server" LabelCssClass="requestpage-custom-label"
                            LabelText="<%$ Txt:3992 Request date%>" TextBoxCssClass="requestpage-calender"
                            TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" ValidationFailedText="<%$ Txt:39921 You must specify a request date%>"
                            IsDojoDateTextBox="true" ValidationGroup="surrogate" />
                    </div>
                    <div class="requestpage-control">
                        <gnsduc:LabelAndDropDown ID="surrogateRequestWayDropDown" runat="server" LabelCssClass="requestpage-custom-label"
                            LabelText="<%$ Txt:3993 How was the request placed?%>" DropDownCssClass="requestpage-combobox"
                            DropDownRequiredCssClass="requiredElement" ValidationFailedText="<%$ Txt:39931 Select how the job request was placed.%>"
                            OnValidationFailed="ShowMessage" ValidationGroup="surrogate" IsDojoFilteringSelect="true" />
                        <gnsduc:LabelAndTextBox ID="surrogateRequestWay" runat="server" LabelCssClass="requestpage-custom-label"
                            LabelText="<%$ Txt:3993 How was the request placed?%>" TextBoxCssClass="requestpage-custom-edit"
                            TextBoxRequiredCssClass="requiredElement" OnValidationFailed="ShowMessage" ValidationFailedText="<%$ Txt:39931 Specify how the job request was placed.%>"
                            ValidationGroup="surrogate" />
                    </div>
                    <div id="divStopAfterProcess" runat="server" class="requestpage-control">
                        <asp:Label runat="server" ID="lblStopAfterProcess" CssClass="requestpage-custom-label"
                            Text="<%$ Txt:3994 Stop the job before it is packaged.%>"></asp:Label>
                        <asp:CheckBox runat="server" ID="chbxStopAferProcess" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <gnsdwc:DsModalDialog ID="MDCreateBusinessUser" runat="server" Title="<%$ Txt:3995 Add a new business user %>" Height="300" Width="800" ZIndex="600" IsUpdatePanel="true" Visible="false">
            <DialogTemplate>
                <gnsduc:CreateBusinessUser ID="ucCreateBusiness" runat="server" HasTitle="false" HasCancel="true" ClientCancelScript="MDCreateBusinessUser.Hide()" />
            </DialogTemplate>
        </gnsdwc:DsModalDialog>
        <gnsdwc:DsModalDialog ID="MDCreateTempUser" runat="server" Title="<%$ Txt:3996 Add a new temp user %>" Height="300" Width="800" ZIndex="600" IsUpdatePanel="true" Visible="false">
            <DialogTemplate>
                <gnsduc:CreateTempUser ID="ucCreateTemp" runat="server" HasTitle="false" HasCancel="true" ClientCancelScript="MDCreateTempUser.Hide()" />
            </DialogTemplate>
        </gnsdwc:DsModalDialog>
    </asp:PlaceHolder>
</div>
