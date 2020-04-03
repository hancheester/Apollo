using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderLoyaltyViewControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }
        public IAccountService AccountService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        public delegate void LoyaltyEventHandler(int oldAllocatedPoint, int newAllocatedPoint, int oldEarnedPoint, int newEarnedPoint);

        public event LoyaltyEventHandler LoyaltyChanged;

        public int OrderId
        {
            set
            {
                var order = OrderService.GetOrderOverviewModelById(value);

                ltlLoyalty.Text = string.Format("{0} ({1:0.00})",
                                                order.AllocatedPoint.ToString(),
                                                AdminStoreUtility.GetFormattedPrice(Convert.ToDecimal(order.AllocatedPoint) / 100M, order.CurrencyCode, CurrencyType.HtmlEntity, order.ExchangeRate));

                hfAllocatedPoint.Value = order.AllocatedPoint.ToString();
                txtLoyalty.Text = order.AllocatedPoint.ToString();
                
                if (order.ProfileId > 0)
                {
                    lblEarned.Text = order.EarnedPoint.ToString();
                    txtEarned.Text = order.EarnedPoint.ToString();
                }
                else
                {
                    lblEarned.Text = "0";
                    txtEarned.Text = "0";
                }
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            int oldAllocatedPoint = Convert.ToInt32(hfAllocatedPoint.Value);
            int newAllocatedPoint = Convert.ToInt32(txtLoyalty.Text);

            int oldEarnedPoint = Convert.ToInt32(lblEarned.Text);
            int newEarnedPoint = Convert.ToInt32(txtEarned.Text);
            
            InvokeChanged(oldAllocatedPoint, newAllocatedPoint, oldEarnedPoint, newEarnedPoint);
        }

        private void InvokeChanged(int oldAllocatedPoint, int newAllocatedPoint, int oldEarnedPoint, int newEarnedPoint)
        {
            LoyaltyEventHandler handler = LoyaltyChanged;

            if (handler != null)
                handler(oldAllocatedPoint, newAllocatedPoint, oldEarnedPoint, newEarnedPoint);
        }
    }
}
