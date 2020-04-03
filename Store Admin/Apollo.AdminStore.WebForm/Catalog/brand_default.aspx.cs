using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class brand_default : BasePage
    {
        private int VIEW_BRAND = 0;
        private int VIEW_PRODUCT = 1;

        public IBrandService BrandService { get; set; }
        public IProductService ProductService { get; set; }
        public IShippingService ShippingService { get; set; }        
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected override void OnInit(EventArgs e)
        {
            var featuredTypeValues = Enum.GetValues(typeof(FeaturedItemType));
            var featuredTypeNames = Enum.GetNames(typeof(FeaturedItemType));
            var k = 0;

            foreach (var item in featuredTypeValues)
            {
                // We want to exclude Position.
                if ((int)item != ((int)FeaturedItemType.Position))
                {
                    ListItem li = new ListItem(featuredTypeNames[k], ((int)item).ToString());
                    ddlFeaturedItemType.Items.Add(li);

                    ListItem fi = new ListItem(featuredTypeNames[k], ((int)item).ToString());
                    ddlFeaturedItemTypeFilter.Items.Add(fi);
                }

                k++;
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (GetIntState(BRAND_ID) == AppConstant.DEFAULT_BRAND)
            {
                phTabs.Visible = false;
            }
            else
            {
                phTabs.Visible = true;
            }

            base.OnPreRenderComplete(e);
        }

        protected void gvBrands_Init(object sender, EventArgs e)
        {
            LoadBrands();
        }

        protected void gvBrands_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBrands.CustomPageIndex = gvBrands.CustomPageIndex + e.NewPageIndex;

            if (gvBrands.CustomPageIndex < 0)
                gvBrands.CustomPageIndex = 0;

            LoadBrands();
        }

        protected void gvBrands_Sorting(object sender, GridViewSortEventArgs e)
        {
            BrandSortingType orderBy = BrandSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                default:
                    orderBy = BrandSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BrandSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = BrandSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BrandSortingType.NameAsc;
                    break;
            }

            SetState("Brand-OrderBy", (int)orderBy);
            LoadBrands();
            SetState(DISPLAY, VIEW_BRAND);
        }

        protected void gvBrands_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.background='#eeff00'");
                e.Row.Attributes.Add("onmouseout", "this.style.background='#ffffff'");
            }
        }

        protected void gvBrands_PreRender(object sender, EventArgs e)
        {
            var brandId = GetIntState(BRAND_ID);
            PopulateBrandInfo(brandId);

            if (GetIntState(BRAND_ID) == -1)
            {
                lbDeleteBrand.Visible = false;
                ltlTitle.Text = "New Brand";
                //hfCurrentPanel.Value = "general";
            }
            else
            {
                var brand = BrandService.GetBrandById(brandId);
                lbDeleteBrand.Visible = true;
                ltlTitle.Text = string.Format("{0} (ID: {1}) {2} {3}",
                    brand.Name,
                    brand.Id,
                    string.Format("<a href='{0}' target='_blank'><i class='fa fa-external-link' aria-hidden='true'></i></a>", AdminStoreUtility.GetBrandUrl(brand.UrlRewrite)),
                    brand.Enabled ? null : "<i class='fa fa-eye-slash' aria-hidden='true' title='offline'></i>");
            }

            if (GetIntState(DISPLAY) == VIEW_PRODUCT)
                hfCurrentPanel.Value = "products";

            if (gvBrands.TopPagerRow != null)
            {
                gvBrands.TopPagerRow.Visible = true;
                ((TextBox)gvBrands.HeaderRow.FindControl("txtBrandFilterId")).Text = GetStringState(BRAND_ID_FILTER);
                ((TextBox)gvBrands.HeaderRow.FindControl("txtBrandFilterName")).Text = GetStringState(BRAND_NAME_FILTER);
            }
        }

        protected void gvBrands_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case EDIT_INFO:
                    ResetScreen();
                    PopulateBrandInfo(Convert.ToInt32(e.CommandArgument));
                    break;
            }
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            LoadProducts();
            hfCurrentPanel.Value = "products";
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var product = e.Row.DataItem as ProductOverviewModel;
                var cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (ChosenProducts.Contains(product.Id))
                    cb.Checked = true;

                SetChosenProducts(product.Id, cb.Checked);
            }
        }

        protected void gvProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ProductSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                default:
                    orderBy = ProductSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.IdAsc;
                    break;
                case "Name":                
                    orderBy = ProductSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.NameAsc;
                    break;
            }
            
            SetState("Product-OrderBy", (int)orderBy);
            LoadProducts();
            SetState(DISPLAY, VIEW_PRODUCT);
        }

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
            {
                gvProducts.TopPagerRow.Visible = true;
                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductFilterName")).Text = GetStringState(PRODUCT_NAME_FILTER);
                ((DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen")).Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
            }
        }

        protected void lbSearchBrand_Click(object sender, EventArgs e)
        {
            SetState(BRAND_ID_FILTER, ((TextBox)gvBrands.HeaderRow.FindControl("txtBrandFilterId")).Text.Trim());
            SetState(BRAND_NAME_FILTER, ((TextBox)gvBrands.HeaderRow.FindControl("txtBrandFilterName")).Text.Trim());

            gvBrands.CustomPageIndex = 0;

            LoadBrands();
        }

        protected void lbSearchProduct_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtProductFilterId")).Text.Trim());
            SetState(PRODUCT_NAME_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtProductFilterName")).Text.Trim());
            SetState(CHOSEN_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen")).SelectedValue);

            LoadProducts();

            hfCurrentPanel.Value = "products";
        }

        protected void btnBrandGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvBrands.TopPagerRow.FindControl("txtPageIndexBrand")).Text.Trim()) - 1;

            if ((gvBrands.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvBrands.CustomPageIndex = gotoIndex;

            LoadBrands();
        }

        protected void btnProductGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndexProduct")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            LoadProducts();
            hfCurrentPanel.Value = "products";
        }

        protected void lbResetBrandFilter_Click(object sender, EventArgs e)
        {
            DisposeState(BRAND_ID_FILTER);
            DisposeState(BRAND_NAME_FILTER); ;

            gvBrands.CustomPageIndex = 0;

            LoadBrands();
        }

        protected void lbResetProductFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(PRODUCT_NAME_FILTER);
            SetState(CHOSEN_FILTER, (GetIntState(BRAND_ID) == AppConstant.DEFAULT_BRAND ? ANY : YES));

            LoadProducts();
            hfCurrentPanel.Value = "products";
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
            ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
            if (GetIntState(BRAND_ID) == AppConstant.DEFAULT_BRAND) ddl.Enabled = false;
        }

        protected void lbSaveBrand_Click(object sender, EventArgs e)
        {
            string urlKey = txtUrlKey.Text.Trim();
            txtUrlKey.BackColor = Color.Empty;

            // If urlKey is empty, regenerate with given category name
            if (urlKey == string.Empty)
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtName.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            // Make sure urlKey is unique
            if (((GetStringState(MODE) == NEW) && (BrandService.GetBrandByUrlKey(urlKey) != null)) ||
            ((GetStringState(MODE) == EDIT) && (BrandService.GetBrandByUrlKey(urlKey) != null) && (BrandService.GetBrandByUrlKey(urlKey).Id != GetIntState(BRAND_ID))))
            {
                enbNotice.Message = "Url Key is not unique.";
            }
            else
            {
                SaveLastViewedProduct();

                Brand brand = new Brand();

                brand.Name = Server.HtmlEncode(txtName.Text.Trim());
                brand.FlashImageHeight = -1;
                brand.FlashImageWidth = -1;
                brand.UrlRewrite = urlKey;

                string logoFilename = string.Empty;
                //added string empty to avoid the exception
                brand.FlashImage = string.Empty;
                if (fuLogo.HasFile)
                {
                    logoFilename = brand.UrlRewrite.Trim().ToLower() + "-logo" + Path.GetExtension(fuLogo.FileName).ToLower();
                    fuLogo.SaveAs(MediaSettings.BrandMediaLocalPath + logoFilename);
                    brand.FlashImage = logoFilename;
                }

                // Get old logo filename
                if (!fuLogo.HasFile && imgLogo.Alt != string.Empty)
                    brand.FlashImage = imgLogo.Alt;

                // Remove logo file
                if (cbRemoveLogo.Checked && imgLogo.Alt != string.Empty)
                {
                    File.Delete(MediaSettings.BrandMediaLocalPath + imgLogo.Alt);
                    brand.FlashImage = string.Empty;
                    cbRemoveLogo.Checked = false;
                }

                brand.Description = txtDesc.Text.Trim();
                brand.HasMicrosite = cbMicrosite.Checked;
                brand.MetaDescription = txtMetaDesc.Text.Trim();
                brand.MetaTitle = txtMetaTitle.Text.Trim();
                brand.MetaKeywords = txtMetaKeywords.Text.Trim();
                brand.EnforceStockCount = cbEnforceStockCount.Checked;
                brand.DeliveryId = Convert.ToInt32(ddlDelivery.SelectedValue);
                brand.Enabled = cbEnabled.Checked;

                if (GetStringState(MODE) == NEW)
                {                    
                    int brandId = BrandService.InsertBrand(brand);
                    brand.Id = brandId;
                    SetState(BRAND_ID, brand.Id);

                    enbNotice.Message = "Brand was created successfully.";
                }
                else // Edit mode
                {
                    brand.Id = GetIntState(BRAND_ID);                    
                    BrandService.UpdateBrand(brand);
                    
                    if (NotChosenProducts.Count > 0)
                        BrandService.UpdateProductWithBrand(AppConstant.DEFAULT_BRAND, NotChosenProducts);

                    enbNotice.Message = "Brand was updated successfully.";
                }
                
                if (ChosenProducts.Count > 0)
                    BrandService.UpdateProductWithBrand(brand.Id, ChosenProducts);                
            }
            
            ChosenProducts.Clear();
            NotChosenProducts.Clear();
            LoadProducts();

            hfCurrentPanel.Value = "general";            
        }

        protected void lbDeleteBrand_Click(object sender, EventArgs e)
        {
            BrandService.DeleteBrand(GetIntState(BRAND_ID));            
            ResetScreen();
            PopulateBrandInfo(AppConstant.DEFAULT_BRAND);
            enbNotice.Message = "Brand was deleted successfully.";
            hfCurrentPanel.Value = "general";
            LoadBrands();
        }
        
        protected void lbReset_Click(object sender, EventArgs e)
        {
            ResetScreen();
            hfCurrentPanel.Value = "general";
            PopulateBrandInfo(AppConstant.DEFAULT_BRAND);
        }

        protected void ddlDelivery_Init(object sender, EventArgs e)
        {
            ddlDelivery.DataTextField = "TimeLine";
            ddlDelivery.DataValueField = "Id";
            ddlDelivery.DataSource = ShippingService.GetDeliveryList();
            ddlDelivery.DataBind();
        }

        protected void lbUploadImage_Click(object sender, EventArgs e)
        {
            if (fuImage.HasFile)
            {
                // Create new brand media row to get id
                BrandMedia newMedia = new BrandMedia();
                newMedia.BrandId = GetIntState(BRAND_ID);
                newMedia.MediaFilename = fuImage.FileName;
                
                newMedia.Id = BrandService.InsertBrandMedia(newMedia);

                // Save image
                string filename = newMedia.Id.ToString() + Path.GetExtension(fuImage.FileName).ToLower();
                fuImage.SaveAs(MediaSettings.BrandMediaLocalPath + filename);

                // Update brand media
                newMedia.MediaFilename = filename;                
                BrandService.UpdateBrandMedia(newMedia);
                
                PopulateBrandInfo(GetIntState(BRAND_ID));

                hfCurrentPanel.Value = "media";

                enbNotice.Message = "Image was added successfully.";
            }
            else
                enbNotice.Message = "Sorry, there is no image to upload.";
        }

        protected void rptImages_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case DELETE:
                    BrandService.DeleteBrandMedia(Convert.ToInt32(e.CommandArgument));
                    enbNotice.Message = "Image was deleted successfully.";
                    break;
            }

            PopulateBrandInfo(GetIntState(BRAND_ID));
            hfCurrentPanel.Value = "media";
        }
        
        protected void lbSaveMetaBrand_Click(object sender, EventArgs e)
        {
            var brandId = GetIntState(BRAND_ID);
            var brand = BrandService.GetBrandById(brandId);

            if (brand != null)
            {
                brand.MetaTitle = txtMetaTitle.Text.Trim();
                brand.MetaDescription = txtMetaDesc.Text.Trim();
                brand.MetaKeywords = txtMetaKeywords.Text.Trim();
                brand.SecondaryKeywords = txtSecondaryKeywords.Text.Trim();

                BrandService.UpdateBrand(brand);

                enbNotice.Message = "Brand metadata was updated successfully.";
            }
            else
                enbNotice.Message = "Sorry, there is no such brand.";
            
            LoadProducts();
            hfCurrentPanel.Value = "meta";
        }
        
        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Brand | CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All brands and products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";

            hfCurrentPanel.Value = "general";
        }

        protected void lbGenerateFeaturedItems_Click(object sender, EventArgs e)
        {
            var brandId = GetIntState(BRAND_ID);
            var featuredItemType = Convert.ToInt32(ddlFeaturedItemType.SelectedValue);
            var quantity = Convert.ToInt32(ddlFeaturedQuantity.SelectedValue);

            ProductService.AutoGenerateFeaturedItemsByBrand(brandId, featuredItemType, quantity);

            enbNotice.Message = string.Format("Featured items for '{0}' was auto-populated successfully.", ddlFeaturedItemType.SelectedItem.Text);

            ddlFeaturedItemTypeFilter.SelectedIndex = -1;
            var item = ddlFeaturedItemTypeFilter.Items.FindByValue(featuredItemType.ToString());
            item.Selected = true;

            LoadFeaturedProducts(featuredItemType: featuredItemType);

            hfCurrentPanel.Value = "featured";
        }
        
        private void SetChosenFeaturedProducts(int productId, bool chosen)
        {
            if (productId != 0)
            {
                if ((chosen) && !ChosenProductsFeatured.Contains(productId))
                {
                    ChosenProductsFeatured.Add(productId);
                    NotChosenProductsFeatured.Remove(productId);
                }
                else if ((!chosen) && (ChosenProductsFeatured.Contains(productId)))
                {
                    ChosenProductsFeatured.Remove(productId);
                    NotChosenProductsFeatured.Add(productId);
                }
            }
        }

        private void SaveLastViewedFeaturedProduct()
        {
            int productId;

            for (int i = 0; i < gvFeaturedProducts.DataKeys.Count; i++)
            {
                CheckBox cb = gvFeaturedProducts.Rows[i].FindControl("cbFeaturedProductChosen") as CheckBox;
                productId = Convert.ToInt32(gvFeaturedProducts.DataKeys[i].Value);

                if (cb != null) SetChosenFeaturedProducts(productId, cb.Checked);
            }
        }

        protected string FindFeaturedItemPriority(int productId, int brandId)
        {
            var matchingFeaturedItem = new BrandFeaturedItem();
            var brand = BrandService.GetBrandById(brandId);

            if (brand != null)
                matchingFeaturedItem = brand.FeaturedItems.Where(x => x.ProductId == productId && x.BrandId == brandId).FirstOrDefault();

            return (matchingFeaturedItem != null) ? matchingFeaturedItem.Priority.ToString() : string.Empty;
        }

        protected bool ProductIsFeaturedInBrand(int productId, int brandId)
        {
            var featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);
            var brand = BrandService.GetBrandById(brandId);

            if (brand != null)
                return brand.FeaturedItems.Where(x => x.ProductId == productId)
                    .Where(x => x.BrandId == brandId)
                    .Where(x => x.FeaturedItemType == featuredItemType)
                    .Count() > 0;
            else
                return false;
        }

        protected void gvBanners_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int brandId = GetIntState(BRAND_ID);

            switch (e.CommandName)
            {
                case "save":
                    if (brandId != AppConstant.DEFAULT_BRAND)
                    {
                        int brandMediaId = Convert.ToInt32(e.CommandArgument);
                        var media = BrandService.GetBrandMediaById(brandMediaId);

                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        int displayOrder = Convert.ToInt32(((TextBox)row.FindControl("txtBannerDisplayOrder")).Text.Trim());
                        media.Priority = displayOrder;

                        string title = ((TextBox)row.FindControl("txtBannerTitle")).Text.Trim();
                        if (string.IsNullOrEmpty(title))
                        {
                            media.Title = null;
                        }
                        else
                        {
                            media.Title = title;
                        }
                        
                        CheckBox cbStatus = ((CheckBox)row.FindControl("cbBannerEnabled"));
                        media.Enabled = cbStatus.Checked;

                        string fromDate = ((TextBox)row.FindControl("txtBannerStartDate")).Text.Trim();
                        if (string.IsNullOrEmpty(fromDate))
                        {
                            media.StartDate = null;
                        }
                        else
                        {
                            media.StartDate = DateTime.ParseExact(fromDate, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                        }

                        string toDate = ((TextBox)row.FindControl("txtBannerEndDate")).Text.Trim();
                        if (string.IsNullOrEmpty(toDate))
                        {
                            media.EndDate = null;
                        }
                        else
                        {
                            media.EndDate = DateTime.ParseExact(toDate, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                        }

                        BrandService.UpdateBrandMedia(media);

                        enbNotice.Message = "Banner was successfully updated.";
                        PopulateBrandInfo(brandId);
                    }
                    break;
                case "remove":
                    if (brandId != AppConstant.DEFAULT_BRAND)
                    {
                        int id = Convert.ToInt32(e.CommandArgument);
                        BrandService.DeleteBrandMedia(id);

                        enbNotice.Message = "Banner was successfully removed.";
                        PopulateBrandInfo(brandId);
                    }
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "banners";
        }

        #region Assigned items

        protected void gvFeaturedProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedFeaturedProduct();

            gvFeaturedProducts.CustomPageIndex = gvFeaturedProducts.CustomPageIndex + e.NewPageIndex;

            if (gvFeaturedProducts.CustomPageIndex < 0)
                gvFeaturedProducts.CustomPageIndex = 0;

            if (HasState(BRAND_ID))
                LoadFeaturedProducts(featuredItemType: Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featured";
        }

        protected void gvFeaturedProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ProductSortingType.NameAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = ProductSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = ProductSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.NameAsc;
                    break;
                default:
                    break;
            }

            SetState("FeaturedProduct-OrderBy", (int)orderBy);

            if (HasState(BRAND_ID))
                LoadFeaturedProducts(featuredItemType: Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featured";
        }

        protected void gvFeaturedProducts_PreRender(object sender, EventArgs e)
        {
            if (gvFeaturedProducts.TopPagerRow != null)
                gvFeaturedProducts.TopPagerRow.Visible = true;
        }

        protected void gvFeaturedProducts_lbSearch_Click(object sender, EventArgs e)
        {
            if (HasState(BRAND_ID))
                LoadFeaturedProducts(featuredItemType: Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featured";
        }

        protected void gvFeaturedProducts_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvFeaturedProducts.TopPagerRow.FindControl("txtFeaturedProductPageIndex")).Text.Trim()) - 1;

            if ((gvFeaturedProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvFeaturedProducts.CustomPageIndex = gotoIndex;

            if (HasState(BRAND_ID))
                LoadFeaturedProducts(featuredItemType: Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featured";
        }
        
        protected void gvFeaturedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int brandId = GetIntState(BRAND_ID);
            int featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);

            switch (e.CommandName)
            {
                case "savePriority":
                    if (brandId != AppConstant.DEFAULT_BRAND)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        int priority = 0;
                        int.TryParse(((TextBox)row.FindControl("txtFeaturedProductPriority")).Text.Trim(), out priority);

                        BrandService.UpdateBrandFeaturedItemForPriority(productId, brandId, featuredItemType, priority);
                        LoadFeaturedProducts(featuredItemType: featuredItemType);

                        enbNotice.Message = "Priority for featured product was successfully updated.";
                    }
                    break;
                case "remove":
                    if (brandId != AppConstant.DEFAULT_BRAND)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        BrandService.DeleteBrandFeaturedItem(productId: productId, brandId: brandId, featuredItemType: featuredItemType);
                        LoadFeaturedProducts(featuredItemType: featuredItemType);
                        enbNotice.Message = "Featured item was successfully removed.";
                    }
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "featured";
        }

        #endregion

        #region Search and assign
        
        protected void gvNewFeaturedProducts_PreRender(object sender, EventArgs e)
        {
            if (gvNewFeaturedProducts.TopPagerRow != null)
                gvNewFeaturedProducts.TopPagerRow.Visible = true;

            if (gvNewFeaturedProducts.TopPagerRow != null)
            {
                gvNewFeaturedProducts.TopPagerRow.Visible = true;
                ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductId")).Text = GetStringState("gvNewFeaturedProduct.Id");
                ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductName")).Text = GetStringState("gvNewFeaturedProduct.Name");
            }
        }

        protected void gvNewFeaturedProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNewFeaturedProducts.CustomPageIndex = gvNewFeaturedProducts.CustomPageIndex + e.NewPageIndex;

            if (gvNewFeaturedProducts.CustomPageIndex < 0)
                gvNewFeaturedProducts.CustomPageIndex = 0;

            if (HasState(BRAND_ID))
                LoadNewFeaturedProducts();

            hfCurrentPanel.Value = "featured";
        }

        protected void gvNewFeaturedProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ProductSortingType.NameAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = ProductSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = ProductSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.NameAsc;
                    break;
                default:
                    break;
            }

            SetState("NewFeaturedProduct-OrderBy", (int)orderBy);

            if (HasState(BRAND_ID))
                LoadNewFeaturedProducts();

            hfCurrentPanel.Value = "featured";
        }

        protected void gvNewFeaturedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "assign":
                    int brandId = GetIntState(BRAND_ID);
                    if (brandId != AppConstant.DEFAULT_BRAND)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        int featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);

                        var featuredItem = new BrandFeaturedItem
                        {
                            BrandId = brandId,
                            ProductId = productId,
                            FeaturedItemType = featuredItemType
                        };

                        var result = BrandService.ProcessBrandFeaturedItemInsertion(featuredItem);

                        if (string.IsNullOrEmpty(result))
                            enbNotice.Message = "Item was successfully assigned.";
                        else
                            enbNotice.Message = "Failed to assign the item. " + result;

                        LoadFeaturedProducts(featuredItemType: featuredItemType);
                        hfCurrentPanel.Value = "featured";
                    }
                    break;
                default:
                    break;
            }
        }

        protected void gvNewFeaturedProducts_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvNewFeaturedProducts.TopPagerRow.FindControl("txtNewFeaturedProductPageIndex")).Text.Trim()) - 1;

            if ((gvNewFeaturedProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvNewFeaturedProducts.CustomPageIndex = gotoIndex;

            if (HasState(BRAND_ID))
                LoadNewFeaturedProducts();

            hfCurrentPanel.Value = "featured";
        }

        protected void lbSearchNewFeaturedProducts_Click(object sender, EventArgs e)
        {
            int productId = 0;
            int.TryParse(((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductId")).Text.Trim(), out productId);
            string name = ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductName")).Text.Trim();

            SetState("gvNewFeaturedProduct.Id", productId == 0 ? null : productId.ToString());
            SetState("gvNewFeaturedProduct.Name", name);

            if (HasState(BRAND_ID))
                LoadNewFeaturedProducts();

            hfCurrentPanel.Value = "featured";
        }

        protected void lbResetNewFeaturedProducts_Click(object sender, EventArgs e)
        {
            DisposeState("gvNewFeaturedProduct.Id");
            DisposeState("gvNewFeaturedProduct.Name");

            if (HasState(BRAND_ID))
                LoadNewFeaturedProducts();

            hfCurrentPanel.Value = "featured";
        }

        #endregion

        private void LoadNewFeaturedProducts()
        {
            int brandId = GetIntState(BRAND_ID);
            int? productId = null;
            string name = null;
            var orderBy = ProductSortingType.Position;

            if (HasState("gvNewFeaturedProduct.Id")) productId = GetIntState("gvNewFeaturedProduct.Id");
            if (HasState("gvNewFeaturedProduct.Name")) name = GetStringState("gvNewFeaturedProduct.Name");

            var result = ProductService.GetPagedProductOverviewModelsByBrand(
                pageIndex: gvNewFeaturedProducts.CustomPageIndex,
                pageSize: gvNewFeaturedProducts.PageSize,
                productId: productId,
                name: name,
                brandId: brandId,
                orderBy: orderBy);

            if (result != null)
            {
                gvNewFeaturedProducts.DataSource = result.Items;
                gvNewFeaturedProducts.RecordCount = result.TotalCount;
                gvNewFeaturedProducts.CustomPageCount = result.TotalPages;
            }

            gvNewFeaturedProducts.DataBind();            
        }

        private void LoadFeaturedProducts(int? featuredItemType = null, int? notFeaturedItemType = null)
        {
            int brandId = GetIntState(BRAND_ID);
            var orderBy = ProductSortingType.Position;

            if (HasState("FeaturedProduct - OrderBy")) orderBy = (ProductSortingType)GetIntState("FeaturedProduct - OrderBy");
            
            var result = ProductService.GetPagedProductOverviewModelsByBrand(
                pageIndex: gvFeaturedProducts.CustomPageIndex,
                pageSize: gvFeaturedProducts.PageSize,
                brandId: brandId,
                featuredItemType: featuredItemType,
                notFeaturedItemType: notFeaturedItemType,
                orderBy: orderBy);

            if (result != null)
            {
                gvFeaturedProducts.DataSource = result.Items;
                gvFeaturedProducts.RecordCount = result.TotalCount;
                gvFeaturedProducts.CustomPageCount = result.TotalPages;
            }

            gvFeaturedProducts.DataBind();

            if (featuredItemType.HasValue)
            {
                switch ((FeaturedItemType)featuredItemType)
                {
                    case FeaturedItemType.Position:
                        ltByFeaturedItemType.Text = "Position";
                        break;
                    case FeaturedItemType.WhatsNew:
                        ltByFeaturedItemType.Text = "WhatsNew";
                        break;
                    case FeaturedItemType.TopRated:
                        ltByFeaturedItemType.Text = "TopRated";
                        break;
                    case FeaturedItemType.BestSeller:
                        ltByFeaturedItemType.Text = "BestSeller";
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveLastViewedProduct()
        {
            int productId;

            for (int i = 0; i < gvProducts.DataKeys.Count; i++)
            {
                CheckBox cb = gvProducts.Rows[i].FindControl("cbChosen") as CheckBox;
                productId = Convert.ToInt32(gvProducts.DataKeys[i].Value);

                if (cb != null) SetChosenProducts(productId, cb.Checked);
            }
        }

        private void PopulateBrandInfo(int brandId)
        {
            SetState(BRAND_ID, brandId);
            Brand brand = BrandService.GetBrandById(brandId);

            if (brand != null)
            {
                SetState(BRAND_NAME, brand.Name);

                txtName.Text = Server.HtmlDecode(brand.Name);

                if (brand.FlashImage == string.Empty)
                {
                    imgLogo.Visible = false;
                    fuLogo.Visible = true;
                    cbRemoveLogo.Visible = false;
                }
                else
                {
                    imgLogo.Src = "/get_image_handler.aspx?type=brand&img=" + brand.FlashImage;
                    imgLogo.Alt = brand.FlashImage;
                    imgLogo.Visible = true;
                    fuLogo.Visible = false;
                    cbRemoveLogo.Visible = true;
                }

                txtDesc.Text = brand.Description;
                txtUrlKey.Text = brand.UrlRewrite;
                cbMicrosite.Checked = brand.HasMicrosite;
                txtMetaDesc.Text = brand.MetaDescription;
                txtMetaTitle.Text = brand.MetaTitle;
                txtMetaKeywords.Text = brand.MetaKeywords;

                ddlDelivery.SelectedIndex = -1;
                ddlDelivery.Items.FindByValue(brand.Delivery.Id.ToString()).Selected = true;
                cbEnforceStockCount.Checked = brand.EnforceStockCount;
                cbEnabled.Checked = brand.Enabled;
                
                gvBanners.DataSource = brand.BrandMedias;
                gvBanners.DataBind();

                LoadNewFeaturedProducts();
            }

            if (brandId == AppConstant.DEFAULT_BRAND)
            {
                SetState(MODE, NEW);
                SetState(CHOSEN_FILTER, ANY);
                lbSaveImage.Enabled = false;
            }
            else
            {
                SetState(MODE, EDIT);
                SetState(CHOSEN_FILTER, YES);
                lbSaveImage.Enabled = true;
            }

            LoadProducts();
        }
        
        private void LoadBrands()
        {
            int[] brandIds = null;
            string name = null;
            bool? hideDisabled = null;
            var orderBy = BrandSortingType.IdAsc;

            if (HasState(BRAND_ID_FILTER))
            {
                string value = GetStringState(BRAND_ID_FILTER);
                int temp;
                brandIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(BRAND_NAME_FILTER)) name = GetStringState(BRAND_NAME_FILTER);
            if (HasState("Brand-OrderBy")) orderBy = (BrandSortingType)GetIntState("Brand-OrderBy");
            if (cbHideDisabled.Checked) hideDisabled = true;

            var result = BrandService.GetPagedBrandOverviewModel(
                pageIndex: gvBrands.CustomPageIndex,
                pageSize: gvBrands.PageSize,
                brandIds: brandIds,
                name: name,
                hideDisabled: hideDisabled,
                orderBy: orderBy);

            if (result != null)
            {
                gvBrands.DataSource = result.Items;
                gvBrands.RecordCount = result.TotalCount;
                gvBrands.CustomPageCount = result.TotalPages;
            }

            gvBrands.DataBind();

            if (gvBrands.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            int[] brandIds = null;
            int? notBrandId = null;
            var orderBy = ProductSortingType.IdAsc;

            if (HasState(PRODUCT_NAME_FILTER)) name = GetStringState(PRODUCT_NAME_FILTER);
            if (HasState("Product-OrderBy")) orderBy = (ProductSortingType)GetIntState("Product-OrderBy");
            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (GetStringState(CHOSEN_FILTER) == YES && HasState(BRAND_ID)) brandIds = new int[] { GetIntState(BRAND_ID) };
            if (GetStringState(CHOSEN_FILTER) == NO) notBrandId = GetIntState(BRAND_ID);

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                productIds: productIds,
                brandIds: brandIds,
                notBrandId: notBrandId,
                keywords: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();

            if (gvProducts.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private void ResetScreen()
        {
            ClearInfo();            
            SetState(MODE, NEW);
            enbNotice.Message = string.Empty;
        }

        private void ClearInfo()
        {
            txtName.Text = string.Empty;
            txtUrlKey.Text = string.Empty;
            imgLogo.Src = string.Empty;
            imgLogo.Alt = string.Empty;
            fuLogo.Visible = true;
            txtMetaDesc.Text = string.Empty;
            txtMetaTitle.Text = string.Empty;
            txtMetaKeywords.Text = string.Empty;
            cbEnforceStockCount.Checked = false;
            ddlDelivery.SelectedIndex = -1;

            cbMicrosite.Checked = false;
            txtDesc.Text = string.Empty;

            ChosenProducts.Clear();
            NotChosenProducts.Clear();

            gvFeaturedProducts.DataBind();
            gvBanners.DataBind();
        }

        private void SetChosenProducts(int productId, bool chosen)
        {
            if (productId != 0)
            {
                if ((chosen) && !ChosenProducts.Contains(productId))
                {
                    ChosenProducts.Add(productId);
                    NotChosenProducts.Remove(productId);
                }
                else if ((!chosen) && (ChosenProducts.Contains(productId)))
                {
                    ChosenProducts.Remove(productId);
                    NotChosenProducts.Add(productId);
                }
            }
        }        
    }
}