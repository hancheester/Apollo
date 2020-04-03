using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Orders;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_shoppingcart : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<ShoppingCartSettings>();
                txtMaxPharmaceuticalProduct.Text = setting.MaxPharmaceuticalProduct.ToString();
                txtLoyaltyRate.Text = setting.LoyaltyRate.ToString();
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<ShoppingCartSettings>();
            setting.MaxPharmaceuticalProduct = Convert.ToInt32(txtMaxPharmaceuticalProduct.Text.Trim());
            setting.LoyaltyRate = Convert.ToInt32(txtLoyaltyRate.Text.Trim());

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Shopping cart settings were updated successfully.";
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Setting);

            if (result)
                enbNotice.Message = "All settings related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }
    }
}