<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="UserManagePage.aspx.cs" Inherits="GNSDatashopAdmin.UserManagePage"
    Title="Admin Interface - Manage Tempusers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script language="javascript" type="text/javascript">
    //This script is used to controll the show/hide functionality of the search form
function Button1_onclick() {
    if (document.getElementById('ctl00_MainPanelContent_searchFields').style.display == 'none'){
        document.getElementById('ctl00_MainPanelContent_searchFields').style.display = '';
    }
    else{
        document.getElementById('ctl00_MainPanelContent_searchFields').style.display = 'none';
        }
}
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <h3>Manage occasional users</h3>
    <div id="search">
        <input id="Button1" type="button" value="Show/Hide Search" onclick="return Button1_onclick()"
            class="searchButton" style="margin-bottom: 6px" />
        <div runat="server" id="searchFields">
            <asp:Panel ID="searchForm" DefaultButton="btnSearch" runat="server">
                <asp:Table ID="tblUserInfo" runat="server" Height="79px" Width="644px" CssClass="search_panel" CellPadding="2">
                    <asp:TableRow ID="TableRow7" runat="server">
                        <asp:TableCell ID="TableCell25" runat="server">
                            <asp:Label ID="Label12" runat="server" Text="Salutation:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell26" runat="server">
                            <asp:TextBox ID="txbAdress" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow1" runat="server">
                        <asp:TableCell ID="TableCell1" runat="server">
                            <asp:Label ID="Label2" runat="server" Text="Firstname:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell2" runat="server">
                            <asp:TextBox ID="txbFirstname" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell3" runat="server">
                            <asp:Label ID="Label3" runat="server" Text="Lastname:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell4" runat="server">
                            <asp:TextBox ID="txbLastname" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow2" runat="server">
                        <asp:TableCell ID="TableCell5" runat="server">
                            <asp:Label ID="Label4" runat="server" Text="Street: "></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell6" runat="server">
                            <asp:TextBox ID="txbStreet" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell7" runat="server">
                            <asp:Label ID="Label5" runat="server" Text="Street no.:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell8" runat="server">
                            <asp:TextBox ID="txbStreetnr" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow3" runat="server">
                        <asp:TableCell ID="TableCell9" runat="server">
                            <asp:Label ID="Label6" runat="server" Text="Postal code:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell10" runat="server">
                            <asp:TextBox ID="txbCitycode" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell11" runat="server">
                            <asp:Label ID="Label7" runat="server" Text="City:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell12" runat="server">
                            <asp:TextBox ID="txbCity" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow4" runat="server">
                        <asp:TableCell ID="TableCell13" runat="server">
                            <asp:Label ID="Label1" runat="server" Text="Email:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell14" runat="server">
                            <asp:TextBox ID="txbEmail" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell15" runat="server">
                            <asp:Label ID="Label8" runat="server" Text="Company:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell16" runat="server">
                            <asp:TextBox ID="txbCompany" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow6" runat="server">
                        <asp:TableCell ID="TableCell21" runat="server">
                            <asp:Label ID="Label10" runat="server" Text="Phone:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell22" runat="server">
                            <asp:TextBox ID="txbTel" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell23" runat="server">
                            <asp:Label ID="Label11" runat="server" Text="Fax:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell24" runat="server">
                            <asp:TextBox ID="txbFax" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow5" runat="server">
                        <asp:TableCell ID="TableCell17" runat="server">
                            <asp:Label ID="Label9" runat="server" Text="UserID:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell18" runat="server">
                            <asp:TextBox ID="txbUserID" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow8" runat="server">
                        <asp:TableCell ID="TableCell19" runat="server">
                        </asp:TableCell>
                        <asp:TableCell ID="TableCell20" runat="server">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btn_Search" />
                            <asp:Button ID="btnSearchStopp" runat="server" Text="Clear Search" OnClick="btn_SearchStopp" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </div>
    </div>
    <div onkeypress="if(event.keyCode==13)return false;">
        <asp:GridView ID="UserGrid" runat="server" AllowSorting="True" CellPadding="4" ForeColor="#333333"
            GridLines="None" OnSorting="UserGrid_Sorting" OnRowEditing="UserGrid_RowEditing"
            DataKeyNames="UserId" OnRowCancelingEdit="UserGrid_RowCancelingEdit" OnRowUpdating="UserGrid_RowUpdating"
            PageSize="20" OnRowCommand="UserGrid_RowCommand" AutoGenerateColumns="False"
            ShowFooter="True" OnRowCreated="UserGrid_RowCreated" CssClass="adminTable">
            <PagerSettings Mode="NumericFirstLast" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle CssClass="pager" HorizontalAlign="Left" />
            <FooterStyle CssClass="pager" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" CommandName="Edit"
                            ImageUrl="images/edit.gif" ToolTip="Edit this user" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="True" CommandName="Update"
                            ImageUrl="images/accept.png" ToolTip="Save" />
                        &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False"
                            CommandName="Cancel" ImageUrl="images/cancel.gif" ToolTip="Cancel" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataTextField="UserId" HeaderText="UserId" DataNavigateUrlFields="UserId"
                    DataNavigateUrlFormatString="UserPage.aspx?UserId={0}" SortExpression="UserId">
                    
                </asp:HyperLinkField>
                <asp:BoundField DataField="Salutation" HeaderText="Salutation" SortExpression="Salutation" />
                <asp:BoundField DataField="FirstName" HeaderText="First name" SortExpression="FirstName" />
                <asp:BoundField DataField="LastName" HeaderText="Last name" SortExpression="LastName" />
                <asp:TemplateField HeaderText="Email" SortExpression="Email">
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("Email", "UserPage.aspx?Email={0}") %>'
                            Text='<%# Eval("Email") %>'></asp:HyperLink>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EmailEdit" runat="server" Text='<%# Eval("Email") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Street" HeaderText="Street" SortExpression="Street" />
                <asp:BoundField DataField="StreetNr" HeaderText="Street no." SortExpression="StreetNr" />
                <asp:BoundField DataField="CityCode" HeaderText="Postal code" SortExpression="CityCode" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
                <asp:BoundField DataField="Company" HeaderText="Company" SortExpression="Company" />
                <asp:BoundField DataField="Tel" HeaderText="Phone" SortExpression="Tel" />
                <asp:BoundField DataField="Fax" HeaderText="Fax" SortExpression="Fax" />                
            </Columns>
            <EmptyDataTemplate>
                <table cellspacing="0" cellpadding="4" border="0" id="ctl00_MainPanelContent_UserGrid"
                    style="color: #333333; border-collapse: collapse;">
                    <tr>
                        <th scope="col">
                            &nbsp;
                        </th>
                        <th scope="col">UserId
                        </th>
                        <th scope="col">First name
                        </th>
                        <th scope="col">Last name
                        </th>
                        <th scope="col">Email
                        </th>
                        <th scope="col">Street
                        </th>
                        <th scope="col">Street no.
                        </th>
                        <th scope="col">Postal code
                        </th>
                        <th scope="col">City
                        </th>
                        <th scope="col">Company
                        </th>
                        <th scope="col">
                                Phone
                        </th>
                        <th scope="col">
                                Fax
                        </th>
                        <th scope="col">Adress
                        </th>
                        <th scope="col">
                            &nbsp;
                        </th>
                    </tr>
                    <tr style="color: #333333; background-color: #F7F6F3;">
                        <td colspan="100" align="center">
                            <b>No users found.</b>
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    <div class="bottomGridView">
        <select name="pageSize" onchange="javascript:__doPostBack('PageSize',this.options[this.options.selectedIndex].value)">
            <option>Page size..</option>
            <option value="5">5</option>
            <option value="10">10</option>
            <option value="20">20</option>
            <option value="50">50</option>
            <option value="100">100</option>
            <option value="200">200</option>
            <option value="2000">2000</option>
        </select>
        |
        <asp:Button ID="newButton" runat="server" Text="Add new user" OnClick="newButton_Click"
            CssClass="searchButton" UseSubmitBehavior="False" />
    </div>
    <asp:Literal ID="javaScript" runat="server"></asp:Literal>
</asp:Content>
