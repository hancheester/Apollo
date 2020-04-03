<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderAccountViewControl" Codebehind="OrderAccountViewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Account information
        <span class="pull-right label label-plain">
            <asp:Hyperlink ID="hlProfile" runat="server" Target="_blank" />
        </span>
    </div>
    <table class="table">
        <tr style='<%= (GetNameCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
            <th>Customer Name</th>
            <td><asp:Literal ID="ltlCustName" runat="server"></asp:Literal>
                <asp:PlaceHolder runat="server" ID="phName" >
                    <span style="float: right;"><asp:LinkButton ID="lbVerifyName" runat="server" Text="Verify" OnClick="lbVerifyName_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr style='<%= (GetCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
            <th>Email</th>
            <td><asp:Literal ID="ltlCustEmail" runat="server"></asp:Literal>
                <asp:PlaceHolder runat="server" ID="phEmail" >
                    <span style="float: right;"><asp:LinkButton ID="lbVerify" runat="server" Text="Verify" OnClick="lbVerify_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <th>Contact Number</th>
            <td><asp:Literal ID="ltlCustContact" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Date of Birth</th>
            <td><asp:Literal ID="ltlCustDOB" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Number of Orders</th>
            <td><asp:Literal ID="ltlNumberOfOrders" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Account Created On</th>
            <td><asp:Literal ID="ltlAccCreatedOn" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Note</th>
            <td><asp:Literal ID="ltlNote" runat="server"></asp:Literal></td>
        </tr>
    </table>
</div>