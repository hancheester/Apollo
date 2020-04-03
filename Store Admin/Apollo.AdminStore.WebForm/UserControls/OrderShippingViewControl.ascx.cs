using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderShippingViewControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        public delegate void ShippingEventHandler(string oldNote, string newNote, decimal oldCost, decimal newCost, int oldShippingId, int newShippingId);

        public event ShippingEventHandler ShippingChanged;

        public int OrderId
        {
            set
            {
                int orderId = value;
                
                var shipping = OrderService.GetShippingOverviewModelByOrderId(orderId);
                var order = OrderService.GetOrderOverviewModelById(value);
                var country = ShippingService.GetCountryById(shipping.ShippingCountryId);
                var shippingOptionId = Convert.ToInt32(shipping.ShippingOptionId);
                var shippingOption = ShippingService.GetShippingOptionById(shippingOptionId);
                var orderItems = OrderService.GetLineItemOverviewModelListByOrderId(value);

                if (country != null)
                {
                    if (country.IsEC)
                        ltlVATMessage.Text = "included";
                    else
                        ltlVATMessage.Text = "excluded";
                }

                if (country != null)
                {
                    ddlOptions.DataSource = ShippingService.GetShippingOptionByCountryAndEnabled(country.Id, true);
                    ddlOptions.DataBind();

                    ListItem found = ddlOptions.Items.FindByValue(shipping.ShippingOptionId.ToString());

                    if (found != null)
                        found.Selected = true;

                    hfCurrentId.Value = shipping.ShippingOptionId.ToString();
                }

                if (shippingOption != null)
                {
                    ltlShippingInfo.Text = shippingOption.Description;
                }
                else
                {
                    ltlShippingInfo.Text = "<div class='alert alert-danger'>No shipping option assigned!</div>";
                }

                ltlCost.Text = AdminStoreUtility.GetFormattedPrice(shipping.ShippingCost, order.CurrencyCode, CurrencyType.HtmlEntity, order.ExchangeRate);
                txtCost.Text = shipping.ShippingCost.ToString();
                hfCost.Value = shipping.ShippingCost.ToString();
                ltlCurrencyCode.Text = order.CurrencyCode;

                decimal vat = OrderService.CalculateVATByOrderId(orderId);
                ltlTaxDiscount.Text = AdminStoreUtility.GetFormattedPrice(vat, order.CurrencyCode, CurrencyType.HtmlEntity);

                ltlWeight.Text = orderItems.Select(i => i.Weight * i.Quantity).Sum() + " g";

                if (order.CurrencyCode != CurrencySettings.PrimaryStoreCurrencyCode)
                {
                    ltlCost.Text = ltlCost.Text + " (" + AdminStoreUtility.GetFormattedPrice(shipping.ShippingCost, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) + ")";
                    ltlTaxDiscount.Text = ltlTaxDiscount.Text + " (" + AdminStoreUtility.GetFormattedPrice(vat / order.ExchangeRate, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) + ")";
                }

                ltlPackingInfo.Text = shipping.Packing;
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            decimal oldCost = Convert.ToDecimal(hfCost.Value);
            decimal newCost = Convert.ToDecimal(txtCost.Text.Trim());
            int oldShippingId = Convert.ToInt32(hfCurrentId.Value);
            int newShippingId = Convert.ToInt32(ddlOptions.SelectedValue);

            InvokeChanged(ltlPackingInfo.Text, txtNote.Text.Trim(), oldCost, newCost, oldShippingId, newShippingId);
        }

        private void InvokeChanged(string oldNote, string newNote, decimal oldCost, decimal newCost, int oldShippingOptionId, int newShippingOptionId)
        {
            ShippingEventHandler handler = ShippingChanged;

            if (handler != null)
                handler(oldNote, newNote, oldCost, newCost, oldShippingOptionId, newShippingOptionId);
        }
    }
}