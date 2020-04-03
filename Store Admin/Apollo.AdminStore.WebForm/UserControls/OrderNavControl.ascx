<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderNavControl" Codebehind="OrderNavControl.ascx.cs" %>
<div class="row wrapper hidden-print">
    <div class="col-lg-12">
        <div class="row"><p></p></div>
        <div class="row">
            <div class="pull-right">
                <asp:HyperLink ID="hlBack" CssClass="btn btn-sm btn-default" runat="server" Text="Back"></asp:HyperLink>
                <asp:PlaceHolder ID="phAuthorise" runat="server">
                    <asp:LinkButton ID="lbAuthorise" OnClick="lbAuthorise_Click" runat="server" OnClientClick="javascript:return confirm('Are you sure to authorise this order?');" CssClass="btn btn-sm btn-warning" Text="Authorise"></asp:LinkButton>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phPayment" runat="server">
                    <a href="/sales/order_payment_new.aspx?orderid=<%= QueryOrderId %>" class="btn btn-sm btn-primary">Payment</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phInvoice" runat="server">
                    <a href="/sales/order_invoice_new.aspx?orderid=<%= QueryOrderId %>" class="btn btn-sm btn-success">Invoice</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phCancel" runat="server">
                    <a href="/sales/order_cancel_new.aspx?orderid=<%= QueryOrderId %>" class="btn btn-sm btn-danger">Cancel</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phRefund" runat="server">
                    <a href="/sales/order_refund_new.aspx?orderid=<%= QueryOrderId %>" class="btn btn-sm btn-info">Refund</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phMarkFraud" runat="server">
                    <asp:LinkButton ID="lbMarkFraud" OnClick="lbMarkFraud_Click" runat="server" OnClientClick="javascript:return confirm('Are you sure to mark this as fraudulent?');" CssClass="btn btn-sm btn-danger" Text="Mark fraudulent"></asp:LinkButton>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phSendOrderEmail" runat="server">
                    <asp:LinkButton Text="Send order email" runat="server" CssClass="btn btn-sm btn-warning" ID="lbSendOrderEmail" OnClick="lbSendOrderEmail_Click"></asp:LinkButton>
                </asp:PlaceHolder> 
                <asp:PlaceHolder ID="phDeleteCancellation" runat="server">
                    <asp:LinkButton ID="btnDeleteCancellation" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete?');" CssClass="btn btn-sm btn-info"  OnClick="btnDeleteCancellation_Click"/>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phDeleteRefund" runat="server">
                    <asp:LinkButton ID="btnDeleteRefund" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete?');" CssClass="btn btn-sm btn-success"  OnClick="btnDeleteRefund_Click"/>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phPrintInvoice" runat="server">
                    <asp:LinkButton ID="lbPrintInvoice" runat="server" Text="Print Invoice" CssClass="btn btn-sm btn-primary" OnClick="lbPrintInvoice_Click"/>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</div>

