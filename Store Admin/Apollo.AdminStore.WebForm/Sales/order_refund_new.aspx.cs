﻿using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_refund_new : BasePage
    {
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            LoadOrderInfo();
            base.OnInit(e);
        }

        private void LoadOrderInfo()
        {
            int orderId = QueryOrderId;
            var orderView = OrderService.GetOrderOverviewModelById(orderId);

            if (orderView == null)
                Response.Redirect("/sales/order_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);

            ltlTitle.Text = string.Format("<h3 class='printHide'>Order # {0}</h3>", orderId);

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
            ltlPaidAmount.Text = AdminStoreUtility.GetFormattedPrice(previousAmount - refundTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlShippingAmount.Text = AdminStoreUtility.GetFormattedPrice(orderView.ShippingCost, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlOrderGrandTotal.Text = AdminStoreUtility.GetFormattedPrice(grandTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
            ltlRefundedAmount.Text = AdminStoreUtility.GetFormattedPrice(refundTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
        }

        protected void lbSubmit_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            decimal refundValue = Convert.ToDecimal(txtRefund.Text.Trim());
            decimal refunded = OrderService.CalculateTotalRefundedAmountByOrderId(orderId);
            int pointValue = Convert.ToInt32(txtPoint.Text.Trim());
            decimal orderTotal = OrderService.CalculateOrderTotalByOrderId(orderId);
            decimal previousAmount = OrderService.CalculateTotalPaidAmountByOrderId(orderId);

            // As user enters refund value in 2 decimal points, order total should be rounded to 2 decimal points too.
            orderTotal = AdminStoreUtility.RoundPrice(orderTotal, places: 2);
            
            if (refundValue == 0M || (refundValue <= (orderTotal - refunded)) || refundValue <= (previousAmount - refunded))
            {
                var orderView = OrderService.GetOrderOverviewModelById(orderId);
                var reason = txtComment.Text.Trim();
                var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
                var refund = new Refund
                {
                    OrderId = orderId,
                    PointToRefund = Convert.ToInt32(txtPoint.Text.Trim()),
                    CurrencyCode = orderView.CurrencyCode,
                    ExchangeRate = orderView.ExchangeRate,
                    ValueToRefund = refundValue / orderView.ExchangeRate,
                    Reason = reason,
                    ProfileId = profileId,
                    IsCancellation = false,
                    IsCompleted = false,
                    DateStamp = DateTime.Now
                };

                var refundId = OrderService.ProcessRefundInsertion(refund);

                // Save to comments
                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                        "Submit Pending Refund",
                                                        "Refund was successfully scheduled.<br/>Amount <i>" + AdminStoreUtility.GetFormattedPrice(refundValue, orderView.CurrencyCode, CurrencyType.Code) + "</i><br/><Point <i>" + pointValue.ToString() + "</i>",
                                                        reason);

                txtRefund.Text = "0";
                txtPoint.Text = "0";
                txtComment.Text = string.Empty;
                
                LoadOrderInfo();
                enbNotice.Message = string.Format("Refund was successfully scheduled. Please <a href='/sales/refund_info.aspx?refundinfoid={0}&orderid={1}' target='_blank'>click here</a> to view refund information.", refundId, orderId);
            }
            else
                enbNotice.Message = "Refund was FAILED to create. Refund amount cannot be more than paid amount.";
        }
    }
}