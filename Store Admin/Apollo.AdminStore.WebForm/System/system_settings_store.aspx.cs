using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_store : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<StoreInformationSettings>();
                txtCompanyName.Text = setting.CompanyName;
                txtStoreFrontLink.Text = setting.StoreFrontLink;
                txtStoreFrontSecuredLink.Text = setting.StoreFrontSecuredLink;
                txtTermURL.Text = setting.TermURL;
                cbDisplayEuCookieLawWarning.Checked = setting.DisplayEuCookieLawWarning;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<StoreInformationSettings>();
            setting.CompanyName = txtCompanyName.Text.Trim();
            setting.StoreFrontLink = txtStoreFrontLink.Text.Trim();
            setting.StoreFrontSecuredLink = txtStoreFrontSecuredLink.Text.Trim();
            setting.TermURL = txtTermURL.Text.Trim();
            setting.DisplayEuCookieLawWarning = cbDisplayEuCookieLawWarning.Checked;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Store information settings were updated successfully.";
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