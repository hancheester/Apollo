using Apollo.Core;
using Apollo.Core.Domain;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models.Checkout;
using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class CheckoutController : BasePublicController
    {
        private const string US = "US";

        private readonly IAccountService _accountService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IShippingService _shippingService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly HttpContextBase _httpContext;
        private readonly StoreInformationSettings _storeInfomationSettings;
        private readonly ApolloSessionState _session;

        public CheckoutController(            
            IAccountService accountService,
            IPaymentService paymentService,
            ICartService cartService,
            IOrderService orderService,
            IShippingService shippingService,
            IProductService productService,
            IWorkContext workContext,
            IPriceFormatter priceFormatter,
            HttpContextBase httpContext,
            StoreInformationSettings storeInfomationSettings,
            ApolloSessionState session)
        {
            _accountService = accountService;
            _paymentService = paymentService;
            _cartService = cartService;
            _orderService = orderService;
            _shippingService = shippingService;
            _productService = productService;
            _workContext = workContext;
            _httpContext = httpContext;
            _priceFormatter = priceFormatter;
            _storeInfomationSettings = storeInfomationSettings;
            _session = session;
        }

        #region Methods (common)

        public ActionResult Index()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            return View();
        }
        
        public ActionResult Completed(int? orderid, int? emailinvoiceid, int? hasnhs)
        {
            if (emailinvoiceid.Value == 0 && _workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();
            
            var thankYouTitle = string.Empty;
            var message = string.Empty;
            // TODO: To implement Trustbadge code
            //var enableReview = true;

            if (emailinvoiceid.HasValue && emailinvoiceid.Value <= 0 && orderid.HasValue && orderid.Value > 0)
            {
                thankYouTitle = string.Format("Your order number is {0}.", orderid.Value);
                
                if (hasnhs.HasValue && hasnhs.Value == 1)
                {
                    message = string.Format(@"<h4>Now post your prescription to Apollo.</h4>
                            <p>Write your order reference number (<b>{0}</b>) on the top of your prescription.<br/>
                              Send your original prescription to the address below. You do not need to attach a stamp.              
                            </p>
                            <p><b>
                                FREEPOST RSRB-YBYA-EGBL,<br />
                                Apollo Prescriptions,<br />                
                                Unit 27, Orbital 25 Business Park,<br />
                                Dwight Road, Watford,<br />
                                Herts WD18 9DA
                               </b>
                            </p>", orderid.Value);
                }
            }

            //if (emailinvoiceid.HasValue && emailinvoiceid.Value > 0)
            //    enableReview = false;

            //if (_workContext.CurrentProfile.IsAnonymous)
            //    enableReview = false;
            
            var model = new CheckoutCompletedModel
            {
                OrderId = orderid,
                EmailInvoiceId = emailinvoiceid,
                ThankYouTitle = thankYouTitle,
                Message = message
            };

            if (orderid.HasValue && orderid.Value > 0 && _workContext.CurrentProfile.IsAnonymous == false)
            {
                var profileId = _workContext.CurrentProfile.Id;
                //var order = _orderService.GetCompleteOrderByIdAndProfileId(orderid.Value, profileId);
                //var account = _accountService.GetAccountByProfileId(profileId);

                //model.Email = account.Email;
                //model.Amount = string.Format("{0:0.00}", order.GrandTotal);
                //model.CurrencyCode = order.CurrencyCode;
                //model.PaymentType = order.PaymentMethod;                

                _cartService.ClearCart(profileId);
            }
            
            return View(model);
        }

        #endregion

        #region Methods (multistep checkout)

        #region 0. Pharmaceutical form

        [HttpGet]
        public ActionResult PharmForm()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            int profileId = _workContext.CurrentProfile.Id;
            bool hasPharmItem = _cartService.HasPharmItem(profileId);

            if (hasPharmItem == false)
            {
                return RedirectToRoute("Shopping Cart");
            }

            var pharmItems = _cartService.GetCartItemOverviewModelByProfileId(profileId, autoRemovePhoneOrderItems:true, isPharmaceutical: true);

            var model = new PharmOrderModel();

            // Owner ages
            for (int i = 1; i <= 100; i++)
            {
                model.OwnerAges.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                });
            }
            model.OwnerAges.Insert(0, new SelectListItem { Text = "< 1 year", Value = "< 1 year" });
            model.OwnerAges.Insert(0, new SelectListItem { Text = "Years", Value =  "" });

            // Load pharm items
            for(int i = 0; i < pharmItems.Count; i++)
            {
                var codeineMessage = string.Empty;
                if (_productService.BelongToThisProductGroup(pharmItems[i].ProductId, Convert.ToInt32(ProductGroupType.Codeine)))
                    codeineMessage = "This item contains codeine. It should not be used for longer than 3 days, if symptoms persist please contact your doctor. Please be aware codeine can be addictive.";

                var item = new PharmItemModel
                {
                    Name = pharmItems[i].Name,
                    ProductId = pharmItems[i].ProductId,
                    ProductPriceId = pharmItems[i].ProductPriceId,
                    Note = codeineMessage
                };

                // Owner ages
                for (int j = 1; j <= 100; j++)
                {
                    item.AvailableAges.Add(new SelectListItem
                    {
                        Text = j.ToString(),
                        Value = j.ToString(),
                    });
                }
                item.AvailableAges.Insert(0, new SelectListItem { Text = "< 1 year", Value = "< 1 year" });
                item.AvailableAges.Insert(0, new SelectListItem { Text = "Years", Value = "" });

                // Persisted for
                for (int k = 1; k <= 120; k++)
                {
                    item.AvailablePersistedInDays.Add(new SelectListItem
                    {
                        Text = k.ToString(),
                        Value = k.ToString(),
                    });
                }
                item.AvailablePersistedInDays.Add(new SelectListItem { Text = "> 120 days", Value = "> 120 days" });
                item.AvailablePersistedInDays.Insert(0, new SelectListItem { Text = "Days", Value = "" });

                // Taken quantity
                for(int l = 1; l < 11; l++)
                {
                    item.AvailableTakenQuantity.Add(new SelectListItem
                    {
                        Text = l.ToString(),
                        Value = l.ToString(),
                    });
                }
                item.AvailableTakenQuantity.Add(new SelectListItem { Text = "> 10", Value = "> 10" });

                model.PharmItems.Add(item);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmPharmForm(PharmOrderModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            int profileId = _workContext.CurrentProfile.Id;

            var entity = model.PrepareCartPharmOrderEntity(profileId);

            for (int i = 0; i < model.PharmItems.Count; i++)
            {
                entity.Items.Add(model.PharmItems[i].PrepareCartPharmItemEntity());
            }

            _cartService.ProcessCartPharmOrder(entity);

            return View();
        }

        #endregion

        #region 1. Address confirmation

        public ActionResult ConfirmAddress()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var profileId = _workContext.CurrentProfile.Id;
            // Check if customer needs to fill in a pharmaceutical form
            var needToFillForm = _cartService.CheckIfNeedPharmForm(profileId);

            if (needToFillForm)
                return RedirectToRoute("Checkout Pharm Form");
            
            var allAddresses = _accountService.GetAddressesByProfileId(profileId);
            var billing = allAddresses.Where(x => x.IsBilling == true).FirstOrDefault();
            var shipping = allAddresses.Where(x => x.IsShipping == true).FirstOrDefault();            
            var hasOnlyFreeNHSPrescriptionItem = _cartService.HasOnlyFreeNHSPrescriptionItem(profileId);

            // If we have only 1 item which is a NHS Prescription, then we do not need to have billing address
            var needBillingAddress = !hasOnlyFreeNHSPrescriptionItem;

            // For FOC order, do not display billing address
            var orderTotals = _cartService.CalculateOrderTotals(profileId);
            if (orderTotals.Total <= 0M) needBillingAddress = false;

            if (billing == null && shipping == null)
            {
                // Choose a default address
                if (allAddresses.Count > 0)
                {
                    billing = allAddresses[0];
                    shipping = allAddresses[0];
                    _accountService.UpdatePrimaryBillingAddress(billing.Id, profileId: profileId);
                    _accountService.UpdatePrimaryShippingAddress(shipping.Id, profileId: profileId);
                }
                else
                {
                    return RedirectToRoute("Checkout New Address", new { type = Convert.ToInt32(AddressType.Both) });
                }
            }

            var model = new CheckoutAddressModel
            {
                NeedBillingAddress = needBillingAddress,
                HasSavedAddress = allAddresses.Count > 0
            };

            if (needBillingAddress && billing != null)
            {
                model.BillingAddress = billing.PrepareAddressModel();
                model.HasBillingAddress = true;
            }

            if (shipping != null)
            {
                model.ShippingAddress =  shipping.PrepareAddressModel();
                model.HasShippingAddress = true;

                // Current selected shipping option country has to match shipping address country.
                if (string.Compare(model.ShippingAddress.CountryId, _workContext.CurrentCountry.Id.ToString()) != 0)
                {
                    model.HasShippingAddress = false;
                    model.DisableProceed = true;
                    ViewBag.ErrorMessage = string.Format("Please change your country in <i>shipping address</i> to {0}. Alternatively, you may change your shipping country at <a href='{1}'>basket page</a>.", _workContext.CurrentCountry.Name, Url.RouteUrl("Shopping Cart"));
                }

                var messages = _cartService.ProcessPostalRestrictionRules(profileId, _workContext.CurrentCountry.ISO3166Code);
                if (messages.Length > 0)
                {
                    model.DisableProceed = true;
                    ViewBag.ErrorMessage = string.Join("<br/>", messages);
                }
            }
            
            if ((needBillingAddress && billing == null) || shipping == null)
                model.DisableProceed = true;

            return View(model);
        }
        
        [HttpGet]
        public ActionResult NewAddress(AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();
            
            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();

            var model = new AddressModel
            {
                AddressType = type,
                AvailableCountries = countries.PrepareCountries(),
                AvailableStates = states.PrepareStates()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewAddress(AddressModel model, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var profileId = _workContext.CurrentProfile.Id;
                var account = _accountService.GetAccountByProfileId(profileId);
                var address = new Address
                {
                    AccountId = account.Id,
                    Name = model.Name,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    County = model.County,
                    City = model.City,
                    PostCode = model.PostCode,
                    CountryId = Convert.ToInt32(model.CountryId),
                    CreatedOnDate = DateTime.Now,
                    UpdatedOnDate = DateTime.Now
                };

                var country = _shippingService.GetCountryById(address.CountryId);
                if (country.ISO3166Code == US)
                    address.USStateId = model.USStateId.Value;

                var allAddresses = _accountService.GetAddressesByAccountId(profileId);

                if (allAddresses.Count == 0 && type == AddressType.Both)
                {
                    address.IsBilling = true;
                    address.IsShipping = true;
                }

                _accountService.InsertAddress(address);

                if (allAddresses.Count == 0)
                {
                    return RedirectToRoute("Checkout Confirm Address");
                }
                
                _session["message"] = "Your address has been successfully added.";
                
                return RedirectToAction("AddressList", new { type = Convert.ToInt32(type) });
            }

            ModelState.AddModelError(string.Empty, "Sorry, there is something wrong with the address.");

            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();
            model.AvailableCountries = countries.PrepareCountries();
            model.AvailableStates = states.PrepareStates();

            return View(model);
        }

        [HttpGet]
        public ActionResult EditAddress(int addressId, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(addressId);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);
            if (address != null && address.AccountId == accountId)
            {
                var countries = _shippingService.GetActiveCountries();
                var states = _shippingService.GetUSStates();
                var model = address.PrepareAddressModel();

                model.AvailableCountries = countries.PrepareCountries();
                model.AvailableStates = states.PrepareStates();
                model.AddressType = type;

                return View(model);
            }
            
            _session["message"] = "Sorry, address could not be found. Pleasey try again.";

            return RedirectToAction("AddressList", new { type = Convert.ToInt32(type) });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAddress(AddressModel model, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();
            model.AvailableCountries = countries.PrepareCountries();
            model.AvailableStates = states.PrepareStates();

            if (ModelState.IsValid)
            {
                var profileId = _workContext.CurrentProfile.Id;
                var account = _accountService.GetAccountByProfileId(profileId);
                var address = _accountService.GetAddressById(model.Id);

                if (address != null && address.AccountId == account.Id)
                {
                    address.Name = model.Name;
                    address.AddressLine1 = model.AddressLine1;
                    address.AddressLine2 = model.AddressLine2;
                    address.City = model.City;
                    address.County = model.County;
                    address.PostCode = model.PostCode;
                    address.CountryId = Convert.ToInt32(model.CountryId);

                    var country = _shippingService.GetCountryById(address.CountryId);
                    if (country.ISO3166Code == US)
                        address.USStateId = model.USStateId.Value;
                    else
                        address.USStateId = 0;
                    
                    address.UpdatedOnDate = DateTime.Now;
                }

                _accountService.UpdateAddress(address);
                
                ViewBag.Message = "Your address has been successfully updated.";

                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Sorry, there is something wrong with the address.");
            
            return View(model);
        }

        [HttpGet]
        public ActionResult AddressList(AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var profileId = _workContext.CurrentProfile.Id;
            var account = _accountService.GetAccountByProfileId(profileId);            
            var shipping = _accountService.GetShippingAddressByAccountId(account.Id);

            if (shipping != null)
            {
                // Current selected shipping option country has to match shipping address country.
                if (shipping.CountryId != _workContext.CurrentCountry.Id)
                {
                    ViewBag.ErrorMessage = "Please change your country in <u>shipping address</u> to country " + _workContext.CurrentCountry.Name + ".";
                }

                var messages = _cartService.ProcessPostalRestrictionRules(profileId, _workContext.CurrentCountry.ISO3166Code);
                if (messages.Length > 0)
                {
                    ViewBag.ErrorMessage = string.Join("<br/>", messages);
                }
            }

            var allAddresses = _accountService.GetAddressesByProfileId(_workContext.CurrentProfile.Id);
            
            if (allAddresses.Count > 0)
            {
                var list = new List<AddressModel>();
                foreach (var item in allAddresses)
                {                    
                    list.Add(item.PrepareAddressModel());
                }

                var model = new CheckoutAddressListModel
                {
                    ExistingAddresses = list,
                    Type = type
                };

                if (string.IsNullOrEmpty(_session["message"] as string) == false)
                {
                    ViewBag.Message = _session["message"].ToString();
                    _session["message"] = null;
                }

                return View(model);
            }

            return RedirectToAction("NewAddress", "Checkout");
            
        }

        [HttpGet]
        public ActionResult SelectAddress(int addressId, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(addressId);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);
            if (address != null && address.AccountId == accountId)
            {
                switch (type)
                {
                    case AddressType.Billing:
                        _accountService.UpdatePrimaryBillingAddress(addressId, accountId: accountId);
                        break;
                    case AddressType.Shipping:
                    default:
                        _accountService.UpdatePrimaryShippingAddress(addressId, accountId: accountId);
                        break;
                }
            }

            return RedirectToAction("ConfirmAddress", "Checkout");
        }

        [HttpGet]
        public ActionResult RemoveAddress(int addressId, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(addressId);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);

            if (address != null && address.AccountId == accountId)
            {
                _accountService.DeleteAddressByAddressId(addressId);
                _session["message"] = "Your address has been successfully removed.";
            }

            return RedirectToAction("AddressList", new { type = Convert.ToInt32(type) });
        }

        #endregion

        #region 2. Order confirmation

        public ActionResult ConfirmOrder()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            return View();
        }

        #endregion

        #region 3. Payment

        /// <summary>
        /// If cart has only NHS Prescription item, we can just create an order without displaying payment page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Payment()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var profileId = _workContext.CurrentProfile.Id;
            var hasOnlyFreeNHSPrescriptionItem =_cartService.HasOnlyFreeNHSPrescriptionItem(profileId);

            if (hasOnlyFreeNHSPrescriptionItem)
            {
                return CreatePrescriptionOrder(profileId, _httpContext.Request.UserHostAddress);
            }

            var totalQuantity = _cartService.GetTotalQuantityCartItemByProfileId(profileId);
            var orderTotals = _cartService.CalculateOrderTotals(profileId);
            var orderTotal = 0M;

            if (orderTotals != null)            
                orderTotal = orderTotals.Total;

            if (orderTotal <= 0M && totalQuantity > 0)
            {
                return CreateFOCOrder(profileId, _httpContext.Request.UserHostAddress);
            }

            var model = new CheckoutPaymentModel
            {
                CardTypes = PrepareCardTypes(),
                ExpireYears = PrepareCardYears(),
                ExpireMonths = PrepareCardMonths(),
                OrderTotal = _priceFormatter.FormatPrice(orderTotal),
                DisableProceed = orderTotal <= 0M
            };
            
            // Check for any error from payment service
            if (string.IsNullOrEmpty(_session["PaymentErrorMessage"] as string) == false)
            {
                ViewBag.ErrorMessage = _session["PaymentErrorMessage"].ToString();
                _session["PaymentErrorMessage"] = null;
            }

            // Check for postal restriction rule
            var messages = _cartService.ProcessPostalRestrictionRules(profileId, _workContext.CurrentCountry.ISO3166Code);
            if (messages.Length > 0)
            {
                model.DisableProceed = true;

                if (string.IsNullOrEmpty(ViewBag.ErrorMessage as string))
                    ViewBag.ErrorMessage = string.Join("<br/>", messages);
                else
                    ViewBag.ErrorMessage += "<br/>" + string.Join("<br/>", messages);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConfirmPayment(CheckoutPaymentModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var profileId = _workContext.CurrentProfile.Id;
            var orderTotals = _cartService.CalculateOrderTotals(profileId);

            if (orderTotals == null || orderTotals.Total <= 0M)
            {
                _session["PaymentErrorMessage"] = "Failed to submit payment as basket value is zero.";
                RedirectToAction("Payment", "Checkout");
            }
            
            var card = new Card
            {
                CardType = model.CardType,
                CardNumber = model.CardNumber,
                HolderName = model.CardHolderName,
                SecurityCode = model.CardCode,
                ExpiryMonth = model.ExpireMonth,
                ExpiryYear = model.ExpireYear,
                StartMonth = string.Empty,
                StartYear = string.Empty,
                IssueNumber = string.Empty
            };

            var account = _accountService.GetAccountByProfileId(profileId);

            var output = _paymentService.ProcessPaymentFromCart(
                profileId,
                model.CardType,
                account.Email,
                account.ContactNumber,
                _httpContext.Request.UserAgent,
                _httpContext.Request.UserHostAddress == "::1" ? "45.1.1.1" : _httpContext.Request.UserHostAddress,
                card,
                sendEmailFlag: true);

            if (output == null)
            {
                _session["PaymentErrorMessage"] = "Failed to submit payment. Please refresh the page and try one more time.";
                return RedirectToAction("Payment", "Checkout");
            }

            if (output.Status == false)
            {
                _session["PaymentErrorMessage"] = output.Message;
                return RedirectToAction("Payment", "Checkout");
            }
                       
            // 3D Secure redirect?
            if (output.Has3DSecure)
            {
                RedirectToSecuredCardPage(output.RedirectUrl, output.PaReq, output.MD);                    
            }
            else
            {
                return RedirectToRoute("Checkout Completed", new { orderid = output.OrderId, emailinvoiceid = 0, hasnhs = output.HasNHSPrescription ? 1 : 0 });
            }
            
            return Content(string.Empty);
        }

        #endregion

        #endregion
        
        #region Methods (email invoice payment)

        public ActionResult InvoicePayment(string k)
        {
            if (string.IsNullOrEmpty(k)) return InvokeHttp400();

            string encodedKey = k.ToLower();           

            var emailInvoice = _orderService.GetEmailInvoiceByEncodedKey(encodedKey);

            // Not found
            // Expired            
            if (emailInvoice == null || emailInvoice.EndDate.CompareTo(DateTime.Now) < 0)
                return InvokeHttp400();

            // Paid
            if (emailInvoice.Paid)
                return InvokeHttp410();

            var paymentModel = new CheckoutPaymentModel
            {
                CardTypes = PrepareCardTypes(),
                ExpireYears = PrepareCardYears(),
                ExpireMonths = PrepareCardMonths(),
                OrderTotal = _priceFormatter.FormatValue(emailInvoice.Amount * emailInvoice.ExchangeRate, emailInvoice.CurrencyCode)
            };
            
            if (string.IsNullOrEmpty(_session["PaymentErrorMessage"] as string) == false)
            {
                ViewBag.ErrorMessage = _session["PaymentErrorMessage"].ToString();
                _session["PaymentErrorMessage"] = null;
            }

            var model = new InvoicePaymentModel
            {
                EmailInvoiceId = emailInvoice.Id,
                Payment = paymentModel
            };

            // countries and states
            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();

            model.BillingAddress.AvailableCountries = countries.PrepareCountries();
            model.BillingAddress.AvailableStates = states.PrepareStates();

            return View(model);
        }

        public ActionResult ConfirmInvoicePayment(InvoicePaymentModel model)
        {
            var address = model.BillingAddress.PrepareAddressEntity();
            var card = new Card
            {
                CardType = model.Payment.CardType,
                CardNumber = model.Payment.CardNumber,
                HolderName = model.Payment.CardHolderName,
                SecurityCode = model.Payment.CardCode,
                ExpiryMonth = model.Payment.ExpireMonth,
                ExpiryYear = model.Payment.ExpireYear,
                StartMonth = string.Empty,
                StartYear = string.Empty,
                IssueNumber = string.Empty
            };

            var output = _paymentService.ProcessPaymentFromEmailInvoice(
                model.EmailInvoiceId,
                address,
                card,
                _httpContext.Request.UserAgent,
                _httpContext.Request.UserHostAddress == "::1" ? "45.1.1.1" : _httpContext.Request.UserHostAddress,
                true);

            if (output.Status)
            {
                // 3D Secure redirect?
                if (output.Has3DSecure)
                {
                    RedirectToSecuredCardPage(output.RedirectUrl, output.PaReq, output.MD);                    
                }
                else
                {
                    return RedirectToRoute("Checkout Completed", new { orderid = 0, emailinvoiceid = model.EmailInvoiceId, hasnhs = 0 });
                }
            }
            else
            {
                var invoice = _orderService.GetEmailInvoiceById(model.EmailInvoiceId);

                _session["PaymentErrorMessage"] = output.Message;
                return RedirectToAction("InvoicePayment", new { k = invoice.EncodedKey });
            }

            return Content(string.Empty);
        }

        #endregion

        #region Utilities

        [NonAction]
        protected ActionResult CreatePrescriptionOrder(int profileId, string clientIPAddress)
        {
            var order = _orderService.CreatePrescriptionOrder(profileId, clientIPAddress);
            return RedirectToRoute("Checkout Completed", new { orderid = order.Id, emailinvoiceid = 0, hasnhs = 1 });
        }

        [NonAction]
        protected ActionResult CreateFOCOrder(int profileId, string clientIPAddress)
        {
            var address = _accountService.GetShippingAddressByProfileId(profileId);
            var order = _orderService.CreateOrderFromCart(profileId, string.Empty, clientIPAddress, null, address, OrderStatusCode.ORDER_PLACED, LineStatusCode.ORDERED);
            return RedirectToRoute("Checkout Completed", new { orderid = order.Id, emailinvoiceid = 0, hasnhs = 0 });
        }

        [NonAction]
        protected IList<SelectListItem> PrepareCardTypes()
        {
            var cards = new List<SelectListItem>
            {
                // TODO: Card types should be in settings
                //CC types
                new SelectListItem
                {
                    Text = "Master card",
                    Value = "MC",
                },
                new SelectListItem
                {
                    Text = "Visa",
                    Value = "VISA",
                },
                new SelectListItem
                {
                    Text = "American Express",
                    Value = "AMEX",
                }
            };

            return cards;
        }

        [NonAction]
        protected IList<SelectListItem> PrepareCardYears()
        {
            var years = new List<SelectListItem>();
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                years.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            return years;
        }

        [NonAction]
        protected IList<SelectListItem> PrepareCardMonths()
        {
            var months = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                months.Add(new SelectListItem
                {
                    Text = text,
                    Value = text,
                });
            }

            return months;
        }
        
        [NonAction]
        protected void RedirectToSecuredCardPage(string redirectUrl, string paReq, string md)
        {
            string termUrl = _storeInfomationSettings.TermURL;
            string companyName = _storeInfomationSettings.CompanyName;

            _httpContext.Response.Write(string.Format("<html><head><title>3D Secure Verification | {0}</title>", companyName));
            _httpContext.Response.Write("<meta name=\"theme-color\" content=\"#231f20\"/>");
            _httpContext.Response.Write("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css\" integrity=\"sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7\" crossorigin=\"anonymous\">");
            _httpContext.Response.Write(@"<style type='text/css'>
                                                    .glyphicon-spin {
                                                        -webkit-animation: spin 2000ms infinite linear;
                                                        animation: spin 2000ms infinite linear;
                                                    }
                                                    @-webkit-keyframes spin {
                                                        0% {
                                                            -webkit-transform: rotate(0deg);
                                                            transform: rotate(0deg);
                                                        }
                                                        100% {
                                                            -webkit-transform: rotate(359deg);
                                                            transform: rotate(359deg);
                                                        }
                                                    }
                                                    @keyframes spin {
                                                        0% {
                                                            -webkit-transform: rotate(0deg);
                                                            transform: rotate(0deg);
                                                        }
                                                        100% {
                                                            -webkit-transform: rotate(359deg);
                                                            transform: rotate(359deg);
                                                        }
                                                    }
                                                    </style>
                                                ");
            _httpContext.Response.Write("<script type='text/javascript'>function OnLoadEvent() { var t = setTimeout(\"document.form.submit()\", 3000); }</script>");
            _httpContext.Response.Write("</head>");
            _httpContext.Response.Write("<body class=\"container\" onload='OnLoadEvent();'>");
            _httpContext.Response.Write("<div class=\"row\">");
            _httpContext.Response.Write("<div class=\"text-center\" style=\"margin-top: 20px;\">");
            _httpContext.Response.Write("<span class=\"glyphicon glyphicon-cog glyphicon-spin\" style=\"font-size: 120px; color: #231f20;\"></span>");
            _httpContext.Response.Write("<p style=\"width: 100%; font-size: 20px; font-family: 'Archivo Narrow', sans-serif; display: inline-block; text-transform: uppercase;\">Contacting your bank now...</p>");
            _httpContext.Response.Write("<p style=\"width: 100%; font-size: 15px; font-family: 'Archivo Narrow', sans-serif; display: inline-block; text-transform: uppercase;\">Please wait while redirecting to your bank secured website.</p>");
            _httpContext.Response.Write("<form name=\"form\" action=\"" + redirectUrl + "\" method=\"POST\"/>");
            _httpContext.Response.Write("<input type=\"hidden\" name=\"PaReq\" value=\"" + paReq + "\"/>");
            _httpContext.Response.Write("<input type=\"hidden\" name=\"TermUrl\" value=\"" + termUrl + "\"/>");
            _httpContext.Response.Write("<input type=\"hidden\" name=\"MD\" value=\"" + md + "\"/>");
            _httpContext.Response.Write("<noscript>");
            _httpContext.Response.Write("<center><p>Please click button below to authenticate your card</p><input type=\"submit\" style=\"width: 70px;\" value=\"Go\"/></p></center>");
            _httpContext.Response.Write("</noscript>");
            _httpContext.Response.Write("</form>");
            _httpContext.Response.Write("</div></div></body></html>");
        }

        #endregion
    }
}