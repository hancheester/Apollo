using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.SagePay;
using Apollo.Core.Services.Payment.SagePay.ElementClass;
using Apollo.Core.Services.Security;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Apollo.Core.Services.Payment.SagePay
{
    public class SagePayPaymentSystemService : IPaymentSystemService
    {
        #region Constants

        const int CARD_HOLDER_MAX_LENGTH = 50;
        const int CARD_NUMBER_MAX_LENGTH = 20;
        const int NAME_MAX_LENGTH = 20;
        const int ADDRESS_MAX_LENGTH = 100;
        const int CITY_MAX_LENGTH = 40;
        const int POST_CODE_MAX_LENGTH = 10;
        const int CUSTOMER_EMAIL_MAX_LENGTH = 255;
        const int BASKET_MAX_LENGTH = 7500;

        const string DESCRIPTION = "Apollo.co.uk provides great deals on a large range of health and beauty products online.";
        const string AUTHORISE_DESCRIPTION = "Total chargeable value is {0}.";
        const string REPEAT_DESCRIPTION = "Total chargeable value is {0}.";
        const string FIRST_NAME_VALUE_FORMAT = "{0}={1}";
        const string NAME_VALUE_FORMAT = "&{0}={1}";
        const string WHITE_SPACE = " ";
        const string SEMI_COLON = ":";
        const string BACKSLASH = @"\";
        const string FORWARDSLASH = @"/";
        const string EQUAL = "=";
        const string APPLICATION_FORM_URLENCODED = "application/x-www-form-urlencoded";
        const string PRICE_ROUND_FORMAT = "{0:0}";
        const string PRICE_FORMAT = "{0:0.00}";
        const string DISCOUNT_LINE_FORMAT = ":Discount:1:---:---:---:-{0:0.00}";
        const string DELIVERY_LINE_FORMAT = ":Delivery:1:---:---:---:{0:0.00}";
        const string ORDER_TOTAL_LINE_FORMAT = ":Total:1:---:---:---:{0:0.00}";
        const string JPY = "JPY";
        const string ZERO = "0";
        const string TWO = "2";
        const string TX_CODE_FORMAT = "{0}-{1}";
        const string DEFERRED = "DEFERRED";
        const string PAYMENT = "PAYMENT";
        const string RELEASE = "RELEASE";
        const string ECOMMERCE_MERCHANT = "E";
        const string CONTINUOUS_AUTHORITY_MERCHANT = "C";
        const string MAIL_ORDER_TELEPHONE_ORDER = "M";

        #endregion

        #region Fields

        private readonly IRepository<Account> _accountRespository;
        private readonly IRepository<SagePayLog> _sagePayLogRepository;
        private readonly IRepository<SagePayDirect> _sagePayDirectRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly ICryptographyService _cryptographyService;
        private readonly SagePaySettings _sagepaySettings;
        private readonly TaxSettings _taxSettings;
        private readonly Regex _regexValidIPv4Address = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\b", RegexOptions.Compiled);

        #endregion

        #region Ctor
        public SagePayPaymentSystemService(
            IRepository<Account> accountRespository,
            IRepository<SagePayLog> sagePayLogRepository,
            IRepository<SagePayDirect> sagePayDirectRepository,
            IRepository<Country> countryRepository,
            IRepository<USState> usStateRepository,
            ICryptographyService cryptographyService,
            SagePaySettings sagepaySettings,
            TaxSettings taxSettings)
        {
            _accountRespository = accountRespository;
            _sagePayLogRepository = sagePayLogRepository;
            _sagePayDirectRepository = sagePayDirectRepository;
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _sagepaySettings = sagepaySettings;
            _taxSettings = taxSettings;
            _cryptographyService = cryptographyService;
        }
        #endregion

        public TransactionOutput ProcessPaymentAfter3DCallback(PaymentEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            SagePayDirect sagePayDirect = (SagePayDirect)entity;
            string nameValues = Build3DAuthResultsHttpPost(sagePayDirect.MD, sagePayDirect.PAReq);

            var logs = new List<SagePayLog>();
            switch (sagePayDirect.TxType)
            {
                case SagePayAccountTxType.AUTHENTICATE:
                    logs.Append(entity.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_AUTHENTICATE);
                    break;

                case SagePayAccountTxType.DEFERRED:
                    logs.Append(entity.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_DEFERRED);
                    break;

                case SagePayAccountTxType.PAYMENT:
                default:
                    logs.Append(entity.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_PAYMENT);
                    break;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var threeDSecureCallbackURL = _sagepaySettings.SagePay3DSecureCallbackLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(threeDSecureCallbackURL);

            var output = new TransactionOutput();

            try
            {
                string responseData = PerformHttpRequestResponse(entity.OrderId, bytes, request, logs);
                logs.Append(entity.OrderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);
                output = ProcessResponse(responseData, sagePayDirect);
                sagePayDirect.Completed = output.Completed;

                if (sagePayDirect.TxType == SagePayAccountTxType.AUTHENTICATE)
                    output.AVSCheck = true; // Assume AVSCheck is always true as there is only 3DSecureStatus for AUTHENTICATE response
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.Message = SagePayMessage.DEFAULT_ERROR_MESSAGE;

                logs.Append(entity.OrderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(entity);
            }

            return output;
        }
        
        public TransactionOutput ProcessCardPayment(Order order, PaymentEntity entity)
        {
            if (order == null) throw new ArgumentNullException("order");
            if (entity == null) throw new ArgumentNullException("entity");
            if (!(entity is SagePayDirect)) throw new ApolloException("It is not type of SagePayDirect.");

            Account customer = _accountRespository.Return(order.ProfileId);
            string email = string.Empty;
            if (customer != null)
                email = customer.Email;

            return ProcessCardPayment(
                order.Id,
                email,
                order.IPAddress,
                order.GrandTotal,
                order.CurrencyCode,
                order.BillTo,
                order.AddressLine1,
                order.AddressLine2,
                order.PostCode,
                order.City,
                order.USState,
                order.Country,
                order.ShipTo,
                order.ShippingAddressLine1,
                order.ShippingAddressLine2,
                order.ShippingPostCode,
                order.ShippingCity,
                order.ShippingUSState,
                order.ShippingCountry,
                entity);
        }
        
        public TransactionOutput ProcessCardPayment(EmailInvoice emailInvoice, PaymentEntity entity)
        {
            if (emailInvoice == null) throw new ArgumentNullException("emailInvoice");
            if (entity == null) throw new ArgumentNullException("entity");
            if (!(entity is SagePayDirect)) throw new ApolloException("It is not type of SagePayDirect.");

            return ProcessCardPayment(
                emailInvoice.OrderId,
                emailInvoice.Email,
                emailInvoice.IPAddress,
                emailInvoice.Amount * emailInvoice.ExchangeRate,
                emailInvoice.CurrencyCode,
                emailInvoice.BillTo,
                emailInvoice.AddressLine1,
                emailInvoice.AddressLine2,
                emailInvoice.PostCode,
                emailInvoice.City,
                emailInvoice.USState,
                emailInvoice.Country,
                emailInvoice.BillTo,
                emailInvoice.AddressLine1,
                emailInvoice.AddressLine2,
                emailInvoice.PostCode,
                emailInvoice.City,
                emailInvoice.USState,
                emailInvoice.Country,
                entity);
        }

        public TransactionOutput ProcessCardPayment(
            int orderId,
            string email,
            string ipAddress,
            decimal amount,
            string currencyCode,
            string billTo,
            string billingAddressLine1,
            string billingAddressLine2,
            string billingPostCode,
            string billingCity,
            USState billingUSState,
            Country billingCountry,
            string shipTo,
            string shippingAddressLine1,
            string shippingAddressLine2,
            string shippingPostCode,
            string shippingCity,
            USState shippingUSState,
            Country shippingCountry,            
            PaymentEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!(entity is SagePayDirect)) throw new ApolloException("It is not type of SagePayDirect.");

            SagePayDirect sagePayDirect = (SagePayDirect)entity;
            string nameValues;

            nameValues = BuildPaymentRegistrationHttpPost(
                email,
                ipAddress,
                amount,
                currencyCode,
                billTo,
                billingAddressLine1,
                billingAddressLine2,
                billingPostCode,
                billingCity,
                billingUSState,
                billingCountry,
                shipTo,
                shippingAddressLine1,
                shippingAddressLine2,
                shippingPostCode,
                shippingCity,
                shippingUSState,
                shippingCountry, 
                sagePayDirect);

            var logs = new List<SagePayLog>();

            switch (sagePayDirect.TxType)
            {
                case SagePayAccountTxType.AUTHENTICATE:
                    logs.Append(orderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_AUTHENTICATE);
                    break;

                case SagePayAccountTxType.DEFERRED:
                    logs.Append(orderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_DEFERRED);
                    break;

                case SagePayAccountTxType.PAYMENT:
                default:
                    logs.Append(orderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_PAYMENT);
                    break;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var registerGatewayURL = _sagepaySettings.SagePayPaymentGatewayLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registerGatewayURL);

            var output = new TransactionOutput();

            sagePayDirect.Completed = output.Completed;

            try
            {
                string responseData = PerformHttpRequestResponse(orderId, bytes, request, logs);
                logs.Append(orderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);
                output = ProcessResponse(responseData, sagePayDirect);
                sagePayDirect.Completed = output.Completed;

                if (sagePayDirect.TxType == SagePayAccountTxType.AUTHENTICATE)
                    output.AVSCheck = true; // Assume AVSCheck is always true as there is only 3DSecureStatus for AUTHENTICATE response
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.Message = SagePayMessage.DEFAULT_ERROR_MESSAGE;

                logs.Append(orderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                // Update database
                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(entity);
            }

            return output;
        }

        /// <summary>
        /// It can only support REFUND on RELEASE/PAYMENT transaction.
        /// </summary>
        /// <param name="refund">Object that holds refund information.</param>
        /// <returns></returns>
        public TransactionOutput ProcessRefund(Refund refund, PaymentEntity paymentEntity = null)
        {
            if (paymentEntity == null)
            {
                paymentEntity = _sagePayDirectRepository.Table
                    .Where(s => s.OrderId == refund.OrderId)
                    .Where(s => s.TxType == RELEASE || s.TxType == PAYMENT)
                    .Where(s => s.Completed == true)
                    .FirstOrDefault();
            }

            if (paymentEntity == null)
            {
                return new TransactionOutput
                {
                    TransactionResult = TransactionResults.PaymentTransactionNotFound,
                    Message = "There is no PAYMENT / RELEASE transaction with this order. There is no payment with this order, thus, refund is not possible."
                };
            }

            var logs = new List<SagePayLog>();

            logs.Append(refund.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_REFUND);

            var refundSagePayEntity = BuildRefundEntity(refund.OrderId, refund.ValueToRefund, refund.CurrencyCode, refund.ExchangeRate);
            var relatedSagePayEntity = (SagePayDirect)paymentEntity;
            string nameValues = BuildRefundRegistrationHttpPost(refundSagePayEntity,
                                                                refund.Reason,
                                                                relatedSagePayEntity.VendorTxCode,
                                                                relatedSagePayEntity.VPSTxId,
                                                                relatedSagePayEntity.SecurityKey,
                                                                relatedSagePayEntity.TxAuthNo);

            logs.Append(refund.OrderId, nameValues, SagePayProgressStatus.NAME_VALUES_GENERATED);

            // Insert REFUND SagePayDirectEntity
            _sagePayDirectRepository.Create(refundSagePayEntity);

            logs.Append(refund.OrderId, string.Empty, SagePayProgressStatus.REFUND_DATABASE_RECORD_CREATED);

            Byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var registerRefundURL = _sagepaySettings.SagePayRegisterRefundLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registerRefundURL);

            var output = new TransactionOutput();

            try
            {
                string responseData = PerformHttpRequestResponse(refund.OrderId, bytes, request, logs);

                logs.Append(refund.OrderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);

                output = ProcessResponse(responseData, refundSagePayEntity);

                refundSagePayEntity.Completed = output.Completed;

                output.Message += string.Format(" Refund amount: {0}{1}", refundSagePayEntity.Currency, RoundPrice(refundSagePayEntity.Amount * refundSagePayEntity.ExchangeRate.Value, 2));
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.TransactionResult = TransactionResults.Error;
                output.Message = SagePayMessage.DEFAULT_TRANSACTION_ERROR_MESSAGE;

                logs.Append(refund.OrderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                // Update database
                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(refundSagePayEntity);
            }

            return output;
        }

        /// <summary>
        /// If PAYMENT transaction is found, it performs REFUND transaction.
        /// If RELEASE transaction is found, it performs REFUND transaction.
        /// If DEFERRED transaction is found, it performs ABORT transaction.        
        /// </summary>
        /// <param name="refund">Refund object that holds refund information</param>
        /// <returns></returns>
        public TransactionOutput ProcessCancel(Refund refund)
        {
            var paymentEntity = _sagePayDirectRepository.Table
                .Where(s => s.OrderId == refund.OrderId)
                .Where(s => s.TxType == RELEASE || s.TxType == PAYMENT)
                .Where(s => s.Completed == true)
                .FirstOrDefault();

            if (paymentEntity == null)
            {
                var deferredEntity = _sagePayDirectRepository.Table
                    .Where(s => s.OrderId == refund.OrderId)
                    .Where(s => s.TxType == DEFERRED)
                    .Where(s => s.Completed == true)
                    .FirstOrDefault();

                if (deferredEntity == null)
                {
                    return new TransactionOutput
                    {
                        TransactionResult = TransactionResults.PaymentTransactionNotFound,
                        Message = "There is no PAYMENT / RELEASE / DEFERRED transaction found with this order. There is no payment with this order, thus, cancellation is not possible. "
                    };
                }
                else
                {
                    return ProcessAbortTransaction(deferredEntity);
                }
            }
            else
            {
                // Perform REFUND transaction here.
                return ProcessRefund(refund, paymentEntity);
            }
        }

        /// <summary>
        /// At the moment, it can only support DEFERRED transactions for order charging.
        /// If DEFERRED transaction is found, it will perform RELEASE transaction.
        /// If DEFERRED and RELEASE transactions are found together, it will perform REPEAT transaction instead.
        /// </summary>
        /// <param name="orderId">To find related order. It will be used to find any DEFERRED/RELEASE transaction with this order id.</param>
        /// <returns>Status after transaction.</returns>
        public TransactionOutput ProcessOrderCharging(OrderPayment orderPayment)
        {
            var deferredFound = _sagePayDirectRepository.Table
                .Where(s => s.OrderId == orderPayment.OrderId)
                .Where(s => s.TxType == DEFERRED)
                .Where(s => s.Completed == true)
                .FirstOrDefault();

            if (deferredFound != null)
            {
                var releaseFound = _sagePayDirectRepository.Table
                .Where(s => s.OrderId == orderPayment.OrderId)
                .Where(s => s.TxType == RELEASE)
                .Where(s => s.Completed == true)
                .FirstOrDefault();

                if (releaseFound != null)
                {
                    // Perform REPEAT transaction
                    return ProcessRepeatTransaction(releaseFound, orderPayment);
                }
                else
                {
                    // Perform RELEASE transaction
                    return ProcessReleaseTransaction(deferredFound, orderPayment);
                }
            }

            return new TransactionOutput
            {
                OrderId = orderPayment.OrderId,
                Message = "The order is not registered with SagePay. DEFERRED transaction is not found."
            };
        }

        public PaymentEntity InsertPaymentEntity(PaymentEntity entity)
        {
            if (!(entity is SagePayDirect)) throw new ApolloException("Entity is not type of SagePayDirect.");

            var sagepayDirect = entity as SagePayDirect;
            _sagePayDirectRepository.Create(sagepayDirect);

            return sagepayDirect;
        }

        public PaymentEntity UpdateAndGetPaymentEntityForOutOfStock(int id)
        {
            var result = Retry.Do(delegate ()
            {
                var entity = _sagePayDirectRepository.Return(id);

                if (entity != null)
                {
                    entity.TxType = DEFERRED;
                    _sagePayDirectRepository.Update(entity);
                }

                return entity;
            }, TimeSpan.FromSeconds(3));

            return result;
        }

        public PaymentEntity GetPaymentEntityByMD(string md)
        {
            // TODO: It's a performance killer here as there would be millions of rows to search for. We need to find a better way. 
            var entity = _sagePayDirectRepository.Table.Where(s => s.MD == md).FirstOrDefault();
            return entity;
        }

        public void UpdatePaymentEntityForPAReq(int id, string paReq)
        {
            var entity = _sagePayDirectRepository.Return(id);

            if (entity != null)
            {
                entity.PAReq = paReq;
                _sagePayDirectRepository.Update(entity);
            }
        }

        public PaymentEntity BuildPaymentEntity(string vendorTxCode,
                                                decimal amount,
                                                decimal exchangeRate,
                                                string currencyCode,
                                                Card card,
                                                string email,
                                                string contactNumber,
                                                string userAgent,
                                                string clientIPAddress,
                                                int orderId,
                                                int? emailInvoiceId = null)
        {
            var spDirect = new SagePayDirect();

            spDirect.AccountType = ECOMMERCE_MERCHANT;
            spDirect.TxType = DEFERRED; // TODO: We need to consider if we should hardcode it or not.
            spDirect.VendorTxCode = vendorTxCode;
            spDirect.Amount = amount;
            spDirect.ClientIPAddress = clientIPAddress;
            spDirect.Apply3DSecure = ZERO;
            spDirect.ApplyAVSCV2 = ZERO;

            spDirect.CardHolder = _cryptographyService.Encrypt(card.HolderName);
            spDirect.CardNumber = _cryptographyService.Encrypt(card.CardNumber);
            spDirect.StartMonth = _cryptographyService.Encrypt(card.StartMonth);
            spDirect.StartYear = _cryptographyService.Encrypt(card.StartYear);
            spDirect.ExpiryMonth = _cryptographyService.Encrypt(card.ExpiryMonth);
            spDirect.ExpiryYear = _cryptographyService.Encrypt(card.ExpiryYear);
            spDirect.IssueNumber = _cryptographyService.Encrypt(card.IssueNumber);
            spDirect.CardType = _cryptographyService.Encrypt(card.CardType);
            spDirect.CV2 = _cryptographyService.Encrypt(card.SecurityCode);

            spDirect.Email = email;
            spDirect.UserAgent = userAgent;
            spDirect.ContactNumber = contactNumber;
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            spDirect.VPSProtocol = vpsProtocol;
            var vendor = _sagepaySettings.SagePayVendor;
            spDirect.Vendor = vendor;
            spDirect.Currency = currencyCode;
            spDirect.OrderId = orderId;

            if (emailInvoiceId.HasValue)
            {
                spDirect.TxType = PAYMENT;
                spDirect.EmailInvoiceId = emailInvoiceId.HasValue ? emailInvoiceId.Value : 0;
            }
            spDirect.ExchangeRate = exchangeRate;
            return spDirect;
        }

        public PaymentEntity BuildPaymentEntityForBackOffice(string vendorTxCode,
                                                             decimal amount,
                                                             decimal exchangeRate,
                                                             string currencyCode,
                                                             Card card,
                                                             string email,
                                                             string contactNumber,
                                                             string userAgent,
                                                             string clientIPAddress,
                                                             int orderId,
                                                             int? emailInvoiceId = null)
        {
            var entity = BuildPaymentEntity(vendorTxCode,
                                            amount,
                                            exchangeRate,
                                            currencyCode,
                                            card,
                                            email,
                                            contactNumber,
                                            userAgent,
                                            clientIPAddress,
                                            orderId,
                                            emailInvoiceId);

            var spDirect = (SagePayDirect)entity;
            spDirect.AccountType = MAIL_ORDER_TELEPHONE_ORDER;
            spDirect.Apply3DSecure = TWO;

            return spDirect;
        }

        public string GetThirdManResult(int orderId)
        {
            XMLGenerator ReportingAdminProxyXMLGenerator = new XMLGenerator();
            var vendor = _sagepaySettings.SagePayVendor;
            var sagePayWebUsername = _sagepaySettings.SagePayWebUser;
            var sagePayWebPassword = _sagepaySettings.SagePayWebPwd;
            ReportingAdminProxyXMLGenerator.vendor = vendor;
            ReportingAdminProxyXMLGenerator.user = sagePayWebUsername;
            ReportingAdminProxyXMLGenerator.password = sagePayWebPassword;
            ReportingAdminProxyXMLGenerator.command = "getTransactionDetail";
            ReportingAdminProxyXMLGenerator.vendortxcode = orderId.ToString();

            string xml = ReportingAdminProxyXMLGenerator.GetXML();

            var sagepayReportingAdminApiUrl = _sagepaySettings.SagePayReportingAdminAPILink;
            var client = new Client(sagepayReportingAdminApiUrl);

            vspaccess objReply = client.SubmitRequest(xml);

            if (objReply.errorcode == "0000")
                return objReply.t3maction + " " + objReply.t3mscore;

            return "Error " + objReply.errorcode + ": " + objReply.error;
        }

        public string GetPaymentDetails(int orderId)
        {
            XMLGenerator ReportingAdminProxyXMLGenerator = new XMLGenerator();
            var vendor = _sagepaySettings.SagePayVendor;
            var sagePayWebUsername = _sagepaySettings.SagePayWebUser;
            var sagePayWebPassword = _sagepaySettings.SagePayWebPwd;
            ReportingAdminProxyXMLGenerator.vendor = vendor;
            ReportingAdminProxyXMLGenerator.user = sagePayWebUsername;
            ReportingAdminProxyXMLGenerator.password = sagePayWebPassword;
            ReportingAdminProxyXMLGenerator.command = "getTransactionDetail";
            ReportingAdminProxyXMLGenerator.vendortxcode = orderId.ToString();

            string xml = ReportingAdminProxyXMLGenerator.GetXML();

            var sagepayReportingAdminApiUrl = _sagepaySettings.SagePayReportingAdminAPILink;
            var client = new Client(sagepayReportingAdminApiUrl);

            vspaccess objReply = client.SubmitRequest(xml);

            if (objReply.errorcode == "0000")
                return objReply.paymentsystemdetails;

            return "Error " + objReply.errorcode + ": " + objReply.error;
        }

        public string GetIPLocation(int orderId)
        {
            XMLGenerator ReportingAdminProxyXMLGenerator = new XMLGenerator();
            var vendor = _sagepaySettings.SagePayVendor;
            var sagePayWebUsername = _sagepaySettings.SagePayWebUser;
            var sagePayWebPassword = _sagepaySettings.SagePayWebPwd;
            ReportingAdminProxyXMLGenerator.vendor = vendor;
            ReportingAdminProxyXMLGenerator.user = sagePayWebUsername;
            ReportingAdminProxyXMLGenerator.password = sagePayWebPassword;
            ReportingAdminProxyXMLGenerator.command = "getTransactionIPDetails";
            ReportingAdminProxyXMLGenerator.vendortxcode = orderId.ToString();

            string xml = ReportingAdminProxyXMLGenerator.GetXML();

            var sagepayReportingAdminApiUrl = _sagepaySettings.SagePayReportingAdminAPILink;
            var client = new Client(sagepayReportingAdminApiUrl);

            vspaccess objReply = client.SubmitRequest(xml);

            if (objReply.errorcode == "0000")
                return objReply.clientip + " - " + objReply.iplocation;

            return "Error " + objReply.errorcode + ": " + objReply.error;
        }

        public string GetLatestStatusFromLog(int orderId)
        {
            var nameValue = _sagePayLogRepository.Table
                .Where(s => s.OrderId == orderId)
                .OrderByDescending(s => s.Id)
                .Select(s => s.NameValue)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(nameValue))
            {
                string status = FindField(SagePayDirectFormName.STATUS, nameValue);
                string statusDetail = FindField(SagePayDirectFormName.STATUS_DETAIL, nameValue);

                return status + " - " + statusDetail + "<br/>";
            }

            return string.Empty;
        }

        public string GetTransactionCardDetails(int orderId)
        {
            XMLGenerator ReportingAdminProxyXMLGenerator = new XMLGenerator();
            var vendor = _sagepaySettings.SagePayVendor;
            var sagePayWebUsername = _sagepaySettings.SagePayWebUser;
            var sagePayWebPassword = _sagepaySettings.SagePayWebPwd;
            ReportingAdminProxyXMLGenerator.vendor = vendor;
            ReportingAdminProxyXMLGenerator.user = sagePayWebUsername;
            ReportingAdminProxyXMLGenerator.password = sagePayWebPassword;
            ReportingAdminProxyXMLGenerator.command = "getTransactionCardDetails";
            ReportingAdminProxyXMLGenerator.vendortxcode = orderId.ToString();

            string xml = ReportingAdminProxyXMLGenerator.GetXML();

            var sagepayReportingAdminApiUrl = _sagepaySettings.SagePayReportingAdminAPILink;
            var client = new Client(sagepayReportingAdminApiUrl);

            vspaccess objReply = client.SubmitRequest(xml);

            if (objReply.errorcode == "0000")
                return objReply.paymentsystem + " - " + objReply.last4digits;

            return "Error " + objReply.errorcode + ": " + objReply.error;
        }

        #region Transaction process methods

        private TransactionOutput ProcessRepeatTransaction(SagePayDirect entity, OrderPayment orderPayment)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (orderPayment == null) throw new ArgumentNullException("orderPayment");

            var logs = new List<SagePayLog>();
            logs.Append(orderPayment.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_REPEAT);

            var repeatSagePayEntity = BuildRepeatEntity(orderPayment.OrderId,
                                                        orderPayment.Amount,
                                                        entity.CV2,
                                                        orderPayment.CurrencyCode,
                                                        orderPayment.ExchangeRate);

            string nameValues = BuildRepeatRegistrationHttpPost(repeatSagePayEntity, entity);

            logs.Append(orderPayment.OrderId, nameValues, SagePayProgressStatus.NAME_VALUES_GENERATED);

            // Insert REPEAT SagePayDirectEntity
            _sagePayDirectRepository.Create(repeatSagePayEntity);

            logs.Append(orderPayment.OrderId, string.Empty, SagePayProgressStatus.REPEAT_DATABASE_RECORD_CREATED);

            byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var registerRepeatURL = _sagepaySettings.SagePayRegisterRepeatLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registerRepeatURL);
            var output = new TransactionOutput();

            try
            {
                string responseData = PerformHttpRequestResponse(orderPayment.OrderId, bytes, request, logs);
                logs.Append(orderPayment.OrderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);
                output = ProcessResponse(responseData, repeatSagePayEntity);
                repeatSagePayEntity.Completed = output.Completed;
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.Message = SagePayMessage.DEFAULT_TRANSACTION_ERROR_MESSAGE;

                logs.Append(orderPayment.OrderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                // Update database
                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(repeatSagePayEntity);
            }

            return output;
        }

        private TransactionOutput ProcessReleaseTransaction(SagePayDirect entity, OrderPayment orderPayment)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (orderPayment == null) throw new ArgumentNullException("orderPayment");

            var logs = new List<SagePayLog>();

            logs.Append(orderPayment.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_RELEASE);

            SagePayDirect releaseSagePayEntity = BuildReleaseEntity(orderPayment.OrderId,
                                                                    orderPayment.Amount,
                                                                    entity.VendorTxCode,
                                                                    entity.VPSTxId,
                                                                    entity.SecurityKey,
                                                                    entity.TxAuthNo,
                                                                    entity.Currency,
                                                                    orderPayment.ExchangeRate);

            string nameValues = BuildReleaseRegistrationHttpPost(releaseSagePayEntity);

            logs.Append(orderPayment.OrderId, nameValues, SagePayProgressStatus.NAME_VALUES_GENERATED);

            // Insert RELEASE SagePayDirectEntity
            _sagePayDirectRepository.Create(releaseSagePayEntity);

            logs.Append(orderPayment.OrderId, string.Empty, SagePayProgressStatus.RELEASE_DATABASE_RECORD_CREATED);

            byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var registerReleaseURL = _sagepaySettings.SagePayRegisterReleaseLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registerReleaseURL);
            var output = new TransactionOutput();

            try
            {
                string responseData = PerformHttpRequestResponse(orderPayment.OrderId, bytes, request, logs);
                logs.Append(orderPayment.OrderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);
                output = ProcessResponse(responseData, releaseSagePayEntity);
                releaseSagePayEntity.Completed = output.Completed;
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.Message = SagePayMessage.DEFAULT_TRANSACTION_ERROR_MESSAGE;

                logs.Append(orderPayment.OrderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                // Update database
                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(releaseSagePayEntity);
            }

            return output;
        }

        private TransactionOutput ProcessAbortTransaction(SagePayDirect entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            var logs = new List<SagePayLog>();

            logs.Append(entity.OrderId, string.Empty, SagePayProgressStatus.REGISTER_TRANSACTION_ABORT);

            var abortSagePayEntity = BuildAbortEntity(entity.OrderId,
                                                      entity.VendorTxCode,
                                                      entity.VPSTxId,
                                                      entity.SecurityKey,
                                                      entity.TxAuthNo);

            string nameValues = BuildAbortRegistrationHttpPost(abortSagePayEntity);

            logs.Append(entity.OrderId, nameValues, SagePayProgressStatus.NAME_VALUES_GENERATED);

            // Insert ABORT SagePayDirectEntity
            _sagePayDirectRepository.Create(abortSagePayEntity);

            logs.Append(entity.OrderId, string.Empty, SagePayProgressStatus.ABORT_DATABASE_RECORD_CREATED);

            byte[] bytes = Encoding.UTF8.GetBytes(nameValues);
            var registerAbortURL = _sagepaySettings.SagePayRegisterAbortLink;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registerAbortURL);
            var output = new TransactionOutput();

            try
            {
                string responseData = PerformHttpRequestResponse(entity.OrderId, bytes, request, logs);
                logs.Append(entity.OrderId, responseData, SagePayProgressStatus.RESPONSE_RECEIVED);
                output = ProcessResponse(responseData, abortSagePayEntity);
                abortSagePayEntity.Completed = output.Completed;
            }
            catch (Exception ex)
            {
                output.Status = false;
                output.Message = SagePayMessage.DEFAULT_TRANSACTION_ERROR_MESSAGE;

                logs.Append(entity.OrderId, GetXmlString(ex), SagePayProgressStatus.ERROR_OCCURRED);
            }
            finally
            {
                request = null;
                bytes = null;

                // Update database
                UpdateDatabaseLog(logs);
                UpdatePaymentEntity(abortSagePayEntity);
            }

            return output;
        }

        private TransactionOutput ProcessResponse(string responseData, SagePayDirect sagePayEntity)
        {
            var output = new TransactionOutput
            {
                OrderId = sagePayEntity.OrderId,
                Email = sagePayEntity.Email,
                EmailInvoiceId = sagePayEntity.EmailInvoiceId
            };

            string vpstxid = FindField(SagePayDirectFormName.VPSTXID, responseData);
            if (vpstxid != string.Empty) sagePayEntity.VPSTxId = vpstxid;
            string securityKey = FindField(SagePayDirectFormName.SECURITY_KEY, responseData);
            if (securityKey != string.Empty) sagePayEntity.SecurityKey = securityKey;

            string txAuthNo = FindField(SagePayDirectFormName.TX_AUTHNO, responseData);
            if (txAuthNo != string.Empty) sagePayEntity.TxAuthNo = txAuthNo;
            string avsCV2 = FindField(SagePayDirectFormName.AVSCV2, responseData);
            if (avsCV2 != string.Empty) sagePayEntity.AVSCV2 = avsCV2;
            string addressResult = FindField(SagePayDirectFormName.ADDRESS_RESULT, responseData);
            if (addressResult != string.Empty) sagePayEntity.AddressResult = addressResult;
            string postCodeResult = FindField(SagePayDirectFormName.POSTCODE_RESULT, responseData);
            if (postCodeResult != string.Empty) sagePayEntity.PostCodeResult = postCodeResult;
            string cv2Result = FindField(SagePayDirectFormName.CV2RESULT, responseData);
            if (cv2Result != string.Empty) sagePayEntity.CV2Result = cv2Result;
            string threeSecureStatus = FindField(SagePayDirectFormName.THREE_SECURE_STATUS, responseData);
            if (threeSecureStatus != string.Empty) sagePayEntity.ThreeDSecureStatus = threeSecureStatus;
            string cavv = FindField(SagePayDirectFormName.CAVV, responseData);
            if (cavv != string.Empty) sagePayEntity.CAVV = cavv;

            string status = FindField(SagePayDirectFormName.STATUS, responseData);
            string statusDetail = FindField(SagePayDirectFormName.STATUS_DETAIL, responseData);

            output.AVSCheck = GetAVSCheckStatus(FindField(SagePayDirectFormName.AVSCV2, responseData));
            output.ThreeDCheck = Get3DCheckStatus(FindField(SagePayDirectFormName.THREE_SECURE_STATUS, responseData));

            switch (status)
            {
                case SagePayDirectStatus.THREE_D_AUTH:
                    string md = FindField(SagePayDirectFormName.MD, responseData);
                    if (md != string.Empty) sagePayEntity.MD = md;
                    string acsUrl = FindField(SagePayDirectFormName.ACSURL, responseData);
                    if (acsUrl != string.Empty) sagePayEntity.ACSUrl = acsUrl;
                    string paReq = FindField(SagePayDirectFormName.PAREQ, responseData);
                    if (paReq != string.Empty) sagePayEntity.PAReq = paReq;

                    output.Has3DSecure = true;

                    output.MD = FindField(SagePayDirectFormName.MD, responseData);
                    output.RedirectUrl = FindField(SagePayDirectFormName.ACSURL, responseData);
                    output.PaReq = FindField(SagePayDirectFormName.PAREQ, responseData);

                    output.Status = true;
                    output.TransactionResult = TransactionResults.URLRedirectRequired;
                    break;

                case SagePayDirectStatus.OK:
                case SagePayDirectStatus.AUTHENTICATED:
                case SagePayDirectStatus.REGISTERED:
                    output.Status = true;
                    output.Completed = true; // To indicate that SagePayDirect entity is completed.

                    if (sagePayEntity.TxType == PAYMENT || sagePayEntity.TxType == RELEASE)
                        output.PaymentReleased = true;

                    output.TransactionResult = TransactionResults.Success;

                    sagePayEntity = RemoveCardInformation(sagePayEntity);
                    break;

                case SagePayDirectStatus.NOTAUTHED:
                    output.Message = SagePayMessage.NOTAUTHORISED_ERROR_MESSAGE;
                    output.Status = false;
                    output.TransactionResult = TransactionResults.Failed;

                    sagePayEntity = RemoveCardInformation(sagePayEntity);
                    break;

                case SagePayDirectStatus.REJECTED:
                    output.Message = SagePayMessage.REJECTED_ERROR_MESSAGE;
                    output.Status = false;
                    output.TransactionResult = TransactionResults.Failed;

                    sagePayEntity = RemoveCardInformation(sagePayEntity);
                    break;

                case SagePayDirectStatus.MALFORMED:
                case SagePayDirectStatus.INVALID:
                case SagePayDirectStatus.ERROR:
                case SagePayDirectStatus.PPREDIRECT: // PayPal
                default:
                    if (sagePayEntity.TxType == SagePayAccountTxType.AUTHENTICATE ||
                        sagePayEntity.TxType == SagePayAccountTxType.DEFERRED ||
                        sagePayEntity.TxType == SagePayAccountTxType.PAYMENT)
                    {
                        output.Message = SagePayMessage.DEFAULT_ERROR_MESSAGE + statusDetail;
                    }
                    else
                    {
                        output.Message = SagePayMessage.DEFAULT_TRANSACTION_ERROR_MESSAGE + statusDetail;
                    }

                    output.Status = false;
                    output.TransactionResult = TransactionResults.Error;

                    sagePayEntity = RemoveCardInformation(sagePayEntity);
                    break;
            }

            return output;
        }

        #endregion

        #region Http post build methods

        private string Build3DAuthResultsHttpPost(string md, string paRes)
        {
            StringBuilder sbPost = new StringBuilder();
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.MD, md);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.PARES, paRes);

            return sbPost.ToString();
        }
        
        private string BuildPaymentRegistrationHttpPost(            
            string email,
            string ipAddress, 
            decimal amount,
            string currencyCode,
            string billTo,
            string billingAddressLine1,
            string billingAddressLine2,
            string billingPostCode,
            string billingCity,
            USState billingUSState,
            Country billingCountry,
            string shipTo,
            string shippingAddressLine1,
            string shippingAddressLine2,
            string shippingPostCode,
            string shippingCity,
            USState shippingUSState,
            Country shippingCountry,
            SagePayDirect sagePayDirect)
        {
            StringBuilder sbPost = new StringBuilder();
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            var vendor = _sagepaySettings.SagePayVendor;
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, vpsProtocol);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, sagePayDirect.TxType);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, vendor);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, sagePayDirect.VendorTxCode);
            
            decimal roundedValue = RoundPrice(amount, 2);
            string priceFormat = PRICE_FORMAT;

            // Special case for JPY
            if (currencyCode.ToUpper() == JPY)
            {
                roundedValue = RoundPrice(amount, 0);
                priceFormat = PRICE_ROUND_FORMAT;
            }

            string encryptedEmptyString = _cryptographyService.Encrypt(string.Empty);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.AMOUNT, string.Format(priceFormat, roundedValue));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CURRENCY, currencyCode);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DESCRIPTION, HttpUtility.UrlEncode(DESCRIPTION));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CARD_HOLDER, TruncateString(CARD_HOLDER_MAX_LENGTH, HttpUtility.UrlEncode(_cryptographyService.Decrypt(sagePayDirect.CardHolder))));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CARD_NUMBER, TruncateString(CARD_NUMBER_MAX_LENGTH, _cryptographyService.Decrypt(sagePayDirect.CardNumber)));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.EXPIRY_DATE, _cryptographyService.Decrypt(sagePayDirect.ExpiryMonth) + _cryptographyService.Decrypt(sagePayDirect.ExpiryYear).Remove(0, 2));

            if (sagePayDirect.StartMonth != encryptedEmptyString && sagePayDirect.StartYear != encryptedEmptyString)
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.START_DATE, _cryptographyService.Decrypt(sagePayDirect.StartMonth) + _cryptographyService.Decrypt(sagePayDirect.StartYear).Remove(0, 2));

            if (sagePayDirect.IssueNumber != encryptedEmptyString)
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.ISSUE_NUMBER, _cryptographyService.Decrypt(sagePayDirect.IssueNumber));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CV2, _cryptographyService.Decrypt(sagePayDirect.CV2));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CARD_TYPE, _cryptographyService.Decrypt(sagePayDirect.CardType).ToUpper());

            string bSurname = string.Empty;
            string bFirstname = string.Empty;
            GetNameInParts(RemoveSlash(billTo.Trim()), NAME_MAX_LENGTH, out bSurname, out bFirstname);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_SURNAME, TruncateString(NAME_MAX_LENGTH, HttpUtility.UrlEncode(bSurname)));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_FIRSTNAMES, TruncateString(NAME_MAX_LENGTH, HttpUtility.UrlEncode(bFirstname)));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_ADDRESS1, TruncateString(ADDRESS_MAX_LENGTH, HttpUtility.UrlEncode(billingAddressLine1)));

            if (!string.IsNullOrWhiteSpace(billingAddressLine2))
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_ADDRESS2, TruncateString(ADDRESS_MAX_LENGTH, HttpUtility.UrlEncode(billingAddressLine2)));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_CITY, TruncateString(CITY_MAX_LENGTH, HttpUtility.UrlEncode(billingCity)));

            if (!string.IsNullOrWhiteSpace(billingPostCode))
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_POSTCODE, TruncateString(POST_CODE_MAX_LENGTH, HttpUtility.UrlEncode(billingPostCode)));
            else
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_POSTCODE, ZERO);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_COUNTRY, HttpUtility.UrlEncode(billingCountry.ISO3166Code));

            var billingState = billingUSState != null ? billingUSState.Code : string.Empty;
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.BILLING_STATE, HttpUtility.UrlEncode(billingState));

            string sSurname = string.Empty;
            string sFirstname = string.Empty;
            GetNameInParts(RemoveSlash(shipTo.Trim()), NAME_MAX_LENGTH, out sSurname, out sFirstname);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_SURNAME, TruncateString(NAME_MAX_LENGTH, HttpUtility.UrlEncode(sSurname)));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_FIRSTNAMES, TruncateString(NAME_MAX_LENGTH, HttpUtility.UrlEncode(sFirstname)));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_ADDRESS1, TruncateString(ADDRESS_MAX_LENGTH, HttpUtility.UrlEncode(shippingAddressLine1)));

            if (!string.IsNullOrWhiteSpace(shippingAddressLine2))
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_ADDRESS2, TruncateString(ADDRESS_MAX_LENGTH, HttpUtility.UrlEncode(shippingAddressLine2)));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_CITY, TruncateString(CITY_MAX_LENGTH, HttpUtility.UrlEncode(shippingCity)));

            if (!string.IsNullOrWhiteSpace(shippingPostCode))
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_POSTCODE, TruncateString(POST_CODE_MAX_LENGTH, HttpUtility.UrlEncode(shippingPostCode)));
            else
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_POSTCODE, ZERO);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_COUNTRY, HttpUtility.UrlEncode(shippingCountry.ISO3166Code));

            var deliveryState = shippingUSState != null ? shippingUSState.Code : string.Empty;
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DELIVERY_STATE, HttpUtility.UrlEncode(deliveryState));

            if (!string.IsNullOrEmpty(email))
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CUSTOMER_EMAIL, TruncateString(CUSTOMER_EMAIL_MAX_LENGTH, HttpUtility.UrlEncode(email)));

            if (sagePayDirect.TxType != SagePayAccountTxType.AUTHENTICATE)
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.APPLY_AVS_CV2, sagePayDirect.ApplyAVSCV2);

            if (_regexValidIPv4Address.IsMatch(ipAddress))
            {
                sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CLIENT_IP_ADDRESS, ipAddress);
            }
            
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.APPLY_3DSECURE, sagePayDirect.Apply3DSecure);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.ACCOUNT_TYPE, sagePayDirect.AccountType);

            return sbPost.ToString();
        }

        private string BuildRefundRegistrationHttpPost(SagePayDirect refundSagePayDirect, string reason, string vendorTxCode, string vpsTxId, string securityKey, string txAuthNo)
        {
            StringBuilder sbPost = new StringBuilder();
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, refundSagePayDirect.VPSProtocol);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, refundSagePayDirect.TxType);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, refundSagePayDirect.Vendor);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, refundSagePayDirect.VendorTxCode);
            decimal roundedValue = RoundPrice(refundSagePayDirect.Amount * refundSagePayDirect.ExchangeRate.Value, 2);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.AMOUNT, string.Format(PRICE_FORMAT, roundedValue));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CURRENCY, refundSagePayDirect.Currency);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DESCRIPTION, TruncateString(100, HttpUtility.UrlEncode(reason)));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VPSTXID, vpsTxId);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VENDOR_TXCODE, vendorTxCode);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_SECURITY_KEY, securityKey);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_TX_AUTHNO, txAuthNo);

            return sbPost.ToString();
        }

        // CANCEL is only for AUTHENTICATE transaction. We don't support AUTHENTICATE at the moment.
        //private string BuildCancelRegistrationHttpPost(SagePayDirect cancelSagePayDirect)
        //{
        //    StringBuilder sbPost = new StringBuilder();
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, cancelSagePayDirect.VPSProtocol);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, cancelSagePayDirect.TxType);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, cancelSagePayDirect.Vendor);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, cancelSagePayDirect.VendorTxCode);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSTXID, cancelSagePayDirect.VPSTxId);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.SECURITY_KEY, cancelSagePayDirect.SecurityKey);

        //    return sbPost.ToString();
        //}

        private string BuildReleaseRegistrationHttpPost(SagePayDirect releaseSagePayDirect)
        {
            StringBuilder sbPost = new StringBuilder();
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, releaseSagePayDirect.VPSProtocol);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, releaseSagePayDirect.TxType);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, releaseSagePayDirect.Vendor);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, releaseSagePayDirect.VendorTxCode);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSTXID, releaseSagePayDirect.VPSTxId);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.SECURITY_KEY, releaseSagePayDirect.SecurityKey);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TX_AUTHNO, releaseSagePayDirect.TxAuthNo);

            decimal roundedValue = RoundPrice(releaseSagePayDirect.Amount * releaseSagePayDirect.ExchangeRate.Value, 2);
            string priceFormat = PRICE_FORMAT;

            // Special case for JPY
            if (releaseSagePayDirect.Currency.ToUpper() == JPY)
            {
                roundedValue = RoundPrice(releaseSagePayDirect.Amount * releaseSagePayDirect.ExchangeRate.Value, 0);
                priceFormat = PRICE_ROUND_FORMAT;
            }

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELEASE_AMOUNT, string.Format(priceFormat, roundedValue));

            return sbPost.ToString();
        }

        // AUTHORISE is only for AUTHENTICATE transaction. We don't support AUTHENTICATE at the moment.
        //private string BuildAuthoriseRegistrationHttpPost(SagePayDirect authoriseSagePayDirect, SagePayDirect relatedSagePayDirect, OrderPayment orderPayment)
        //{
        //    StringBuilder sbPost = new StringBuilder();
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, authoriseSagePayDirect.VPSProtocol);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, authoriseSagePayDirect.TxType);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, authoriseSagePayDirect.Vendor);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, authoriseSagePayDirect.VendorTxCode);

        //    decimal roundedValue = RoundPrice(orderPayment.Amount, 2);
        //    string priceFormat = PRICE_FORMAT;

        //    // Special case for JPY
        //    if (orderPayment.CurrencyCode.ToUpper() == JPY)
        //    {
        //        roundedValue = RoundPrice(orderPayment.Amount, 0);
        //        priceFormat = PRICE_ROUND_FORMAT;
        //    }

        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.AMOUNT, string.Format(priceFormat, roundedValue));
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.APPLY_AVS_CV2, authoriseSagePayDirect.ApplyAVSCV2);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DESCRIPTION, string.Format(AUTHORISE_DESCRIPTION, orderPayment.CurrencyCode + string.Format(PRICE_FORMAT, roundedValue)));

        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VPSTXID, relatedSagePayDirect.VPSTxId);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VENDOR_TXCODE, relatedSagePayDirect.VendorTxCode);
        //    sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_SECURITY_KEY, relatedSagePayDirect.SecurityKey);

        //    return sbPost.ToString();
        //}

        private string BuildRepeatRegistrationHttpPost(SagePayDirect repeatSagePayDirect, SagePayDirect relatedSagePayDirect)
        {
            StringBuilder sbPost = new StringBuilder();
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, repeatSagePayDirect.VPSProtocol);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, repeatSagePayDirect.TxType);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, repeatSagePayDirect.Vendor);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, repeatSagePayDirect.VendorTxCode);

            decimal roundedValue = RoundPrice(repeatSagePayDirect.Amount * repeatSagePayDirect.ExchangeRate.Value, 2);
            string priceFormat = PRICE_FORMAT;

            // Special case for JPY
            if (repeatSagePayDirect.Currency.ToUpper() == JPY)
            {
                roundedValue = RoundPrice(repeatSagePayDirect.Amount * repeatSagePayDirect.ExchangeRate.Value, 0);
                priceFormat = PRICE_ROUND_FORMAT;
            }

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.AMOUNT, string.Format(priceFormat, roundedValue));
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CURRENCY, repeatSagePayDirect.Currency);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.DESCRIPTION, string.Format(REPEAT_DESCRIPTION, repeatSagePayDirect.Currency.ToUpper() + string.Format(PRICE_FORMAT, roundedValue)));

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VPSTXID, relatedSagePayDirect.VPSTxId);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_VENDOR_TXCODE, relatedSagePayDirect.VendorTxCode);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_SECURITY_KEY, relatedSagePayDirect.SecurityKey);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.RELATED_TX_AUTHNO, relatedSagePayDirect.TxAuthNo);

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.CV2, _cryptographyService.Decrypt(repeatSagePayDirect.CV2));

            return sbPost.ToString();
        }

        private string BuildAbortRegistrationHttpPost(SagePayDirect abortSagePayDirect)
        {
            StringBuilder sbPost = new StringBuilder();

            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSPROTOCOL, abortSagePayDirect.VPSProtocol);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TXTYPE, abortSagePayDirect.TxType);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR, abortSagePayDirect.Vendor);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VENDOR_TXCODE, abortSagePayDirect.VendorTxCode);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.VPSTXID, abortSagePayDirect.VPSTxId);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.SECURITY_KEY, abortSagePayDirect.SecurityKey);
            sbPost = AppendNameValue(sbPost, SagePayDirectFormName.TX_AUTHNO, abortSagePayDirect.TxAuthNo);

            return sbPost.ToString();
        }

        #endregion

        #region Payment entity build methods

        private SagePayDirect BuildAbortEntity(int orderId, string vendorTxCode, string vpsTxId, string securityKey, string txAuthNo)
        {
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            var vendor = _sagepaySettings.SagePayVendor;

            SagePayDirect spDirect = new SagePayDirect
            {
                OrderId = orderId,
                VPSProtocol = vpsProtocol,
                TxType = SagePayAccountTxType.ABORT,
                Vendor = vendor,
                VendorTxCode = vendorTxCode,
                VPSTxId = vpsTxId,
                SecurityKey = securityKey,
                TxAuthNo = txAuthNo
            };

            return spDirect;
        }

        /// <summary>
        /// Build refund entity. Format for VendorTxCode is REFUND-[ddMMyyyyHHmmss].
        /// </summary>
        /// <param name="valueToRefund">Amount in GBP.</param>
        private SagePayDirect BuildRefundEntity(int orderId, decimal valueToRefund, string currencyCode, decimal exchangeRate)
        {
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            var vendor = _sagepaySettings.SagePayVendor;

            var spDirect = new SagePayDirect
            {
                OrderId = orderId,
                VPSProtocol = vpsProtocol,
                TxType = SagePayAccountTxType.REFUND,
                Vendor = vendor,
                // Format for VendorTxCode = REFUND-[ddMMyyyyHHmmss]
                VendorTxCode = string.Format(TX_CODE_FORMAT, SagePayAccountTxType.REFUND, DateTime.Now.ToString("ddMMyyyyHHmmss")),
                Amount = valueToRefund,
                Currency = currencyCode,
                ExchangeRate = exchangeRate
            };

            return spDirect;
        }

        // CANCEL is only for AUTHENTICATE transaction. We don't support AUTHENTICATE at the moment.
        //private SagePayDirect BuildCancelEntity(int orderId, string vendorTxCode, string vpsTxId, string securityKey)
        //{
        //    var spDirect = new SagePayDirect();

        //    spDirect.OrderId = orderId;
        //    spDirect.VPSProtocol = _vpsProtocol;
        //    spDirect.TxType = SagePayAccountTxType.CANCEL;
        //    spDirect.Vendor = _vendor;
        //    spDirect.VendorTxCode = vendorTxCode;
        //    spDirect.VPSTxId = vpsTxId;
        //    spDirect.SecurityKey = securityKey;

        //    return spDirect;
        //}

        /// <summary>
        /// Build repeat entity. Format for VendorTxCode is REPEAT-[ddMMyyyyHHmmss].
        /// </summary>
        /// <param name="amount">Amount in GBP.</param>
        private SagePayDirect BuildRepeatEntity(int orderId, decimal amount, string cv2, string currencyCode, decimal exchangeRate)
        {
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            var vendor = _sagepaySettings.SagePayVendor;

            var spDirect = new SagePayDirect
            {
                OrderId = orderId,
                VPSProtocol = vpsProtocol,
                TxType = SagePayAccountTxType.REPEAT,
                Vendor = vendor,
                // Format for VendorTxCode = REPEAT-[ddMMyyyyHHmmss]
                VendorTxCode = string.Format(TX_CODE_FORMAT, SagePayAccountTxType.REPEAT, DateTime.Now.ToString("ddMMyyyyHHmmss")),
                Amount = amount,
                CV2 = cv2,
                Currency = currencyCode,
                ExchangeRate = exchangeRate
            };

            return spDirect;
        }

        /// <summary>
        /// Build release entity.
        /// </summary>
        /// <param name="amount">Amount in GBP.</param>
        private SagePayDirect BuildReleaseEntity(int orderId, decimal amount, string vendorTxCode, string vpsTxId, string securityKey, string txAuthNo, string currencyCode, decimal exchangeRate)
        {
            var vpsProtocol = _sagepaySettings.SagePayVPSProtocol;
            var vendor = _sagepaySettings.SagePayVendor;

            var spDirect = new SagePayDirect
            {
                OrderId = orderId,
                VPSProtocol = vpsProtocol,
                TxType = SagePayAccountTxType.RELEASE,
                Vendor = vendor,
                VendorTxCode = vendorTxCode,
                VPSTxId = vpsTxId,
                SecurityKey = securityKey,
                TxAuthNo = txAuthNo,
                Amount = amount,
                Currency = currencyCode,
                ExchangeRate = exchangeRate
            };

            return spDirect;
        }

        #endregion

        #region Helper methods

        private SagePayDirect RemoveCardInformation(SagePayDirect sagePayEntity)
        {
            sagePayEntity.CardHolder = null;
            sagePayEntity.CardNumber = null;
            sagePayEntity.StartMonth = null;
            sagePayEntity.StartYear = null;
            sagePayEntity.ExpiryMonth = null;
            sagePayEntity.ExpiryYear = null;
            sagePayEntity.IssueNumber = null;

            return sagePayEntity;
        }

        private void UpdateDatabaseLog(List<SagePayLog> logs)
        {
            _sagePayLogRepository.Create(logs);
            logs.Clear();
        }

        private void UpdatePaymentEntity(object entity)
        {
            SagePayDirect sagePayEntity = (SagePayDirect)entity;
            _sagePayDirectRepository.Update(sagePayEntity);
        }

        private string PerformHttpRequestResponse(int orderId, byte[] content, HttpWebRequest request, List<SagePayLog> logs)
        { 
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            request.Method = WebRequestMethods.Http.Post;
            request.KeepAlive = false;
            request.ContentType = APPLICATION_FORM_URLENCODED;
            request.ContentLength = content.Length;

            Stream writer = request.GetRequestStream();
            writer.Write(content, 0, content.Length);
            writer.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            logs.Append(orderId, string.Empty, SagePayProgressStatus.REQUEST_SENT);

            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
            string responseData = reader.ReadToEnd();
            reader.Close();

            return responseData;
        }

        private StringBuilder AppendNameValue(StringBuilder sb, string name, string value)
        {
            if (sb.ToString() == string.Empty)
                return sb.AppendFormat(FIRST_NAME_VALUE_FORMAT, name, value);

            return sb.AppendFormat(NAME_VALUE_FORMAT, name, value);
        }

        private void GetNameInParts(string fullName, int maxLength, out string surname, out string firstnames)
        {
            string[] bNameParts = fullName.Split(' ');
            surname = string.Empty;
            firstnames = string.Empty;

            if (bNameParts.Length > 1)
            {
                surname = bNameParts[bNameParts.Length - 1];

                for (int i = 0; i < bNameParts.Length - 1; i++)
                    firstnames = firstnames + bNameParts[i] + " ";
            }
            else
            {
                surname = fullName;
                firstnames = fullName;
            }

            surname = surname.Trim();
            firstnames = firstnames.Trim();
        }

        private string TruncateString(int maxLength, string input)
        {
            if (input.Length > maxLength)
                input = input.Substring(0, maxLength);

            return input;
        }

        private bool GetAVSCheckStatus(string avsCheck)
        {
            switch (avsCheck)
            {
                case SagePayAVSCV2.ALL_MATCH:                
                case SagePayAVSCV2.ADDRESS_MATCH_ONLY:
                    return true;                
                case SagePayAVSCV2.SECURITY_CODE_MATCH_ONLY:
                case SagePayAVSCV2.NO_DATA_MATCHES:
                case SagePayAVSCV2.DATA_NOT_CHECKED:
                default:
                    return false;
            }
        }

        private bool Get3DCheckStatus(string threeDCheck)
        {
            switch (threeDCheck)
            {
                case SagePay3DSecureStatus.OK:
                    return true;
                default:
                case SagePay3DSecureStatus.NOAUTH:
                case SagePay3DSecureStatus.CANTAUTH:
                case SagePay3DSecureStatus.NOTAUTHED:
                case SagePay3DSecureStatus.ATTEMPTONLY:
                case SagePay3DSecureStatus.NOTCHECKED:
                case SagePay3DSecureStatus.INCOMPLETE:
                case SagePay3DSecureStatus.MALFORMED:
                case SagePay3DSecureStatus.INVALID:
                case SagePay3DSecureStatus.ERROR:
                    return false;
            }
        }

        private string RemoveSemiColon(string input)
        {
            return input.Replace(SEMI_COLON, WHITE_SPACE);
        }

        private string RemoveSlash(string input)
        {
            return input.Replace(BACKSLASH, WHITE_SPACE).Replace(FORWARDSLASH, WHITE_SPACE);
        }

        private string FindField(string fieldName, string responseData)
        {
            string[] items = responseData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string target = fieldName + EQUAL;

            for (int i = 0; i < items.Length; i++)
                if (items[i].Contains(target))
                    return items[i].Replace(target, string.Empty);

            return string.Empty;
        }

        private decimal RoundPrice(decimal value, int decimalPlace)
        {
            return Math.Round(value, decimalPlace, MidpointRounding.AwayFromZero);
        }

        private string GetXmlString(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            StringWriter sw = new StringWriter();
            using (XmlWriter xw = XmlWriter.Create(sw))
            {
                WriteException(xw, "exception", exception);
            }
            return sw.ToString();
        }

        private void WriteException(XmlWriter writer, string name, Exception exception)
        {
            if (exception == null) return;
            writer.WriteStartElement(name);
            writer.WriteElementString("message", exception.Message);
            writer.WriteElementString("source", exception.Source);
            WriteException(writer, "innerException", exception.InnerException);
            writer.WriteEndElement();
        }

        private string GetBasketItemContentString(ICollection<LineItem> items, bool isEC, decimal shippingCost, decimal discount, decimal orderTotal)
        {
            StringBuilder sb = new StringBuilder();

            int numberOfLines = items.Count + 2;
            decimal totalTax = 0M;
            
            foreach (var item in items)
            {
                totalTax = totalTax + ((item.PriceInclTax - item.PriceExclTax) * item.Quantity);

                sb.Append(SEMI_COLON).Append(RemoveSemiColon(item.Name));
                sb.Append(SEMI_COLON).Append(item.Quantity);
                sb.Append(SEMI_COLON).AppendFormat(PRICE_FORMAT, item.PriceExclTax);
                sb.Append(SEMI_COLON).AppendFormat(PRICE_FORMAT, isEC ? (item.PriceInclTax - item.PriceExclTax) : 0M);
                sb.Append(SEMI_COLON).AppendFormat(PRICE_FORMAT, isEC ? item.PriceInclTax : item.PriceExclTax);
                sb.Append(SEMI_COLON).AppendFormat(PRICE_FORMAT, (isEC ? item.PriceInclTax : item.PriceExclTax) * item.Quantity);

                if (sb.Length > BASKET_MAX_LENGTH)
                    break;
            }

            if (discount > 0M)
            {
                numberOfLines++;
                sb.AppendFormat(DISCOUNT_LINE_FORMAT, discount);
            }

            totalTax = RoundPrice(totalTax, 2);

            if (sb.Length < 7440)
            {
                sb.AppendFormat(DELIVERY_LINE_FORMAT, shippingCost);
                sb.AppendFormat(ORDER_TOTAL_LINE_FORMAT, orderTotal);
                sb.Insert(0, numberOfLines);

                return sb.ToString();
            }
            else
                return string.Empty;
        }

        #endregion
    }
}
