<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderHeaderControl" Codebehind="OrderHeaderControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        <asp:Literal ID="ltlOrderTitle" runat="server"></asp:Literal>
    </div>
    <table class="table">
        <tr>
            <th>Order Date</th>
            <td><asp:Literal ID="ltlOrderDate" runat="server"></asp:Literal></td>
        </tr>   
        <tr>
            <th>Order Status</th>
            <td><asp:Literal ID="ltlStatus" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Progress</th>
            <td><asp:Literal ID="ltlIssue" runat="server"></asp:Literal></td>
        </tr>   
        <asp:PlaceHolder ID="phExtraInfo" runat="server">
        <tr>
            <th>IP Location</th>
            <td>
                <div id='iploc_<%= OrderId %>' class="ipLocation">
                    <a href="javascript:void(0);" onclick="javascript:showIPLocation(this, <%= OrderId  %>);"><i class="ipInfo fa fa-question-circle"></i></a> 
                </div>
            </td>
        </tr>        
        <tr>
            <th>Payment Status</th>
            <td>
                <div id='payment_<%= OrderId %>' class="paymentStatus">
                    <a href="javascript:void(0);" onclick="javascript:showPaymentStatus(this, <%= OrderId  %>);"><i class="psInfo fa fa-question-circle"></i></a> 
                </div>
            </td>
        </tr>
        </asp:PlaceHolder>
    </table>
</div>

<script type="text/javascript">
    function showPaymentStatus(sender, orderid) {
        $(sender).find('.psInfo').attr("class", "fa fa-spinner fa-spin");
        
        <%= this.Page.ClientScript.GetCallbackEventReference(this, "'paymentstatus_' + orderid", "loadingPaymentStatus", "orderid") %>;
    }
    
    function showIPLocation(sender, orderid) {
        $(sender).find('.ipInfo').attr("class", "fa fa-spinner fa-spin");
        
        <%= this.Page.ClientScript.GetCallbackEventReference(this, "'iplocation_' + orderid", "loadingIPLocation", "orderid") %>;
    }
    
    function loadingPaymentStatus(msg, context) {
        $('#payment_' + context).html(msg);
    }
    
    function loadingIPLocation(msg, context) {
        $('#iploc_' + context).html(msg);
    }
</script>