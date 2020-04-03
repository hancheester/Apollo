using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.Core.Services.Security;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Services.Payment
{
    public class PaymentService : BaseRepository, IPaymentService
    {
        private const int ORDERS_IN_LAST_NUMBER_OF_DAYS = 14;

        #region Fields

        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Address> _addressRepository;        
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<EmailInvoice> _emailInvoiceRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderPayment> _orderPaymentRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<SystemCheck> _systemCheckRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;
        private readonly IPaymentSystemService _paymentSystem;
        private readonly IEmailManager _emailManager;
        private readonly ISystemCheckService _systemCheckService;
        private readonly IAddressBuilder _addressBuilder;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public PaymentService(  
            IRepository<Account> accountRepository,
            IRepository<Address> addressRepository,
            IRepository<Product> productRepository,
            IRepository<Brand> brandRepository,
            IRepository<EmailInvoice> emailInvoiceRepository,
            IRepository<ProductPrice> productPriceRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderPayment> orderPaymentRepository,
            IRepository<LineItem> lineItemRepository,
            IRepository<SystemCheck> systemCheckRepository,
            IRepository<Country> countryRepository,
            IRepository<USState> usStateRepository,
            IOrderService orderService,
            IPaymentSystemService paymentSystem,
            IAccountService accountService,
            IEmailManager emailManager,
            ILogBuilder logBuilder,
            ISystemCheckService systemCheckService,
            IAddressBuilder addressBuilder,
            ICacheManager cacheManager)
        {
            _accountRepository = accountRepository;
            _addressRepository = addressRepository;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _emailInvoiceRepository = emailInvoiceRepository;
            _productPriceRepository = productPriceRepository;
            _orderRepository = orderRepository;            
            _orderPaymentRepository = orderPaymentRepository;
            _lineItemRepository = lineItemRepository;
            _systemCheckRepository = systemCheckRepository;
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _orderService = orderService;
            _accountService = accountService;
            _paymentSystem = paymentSystem;
            _emailManager = emailManager;            
            _systemCheckService = systemCheckService;
            _addressBuilder = addressBuilder;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Command

        public TransactionOutput ProcessPaymentAfter3DCallback(string md, string paRes, bool sendEmailFlag)
        {
            var paymentEntity = _paymentSystem.GetPaymentEntityByMD(md);

            // If Completed is set to true, it means the transaction is completed and we shouldn't process this transaction again.
            if (paymentEntity.Completed)
                return new TransactionOutput
                {
                    Status = true,
                    EmailInvoiceId = paymentEntity.EmailInvoiceId != 0 ? paymentEntity.EmailInvoiceId : 0,
                    OrderId = paymentEntity.EmailInvoiceId == 0 ? paymentEntity.OrderId : 0
                };

            paymentEntity.PAReq = paRes;
            _paymentSystem.UpdatePaymentEntityForPAReq(paymentEntity.Id, paRes);

            // Process card payment
            var output = _paymentSystem.ProcessPaymentAfter3DCallback(paymentEntity);

            // It could be for Order or Email Invoice, but Email Invoice will be the first priority.
            if (paymentEntity.EmailInvoiceId != 0)
            {
                var emailInvoice = _emailInvoiceRepository.Return(paymentEntity.EmailInvoiceId);
                emailInvoice = BuildEmailInvoice(emailInvoice);

                output.EmailInvoiceEncodedKey = emailInvoice.EncodedKey;
                ProcessPaymentOutputForEmailInvoice(emailInvoice, output, sendEmailFlag);
            }
            else
            {
                var order = _orderService.GetCompleteOrderById(paymentEntity.OrderId);

                // Set order as discarded with status code if it still requires 3D secure redirect.
                if (output.Has3DSecure)
                {
                    order.StatusCode = OrderStatusCode.DISCARDED;
                    _orderRepository.Update(order);

                    // Set line item as cancelled with status code
                    var list = order.LineItemCollection.Select(l => l.Id).ToList();
                    _orderService.UpdateLineItemStatusCodeByLineItemIdList(list, LineStatusCode.CANCELLED);
                }
                else
                {
                    ProcessPaymentOutputForOrder(order, output, sendEmailFlag);
                }
            }            

            return output;
        }

        // TODO: In future, we might need to process payment from single basket page. 
        // ProcessPaymentFromSingleBasketPage(billing address, shipping address, cart items,....) ??

        public TransactionOutput ProcessPaymentFromEmailInvoice(int emailInvoiceId,
                                                                Address billingAddress,
                                                                Card card,
                                                                string userAgent,
                                                                string clientIPAddress,
                                                                bool sendEmailFlag)
        {
            var emailInvoice = _emailInvoiceRepository.Return(emailInvoiceId);
            emailInvoice = BuildEmailInvoice(emailInvoice);

            emailInvoice.BillTo = billingAddress.Name;
            emailInvoice.AddressLine1 = billingAddress.AddressLine1;
            emailInvoice.AddressLine2 = billingAddress.AddressLine2;
            emailInvoice.City = billingAddress.City;
            emailInvoice.County = billingAddress.County;
            emailInvoice.PostCode = billingAddress.PostCode;

            emailInvoice.CountryId = billingAddress.CountryId;
            emailInvoice.Country = _countryRepository.Return(billingAddress.CountryId);

            emailInvoice.USStateId = billingAddress.USStateId;
            emailInvoice.USState = _usStateRepository.Return(billingAddress.USStateId);

            emailInvoice.IPAddress = clientIPAddress;

            // Update email invoice
            _emailInvoiceRepository.Update(emailInvoice);

            // Create payment entity
            var paymentEntity = _paymentSystem.BuildPaymentEntity(emailInvoice.PaymentRef,
                                                                  emailInvoice.Amount,
                                                                  emailInvoice.ExchangeRate,
                                                                  emailInvoice.CurrencyCode,
                                                                  card,
                                                                  emailInvoice.Email,
                                                                  emailInvoice.ContactNumber,
                                                                  userAgent,
                                                                  emailInvoice.IPAddress,
                                                                  orderId: emailInvoice.OrderId,
                                                                  emailInvoiceId: emailInvoice.Id);

            var output = ProcessPaymentForEmailInvoice(emailInvoice, paymentEntity, sendEmailFlag);

            return output;
        }
        
        public TransactionOutput ProcessPaymentFromBackOffice(int profileId,
                                                              string paymentMethod,
                                                              string email,
                                                              string contactNumber,
                                                              string userAgent,
                                                              string clientIPAddress,
                                                              Card card,
                                                              Address billingAddress,
                                                              Address shippingAddress,
                                                              bool sendEmailFlag,
                                                              bool exemptedFromPayment = false)
        {
            var order = _orderService.CreateOrderFromCart(profileId, paymentMethod, clientIPAddress, billingAddress, shippingAddress);

            if (exemptedFromPayment == false)
            {
                // Create payment entity       
                var paymentEntity = _paymentSystem.BuildPaymentEntityForBackOffice(order.Id.ToString(),
                                                                                    order.GrandTotal / order.ExchangeRate,
                                                                                    order.ExchangeRate,
                                                                                    order.CurrencyCode,
                                                                                    card,
                                                                                    email,
                                                                                    contactNumber,
                                                                                    userAgent,
                                                                                    clientIPAddress,
                                                                                    orderId: order.Id);

                return ProcessPaymentForOrder(order, paymentEntity, sendEmailFlag);
            }
            else
            {
                var output = new TransactionOutput
                {
                    Status = true
                };

                return ProcessPaymentOutputForOrder(order, output, sendEmailFlag);
            }
        }

        public TransactionOutput ProcessPaymentFromCart(int profileId,
                                                        string paymentMethod,
                                                        string email,
                                                        string contactNumber,
                                                        string userAgent,
                                                        string clientIPAddress,
                                                        Card card,
                                                        bool sendEmailFlag)
        {
            try
            {
                // Get chosen addresses by profile id           
                var accountId = GetAccountIdByProfileId(profileId);
                var billingAddress = GetBillingAddressByAccountId(accountId);
                var shippingAddress = GetShippingAddressByAccountId(accountId);
                var order = _orderService.CreateOrderFromCart(profileId, paymentMethod, clientIPAddress, billingAddress, shippingAddress);

                // Create payment entity       
                var paymentEntity = _paymentSystem.BuildPaymentEntity(order.Id.ToString(),
                                                                      order.GrandTotal / order.ExchangeRate,
                                                                      order.ExchangeRate,
                                                                      order.CurrencyCode,
                                                                      card,
                                                                      email,
                                                                      contactNumber,
                                                                      userAgent,
                                                                      clientIPAddress,
                                                                      orderId: order.Id);

                return ProcessPaymentForOrder(order, paymentEntity, sendEmailFlag);
            }
            catch (ApolloException ApolloEx)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("{0}. Profile ID={{{1}}}", ApolloEx.Message, profileId), ApolloEx);
                return new TransactionOutput { Status = false, Message = ApolloEx.Message };
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
            var output = _paymentSystem.ProcessOrderCharging(orderPayment);

            if (output.Status && output.PaymentReleased)
            {
                orderPayment.IsCompleted = true;

                // Create order payment if payment is released
                _orderPaymentRepository.Create(orderPayment);
            }

            return output;
        }

        #endregion

        #region Private methods

        private int GetAccountIdByProfileId(int profileId)
        {
            var accountId = _accountRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Select(x => x.Id)
                .FirstOrDefault();

            return accountId;
        }

        private Address GetBillingAddressByAccountId(int accountId)
        {
            var item = _addressRepository.Table
                .Where(a => a.AccountId == accountId && a.IsBilling == true)
                .FirstOrDefault();
            return _addressBuilder.Build(item);
        }

        private Address GetShippingAddressByAccountId(int accountId)
        {
            var item = _addressRepository.Table
                .Where(a => a.AccountId == accountId && a.IsShipping == true)
                .FirstOrDefault();
            return _addressBuilder.Build(item);
        }

        private TransactionOutput ProcessPaymentForEmailInvoice(EmailInvoice emailInvoice, PaymentEntity paymentEntity, bool sendEmailFlag)
        {
            _paymentSystem.InsertPaymentEntity(paymentEntity);

            var output = _paymentSystem.ProcessCardPayment(emailInvoice, paymentEntity);

            ProcessPaymentOutputForEmailInvoice(emailInvoice, output, sendEmailFlag);

            return output;
        }

        private TransactionOutput ProcessPaymentForOrder(Order order, PaymentEntity paymentEntity, bool sendEmailFlag)
        {
            if (order == null)
            {
                return new TransactionOutput
                {
                    Message = "Sorry, it is failed to create an order."
                };
            }

            // Check if there is any item in basket
            if (order.LineItemCollection.Count == 0)
            {
                return new TransactionOutput
                {
                    Message = "Sorry, there is no available item."
                };
            }

            // Stock checking
            StockStatus stockStatus = ProcessStockChecking(order.LineItemCollection);

            if (stockStatus.HasOutOfStockItem)
            {
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < stockStatus.CartItemIdList.Count; i++)
                {
                    var item = _lineItemRepository.Return(stockStatus.CartItemIdList[i]);
                    sb.AppendFormat("- {0}<br/>", item.Name);
                }
                
                return new TransactionOutput {
                    Message = "Sorry, items below are out of stock.<br/>" + sb.ToString(),
                    StockStatus = stockStatus
                };
            }
            
            _paymentSystem.InsertPaymentEntity(paymentEntity);

            // If there is stock alert, process payment entity accordingly
            if (stockStatus.IsOutOfStock)
                paymentEntity = _paymentSystem.UpdateAndGetPaymentEntityForOutOfStock(paymentEntity.Id);

            // Process card payment
            var output = _paymentSystem.ProcessCardPayment(order, paymentEntity);

            // Process transaction output
            output = ProcessPaymentOutputForOrder(order, output, sendEmailFlag);

            return output;
        }

        private void ProcessPaymentOutputForEmailInvoice(EmailInvoice emailInvoice, TransactionOutput output, bool sendEmailFlag)
        {
            if (output.Status && !output.Has3DSecure)
            {
                // Create order payment
                var orderPayment = new OrderPayment
                {
                    OrderId = emailInvoice.OrderId,
                    TimeStamp = DateTime.Now,
                    Amount = emailInvoice.Amount,
                    CurrencyCode = emailInvoice.CurrencyCode,
                    ExchangeRate = emailInvoice.ExchangeRate,
                    IsCompleted = true
                };
                
                int orderPaymentId = _orderPaymentRepository.Create(orderPayment);

                // Update email invoice
                emailInvoice.OrderPaymentId = orderPaymentId;
                emailInvoice.Paid = true;
                emailInvoice.DatePaid = DateTime.Now;

                _emailInvoiceRepository.Update(emailInvoice);

                // Save to comment
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<b>{0}</b>", "Payment Complete");

                string address = BuildEmailInvoiceBillingAddress(emailInvoice);
                string amount = string.Format("{0}{1:0.00}",
                    emailInvoice.CurrencyCode,
                    Math.Round(emailInvoice.Amount * emailInvoice.ExchangeRate, 2, MidpointRounding.AwayFromZero));
                string message = string.Format("Payment Reference:<br/>{0}<br/><br/>Ammount:<br/>{1}<br/><br/>{2}",
                                               emailInvoice.PaymentRef,
                                               amount,
                                               address);

                _orderService.ProcessOrderCommentInsertion(emailInvoice.OrderId,
                                                           emailInvoice.ProfileId,
                                                           emailInvoice.FirstName,
                                                           "Payment Complete",
                                                           message,
                                                           string.Empty);

                // Send confirmation email if needed
                if (sendEmailFlag)                
                    _emailManager.SendPaymentInvoiceConfirmationEmail(emailInvoice.Email, emailInvoice.FirstName, amount);
            }
        }

        private TransactionOutput ProcessPaymentOutputForOrder(Order order, TransactionOutput output, bool sendEmailFlag)
        {
            // If response status is success and it is not 3D secure then update stock
            if (output.Status && !output.Has3DSecure)
            {
                // Update stock                
                TakeStock(order.LineItemCollection);

                // Set order as paid with status code
                order.Paid = true;
                order.DatePaid = DateTime.Now;
                order.StatusCode = OrderStatusCode.ORDER_PLACED;

                AccountOverviewModel account = null;
                if (order.ProfileId > 0)
                    account = _accountService.GetAccountOverviewModelByProfileId(order.ProfileId);

                // Take loyalty point first
                if (account != null && order.AllocatedPoint > 0)
                {
                    _accountService.InsertRewardPointHistory(account.Id,
                        message: string.Format("Reduced for order ID {0}.", order.Id),
                        orderId: order.Id,
                        usedPoints: order.AllocatedPoint);
                }
                
                // Set line item as ordered with status code
                var list = order.LineItemCollection.Select(l => l.Id).ToList();
                _orderService.UpdateLineItemStatusCodeByLineItemIdList(list, LineStatusCode.ORDERED);

                // Assign order id to output
                output.OrderId = order.Id;

                // Create order payment if payment is released
                if (output.PaymentReleased)
                {
                    var orderPayment = new OrderPayment
                    {
                        OrderId = order.Id,
                        TimeStamp = DateTime.Now,
                        Amount = order.GrandTotal / order.ExchangeRate, // In GBP
                        CurrencyCode = order.CurrencyCode,
                        ExchangeRate = order.ExchangeRate,
                        IsCompleted = true
                    };

                    _orderPaymentRepository.Create(orderPayment);
                }

                // Get name and email
                string name = string.Empty;
                string email = string.Empty;

                if (account != null)
                {
                    name = account.Name;
                    email = account.Email;
                }
                else if (!string.IsNullOrEmpty(output.Email))
                {
                    name = order.BillTo;
                    email = output.Email;
                }

                // Perform system check
                var systemCheck = _systemCheckService.ProcessSystemChecking(order, email, name, output.AVSCheck);
                if (systemCheck != null)
                {
                    _systemCheckRepository.Create(systemCheck);

                    if (!systemCheck.AvsCheck
                        || !systemCheck.BillingAddressCheck
                        || !systemCheck.ShippingAddressCheck
                        || !systemCheck.EmailCheck
                        || !systemCheck.BillingPostCodeCheck
                        || !systemCheck.ShippingPostCodeCheck
                        || !systemCheck.NameCheck
                        || !systemCheck.BillingNameCheck
                        || !systemCheck.ShippingNameCheck)
                    {
                        order.StatusCode = OrderStatusCode.ON_HOLD;
                        order.IssueCode = OrderIssueCode.SYSTEM_CHECK_FAILED;
                    }
                }
                
                // Flag order which user has more than 2 orders in the last number of days                
                int lastNumberOfDaysOrders = _orderService.GetLastNumberOfDaysOrdersByProfileId(order.ProfileId, ORDERS_IN_LAST_NUMBER_OF_DAYS);
                if(lastNumberOfDaysOrders > 2)
                {
                    order.StatusCode = OrderStatusCode.ON_HOLD;

                    string message = string.Format("Number of orders placed in the last {1} days is {0}.", lastNumberOfDaysOrders, ORDERS_IN_LAST_NUMBER_OF_DAYS);

                    _orderService.ProcessOrderCommentInsertion(order.Id,
                                                               0,
                                                               "Apollo WebStore",
                                                               "Suspected Trade Customer",
                                                               message,
                                                               string.Empty);
                }

                // Flag OTC order
                if (order.PharmOrder != null)
                {
                    order.StatusCode = OrderStatusCode.ON_HOLD;
                    if (order.IssueCode == OrderIssueCode.SYSTEM_CHECK_FAILED)
                        order.IssueCode = OrderIssueCode.OTC_ORDER_AND_SYSTEM_CHECK_FAILED;
                    else
                        order.IssueCode = OrderIssueCode.OTC_ORDER;
                }
                
                // Send order confirmation email if needed
                // Anonymous order will have user id as 0.                
                if (sendEmailFlag && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
                {
                    _orderService.SendOrderConfirmationEmail(order, name, email);
                }
            }

            // If transaction is failed, 
            // check whether the status is already flagged with positive status, eg. OP, OH, from previous transaction
            if (!output.Status) output.Status = HasPositiveStatus(order.Id);

            // Set order as discarded with status code D
            if (!output.Status)
            {
                order.StatusCode = OrderStatusCode.DISCARDED;
                
                // Set line item as cancelled with status code C
                var list = order.LineItemCollection.Select(l => l.Id).ToList();
                _orderService.UpdateLineItemStatusCodeByLineItemIdList(list, LineStatusCode.CANCELLED);
            }

            _orderRepository.Update(order);

            return output;
        }

        private string BuildEmailInvoiceBillingAddress(EmailInvoice emailInvoice)
        {
            var sb = new StringBuilder();

            sb.Append("Billing address:<br/>");
            sb.AppendFormat("{0}<br/>", emailInvoice.BillTo);
            sb.AppendFormat("{0}<br/>", emailInvoice.AddressLine1);
            sb.AppendFormat("{0}<br/>", emailInvoice.AddressLine2);
            sb.AppendFormat("<br/>", emailInvoice.City);
            if (emailInvoice.USState != null)
                sb.AppendFormat("{0}<br/>", emailInvoice.USState.State);
            sb.AppendFormat("{0}<br/>", emailInvoice.PostCode);
            sb.AppendFormat("{0}<br/>", emailInvoice.Country.Name);

            return sb.ToString();
        }

        private EmailInvoice BuildEmailInvoice(EmailInvoice emailInvoice)
        {
            emailInvoice.CountryId = emailInvoice.CountryId;
            emailInvoice.Country = _countryRepository.Return(emailInvoice.CountryId);

            emailInvoice.USStateId = emailInvoice.USStateId;
            emailInvoice.USState = _usStateRepository.Return(emailInvoice.USStateId);

            return emailInvoice;
        }
       
        private bool HasPositiveStatus(int orderId)
        {
            string[] VALID_STATUSES = new string[] { OrderStatusCode.ORDER_PLACED,
                                                    OrderStatusCode.PARTIAL_SHIPPING,
                                                    OrderStatusCode.SHIPPING,
                                                    OrderStatusCode.STOCK_WARNING,
                                                    OrderStatusCode.INVOICED,
                                                    OrderStatusCode.ON_HOLD };

            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                for (int i = 0; i < VALID_STATUSES.Length; i++)
                {
                    if (VALID_STATUSES[i] == order.StatusCode)
                        return true;
                }
            }

            return false;
        }
        
        private StockStatus ProcessStockChecking(ICollection<LineItem> lineItems)
        {
            StockStatus status = new StockStatus();

            foreach (var item in lineItems)
            {
                Product product = _productRepository.Return(item.ProductId);

                if (product != null)
                {
                    // We assume that if a product is found, we can almost be certain that other related entities are available.
                    var brand = _brandRepository.Return(product.BrandId);
                    var price = _productPriceRepository.Return(item.ProductPriceId);

                    if (price.Stock <= 0)
                    {
                        status.IsOutOfStock = true;

                        // EnforceStockCount priority Brand < Product. Product has higher priority (final say) than Brand.
                        if (product.EnforceStockCount || (!product.EnforceStockCount && brand.EnforceStockCount))
                        {
                            status.CartItemIdList.Add(item.Id);
                            status.HasOutOfStockItem = true;
                        }
                    }
                }
                else
                {
                    status.CartItemIdList.Add(item.Id);
                    status.HasOutOfStockItem = true;
                }
            }

            return status;
        }

        private void TakeStock(ICollection<LineItem> lineItems)
        {
            foreach (var item in lineItems)
            {
                var price = _productPriceRepository.Return(item.ProductPriceId);
                price.Stock = price.Stock - item.Quantity;
                _productPriceRepository.Update(price);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);
        }
        
        #endregion
    }
}
