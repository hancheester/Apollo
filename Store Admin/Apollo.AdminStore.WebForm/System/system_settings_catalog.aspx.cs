using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Catalog;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_catalog : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<CatalogSettings>();
                txtPhoneOrderMessage.Text = setting.PhoneOrderMessage;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<CatalogSettings>();
            setting.PhoneOrderMessage = txtPhoneOrderMessage.Text.Trim();

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Catalog settings were updated successfully.";
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