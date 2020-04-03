using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Media;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class promo_cart_offer_info : BasePage
    {
        #region Constants

        private const string PROMO_CART_OFFER_INFO_PAGE_URL_FORMAT = "/marketing/promo_cart_offer_info.aspx?offerruleid={0}&userid={1}";
        private const string DDL_OPTIONS = "ddlOptions";
        private const string LIT_SINGLE_OPTION = "litSingleOption";
        private const string HDN_SINGLE_OPTION_ID = "hdnSingleOptionId";
        private const string TXT_QTY = "txtQty";
        private const string DT_PRODUCT_PRICE_ID = "ProductPriceId";
        private const string DT_OPTION_NAME = "OptionName";

        private const string DT_OPTION_NAME_FORMAT_CURRENCY = "{0}{1:0.00} - {2}, {3} in stock";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {5} in stock";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY = "{0}{1:0.00} - {2}, {5} in stock, was {3}{4:0.00}";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {9} in stock, was {5}{6:0.00} ({7}{8:0.00})";

        #endregion

        public IAccountService AccountService { get; set; }
        public ICartService CartService { get; set; }
        public IProductService ProductService { get; set; }
        public IOfferService OfferService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IShippingService ShippingService { get; set; }
        public OfferUtility OfferUtility { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }        
        public MediaSettings MediaSettings { get; set; }
        public CurrencySettings CurrencySettings { get; set; }
        public ShippingSettings ShippingSettings { get; set; }

        protected override void OnInit(EventArgs e)
        {
            rblStatus.SelectedIndex = 1;
            rblProceed.SelectedIndex = 0;

            ddlAction.DataSource = OfferService.GetOfferActionAttributes(isCatalog: null, isCart: true);
            ddlAction.DataTextField = "Name";
            ddlAction.DataValueField = "Id";
            ddlAction.DataBind();
            
            ddlOptionOperator.DataSource = OfferService.GetOfferOperatorsByAttribute((int)OfferAttributeType.OPTION);
            ddlOptionOperator.DataTextField = "Operator";
            ddlOptionOperator.DataValueField = "Id";
            ddlOptionOperator.DataBind();

            var types = OfferService.GetOfferTypes().ToList();
            types.Insert(0, new OfferType { Type = AppConstant.DEFAULT_SELECT });
            ddlOfferTypes.DataTextField = "Type";
            ddlOfferTypes.DataValueField = "Id";
            ddlOfferTypes.DataSource = types;
            ddlOfferTypes.DataBind();

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int offerRuleId = QueryOfferRuleId;
            int profileId = QueryUserId;

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
                Response.Redirect(string.Format(PROMO_CART_OFFER_INFO_PAGE_URL_FORMAT, offerRuleId, profileId));
            }

            if (!IsPostBack)
            {
                LoadCartOfferInfo();
                LoadProducts();
            }                
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

            gvRelatedProducts.DataSource = rule.RelatedItems;
            gvRelatedProducts.DataBind();

            int profileId = QueryUserId;

            if (profileId > 0)
            {
                var items = CartService.GetCartItemsByProfileId(profileId, true);
                gvTempCart.DataSource = items;
                gvTempCart.DataBind();
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            string urlKey = txtUrlKey.Text.Trim();

            // If urlKey is empty, regenerate with given name
            if (urlKey == string.Empty)
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtRuleAlias.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            // Make sure urlKey is unique
            if (((OfferService.GetOfferRuleByUrlKey(urlKey) != null) && (OfferService.GetOfferRuleByUrlKey(urlKey).Id != QueryOfferRuleId)))
            {
                enbNotice.Message = "Offer was failed to save. URL key is not unique.";
            }
            else
            {
                string htmlMsg;

                if (ftbDesc.Visible)
                    htmlMsg = AdminStoreUtility.CleanFtbOutput(ftbDesc.Text);
                else
                    htmlMsg = txtDescription.Text.Trim();

                // Retrieve rule from database
                var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

                rule.Name = txtRuleName.Text.Trim();
                rule.ProceedForNext = Convert.ToBoolean(rblProceed.SelectedValue);
                rule.IsActive = Convert.ToBoolean(rblStatus.SelectedValue);
                rule.PromoCode = txtPromoCode.Text.Trim();
                rule.UsesPerCustomer = Convert.ToInt32(txtUsesPerCust.Text.Trim());
                rule.Priority = Convert.ToInt32(txtPriority.Text.Trim());
                rule.HtmlMessage = htmlMsg;
                rule.OfferedItemIncluded = cbOfferedItemIncluded.Checked;
                rule.Alias = txtRuleAlias.Text.Trim();
                rule.PointSpendable = chkPointSpendable.Checked;
                rule.UrlRewrite = urlKey;
                rule.UseInitialPrice = cbUseInitialPrice.Checked;
                rule.NewCustomerOnly = cbNewCustomerOnly.Checked;               
                rule.ShowCountDown = cbShowCountDownTimer.Checked;
                rule.DisplayOnProductPage = cbDisplayOnProductPage.Checked;
                rule.OfferTypeId = Convert.ToInt32(ddlOfferTypes.SelectedValue);

                if (ddlRelatedTypes.SelectedValue != string.Empty)
                {
                    switch (ddlRelatedTypes.SelectedValue)
                    {
                        case "products":
                            rule.RelatedProducts = txtRelatedItems.Text.ToString();
                            break;

                        case "brands":
                            rule.RelatedBrands = txtRelatedItems.Text.ToString();
                            break;

                        case "category":
                            rule.RelatedCategory = txtRelatedItems.Text.ToString();
                            break;
                    }
                }
                else
                {
                    rule.RelatedProducts = string.Empty;
                    rule.RelatedBrands = string.Empty;
                    rule.RelatedCategory = string.Empty;
                }

                if (!string.IsNullOrEmpty(txtDateFrom.Text.Trim()))
                    rule.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                else
                    rule.StartDate = null;

                if (!string.IsNullOrEmpty(txtDateTo.Text.Trim()))
                    rule.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                else
                    rule.EndDate = null;

                rule.ShortDescription = txtShortDescription.Text;

                string longDesc;

                if (!ftbLongDesc.Visible && txtLongDesc.Visible)
                {
                    if (txtLongDesc.Text != string.Empty)
                        longDesc = txtLongDesc.Text.Trim();
                    else
                        longDesc = txtShortDescription.Text.Trim();
                }
                else if (ftbLongDesc.Visible)
                    longDesc = AdminStoreUtility.CleanFtbOutput(ftbLongDesc.Text);
                else
                    longDesc = txtShortDescription.Text.Trim();

                rule.LongDescription = longDesc;                
                rule.ShowInOfferPage = chkShowOnOfferPage.Checked;
                rule.DisplayOnHeaderStrip = chkDisplayOnHeaderStrip.Checked;
                rule.OfferLabel = txtOfferLabel.Text.Trim();
                rule.DisableOfferLabel = cbDisableOfferLabel.Checked;
                rule.OfferUrl = txtViewOfferURL.Text.Trim();

                if (fuSmallImage.HasFile)
                {
                    string filename = rule.Id.ToString() + "_small" + Path.GetExtension(fuSmallImage.FileName).ToLower();
                    fuSmallImage.SaveAs(MediaSettings.OfferMediaLocalPath + filename);

                    rule.SmallImage = filename;
                }

                if (fuLargeImage.HasFile)
                {
                    string filename = rule.Id.ToString() + "_large" + Path.GetExtension(fuLargeImage.FileName).ToLower();
                    fuLargeImage.SaveAs(MediaSettings.OfferMediaLocalPath + filename);

                    rule.LargeImage = filename;
                }
                
                object freeProductItself = DBNull.Value;
                object freeProductId = DBNull.Value;
                object freeProductPriceId = DBNull.Value;
                object freeProductQty = DBNull.Value;

                // Determine if there is any free item settings
                if (rbFreeItself.Checked || rbFreeItem.Checked)
                {
                    freeProductItself = rbFreeItself.Checked;

                    if (rbFreeItem.Checked)
                    {
                        freeProductId = txtFreeProductId.Text.Trim();
                        freeProductPriceId = txtFreeProductPriceId.Text.Trim();
                        freeProductQty = txtFreeQuantity.Text.Trim();
                    }
                }

                object discountQtyStep = DBNull.Value;
                object minimumAmount = DBNull.Value;
                object discountAmount = DBNull.Value;
                int rewardPoint = 0;

                if (txtDiscountQtyStep.Text.Trim() != string.Empty)
                    discountQtyStep = txtDiscountQtyStep.Text.Trim();

                if (txtMinimumAmount.Text.Trim() != string.Empty)
                    minimumAmount = txtMinimumAmount.Text.Trim();

                if (txtDiscountAmount.Text.Trim() != string.Empty)
                    discountAmount = txtDiscountAmount.Text.Trim();

                if (txtRewardPoint.Text.Trim() != string.Empty)
                    rewardPoint = Convert.ToInt32(txtRewardPoint.Text.Trim());

                // Retrieve size target if any
                object optionOperatorId = DBNull.Value;
                object optionOperator = DBNull.Value;
                object optionOperand = DBNull.Value;

                if (txtOption.Text.Trim() != string.Empty)
                {
                    optionOperand = txtOption.Text.Trim();
                    optionOperatorId = ddlOptionOperator.SelectedValue;
                    optionOperator = OfferService.GetOfferOperator(Convert.ToInt32(optionOperatorId)).Operator;
                }

                int offerActionId = OfferService.GetOfferRuleById(QueryOfferRuleId).Action.Id;

                // Assign action to this rule object's action
                var newAction = new OfferAction
                {
                    Id = offerActionId,
                    OfferRuleId = QueryOfferRuleId
                };

                if (discountAmount != DBNull.Value) newAction.DiscountAmount = Convert.ToDecimal(discountAmount);
                if (freeProductItself != DBNull.Value) newAction.FreeProductItself = Convert.ToBoolean(freeProductItself);
                if (freeProductId != DBNull.Value) newAction.FreeProductId = Convert.ToInt32(freeProductId);
                if (freeProductPriceId != DBNull.Value) newAction.FreeProductPriceId = Convert.ToInt32(freeProductPriceId);
                if (freeProductQty != DBNull.Value) newAction.FreeProductQty = Convert.ToInt32(freeProductQty);
                newAction.OfferActionAttributeId = Convert.ToInt32(ddlAction.SelectedValue);
                newAction.Name = ddlAction.SelectedItem.Text;
                newAction.IsCart = true;
                if (discountQtyStep != DBNull.Value) newAction.DiscountQtyStep = Convert.ToInt32(discountQtyStep);
                if (minimumAmount != DBNull.Value) newAction.MinimumAmount = Convert.ToDecimal(minimumAmount);
                newAction.RewardPoint = rewardPoint;
                if (optionOperatorId != DBNull.Value) newAction.OptionOperatorId = Convert.ToInt32(optionOperatorId);
                if (optionOperand != DBNull.Value) newAction.OptionOperand = Convert.ToString(optionOperand);

                if ((optionOperator != DBNull.Value) && (optionOperatorId != DBNull.Value))
                    newAction.OptionOperator = new OfferOperator {
                        Id = Convert.ToInt32(optionOperatorId),
                        Operator = Convert.ToString(optionOperator)
                    };

                newAction.XValue = Convert.ToInt32(txtXValue.Text.Trim());
                newAction.YValue = Convert.ToDecimal(txtYValue.Text.Trim());

                rule.Action = newAction;

                var proceed = true;

                // Assign root condition to this rule object's condition
                rule.Condition = OfferUtility.OfferRuleConditions[rule.Id];
                
                if (rule.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to update. There is no condition setup for this offer. It could be due to session lost. Please try to create again.";
                    proceed = false;
                }

                // Assign root condition to this action object's condition
                rule.Action.Condition = OfferUtility.OfferActionConditions[rule.Id];

                if (rule.Action.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to update. There is no <u>action</u> condition setup for this offer. It could be due to session lost. Please try to create again.";
                    proceed = false;
                }

                if (proceed)
                {
                    // Update database
                    var success = OfferService.ProcessOfferUpdation(rule);

                    if (success)
                        enbNotice.Message = "Offer was updated successfully.";
                    else
                        enbNotice.Message = "Offer was failed to update.";
                }

                LoadCartOfferInfo();                
            }
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Offer | CacheEntityKey.Category | CacheEntityKey.Brand | CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All offers and products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected void lbClean_Click(object sender, EventArgs e)
        {
            OfferService.RemoveOfferedItemsFromBaskets(QueryOfferRuleId);
            enbNotice.Message = "All items which are related this offer were successfully to be removed from basket.";
        }
        
        #region Related products
        private void LoadProductsForRelatedProductSelector()
        {
            int[] productIds = null;
            var orderBy = ProductSortingType.IdAsc;
            bool enabled = true;

            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState("RelatedProduct-OrderBy")) orderBy = (ProductSortingType)GetIntState("RelatedProduct-OrderBy");

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvRelatedProductSelector.CustomPageIndex,
                pageSize: gvRelatedProductSelector.PageSize,               
                productIds: productIds,
                enabled: enabled,
                orderBy: orderBy);

            if (result != null)
            {
                gvRelatedProductSelector.DataSource = result.Items;
                gvRelatedProductSelector.RecordCount = result.TotalCount;
                gvRelatedProductSelector.CustomPageCount = result.TotalPages;
            }

            gvRelatedProductSelector.DataBind();
        }
        protected void gvRelatedProductSelector_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "select":

                    var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);
                    var product = ProductService.GetProductById(Convert.ToInt32(e.CommandArgument.ToString()));

                    if (product != null && rule != null)
                    {
                        var relatedItem = new OfferRelatedItem
                        {
                            OfferRuleId = rule.Id,
                            ProductId = product.Id,
                            Enabled = true,
                            Priority = 0
                        };

                        OfferService.InsertOfferRelatedItem(relatedItem);
                        LoadProductsForRelatedProductSelector();
                    }

                    break;
            }

            hfCurrentPanel.Value = "relatedProducts";
        }
        protected void gvRelatedProductSelector_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRelatedProductSelector.CustomPageIndex = gvRelatedProductSelector.CustomPageIndex + e.NewPageIndex;

            if (gvRelatedProductSelector.CustomPageIndex < 0)
                gvRelatedProductSelector.CustomPageIndex = 0;

            LoadProductsForRelatedProductSelector();
        }        
        protected void gvRelatedProductSelector_Sorting(object sender, GridViewSortEventArgs e)
        {
            ProductSortingType orderBy = ProductSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                default:
                    orderBy = ProductSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.IdAsc;
                    break;
            }

            SetState("RelatedProduct-OrderBy", (int)orderBy);            
            LoadProductsForRelatedProductSelector();
        }
        protected void gvRelatedProductSelector_PreRender(object sender, EventArgs e)
        {
            if (gvRelatedProductSelector.TopPagerRow != null)
            {
                gvRelatedProductSelector.TopPagerRow.Visible = true;

                ((TextBox)gvRelatedProductSelector.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
            }
        }
        protected void gvRelatedProductSelector_lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvRelatedProductSelector.HeaderRow.FindControl("txtFilterId")).Text.Trim());

            LoadProductsForRelatedProductSelector();

            hfCurrentPanel.Value = "relatedProducts";
        }
        protected void gvRelatedProductSelector_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvRelatedProductSelector.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvRelatedProductSelector.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvRelatedProductSelector.CustomPageIndex = gotoIndex;

            LoadProductsForRelatedProductSelector();

            hfCurrentPanel.Value = "relatedProducts";
        }
        protected void gvRelatedProductSelector_lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            SetState(CHOSEN_FILTER, (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY ? ANY : YES));

            LoadProductsForRelatedProductSelector();

            hfCurrentPanel.Value = "relatedProducts";
        }
        protected void gvRelatedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "deleteRelated":
                    OfferService.DeleteOfferRelatedItem(Convert.ToInt32(e.CommandArgument.ToString()));
                    LoadProductsForRelatedProductSelector();
                    break;
                case "updateRelated":
                    var relatedItems = OfferService.GetOfferRuleById(QueryOfferRuleId).RelatedItems;
                    var relatedItem = relatedItems.Where(x => x.Id == Convert.ToInt32(e.CommandArgument)).FirstOrDefault();
                    
                    if (relatedItem != null)
                    {
                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        relatedItem.Priority = Convert.ToInt32(((TextBox)row.FindControl("txtPriority")).Text);
                        relatedItem.Enabled = ((CheckBox)row.FindControl("chkEnabled")).Checked;

                        OfferService.UpdateOfferRelatedItem(relatedItem);
                        LoadProductsForRelatedProductSelector();
                    }
                    break;
            }

            hfCurrentPanel.Value = "relatedProducts";
        }
        #endregion

        protected void gvTempCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row;
            int profileId = QueryUserId;
            int offerRuleId = QueryOfferRuleId;
            int productPriceId;
            Country country;

            switch (e.CommandName)
            {
                case "updateItem":
                    row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    productPriceId = Convert.ToInt32(e.CommandArgument);

                    TextBox txtQty = (TextBox)row.FindControl("txtQty");
                    int quantity = Convert.ToInt32(txtQty.Text.Trim());

                    CartService.UpdateLineQuantityByProfileIdAndProductPriceId(profileId, productPriceId, quantity);

                    country = ShippingService.GetCountryById(ShippingSettings.PrimaryStoreCountryId);

                    OfferService.ProcessCartOfferByProfileId(profileId, country.ISO3166Code, offerRuleId);

                    LoadCart(profileId);
                    break;

                case "removeItem":
                    productPriceId = Convert.ToInt32(e.CommandArgument);                    
                    CartService.DeleteCartItemsByProfileIdAndPriceId(profileId, productPriceId, freeItemIncluded: true);
                    country = ShippingService.GetCountryById(ShippingSettings.PrimaryStoreCountryId);
                    OfferService.ProcessCartOfferByProfileId(profileId, country.ISO3166Code, offerRuleId);

                    LoadCart(profileId);
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "test";
        }

        protected void gvTestProducts_PreRender(object sender, EventArgs e)
        {
            if (gvTestProducts.TopPagerRow != null)
            {
                gvTestProducts.TopPagerRow.Visible = true;
                ((TextBox)gvTestProducts.HeaderRow.FindControl("txtTestProductId")).Text = GetStringState("testproductidfilter");
                ((TextBox)gvTestProducts.HeaderRow.FindControl("txtTestProductName")).Text = GetStringState("testproductnamefilter");
            }
        }

        protected void gvTestProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTestProducts.CustomPageIndex = gvTestProducts.CustomPageIndex + e.NewPageIndex;

            if (gvTestProducts.CustomPageIndex < 0)
                gvTestProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvTestProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "addToBasket":
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

                            message = AddToCart(profileId, productId, productPriceId, CurrencySettings.PrimaryStoreCurrencyCode, qtyToAdd);
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

                    enbTempCart.Message = message;
                    LoadCart(profileId);
                    
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "test";
        }

        protected void gvTestProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var product = (ProductOverviewModel)e.Row.DataItem;
                if (product.Id > 0)
                {
                    DropDownList ddlOptions = e.Row.FindControl(DDL_OPTIONS) as DropDownList;
                    TextBox txtQty = e.Row.FindControl(TXT_QTY) as TextBox;

                    BuildAddItemRow(e.Row, product.Id);
                    
                    var productPrices = ProductService.GetProductPricesByProductId(product.Id);

                    // If product doesn't have any options, hide it
                    if (productPrices.Count == 0)
                    {
                        var lbAddToBasket = e.Row.FindControl("lbAddToBasket") as LinkButton;

                        lbAddToBasket.Visible = false;                        
                        txtQty.Visible = false;
                    }
                }
            }
        }

        protected void lbTestProductSearch_Click(object sender, EventArgs e)
        {
            SetState("testproductidfilter", ((TextBox)gvTestProducts.HeaderRow.FindControl("txtTestProductId")).Text.Trim());
            SetState("testproductnamefilter", ((TextBox)gvTestProducts.HeaderRow.FindControl("txtTestProductName")).Text.Trim());
            LoadProducts();

            hfCurrentPanel.Value = "test";
        }

        protected void lbTestProductReset_Click(object sender, EventArgs e)
        {
            DisposeState("testproductidfilter");
            DisposeState("testproductnamefilter");

            LoadProducts();

            hfCurrentPanel.Value = "test";
        }

        protected void lbApplyTestPromoCode_Click(object sender, EventArgs e)
        {
            var profileId = QueryUserId;
            var promoCode = txtTestPromoCode.Text.Trim();

            UtilityService.SaveAttribute(profileId,
                "Profile",
                SystemCustomerAttributeNames.DiscountCouponCode,
                promoCode);
            
            int offerRuleId = QueryOfferRuleId;
            var country = ShippingService.GetCountryById(ShippingSettings.PrimaryStoreCountryId);
            OfferService.ProcessCartOfferByProfileId(profileId, country.ISO3166Code, offerRuleId);

            LoadCart(profileId);

            hfCurrentPanel.Value = "test";

            enbTempCart.Message = "Promo code was applied successfully.";
        }

        protected string GetOfferName(int offerRuleId)
        {
            if (offerRuleId == 0) return string.Empty;

            var offer = OfferService.GetOfferRuleOnlyById(offerRuleId);
            if (offer == null) return string.Empty;

            if (offer.IsCart)
                return string.Format("<a href='/marketing/promo_cart_offer_info.aspx?offerruleid={1}' target='_blank'>Offer '{0}' (ID: {1})</a>", offer.Name, offer.Id);
            else
                return string.Format("<a href='/marketing/promo_catalog_offer_info.aspx?offerruleid={1}' target='_blank'>Offer '{0}' (ID: {1})</a>", offer.Name, offer.Id);
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

            var currencyCode = CurrencySettings.PrimaryStoreCurrencyCode;
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
                optionName = string.Format(DT_OPTION_NAME_FORMAT_CURRENCY,
                                            currencyCode,
                                            priceOption.PriceInclTax,
                                            option,
                                            priceOption.Stock);                
            }

            return optionName;
        }

        private void LoadCartOfferInfo()
        {
            var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

            if (rule == null)
                Response.Redirect("/marketing/promo_cart_offer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OfferNotFound);

            ltlTitle.Text = string.Format("Offer '{0}' (ID: {1})", rule.Name, rule.Id);

            txtRuleName.Text = rule.Name;
            txtRuleAlias.Text = rule.Alias;
            if (IsFireFoxBrowser)
            {
                ftbDesc.Visible = false;
                txtDescription.Text = rule.HtmlMessage;

                ftbLongDesc.Visible = false;
                txtLongDesc.Text = rule.LongDescription;
            }
            else
            {
                txtDescription.Visible = false;
                ftbDesc.Text = rule.HtmlMessage;

                txtLongDesc.Visible = false;
                ftbLongDesc.Text = rule.LongDescription;
            }

            if (IsFireFoxBrowser)
            {
                ftbDesc.Visible = false;
                txtDescription.Text = rule.HtmlMessage;
            }
            else
            {
                txtDescription.Visible = false;
                ftbDesc.Text = rule.HtmlMessage;
            }
            if (rule.IsActive)
                rblStatus.SelectedIndex = 0;
            else
                rblStatus.SelectedIndex = 1;
            
            txtDateFrom.Text = rule.StartDate.HasValue ? rule.StartDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;
            txtDateTo.Text = rule.EndDate.HasValue ? rule.EndDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;

            txtPromoCode.Text = rule.PromoCode;
            txtTestPromoCode.Text = rule.PromoCode;
            txtUsesPerCust.Text = rule.UsesPerCustomer.ToString();
            cbOfferedItemIncluded.Checked = rule.OfferedItemIncluded;
            txtPriority.Text = rule.Priority.ToString();
            chkPointSpendable.Checked = rule.PointSpendable;
            cbUseInitialPrice.Checked = rule.UseInitialPrice;
            cbNewCustomerOnly.Checked = rule.NewCustomerOnly;
            var foundOfferType = ddlOfferTypes.Items.FindByValue(rule.OfferTypeId.ToString());
            if (foundOfferType != null) foundOfferType.Selected = true;

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            OfferUtility.OfferRuleConditions[rule.Id] = rule.Condition;
            if (rule.Condition != null)
                OfferUtility.RenderCondition(rule.Condition, hw, OfferUtility.OfferRenderType.Cart);
            else
            {
                OfferUtility.OfferRuleConditions[rule.Id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
                OfferUtility.RenderCondition(OfferUtility.OfferRuleConditions[rule.Id], hw, OfferUtility.OfferRenderType.Cart);
            }

            ltlConditions.Text = sb.ToString();

            ddlAction.Items.FindByValue(rule.Action.OfferActionAttributeId.ToString()).Selected = true;

            if (rule.Action.DiscountAmount.HasValue)
                txtDiscountAmount.Text = rule.Action.DiscountAmount.Value.ToString();
            else
                txtDiscountAmount.Text = string.Empty;

            if (rule.Action.OptionOperator != null)
            {
                ddlOptionOperator.Items.FindByValue(rule.Action.OptionOperatorId.ToString()).Selected = true;
                txtOption.Text = rule.Action.OptionOperand;
            }

            if (rule.ProceedForNext)
                rblProceed.SelectedIndex = 1;
            else
                rblProceed.SelectedIndex = 0;

            if (rule.Action.FreeProductItself.HasValue || rule.Action.FreeProductId.HasValue)
            {
                if (rule.Action.FreeProductItself.HasValue && rule.Action.FreeProductItself.Value)
                    rbFreeItself.Checked = true;
                else
                {
                    rbFreeItem.Checked = true;
                    txtFreeProductId.Text = rule.Action.FreeProductId.Value.ToString();
                    txtFreeProductPriceId.Text = rule.Action.FreeProductPriceId.Value.ToString();
                    txtFreeQuantity.Text = rule.Action.FreeProductQty.Value.ToString();
                }
            }

            txtXValue.Text = rule.Action.XValue.Value.ToString();
            txtYValue.Text = rule.Action.YValue.Value.ToString();

            if (rule.Action.DiscountQtyStep.HasValue)
                txtDiscountQtyStep.Text = rule.Action.DiscountQtyStep.Value.ToString();

            if (rule.Action.MinimumAmount.HasValue)
                txtMinimumAmount.Text = rule.Action.MinimumAmount.Value.ToString();

            txtRewardPoint.Text = rule.Action.RewardPoint.ToString();

            StringBuilder sb2 = new StringBuilder();
            StringWriter tw2 = new StringWriter(sb2);
            HtmlTextWriter hw2 = new HtmlTextWriter(tw2);

            OfferUtility.OfferActionConditions[rule.Id] = rule.Action.Condition;
            if (rule.Action.Condition != null)
            {
                OfferUtility.RenderCondition(rule.Action.Condition, hw2, OfferUtility.OfferRenderType.Action);                
            }
            else
            {
                OfferUtility.OfferActionConditions[rule.Id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
                OfferUtility.RenderCondition(OfferUtility.OfferActionConditions[rule.Id], hw2, OfferUtility.OfferRenderType.Action);
            }

            ltlActionCondition.Text = sb2.ToString();

            txtUrlKey.Text = rule.UrlRewrite;
            txtShortDescription.Text = rule.ShortDescription;
            ltlSmallImage.Text = string.Format("<img src='/get_image_handler.aspx?" + QueryKey.TYPE + "=" + ImageHandlerType.OFFER + "&img={0}'/><br />", rule.SmallImage);
            //ftbLongDesc.Text = rule.LongDescription;
            ltlLargeImage.Text = string.Format("<img src='/get_image_handler.aspx?" + QueryKey.TYPE + "=" + ImageHandlerType.OFFER + "&img={0}'/><br />", rule.LargeImage);
            
            chkShowOnOfferPage.Checked = rule.ShowInOfferPage;
            chkDisplayOnHeaderStrip.Checked = rule.DisplayOnHeaderStrip;
            txtViewOfferURL.Text = rule.OfferUrl;
            cbShowCountDownTimer.Checked = rule.ShowCountDown;
            cbDisplayOnProductPage.Checked = rule.DisplayOnProductPage;
            cbDisableOfferLabel.Checked = rule.DisableOfferLabel;
            txtOfferLabel.Text = rule.OfferLabel;

            if (rule.RelatedBrands != string.Empty)
            {
                ddlRelatedTypes.Items.FindByValue("brands").Selected = true;
                txtRelatedItems.Text = rule.RelatedBrands;
            }
            else if (rule.RelatedProducts != string.Empty)
            {
                ddlRelatedTypes.Items.FindByValue("products").Selected = true;
                txtRelatedItems.Text = rule.RelatedProducts;
            }
            else if (rule.RelatedCategory != string.Empty)
            {
                ddlRelatedTypes.Items.FindByValue("category").Selected = true;
                txtRelatedItems.Text = rule.RelatedCategory;
            }
            
            LoadProductsForRelatedProductSelector();
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState("OrderBy")) orderBy = (ProductSortingType)GetIntState("OrderBy");
            if (HasState("testproductidfilter"))
            {
                string value = GetStringState("testproductidfilter");
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState("testproductnamefilter")) name = GetStringState("testproductnamefilter");

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvTestProducts.CustomPageIndex,
                pageSize: gvTestProducts.PageSize,
                productIds: productIds,
                keywords: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvTestProducts.DataSource = result.Items;
                gvTestProducts.RecordCount = result.TotalCount;
                gvTestProducts.CustomPageCount = result.TotalPages;
            }

            gvTestProducts.DataBind();

            if (gvTestProducts.Rows.Count <= 0) enbTestProducts.Message = "No records found.";
        }

        private void LoadCart(int userId)
        {
            var items = CartService.GetCartItemsByProfileId(userId, true);

            gvTempCart.DataSource = items;
            gvTempCart.DataBind();

            LoadOrderSummary();
        }

        private string AddToCart(int profileId, int productId, int productPriceId, string currencyCode, int quantity)
        {
            // Delete existing cart item first
            CartService.DeleteCartItemsByProfileIdAndPriceId(profileId, productPriceId, true);

            var profile = AccountService.GetProfileById(profileId);
            var countryId = profile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.CountryId, UtilityService);
            var country = ShippingService.GetCountryById(countryId);

            var message = CartService.ProcessItemAddition(
                profileId,
                productId,
                productPriceId,
                country.ISO3166Code,
                quantity,                
                QueryOfferRuleId);

            return message;
        }

        private void LoadOrderSummary()
        {
            int profileId = QueryUserId;
            int offerRuleId = QueryOfferRuleId;
            var orderTotals = CartService.CalculateOrderTotals(profileId, true, offerRuleId);

            var profile = AccountService.GetProfileById(profileId);
            var promoCode = profile.GetAttribute<string>("Profile", SystemCustomerAttributeNames.DiscountCouponCode, UtilityService);

            litAppliedPromocode.Text = promoCode;
            litTotalsDiscount.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.Discount, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
            litTotalsSubTot.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.Subtotal, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
            litTotalsTotal.Text = AdminStoreUtility.GetFormattedPrice(orderTotals.Total, orderTotals.CurrencyCode, CurrencyType.HtmlEntity);
        }        
    }
}