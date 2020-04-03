using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.SagePay;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_plugins_sagepay_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<SagePaySettings>();
                txtSagePay3DSecureCallbackLink.Text = setting.SagePay3DSecureCallbackLink;
                txtSagePayPaymentGatewayLink.Text = setting.SagePayPaymentGatewayLink;
                txtSagePayRegisterRefundLink.Text = setting.SagePayRegisterRefundLink;
                txtSagePayRegisterAbortLink.Text = setting.SagePayRegisterAbortLink;
                txtSagePayRegisterReleaseLink.Text = setting.SagePayRegisterReleaseLink;
                txtSagePayRegisterRepeatLink.Text = setting.SagePayRegisterRepeatLink;
                txtSagePayReportingAdminAPILink.Text = setting.SagePayReportingAdminAPILink;
                txtSagePayVPSProtocol.Text = setting.SagePayVPSProtocol;
                txtSagePayVendor.Text = setting.SagePayVendor;
                txtSagePayWebUser.Text = setting.SagePayWebUser;
                txtSagePayWebPwd.Text = setting.SagePayWebPwd;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<SagePaySettings>();            
            setting.SagePay3DSecureCallbackLink = txtSagePay3DSecureCallbackLink.Text.Trim();
            setting.SagePayPaymentGatewayLink = txtSagePayPaymentGatewayLink.Text.Trim();
            setting.SagePayRegisterRefundLink = txtSagePayRegisterRefundLink.Text.Trim();
            setting.SagePayRegisterAbortLink = txtSagePayRegisterAbortLink.Text.Trim();
            setting.SagePayRegisterReleaseLink = txtSagePayRegisterReleaseLink.Text.Trim();
            setting.SagePayRegisterRepeatLink = txtSagePayRegisterRepeatLink.Text.Trim();
            setting.SagePayReportingAdminAPILink = txtSagePayReportingAdminAPILink.Text.Trim();
            setting.SagePayVPSProtocol = txtSagePayVPSProtocol.Text.Trim();
            setting.SagePayVendor = txtSagePayVendor.Text.Trim();
            setting.SagePayWebUser = txtSagePayWebUser.Text.Trim();
            setting.SagePayWebPwd = txtSagePayWebPwd.Text.Trim();

            SettingService.SaveSetting(setting);

            enbNotice.Message = "SagePay settings were updated successfully.";
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
