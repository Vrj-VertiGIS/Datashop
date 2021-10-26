<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
    CodeBehind="ActivateUsers.aspx.cs" Inherits="GNSDatashopAdmin.ActivateUsers"
    Title="Admin Interface - Unlock Users" ValidateRequest="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
    <h3>Unlock business users</h3>
    <p style="margin:10px 0 10px 0">
        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
    </p>
    <asp:GridView ID="BizGrid" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
        AutoGenerateColumns="False" OnRowCommand="BizGrid_RowCommand" EnableViewState="true" CssClass="adminTable">
         <PagerSettings Mode="NumericFirstLast" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <PagerStyle CssClass="pager" HorizontalAlign="Left" />
            <FooterStyle CssClass="pager" HorizontalAlign="Left" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:HyperLinkField DataTextField="UserId" HeaderText="UserId" DataNavigateUrlFields="UserId"
                DataNavigateUrlFormatString="UserPage.aspx?UserId={0}" SortExpression="UserId">
            </asp:HyperLinkField>
            <asp:BoundField DataField="Salutation" HeaderText="Salutation" SortExpression="Salutation" />
            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
            <asp:TemplateField HeaderText="Email" SortExpression="Email">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("Email", "UserPage.aspx?Email={0}") %>'
                        Text='<%# Eval("Email") %>'></asp:HyperLink>
                </ItemTemplate>
                
            </asp:TemplateField>
            <asp:BoundField DataField="Street" HeaderText="Street" SortExpression="Street" />
            <asp:BoundField DataField="StreetNr" HeaderText="Street no." SortExpression="StreetNr" />
            <asp:BoundField DataField="CityCode" HeaderText="Postal code" SortExpression="CityCode" />
            <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
            <asp:BoundField DataField="Company" HeaderText="Company" SortExpression="Company" />
            <asp:BoundField DataField="Tel" HeaderText="Phone" SortExpression="Tel" />
            <asp:BoundField DataField="Fax" HeaderText="Fax" SortExpression="Fax" />            
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("BizUser.UserStatus") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Unlock">
                <ItemTemplate>
                    <asp:ImageButton ID="btnActivate" runat="server" CommandArgument='<%# Eval("UserId") %>'
                        CommandName="Activate" ToolTip="Unlock this user" ImageUrl="images/accept.png"
                        ImageAlign="Middle" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Remove">
                <ItemTemplate>
                    <asp:ImageButton OnClientClick="return confirm('Are you sure you want to remove this user?');"
                        ID="btnReject" runat="server" CommandArgument='<%# Eval("UserId") %>' CommandName="Reject"
                        ToolTip="Remove this user" ImageUrl="images/cancel.gif" ImageAlign="Middle" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <table cellspacing="0" cellpadding="4" border="0" id="ctl00_MainPanelContent_BizGrid"
                style="color: #333333; border-collapse: collapse;">
                <tr>
                    <th scope="col">
                        UserId
                    </th>
                    <th scope="col">
                        First name
                    </th>
                    <th scope="col">
                        Last name
                    </th>
                    <th scope="col">
                        Email
                    </th>
                    <th scope="col">
                        Street
                    </th>
                    <th scope="col">
                        Street no.
                    </th>
                    <th scope="col">
                        Postal code
                    </th>
                    <th scope="col">
                        City
                    </th>
                    <th scope="col">
                        Company
                    </th>
                    <th scope="col">
                        Phone
                    </th>
                    <th scope="col">
                        Fax
                    </th>
                    <th scope="col">
                        Address
                    </th>
                    <th scope="col">
                        Status
                    </th>
                    <th scope="col">
                        Unlock
                    </th>
                    <th scope="col">
                        Remove
                    </th>
                </tr>
                <tbody>
                    <tr>
                        <td colspan="100" style="background-color: #FFFFFF" align="center">
                            <b>Found no business users who have not been activated yet.</b>
                        </td>
                    </tr>
                </tbody>
            </table>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
 