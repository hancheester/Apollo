using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Email
{
    public class EmailSettings : ISettings
    {
        public string AccountRegisterEmailTemplateLocalPath { get; set; }
        public string AccountRegisterWithPasswordEmailTemplateLocalPath { get; set; }
        public string PasswordRetrievalEmailTemplateLocalPath { get; set; }
        public string NewUsernameEmailTemplateLocalPath { get; set; }
        public string DespatchConfirmationEmailTemplateLocalPath { get; set; }
        public string OrderConfirmationEmailTemplateLocalPath { get; set; }
        public string PaymentInvoiceEmailTemplateLocalPath { get; set; }
        public string PaymentInvoiceConfirmationEmailTemplateLocalPath { get; set; }
        public string StockNotificationEmailTemplateLocalPath { get; set; }
        public string ContactUsEmail { get; set; }
        public string EmailHost { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public string EmailFrom { get; set; }
        public string EmailDisplayFrom { get; set; }
        public string EmailBCC { get; set; }
    }
}
