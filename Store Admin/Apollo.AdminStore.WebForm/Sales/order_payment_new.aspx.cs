using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_payment_new : BasePage
    {
        public IOrderService OrderService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var orderView = OrderService.GetOrderOverviewModelById(QueryOrderId);

            if (orderView != null)
            {
                ltlTitle.Text = string.Format("<h3 class='printHide'>Order # {0}</h3>", orderView.Id.ToString());
            }
            else
                Response.Redirect("/sales/order_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);
        }

        protected void ddlCurrency_OnInit(object sender, EventArgs e)
        {
            ddlCurrency.DataSource = UtilityService.GetAllCurrency();
            ddlCurrency.DataBind();

            var orderView = OrderService.GetOrderOverviewModelById(QueryOrderId);

            if (orderView != null)
            {
                ddlCurrency.Items.FindByValue(orderView.CurrencyCode).Selected = true;
                ddlCurrency.Enabled = false;
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var orderView = OrderService.GetOrderOverviewModelById(QueryOrderId);
            var amount = Convert.ToDecimal(txtAmount.Text.Trim());
            var currencyCode = ddlCurrency.SelectedValue.ToUpper();
            var paymentReference = txtPaymentRef.Text.Trim();

            OrderPayment payment = new OrderPayment
            {
                OrderId = orderView.Id,
                PaymentReference = paymentReference,
                Amount = amount / orderView.ExchangeRate,
                CurrencyCode = currencyCode,
                ExchangeRate = orderView.ExchangeRate,
                TimeStamp = DateTime.Now,
                IsCompleted = true
            };
            
            OrderService.InsertOrderPayment(payment);

            // Save to comments
            OrderService.ProcessOrderCommentInsertion(orderView.Id,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Submit Payment",
                                                    string.Format("Payment was saved successfully.<br/>Amount <i>{0}{1:0.00}</i><br/>Payment Reference <i>{2}</i>", currencyCode, amount, paymentReference),
                                                    string.Empty);

            txtAmount.Text = "0";
            txtPaymentRef.Text = string.Empty;
            
            enbNotice.Message = "Order payment was saved successfully.";
        }
    }
}