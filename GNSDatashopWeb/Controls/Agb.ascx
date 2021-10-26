<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Agb.ascx.cs" Inherits="GEOCOM.GNSD.Web.Controls.Agb" %>

<asp:CheckBox ID="chkAcceptAGB" runat="server" Text="<%$ Txt:2034 I accept the general terms and conditions.%>"  CausesValidation="false"  />
<asp:HyperLink ImageUrl="~/images/login/icon_pdf.png" Height="15px" runat="server" ID="linkAgb" Target="_blank" />
 