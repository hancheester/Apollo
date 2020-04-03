using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_currency : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<CurrencySettings>();
                txtPrimaryStoreCurrencyId.Text = setting.PrimaryStoreCurrencyId.ToString();
                txtPrimaryStoreCurrencyCode.Text = setting.PrimaryStoreCurrencyCode;
                txtExchangeRateProviderLink.Text = setting.ExchangeRateProviderLink;
                txtExchangeRateFactor.Text = setting.ExchangeRateFactor.ToString();
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<CurrencySettings>();
            setting.PrimaryStoreCurrencyId = Convert.ToInt32(txtPrimaryStoreCurrencyId.Text.Trim());
            setting.PrimaryStoreCurrencyCode = txtPrimaryStoreCurrencyCode.Text.Trim();
            setting.ExchangeRateProviderLink = txtExchangeRateProviderLink.Text.Trim();
            setting.ExchangeRateFactor = Convert.ToDecimal(txtExchangeRateFactor.Text.Trim());

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Currency settings were updated successfully.";
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
