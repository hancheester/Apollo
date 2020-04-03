<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSideMenuControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.OrderSideMenuControl" %>
<ul class="nav nav-tabs hidden-print">
    <asp:Literal ID="ltlInformation" runat="server"></asp:Literal>
    <asp:Literal ID="ltlShipments" runat="server"></asp:Literal>
    <asp:Literal ID="ltlComments" runat="server"></asp:Literal>
    <asp:Literal ID="ltlPharmForm" runat="server"></asp:Literal>
    <asp:Literal ID="ltlTransaction" runat="server"></asp:Literal>
    <asp:Literal ID="ltlEmailPayment" runat="server"></asp:Literal>
</ul>