using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class brand_category_info : BasePage
    {
        public IBrandService BrandService { get; set; }
        public IProductService ProductService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected void ectBrandCategory_TreeChanged(string brandCategoryName, int brandCategoryId)
        {
            SetState(LAST_CHOSEN_BRAND_CATEGORY, brandCategoryId);
            ResetScreen();
            PopulateBrandCategoryInfo(brandCategoryId);
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState(CHOSEN_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen")).SelectedValue);

            LoadProducts();

            hfCurrentPanel.Value = "products";
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            LoadProducts();
            hfCurrentPanel.Value = "products";
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            SetState(CHOSEN_FILTER, (GetIntState(BRAND_CATEGORY_ID) == -1 ? ANY : YES));

            LoadProducts();

            hfCurrentPanel.Value = "products";
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
            if (GetIntState(BRAND_CATEGORY_ID) == -1) ddl.Enabled = false;
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedProduct();

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
            hfCurrentPanel.Value = "products";
        }

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
                gvProducts.TopPagerRow.Visible = true;
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            ResetScreen();
            LinkButton button = sender as LinkButton;

            if (button != null && button.CommandArgument == ADD_BRAND_CATEGORY)
            {
                hfParent.Value = string.Empty;
                ltlParent.Text = string.Empty;
                
                SetState(LAST_CHOSEN_BRAND_CATEGORY, AppConstant.DEFAULT_BRAND_CATEGORY);
            }
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            SaveLastViewedProduct();

            if (GetStringState(MODE) == EDIT)
            {
                if (NotChosenProducts.Count > 0)
                    BrandService.UpdateProductWithBrandCategory(AppConstant.DEFAULT_BRAND_CATEGORY, NotChosenProducts);

                if (ChosenProducts.Count > 0)
                    BrandService.UpdateProductWithBrandCategory(GetIntState(LAST_CHOSEN_BRAND_CATEGORY), ChosenProducts);

                var treeList = BrandService.GetBrandCategoryTreeList(GetIntState(LAST_CHOSEN_BRAND_CATEGORY));

                ectBrandCategory.FindSelectedNode(AppConstant.DEFAULT_BRAND_CATEGORY, treeList);

                DisposeState(PRODUCT_ID_FILTER);
                DisposeState(NAME_FILTER);
                SetState(CHOSEN_FILTER, (GetIntState(BRAND_CATEGORY_ID) == -1 ? ANY : YES));

                // Reset page number to 1
                gvProducts.CustomPageIndex = 0;
                UtilityService.RefreshCache(CacheEntityKey.Brand | CacheEntityKey.Product);

                LoadProducts();

                enbNotice.Message = "Product(s) was saved successfully.";
            }
            else
                enbNotice.Message = "Please save new brand category first.";

            hfCurrentPanel.Value = "products";
        }

        protected void lbSaveBrandCategory_Click(object sender, EventArgs e)
        {
            string urlKey = txtUrlKey.Text.Trim();
            txtUrlKey.BackColor = Color.Empty;

            // If url key is empty, regenerate with given category name
            if (urlKey == string.Empty)
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtName.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            // Make sure url key is unique
            if (((GetStringState(MODE) == NEW) && (BrandService.GetBrandCategoryByUrlKey(urlKey) != null)) ||
                ((GetStringState(MODE) == EDIT) && (BrandService.GetBrandCategoryByUrlKey(urlKey) != null) && (BrandService.GetBrandCategoryByUrlKey(urlKey).Id != GetIntState(BRAND_CATEGORY_ID))))
            {
                enbNotice.Message = "Url key is not unique.";
            }
            else
            {
                string thumbnailFilename = string.Empty;

                if (fuThumbnail.HasFile)
                {
                    thumbnailFilename = txtUrlKey.Text.Trim() + Path.GetExtension(fuThumbnail.FileName).ToLower();
                    fuThumbnail.SaveAs(MediaSettings.BrandMediaLocalPath + thumbnailFilename);
                }
                else
                {
                    if (!cbRemoveThumb.Checked)
                    {
                        BrandCategory category = BrandService.GetBrandCategoryByUrlKey(urlKey);

                        if (category != null)
                            thumbnailFilename = category.ImageUrl;
                    }
                }

                BrandCategory brandCategory = new BrandCategory();
                brandCategory.Name = Server.HtmlEncode(txtName.Text.Trim());
                brandCategory.BrandId = QueryBrandId;
                brandCategory.Description = Server.HtmlEncode(txtDesc.Text.Trim());
                brandCategory.ImageUrl = thumbnailFilename;
                brandCategory.UrlRewrite = txtUrlKey.Text.Trim();
                brandCategory.Visible = cbVisible.Checked;

                if (!string.IsNullOrEmpty(hfParent.Value))
                    brandCategory.ParentId = Convert.ToInt32(hfParent.Value);

                if (GetStringState(MODE) == NEW)
                {
                    if (string.IsNullOrEmpty(hfParent.Value))
                        brandCategory.ParentId = AppConstant.DEFAULT_BRAND_CATEGORY;
                    
                    brandCategory.Id = BrandService.InsertBrandCategory(brandCategory);
                }
                else // Edit mode
                {
                    brandCategory.Id = GetIntState(LAST_CHOSEN_BRAND_CATEGORY);
                    BrandService.UpdateBrandCategory(brandCategory);
                }

                ectBrandCategory.Repopulate();

                var treeList = BrandService.GetBrandCategoryTreeList(brandCategory.Id);
                ectBrandCategory.FindSelectedNode(AppConstant.DEFAULT_BRAND_CATEGORY, treeList);

                PopulateBrandCategoryInfo(brandCategory.Id);

                enbNotice.Message = "Brand category was saved successfully.";
            }

            cbRemoveThumb.Checked = false;

            ChosenProducts.Clear();
            NotChosenProducts.Clear();
            LoadProducts();
        }

        protected void lbDeleteBrandCategory_Click(object sender, EventArgs e)
        {
            BrandService.ProcessBrandCategoryRemoval(GetIntState(BRAND_CATEGORY_ID));
            ResetScreen();
            ectBrandCategory.Repopulate();
        }

        protected void lbEditParent_Click(object sender, EventArgs e)
        {
            mvParent.ActiveViewIndex = 1;
        }

        protected void ectParent_TreeNodeSelected(string brandCategoryName, int brandCategoryId)
        {
            ltlParent.Text = brandCategoryName;
            hfParent.Value = brandCategoryId.ToString();
            mvParent.ActiveViewIndex = 0;
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Brand | CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All brands and products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected override void OnPreRender(EventArgs e)
        {
            if ((gvProducts != null) && (gvProducts.HeaderRow != null))
            {
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
                ((DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen")).Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
            }

            base.OnPreRender(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (GetIntState(BRAND_CATEGORY_ID) == AppConstant.DEFAULT_BRAND_CATEGORY)
            {
                lbDeleteBrandCategory.Visible = false;
                ltlTitle.Text = "New Brand Category";
            }
            else
            {
                var brandCategoryId = GetIntState(BRAND_CATEGORY_ID);
                var brandCategory = BrandService.GetBrandCategoryById(brandCategoryId);

                if (brandCategory != null)
                {
                    lbDeleteBrandCategory.Visible = true;
                    ltlTitle.Text = string.Format("{0} (ID: {1})", brandCategory.Name, GetIntState(BRAND_CATEGORY_ID).ToString());
                }
            }

            base.OnPreRenderComplete(e);
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

        private void ResetScreen()
        {
            SetState("sortdirection", string.Empty);
            ClearInfo();
            SetState(MODE, NEW);
            hfParent.Value = string.Empty;
            ltlParent.Text = string.Empty;
            enbNotice.Message = string.Empty;
            PopulateBrandCategoryInfo(AppConstant.DEFAULT_BRAND_CATEGORY);
        }

        private void PopulateBrandCategoryInfo(int brandCategoryId)
        {
            BrandCategory brandCategory = BrandService.GetBrandCategoryById(brandCategoryId);

            if (brandCategory != null)
            {
                txtName.Text = Server.HtmlDecode(brandCategory.Name);
                txtDesc.Text = Server.HtmlDecode(brandCategory.Description);
                txtUrlKey.Text = brandCategory.UrlRewrite;

                if (brandCategory.ImageUrl == string.Empty)
                {
                    imgThumbnail.Visible = false;
                    cbRemoveThumb.Visible = false;
                }
                else
                {
                    imgThumbnail.Src = "/get_image_handler.aspx?type=brand&img=" + brandCategory.ImageUrl;
                    imgThumbnail.Visible = true;
                    cbRemoveThumb.Visible = true;
                }

                cbVisible.Checked = brandCategory.Visible;
                hfParent.Value = brandCategory.ParentId.ToString();
                
                if (brandCategory.ParentId != AppConstant.DEFAULT_BRAND_CATEGORY) ltlParent.Text = BrandService.GetBrandCategoryById(brandCategory.ParentId).Name;
            }

            SetState(BRAND_CATEGORY_ID, brandCategoryId);

            if (GetIntState(BRAND_CATEGORY_ID) == AppConstant.DEFAULT_BRAND_CATEGORY)
            {
                SetState(MODE, NEW);
                SetState(CHOSEN_FILTER, ANY);
            }
            else
            {
                SetState(MODE, EDIT);
                SetState(CHOSEN_FILTER, YES);
            }

            LoadProducts();

            hfCurrentPanel.Value = "general";
        }
        
        private void ClearInfo()
        {
            txtName.Text = string.Empty;
            txtDesc.Text = string.Empty;
            txtUrlKey.Text = string.Empty;
            imgThumbnail.Visible = false;
            imgThumbnail.Src = string.Empty;
            ChosenProducts.Clear();
            NotChosenProducts.Clear();
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            int[] brandCategoryIds = null;
            int? notBrandCategoryId = null;
            var orderBy = ProductSortingType.IdAsc;

            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
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
            if (GetStringState(CHOSEN_FILTER) == YES && (GetIntState(BRAND_CATEGORY_ID) != AppConstant.DEFAULT_BRAND_CATEGORY))
            {
                brandCategoryIds = new int[] { GetIntState(BRAND_CATEGORY_ID) };
            }
            if (GetStringState(CHOSEN_FILTER) == NO) notBrandCategoryId = GetIntState(BRAND_CATEGORY_ID);

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                productIds: productIds,
                brandCategoryIds: brandCategoryIds,
                notBrandCategoryId: notBrandCategoryId,
                keywords: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();

            //if (gvProducts.Rows.Count <= 0) enbNotice.Message = "No records found.";            
        }        
    }
}