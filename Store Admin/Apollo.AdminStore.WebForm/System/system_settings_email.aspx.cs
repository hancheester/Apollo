using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Email;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_email : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<EmailSettings>();
                txtAccountRegisterEmailTemplateLocalPath.Text = setting.AccountRegisterEmailTemplateLocalPath;
                txtAccountRegisterWithPasswordEmailTemplateLocalPath.Text = setting.AccountRegisterWithPasswordEmailTemplateLocalPath;
                txtPasswordRetrievalEmailTemplateLocalPath.Text = setting.PasswordRetrievalEmailTemplateLocalPath;
                txtNewUsernameEmailTemplateLocalPath.Text = setting.NewUsernameEmailTemplateLocalPath;
                txtDespatchConfirmationEmailTemplateLocalPath.Text = setting.DespatchConfirmationEmailTemplateLocalPath;
                txtOrderConfirmationEmailTemplateLocalPath.Text = setting.OrderConfirmationEmailTemplateLocalPath;
                txtPaymentInvoiceEmailTemplateLocalPath.Text = setting.PaymentInvoiceEmailTemplateLocalPath;
                txtPaymentInvoiceConfirmationEmailTemplateLocalPath.Text = setting.PaymentInvoiceConfirmationEmailTemplateLocalPath;
                txtContactUsEmail.Text = setting.ContactUsEmail;
                txtEmailHost.Text = setting.EmailHost;
                txtEmailUsername.Text = setting.EmailUsername;
                txtEmailPassword.Text = setting.EmailPassword;
                txtEmailFrom.Text = setting.EmailFrom;
                txtEmailDisplayFrom.Text = setting.EmailDisplayFrom;
                txtEmailBCC.Text = setting.EmailBCC;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<EmailSettings>();
            setting.AccountRegisterEmailTemplateLocalPath = txtAccountRegisterEmailTemplateLocalPath.Text;
            setting.AccountRegisterWithPasswordEmailTemplateLocalPath = txtAccountRegisterWithPasswordEmailTemplateLocalPath.Text;
            setting.PasswordRetrievalEmailTemplateLocalPath = txtPasswordRetrievalEmailTemplateLocalPath.Text;
            setting.NewUsernameEmailTemplateLocalPath = txtNewUsernameEmailTemplateLocalPath.Text;
            setting.DespatchConfirmationEmailTemplateLocalPath = txtDespatchConfirmationEmailTemplateLocalPath.Text;
            setting.OrderConfirmationEmailTemplateLocalPath = txtOrderConfirmationEmailTemplateLocalPath.Text;
            setting.PaymentInvoiceEmailTemplateLocalPath = txtPaymentInvoiceEmailTemplateLocalPath.Text;
            setting.PaymentInvoiceConfirmationEmailTemplateLocalPath = txtPaymentInvoiceConfirmationEmailTemplateLocalPath.Text;
            setting.ContactUsEmail = txtContactUsEmail.Text;
            setting.EmailHost = txtEmailHost.Text;
            setting.EmailUsername = txtEmailUsername.Text;
            setting.EmailPassword = txtEmailPassword.Text;
            setting.EmailFrom = txtEmailFrom.Text;
            setting.EmailDisplayFrom = txtEmailDisplayFrom.Text;
            setting.EmailBCC = txtEmailBCC.Text;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Email settings were updated successfully.";
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