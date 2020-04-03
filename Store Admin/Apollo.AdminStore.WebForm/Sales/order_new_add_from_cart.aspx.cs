using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_new_add_from_cart : BasePage
    {
        #region Constants
        private const string DDL_OPTIONS = "ddlOptions";
        private const string LIT_SINGLE_OPTION = "litSingleOption";
        private const string HDN_SINGLE_OPTION_ID = "hdnSingleOptionId";
        private const string HDN_PRODUCT_PRICE_ID = "hdnProductPriceId";
        private const string TXT_QTY = "txtQty";
        private const string CB_CHOSEN = "cbChosen";
        private const string DT_PRODUCT_PRICE_ID = "ProductPriceId";
        private const string DT_OPTION_NAME = "OptionName";
        private const string ORDER_ = "Order";
        private const string ORDER_ID_ = "OrderId";        
        private const string DT_OPTION_NAME_FORMAT_CURRENCY = "{0}{1:0.00} - {2}, {3} in stock";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {5} in stock";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY = "{0}{1:0.00} - {2}, {5} in stock, was {3}{4:0.00}";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {9} in stock, was {5}{6:0.00} ({7}{8:0.00})";
        #endregion

        public IAccountService AccountService { get; set; }
        public IOfferService OfferService { get; set; }
        public ICartService CartService { get; set; }
        public IProductService ProductService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }
        public ShippingSettings ShippingSettings { get; set; }

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
            if (!IsPostBack)
            {
                int userId = QueryUserId;

                if (userId > 0)
                {
                    ectTogRightNav.SetBackUrl = "/sales/order_new.aspx?userid=" + userId;
                    LoadCart(userId);
                }
            }
        }
        
        protected bool IsDiscontinued(int productId)
        {
            bool isDiscontinued = ProductService.GetProductDiscontinuedStatus(productId);
            return isDiscontinued;
        }

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

                    TextBox txtQty = (TextBox)row.FindControl("txtQty");
                    int quantity = Convert.ToInt32(txtQty.Text.Trim());

                    CartService.UpdateLineQuantityByProfileIdAndProductPriceId(profileId, productPriceId, quantity);

                    OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);

                    LoadCart(profileId);
                    break;

                case "removeItem":
                    row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;                    
                    productPriceId = Convert.ToInt32(e.CommandArgument);
                    CartService.DeleteCartItemsByProfileIdAndPriceId(profileId, productPriceId, freeItemIncluded: true);
                    OfferService.ProcessCartOfferByProfileId(profileId, CustomerCountry.ISO3166Code);

                    LoadCart(profileId);
                    break;
                default:
                    break;
            }
        }

        protected void gvCustCart_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var cartItem = (CartItem)e.Row.DataItem;
                BuildAddItemRow(e.Row, cartItem.ProductId, cartItem.ProductPriceId);
            }
        }

        protected void gvCustCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "addProduct":
                    int profileId = QueryUserId;
                    GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    DropDownList ddlOptions = (DropDownList)row.FindControl(DDL_OPTIONS);
                    HiddenField hdnSingleOptionId = (HiddenField)row.FindControl(HDN_SINGLE_OPTION_ID);
                    TextBox txtQty = (TextBox)row.FindControl(TXT_QTY);

                    int qtyToAdd;
                    string message = string.Empty;

                    if (int.TryParse(txtQty.Text, out qtyToAdd))
                    {
                        if (qtyToAdd > 0)
                        {
                            int productId = Convert.ToInt32(e.CommandArgument);
                            int productPriceId;

                            if (ddlOptions.Visible)
                                productPriceId = Convert.ToInt32(ddlOptions.SelectedValue);
                            else
                                productPriceId = Convert.ToInt32(hdnSingleOptionId.Value);

                            message = AddToCart(profileId, productId, productPriceId, CustomerCurrency.CurrencyCode, qtyToAdd);

                            LoadCart(profileId);
                        }
                        else
                        {
                            message = "Please enter a quantity to add which is greater than 0.";
                        }
                    }
                    else
                    {
                        message = "Please enter a whole, numerical quanity.";
                    }

                    enbNotice.Message = message;

                    break;
                default:
                    break;
            }
        }

        protected void lbHideAddItemView_Click(object sender, EventArgs e)
        {
            if (QueryOrderId != 0)
                Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}&orderid={1}&isloaded=1", QueryUserId, QueryOrderId));

            Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}", QueryUserId));
        }
        
        protected void lbAddFromProductSearch_Click(object sender, EventArgs e)
        {
            if (QueryOrderId != 0)
                Response.Redirect(string.Format("/sales/order_new_add_from_product_search.aspx?userid={0}&orderid={1}&isloaded=1", QueryUserId, QueryOrderId));
            
            Response.Redirect(string.Format("/sales/order_new_add_from_product_search.aspx?userid={0}", QueryUserId));
        }

        private void BuildAddItemRow(GridViewRow row, int productId, int loadSelectedProductPriceId = 0)
        {
            DropDownList ddlOptions = (DropDownList)row.FindControl(DDL_OPTIONS);
            Literal litSingleOption = (Literal)row.FindControl(LIT_SINGLE_OPTION);
            HiddenField hdnSingleOptionId = (HiddenField)row.FindControl(HDN_SINGLE_OPTION_ID);
            TextBox txtQty = (TextBox)row.FindControl(TXT_QTY);

            DataTable dt = new DataTable();
            dt.Columns.Add(DT_PRODUCT_PRICE_ID);
            dt.Columns.Add(DT_OPTION_NAME);

            var currencyCode = CustomerCurrency.CurrencyCode;
            var product = ProductService.GetProductById(productId);

            for (int i = 0; i < product.ProductPrices.Count; i++)
            {
                DataRow option = dt.NewRow();
                option["ProductPriceId"] = product.ProductPrices[i].Id;
                option["OptionName"] = BuildOptionName(product.ProductPrices[i], currencyCode, product.ProductPrices[i].Option);

                dt.Rows.Add(option);
            }

            if (dt.Rows.Count > 1)
            {
                ddlOptions.DataSource = dt;
                ddlOptions.DataTextField = DT_OPTION_NAME;
                ddlOptions.DataValueField = DT_PRODUCT_PRICE_ID;
                ddlOptions.DataBind();

                litSingleOption.Visible = false;
                hdnSingleOptionId.Visible = false;
            }
            else if (dt.Rows.Count == 1)
            {
                litSingleOption.Text = Convert.ToString(dt.Rows[0][DT_OPTION_NAME]);
                hdnSingleOptionId.Value = Convert.ToString(dt.Rows[0][DT_PRODUCT_PRICE_ID]);
                ddlOptions.Visible = false;
            }
            else
            {
                ddlOptions.Visible = false;
                litSingleOption.Visible = false;
                hdnSingleOptionId.Visible = false;
            }

            if (loadSelectedProductPriceId != 0 && ddlOptions.Visible)
                ddlOptions.Items.FindByValue(loadSelectedProductPriceId.ToString()).Selected = true;
        }

        private string BuildOptionName(ProductPrice priceOption, string currencyCode, string option)
        {
            string optionName = string.Empty;

            if (priceOption.OfferRuleId != 0)
            {
                if (CustomerCurrency.Id == CurrencySettings.PrimaryStoreCurrencyId)
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_OFFER_CURRENCY,
                                               currencyCode,
                                               priceOption.OfferPriceInclTax,
                                               option,
                                               currencyCode,
                                               priceOption.PriceInclTax,
                                               priceOption.Stock);
                }
                else
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL,
                                               currencyCode,
                                               priceOption.OfferPriceInclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.OfferPriceInclTax / CustomerCurrency.ExchangeRate,
                                               option,
                                               currencyCode,
                                               priceOption.PriceInclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.PriceInclTax / CustomerCurrency.ExchangeRate,
                                               priceOption.Stock);
                }
            }
            else
            {
                if (CustomerCurrency.Id == CurrencySettings.PrimaryStoreCurrencyId)
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_CURRENCY,
                                               currencyCode,
                                               priceOption.PriceInclTax,
                                               option,
                                               priceOption.Stock);
                }
                else
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL,
                                               currencyCode,
                                               priceOption.PriceInclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.PriceInclTax / CustomerCurrency.ExchangeRate,
                                               option,
                                               priceOption.Stock);
                }
            }

            return optionName;
        }

        private string AddToCart(int profileId, int productId, int productPriceId, string currencyCode, int quantity)
        {
            var profile = AccountService.GetProfileById(profileId);
            var countryId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId, UtilityService);
            var country = ShippingService.GetCountryById(countryId);
            var message = CartService.ProcessItemAddition(
                profileId,
                productId,
                productPriceId,
                country.ISO3166Code,
                quantity,
                disablePhoneOrderCheck: true);

            return message;
        }

        private void LoadCart(int profileId)
        {
            var items = CartService.GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems: false);
            
            gvTempCart.DataSource = items;
            gvTempCart.DataBind();

            var isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);

            if (isAnonymous == false)
            {
                gvCustCart.DataSource = items;
                gvCustCart.DataBind();
            }

            phCustomerCart.Visible = !isAnonymous && items.Count > 0;
        }
    }
}