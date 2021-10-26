<%@ Page Language="C#" MasterPageFile="~/DatashopAdmin.Master" AutoEventWireup="true"
	CodeBehind="Utilities.aspx.cs" Inherits="GNSDatashopAdmin.Utilities" Title="Admin Interface - Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftPanelContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainPanelContent" runat="server">
	<h3>Utilities</h3>
	<asp:Table runat="server" CellPadding="10">
		<asp:TableRow>
			<asp:TableCell ColumnSpan="2" Height="60px">
				<asp:Label ID="lblJobProzStatus" runat="server" Text="" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="200px">
				<asp:Button runat="server" ID="UpdatePlotDefs" Text="Update Plot Templates" OnClick="UpdatePlotDefsClick" />
			</asp:TableCell><asp:TableCell
				Width="500px">Update the plot definitions here. This will read the folder containing the *.mxt/*.mxd files and make an entry in the database for each found template. Missing templates will be removed. New templates will be added and set to roles 'BUSINESS, TEMP, ADMIN'.</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="2" Height="60px">
				<asp:Label ID="lblUpdatePlotDefStatus" runat="server" Text="" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<h3>Plot templates</h3>
	<div id="plotTemplatesView">
		<asp:GridView ID="TemplateGridView" runat="server" CellPadding="4" ForeColor="#333333"
			GridLines="None" PageSize="1000" AutoGenerateColumns="False" OnRowCancelingEdit="TemplateGridView_RowCancelingEdit"
			OnRowCommand="TemplateGridView_RowCommand" OnRowEditing="TemplateGridView_RowEditing"
			OnRowUpdating="TemplateGridView_RowUpdating" OnRowDeleting="TemplateGridView_RowDeleting" CssClass="adminTable">
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
							ImageUrl="images/edit.gif" ToolTip="Edit this template" />
					</ItemTemplate>
					<EditItemTemplate>
						<asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="True" CommandName="Update"
							ImageUrl="images/accept.png" ToolTip="Save" />
						&nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False"
							CommandName="Cancel" ImageUrl="images/cancel.gif" ToolTip="Cancel" />
					</EditItemTemplate>
				</asp:TemplateField>

				<asp:BoundField DataField="PlotdefinitionKey.Template" HeaderText="Template" ReadOnly="True" SortExpression="Template" />
				<asp:BoundField DataField="PlotHeightCm" HeaderText="Height (cm)" ReadOnly="True"
					SortExpression="PlotHeightCM" />
				<asp:BoundField DataField="PlotWidthCm" HeaderText="Width (cm)" ReadOnly="True" SortExpression="PlotWidthCM" />
				<asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
				<asp:BoundField DataField="Roles" HeaderText="Roles" SortExpression="Roles" />
				<asp:BoundField DataField="LimitsTimePeriods" HeaderText="Time periods of limits [#days or SESSION]" />
				<asp:BoundField DataField="Limits" HeaderText="Limits" />
			</Columns>

		</asp:GridView>

		<h3>Explanations: </h3>
		<ul>
			<li><span style="font-weight: bold">Template </span>- (Filled automatically) The name of a template. Template names must correspond to template files in the plottemplates folder used by the Job Engine. </li>
			<li><span style="font-weight: bold">Height (cm) </span>- (Filled automatically) The width of the template. Required in the DatashopWeb to display plot frames correctly. </li>
			<li><span style="font-weight: bold">Width (cm) </span>- (Filled automatically) The height of the template. Required in the DatashopWeb to display plot frames correctly. </li>
			<li><span style="font-weight: bold">Description </span>- (Filled automatically) The description of the template. Currently not used. </li>
			<li><span style="font-weight: bold">Roles </span>- Enumeration of roles for which the template is enabled. Use comma separated name of roles. E.g. BUSINESS,TEMP </li>
			<li><span style="font-weight: bold">Time periods of limits [#days or SESSION] </span>
				- Determines time validity of the Limits (next column) for Roles (previous column). There are four different formats of the limits
				<br />
				<span style="padding-left: 50px; display: block">1. Single number, e.g. "10" - Validity of all limits for all roles is 10 days.
					<br />
					2. Comma separated numbers, e.g. "10,11,12" - The day limits for individual roles. The count of day limits has to correspond to the number of the Roles. E.g. "BUSINESS,TEMP" | "10,11" means that limit for the BUSINESS role is 10 days and for the TEMP role is 11.
					<br />
					3. SESSION - This value shortens the validity of limits for role just for duration of a user session (till the user is signed out)
					<br />
					4. Comma separated combination of numbers and SESSIONs -  E.g. "BUSINESS,TEMP,ADMIN"|"10,SESSION,12" means 10 days validity of limit for the BUSINESS role,  SESSION validity of limit for the TEMP role and 12 day validity for the ADMIN role.
				</span>

			</li>
			<li><span style="font-weight: bold">Limits</span> - Set plot limits for the Roles. One can set single limit for all roles by providing a single number, 
				e.g. "10" or for every role individually by providing comma separated list limits, e.g. "10,11,12". Be aware that the validity of the limit in previous column.</li>
			<br/>
			
		</ul>
		<br/>
		<h3>Note: </h3>
			<li>Beside these database limits, one can also configure limits in the HostServiceConfig.xml file in the section &lt;restrictions/&gt;.</li>
		<li>The DB and the XML limits are computed independently and the lesser limit is used.</li>
		<li>Limits must not be defined and one or the other can be omitted.</li>
		<li>Should there be no limits configuration whatsoever (either DB or XML), the limits would be set to zero and no plots could be created.</li>


	</div>
</asp:Content>
