using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class product_new : BasePage
    {
        public IBrandService BrandService { get; set; }
        public ICategoryService CategoryService { get; set; }
        public IProductService ProductService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        private const string SESSION_BRAND_ID = "brand_id";
        private const string ROOT_BRAND_CAT = "Root";
        private const string ROOT_CAT = "Root Category";

        protected override void OnInit(EventArgs e)
        {
            #region Brand

            ddlBrand.DataTextField = "Name";
            ddlBrand.DataValueField = "Id";
            ddlBrand.DataSource = BrandService.GetBrandList();
            ddlBrand.DataBind();

            for (int i = 0; i < ddlBrand.Items.Count; i++)
                ddlBrand.Items[i].Text = Server.HtmlDecode(ddlBrand.Items[i].Text);

            #endregion

            #region Delivery 

            ddlDelivery.DataTextField = "TimeLine";
            ddlDelivery.DataValueField = "Id";
            ddlDelivery.DataSource = ShippingService.GetDeliveryList();
            ddlDelivery.DataBind();

            #endregion

            #region Option Type

            ddlOptionType.Items.AddRange(OptionType.None.ToListItemArray());

            #endregion

            #region Product Mark

            ddlProductMarks.Items.AddRange(ProductMarkType.None.ToListItemArray());

            #endregion

            #region Tax Category

            ddlTaxCategory.DataTextField = "Name";
            ddlTaxCategory.DataValueField = "Id";
            ddlTaxCategory.DataSource = ProductService.GetTaxCategories();
            ddlTaxCategory.DataBind();

            #endregion

            base.OnInit(e);
        }

        protected void lbEditCategory_Click(object sender, EventArgs e)
        {
            mvCategory.ActiveViewIndex = 1;

            if (hfCategory.Value != 0.ToString())
            {
                int categoryId = Convert.ToInt32(hfCategory.Value);
                var treeList = CategoryService.GetTreeList(categoryId);
                ectCategory.FindSelectedNode(0, treeList);
            }
        }

        protected void ectCategory_TreeNodeSelected(string categoryName, int categoryId)
        {
            if (categoryName != ROOT_CAT)
                ltlCategory.Text = categoryName;

            hfCategory.Value = categoryId.ToString();
            ectCategory.Clear();
            mvCategory.ActiveViewIndex = 0;
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            string urlKey = txtUrlKey.Text.Trim();

            // If urlKey is empty, regenerate with given name
            if (urlKey == string.Empty)
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtName.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            // Make sure urlKey is unique
            var foundProduct = ProductService.GetProductByUrlKey(urlKey);
            int productId = QueryProductId;
            string message = string.Empty;

            if (((foundProduct != null) && (foundProduct.Id != productId)))
            {
                ErrorSummary.AddError("Url Key is not unique.", "vgNewProduct", this.Page);                
            }
            else
            {
                Product product = new Product();
                product.Name = Server.HtmlEncode(txtName.Text.Trim());
                product.Description = string.Empty;
                product.BrandId = Convert.ToInt32(ddlBrand.SelectedValue);
                product.DeliveryId = Convert.ToInt32(ddlDelivery.SelectedValue);
                product.UrlRewrite = urlKey;
                product.Enabled = ddlStatus.Items.FindByValue(product.Enabled ? ENABLED : DISABLED).Selected = true;
                product.IsPharmaceutical = cbIsPharm.Checked;
                product.OpenForOffer = cbOpenForOffer.Checked;
                product.Discontinued = cbDiscontinued.Checked;
                product.ShowPreOrderButton = cbDisplayPreOrder.Checked;
                product.EnforceStockCount = cbEnforceStockCount.Checked;
                product.IsGoogleProductSearchDisabled = cbGoogleProductSearchDisabled.Checked;
                product.ProductCode = txtProductCode.Text.Trim();
                product.HasFreeWrapping = cbFreeWrap.Checked;
                product.OptionType = Convert.ToInt32(ddlOptionType.SelectedValue);
                product.ProductMark = txtProductMark.Text.Trim();
                product.ProductMarkType = Convert.ToInt32(ddlProductMarks.SelectedValue);

                if (!string.IsNullOrEmpty(txtProductMarkExpiryDate.Text.Trim()))
                    product.ProductMarkExpiryDate = DateTime.ParseExact(txtProductMarkExpiryDate.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);

                product.IsPhoneOrder = cbIsPhoneOrder.Checked;
                product.VisibleIndividually = cbVisibleIndividually.Checked;

                int taxCategoryId = Convert.ToInt32(ddlTaxCategory.SelectedValue);
                var taxCategory = ProductService.GetTaxCategory(taxCategoryId);
                product.TaxCategory = taxCategory;

                if (txtStepQuantity.Text.Trim() != string.Empty)
                {
                    int defaultQty = 1;
                    if (!int.TryParse(txtStepQuantity.Text.Trim(), out defaultQty))
                        defaultQty = 1;

                    if (defaultQty <= 0)
                        defaultQty = 1;

                    product.StepQuantity = Convert.ToInt32(txtStepQuantity.Text.Trim());

                    if (cbIsPharm.Checked) product.StepQuantity = 1;
                }
                
                product.Id = ProductService.InsertProduct(product);

                var categoryId = Convert.ToInt32(hfCategory.Value);
                if (categoryId != AppConstant.DEFAULT_CATEGORY)
                {
                    CategoryService.ProcessCategoryAssignmentForProduct(categoryId, product.Id);
                }

                Response.Redirect("/catalog/product_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductCreated + "&productid=" + product.Id);
            }
        }

        protected void ectBrandCategory_TreeChanged(string brandCategoryName, int brandCategoryId)
        {
            SetState(BRAND_CATEGORY_NAME, brandCategoryName);
            SetState(LAST_CHOSEN_BRAND_CATEGORY, brandCategoryId);
        }
    }
}