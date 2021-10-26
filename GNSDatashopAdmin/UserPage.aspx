<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="UserPage.aspx.cs" Inherits="GNSDatashopAdmin.UserPage" Title="Admin Interface - Userdetails"
    ValidateRequest="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
<h3>User details</h3>
    <asp:Label ID="ErrorLable" runat="server" Text=""></asp:Label>
    <asp:GridView ID="UserGrid" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
        DataKeyNames="UserId" AutoGenerateColumns="False" CssClass="adminTable">
         <PagerSettings Mode="NumericFirstLast" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle CssClass="pager" HorizontalAlign="Left" />
            <FooterStyle CssClass="pager" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="UserId" ReadOnly="True" SortExpression="UserId" />
            <asp:BoundField DataField="Salutation" HeaderText="Salutation" SortExpression="Salutation" />
            <asp:BoundField DataField="Firstname" HeaderText="Firstname" SortExpression="Firstname" />
            <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="Street" HeaderText="Street" SortExpression="Street" />
            <asp:BoundField DataField="Streetnr" HeaderText="Street no." SortExpression="Streetnr" />
            <asp:BoundField DataField="Citycode" HeaderText="Postal code" SortExpression="Citycode" />
            <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
            <asp:BoundField DataField="Company" HeaderText="Company" SortExpression="Company" />
            <asp:BoundField DataField="Tel" HeaderText="Phone" SortExpression="Tel" />
            <asp:BoundField DataField="Fax" HeaderText="Fax" SortExpression="Fax" />
            <asp:BoundField DataField="BizUserId" DataFormatString="Yes" HeaderText="Businessuser" NullDisplayText="No" SortExpression="BizUserId" />
        </Columns>
    </asp:GridView>
    <br />
    
    <asp:Label ID="JobError" runat="server" Text="" ></asp:Label><br />        
    <br />
    
    <div>    
    <asp:GridView ID="JobGrid" runat="server" AllowSorting="True" CellPadding="4" ForeColor="#333333"
        GridLines="None" OnSorting="JobGrid_Sorting" OnRowCreated="JobGrid_RowCreated" DataKeyNames="JobId" PageSize="20" 
        AutoGenerateColumns="False" ShowFooter="True" CssClass="adminTable">
        <PagerSettings Mode="NumericFirstLast" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle CssClass="pager" HorizontalAlign="Left" />
            <FooterStyle CssClass="pager" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" />
        <Columns>

            <asp:HyperLinkField DataTextField="JobId" HeaderText="JobId" DataNavigateUrlFields="JobId"
                DataNavigateUrlFormatString="JobDetailDynamic.aspx?JobId={0}" SortExpression="JobId">
            </asp:HyperLinkField>
            <asp:HyperLinkField DataTextField="UserId" HeaderText="UserId" DataNavigateUrlFields="UserId"
                DataNavigateUrlFormatString="UserPage.aspx?UserId={0}" SortExpression="UserId">
                
            </asp:HyperLinkField>
            <asp:BoundField DataField="FirstName" HeaderText="First name" ReadOnly="True" SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Last name" ReadOnly="True" SortExpression="LastName" />
            <asp:BoundField DataField="CreateDate" HeaderText="Create date" ReadOnly="True" SortExpression="CreateDate" />
            <asp:BoundField DataField="LastStateChangeDate" HeaderText="Last state change" ReadOnly="True" SortExpression="LastStateChangeDate" />
            <asp:TemplateField HeaderText="Step" SortExpression="Step">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server"><%# OnStepBind(Eval("Step"), Eval("ProcessorClassId"))%></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>                            
            <asp:TemplateField HeaderText="State" SortExpression="State">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server"><%# OnStateBind(Eval("State"))%></asp:Label>
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
            <asp:TemplateField HeaderText="Archived" SortExpression="IsArchived">
                <ItemTemplate>
                    <asp:Label ID="lblIsArchived" runat="server" Text='<%# (bool)Eval("IsArchived") == true ? "Yes" : "No" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>       
            <asp:TemplateField>
                <FooterTemplate>
                    <asp:Label ID="FooterPagging" runat="server" Text="test"></asp:Label>
                </FooterTemplate>
            </asp:TemplateField>                 
        </Columns>
    </asp:GridView>
    </div>
    <br />
    <div style="float: left">
        <asp:DropDownList ID="ddlArchive" runat="server" OnSelectedIndexChanged="ArchiveIndexChange"
            AutoPostBack="True">
            <asp:ListItem Value="3">Show all jobs</asp:ListItem>
            <asp:ListItem Value="2">Show archived jobs</asp:ListItem>
            <asp:ListItem Value="1" Selected="True">Show not archived jobs</asp:ListItem>
        </asp:DropDownList>    
        &nbsp;&nbsp;    
        <select name="pageSize" onchange="javascript:__doPostBack('PageSize',this.options[this.options.selectedIndex].value)">
            <option>Page size..</option>
            <option value="5">5</option>
            <option value="10">10</option>
            <option value="20">20</option>
            <option value="50">50</option>
            <option value="100">100</option>
            <option value="200">200</option>
        </select>
    </div>    
    
    <asp:Literal ID="javaScript" runat="server" Text=""></asp:Literal>
</asp:Content>
