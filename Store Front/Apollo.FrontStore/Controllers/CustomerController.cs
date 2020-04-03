using Apollo.Core;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Authentication;
using Apollo.Web.Framework.Services.Common;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class CustomerController : BasePublicController
    {
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly IShippingService _shippingService;
        private readonly IUtilityService _utilityService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IIdentityExternalAuthService _identityExternalAuthService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ApolloSessionState _session;

        public CustomerController(
            IAccountService accountService,
            ICartService cartService,
            IShippingService shippingService,
            IUtilityService utilityService,
            IAuthenticationService authenticationService,
            IIdentityExternalAuthService identityExternalAuthService,
            IWorkContext workContext,
            ILogBuilder logBuilder,
            ApolloSessionState session)
        {
            _accountService = accountService;
            _cartService = cartService;
            _shippingService = shippingService;
            _utilityService = utilityService;
            _authenticationService = authenticationService;
            _identityExternalAuthService = identityExternalAuthService;
            _workContext = workContext;
            _session = session;
            _logger = logBuilder.CreateLogger(typeof(CustomerController).FullName);
        }

        #region Register
        
        public ActionResult Register()
        {
            if (_workContext.CurrentProfile.IsAnonymous == false) return RedirectToRoute("Home");

            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]        
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (!VerifyRecaptcha())
            {
                ModelState.AddModelError(string.Empty, "Sorry, please try again.");
                return View(model);
            }

            if (_workContext.CurrentProfile.IsAnonymous == false) _authenticationService.SignOut();
             
            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    Name = model.Name,
                    Email = model.Email.ToLower(),
                    ContactNumber = model.ContactNumber,
                    DOB = model.ParseDateOfBirth().HasValue ? model.ParseDateOfBirth().Value.ToString("dd/MM/yyyy") : null,
                    Username = model.Email.ToLower(),
                    DisplayContactNumberInDespatch = model.DisplayContactNumberInDespatch,
                    ProfileId = _workContext.CurrentProfile.Id,
                };
                
                var result = _accountService.ProcessRegistration(account, model.Password, sendEmailFlag: true);
                
                if (string.IsNullOrEmpty(result.Message))
                {
                    account.Id = result.UserId;
                    _authenticationService.SignIn(account.Username, model.Password, isPersistent: true, shouldLockOut: true);

                    return RedirectToRoute("Register Result", new { resultId = Convert.ToInt32(UserRegistrationType.Standard), returnUrl });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }                
            }

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult ExternalLoginRegistration(RegisterExternalModel model, string returnUrl)
        {
            if (_workContext.CurrentProfile.IsAnonymous == false) _authenticationService.SignOut();

            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    Name = model.Name,
                    Email = model.Email.ToLower(),
                    ContactNumber = model.ContactNumber,
                    DOB = model.ParseDateOfBirth().HasValue ? model.ParseDateOfBirth().Value.ToString("dd/MM/yyyy") : null,
                    Username = model.Email,
                    DisplayContactNumberInDespatch = model.DisplayContactNumberInDespatch,
                    ProfileId = _workContext.CurrentProfile.Id,
                };
               
                var result = _accountService.ProcessRegistrationWithExternalLogin(account, loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey, sendEmailFlag: true);
                
                if (string.IsNullOrEmpty(result.Message))
                {
                    account.Id = result.UserId;
                    _identityExternalAuthService.ExternalSignIn(loginInfo.Email, loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey, isPersistent: true);

                    return RedirectToRoute("Register Result", new { resultId = Convert.ToInt32(UserRegistrationType.Standard),  returnUrl });
                }
                else
                {
                    model.LoginProvider = loginInfo.Login.LoginProvider;
                    model.Email = loginInfo.Email;
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        public ActionResult RegisterResult(int resultId)
        {
            var resultText = string.Empty;
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = "Registration not allowed. Please contact us for more information.";
                    break;
                case UserRegistrationType.Standard:
                    resultText = "Your registration completed.";
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = "Your account is pending for approval.";
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = "Your registration has been successfully completed. You have just been sent an email containing membership activation instructions.";
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return View(model);
        }

        #endregion

        #region Account
        
        public ActionResult Account()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var account = _accountService.GetAccountByProfileId(_workContext.CurrentProfile.Id);
            var model = PrepareAccountModel(account);

            if (string.IsNullOrEmpty(_session["message"] as string) == false)
            {
                ViewBag.Message = _session["message"].ToString();
                _session["message"] = null;
            }

            if (string.IsNullOrEmpty(_session["error"] as string) == false)
            {
                ViewBag.ErrorMessage = _session["error"].ToString();
                _session["error"] = null;
            }

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]        
        public ActionResult Account(AccountModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var account = _accountService.GetAccountByProfileId(_workContext.CurrentProfile.Id);
            account.Name = model.Name;            
            account.ContactNumber = model.ContactNumber;
            account.DisplayContactNumberInDespatch = model.DisplayContactNumberInDespatch;
          
            var result = _accountService.ProcessAccountUpdate(account);

            switch (result)
            {
                case AccountUpdateResults.Successful:
                    _session["message"] = "Your account has been successfully updated.";
                    break;
                case AccountUpdateResults.MemberNotExist:
                case AccountUpdateResults.ExistingEmail:
                default:
                    _session["error"] = "Failed to update. Please contact us for more information.";
                    break;
            }
            
            return RedirectToAction("Account");
        }

        public ActionResult PrimaryAddress()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var allAddresses = _accountService.GetAddressesByProfileId(_workContext.CurrentProfile.Id);
            var billing = allAddresses.Where(x => x.IsBilling == true).FirstOrDefault();
            var shipping = allAddresses.Where(x => x.IsShipping == true).FirstOrDefault();
            
            var model = new AccountPrimaryAddressModel
            {
                Billing = billing?.PrepareAddressModel(),
                Shipping = shipping?.PrepareAddressModel()
            };

            if (string.IsNullOrEmpty(_session["message"] as string) == false)
            {
                ViewBag.Message = _session["message"].ToString();
                _session["message"] = null;
            }

            return View(model);
        }
        
        public ActionResult Addresses()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var model = new List<AddressModel>();

            var allAddresses = _accountService.GetAddressesByProfileId(_workContext.CurrentProfile.Id);

            if (allAddresses.Count > 0)
            {
                foreach (var item in allAddresses)
                {
                    model.Add(item.PrepareAddressModel());
                }                
            }

            if (string.IsNullOrEmpty(_session["message"] as string) == false)
            {
                ViewBag.Message = _session["message"].ToString();
                _session["message"] = null;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult NewAddress()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();

            var model = new AddressModel
            {
                AvailableCountries = countries.PrepareCountries(),
                AvailableStates = states.PrepareStates()
            };

            if (string.IsNullOrEmpty(_session["message"] as string) == false)
            {
                ViewBag.Message = _session["message"].ToString();
                _session["message"] = null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewAddress(AddressModel model)
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
                if (country.ISO3166Code == "US")
                    address.USStateId = model.USStateId.Value;

                var allAddresses = _accountService.GetAddressesByAccountId(profileId);

                if (allAddresses.Count == 0)
                {
                    address.IsBilling = true;
                    address.IsShipping = true;
                }

                _accountService.InsertAddress(address);

                _session["message"] = "Your address has been successfully added.";

                return RedirectToAction("Addresses", "Customer");
            }

            ModelState.AddModelError(string.Empty, "Sorry, there is something wrong with the address.");

            var countries = _shippingService.GetActiveCountries();
            var states = _shippingService.GetUSStates();
            model.AvailableCountries = countries.PrepareCountries();
            model.AvailableStates = states.PrepareStates();

            return View(model);
        }

        public ActionResult SetAddress(int id, AddressType type)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(id);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);
            if (address != null && address.AccountId == accountId)
            {
                switch (type)
                {
                    case AddressType.Billing:
                        _accountService.UpdatePrimaryBillingAddress(address.Id, accountId: accountId);
                        _session["message"] = "Thanks, your primary billing address has been selected.";
                        break;
                    case AddressType.Shipping:
                    default:
                        _accountService.UpdatePrimaryShippingAddress(address.Id, accountId: accountId);
                        _session["message"] = "Thanks, your primary shipping address has been selected.";
                        break;
                }
            }
                        
            return RedirectToAction("PrimaryAddress", "Customer");
        }

        [HttpGet]
        public ActionResult EditAddress(int id)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(id);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);
            if (address != null && address.AccountId == accountId)
            {
                var countries = _shippingService.GetActiveCountries();
                var states = _shippingService.GetUSStates();
                var model = address.PrepareAddressModel();

                model.AvailableCountries = countries.PrepareCountries();
                model.AvailableStates = states.PrepareStates();
                
                return View(model);
            }

            ViewBag.Message = "Sorry, address could not be found. Pleasey try again.";

            return RedirectToAction("Addresses", "Customer");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAddress(AddressModel model)
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
                    
                    if (country.ISO3166Code == "US")
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
        public ActionResult RemoveAddress(int id)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var address = _accountService.GetAddressById(id);
            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);

            if (address != null && address.AccountId == accountId)
            {
                _accountService.DeleteAddressByAddressId(id);
                _session["message"] = "Your address has been successfully removed.";
            }

            return RedirectToAction("Addresses", "Customer");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _identityExternalAuthService.ChangePassword(_workContext.CurrentProfile.Username, model.OldPassword, model.NewPassword);

            if (result)
            {
                _session["message"] = "Your password has been successfully updated.";
            }
            else
            {
                _session["error"] = "Failed to update password. Please contact us for more information.";
            }

            return RedirectToAction("Account");
        }

        [HttpGet]
        public ActionResult SetPassword()
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPassword(SetPasswordModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var result = _identityExternalAuthService.SetPassword(_workContext.CurrentProfile.Username, model.NewPassword);

                if (result)
                {
                    _session["message"] = "Your password has been successfully added. You can now can log onto Apollo with just your email address";
                }
                else
                {
                    _session["error"] = "Failed to set password. Please contact us for more information.";
                }
            }

            return RedirectToAction("Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveLogin(string loginProvider, string providerKey)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            if (_identityExternalAuthService.RemoveLogin(_workContext.CurrentProfile.Username, loginProvider, providerKey))
            {
                _session["message"] = $"Your {loginProvider} credential has been successfully removed.";
            }
            else
            {
                _session["error"] = "Failed to remove credential. Please contact us for more information.";
            }

            return RedirectToAction("Account");
        }

        #endregion

        #region Login

        [HttpGet]        
        public ActionResult Login()
        {
            if (_workContext.CurrentProfile.IsAnonymous == false)
                return RedirectToRoute("Home");

            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form, string returnUrl)
        {
            Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
            string data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();

            string email = form["Email"];
            string password = form["Password"];

            //TODO: To be deleted
            //var result = _accountService.ValidateUser(email, password);
            var result = _authenticationService.SignIn(email, password, isPersistent: true, shouldLockOut: true);

            switch (result)
            {
                case CustomerLoginResults.Error:
                    ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                    _logger.InsertLog(LogLevel.Error, "Error occured while validating user. Email = {{{0}}}", email);
                    break;
                case CustomerLoginResults.Successful:
                    var account = _accountService.GetAccountByUsername(email);

                    if (account == null)
                    {
                        ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                        _logger.InsertLog(LogLevel.Error, "Account could not be found. Email = {{{0}}}", email);
                    }
                    else
                    {
                        // Migrate currency
                        var currency = _utilityService.GetCurrencyByCurrencyCode(_workContext.WorkingCurrency.CurrencyCode);
                        if (currency != null)
                        {
                            _utilityService.SaveAttribute(account.ProfileId, "Profile", SystemCustomerAttributeNames.CurrencyId, currency.Id.ToString());
                        }

                        // Migrate country
                        var country = _shippingService.GetCountryById(_workContext.CurrentCountry.Id);
                        if (country != null)
                        {
                            _utilityService.SaveAttribute(account.ProfileId, "Profile", SystemCustomerAttributeNames.CountryId, country.Id.ToString());
                        }

                        var profileId = _workContext.CurrentProfile.Id;
                        var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);

                        // Migrate shipping option
                        _utilityService.SaveAttribute(account.ProfileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, options[0].Id.ToString());

                        // Migrate shopping cart
                        _cartService.MigrateShoppingCart(profileId, account.ProfileId, _workContext.CurrentCountry.ISO3166Code);

                        // Migrate EU Cookie Law acceptance
                        var euCookieLawAccepted =_workContext.CurrentProfile.GetAttribute<bool>("Profile", SystemCustomerAttributeNames.EuCookieLawAccepted);
                        _utilityService.SaveAttribute(account.ProfileId, "Profile", SystemCustomerAttributeNames.EuCookieLawAccepted, euCookieLawAccepted.ToString());

                        //TODO: To be deleted
                        // Sign in
                        //_authenticationService.SignIn(account, createPersistentCookie: true);

                        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToRoute("Home");

                        return Redirect(returnUrl);
                    }
                    break;
                                        
                case CustomerLoginResults.MemberNotExists:
                    ViewBag.ErrorMessage = "Login failed. Please login with correct username or password.";
                    break;
                case CustomerLoginResults.ProfileNotExists:
                    ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                    _logger.InsertLog(LogLevel.Error, "Profile could not be found. Email = {{{0}}}", email);
                    break;
                case CustomerLoginResults.AccountNotExists:
                    ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                    _logger.InsertLog(LogLevel.Error, "Account could not be found. Email = {{{0}}}", email);
                    break;
                case CustomerLoginResults.WrongPassword:
                    ViewBag.ErrorMessage = "Login failed. Please login with correct username or password.";
                    break;
                case CustomerLoginResults.IsLockedOut:
                    ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                    break;
                case CustomerLoginResults.NotApproved:
                    ViewBag.ErrorMessage = "There is an issue with your account. Please contact customerservices@Apollo.co.uk.";
                    break;
                default:
                    break;
            }

            return View();
        }

        public ActionResult Logout()
        {
            _authenticationService.SignOut();
            return RedirectToRoute("Home");
        }

        [HttpGet]        
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordModel();
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult ForgotPassword(ForgotPasswordModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.ProcessPasswordReset(model.Email, true);                
                return RedirectToRoute("Forgot Password Result", new { resultId = (int)result });

            }

            return View(model);
        }

        public ActionResult ForgotPasswordResult(int resultId)
        {
            var resultText = string.Empty;
            switch ((PasswordResetResults)resultId)
            {
                case PasswordResetResults.Successful:
                    resultText = "Your password has been successfully reset and emailed to you. Please check your email now.";                    
                    break;
                case PasswordResetResults.AccountNotExist:
                case PasswordResetResults.MemberNotExist:
                    resultText = "Sorry, there is no account found with the entered email address.";
                    break;
                case PasswordResetResults.IsLockedOut:
                    resultText = "Sorry, your account has been locked out. Please contact <a href=\"mailto:customerservices@Apollo.co.uk\">customerservices@Apollo.co.uk</a>.";
                    break;
                case PasswordResetResults.Error:
                default:
                    resultText = "Sorry, there is an error in the system. Please contact <a href=\"mailto:customerservices@Apollo.co.uk\">customerservices@Apollo.co.uk</a>.";
                    break;                
            }

            var model = new ForgotPasswordResultModel
            {
                Result = resultText
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalAuthentication(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Customer", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var email = loginInfo.Email;

            if (loginInfo.Login.LoginProvider.ToLower() == "twitter")
            {
                string accessToken = loginInfo.ExternalIdentity.Claims.Where(x => x.Type == "urn:twitter:access_token").Select(x => x.Value).FirstOrDefault();
                string accessSecret = loginInfo.ExternalIdentity.Claims.Where(x => x.Type == "urn:twitter:access_secret").Select(x => x.Value).FirstOrDefault();
                var twitterLogin = TwitterHelper.GetTwitterLogin(accessToken, accessSecret, 
                    ConfigurationManager.AppSettings["twitter:ConsumerKey"], ConfigurationManager.AppSettings["twitter:ConsumerSecret"]);

                email = twitterLogin.Email;
            }

            var result = _identityExternalAuthService.ExternalSignIn(email, loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey, isPersistent: true);

            switch (result)
            {
                case CustomerLoginResults.Successful:
                    if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                        return RedirectToRoute("Home");
                    return Redirect(returnUrl);
                    
                case CustomerLoginResults.MemberNotExists:
                    ViewBag.ReturnUrl = returnUrl;                    
                    return View("ExternalLoginRegistration", new RegisterExternalModel
                    {
                        Email = email,
                        LoginProvider = loginInfo.Login.LoginProvider
                    });
                    
                case CustomerLoginResults.IsLockedOut:
                    return View("AccountLockout");

                default:
                    return RedirectToAction("Login");
            }
        }

        #endregion

        #region Utilities

        [NonAction]
        protected AccountModel PrepareAccountModel(Account account)
        {
            return new AccountModel
            {
                Id = account.Id,
                Name = account.Name,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                DisplayContactNumberInDespatch = account.DisplayContactNumberInDespatch,
                HasPassword = _accountService.HasPassword(account.Username),
                Credentials = _identityExternalAuthService.GetLogins(account.Username),
            };
        }

        [NonAction]
        protected bool VerifyRecaptcha()
        {
            var values = new Dictionary<string, string>
            {
                { "secret", ConfigurationManager.AppSettings["recaptcha:SecretKey"] },
                { "response", HttpContext.Request["g-recaptcha-response"] },
                { "remoteip", HttpContext.Request.UserHostAddress },
            };

            var content = new FormUrlEncodedContent(values);
            var client = new HttpClient();
            var response = Task.Run(() => client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content)).Result;
            var responseString = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
            dynamic recaptchaResult = JsonConvert.DeserializeObject(responseString);
            string success = recaptchaResult.success;

            return success.ToLower() == "true";
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}