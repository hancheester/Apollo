using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderShippingNewControl : BaseUserControl
    {
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        private readonly List<string> _special_deliveries = new List<string> { "Next Day",
                                                                               "International Traceable",
                                                                               "Tracked" };

        public OrderOverviewModel OrderOverviewModel
        {
            set
            {
                var option = ShippingService.GetShippingOptionById(value.ShippingOptionId);

                if (option != null)
                {
                    if (_special_deliveries.Where(s => option.Name.Contains(s)).Any())
                    {
                        ltlShippingInfo.Text = string.Format("<div class='alert alert-warning'>{0} &#163;{1:0.00}</div>",
                                                             option.Name, value.ShippingCost);
                    }
                    else
                    {
                        ltlShippingInfo.Text = string.Format("{0} {1}{2:0.00}",
                                                             option.Name, value.CurrencyCode, value.ShippingCost);
                    }
                }
                else
                {
                    ltlShippingInfo.Text = "<div class='alert alert-danger'>No shipping option assigned!</div>";
                }

                ltlPackingInfo.Text = value.Packing;

                string currentOrderValue = AdminStoreUtility.GetFormattedPrice(value.GrandTotal, value.CurrencyCode, CurrencyType.HtmlEntity);
                string gbpOrderValue = string.Empty;

                if (value.CurrencyCode != CurrencySettings.PrimaryStoreCurrencyCode)
                    gbpOrderValue = string.Format("({0})", 
                        AdminStoreUtility.GetFormattedPrice(value.GrandTotal / value.ExchangeRate, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity));

                ltlOrderValue.Text = string.Format("{0} {1}", currentOrderValue, gbpOrderValue);
            }
        }

        public string Carrier
        {
            get { return ddlCarrier.SelectedValue; }
        }

        public string TrackingNumber
        {
            get { return txtTrackingNumber.Text.Trim(); }
        }

        public bool NotificationEmail
        {
            get { return cbNotificationEmail.Checked; }
        }

        public void Clear()
        {
            ddlCarrier.SelectedIndex = -1;
            txtTrackingNumber.Text = string.Empty;
        }
    }
}