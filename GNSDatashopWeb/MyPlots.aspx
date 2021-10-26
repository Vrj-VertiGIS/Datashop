<%@ Page Title="<%$ Txt:1000 Externe Map Request - GEOCOM Informatik AG%>" Language="C#" MasterPageFile="~/DatashopWeb.Master" AutoEventWireup="true" CodeBehind="MyPlots.aspx.cs" Inherits="GEOCOM.GNSD.Web.MyPlots" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="js/calendars.js" type="text/javascript"></script>
    <script type="text/javascript">
        dojo.require("dijit.form.DateTextBox");
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <div id="fullColumn">
        <asp:UpdatePanel ID="upnMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <fieldset class="groupBox plots">
                    <legend>
                        <asp:Literal ID="litSearchlegend" runat="server" Text="<%$ Txt:5000 Search%>" />
                    </legend>
                    <div>
                        <geocom:LabelAndTextBox ID="txtJobId" runat="server" LabelText="<%$ Txt:5001 Job Id %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="txtUserId" runat="server" LabelText="<%$ Txt:5002 User Id %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div id="divRepresentativeFields" runat="server">
                        <geocom:LabelAndTextBox ID="txtFirstname" runat="server" LabelText="<%$ Txt:5003 First name %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="txtLastname" runat="server" LabelText="<%$ Txt:5004 Last name %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div id="divRepresentativeFields2" runat="server">
                        <geocom:LabelAndTextBox ID="txtCompany" runat="server" LabelText="<%$ Txt:5005 Company %>" LabelCssClass="formElementLabel" />
                        <span>
                            <asp:Label ID="lblDownloaded" runat="server" Text="<%$ Txt:5008 Downloaded %>" CssClass="formElementLabel" />
                            <asp:DropDownList ID="cboDownloaded" runat="server" Style="margin-left: -5px;">
                                <asp:ListItem Text="-" Value="" />
                                <asp:ListItem Text="<%$ Txt:50080 Yes %>" Value="true" />
                                <asp:ListItem Text="<%$ Txt:50081 No %>" Value="false" />
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div>
                        <script type="text/javascript">
                            // these variables are used in calendars.js - a little totally sweet hackli ;-)
                            var calender1ClientId = "<% = calender1.ClientID %>";
                            var calender2ClientId = "<% = calender2.ClientID %>";
                        </script>
                        <div>
                            <asp:Label ID="lblStartDate" runat="server" Text="<%$ Txt:5006 Time period from%>" CssClass="formElementLabel calendar" />
                            <input runat="server" type="text" name="calender1" id="calender1" dojotype="dijit.form.DateTextBox" invalidmessage="<%$ Txt:39173 Invalid start date %>"
                                required="false" onclick="firstCalendarOnClick" onchange="firstCalendarOnChange" class="plotSearchCalendar" />
                        </div>
                        <div>
                            <asp:Label ID="lblEndDate" runat="server" Text="<%$ Txt:5007 Time period to%>" CssClass="formElementLabel calendar" />
                            <input runat="server" type="text" name="calender2" id="calender2" dojotype="dijit.form.DateTextBox" invalidmessage="<%$ Txt:39173 Invalid end date %>"
                                required="false" onclick="secondCalendarOnClick" onchange="secondCalendarOnChange" class="plotSearchCalendar" />
                        </div>
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="custom1" runat="server" LabelText="<%$ Txt:39161 Additional Field 1 %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="custom2" runat="server" LabelText="<%$ Txt:39162 Additional Field 2 %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="custom3" runat="server" LabelText="<%$ Txt:39163 Additional Field 3 %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="custom4" runat="server" LabelText="<%$ Txt:39164 Additional Field 4 %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="custom5" runat="server" LabelText="<%$ Txt:39165 Additional Field 5 %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="custom6" runat="server" LabelText="<%$ Txt:39166 Additional Field 6 %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="custom7" runat="server" LabelText="<%$ Txt:39167 Additional Field 7 %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="custom8" runat="server" LabelText="<%$ Txt:39168 Additional Field 8 %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="custom9" runat="server" LabelText="<%$ Txt:39169 Additional Field 9 %>" LabelCssClass="formElementLabel" />
                        <geocom:LabelAndTextBox ID="custom10" runat="server" LabelText="<%$ Txt:391610 Additional Field 10 %>" LabelCssClass="formElementLabel" />
                    </div>
                    <div>
                        <geocom:LabelAndTextBox ID="JobParcelNumber" runat="server" LabelText="<%$ Txt:3925 Parcel Number%>" LabelCssClass="formElementLabel" />
                        <asp:Label ID="lblPurpose" runat="server" Text="<%$ Txt:3910 Reason for the request%>" CssClass="formElementLabel" /><asp:DropDownList 
                            ID="cboReason" runat="server" OnLoad="CbSelectReasonLoad" AutoPostBack="true" />
                    </div>
                    <div>
                        <asp:Button runat="server" ID="btnSearch" Text="<%$ Txt:5009 Search %>" OnClick="btnSearch_Click" />
                        <asp:Button runat="server" ID="btnReset" Text="<%$ Txt:5010 Reset %>" OnClick="btnReset_Click" CssClass="noIndent" />
                    </div>
                </fieldset>

                <fieldset class="groupBox plots">
                    <legend>
                        <asp:Literal ID="litOptionsLegend" runat="server" Text="<%$ Txt:5011 Options %>" />
                    </legend>

                    <asp:Label ID="lblPageSize" runat="server" Text="<%$ Txt:5012 Page size %>" CssClass="formElementLabel" />
                    <asp:DropDownList ID="cboPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboPageSize_SelectedIndexChanged" LabelText="Page Size" LabelCssClass="formElementLabel">
                        <asp:ListItem Text="1" Value="1" />
                        <asp:ListItem Text="5" Value="5" />
                        <asp:ListItem Text="10" Value="10" />
                        <asp:ListItem Text="20" Value="20" />
                        <asp:ListItem Text="50" Value="50" />
                        <asp:ListItem Text="100" Value="100" />
                    </asp:DropDownList>

                    <asp:Label ID="lblExportAsCsv" runat="server" Text="<%$ Txt:5013 Export as .csv %>" CssClass="formElementLabel" />
                    <asp:Button runat="server" ID="btnExportAsCsv" Text="<%$ Txt:5014 Export %>" OnClick="btnExportAsCsv_Click" CssClass="noIndent" />
                </fieldset>

                <asp:Label ID="lblRecordCount" runat="server" />

                <asp:GridView ID="gvwPlots" runat="server"
                    AllowPaging="true" AllowSorting="true"
                    AutoGenerateColumns="false"
                    OnPageIndexChanging="gvwPlots_PageIndexChanged"
                    OnSorting="gvwPlots_Sorting"
                    OnRowDataBound="gvwPlots_RowDataBound"
                    OnDataBound="gvwPlots_DataBound" CssClass="datashopGridView">
                    <Columns>
                        <asp:BoundField HeaderText="<%$ Txt:5016 Job Id %>" SortExpression="JobId" DataField="JobId" />
                        <asp:BoundField HeaderText="<%$ Txt:5017 User Id %>" SortExpression="RepresentedUserId" DataField="RepresentedUserId" />
                        <asp:BoundField HeaderText="<%$ Txt:5018 First name %>" SortExpression="RepresentedUserFirstName" DataField="RepresentedUserFirstName" />
                        <asp:BoundField HeaderText="<%$ Txt:5019 Last name %>" SortExpression="RepresentedUserLastName" DataField="RepresentedUserLastName" />
                        <asp:BoundField HeaderText="<%$ Txt:5020 Company %>" SortExpression="RepresentedUserCompany" DataField="RepresentedUserCompany" />

                        <asp:BoundField HeaderText="<%$ Txt:5025 Created date %>" SortExpression="CreatedDate" DataField="CreatedDate" />
                        <asp:BoundField HeaderText="<%$ Txt:39161 Additional Field 1 %>" SortExpression="Custom1" DataField="Custom1" />
                        <asp:BoundField HeaderText="<%$ Txt:39162 Additional Field 2 %>" SortExpression="Custom2" DataField="Custom2" />
                        <asp:BoundField HeaderText="<%$ Txt:39163 Additional Field 3 %>" SortExpression="Custom3" DataField="Custom3" />
                        <asp:BoundField HeaderText="<%$ Txt:39164 Additional Field 4 %>" SortExpression="Custom4" DataField="Custom4" />
                        <asp:BoundField HeaderText="<%$ Txt:39165 Additional Field 5 %>" SortExpression="Custom5" DataField="Custom5" />
                        <asp:BoundField HeaderText="<%$ Txt:39166 Additional Field 6 %>" SortExpression="Custom6" DataField="Custom6" />
                        <asp:BoundField HeaderText="<%$ Txt:39167 Additional Field 7 %>" SortExpression="Custom7" DataField="Custom7" />
                        <asp:BoundField HeaderText="<%$ Txt:39168 Additional Field 8 %>" SortExpression="Custom8" DataField="Custom8" />
                        <asp:BoundField HeaderText="<%$ Txt:39169 Additional Field 9 %>" SortExpression="Custom9" DataField="Custom9" />
                        <asp:BoundField HeaderText="<%$ Txt:391610 Additional Field 10 %>" SortExpression="Custom10" DataField="Custom10" />
                        <asp:BoundField HeaderText="<%$ Txt:3925 Parcel Number%>" SortExpression="ParcelNumber" DataField="ParcelNumber" />
                        <asp:BoundField HeaderText="<%$ Txt:3910 Reason for the request%>" SortExpression="Reason" DataField="Reason" />


                        <asp:BoundField HeaderText="<%$ Txt:5026 Downloaded %>" SortExpression="DownloadCount" DataField="DownloadCount" />


                        <asp:TemplateField HeaderText="Archived">
                            <ItemTemplate>
                                <div style="text-align: center; height: 20px;">
                                    <asp:Image runat="server" ID="imgArchived" ImageUrl="~/images/Tick-mark-icon-png-6619.png" Height="20" Width="20" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="download">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="btnRestart"
                                    ImageUrl="~/images/refresh.png" CausesValidation="false" AlternateText="<%$ Txt:5027666 Restart %>"
                                    OnClick="btnRestart_Click" CommandArgument='<%# Eval("JobId") %>' />
                                <asp:Image runat="server" ID="imgRestartDisabled" ImageUrl="~/images/refreshdisabled.png" Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="download">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="btnDownload"
                                    ImageUrl="~/images/disk.png" CausesValidation="false" AlternateText="<%$ Txt:5027 Download %>"
                                    OnClick="btnDownload_Click" CommandArgument='<%# Eval("JobId") %>' />
                                <asp:Image runat="server" ID="imgDownloadDisabled" ImageUrl="~/images/diskdisabled.png" Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings Mode="NumericFirstLast" Position="Bottom" />
                    <HeaderStyle CssClass="header" />
                    <PagerStyle CssClass="pager" />
                    <RowStyle CssClass="row" />
                    <AlternatingRowStyle CssClass="row alt" />
                </asp:GridView>

            </ContentTemplate>

        </asp:UpdatePanel>

    </div>
</asp:Content>
