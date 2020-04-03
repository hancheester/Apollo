using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Customers;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    /// <summary>
    /// By default, anonymous profile will be created if there is no valid user id found in both query and order.
    /// If an anonymous order is placed, user id will be zero.
    /// </summary>
    public partial class order_new : BasePage
    {
        private const string SUBTRACT_FORM = "-{0}";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(order_new).FullName);

        private Dictionary<string, SupportedCard> _supportedCards = ((SupportedCardSectionHandler)ConfigurationManager.GetSection("supportedCards")).Cards;

        #region Constants
        private const int MAX_YEAR = 10;
        private const string KEY = "key";
        private const string VALUE = "value";
        private const string TXT_QTY = "txtQty";
        private const string ORDER_ = "Order";
        private const string ORDER_ID_ = "OrderId";

        protected const string VPAGESTATE_VALID = "valid";
        protected const string VPAGESTATE_INVALID = "invalid";
        protected const string VPAGESTATE_NORMAL = "normal";

        private const string ORDER_NEW_PAGE_URL_FORMAT1 = "/sales/order_new.aspx?userid={0}&orderid={1}";
        private const string ORDER_NEW_PAGE_URL_FORMAT2 = "/sales/order_new.aspx?orderid={0}";
        private const string ORDER_NEW_PAGE_URL_FORMAT3 = "/sales/order_new.aspx?userid={0}";
        private const string AVAILABLE_POINTS_FORMAT = "{0} ({1} applied to this order).";
        private const string LOYALTY_POINTS_VALIDATOR_ERROR_MESSAGE = "Please enter an amount of loyalty points between 0 and {0}.";
        private const string PRICE_CHANGE_FORMAT = "The price of {0} has changed to {1}.";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY = "{0}{1:0.00} - {2}, {3} in stock";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {5} in stock";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY = "{0}{1:0.00} - {2}, {5} in stock, was {3}{4:0.00}";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {9} in stock, was {5}{6:0.00} ({7}{8:0.00})";
        private const string EXISTING_ORDER_FORMAT = "{0}, {1}, {2} {3}, Date placed: {4} at {5}, Number of items {6}, Order total {7}{8:0.00}";
        #endregion
        
        public IAccountService AccountService { get; set; }
        public ICartService CartService { get; set; }
        public IOfferService OfferService { get; set; }
        public IOrderService OrderService { get; set; }
        public IPaymentService PaymentService { get; set; }
        public IProductService ProductService { get; set; }
        public IShippingService ShippingService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }
        public ShippingSettings ShippingSettings { get; set; }
        public CustomerSettings CustomerSettings { get; set; }

        protected Currency CustomerCurrency
        {
            get
            {
                var profile = AccountService.GetProfileById(QueryUserId);
                var currencyId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CurrencyId, UtilityService);
                Currency currency = null;
                if (currencyId == 0)
                {
                    currencyId = CurrencySettings.PrimaryStoreCurrencyId;                    
                    UtilityService.SaveAttribute(profile.Id, "Profile", SystemCustomerAttributeNames.CurrencyId, currencyId.ToString());
                }
                
                currency = UtilityService.GetCurrency(currencyId);

                return currency;
            }
        }

        protected Country CustomerCountry
        {
            get
            {
                var profile = AccountService.GetProfileById(QueryUserId);
                var countryId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId, UtilityService);
                Country country = null;
                if (countryId == 0)
                {
                    countryId = ShippingSettings.PrimaryStoreCountryId;
                    UtilityService.SaveAttribute(profile.Id, "Profile", SystemCustomerAttributeNames.CountryId, country.Id.ToString());
                }
                
                country = ShippingService.GetCountryById(countryId);

                return country;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            int profileId = QueryUserId;
            string isLoaded = QueryIsLoaded;
            isLoaded = string.IsNullOrEmpty(isLoaded) ? "0" : isLoaded;

            if (orderId > 0 && profileId == 0)
            {
                var foundUserId = OrderService.GetProfileIdByOrderId(orderId);

                // Refresh page again if user id is not zero (anonymous profile, therefore, it will not have account information)
                if (foundUserId > 0)
                    Response.Redirect(string.Format(ORDER_NEW_PAGE_URL_FORMAT1, foundUserId, orderId));
            }

            // If profile id not found, create anonymous profile to get profile id
            if (profileId == 0)
            {
                string username = Guid.NewGuid().ToString();
                profileId = AccountService.InsertProfile(username, false);

                UtilityService.SaveAttribute(
                    profileId,
                    "Profile",
                    SystemCustomerAttributeNames.CountryId,
                    ShippingSettings.PrimaryStoreCountryId.ToString());

                UtilityService.SaveAttribute(
                    profileId, 
                    "Profile", 
                    SystemCustomerAttributeNames.CurrencyId, 
                    CurrencySettings.PrimaryStoreCurrencyId.ToString());

                // Refresh page with valid anonymous user id
                Response.Redirect(string.Format(ORDER_NEW_PAGE_URL_FORMAT3, profileId));
            }

            // If profile is not anonymous, load addresses
            var profile = AccountService.GetProfileById(profileId);
            ectTogRightNav.Visible = !profile.IsAnonymous;
            
            if (!IsPostBack)
            {
                if (orderId > 0 && isLoaded == "0")
                {
                    // If order id is valid, load order information
                    Order order = OrderService.GetCompleteOrderById(orderId);

                    if (order != null)
                    {
                        // Load line items in cart
                        LoadLineItemsToCart(order.LineItemCollection.ToList(), profileId);

                        adrBilling.Address = BuildBillingAddressFromOrder(order);
                        adrShipping.Address = BuildShippingAddressFromOrder(order);
                    }

                    var countryId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId, UtilityService);
                    ddlCurrency.SelectedIndex = ddlCurrency.Items.IndexOf(ddlCurrency.Items.FindByValue(CustomerCurrency.CurrencyCode));

                    if (countryId == 0)
                    {
                        countryId = ShippingSettings.PrimaryStoreCountryId;
                        UtilityService.SaveAttribute(profile.Id, "Profile", SystemCustomerAttributeNames.CountryId, countryId.ToString());
                    }

                    adrShipping.SetCountryId = countryId;

                    Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}&orderid={1}&isloaded={2}", QueryUserId, orderId, "1"));
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            #region Load user

            int profileId = QueryUserId;

            if (profileId > 0)
            {
                var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);
                gvTempCart.DataSource = items;
                gvTempCart.DataBind();
                
                rptPharmItems.DataSource = items.Where(x => x.Product.IsPharmaceutical == true).ToList();
                rptPharmItems.DataBind();
                phPharm.Visible = rptPharmItems.Items.Count > 0;
                phPharmFormTab.Visible = rptPharmItems.Items.Count > 0;
                
                var isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);

                if (isAnonymous == false)
                {
                    var account = AccountService.GetAccountByProfileId(profileId);

                    if (account == null)
                    {
                        _logger.WarnFormat("Account could not be loaded. Account ID={{{0}}}.", profileId);
                        Response.Redirect("/sales/order_new_select_customer.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotLoaded);
                    }

                    // Validate profile attributes
                    var profile = AccountService.GetProfileById(profileId);

                    // Check if the profile has predefined country id
                    var countryId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId, UtilityService);
                    if (countryId == 0)
                    {
                        // If user does not have predefined country id, check primary shipping address for country id. 
                        // Remember that old generic attributes are deleted every 12 hours.
                        // Please refer Apollo.Core.Services.Common.ClearGenericAttributeTask class for more information.
                        var primaryShippingAddress = AccountService.GetShippingAddressByAccountId(account.Id);

                        if (primaryShippingAddress != null)
                            countryId = primaryShippingAddress.CountryId;
                        else
                            countryId = ShippingSettings.PrimaryStoreCountryId;

                        UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CountryId, countryId.ToString());
                    }                        

                    // Check if the profile has predefined currency id                    
                    var currencyId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CurrencyId, UtilityService);
                    if (currencyId == 0) UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CurrencyId, CurrencySettings.PrimaryStoreCurrencyId.ToString());
                    
                    int loyaltyPoints = AccountService.GetLoyaltyPointsBalanceByAccountId(account.Id);
                    var allocatedPoints = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.AllocatedPoint, UtilityService);

                    litLoyaltyPointsAvail.Text = string.Format(AVAILABLE_POINTS_FORMAT, loyaltyPoints - allocatedPoints, allocatedPoints);
                    var maxValue = loyaltyPoints - allocatedPoints;
                    valLoyaltyPoints.MinimumValue = "0";
                    valLoyaltyPoints.MaximumValue = maxValue.ToString();
                    valLoyaltyPoints.ErrorMessage = string.Format(LOYALTY_POINTS_VALIDATOR_ERROR_MESSAGE, maxValue);

                    LoadExistingOrders(profileId);
                    LoadCustomerDetails(account);
                }
                else
                {
                    var savedAccountData = SessionFacade.AccountLite;
                    if (savedAccountData != null && savedAccountData.ProfileId == profileId)
                        LoadSavedContactInformation(savedAccountData);
                    else
                        SessionFacade.AccountLite = null;

                    hfCurrentTab.Value = "addresses";
                    phLoyalty.Visible = false;
                }

                // Load selected currency
                ddlCurrency.SelectedIndex = -1;
                var item = ddlCurrency.Items.FindByValue(CustomerCurrency.CurrencyCode);
                if (item != null) item.Selected = true;
            }

            #endregion
            
            LoadDeliveryRate(profileId);
            LoadOrderSummary();
        }

        protected override void OnInit(EventArgs e)
        {
            LoadCustomerAgesForPharmForm(ddlOwnerAge);

            ddlPaymentType.DataTextField = KEY;
            ddlPaymentType.DataValueField = VALUE;
            ddlPaymentType.DataSource = _supportedCards;
            ddlPaymentType.DataBind();

            base.OnInit(e);
        }

        #region Existing order

        protected void ddlExistingOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}&orderid={1}", QueryUserId, ddlExistingOrders.SelectedValue));
        }

        protected void ddlDiscardedOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState.Clear();
            int orderId = Convert.ToInt32(ddlDiscardedOrders.SelectedValue);
            
            Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}&orderid={1}", QueryUserId, orderId));
        }

        private void LoadLineItemsToCart(IList<LineItem> items, int profileId)
        {
            // Clear existing items from cart
            CartService.DeleteCartItemsByProfileId(profileId);

            enbNotice.ClearMessage();

            var cartItems = new List<CartItem>();

            // Add line items into cart
            for (int i = 0; i < items.Count; i++)
            {
                var product = ProductService.GetProductById(items[i].ProductId);
                if (product == null)
                {
                    enbNotice.AppendMessage = string.Format("Product '{0}' could not be loaded. Product ID = {1}.", items[i].Name, items[i].ProductId);
                    continue;
                }

                var productPrice = product.ProductPrices.Where(x => x.Id == items[i].ProductPriceId && x.Enabled == true).FirstOrDefault();
                if (productPrice == null)
                {
                    enbNotice.AppendMessage = string.Format("Product price '{0}' could not be loaded. Product Price ID = {1}.", items[i].Name, items[i].ProductPriceId);
                    continue;
                }

                var option = string.Empty;
                switch ((OptionType)product.OptionType)
                {
                    case OptionType.None:
                        break;
                    case OptionType.Size:
                        option = productPrice.Size;
                        break;
                    case OptionType.Colour:
                        if (productPrice.ColourId.HasValue)
                        {
                            var color = ProductService.GetColour(productPrice.ColourId.Value);
                            if (color != null) option = color.Value;
                        }
                        break;
                    case OptionType.GiftCard:
                        break;
                    default:
                        break;
                }
                
                var cartItem = new CartItem
                {
                    ProfileId = profileId,
                    ProductId = product.Id,
                    ProductPriceId = productPrice.Id,
                    Quantity = items[i].Quantity
                };

                CartService.InsertCartItem(cartItem);
            }
        }

        private void LoadExistingOrders(int profileId)
        {
            LoadOrdersInDropDownList(OrderService.GetOrderIdListByProfileIdAndStatusCode(profileId,
                OrderStatusCode.AWAITING_COMPLETION), ddlExistingOrders);

            LoadOrdersInDropDownList(OrderService.GetOrderIdListByProfileIdAndStatusCode(profileId,
                OrderStatusCode.DISCARDED), ddlDiscardedOrders);
        }

        private void LoadOrdersInDropDownList(IList<int> list, DropDownList ddl)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(ORDER_ID_);
            dt.Columns.Add(ORDER_);
            
            int maxCount = 10;
            if (list.Count < 10) maxCount = list.Count;

            list = list.OrderBy(x => x).ToList();

            for (int i = 0; i < maxCount; i++)
            {
                if (i == 0)
                {
                    DataRow firstRow = dt.NewRow();
                    firstRow[ORDER_ID_] = 0;
                    firstRow[ORDER_] = AppConstant.DEFAULT_SELECT;
                    dt.Rows.Add(firstRow);
                }

                DataRow row = dt.NewRow();
                var order = OrderService.GetCompleteOrderById(list[i]);

                row[ORDER_ID_] = order.Id;

                string existingOrderText = string.Format(EXISTING_ORDER_FORMAT,
                    order.Id,
                    order.BillTo,
                    order.AddressLine1,
                    order.PostCode,
                    order.OrderPlaced.Value.ToShortDateString(),
                    order.OrderPlaced.Value.ToLongTimeString(),
                    order.ItemQuantity,
                    UtilityService.GetCurrencyByCurrencyCode(order.CurrencyCode).Symbol,
                    order.GrandTotal);

                row[ORDER_] = existingOrderText;
                dt.Rows.Add(row);
            }

            ddl.DataTextField = "Order";
            ddl.DataValueField = "OrderId";
            ddl.DataSource = dt;
            ddl.DataBind();
            
            ddl.Visible = dt.Rows.Count > 0;
        }

        private void LoadOrder(int orderId, int profileId, bool loadAddressFlag = false)
        {
            var order = OrderService.GetCompleteOrderById(orderId);
            
            UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CountryId, order.ShippingCountryId.ToString());
            var currency = UtilityService.GetCurrencyByCurrencyCode(order.CurrencyCode);
            UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CurrencyId, currency.Id.ToString());
            
            int pharmCount = order.LineItemCollection.Where(x => x.IsPharmaceutical == true).Count();
            if (pharmCount > 0)
            {
                rptPharmItems.DataSource = order.LineItemCollection.Select(x => x.IsPharmaceutical).ToList();
                rptPharmItems.DataBind();                
            }

            phPharm.Visible = pharmCount > 0;
            phPharmFormTab.Visible = pharmCount > 0;
            
            var profile = AccountService.GetProfileById(profileId);

            var allocatedPoints = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.AllocatedPoint, UtilityService);

            txtLoyaltyPointsToUse.Text = allocatedPoints.ToString();
            txtPromoCode.Text = order.PromoCode;
            txtNotes.Text = order.Packing;

            ddlCurrency.SelectedIndex = ddlCurrency.Items.IndexOf(ddlCurrency.Items.FindByValue(order.CurrencyCode));

            if (loadAddressFlag == true)
            {
                adrBilling.Address = BuildBillingAddressFromOrder(order);
                adrShipping.Address = BuildShippingAddressFromOrder(order);
            }

            LoadLineItemsToCart(order.LineItemCollection.ToList(), profileId);

            LoadOrderSummary();
            
            var shippingOptionId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.SelectedShippingOption, UtilityService);
            
            var item = rblDeliveryRate.Items.FindByValue(shippingOptionId.ToString());
            if (item != null)rblDeliveryRate.SelectedIndex = rblDeliveryRate.Items.IndexOf(item);
        }
        
        private void LoadCustomerDetails(Account account)
        {
            litCIName.Text = account.Name;
            litCIEmail.Text = txtCIEmail.Text = account.Email;
            litCIDOB.Text = account.DOB;
            litCIContactNumber.Text = txtCIContactNumber.Text = account.ContactNumber;

            txtCIName.Visible = false;
            txtCIEmail.Visible = false;
            txtCIDOB.Visible = false;
            txtCIContactNumber.Visible = false;

            rfvCIName.Visible = false;
            rfvCIEmail.Visible = false;

            chkCreateAccount.Checked = false;
            phCreateAccount.Visible = false;
        }

        private void LoadSavedContactInformation(AccountLite account)
        {
            txtCIName.Text = account.Name;
            txtCIEmail.Text = account.Email;
            txtCIContactNumber.Text = account.ContactNumber;
            txtCIDOB.Text = account.DateOfBirth;
            chkCreateAccount.Checked = account.CreateAccount;
        }

        #endregion

        #region Addresses        

        protected void adrBilling_AddressCountryChanged(int countryId)
        {
            hfCurrentTab.Value = "addresses";
        }

        protected void adrShipping_AddressCountryChanged(int countryId)
        {
            int profileId = QueryUserId;
            UtilityService.SaveAttribute(profileId,
                                       "Profile",
                                        SystemCustomerAttributeNames.CountryId,
                                        countryId.ToString());

            //LoadDeliveryRate(profileId);
            var country = ShippingService.GetCountryById(countryId);
            var messages = CartService.ProcessPostalRestrictionRules(profileId, country.ISO3166Code);

            if (messages.Length > 0)
            {
                enbNotice.Message = string.Join("<br/><br/>", messages);
            }

            RefreshVariablePrices();
            hfCurrentTab.Value = "addresses";
        }

        protected void adrBilling_AddressChanged(int addressId)
        {
            hfCurrentTab.Value = "addresses";
        }

        protected void adrShipping_AddressChanged(int addressId)
        {
            var address = AccountService.GetAddressById(addressId);

            if (address == null)
            {
                enbNotice.Message = "Address is not found. Please try another address.";                
            }
            else
            {
                int profileId = QueryUserId;
                UtilityService.SaveAttribute(
                    profileId,
                    "Profile",
                    SystemCustomerAttributeNames.CountryId,
                    address.CountryId.ToString());
                LoadDeliveryRate(profileId);
            }
            
            RefreshVariablePrices();
            hfCurrentTab.Value = "addresses";
        }

        protected void lbCopyToBilling_Click(object sender, EventArgs e)
        {
            adrBilling.Address = adrShipping.Address;
            adrBilling.ForceAddressLoading = true;
            enbNotice.Message = "Address is copied to billing address successfully.";
            hfCurrentTab.Value = "addresses";
        }

        protected void lbCopyShipping_Click(object sender, EventArgs e)
        {
            adrShipping.Address = adrBilling.Address;
            adrShipping.ForceAddressLoading = true;
            enbNotice.Message = "Address is copied to shipping address successfully.";
            hfCurrentTab.Value = "addresses";
        }

        private void SaveAddresses(int accountId)
        {
            if (adrBilling.SaveAddress)
            {
                var address = adrBilling.Address;
                address.AccountId = accountId;
                AccountService.InsertAddress(address);
            }

            if (adrShipping.SaveAddress)
            {
                var address = adrShipping.Address;
                address.AccountId = accountId;
                AccountService.InsertAddress(address);
            }
        }

        private Address BuildBillingAddressFromOrder(Order order)
        {
            return new Address
            {
                Name = order.BillTo,
                AddressLine1 = order.AddressLine1,
                AddressLine2 = order.AddressLine2,
                City = order.City,
                County = order.County,
                CountryId = order.CountryId,
                Country = order.Country,
                PostCode = order.PostCode,
                IsBilling = true,
                USStateId = order.USStateId,
                USState = order.USState
            };
        }

        private Address BuildShippingAddressFromOrder(Order order)
        {
            return new Address
            {
                Name = order.ShipTo,
                AddressLine1 = order.ShippingAddressLine1,
                AddressLine2 = order.ShippingAddressLine2,
                City = order.ShippingCity,
                County = order.ShippingCounty,
                CountryId = order.ShippingCountryId,
                Country = order.ShippingCountry,
                PostCode = order.ShippingPostCode,
                IsShipping = true,
                USStateId = order.ShippingUSStateId,
                USState = order.ShippingUSState
            };
        }

        #endregion

        #region Promo code / Loyalty points

        protected void lbApplyPromo_Click(object sender, EventArgs e)
        {
            var profileId = QueryUserId;
            UtilityService.SaveAttribute(profileId, 
                "Profile", 
                SystemCustomerAttributeNames.DiscountCouponCode, 
                txtPromoCode.Text.Trim());
            RefreshVariablePrices();
            hfCurrentTab.Value = "promo";

            enbNotice.Message = "Promo code was applied successfully.";
        }

        protected void lbRemovePromo_Click(object sender, EventArgs e)
        {
            var profileId = QueryUserId;
            UtilityService.SaveAttribute(profileId,
                "Profile",
                SystemCustomerAttributeNames.DiscountCouponCode,
                string.Empty);

            RefreshVariablePrices();            
            txtPromoCode.Text = string.Empty;
            hfCurrentTab.Value = "promo";

            enbNotice.Message = "Promo code was removed successfully.";
        }

        protected void lbClearPoints_Click(object sender, EventArgs e)
        {
            UtilityService.SaveAttribute(QueryUserId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, "0");

            txtLoyaltyPointsToUse.Text = "0";
            RefreshVariablePrices();
            hfCurrentTab.Value = "promo";

            enbNotice.Message = "Points were cleared successfully.";
        }

        /// <summary>
        /// This link button would be hidden if user is anonymous.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbApplyPoints_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;            
            int inputPoints = txtLoyaltyPointsToUse.Text.Trim().Length != 0 ? Convert.ToInt32(txtLoyaltyPointsToUse.Text) : 0;

            var result = CartService.ProcessAllocatedPoints(profileId, inputPoints, false);

            if (string.IsNullOrEmpty(result.Message))
            {
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, result.AllocatedPoints.ToString());
                enbNotice.Message = "Points were applied successfully.";
            }
            else
            {
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, "0");
                enbNotice.Message = result.Message;
            }

            RefreshVariablePrices();

            hfCurrentTab.Value = "promo";
        }

        #endregion

        #region Pharm form

        protected void ddlAge_Init(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            LoadCustomerAgesForPharmForm(ddl);
        }

        protected void ddlPersistedDays_Init(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            LoadPersistedInDaysForPharmForm(ddl);
        }

        protected string CheckForCodeineMessage(int productId)
        {
            var found = ProductService.BelongToThisProductGroup(productId, Convert.ToInt32(ProductGroupType.Codeine));
            if (found)
                return "<p class=\"text-danger\">This item contains codeine. It should not be used for longer than 3 days, if symptoms persist please contact your doctor. Please be aware codeine can be addictive.</p>";
            return string.Empty;
        }

        private void LoadCustomerAgesForPharmForm(DropDownList ddl)
        {
            for (int i = 1; i <= 100; i++)
                ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));

            ddl.Items.Insert(0, new ListItem("< 1 year", "< 1 year"));
            ddl.Items.Insert(0, new ListItem("Years", ""));
        }

        private void LoadPersistedInDaysForPharmForm(DropDownList ddl)
        {
            for (int i = 1; i <= 120; i++)
                ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));

            ddl.Items.Add(new ListItem("> 120 days", "> 120 days"));
            ddl.Items.Insert(0, new ListItem("Days", ""));
        }

        private CartPharmOrder BuildCartPharmOrder(out string message)
        {
            message = string.Empty;
            int profileId = QueryUserId;
            bool proceed = CartService.HasPharmItem(profileId);
            if (proceed == false) return null;

            if (cbOtherCondOwner.Checked && txtOwnerOtherCond.Text.Trim() == string.Empty)
            {
                message = "Pharm form: Please enter conditions or other medication of patient.";
                return null;
            }

            var pharmForm = new CartPharmOrder
            {
                ProfileId = profileId,
                TakenByOwner = cbTakenByOwner.Checked,
                OwnerAge = ddlOwnerAge.SelectedValue,
                HasOtherCondMed = cbOtherCondOwner.Checked,
                Allergy = string.IsNullOrEmpty(txtAllergy.Text.Trim()) ? null : txtAllergy.Text.Trim()
            };            

            if (pharmForm.HasOtherCondMed)
                pharmForm.OtherCondMed = Server.HtmlEncode(txtOwnerOtherCond.Text.Trim());

            for (int i = 0; i < rptPharmItems.Items.Count; i++)
            {
                PlaceHolder ph = rptPharmItems.Items[i].FindControl("phItemArea") as PlaceHolder;

                if (ph.Visible)
                {
                    var hfProductId = rptPharmItems.Items[i].FindControl("hfProductId") as HiddenField;
                    var hfProductPriceId = rptPharmItems.Items[i].FindControl("hfProductPriceId") as HiddenField;
                    var txtSymptom = rptPharmItems.Items[i].FindControl("txtSymptom") as TextBox;
                    var txtMedForSymptom = rptPharmItems.Items[i].FindControl("txtMedForSymptom") as TextBox;
                    var cbOtherCond = rptPharmItems.Items[i].FindControl("cbOtherCond") as CheckBox;
                    var ddlPersistedDays = rptPharmItems.Items[i].FindControl("ddlPersistedDays") as DropDownList;
                    var cbHasTaken = rptPharmItems.Items[i].FindControl("cbHasTaken") as CheckBox;
                    var txtActionTaken = rptPharmItems.Items[i].FindControl("txtActionTaken") as TextBox;

                    var item = new CartPharmItem();
                    item.ProductId = Convert.ToInt32(hfProductId.Value);
                    item.ProductPriceId = Convert.ToInt32(hfProductPriceId.Value);
                    item.Symptoms = Server.HtmlEncode(txtSymptom.Text.Trim());
                    item.MedForSymptom = Server.HtmlEncode(txtMedForSymptom.Text.Trim());

                    if (ddlPersistedDays.SelectedIndex > 0)
                        item.PersistedInDays = ddlPersistedDays.SelectedValue;

                    item.ActionTaken = txtActionTaken.Text.Trim();

                    if (!pharmForm.TakenByOwner)
                    {
                        var ddl = rptPharmItems.Items[i].FindControl("ddlAge") as DropDownList;
                        bool hasOtherCond = cbOtherCond.Checked;

                        item.Age = ddl.SelectedValue;
                        item.HasOtherCondMed = hasOtherCond;

                        if (hasOtherCond)
                        {
                            TextBox otherCond = rptPharmItems.Items[i].FindControl("txtOtherCond") as TextBox;
                            item.OtherCondMed = Server.HtmlEncode(otherCond.Text.Trim());
                        }

                        bool hasTaken = cbTakenByOwner.Checked;

                        item.HasTaken = hasTaken.ToString();
                        
                        if (hasTaken)
                        {
                            var ddlTakenQuantity = rptPharmItems.Items[i].FindControl("ddlTakenQuantity") as DropDownList;
                            var txtLastTimeTaken = rptPharmItems.Items[i].FindControl("txtLastTimeTaken") as TextBox;

                            item.TakenQuantity = ddlTakenQuantity.SelectedValue;
                            item.LastTimeTaken = txtLastTimeTaken.Text;
                        }
                    }

                    pharmForm.Items.Add(item);
                }
            }

            return pharmForm;
        }
        
        #endregion

        #region Payment / Delivery

        private void LoadDeliveryRate(int profileId)
        {
            var options = CartService.GetCustomerShippingOptionByCountryAndPriority(profileId, false);

            foreach (var option in options)
            {
                if (option.Cost <= 0M)
                    option.Description += " - FREE";
                else
                    option.Description += " - " + AdminStoreUtility.GetFormattedPrice(option.Cost, CustomerCurrency.CurrencyCode, CurrencyType.HtmlEntity);
            }

            rblDeliveryRate.DataSource = options;
            rblDeliveryRate.DataBind();

            var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);
            var itemTotalQuantity = items.Select(i => i.Quantity).Sum();
            var profile = AccountService.GetProfileById(profileId);
            var shippingOptionId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.SelectedShippingOption, UtilityService);
            ShippingOption customerShippingOption = ShippingService.GetShippingOptionById(shippingOptionId);

            ltEstimatedWeight.Text = items.Select(i => i.ProductPrice.Weight).Sum().ToString();

            for (int i = 0; i < rblDeliveryRate.Items.Count; i++)
            {
                var currentShippingId = Convert.ToInt32(rblDeliveryRate.Items[i].Value);

                if (customerShippingOption != null && customerShippingOption.Id == currentShippingId)
                    rblDeliveryRate.Items[i].Selected = true;
            }

            if (rblDeliveryRate.SelectedIndex == -1)
            {
                rblDeliveryRate.SelectedIndex = 0; // Choose the default value
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, rblDeliveryRate.SelectedValue);
            }

        }
        
        protected void rblDeliveryRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblDeliveryRate.SelectedItem != null)
            {
                var profileId = QueryUserId;
                var option = ShippingService.GetShippingOptionById(Convert.ToInt32(rblDeliveryRate.SelectedValue)); // Assign the default
                
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, rblDeliveryRate.SelectedValue);

                //adrShipping.ProfileId = profileId;
                if (adrShipping.Address.CountryId != option.CountryId)
                    adrShipping.ClearAddress();
                
                RefreshVariablePrices();
                hfCurrentTab.Value = "payment";
            }
        }

        protected void ddlExpiryDateYY_Init(object sender, EventArgs e)
        {
            ddlExpiryDateYY.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < MAX_YEAR; i++)
                ddlExpiryDateYY.Items.Add((currentYear + i).ToString());
        }

        protected void ddlStartDateYY_Init(object sender, EventArgs e)
        {
            ddlStartDateYY.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < MAX_YEAR; i++)
                ddlStartDateYY.Items.Add((currentYear - i).ToString());
            ddlStartDateYY.Items.Insert(0, string.Empty);
        }

        protected void lbPlaceOrder_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;
            string message = string.Empty;
            var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);

            #region Validate account creation

            if (chkCreateAccount.Checked)
            {
                message = ConvertToRegisteredAccount(profileId);

                if (!string.IsNullOrEmpty(message))
                {
                    enbNotice.Message = message;
                    return;
                }
            }

            #endregion
            
            #region Pharm form

            var form = BuildCartPharmOrder(out message);
            if (form != null) CartService.ProcessCartPharmOrder(form);

            if (!string.IsNullOrEmpty(message))
            {
                enbNotice.Message = message;
                return;
            }

            #endregion

            string paymentMethod = ddlPaymentType.SelectedValue;
            var billingAddress = adrBilling.Address;
            var shippingAddress = adrShipping.Address;
            var earnedPoint = CartService.CalculateEarnedLoyaltyPointsFromCart(profileId);
            string email = txtCIEmail.Text.Trim();
            string contactNumber = txtCIContactNumber.Text.Trim();
            bool sendEmailFlag = string.IsNullOrEmpty(email) ? false : true;
            var card = GetCard();

            var orderTotals = CartService.CalculateOrderTotals(profileId, false);
            var exemptedFromPayment = orderTotals.Total <= 0M && orderTotals.Subtotal > 0M;

            var output = PaymentService.ProcessPaymentFromBackOffice(
                profileId,
                paymentMethod,
                email,
                contactNumber,
                Request.UserAgent,
                Request.UserHostAddress == "::1" ? "45.0.1.100" : Request.UserHostAddress,
                card,
                billingAddress,
                shippingAddress,
                sendEmailFlag,
                exemptedFromPayment);

            string msg = string.Empty;

            if (output.Status)
            {
                // Reset related session variables
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, "0");
                CartService.ClearCart(profileId);

                msg = "Order was placed successfully. The Order ID is <a href='/sales/order_info.aspx?orderid=" + output.OrderId + "'>" + output.OrderId + "</a>.";

                if (sendEmailFlag)
                    msg += " An order confirmation email should have been sent to customer.";
                else
                    msg += " Order confirmation email is NOT sent as email is not entered.";
            }
            else
            {
                msg = "Order was failed to place. The order has been discarded, however a new one can be rebuilt using the supplied information. <br/>" + output.Message;
            }

            enbNotice.Message = msg;
        }

        private Card GetCard()
        {
            Card card = new Card
            {
                CardType = ddlPaymentType.SelectedValue,
                CardNumber = RegexType.WhiteSpace.Replace(txtCardNumber.Text, string.Empty),
                HolderName = txtCardHolderName.Text.Trim(),
                SecurityCode = txtVerificationNumber.Text.Trim(),
                ExpiryMonth = ddlExpiryDateMM.SelectedValue,
                ExpiryYear = ddlExpiryDateYY.SelectedValue,
                StartMonth = _supportedCards[ddlPaymentType.SelectedItem.Text].HasStartDate ? ddlStartDateMM.SelectedValue : string.Empty,
                StartYear = _supportedCards[ddlPaymentType.SelectedItem.Text].HasStartDate ? ddlStartDateYY.SelectedValue : string.Empty,
                IssueNumber = _supportedCards[ddlPaymentType.SelectedItem.Text].HasIssueNumber ? txtIssueNumber.Text.Trim() : string.Empty,
            };

            return card;
        }

        #endregion

        #region Currency

        protected void ddlCurrency_Init(object sender, EventArgs e)
        {
            var list = UtilityService.GetAllCurrency().ToList();
            list.Insert(0, new Currency { CurrencyCode = AppConstant.DEFAULT_SELECT });
            ddlCurrency.DataSource = list;
            ddlCurrency.DataBind();
        }

        protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currencyCode = ddlCurrency.SelectedValue;
            var currency = UtilityService.GetCurrencyByCurrencyCode(currencyCode);
            UtilityService.SaveAttribute(QueryUserId, "Profile", SystemCustomerAttributeNames.CurrencyId, currency.Id.ToString());
            
            RefreshVariablePrices();
        }

        #endregion
        
        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
        }

        protected void gvTempCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row;            
            int profileId = QueryUserId;
            int productPriceId;
            
            switch (e.CommandName)
            {
                case "updateItem":
                    row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    
                    productPriceId = Convert.ToInt32(e.CommandArgument);
                    
                    TextBox txtQty = (TextBox)row.FindControl(TXT_QTY);
                    int quantity = Convert.ToInt32(txtQty.Text.Trim());

                    CartService.UpdateLineQuantityByProfileIdAndProductPriceId(profileId, productPriceId, quantity);
                    break;
                case "removeItem":
                    row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    
                    productPriceId = Convert.ToInt32(e.CommandArgument);
                    
                    CartService.DeleteCartItemsByProfileIdAndPriceId(profileId, productPriceId, freeItemIncluded: true);
                    break;
                default:
                    break;
            }

            var cartOffer = OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);

            if (cartOffer.IsValid && string.IsNullOrEmpty(cartOffer.Description) == false)
                enbNotice.AppendMessage = cartOffer.Description;
        }
        
        protected void lbAddItems_Click(object sender, EventArgs e)
        {
            var orderId = QueryOrderId;
            var profileId = QueryUserId;
            var isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);

            // If it is a anonymous profile, please save contact information before proceed.
            if (isAnonymous)
            {
                SessionFacade.AccountLite = new AccountLite
                {
                    ProfileId = profileId,
                    Name = txtCIName.Text.Trim(),
                    Email = txtCIEmail.Text.Trim(),
                    ContactNumber = txtCIContactNumber.Text.Trim(),
                    DateOfBirth = txtCIDOB.Text.Trim(),
                    CreateAccount = chkCreateAccount.Checked
                };
            }

            if (orderId != 0)
                Response.Redirect(string.Format("/sales/order_new_add_from_cart.aspx?userid={0}&orderid={1}&isloaded=1", profileId, orderId));

            Response.Redirect(string.Format("/sales/order_new_add_from_cart.aspx?userid={0}", profileId));
        }
        
        protected void lbSaveOrder_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;
            string message = string.Empty;

            // chkCreateAccount will only appear if user is anonymous
            if (chkCreateAccount.Checked)
            {
                message = ConvertToRegisteredAccount(profileId);

                if (!string.IsNullOrEmpty(message))
                {
                    enbNotice.Message = message;
                    return;
                }
            }
            
            // Save address only if user is not anonymous
            bool isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);
            if (isAnonymous == false)
            {
                var account = AccountService.GetAccountByProfileId(profileId);
                SaveAddresses(account.Id);
            }

            if (QueryOrderId == 0)
            {
                var orderId = CreateOrder();
                Response.Redirect("/sales/order_new.aspx?userid=" + QueryUserId + "&orderid=" + orderId + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderSaved);
            }
            else
                SaveOrder(QueryOrderId);
        }

        protected void lbSaveOrderAndDeleteBasket_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;
            string message = string.Empty;

            // chkCreateAccount will only appear if user is anonymous
            if (chkCreateAccount.Checked)
            {
                message = ConvertToRegisteredAccount(profileId);

                if (!string.IsNullOrEmpty(message))
                {
                    enbNotice.Message = message;
                    return;
                }
            }

            // Save address only if user is not anonymous
            bool isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);
            if (isAnonymous == false)
            {
                var account = AccountService.GetAccountByProfileId(profileId);
                SaveAddresses(account.Id);
            }

            if (QueryOrderId == 0)
            {
                var orderId = CreateOrder();
                //Response.Redirect("/sales/order_new.aspx?userid=" + QueryUserId + "&orderid=" + orderId + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderSaved);

                // Reset related session variables
                UtilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, "0");
                CartService.ClearCart(profileId);

                var msg = "Order was saved successfully. The Order ID is <a href='/sales/order_info.aspx?orderid=" + orderId + "'>" + orderId + "</a>.";

                enbNotice.Message = msg;
            }
            else
                SaveOrder(QueryOrderId);
        }

        protected void lbRefreshOrder_Click(object sender, EventArgs e)
        {
            RefreshVariablePrices();
        }
        
        private int CreateOrder()
        {
            int profileId = QueryUserId;

            var order = new Order();

            order.ProfileId = profileId;
            order.BillTo = adrBilling.Address.Name;
            order.AddressLine1 = adrBilling.Address.AddressLine1;
            order.AddressLine2 = adrBilling.Address.AddressLine2;
            order.City = adrBilling.Address.City;
            order.County = adrBilling.Address.County;
            order.CountryId = adrBilling.Address.CountryId;
            order.PostCode = adrBilling.Address.PostCode;
            order.USStateId = adrBilling.Address.USStateId;
            order.ShipTo = adrShipping.Address.Name;
            order.ShippingAddressLine1 = adrShipping.Address.AddressLine1;
            order.ShippingAddressLine2 = adrShipping.Address.AddressLine2;
            order.ShippingCity = adrShipping.Address.City;
            order.ShippingCounty = adrShipping.Address.County;
            order.ShippingCountryId = adrShipping.Address.CountryId;
            order.ShippingPostCode = adrShipping.Address.PostCode;
            order.ShippingUSStateId = adrShipping.Address.USStateId;
            order.Packing = txtNotes.Text.Trim();

            var orderTotals = CartService.CalculateOrderTotals(profileId, false);
            var profile = AccountService.GetProfileById(profileId);
            var shippingOptionId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.SelectedShippingOption, UtilityService);
            order.ShippingOptionId = shippingOptionId;

            var promocode = profile.GetAttribute<string>("Profile", SystemCustomerAttributeNames.DiscountCouponCode, UtilityService);
            order.PromoCode = promocode;

            var cartOffer = OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);
            order.DiscountAmount = cartOffer.DiscountAmount * orderTotals.ExchangeRate;
            order.AwardedPoint = cartOffer.RewardPoint;

            order.ShippingCost = orderTotals.ShippingCost * orderTotals.ExchangeRate;
            order.AllocatedPoint = orderTotals.AllocatedPoints;
            order.EarnedPoint = orderTotals.EarnedPoints;
            order.CurrencyCode = orderTotals.CurrencyCode;
            order.ExchangeRate = orderTotals.ExchangeRate;
            order.IPAddress = Request.UserHostAddress;
            order.StatusCode = OrderStatusCode.AWAITING_COMPLETION;
            order.LastAlertDate = DateTime.Now;
            order.OrderPlaced = DateTime.Now;

            order.Id = OrderService.InsertOrder(order);

            #region Line items

            var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);
            if (items.Count > 0)
                OrderService.InsertLineItemsFromCartItems(items, order.Id, orderTotals.CurrencyCode, orderTotals.ExchangeRate);

            #endregion

            #region Pharm form

            var message = string.Empty;
            var form = BuildCartPharmOrder(out message);
            
            if (form != null)
            {
                // Create cart pharm form
                CartService.ProcessCartPharmOrder(form);

                // Create order pharm form from cart
                OrderService.PreparePharmOrderFromCartPharmOrder(profileId, order.Id);
            }

            #endregion

            return order.Id;
        }

        private void SaveOrder(int orderId)
        {
            int profileId = QueryUserId;
            var order = OrderService.GetCompleteOrderById(orderId);

            order.ProfileId = profileId;
            order.BillTo = adrBilling.Address.Name;
            order.AddressLine1 = adrBilling.Address.AddressLine1;
            order.AddressLine2 = adrBilling.Address.AddressLine2;
            order.City = adrBilling.Address.City;
            order.County = adrBilling.Address.County;
            order.CountryId = adrBilling.Address.CountryId;
            order.PostCode = adrBilling.Address.PostCode;
            order.USStateId = adrBilling.Address.USStateId;
            order.ShipTo = adrShipping.Address.Name;
            order.ShippingAddressLine1 = adrShipping.Address.AddressLine1;
            order.ShippingAddressLine2 = adrShipping.Address.AddressLine2;
            order.ShippingCity = adrShipping.Address.City;
            order.ShippingCounty = adrShipping.Address.County;
            order.ShippingCountryId = adrShipping.Address.CountryId;
            order.ShippingPostCode = adrShipping.Address.PostCode;
            order.ShippingUSStateId = adrShipping.Address.USStateId;
            order.Packing = txtNotes.Text.Trim();

            var orderTotals = CartService.CalculateOrderTotals(profileId, false);
            var profile = AccountService.GetProfileById(profileId);
            var shippingOptionId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.SelectedShippingOption, UtilityService);
            order.ShippingOptionId = shippingOptionId;

            var promocode = profile.GetAttribute<string>("Profile", SystemCustomerAttributeNames.DiscountCouponCode, UtilityService);
            order.PromoCode = promocode;

            var cartOffer = OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);
            order.DiscountAmount = cartOffer.DiscountAmount * orderTotals.ExchangeRate;
            order.AwardedPoint = cartOffer.RewardPoint;

            order.ShippingCost = orderTotals.ShippingCost * orderTotals.ExchangeRate;
            order.AllocatedPoint = orderTotals.AllocatedPoints;
            order.EarnedPoint = orderTotals.EarnedPoints;
            order.CurrencyCode = orderTotals.CurrencyCode;
            order.ExchangeRate = orderTotals.ExchangeRate;
            order.IPAddress = Request.UserHostAddress;
            order.StatusCode = OrderStatusCode.AWAITING_COMPLETION;
            order.LastAlertDate = DateTime.Now;
            order.OrderPlaced = DateTime.Now;

            OrderService.UpdateOrder(order);

            #region Line items

            OrderService.DeleteLineItemsByOrderId(order.Id);
            var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);
            OrderService.InsertLineItemsFromCartItems(items.ToList(), order.Id, CustomerCurrency.CurrencyCode, CustomerCurrency.ExchangeRate);

            #endregion
            
            #region Pharm form

            OrderService.DeletePharmOrderAndItemsByOrderId(order.Id);
            var message = string.Empty;
            var form = BuildCartPharmOrder(out message);

            if (!string.IsNullOrEmpty(message))
            {
                enbNotice.Message = message;
                return;
            }

            if (form != null)
            {
                // Create cart pharm form
                CartService.ProcessCartPharmOrder(form);

                // Create order pharm form from cart
                OrderService.PreparePharmOrderFromCartPharmOrder(profileId, order.Id);
            }
            
            #endregion
        }

        private void LoadOrderSummary()
        {
            int profileId = QueryUserId;
            var orderTotals = CartService.CalculateOrderTotals(profileId, false);

            // It could be free if client uses all the points to purchase.
            lbPlaceOrder.Enabled = orderTotals.Subtotal > 0M;
            phPaymentMethod.Visible = orderTotals.Total > 0M;

            litTotalsSubTot.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.Subtotal * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
            litTotalsDiscount.Text = string.Format(SUBTRACT_FORM, AdminStoreUtility.GetFormattedPrice(orderTotals.Discount * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity));

            if(orderTotals.Discounts.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in orderTotals.Discounts)
                {
                    sb.AppendFormat("{0} - {1} {2:0.00}<br/>", item.Key, orderTotals.CurrencyCode, item.Value * orderTotals.ExchangeRate);
                }

                litDiscounts.Text = string.Format("<tr><td colspan='2'>{0}</td></tr>", sb.ToString());
            }
            
            litTotalsPoints.Text = string.Format(SUBTRACT_FORM, AdminStoreUtility.GetFormattedPrice(orderTotals.AllocatedPoints / 100M * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity));

            var tax = 0M;
            if (orderTotals.DisplayTax) tax = orderTotals.Tax;
            litTotalsVAT.Text = AdminStoreUtility.GetFormattedPrice(tax * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);            
            litTotalsDelivery.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.ShippingCost * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
            litTotalsTotal.Text = ltlTotalValue.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.Total * orderTotals.ExchangeRate, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
        }

        private void RefreshVariablePrices()
        {
            int profileId = QueryUserId;
            // Reload offers
            OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);

            // Refresh shipping option selection
            LoadDeliveryRate(profileId);

            // Recalculate order summary
            LoadOrderSummary();
        }

        private Account BuildAccount(int profileId)
        {
            Account account = new Account
            {
                Name = txtCIName.Text.Trim(),
                Username = txtCIEmail.Text.Trim().ToLower(),
                Email = txtCIEmail.Text.Trim().ToLower(),
                ContactNumber = txtCIContactNumber.Text.Trim(),
                DOB = txtCIDOB.Text.Trim()                
            };

            return account;
        }

        private string ConvertToRegisteredAccount(int profileId)
        {
            var account = BuildAccount(profileId);
            var password = AdminStoreUtility.GenerateRandomPasswordGUID(8);
            var output = AccountService.ProcessRegistration(account, password, sendEmailFlag: true, sendPasswordEmailFlag: true);

            if (!string.IsNullOrEmpty(output.Message))
                return output.Message;

            return string.Empty;
        }        
    }
}