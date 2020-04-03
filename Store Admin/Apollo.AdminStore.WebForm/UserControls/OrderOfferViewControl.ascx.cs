using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderOfferViewControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        public delegate void OfferEventHandler(decimal oldDiscount, decimal newDiscount);

        public event OfferEventHandler OfferChanged;

        public int OrderId
        {
            set
            {
                var order = OrderService.GetOrderOverviewModelById(value);
                string name = OrderService.GetOfferNameByOrderId(value);

                ltlName.Text = name;
                ltlDiscount.Text = AdminStoreUtility.GetFormattedPrice(order.DiscountValue, order.CurrencyCode, CurrencyType.HtmlEntity, order.ExchangeRate, 2);
                hfDiscount.Value = AdminStoreUtility.GetFormattedPrice(order.DiscountValue, order.CurrencyCode, CurrencyType.None, order.ExchangeRate, 2);
                txtDiscount.Text = hfDiscount.Value;
                ltlCurrencyCode.Text = order.CurrencyCode;
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            decimal oldDiscount = Convert.ToDecimal(hfDiscount.Value);
            decimal newDiscount = 0;
            decimal.TryParse(txtDiscount.Text.Trim(), out newDiscount);

            InvokeChanged(oldDiscount, newDiscount);
        }

        private void InvokeChanged(decimal oldDiscount, decimal newDiscount)
        {
            OfferEventHandler handler = OfferChanged;

            if (handler != null)
                handler(oldDiscount, newDiscount);
        }
    }
}