using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderNavControl : BaseUserControl
    {
        public delegate void OrderNavEventHandler(string message, bool refresh);
        public event OrderNavEventHandler ActionOccurred;

        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public IPaymentService PaymentService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        
        public OrderNavType Type { get; set; }
        
        protected int QueryOrderId
        {
            get { return ((BasePage)Page).QueryOrderId; }
        }

        protected int QueryRefundInfoId
        {
            get { return ((BasePage)Page).QueryRefundInfoId; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadButtons();
        }

        protected void lbMarkFraud_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;

            // Assign issue IC3 'System Check Failed' to this order
            OrderService.UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.SYSTEM_CHECK_FAILED);

            // Assign status 'On Hold' to this order
            OrderService.UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);

            // Assign status 'Ordered' to these line items
            OrderService.UpdateLineItemStatusCodeByOrderId(orderId, LineStatusCode.ORDERED);

            // Retrieve information and put into system check database. All system check items should be true now.
            OrderService.SetAllSystemCheckItemsByOrderId(orderId, true);

            var systemCheck = OrderService.GetSystemCheckByOrderId(orderId);

            if (systemCheck == null)
            {
                systemCheck = new SystemCheck()
                {
                    OrderId = orderId
                };

                OrderService.InsertSystemCheck(systemCheck);
            }

            // Process system checking again
            OrderService.ProcessSystemChecking(orderId);

            // Save to comments
            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Mark Fraudulent",
                                                    "Order issue (System check failed) was updated successfully. Information from this order has been used to update system checking data store.",
                                                    string.Empty);
            
            // Display message
            InvokeNewMessage("Order issue was updated successfully. Information from this order has been used to update system checking data store.", true);

            // Load this page again
            LoadButtons();
        }

        protected void lbAuthorise_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            var orderView = OrderService.GetOrderOverviewModelById(orderId);
            decimal totalRefunded = OrderService.CalculateTotalRefundedAmountByOrderId(orderId);
            decimal totalPaidPayments = OrderService.CalculateTotalPaidAmountByOrderId(orderId);
            decimal totalOrder = OrderService.CalculateOrderTotalByOrderId(orderId);
            decimal totalPending = totalOrder - totalRefunded - totalPaidPayments;

            OrderPayment payment = new OrderPayment
            {
                OrderId = orderId,
                Amount = totalPending / orderView.ExchangeRate,
                CurrencyCode = orderView.CurrencyCode,
                ExchangeRate = orderView.ExchangeRate,
                TimeStamp = DateTime.Now
            };
            
            var output = PaymentService.ProcessOrderCharging(payment);
            var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            var fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

            if (output.Status)
            {
                var message = " Release amount: " + AdminStoreUtility.GetFormattedPrice(totalPending, payment.CurrencyCode.ToUpper(), CurrencyType.Code);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Payment",
                                                        "Payment was authorised successfully. " + message,
                                                        string.Empty);
                
                // Display message and refresh
                InvokeNewMessage("Order was charged successfully.", true);
            }
            else
            {
                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Payment",
                                                        "Payment was FAILED to authorise. " + output.Message,
                                                        string.Empty);

                OrderService.UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.FAILED_TO_CHARGE);

                InvokeNewMessage("Order was FAILED to charge. " + output.Message, true);
            }
        }

        protected void lbSendOrderEmail_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            var profileId = OrderService.GetProfileIdByOrderId(orderId);
            var account = AccountService.GetAccountOverviewModelByProfileId(profileId);

            if (account == null)
            {
                // Display message and refresh page
                InvokeNewMessage("No account was found with this profile. Order email was not sent.", true);
            }
            else
            {
                OrderService.SendOrderConfirmationEmail(orderId, account.Name, account.Email);

                // Save to comments
                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                        Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                        "Send Order Email",
                                                        "Order email was sent successfully.",
                                                        string.Empty);

                // Display message and refresh page
                InvokeNewMessage("Order email was sent successfully.", true);
            }
        }

        protected void lbPrintInvoice_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            byte[] bytes = OrderService.PrintOrderToInvoicePdf(orderId);

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("order_{0}.pdf", orderId));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(bytes);
            Response.End();
        }

        protected void btnDeleteCancellation_Click(object sender, EventArgs e)
        {
            string presentUrl = HttpContext.Current.Request.Url.AbsolutePath;
            OrderService.DeleteRefund(QueryRefundInfoId);
            Response.Redirect("/sales/cancellation_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderCancellationDeleted);
        }

        protected void btnDeleteRefund_Click(object sender, EventArgs e)
        {
            string presentUrl = HttpContext.Current.Request.Url.AbsolutePath;
            OrderService.DeleteRefund(QueryRefundInfoId);
            Response.Redirect("/sales/refund_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderRefundDeleted);
        }

        private void LoadButtons()
        {
            switch (Type)
            {
                case OrderNavType.Order:
                    int orderId = QueryOrderId;
                    bool fullyPaid = OrderService.HasFullyPaid(orderId);

                    if (!fullyPaid)
                    {
                        decimal totalRefunded = OrderService.CalculateTotalRefundedAmountByOrderId(orderId);
                        decimal totalPaidPayments = OrderService.CalculateTotalPaidAmountByOrderId(orderId);
                        decimal totalOrder = OrderService.CalculateOrderTotalByOrderId(orderId);
                        decimal totalPending = totalOrder - totalRefunded - totalPaidPayments;
                        var currencyCode = OrderService.GetCurrencyCodeByOrderId(orderId);

                        totalPending = AdminStoreUtility.RoundPrice(totalPending, places: 2);
                        if (totalPending > 0M)
                            lbAuthorise.OnClientClick = string.Format("javascript: return confirm('Total amount to be charged is {0}{1:0.00}.\\nAre you sure to authorise this order?');", currencyCode, totalPending);
                        else
                            lbAuthorise.Enabled = false;
                    }

                    lbAuthorise.Enabled = !fullyPaid;
                    hlBack.NavigateUrl = "/sales/order_default.aspx";

                    phDeleteCancellation.Visible = false;
                    phDeleteRefund.Visible = false;
                    break;
                case OrderNavType.Refund:
                    hlBack.NavigateUrl = "/sales/refund_default.aspx";

                    phAuthorise.Visible = false;
                    phPayment.Visible = false;
                    phInvoice.Visible = false;
                    phCancel.Visible = false;
                    phRefund.Visible = false;
                    phMarkFraud.Visible = false;
                    phSendOrderEmail.Visible = false;
                    phDeleteCancellation.Visible = false;
                    phPrintInvoice.Visible = false;
                    break;
                case OrderNavType.Cancellation:
                    hlBack.NavigateUrl = "/sales/cancellation_default.aspx";

                    phAuthorise.Visible = false;
                    phPayment.Visible = false;
                    phInvoice.Visible = false;
                    phCancel.Visible = false;
                    phRefund.Visible = false;
                    phMarkFraud.Visible = false;
                    phSendOrderEmail.Visible = false;
                    phDeleteRefund.Visible = false;
                    phPrintInvoice.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void InvokeNewMessage(string message, bool refresh)
        {
            OrderNavEventHandler handler = ActionOccurred;
            if (handler != null)
                handler(message, refresh);
        }
    }
}