using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class refund_info : BasePage
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(refund_info).FullName);

        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            LoadOrderInfo();
            base.OnInit(e);
        }
        
        protected void lbConfirm_Click(object sender, EventArgs e)
        {
            int refundId = QueryRefundInfoId;
            decimal refundAmount = Convert.ToDecimal(txtRefund.Text.Trim());

            int refundPoint = 0;
            if (!string.IsNullOrEmpty(txtPoint.Text.Trim())) refundPoint = Convert.ToInt32(txtPoint.Text.Trim());

            string reason = AdminStoreUtility.CleanHtml(txtComment.Text.Trim(), 100);
            var result = OrderService.ProcessOrderRefund(refundId, refundAmount, refundPoint, reason);
            string message = string.Empty;

            switch (result)
            {
                case OrderRefundResults.RefundNotFound:
                    message = "Refund could not be found.";
                    break;
                case OrderRefundResults.Error:
                    message = "Refund was failed to commit. Please look at related transaction logs or contact administrator for more information.";
                    break;
                case OrderRefundResults.Success:
                    message = "Refund was successfully committed.";
                    UpdateOrderComment(QueryOrderId, refundAmount, refundPoint);
                    break;
                case OrderRefundResults.SuccessButNoLoyaltyFound:
                    message = "Refund was successfully committed. However, loyalty point could not refunded as loyalty data could not be found.";
                    UpdateOrderComment(QueryOrderId, refundAmount, refundPoint, "<br/>However, loyalty point could not refunded as loyalty data could not be found.");
                    break;
                default:
                    break;
            }

            LoadTitle(refundId);
            eoiView.Refresh();
            enbNotice.Message = message;            
        }

        private void UpdateOrderComment(int orderId, decimal refundAmount, int refundPoint, string appendMessage = "")
        {
            var currencyCode = OrderService.GetCurrencyCodeByOrderId(orderId);

            // Save to comments
            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Confirm Refund",
                                                    "Refund was successfully committed.<br/>Amount <i>" + AdminStoreUtility.GetFormattedPrice(refundAmount, currencyCode, CurrencyType.Code) + "</i><br/>Point <i>" + refundPoint.ToString() + "</i>" + appendMessage,
                                                    txtComment.Text.Trim());
        }

        private void LoadTitle(int refundId)
        {
            var refund = OrderService.GetRefundById(QueryRefundInfoId);
            var account = AccountService.GetAccountByProfileId(refund.ProfileId);
            string fullName = string.Empty;

            if (account == null)
                _logger.WarnFormat("Account could not be loaded. Account ID={{{0}}}", refund.ProfileId);
            else
                fullName = account.Name;

            ltlTitle.Text = string.Format("Order # {0} by {1} | {2}", refund.OrderId, fullName, refund.IsCompleted ? "<b>Completed</b>" : "<b>Pending</b>");
        }

        private void LoadOrderInfo()
        {
            Refund refund = OrderService.GetRefundById(QueryRefundInfoId);

            if (refund == null)
            {
                _logger.ErrorFormat("Refund could not be found. Refund ID={{{0}}}", QueryRefundInfoId);
                Response.Redirect("/sales/refund_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderRefundNotFound);
            }

            var orderView = OrderService.GetOrderOverviewModelById(refund.OrderId);
            if (orderView == null)
            {
                _logger.ErrorFormat("Order could not be found. Order ID={{{0}}}", refund.OrderId);
                Response.Redirect("/sales/refund_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);
            }
            
            LoadTitle(refund.Id);

            SetState(ORDER_ID, orderView.Id);

            hlOrderView.NavigateUrl = string.Format("/sales/order_info.aspx?orderid={0}", orderView.Id);

            if (orderView.ProfileId != 0)
                SetState(USER_ID, orderView.ProfileId);
            else
                DisposeState(USER_ID);

            var orderItems = OrderService.GetLineItemOverviewModelListByOrderId(orderView.Id);
            decimal subTotal = orderItems
                                .Where(i => i.StatusCode != LineStatusCode.CANCELLED)
                                .Select(i => i.Quantity * i.PriceInclTax * i.ExchangeRate)
                                .Sum();
            decimal vat = OrderService.CalculateVATByOrderId(orderView.Id);
            decimal grandTotal = OrderService.CalculateOrderTotalByOrderId(orderView.Id);
            decimal refundTotal = OrderService.CalculateTotalRefundedAmountByOrderId(orderView.Id);
            decimal previousAmount = OrderService.CalculateTotalPaidAmountByOrderId(orderView.Id);

            ltlCurrencyCode.Text = orderView.CurrencyCode;
            ltlPaidAmount.Text = orderView.Paid ? AdminStoreUtility.GetFormattedPrice(previousAmount - refundTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity) : AdminStoreUtility.GetFormattedPrice(0M, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlShippingAmount.Text = AdminStoreUtility.GetFormattedPrice(orderView.ShippingCost, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlOrderGrandTotal.Text = AdminStoreUtility.GetFormattedPrice(grandTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlRefundedAmount.Text = AdminStoreUtility.GetFormattedPrice(refundTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);

            txtRefund.Text = AdminStoreUtility.GetFormattedPrice(refund.ValueToRefund * refund.ExchangeRate, refund.CurrencyCode, CurrencyType.None);
            txtPoint.Text = refund.PointToRefund.ToString();
            txtComment.Text = refund.Reason;

            if (refund.IsCompleted)
            {
                txtRefund.Enabled = false;
                txtPoint.Enabled = false;
                lbConfirm.Enabled = false;
                lbConfirm.Text = "Completed";
            }
        }
    }
}