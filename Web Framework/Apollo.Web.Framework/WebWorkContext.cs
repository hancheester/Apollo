using Apollo.Core;
using Apollo.Core.Domain.Customers;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Web.Framework.Services.Authentication;
using Apollo.Web.Framework.Services.Common;
using Apollo.Web.Framework.Services.Helpers;
using System;
using System.Linq;
using System.Web;

namespace Apollo.Web.Framework
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields
        
        private readonly IAccountService _accountService;
        private readonly IUtilityService _utilityService;
        private readonly IShippingService _shippingService;        
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly HttpContextBase _httpContext;
        //private readonly ILogger _logger; // Useful for debugging

        private Profile _cachedProfile;
        private Currency _cachedCurrency;
        private Country _cachedCountry;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(
            HttpContextBase httpContext,
            IAccountService accountService,
            IUtilityService utilityService,
            IShippingService shippingService,            
            IAuthenticationService authenticationService,
            IUserAgentHelper userAgentHelper,
            //ILogBuilder logBuilder,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            ShippingSettings shippingSettings)
        {
            _httpContext = httpContext;
            _accountService = accountService;
            _utilityService = utilityService;
            _shippingService = shippingService;
            _taxSettings = taxSettings;
            _currencySettings = currencySettings;
            _shippingSettings = shippingSettings;
            _authenticationService = authenticationService;
            _userAgentHelper = userAgentHelper;
            //_logger = logBuilder.CreateLogger(typeof(WebWorkContext).FullName);
        }

        #endregion

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[SystemCookieNames.CustomerCookie];
        }

        protected virtual void SetCustomerCookie(string username)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var isValid = ValidateCustomerCookie(username);

                if (isValid == false)
                {
                    var cookie = new HttpCookie(SystemCookieNames.CustomerCookie);
                    cookie.HttpOnly = true;                    
                    cookie.Values.Add("username", username);
                    
                    if (string.IsNullOrEmpty(username))
                    {
                        cookie.Expires = DateTime.Now.AddMonths(-1);
                        cookie.Values.Add("exp", DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    else
                    {
                        int cookieExpires = 24 * 7;
                        cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                        cookie.Values.Add("exp", DateTime.Now.AddHours(cookieExpires).ToString("dd/MM/yyyy HH:mm:ss"));
                    }

                    _httpContext.Response.Cookies.Remove(SystemCookieNames.CustomerCookie);
                    _httpContext.Response.Cookies.Add(cookie);
                }
            }
        }

        private bool ValidateCustomerCookie(string username)
        {
            // You cannot read the cookie's expiration date and time.
            //http://stackoverflow.com/questions/21441125/why-is-the-cookie-expiration-date-not-surviving-across-sessions-in-asp-net

            var found = _httpContext.Request.Cookies.AllKeys.Contains(SystemCookieNames.CustomerCookie);
            if (found == false) return false;

            var foundCookie = _httpContext.Request.Cookies[SystemCookieNames.CustomerCookie];
            if (foundCookie == null) return false;

            if (foundCookie.Values["username"] != username) return false;

            var exp = foundCookie.Values["exp"].ToString();
            var expDate = DateTime.Now;

            if (DateTime.TryParse(exp, out expDate))
            {
                return (expDate.CompareTo(DateTime.Now) > 0);
            }

            return false;

            //var exp = Convert.ToDateTime(foundCookie.Values["exp"]);
            //if (exp.CompareTo(DateTime.Now) < 0) return false;
            
            //return true;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current profile
        /// </summary>
        public virtual Profile CurrentProfile
        {
            get
            {
                if (_cachedProfile != null)
                    return _cachedProfile;

               Profile profile = null;
                //if (_httpContext == null || _httpContext is FakeHttpContext)
                //{
                //    //check whether request is made by a background task
                //    //in this case return built-in customer record for background task
                //    //customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                //}

                //check whether request is made by a search engine
                //in this case return built-in profile record for search engines                
                if (profile == null)
                {
                    if (_userAgentHelper.IsSearchEngine())
                    {
                        profile = _accountService.GetProfileBySystemName(SystemCustomerNames.SearchEngine);   
                    }                        
                }

                //registered user
                if (profile == null)
                {
                    var account = _authenticationService.GetAuthenticatedAccount();                    
                    if (account != null)
                    {
                        profile = _accountService.GetProfileById(account.ProfileId);
                    }
                }

                //impersonate user if required (currently used for 'phone order' support)
                //if (customer != null && !customer.Deleted && customer.Active)
                //{
                //    var impersonatedCustomerId = customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                //    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                //    {
                //        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                //        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                //        {
                //            //set impersonated customer
                //            _originalCustomerIfImpersonated = customer;
                //            customer = impersonatedCustomer;
                //        }
                //    }
                //}

                //load guest profile                
                if (profile == null)
                {
                    var customerCookie = GetCustomerCookie();                    
                    if (customerCookie != null && !string.IsNullOrEmpty(customerCookie["username"]))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie["username"], out customerGuid))
                        {                            
                            var customerByCookie = _accountService.GetProfileByUsername(customerGuid.ToString());
                            if (customerByCookie != null && customerByCookie.IsAnonymous)
                                profile = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (profile == null)
                {
                    string username = Guid.NewGuid().ToString();
                    profile = _accountService.GenerateVisitorProfileByUsername(username, false);                    
                }
                
                if (profile != null)
                {
                    SetCustomerCookie(profile.Username);
                    _cachedProfile = profile;
                }

                return _cachedProfile;
            }
            set
            {
                SetCustomerCookie(value.Username);
                _cachedProfile = value;
            }
        }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        //public virtual Customer OriginalCustomerIfImpersonated
        //{
        //    get
        //    {
        //        return _originalCustomerIfImpersonated;
        //    }
        //}

        /// <summary>
        /// Gets or sets the current vendor (logged-in manager)
        /// </summary>
        //public virtual Vendor CurrentVendor
        //{
        //    get
        //    {
        //        if (_cachedVendor != null)
        //            return _cachedVendor;

        //        var currentCustomer = CurrentCustomer;
        //        if (currentCustomer == null)
        //            return null;

        //        var vendor = _vendorService.GetVendorById(currentCustomer.VendorId);

        //        //validation
        //        if (vendor != null && !vendor.Deleted && vendor.Active)
        //            _cachedVendor = vendor;

        //        return _cachedVendor;
        //    }
        //}

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        //public virtual Language WorkingLanguage
        //{
        //    get
        //    {
        //        if (_cachedLanguage != null)
        //            return _cachedLanguage;

        //        Language detectedLanguage = null;
        //        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
        //        {
        //            //get language from URL
        //            detectedLanguage = GetLanguageFromUrl();
        //        }
        //        if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
        //        {
        //            //get language from browser settings
        //            //but we do it only once
        //            if (!CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
        //                _genericAttributeService, _storeContext.CurrentStore.Id))
        //            {
        //                detectedLanguage = GetLanguageFromBrowserSettings();
        //                if (detectedLanguage != null)
        //                {
        //                    _genericAttributeService.SaveAttribute(CurrentCustomer, SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
        //                         true, _storeContext.CurrentStore.Id);
        //                }
        //            }
        //        }
        //        if (detectedLanguage != null)
        //        {
        //            //the language is detected. now we need to save it
        //            if (CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
        //                _genericAttributeService, _storeContext.CurrentStore.Id) != detectedLanguage.Id)
        //            {
        //                _genericAttributeService.SaveAttribute(CurrentCustomer, SystemCustomerAttributeNames.LanguageId,
        //                    detectedLanguage.Id, _storeContext.CurrentStore.Id);
        //            }
        //        }

        //        var allLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);
        //        //find current customer language
        //        var languageId = CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
        //            _genericAttributeService, _storeContext.CurrentStore.Id);
        //        var language = allLanguages.FirstOrDefault(x => x.Id == languageId);
        //        if (language == null)
        //        {
        //            //it not specified, then return the first (filtered by current store) found one
        //            language = allLanguages.FirstOrDefault();
        //        }
        //        if (language == null)
        //        {
        //            //it not specified, then return the first found one
        //            language = _languageService.GetAllLanguages().FirstOrDefault();
        //        }

        //        //cache
        //        _cachedLanguage = language;
        //        return _cachedLanguage;
        //    }
        //    set
        //    {
        //        var languageId = value != null ? value.Id : 0;
        //        _genericAttributeService.SaveAttribute(CurrentCustomer,
        //            SystemCustomerAttributeNames.LanguageId,
        //            languageId, _storeContext.CurrentStore.Id);

        //        //reset cache
        //        _cachedLanguage = null;
        //    }
        //}

        public virtual Currency WorkingCurrency
        {
            get
            {
                if (_cachedCurrency != null)
                    return _cachedCurrency;

                var allCurrencies = _utilityService.GetAllCurrency();

                //find a currency previously selected by a customer
                var currencyId = CurrentProfile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CurrencyId);
                var currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);

                if (currency == null)
                {
                    //it not found, then let's load the primary currency
                    var primaryCurrency = _utilityService.GetCurrency(_currencySettings.PrimaryStoreCurrencyId);
                    currency = primaryCurrency;
                }

                if (currency == null)
                {
                    //it not found, then return the first (filtered by current store) found one
                    currency = allCurrencies.FirstOrDefault();
                }
                
                //save only if it's not a system profile (eg. Search Engine) and it doesn't match the database value
                if (CurrentProfile.IsSystemProfile == false && currencyId != currency.Id)
                    WorkingCurrency = currency;

                //if it is a system profile, always return primary currency
                if (CurrentProfile.IsSystemProfile)
                {
                    var primaryCurrency = _utilityService.GetCurrency(_currencySettings.PrimaryStoreCurrencyId);
                    currency = allCurrencies.FirstOrDefault(x => x.Id == primaryCurrency.Id);
                }

                //cache
                _cachedCurrency = currency;

                return _cachedCurrency;
            }
            set
            {
                var currencyId = value != null ? value.Id : 0;

                //if it is a system profile, always save primary currency
                if (CurrentProfile.IsSystemProfile)
                {
                    var primaryCurrency = _utilityService.GetCurrency(_currencySettings.PrimaryStoreCurrencyId);
                    currencyId = primaryCurrency.Id;
                }

                _utilityService.SaveAttribute(
                    CurrentProfile.Id, 
                    "Profile", 
                    SystemCustomerAttributeNames.CurrencyId, 
                    currencyId.ToString());
                
                //reset cache
                _cachedCurrency = null;
            }
        }

        public virtual Country CurrentCountry
        {
            get
            {
                if (_cachedCountry != null)
                    return _cachedCountry;

                var countryId = CurrentProfile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId);
                var country = _shippingService.GetCountryById(countryId);

                if (country == null)
                {
                    string ip = _httpContext.Request.UserHostAddress;
                    string countryCode = _utilityService.ConvertIPToCountryName(ip);

                    //if it's not found, then let's load from user's ip address
                    country = _shippingService.GetCountryByCountryCode(countryCode);
                }

                if (country == null)
                {
                    //if it's not found, then default country
                    var primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;                    
                    country = _shippingService.GetCountryById(primaryStoreCountryId);
                }

                if (country == null)
                {
                    //if it's not found, then load first found one
                    country = _shippingService.GetActiveCountries().FirstOrDefault();
                }

                //save only if it's not a system profile (eg. Search Engine) and it doesn't match the database value
                if (CurrentProfile.IsSystemProfile == false && countryId != country.Id)
                    CurrentCountry = country;

                //cache
                _cachedCountry = country;
                
                return _cachedCountry;
            }
            set
            {
                var countryId = value != null ? value.Id : 0;
                _utilityService.SaveAttribute(
                    CurrentProfile.Id,
                    "Profile",
                    SystemCustomerAttributeNames.CountryId,
                    countryId.ToString());

                //reset cache
                _cachedCountry = null;
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                //cache
                if (_cachedTaxDisplayType != null)
                    return _cachedTaxDisplayType.Value;

                TaxDisplayType taxDisplayType;
                if (CurrentProfile != null)
                {
                    taxDisplayType = (TaxDisplayType)CurrentProfile.GetAttribute<int>(
                        "Profile", 
                        SystemCustomerAttributeNames.TaxDisplayTypeId);
                }
                else
                {
                    taxDisplayType = _taxSettings.TaxDisplayType;
                }

                //cache
                _cachedTaxDisplayType = taxDisplayType;
                return _cachedTaxDisplayType.Value;

            }
            set
            {
                _utilityService.SaveAttribute(
                    CurrentProfile.Id,
                    "Profile",
                    SystemCustomerAttributeNames.TaxDisplayTypeId,
                    ((int)value).ToString());

                //reset cache
                _cachedTaxDisplayType = null;

            }
        }
        #endregion
    }
}
