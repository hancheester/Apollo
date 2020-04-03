using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_widgets_google_analytics_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<GoogleAnalyticsSettings>();
                txtGoogleId.Text = setting.GoogleId;
                txtTrackingScript.Text = setting.TrackingScript;
                txtEcommerceScript.Text = setting.EcommerceScript;
                txtEcommerceDetailScript.Text = setting.EcommerceDetailScript;
                cbIncludingTax.Checked = setting.IncludingTax;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<GoogleAnalyticsSettings>();
            setting.GoogleId = txtGoogleId.Text.Trim();
            setting.TrackingScript = txtTrackingScript.Text.Trim();
            setting.EcommerceScript = txtEcommerceScript.Text.Trim();
            setting.EcommerceDetailScript = txtEcommerceDetailScript.Text.Trim();
            setting.IncludingTax = cbIncludingTax.Checked;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Google Analytics settings were updated successfully.";
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