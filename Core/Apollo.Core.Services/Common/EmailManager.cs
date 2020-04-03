using Apollo.Core.Domain.Email;
using Apollo.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Apollo.Core.Services.Common
{
    public class EmailManager : IEmailManager
    {
        private ILogger _logger;
        private EmailSettings _emailSettings;
       
        public EmailManager(ILogBuilder logBuilder, EmailSettings emailSettings)
        {
            _logger = logBuilder.CreateLogger(typeof(EmailManager).FullName);
        }

        public void SendEmailAsync(string email, string subject, string body)
        {
            // TODO: To make it as a single thread
            SendEmail(email, subject, body);
        }

        public void SendAccountRegistrationEmail(string email, string firstName)
        {
            string body = string.Empty;
            string accountRegisterEmailPath = _emailSettings.AccountRegisterEmailTemplateLocalPath;
            using (StreamReader sr = new StreamReader(accountRegisterEmailPath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", firstName.Trim());
            }

            SendEmailAsync(email, "Welcome to Apollo.co.uk", body);
        }

        public void SendAccountRegistrationWithPasswordEmail(string email, string firstName, string password)
        {
            string body = string.Empty;
            string accountRegisterWithPasswordEmailPath = _emailSettings.AccountRegisterWithPasswordEmailTemplateLocalPath;
            using (StreamReader sr = new StreamReader(accountRegisterWithPasswordEmailPath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", firstName);
                body = body.Replace("@@ResetPassword", password);
            }

            SendEmailAsync(email, "Welcome to Apollo.co.uk", body);
        }

        public void SendPasswordRetrievalEmail(string email, string firstName, string password)
        {
            string body = string.Empty;
            string passwordRetrievalEmailPath = _emailSettings.PasswordRetrievalEmailTemplateLocalPath;
            using (StreamReader sr = new StreamReader(passwordRetrievalEmailPath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", firstName);
                body = body.Replace("@@ResetPassword", password);
            }

            SendEmailAsync(email, "Password Retrieval", body);
        }

        public void SendNewUsernameEmail(string email, string firstName)
        {
            string body = string.Empty;
            string newUsernameEmailPath = _emailSettings.NewUsernameEmailTemplateLocalPath;
            using (StreamReader sr = new StreamReader(newUsernameEmailPath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", firstName);
                body = body.Replace("@@NewEmail", email.ToLower());
            }

            SendEmailAsync(email, "New E-mail Address", body);
        }

        public void SendPaymentInvoiceConfirmationEmail(string email, string name, string amount)
        {
            var paymentInvoiceConfirmationEmailPath = _emailSettings.PaymentInvoiceConfirmationEmailTemplateLocalPath;
            string body;
            using (StreamReader sr = new StreamReader(paymentInvoiceConfirmationEmailPath))
            {
                body = sr.ReadToEnd();

                body = body.Replace("@@Name", name);
                body = body.Replace("@@Amount", amount);
            }

            SendEmailAsync(email, "Payment Confirmation", body);
        }

        public void SendDespatchConfirmationEmail(string email, string name, string orderId, string items)
        {
            var emailTemplatePath = _emailSettings.DespatchConfirmationEmailTemplateLocalPath;
            string body;
            using (StreamReader sr = new StreamReader(emailTemplatePath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", name);
                body = body.Replace("@@OrderNumber", orderId);
                body = body.Replace("@@Items", items);
            }

            SendEmailAsync(email, "Your online order with Apollo.co.uk has been despatched.", body);
        }

        public void SendPaymentInvoiceEmail(string email, string name, string message, string amount, string payLink)
        {
            var emailTemplatePath = _emailSettings.PaymentInvoiceEmailTemplateLocalPath;
            string body;
            using (StreamReader sr = new StreamReader(emailTemplatePath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@Name", name);
                body = body.Replace("@@Message", message);
                body = body.Replace("@@Amount", amount);
                body = body.Replace("@@PayLink", payLink);
            }

            SendEmailAsync(email, "Payment Request", body);
        }

        public void SendBackInStockEmail(string email, string productName, string productUrl, string option, string productImage)
        {
            var emailTemplatePath = _emailSettings.StockNotificationEmailTemplateLocalPath;
            string body;
            using (StreamReader sr = new StreamReader(emailTemplatePath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@ProductName", productName);
                body = body.Replace("@@ProductUrl", productUrl);
                body = body.Replace("@@ProductOption", option);
                body = body.Replace("@@ProductImage", productImage);
            }

            SendEmailAsync(email, "Back In Stock!", body);
        }

        public void SendOrderConfirmationEmail(
            string email, 
            string name, 
            string orderId, 
            string billingAddress, 
            string shippingAddress, 
            string shippingOption, 
            string items,
            string itemTotal,
            string discountTitle,
            string discount,
            string loyaltyPointTitle,
            string loyaltyPoint,            
            string vat,
            string shippingCost,
            string total,
            string nhsNotice)
        {
            var emailTemplatePath = _emailSettings.OrderConfirmationEmailTemplateLocalPath;
            string body;
            using (StreamReader sr = new StreamReader(emailTemplatePath))
            {
                body = sr.ReadToEnd();
                body = body.Replace("@@FirstName", name);
                body = body.Replace("@@OrderNumber", orderId);
                body = body.Replace("@@BillingAddress", billingAddress);
                body = body.Replace("@@ShippingAddress", shippingAddress);
                body = body.Replace("@@ShippingOption", shippingOption);
                body = body.Replace("@@Items", items);
                body = body.Replace("@@ItemTotal", itemTotal);
                body = body.Replace("@@DiscountTitle", discountTitle);
                body = body.Replace("@@Discount", discount);
                body = body.Replace("@@LoyaltyPointTitle", loyaltyPointTitle);
                body = body.Replace("@@LoyaltyPoint", loyaltyPoint);
                body = body.Replace("@@VAT", vat);
                body = body.Replace("@@ShippingCost", shippingCost);
                body = body.Replace("@@Total", total);
                body = body.Replace("@@NHSNotice", nhsNotice);
            }

            SendEmailAsync(email, "Order Confirmation", body);
        }

        private void SendEmail(string emailTo, string subject, string body)
        {
            var emailHost = _emailSettings.EmailHost;
            var emailUserName = _emailSettings.EmailUsername;
            var emailPassword = _emailSettings.EmailPassword;
            var emailFrom = _emailSettings.EmailFrom;
            var emailDisplayFrom = _emailSettings.EmailDisplayFrom;
            var emailBcc = _emailSettings.EmailBCC;

            MailMessage mmsMessage = new MailMessage();
            SmtpClient scSender = new SmtpClient();

            mmsMessage.Subject = subject;
            mmsMessage.From = new MailAddress(emailFrom, emailDisplayFrom);
            mmsMessage.To.Add(emailTo);

            if (string.IsNullOrEmpty(emailBcc) == false) mmsMessage.Bcc.Add(emailBcc);
            
            mmsMessage.IsBodyHtml = true;
            mmsMessage.Body = body;

            scSender.Host = emailHost;
            scSender.Credentials = new NetworkCredential(emailUserName, emailPassword);
            
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                scSender.Send(mmsMessage);
                sw.Stop();

                _logger.InsertLog(LogLevel.Information, string.Format("Action={{{0}}}, Time Elapsed(ms)={{{1}}}", "Send Email", sw.ElapsedMilliseconds));
            }            
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Warning, string.Format("Failed to send email. {0} Email={{{1}}}, Subject={{{2}}}", ex.Message, emailTo, subject), ex);
            }            
        }        
    }
}
