using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class promo_catalog_offer_info : BasePage
    {
        public IOfferService OfferService { get; set; }
        public IProductService ProductService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public OfferUtility OfferUtility { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }        
        public MediaSettings MediaSettings { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        protected override void OnInit(EventArgs e)
        {
            ddlAction.DataSource = OfferService.GetOfferActionAttributes(isCatalog: true, isCart: null);
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
            if (!IsPostBack)
            {
                LoadCatalogInfo();
                LoadProducts();
            }                
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

            gvRelatedProducts.DataSource = rule.RelatedItems;
            gvRelatedProducts.DataBind();
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
                var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

                rule.UrlRewrite = urlKey;
                rule.Name = txtRuleName.Text.Trim();
                rule.ProceedForNext = Convert.ToBoolean(rblProceed.SelectedValue);
                rule.IsActive = Convert.ToBoolean(rblStatus.SelectedValue);
                rule.Priority = Convert.ToInt32(txtPriority.Text.Trim());
                rule.Alias = txtRuleAlias.Text.Trim();
                rule.ShowOfferTag = chkShowOfferTag.Checked;
                rule.ShowRRP = chkShowRRP.Checked;
                rule.OfferUrl = txtViewOfferURL.Text.Trim();
                rule.PointSpendable = chkPointSpendable.Checked;
                rule.ShowCountDown = cbShowCountDownTimer.Checked;

                if (!string.IsNullOrEmpty(txtDateFrom.Text.Trim()))
                    rule.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                else
                    rule.StartDate = null;

                if (!string.IsNullOrEmpty(txtDateTo.Text.Trim()))
                    rule.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                else
                    rule.EndDate = null;
                
                rule.ShortDescription = txtShortDescription.Text;

                string htmlMsg;

                if (ftbLongDesc.Visible)
                    htmlMsg = AdminStoreUtility.CleanFtbOutput(ftbLongDesc.Text);
                else
                    htmlMsg = txtShortDescription.Text.Trim();

                rule.LongDescription = htmlMsg;
                rule.ShowInOfferPage = chkShowOnOfferPage.Checked;
                rule.DisplayOnHeaderStrip = chkDisplayOnHeaderStrip.Checked;
                rule.OfferLabel = txtOfferLabel.Text.Trim();
                rule.DisableOfferLabel = cbDisableOfferLabel.Checked;
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

                var newAction = new OfferAction
                {
                    Id = rule.Action.Id,
                    OfferRuleId = rule.Action.OfferRuleId,
                    DiscountAmount = Convert.ToDecimal(txtDiscountAmount.Text),
                    OfferActionAttributeId = Convert.ToInt32(ddlAction.SelectedValue),
                    Name = ddlAction.SelectedItem.Text,
                    IsCatalog = true
                };

                // Retrieve size target if any
                string optionOperatorId = null;
                string optionOperator = null;
                string optionOperand = null;

                if (txtOption.Text.Trim() != string.Empty)
                {
                    optionOperand = txtOption.Text.Trim();
                    optionOperatorId = ddlOptionOperator.SelectedValue;
                    optionOperator = OfferService.GetOfferOperator(Convert.ToInt32(optionOperatorId)).Operator;
                }

                if (!string.IsNullOrEmpty(optionOperatorId)) newAction.OptionOperatorId = Convert.ToInt32(optionOperatorId);
                if (!string.IsNullOrEmpty(optionOperand)) newAction.OptionOperand = Convert.ToString(optionOperand);                
                if ((!string.IsNullOrEmpty(optionOperator)) && (!string.IsNullOrEmpty(optionOperatorId)))
                    newAction.OptionOperator = new OfferOperator
                    {
                        Id = Convert.ToInt32(optionOperatorId),
                        Operator = Convert.ToString(optionOperator)
                    };
                
                rule.Action = newAction;

                var proceed = true;

                // Assign root condition to this rule object's condition
                rule.Condition = OfferUtility.OfferRuleConditions[rule.Id];
                
                if (rule.Condition == null)
                {
                    enbNotice.Message = "Offer was failed to update. There is no condition setup for this offer. It could be due to session lost. Please try to create again.";
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

                    LoadCatalogInfo();
                }
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
                case "preview":
                    var productId = Convert.ToInt32(e.CommandArgument);
                    var product = ProductService.GetProductById(productId);
                    if (product != null)
                    {
                        ltTestProductId.Text = string.Format("<a href='/catalog/product_info.aspx?productid={0}' target='_blank'>{0}</a>", product.Id);
                        ltTestProductName.Text = string.Format("<a href='/catalog/product_info.aspx?productid={0}' target='_blank'>{1}</a>", product.Id, product.Name);

                        product = OfferService.ProcessCatalog(product, QueryOfferRuleId);

                        rptTestProductPrices.DataSource = product.ProductPrices;
                        rptTestProductPrices.DataBind();

                        phPreviewProduct.Visible = true;
                    }
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "test";
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

        protected string GetOfferName(int offerRuleId)
        {
            if (offerRuleId == 0) return string.Empty;

            var offer = OfferService.GetOfferRuleOnlyById(offerRuleId);
            if (offer == null) return string.Empty;
            return string.Format("<a href='/marketing/promo_catalog_offer_info.aspx?offerruleid={1}' target='_blank'>Offer '{0}' (ID: {1})</a>", offer.Name, offer.Id);
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

        private void LoadCatalogInfo()
        {
            var rule = OfferService.GetOfferRuleById(QueryOfferRuleId);

            if (rule == null)
                Response.Redirect("/marketing/promo_catalog_offer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OfferNotFound);

            ltlTitle.Text = string.Format("Offer '{0}' (ID: {1})", rule.Name, rule.Id);

            txtRuleName.Text = rule.Name;
            txtRuleAlias.Text = rule.Alias;
            if (rule.IsActive)
                rblStatus.SelectedIndex = 0;
            else
                rblStatus.SelectedIndex = 1;

            txtDateFrom.Text = rule.StartDate.HasValue ? rule.StartDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;
            txtDateTo.Text = rule.EndDate.HasValue ? rule.EndDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;            
            txtPriority.Text = rule.Priority.ToString();
            chkPointSpendable.Checked = rule.PointSpendable;

            var foundOfferType = ddlOfferTypes.Items.FindByValue(rule.OfferTypeId.ToString());
            if (foundOfferType != null) foundOfferType.Selected = true;

            #region Conditions
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            OfferUtility.OfferRuleConditions[rule.Id] = rule.Condition;
            if (rule.Condition != null)
                OfferUtility.RenderCondition(rule.Condition, hw, OfferUtility.OfferRenderType.Catalog);
            else
            {
                OfferUtility.OfferRuleConditions[rule.Id] = new OfferCondition { IsAny = false, IsAll = true, Matched = true };
                OfferUtility.RenderCondition(OfferUtility.OfferRuleConditions[rule.Id], hw, OfferUtility.OfferRenderType.Catalog);
            }
            ltlConditions.Text = sb.ToString();
            #endregion

            ddlAction.Items.FindByValue(rule.Action.OfferActionAttributeId.ToString()).Selected = true;

            txtDiscountAmount.Text = rule.Action.DiscountAmount.Value.ToString();

            if (rule.Action.OptionOperator != null)
            {
                ddlOptionOperator.Items.FindByValue(rule.Action.OptionOperatorId.ToString()).Selected = true;
                txtOption.Text = rule.Action.OptionOperand;
            }
            
            rblProceed.SelectedIndex = rule.ProceedForNext ? 1 : 0;            
            chkShowOfferTag.Checked = rule.ShowOfferTag;
            chkShowRRP.Checked = rule.ShowRRP;
            txtUrlKey.Text = rule.UrlRewrite;
            txtShortDescription.Text = rule.ShortDescription;
            ltlSmallImage.Text = string.Format("<img src='/get_image_handler.aspx?" + QueryKey.TYPE + "=" + ImageHandlerType.OFFER + "&img={0}'/><br />", rule.SmallImage);
            ftbLongDesc.Text = rule.LongDescription;
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
    }
}