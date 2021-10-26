<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobGridControl.ascx.cs" 
    Inherits="GNSDatashopAdmin.Controls.JobGridControl" %>
        <asp:Button ID="cmdShowSearch" runat="server" Text="Show/Hide Search" OnClick="CmdShowSearchClick" 
            CssClass="searchButton"  style="margin-bottom: 6px;"/>
        <asp:Panel runat="server" id="searchFields" Visible="False" DefaultButton="btnSearch">
            <asp:CustomValidator ID="ValidateDate" ControlToValidate="txbCreateDateOld" runat="server"
                ErrorMessage="'Createdate from' is invalid. (Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33')"
                OnServerValidate="DateTimeValidator" Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup"/>
            <asp:CustomValidator ID="CustomValidator1" ControlToValidate="txbCreateDateNew" runat="server"
                ErrorMessage="'Createdate to' is invalid. (Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33')"
                OnServerValidate="DateTimeValidator" Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup" />
            <asp:CustomValidator ID="CustomValidator2" ControlToValidate="txbStateDateOld" runat="server"
                ErrorMessage="'Statusdate from' is invalid. (Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33')"
                OnServerValidate="DateTimeValidator" Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup"/>
            <asp:CustomValidator ID="CustomValidator3" ControlToValidate="txbStateDateNew" runat="server"
                ErrorMessage="'Statusdate to' is invalid. (Format: '23.04.09' or 'Aug 09' or '12.10.2009 12:33')"
                OnServerValidate="DateTimeValidator" Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup" />
            <asp:CompareValidator ID="CompareValidator1" runat="server" Operator="DataTypeCheck"
                Type="Integer" ControlToValidate="txbUserId" ErrorMessage="UserID must be a whole number." Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup"/>
            <asp:CompareValidator ID="CompareValidator2" runat="server" Operator="DataTypeCheck"
                Type="Integer" ControlToValidate="txbJobId" ErrorMessage="JobID must be a whole number." Display="None" Visible="true" ValidationGroup="JobGridControlValidationGroup"/>
            <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="JobGridControlValidationGroup" />
            <asp:Table ID="tblJobInfo" runat="server" Height="79px" Width="644px" CssClass="search_panel">
               
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell5" runat="server">
                        <asp:Label ID="Label4" runat="server" Text="Createdate from: "></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">
                        <asp:TextBox ID="txbCreateDateOld" runat="server"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell7" runat="server">
                        <asp:Label ID="Label5" runat="server" Text="Createdate to:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server">
                        <asp:TextBox ID="txbCreateDateNew" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="TableCell9" runat="server">
                        <asp:Label ID="Label6" runat="server" Text="Statusdate from: "></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server">
                        <asp:TextBox ID="txbStateDateOld" runat="server"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server">
                        <asp:Label ID="Label7" runat="server" Text="Statusdate to:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server">
                        <asp:TextBox ID="txbStateDateNew" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow4" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server">
                        <asp:Label ID="Label1" runat="server" Text="Reason:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server">
                        <asp:DropDownList ID="ddlReason" runat="server">
                        </asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server">
                        <asp:Label ID="Label8" runat="server" Text="Status:"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server">
                        <asp:DropDownList ID="ddlStatus" runat="server">
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow100" runat="server" Visible="True">
                    <asp:TableCell ID="TableCell100" runat="server">
                        <asp:Label ID="lblFree1" runat="server" Text="Custom1: " /></asp:TableCell>
                    <asp:TableCell ID="TableCell101" runat="server">
                        <asp:TextBox ID="txbFree1" runat="server" /></asp:TableCell>
                   
                    <asp:TableCell ID="TableCell3" runat="server">
                        <asp:Label ID="lblFree2" runat="server" Text="Custom2: " /></asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">
                        <asp:TextBox ID="txbFree2" runat="server" /></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell  runat="server">
                        <asp:Label  runat="server" Text="Custom3: " /></asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="txbFree3" runat="server" /></asp:TableCell>

                    <asp:TableCell ID="TableCell1" runat="server">
                        <asp:Label ID="Label2" runat="server" Text="JobID: "></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server">
                        <asp:TextBox ID="txbJobId" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server"><asp:Label  runat="server" Text="FirstName"/></asp:TableCell>
                    <asp:TableCell runat="server"><asp:TextBox runat="server" ID="tbxFirstName" /></asp:TableCell>
                    
                     <asp:TableCell runat="server"><asp:Label runat="server" Text="LastName"/></asp:TableCell>
                    <asp:TableCell runat="server"><asp:TextBox runat="server" ID="tbxLastName" /></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                     <asp:TableCell ID="TableCell20" runat="server"><asp:Label runat="server" Text="UserID"/></asp:TableCell>
                    <asp:TableCell ID="TableCell21" runat="server"><asp:TextBox runat="server" ID="txbUserId" /></asp:TableCell>
                    
                      <asp:TableCell ID="TableCell18" runat="server"><asp:Label runat="server" Text="MachineName"/></asp:TableCell>
                    <asp:TableCell ID="TableCell19" runat="server"><asp:TextBox runat="server" ID="txbMachineName" /></asp:TableCell>
                </asp:TableRow>
                 <asp:TableRow ID="TableRow1" runat="server">
                     <asp:TableCell runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell17" runat="server" ColumnSpan="2">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="BtnSearch" />
                        <asp:Button ID="btnSearchStopp" runat="server" Text="Clear Search" OnClick="BtnSearchStopp" />      
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>           
        </asp:Panel>
    <asp:GridView ID="JobGrid" runat="server" AllowSorting="True" CellPadding="4" ForeColor="#333333"
        GridLines="None" OnSorting="JobGridSorting" DataKeyNames="JobId" PageSize="20" 
        AutoGenerateColumns="False" OnRowCreated="JobGridRowCreated" ShowFooter="True" CssClass="adminTable">
         <PagerSettings Mode="NumericFirstLast" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle CssClass="pager" HorizontalAlign="Left" />
            <FooterStyle CssClass="pager" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" />
        <Columns>
            
            <asp:HyperLinkField DataTextField="JobId" HeaderText="JobId" DataNavigateUrlFields="JobId"
                DataNavigateUrlFormatString="~/JobDetailDynamic.aspx?JobId={0}" SortExpression="JobId">
                
            </asp:HyperLinkField>
            <asp:HyperLinkField DataTextField="UserId" HeaderText="UserId" DataNavigateUrlFields="UserId"
                DataNavigateUrlFormatString="~/UserPage.aspx?UserId={0}" SortExpression="UserId">
                
            </asp:HyperLinkField>
            <asp:BoundField DataField="FirstName" HeaderText="First name" ReadOnly="True" SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Last name" ReadOnly="True" SortExpression="LastName" />
            <asp:BoundField DataField="CreateDate" HeaderText="Create date" ReadOnly="True" SortExpression="CreateDate" />
            <asp:BoundField DataField="LastStateChangeDate" HeaderText="Last state change" ReadOnly="True" SortExpression="LastStateChangeDate" />
            <asp:TemplateField HeaderText="Step" SortExpression="Step">
                <ItemTemplate>
                    <asp:Label runat="server"><%# OnStepBind(Eval("Step"), Eval("ProcessorClassId"))%></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>                            
            <asp:TemplateField HeaderText="State" SortExpression="State">
                <ItemTemplate>
                    <asp:Label runat="server"><%# OnStateBind(Eval("State"))%></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="Reason" HeaderText="Reason" SortExpression="Reason" />
            <asp:BoundField DataField="Custom1" HeaderText="Custom1" SortExpression="Custom1" />            
            <asp:BoundField DataField="Custom2" HeaderText="Custom2" SortExpression="Custom2" />            
            <asp:BoundField DataField="Custom3" HeaderText="Custom3" SortExpression="Custom3" />            
            <asp:BoundField DataField="Custom4" HeaderText="Custom4" SortExpression="Custom4" />            
            <asp:BoundField DataField="Custom5" HeaderText="Custom5" SortExpression="Custom5" />            
            <asp:BoundField DataField="Custom6" HeaderText="Custom6" SortExpression="Custom6" />            
            <asp:BoundField DataField="Custom7" HeaderText="Custom7" SortExpression="Custom7" />            
            <asp:BoundField DataField="Custom8" HeaderText="Custom8" SortExpression="Custom8" />            
            <asp:BoundField DataField="Custom9" HeaderText="Custom9" SortExpression="Custom9" />            
            <asp:BoundField DataField="Custom10" HeaderText="Custom10" SortExpression="Custom10" />                                                       
            <asp:BoundField DataField="DownloadCount" HeaderText="Downloads" SortExpression="DownloadCount" />
            <asp:BoundField DataField="MapExtentCount" HeaderText="MapExtents" SortExpression="MapExtentCount" />
            <asp:TemplateField HeaderText="Dxf Export" SortExpression="DxfExport">
                <ItemTemplate>
                    <asp:Label ID="lblIsDxfExport" runat="server" Text='<%# ((bool)Eval("DxfExport")) ? "Yes" : "No" %>' />
                </ItemTemplate>
            </asp:TemplateField> 
            <%--<asp:BoundField DataField="DxfExport" HeaderText="Dxf Export" ReadOnly="True" SortExpression="DxfExport" />--%>
            <asp:BoundField DataField="MachineName" HeaderText="Machine name" ReadOnly="True" SortExpression="MachineName" />
            <%--           <asp:TemplateField HeaderText="Archived" SortExpression="IsArchived">
                <ItemTemplate>
                    <asp:Label ID="lblIsArchived" runat="server" Text='<%# (bool)Eval("IsArchived") == true ? "Yes" : "No" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>   --%>

            <asp:TemplateField>
                <FooterTemplate>
                    <asp:Label ID="FooterPagging" runat="server" Text=""></asp:Label>
                </FooterTemplate>
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            <table cellspacing="0" cellpadding="4" border="0" id="ctl00_MainPanelContent_JobGrid"
                style="color: #333333; border-collapse: collapse;">
                <tr style="color: White; background-color: #5D7B9D; font-weight: bold;">
                    <th scope="col">
                        JobID
                    </th>
                    <th scope="col">
                        UserID
                    </th>
                    <th scope="col">
                        Firstname
                    </th>
                    <th scope="col">
                        Lastname
                    </th>
                    <th scope="col">
                        Createdate
                    </th>
                    <th scope="col">
                        Statusdate
                    </th>
                    <th scope="col">
                        Step
                    </th>
                    <th scope="col">
                        Status
                    </th>
                    <th scope="col">
                        Reason
                    </th>
                    <th scope="col">
                        Uploadjob
                    </th>
                    <th scope="col">
                        Archived
                    </th>
                    <th scope="col">
                        &nbsp;
                    </th>
                    <th scope="col">
                        &nbsp;
                    </th>
                </tr>
                <tr style="color: #333333; background-color: #F7F6F3;">
                    <td colspan="100" align="center">
                        <b>No jobs found.</b>
                    </td>
                </tr>
            </table>
        </EmptyDataTemplate>
    </asp:GridView>
    <br />
    <div style="float: left">
        <asp:DropDownList ID="ddlArchive" runat="server" OnSelectedIndexChanged="ArchiveIndexChange"
            AutoPostBack="True" Visible="False" >
            <asp:ListItem Value="3">Show all jobs</asp:ListItem>
            <asp:ListItem Value="2">Show archived jobs</asp:ListItem>
            <asp:ListItem Value="1" Selected="True">Show not archived jobs</asp:ListItem>
        </asp:DropDownList>    
        <asp:DropDownList ID="DdlPageSize" runat="server" OnSelectedIndexChanged="DdlPageSizeIndexChange"
            AutoPostBack="True">
            <asp:ListItem Value="5">5 jobs per page</asp:ListItem>
            <asp:ListItem Value="10">10 jobs per page</asp:ListItem>
            <asp:ListItem Value="20" Selected="True">20 jobs per page</asp:ListItem>
            <asp:ListItem Value="50">50 jobs per page</asp:ListItem>
            <asp:ListItem Value="100">100 jobs per page</asp:ListItem>
            <asp:ListItem Value="200">200 jobs per page</asp:ListItem>
        </asp:DropDownList>    
    </div>
    
    <asp:Literal ID="javaScript" runat="server"></asp:Literal>