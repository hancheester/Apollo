using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Security;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_security_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<SecuritySettings>();
                txtSecretKey.Text = setting.SecretKey;
                txtIVKey.Text = setting.IVKey;
                cbDisableSSL.Checked = setting.DisableSSL;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<SecuritySettings>();
            setting.SecretKey = txtSecretKey.Text.Trim();
            setting.IVKey = txtIVKey.Text.Trim();
            setting.DisableSSL = cbDisableSSL.Checked;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Security settings were updated successfully.";
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