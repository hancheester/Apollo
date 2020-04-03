using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_new_add_from_product_search : BasePage
    {
        #region Constants
        private const int MAX_YEAR = 10;
        private const string KEY = "key";
        private const string VALUE = "value";
        
        private const string DDL_OPTIONS = "ddlOptions";
        private const string LIT_SINGLE_OPTION = "litSingleOption";
        private const string HDN_SINGLE_OPTION_ID = "hdnSingleOptionId";
        private const string TXT_QTY = "txtQty";

        private const string CB_CHOSEN = "cbChosen";
        private const string DT_PRODUCT_PRICE_ID = "ProductPriceId";
        private const string DT_OPTION_NAME = "OptionName";
        
        private const string DT_OPTION_NAME_FORMAT_CURRENCY = "{0}{1:0.00} - {2}, {3} in stock";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {5} in stock";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY = "{0}{1:0.00} - {2}, {5} in stock, was {3}{4:0.00}";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {9} in stock, was {5}{6:0.00} ({7}{8:0.00})";
   
        private const string LINE_BREAK_TAG = "<br />";
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

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            var products = gvProducts.DataSource as ProductOverviewModel[];

            if (products != null && products.Length > 0 && products[0].Id > 0)
            {
                int userId = QueryUserId;

                for (int i = 0; i < gvProducts.Rows.Count; i++)
                {
                    CheckBox cb = gvProducts.Rows[i].FindControl(CB_CHOSEN) as CheckBox;
                    var item = BuildCartItem(Convert.ToInt32(gvProducts.DataKeys[i].Value), userId, gvProducts.Rows[i]);

                    cb.Checked = ExistInChosenToAddItems(item);

                    SetChosenProducts(item, cb.Checked);
                }
            }
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var product = (ProductOverviewModel)e.Row.DataItem;
                if (product.Id > 0)
                {
                    DropDownList ddlOptions = e.Row.FindControl(DDL_OPTIONS) as DropDownList;
                    TextBox txtQty = e.Row.FindControl(TXT_QTY) as TextBox;

                    BuildAddItemRow(e.Row, product.Id);

                    var item = BuildCartItem(product.Id, QueryUserId, e.Row);

                    // Format: ProductId, ProductPriceId, Price, Quantity
                    Tuple<int, int, decimal, int> chosenItem = null;

                    if (ExistInChosenToAddItems(item))
                    {
                        chosenItem = SessionFacade.ChosenToAddItems.Find(delegate (Tuple<int, int, decimal, int> cartItem)
                        {
                            return cartItem.Item1 == item.Item1
                                  && cartItem.Item2 == item.Item2
                                  && cartItem.Item3 == item.Item3;
                        });
                    }

                    if (chosenItem != null)
                        txtQty.Text = chosenItem.Item4.ToString();

                    var productPrices = ProductService.GetProductPricesByProductId(product.Id);

                    if (chosenItem != null && productPrices.Count > 1)
                        ddlOptions.SelectedIndex = ddlOptions.Items.IndexOf(ddlOptions.Items.FindByValue(chosenItem.Item2.ToString()));

                    // If product doesn't have any options, hide it
                    if (productPrices.Count == 0)
                    {
                        var cbChosen = e.Row.FindControl("cbChosen") as CheckBox;
                        var lbAddToBasket = e.Row.FindControl("lbAddToBasket") as LinkButton;

                        lbAddToBasket.Visible = false;
                        cbChosen.Visible = false;
                        txtQty.Visible = false;
                    }
                }
            }
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    LoadCart(profileId);

                    break;
                default:
                    break;
            }
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedProduct();

            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void lbAddFromCustomerCart_Click(object sender, EventArgs e)
        {
            if (QueryOrderId != 0)
                Response.Redirect("/sales/order_new_add_from_cart.aspx?userid=" + QueryUserId + "&orderid=" + QueryOrderId);

            Response.Redirect("/sales/order_new_add_from_cart.aspx?userid=" + QueryUserId);
        }
        
        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, txtProductId.Text.Trim());
            SetState(NAME_FILTER, txtProductName.Text.Trim());
            SetState(BRAND_ID_FILTER, txtBrandId.Text.Trim());
            SetState(CATEGORY_ID_FILTER, txtCategpryId.Text.Trim());
            LoadProducts();
        }

        protected void lbBulkAddFromSearch_Click(object sender, EventArgs e)
        {
            SaveLastViewedProduct();
            StringBuilder sb = new StringBuilder();
            int profileId = QueryUserId;
            string currencyCode = CustomerCurrency.CurrencyCode;

            foreach (var item in SessionFacade.ChosenToAddItems)
            {
                var message = AddToCart(profileId, item.Item1, item.Item2, currencyCode, item.Item4);

                if (!string.IsNullOrEmpty(message))
                {
                    sb.Append(LINE_BREAK_TAG);
                    sb.Append(message);
                }
            }

            var errorMessage = sb.ToString();
            if (!string.IsNullOrEmpty(errorMessage)) enbNotice.Message = errorMessage;

            LoadProducts();
        }

        protected void lbHideAddItemView_Click(object sender, EventArgs e)
        {
            if (QueryOrderId != 0)
                Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}&orderid={1}&isloaded=1", QueryUserId, QueryOrderId));

            Response.Redirect(string.Format("/sales/order_new.aspx?userid={0}", QueryUserId));
        }

        private void LoadCart(int userId)
        {
            var items = CartService.GetCartItemsByProfileId(userId, autoRemovePhoneOrderItems: false);

            gvTempCart.DataSource = items;
            gvTempCart.DataBind();
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            int[] brandIds = null;
            int[] categoryIds = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState("OrderBy")) orderBy = (ProductSortingType)GetIntState("OrderBy");
            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(BRAND_ID_FILTER))
            {
                string value = GetStringState(BRAND_ID_FILTER);
                int temp;
                brandIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(CATEGORY_ID_FILTER))
            {
                string value = GetStringState(CATEGORY_ID_FILTER);
                int temp;
                categoryIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                brandIds: brandIds,
                categoryIds: categoryIds,
                productIds: productIds,
                keywords: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();

            if (gvProducts.Rows.Count <= 0) enbProducts.Message = "No records found.";
        }

        private void SaveLastViewedProduct()
        {
            int userId = QueryUserId;
            for (int i = 0; i < gvProducts.Rows.Count; i++)
            {
                CheckBox cb = gvProducts.Rows[i].FindControl(CB_CHOSEN) as CheckBox;
                HiddenField hfProduct = gvProducts.Rows[i].FindControl("hfProductId") as HiddenField;
                BuildAddItemRow(gvProducts.Rows[i], Convert.ToInt32(hfProduct.Value));
                var item = BuildCartItem(Convert.ToInt32(hfProduct.Value), userId, gvProducts.Rows[i]);

                if (cb != null) SetChosenProducts(item, cb.Checked);
            }
        }

        private void SetChosenProducts(Tuple<int, int, decimal, int> item, bool chosen)
        {
            if (chosen)
            {
                if (ExistInChosenToAddItems(item))
                {
                    SessionFacade.ChosenToAddItems.RemoveAll(delegate (Tuple<int, int, decimal, int> arg)
                    {
                        return arg.Item1 == item.Item1
                            && arg.Item2 == item.Item2
                            && arg.Item3 == item.Item3;
                    });
                }

                SessionFacade.ChosenToAddItems.Add(item);
                SessionFacade.NotChosenToAddItems.RemoveAll(delegate (Tuple<int, int, decimal, int> arg)
                {
                    return arg.Item1 == item.Item1
                            && arg.Item2 == item.Item2
                            && arg.Item3 == item.Item3;
                });
            }
            else
            {
                SessionFacade.ChosenToAddItems.RemoveAll(delegate (Tuple<int, int, decimal, int> arg)
                {
                    return arg.Item1 == item.Item1
                        && arg.Item2 == item.Item2
                        && arg.Item3 == item.Item3;
                });

                if (ExistInNotChosenToAddItems(item))
                {
                    SessionFacade.NotChosenToAddItems.RemoveAll(delegate (Tuple<int, int, decimal, int> arg)
                    {
                        return arg.Item1 == item.Item1
                            && arg.Item2 == item.Item2
                            && arg.Item3 == item.Item3;
                    });
                }

                SessionFacade.NotChosenToAddItems.Add(item);
            }
        }

        private bool ExistInChosenToAddItems(Tuple<int, int, decimal, int> item)
        {
            bool exist = false;
            for (int j = 0; j < SessionFacade.ChosenToAddItems.Count; j++)
            {
                exist = IsEqual(SessionFacade.ChosenToAddItems[j], item);

                if (exist) break;
            }
            return exist;
        }

        private bool ExistInNotChosenToAddItems(Tuple<int, int, decimal, int> item)
        {
            bool exist = false;
            for (int j = 0; j < SessionFacade.NotChosenToAddItems.Count; j++)
            {
                exist = IsEqual(SessionFacade.NotChosenToAddItems[j], item);

                if (exist) break;
            }
            return exist;
        }

        private bool IsEqual(Tuple<int, int, decimal, int> item1, Tuple<int, int, decimal, int> item2)
        {
            PropertyInfo[] properties1 = item1.GetType().GetProperties();
            PropertyInfo[] properties2 = item2.GetType().GetProperties();
            object value1;
            object value2;

            Type type = item1.GetType();
            bool isEqual = false;

            for (int i = 0; i < properties1.Length; i++)
            {
                value1 = properties1[i].GetValue(item1, null);
                value2 = properties2[i].GetValue(item2, null);

                if (value1 != null && value2 != null)
                    isEqual = value1.Equals(value2);

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        private void BuildAddItemRow(GridViewRow row, int productId, int loadSelectedProductPriceId = 0)
        {
            DropDownList ddlOptions = row.FindControl(DDL_OPTIONS) as DropDownList;
            Literal litSingleOption = row.FindControl(LIT_SINGLE_OPTION) as Literal;
            HiddenField hdnSingleOptionId = row.FindControl(HDN_SINGLE_OPTION_ID) as HiddenField;
            TextBox txtQty = row.FindControl(TXT_QTY) as TextBox;

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

        private Tuple<int, int, decimal, int> BuildCartItem(int productId, int userId, GridViewRow row)
        {
            int productPriceId = 0;
            decimal price = 0M;

            Product product = ProductService.GetProductById(productId);
            DropDownList ddlOptions = (DropDownList)row.FindControl(DDL_OPTIONS);
            HiddenField hdnSingleOptionId = (HiddenField)row.FindControl(HDN_SINGLE_OPTION_ID);

            int selectedProductPriceId = 0;

            if (product.ProductPrices.Count > 1)
                selectedProductPriceId = Convert.ToInt32(ddlOptions.SelectedValue);
            else if (product.ProductPrices.Count == 1)
                selectedProductPriceId = Convert.ToInt32(hdnSingleOptionId.Value);

            var selectedProductPrice = product.ProductPrices.Where(pp => pp.Id == selectedProductPriceId).FirstOrDefault();

            if (selectedProductPrice != null)
            {
                productPriceId = selectedProductPrice.Id;

                if (selectedProductPrice.OfferRuleId > 0)
                {
                    price = selectedProductPrice.OfferPriceExclTax;
                }
                else
                    price = selectedProductPrice.PriceExclTax;
            }

            TextBox txtQty = (TextBox)row.FindControl(TXT_QTY);
            int quantity = Convert.ToInt32(txtQty.Text.Trim());

            return new Tuple<int, int, decimal, int>(productId, productPriceId, price, quantity);
        }

        private string AddToCart(int profileId, int productId, int productPriceId, string currencyCode, int quantity)
        {
            // Delete existing cart item first
            CartService.DeleteCartItemsByProfileIdAndPriceId(profileId, productPriceId, freeItemIncluded: true);

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
    }
}