<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderPaymentViewControl" Codebehind="OrderPaymentViewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Payment information <asp:HiddenField ID="hfOrderId" runat="server" />
    </div>
    <asp:PlaceHolder ID="phPayByPhone" runat="server" Visible="false">
        <table class="table">
            <tr>
                <th>Pay by Phone</th>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Enter payment reference<strong>*</strong></td>
                <td><asp:TextBox ID="txtPaymentRef" CssClass="form-control" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvPayRef" runat="server" ControlToValidate="txtPaymentRef" Display="Dynamic"
                         ValidationGroup="vgPayment" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Please enter a payment reference.</span>" />
                </td>
            </tr>
            <tr>
                <td colspan="2"><asp:LinkButton ID="lbPayByPhone" runat="server" Text="Submit" CssClass="pull-right btn btn-outline btn-danger" OnClick="lbPayByPhone_Click" ValidationGroup="vgPayment" /></td>
            </tr>
        </table>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phPaymentDetails" runat="server">
        <table class="table">
            <tr>
                <th>Payment Method</th>
                <td><asp:Literal ID="ltlPaymentMethod" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Payment Reference</th>
                <td><asp:Literal ID="ltlPaymentRef" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Payment Check</th>
                <td><asp:Literal ID="ltPaymentCheck" runat="server" /></td>
            </tr>
            <tr>
                <th>3rd Man Score</th>
                <td>
                    <asp:PlaceHolder ID="phThirdScore" runat="server">
                        <div id='third_<%= OrderId %>' class="paymentStatus">
                            <a href="javascript:void(0);" onclick="javascript:showThirdMan(this, <%= OrderId  %>);"><i class="p3rdMan fa fa-question-circle"></i></a> 
                        </div>
                    </asp:PlaceHolder>
                    <asp:Literal ID="ltlThirdScoreNow" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr style='<%= (GetAvsCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
                <th>Address Verification System (AVS) <a href="https://en.wikipedia.org/wiki/Address_Verification_System" target="_blank"><i class="ipInfo fa fa-question-circle"></i></a></th>
                <td>
                    <asp:Literal ID="ltAVS" runat="server"></asp:Literal>                    
                    <asp:PlaceHolder runat="server" ID="phAVS" Visible="false" >
                        <span style="float: right;"><asp:LinkButton ID="lbVerifyAVS" runat="server" Text="Verify" OnClick="lbVerifyAVS_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                    </asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <th>Payment Details</th>
                <td>
                    <asp:PlaceHolder ID="phPaymentSagePayDetail" runat="server">
                        <div id='details_<%= OrderId %>' class="paymentDetails">
                            <a href="javascript:void(0);" onclick="javascript:showPaymentDetails(this, <%= OrderId  %>);"><i class="pPaymentDetails fa fa-question-circle"></i></a> 
                        </div>
                    </asp:PlaceHolder>
                    <asp:Literal ID="ltlPaymentDetailsNow" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <th>Paid Email Payments</th>
                <td>
                    <asp:Repeater ID="rptEmailInvoice" runat="server">
                        <ItemTemplate>
                            <%# Eval("PaymentRef") + " (" + AdminStoreUtility.GetFormattedPrice(Convert.ToDecimal(Eval("Amount")), Eval("CurrencyCode").ToString(), CurrencyType.Code, Convert.ToDecimal(Eval("ExchangeRate")), places: 2) + ")" %><br />
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>                                
</div>
 
 <script type="text/javascript">
    function showThirdMan(sender, orderid) {
        $(sender).find('.p3rdMan').attr('class', 'fa fa-spinner fa-spin');
        
        <%= this.Page.ClientScript.GetCallbackEventReference(this, "orderid + '_3rd'", "loadingThirdMan", "orderid") %>;
    }
    
    function showPaymentDetails(sender, orderid) {
        $(sender).find('.pPaymentDetails').attr('class', 'fa fa-spinner fa-spin');
        
        <%= this.Page.ClientScript.GetCallbackEventReference(this, "orderid + '_details'", "loadingPaymentDetails", "orderid") %>;
    }
    
    function loadingThirdMan(msg, context) {
        $('#third_' + context).html(msg);
    }
    
    function loadingPaymentDetails(msg, context) {
        $('#details_' + context).html(msg);
    }
</script>