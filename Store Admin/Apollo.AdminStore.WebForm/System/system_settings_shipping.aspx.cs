using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_shipping : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<ShippingSettings>();
                txtPrimaryStoreCountryId.Text = setting.PrimaryStoreCountryId.ToString();
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<ShippingSettings>();
            setting.PrimaryStoreCountryId = Convert.ToInt32(txtPrimaryStoreCountryId.Text.Trim());

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Shipping settings were updated successfully.";
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