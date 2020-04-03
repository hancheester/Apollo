using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class category_default : BasePage
    {
        public IBrandService BrandService { get; set; }
        public ICategoryService CategoryService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IProductService ProductService { get; set; }
        public ICampaignService CampaignService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }        
        public MediaSettings MediaSettings { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            var featuredTypeValues = Enum.GetValues(typeof(FeaturedItemType));
            var featuredTypeNames = Enum.GetNames(typeof(FeaturedItemType));
            var k = 0;

            foreach (var item in featuredTypeValues)
            {
                ListItem li = new ListItem(featuredTypeNames[k], ((int)item).ToString());
                ddlFeaturedItemType.Items.Add(li);

                ListItem fi = new ListItem(featuredTypeNames[k], ((int)item).ToString());
                ddlFeaturedItemTypeFilter.Items.Add(fi);

                k++;
            }

            var templates = CategoryService.GetAllCategoryTemplates();
            foreach (var template in templates)
            {
                ddlCategoryTemplate.Items.Add(new ListItem(template.Name, template.Id.ToString()));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            DropDownList ddlFilterChosen;

            if ((gvProducts != null) && (gvProducts.HeaderRow != null))
            {
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
                ddlFilterChosen = (DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen");
                ddlFilterChosen.SelectedIndex = -1;
                var productItem = ddlFilterChosen.Items.FindByValue(GetStringState(CHOSEN_FILTER));
                if (productItem != null) productItem.Selected = true;
            }

            if ((gvNewFeaturedProducts != null) && (gvNewFeaturedProducts.HeaderRow != null))
            {
                ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductId")).Text = GetStringState("gvNewFeaturedProduct.Id");
                ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductName")).Text = GetStringState("gvNewFeaturedProduct.Name");
            }

            if ((gvFeaturedBrands != null) && (gvFeaturedBrands.HeaderRow != null))
            {
                ((TextBox)gvFeaturedBrands.HeaderRow.FindControl("txtFeaturedBrandFilterId")).Text = GetStringState(BRAND_ID_FILTER);
                ((TextBox)gvFeaturedBrands.HeaderRow.FindControl("txtFeaturedBrandFilterName")).Text = GetStringState(NAME_FILTER_3);
                ddlFilterChosen = (DropDownList)gvFeaturedBrands.HeaderRow.FindControl("ddlFeaturedBrandFilterChosen");
                ddlFilterChosen.SelectedIndex = -1;
                var featuredBrand = ddlFilterChosen.Items.FindByValue(GetStringState(CHOSEN_FILTER_3));
                if (featuredBrand != null) featuredBrand.Selected = true;
            }

            base.OnPreRender(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY)
            {
                if (GetIntState(CATEGORY_ID_FILTER) != AppConstant.DEFAULT_CATEGORY_FILTER)
                {
                    lbDeleteCategory.Visible = false;
                    ltlTitle.Text = string.Format("{0} (FILTER ID: {1})", GetStringState(CATEGORY_NAME), GetIntState(CATEGORY_ID_FILTER).ToString());
                }
                else
                {
                    lbDeleteCategory.Visible = false;
                    ltlTitle.Text = "New Category";
                    phTabs.Visible = false;
                }
            }
            else
            {
                var categoryId = GetIntState(CATEGORY_ID);
                var category = CategoryService.GetCategory(categoryId);
                var treeList = CategoryService.GetTreeList(categoryId);
                var topUrlKey = string.Empty;
                var secondUrlKey = string.Empty;
                var thirdUrlKey = string.Empty;
                for (int i = 0; i < treeList.Count; i++)
                {
                    if (i == 0)
                    {
                        var topCategory = CategoryService.GetCategory(treeList[i]);
                        if (topCategory != null) topUrlKey = topCategory.UrlRewrite;
                    }

                    if (i == 1)
                    {
                        var secondCategory = CategoryService.GetCategory(treeList[i]);
                        if (secondCategory != null) secondUrlKey = secondCategory.UrlRewrite;
                    }

                    if (i == 2)
                    {
                        var thirdCategory = CategoryService.GetCategory(treeList[i]);
                        if (thirdCategory != null) thirdUrlKey = thirdCategory.UrlRewrite;
                    }
                }
                
                lbDeleteCategory.Visible = true;
                ltlTitle.Text = string.Format("{0} (ID: {1}) {2} {3}", 
                    GetStringState(CATEGORY_NAME), 
                    GetIntState(CATEGORY_ID).ToString(),
                    string.Format("<a href='{0}' target='_blank'><i class='fa fa-external-link' aria-hidden='true'></i></a>", AdminStoreUtility.GetCategoryUrl(topUrlKey, secondUrlKey, thirdUrlKey)),
                    category.Visible ? null : "<i class='fa fa-eye-slash' aria-hidden='true' title='offline'></i>");
                phTabs.Visible = true;
            }

            base.OnPreRenderComplete(e);
        }

        protected void ectCategory_TreeChanged(string categoryName, int categoryId)
        {
            SetState(CATEGORY_NAME, categoryName);
            SetState(LAST_CHOSEN_CATEGORY, categoryId);
            SetState(CATEGORY_ID, categoryId);
            SetState(CATEGORY_ID_FILTER, null);
            lbEditParent.Enabled = true;
            ectParent.Visible = true;
            ResetScreen();

            hfCurrentPanel.Value = "general";

            if (categoryId != AppConstant.DEFAULT_CATEGORY)
                PopulateCategory(categoryId, true);
        }
        
        protected void ectParent_TreeNodeSelected(string categoryName, int categoryId)
        {
            ltlParent.Text = categoryName;
            hfParent.Value = categoryId.ToString();
            enbNotice.Message = string.Empty;
            ectParent.Clear();
            mvParent.ActiveViewIndex = 0;
        }
        
        protected void Reset(object sender, EventArgs e)
        {
            ResetScreen();
            LinkButton button = sender as LinkButton;

            if (button != null && button.CommandArgument == ADD_CATEGORY)
            {
                hfParent.Value = GetIntState(CATEGORY_ID).ToString();
                ltlParent.Text = GetStringState(CATEGORY_NAME);
            }
        }
        
        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
            ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
            if (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY) ddl.Enabled = false;
            if (GetIntState(CATEGORY_ID_FILTER) != AppConstant.DEFAULT_CATEGORY_FILTER) ddl.Enabled = true;
        }

        protected void lbSaveCategory_Click(object sender, EventArgs e)
        {
            int parentId = 0;            
            int defaultTreeLevel = 1;
            string urlKey = txtUrlKey.Text.Trim();
            txtUrlKey.BackColor = Color.Empty;

            if (string.IsNullOrEmpty(hfParent.Value) == false)
                parentId = Convert.ToInt32(hfParent.Value);
            
            // If urlKey is empty, regenerate with given category name
            if (string.IsNullOrEmpty(urlKey))
            {
                urlKey = AdminStoreUtility.GetFriendlyUrlKey(txtName.Text.Trim());
                txtUrlKey.Text = urlKey;
            }

            int categoryId = 0;
            string categoryName = string.Empty;

            // Make sure urlKey is unique
            var foundCategory = CategoryService.GetCategoryByUrlKey(urlKey);
            if (((GetStringState(MODE) == NEW) && foundCategory != null) ||
                ((GetStringState(MODE) == EDIT) && foundCategory != null) && foundCategory.Id != GetIntState(CATEGORY_ID))
            {
                enbNotice.Message = "Url key is not unique.";
                return;
            }

            string thumbnailFilename = string.Empty;

            if (fuThumbnail.HasFile)
            {
                thumbnailFilename = txtUrlKey.Text.Trim() + Path.GetExtension(fuThumbnail.FileName).ToLower();
                fuThumbnail.SaveAs(MediaSettings.CategoryMediaLocalPath + thumbnailFilename);
            }

            SaveLastViewedProduct();

            Category category = new Category
            {
                ParentId = parentId,
                CategoryName = Server.HtmlEncode(txtName.Text.Trim()),
                MetaTitle = Server.HtmlEncode(txtMetaTitle.Text.Trim()),
                MetaKeywords = Server.HtmlEncode(txtMetaKeywords.Text.Trim()),
                SecondaryKeywords = Server.HtmlEncode(txtSecondaryKeywords.Text.Trim()),
                H1Title = Server.HtmlEncode(txtH1Title.Text.Trim()),
                UrlRewrite = urlKey.ToLower(),
                ShortDescription = txtShortDesc.Text.Trim(),
                Description = txtDesc.Text.Trim(),
                TreeLevel = defaultTreeLevel,
                ColourScheme = ddlColourScheme.SelectedValue,
                Visible = cbVisible.Checked,
                ThumbnailFilename = thumbnailFilename,
                Priority = Convert.ToInt32(txtPriority.Text),
                CategoryTemplateId = Convert.ToInt32(ddlCategoryTemplate.SelectedValue)
            };

            if (GetStringState(MODE) == NEW)
            {
                if (GetIntState(LAST_CHOSEN_CATEGORY) != AppConstant.DEFAULT_CATEGORY)
                    category.ParentId = GetIntState(LAST_CHOSEN_CATEGORY);

                category.TreeLevel = GenerateTreeLevel(parentId);
                category.Id = CategoryService.InsertCategory(category);
                categoryId = category.Id;
                categoryName = category.CategoryName;
            }
            else // Edit mode
            {
                category.Id = GetIntState(LAST_CHOSEN_CATEGORY);

                Category currentCategory = CategoryService.GetActiveCategory(category.Id);
                categoryId = category.Id;
                categoryName = category.CategoryName;

                // Get thumbnail filename
                if (!fuThumbnail.HasFile && currentCategory != null)
                    category.ThumbnailFilename = currentCategory.ThumbnailFilename;

                if (hfParent.Value != string.Empty)
                    category.ParentId = Convert.ToInt32(hfParent.Value);

                // Remove thumbnail filename
                if (cbRemoveThumb.Checked && !string.IsNullOrWhiteSpace(category.ThumbnailFilename))
                {
                    File.Delete(MediaSettings.CategoryMediaLocalPath + category.ThumbnailFilename);
                    category.ThumbnailFilename = string.Empty;
                }

                category.TreeLevel = GenerateTreeLevel(parentId);

                CategoryService.UpdateCategory(category);

                if (NotChosenProducts.Count > 0)
                    CategoryService.DeleteCategoryFromProductIdList(category.Id, NotChosenProducts);
                
                if (NotChosenBrandsFeatured.Count > 0)
                    BrandService.DeleteFeaturedBrandListInCategory(category.Id, NotChosenBrandsFeatured);
            }

            if (ChosenProducts.Count > 0)
                CategoryService.ProcessProductCategoryByCategoryIdAndProductIdListInsertion(category.Id, ChosenProducts);
            
            if (ChosenBrandsFeatured.Count > 0)
                BrandService.ProcessNewBrandFeaturedInCategory(category.Id, ChosenBrandsFeatured.ToArray());

            if (GetStringState(MODE) == NEW)
            {
                enbNotice.Message = "Category was created successfully.";
            }
            else
            {
                enbNotice.Message = "Category was updated successfully.";
            }

            var treeList = CategoryService.GetTreeList(category.Id);

            ectCategory.Repopulate();
            ectCategory.FindSelectedNode(AppConstant.DEFAULT_CATEGORY, treeList);

            ChosenProducts.Clear();
            NotChosenProducts.Clear();
            ChosenProductsFeatured.Clear();
            NotChosenProductsFeatured.Clear();
            ChosenBrandsFeatured.Clear();
            NotChosenBrandsFeatured.Clear();

            //int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
            SetState(LAST_CHOSEN_CATEGORY, categoryId);
            SetState(CATEGORY_ID, categoryId);
            SetState(CATEGORY_NAME, categoryName);
            PopulateCategory(categoryId);

        }

        protected void lbDeleteCategory_Click(object sender, EventArgs e)
        {
            int categoryId = GetIntState(CATEGORY_ID);

            // We need to remove image files first.
            var items = CategoryService.GetCategoryMediasByCategoryId(categoryId);
            if(items != null)
            {
                string categoryMediaPath = MediaSettings.CategoryMediaLocalPath;
                foreach (var item in items)
                {
                    var filePath = categoryMediaPath + item.MediaFilename;
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
            }

            CategoryService.ProcessCategoryRemoval(categoryId);
            
            Response.Redirect("/catalog/category_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CategoryDeleted);
        }

        protected void lbEditParent_Click(object sender, EventArgs e)
        {
            mvParent.ActiveViewIndex = 1;
            int categoryId = 0;
            int.TryParse(hfParent.Value, out categoryId);
            var treeList = CategoryService.GetTreeList(categoryId);
            ectParent.FindSelectedNode(AppConstant.DEFAULT_CATEGORY, treeList);           
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Category | CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All categories and products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        #region Category products

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedProduct();

            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            if (HasState(CATEGORY_ID))
                LoadProducts(GetIntState(CATEGORY_ID));

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

            SetState("ProductCategory-OrderBy", (int)orderBy);
            if (HasState(CATEGORY_ID))
                LoadProducts(GetIntState(CATEGORY_ID));
            hfCurrentPanel.Value = "products";
        }

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
                gvProducts.TopPagerRow.Visible = true;
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState(CHOSEN_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlFilterChosen")).SelectedValue);

            if (HasState(CATEGORY_ID))
                LoadProducts(GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "products";
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            if (HasState(CATEGORY_ID))
                LoadProducts(GetIntState(CATEGORY_ID));
            hfCurrentPanel.Value = "products";
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            SetState(CHOSEN_FILTER, (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY ? ANY : YES));

            LoadProducts(GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "products";
        }

        protected void lbSaveProducts_Click(object sender, EventArgs e)
        {
            SaveLastViewedProduct();

            int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

            if (NotChosenProducts.Count > 0)
                CategoryService.DeleteCategoryFromProductIdList(categoryId, NotChosenProducts);

            if (ChosenProducts.Count > 0)
                CategoryService.ProcessProductCategoryByCategoryIdAndProductIdListInsertion(categoryId, ChosenProducts);

            SetState(CHOSEN_FILTER, YES);
            PopulateCategory(GetIntState(LAST_CHOSEN_CATEGORY));

            var treeList = CategoryService.GetTreeList(categoryId);

            ectCategory.Repopulate();
            ectCategory.FindSelectedNode(AppConstant.DEFAULT_CATEGORY, treeList);

            hfCurrentPanel.Value = "products";
            enbNotice.Message = "Products were updated successfully.";
        }

        protected string GetAssignedCategory(int productId)
        {
            var assignedCategory = CategoryService.GetCategoriesByProductId(productId);
            var listBuilder = new StringBuilder("<ul>");

            foreach (var item in assignedCategory)
            {
                StringBuilder sb = new StringBuilder();
                var tree = CategoryService.GetTreeList(item.Id);

                foreach (var leaf in tree)
                {
                    var leafCategory = CategoryService.GetCategory(leaf);

                    if (leafCategory != null)
                    {
                        if (sb.ToString() != string.Empty) sb.Append(" > ");

                        var visibilitySymbol = string.Empty;
                        if (leafCategory.Visible == false)
                        {
                            visibilitySymbol = " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>";
                        }

                        sb.Append(Server.HtmlDecode(leafCategory.CategoryName + visibilitySymbol));
                    }
                }

                listBuilder.AppendFormat("<li>{0}</li>", Server.HtmlDecode(sb.ToString()));
            }

            return listBuilder.Append("</ul>").ToString();
        }

        private void LoadProducts(int categoryId, bool firstTimeLoading = false)
        {
            int[] categoryIds = null;
            string name = null;
            int[] productIds = null;
            int? notCategoryId = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (GetStringState(CHOSEN_FILTER) == YES) categoryIds = new int[] { categoryId };
            if (GetStringState(CHOSEN_FILTER) == NO) notCategoryId = categoryId;
            if (HasState("ProductCategory-OrderBy")) orderBy = (ProductSortingType)GetIntState("ProductCategory-OrderBy");
            if (firstTimeLoading) gvProducts.CustomPageIndex = 0;

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                categoryIds: categoryIds,
                productIds: productIds,
                keywords: name,
                notCategoryId: notCategoryId,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();
        }

        #endregion

        #region Category media

        protected void rptImages_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case DELETE:
                    CategoryService.DeleteCategoryMedia(Convert.ToInt32(e.CommandArgument));
                    enbNotice.Message = "Image was deleted successfully.";
                    break;
            }

            PopulateCategory(GetIntState(LAST_CHOSEN_CATEGORY));
            hfCurrentPanel.Value = "media";
        }

        protected void lbUploadImage_Click(object sender, EventArgs e)
        {
            if (fuImage.HasFile)
            {
                // Create new category media row to get id
                CategoryMedia newMedia = new CategoryMedia
                {
                    CategoryId = GetIntState(LAST_CHOSEN_CATEGORY)
                };

                // Save image
                string filename = DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(fuImage.FileName).ToLower();
                fuImage.SaveAs(MediaSettings.CategoryMediaLocalPath + filename);

                // Insert category media
                newMedia.MediaFilename = filename;
                CategoryService.InsertCategoryMedia(newMedia);

                PopulateCategory(GetIntState(LAST_CHOSEN_CATEGORY));

                hfCurrentPanel.Value = "media";

                enbNotice.Message = "Image was added successfully.";
            }
            else
                enbNotice.Message = "Sorry, there is no image to upload.";
        }

        #endregion

        #region Featured products

        #region Assigned items

        protected void gvFeaturedProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedFeaturedProduct();

            gvFeaturedProducts.CustomPageIndex = gvFeaturedProducts.CustomPageIndex + e.NewPageIndex;

            if (gvFeaturedProducts.CustomPageIndex < 0)
                gvFeaturedProducts.CustomPageIndex = 0;

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvFeaturedProducts, GetIntState(CATEGORY_ID), featuredItemType:Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));
            
            hfCurrentPanel.Value = "featProds";
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

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvFeaturedProducts, GetIntState(CATEGORY_ID), featuredItemType:Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featProds";
        }

        protected void gvFeaturedProducts_PreRender(object sender, EventArgs e)
        {
            if (gvFeaturedProducts.TopPagerRow != null)
                gvFeaturedProducts.TopPagerRow.Visible = true;
        }
        
        protected void gvFeaturedProducts_lbSearch_Click(object sender, EventArgs e)
        {
            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvFeaturedProducts, GetIntState(CATEGORY_ID), featuredItemType:Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featProds";
        }

        protected void gvFeaturedProducts_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvFeaturedProducts.TopPagerRow.FindControl("txtFeaturedProductPageIndex")).Text.Trim()) - 1;

            if ((gvFeaturedProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvFeaturedProducts.CustomPageIndex = gotoIndex;

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvFeaturedProducts, GetIntState(CATEGORY_ID), featuredItemType:Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue));

            hfCurrentPanel.Value = "featProds";
        }

        protected void gvFeaturedProducts_ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;

            ddl.SelectedIndex = -1;
            var featuredChosenItem = ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER_2));
            if (featuredChosenItem != null) featuredChosenItem.Selected = true;

            if (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY) ddl.Enabled = false;
            if (GetIntState(CATEGORY_ID_FILTER) != AppConstant.DEFAULT_CATEGORY_FILTER) ddl.Enabled = true;
        }

        protected void gvFeaturedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
            int featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);

            switch (e.CommandName)
            {
                case "savePriority":        
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        int priority = 0;
                        int.TryParse(((TextBox)row.FindControl("txtFeaturedProductPriority")).Text.Trim(), out priority);

                        CategoryService.UpdateCategoryFeaturedItemForPriority(productId, categoryId, featuredItemType, priority);
                        LoadFeaturedProducts(gvFeaturedProducts, categoryId, featuredItemType: featuredItemType);
                        
                        enbNotice.Message = "Priority for featured product was successfully updated.";
                    }
                    break;
                case "remove":
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        CategoryService.DeleteCategoryFeaturedItem(productId: productId, categoryId: categoryId, featuredItemType: featuredItemType);
                        LoadFeaturedProducts(gvFeaturedProducts, categoryId, featuredItemType: featuredItemType);                        
                        enbNotice.Message = "Featured item was successfully removed.";
                    }
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "featProds";
        }

        #endregion

        #region Search and assign

        protected void gvNewFeaturedProducts_PreRender(object sender, EventArgs e)
        {
            if (gvNewFeaturedProducts.TopPagerRow != null)
                gvNewFeaturedProducts.TopPagerRow.Visible = true;
        }

        protected void gvNewFeaturedProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNewFeaturedProducts.CustomPageIndex = gvNewFeaturedProducts.CustomPageIndex + e.NewPageIndex;

            if (gvNewFeaturedProducts.CustomPageIndex < 0)
                gvNewFeaturedProducts.CustomPageIndex = 0;

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvNewFeaturedProducts, GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "featProds";
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

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvNewFeaturedProducts, GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "featProds";
        }

        protected void gvNewFeaturedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "assign":
                    int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int productId = Convert.ToInt32(e.CommandArgument);
                        int featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);

                        var featuredItem = new CategoryFeaturedItem
                        {
                            CategoryId = categoryId,
                            ProductId = productId,
                            FeaturedItemType = featuredItemType
                        };

                        var result = CategoryService.ProcessCategoryFeaturedItemInsertion(featuredItem);
                        
                        if (string.IsNullOrEmpty(result))
                            enbNotice.Message = "Item was successfully assigned.";
                        else
                            enbNotice.Message = "Failed to assign the item. " + result;

                        LoadFeaturedProducts(gvFeaturedProducts, categoryId, featuredItemType: featuredItemType);
                        hfCurrentPanel.Value = "featProds";
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

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvNewFeaturedProducts, GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "featProds";
        }

        protected void lbSearchNewFeaturedProducts_Click(object sender, EventArgs e)
        {
            int productId = 0;
            int.TryParse(((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductId")).Text.Trim(), out productId);
            string name = ((TextBox)gvNewFeaturedProducts.HeaderRow.FindControl("txtNewFeaturedProductName")).Text.Trim();
            
            SetState("gvNewFeaturedProduct.Id", productId == 0 ? null : productId.ToString());
            SetState("gvNewFeaturedProduct.Name", name);
            
            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(
                    gvNewFeaturedProducts, 
                    GetIntState(CATEGORY_ID), 
                    productId: productId == 0 ? default(int?) : productId, 
                    name: name);

            hfCurrentPanel.Value = "featProds";
        }

        protected void lbResetNewFeaturedProducts_Click(object sender, EventArgs e)
        {
            DisposeState("gvNewFeaturedProduct.Id");
            DisposeState("gvNewFeaturedProduct.Name");

            if (HasState(CATEGORY_ID))
                LoadFeaturedProducts(gvNewFeaturedProducts, GetIntState(CATEGORY_ID));
            
            hfCurrentPanel.Value = "featProds";
        }
        
        #endregion

        private void LoadFeaturedProducts(CustomGrid cg, int categoryId, int? productId = null, string name = null, int? featuredItemType = null, int? notFeaturedItemType = null)
        {
            ProductSortingType orderBy = ProductSortingType.NameAsc;
            
            var result = ProductService.GetPagedProductOverviewModelsByCategoryHierarchy(
                pageIndex: cg.CustomPageIndex,
                pageSize: cg.PageSize,
                productId: productId,
                name: name,
                categoryId: categoryId,
                featuredItemType: featuredItemType,
                notFeaturedItemType: notFeaturedItemType,
                orderBy: orderBy);

            if (result != null)
            {
                cg.DataSource = result.Items;
                cg.RecordCount = result.TotalCount;
                cg.CustomPageCount = result.TotalPages;
            }

            cg.DataBind();

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
        
        protected string FindFeaturedItemPriority(int productId, int categoryId)
        {
            CategoryFeaturedItem matchingFeaturedItem = new CategoryFeaturedItem();
            Category category = CategoryService.GetActiveCategory(categoryId);
            if (category != null)
                matchingFeaturedItem = category.FeaturedItems.Where(x => x.ProductId == productId && x.CategoryId == categoryId).FirstOrDefault();
            return (matchingFeaturedItem != null) ? matchingFeaturedItem.Priority.ToString() : string.Empty;
        }

        protected bool ProductIsFeaturedInCategory(int productId, int categoryId)
        {
            var featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);
            Category category = CategoryService.GetActiveCategory(categoryId);
            if (category != null)
                return category.FeaturedItems.Where(x => x.ProductId == productId)
                    .Where(x => x.CategoryId == categoryId)
                    .Where(x => x.FeaturedItemType == featuredItemType)
                    .Count() > 0;
            else
                return false;
        }

        protected void lbGenerateFeaturedItems_Click(object sender, EventArgs e)
        {
            var categoryId = GetIntState(CATEGORY_ID);
            var featuredItemType = Convert.ToInt32(ddlFeaturedItemType.SelectedValue);
            var quantity = Convert.ToInt32(ddlFeaturedQuantity.SelectedValue);

            ProductService.AutoGenerateFeaturedItemsByCategory(categoryId, featuredItemType, quantity);

            enbNotice.Message = string.Format("Featured items for '{0}' was auto-populated successfully.", ddlFeaturedItemType.SelectedItem.Text);

            ddlFeaturedItemTypeFilter.SelectedIndex = -1;
            var item = ddlFeaturedItemTypeFilter.Items.FindByValue(featuredItemType.ToString());
            item.Selected = true;

            LoadFeaturedProducts(gvFeaturedProducts, categoryId, featuredItemType:featuredItemType);

            hfCurrentPanel.Value = "featProds";
        }
        
        #endregion

        #region Featured brands

        protected void gvFeaturedBrands_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedBrand();

            gvFeaturedBrands.CustomPageIndex = gvFeaturedBrands.CustomPageIndex + e.NewPageIndex;

            if (gvFeaturedBrands.CustomPageIndex < 0)
                gvFeaturedBrands.CustomPageIndex = 0;

            if (HasState(CATEGORY_ID))
                LoadFeaturedBrands(GetIntState(CATEGORY_ID));
           
            hfCurrentPanel.Value = "featBrands";
        }

        protected void gvFeaturedBrands_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var brand = e.Row.DataItem as BrandOverviewModel;
                var cb = e.Row.FindControl("cbFeaturedBrandChosen") as CheckBox;

                if (ChosenBrandsFeatured.Contains(brand.Id))
                    cb.Checked = true;

                SetChosenFeaturedBrands(brand.Id, cb.Checked);
            }
        }

        protected void gvFeaturedBrands_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = BrandSortingType.NameAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = BrandSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BrandSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = BrandSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BrandSortingType.NameAsc;
                    break;
                default:
                    break;
            }

            SetState("FeaturedBrand-OrderBy", (int)orderBy);

            if (HasState(CATEGORY_ID))
                LoadFeaturedBrands(GetIntState(CATEGORY_ID));
            
            hfCurrentPanel.Value = "featBrands";
        }

        protected void gvFeaturedBrands_PreRender(object sender, EventArgs e)
        {
            if (gvFeaturedBrands.TopPagerRow != null)
                gvFeaturedBrands.TopPagerRow.Visible = true;
        }

        protected void gvFeaturedBrands_lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(BRAND_ID_FILTER);
            DisposeState(NAME_FILTER_3);
            SetState(CHOSEN_FILTER_3, (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY ? ANY : YES));
            
            LoadFeaturedBrands(GetIntState(CATEGORY_ID));
            
            hfCurrentPanel.Value = "featBrands";
        }

        protected void gvFeaturedBrands_lbSaveProducts_Click(object sender, EventArgs e)
        {
            SaveLastViewedBrand();

            int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

            if (NotChosenBrandsFeatured.Count > 0)
                BrandService.DeleteFeaturedBrandListInCategory(categoryId, NotChosenBrandsFeatured);

            if (ChosenBrandsFeatured.Count > 0)
                BrandService.ProcessNewBrandFeaturedInCategory(categoryId, ChosenBrandsFeatured.ToArray());

            PopulateCategory(GetIntState(LAST_CHOSEN_CATEGORY));

            hfCurrentPanel.Value = "featBrands";

            enbNotice.Message = "Featured brands were updated successfully.";
        }

        protected void gvFeaturedBrands_lbSearch_Click(object sender, EventArgs e)
        {
            SetState(BRAND_ID_FILTER, ((TextBox)gvFeaturedBrands.HeaderRow.FindControl("txtFeaturedBrandFilterId")).Text.Trim());
            SetState(NAME_FILTER_3, ((TextBox)gvFeaturedBrands.HeaderRow.FindControl("txtFeaturedBrandFilterName")).Text.Trim());
            SetState(CHOSEN_FILTER_3, ((DropDownList)gvFeaturedBrands.HeaderRow.FindControl("ddlFeaturedBrandFilterChosen")).SelectedValue);

            if (HasState(CATEGORY_ID))
                LoadFeaturedBrands(GetIntState(CATEGORY_ID));
            
            hfCurrentPanel.Value = "featBrands";
        }

        protected void gvFeaturedBrands_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvFeaturedBrands.TopPagerRow.FindControl("txtFeaturedBrandPageIndex")).Text.Trim()) - 1;

            if ((gvFeaturedBrands.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvFeaturedBrands.CustomPageIndex = gotoIndex;

            if (HasState(CATEGORY_ID))
                LoadFeaturedBrands(GetIntState(CATEGORY_ID));

            hfCurrentPanel.Value = "featBrands";
        }

        protected void gvFeaturedBrands_ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;            
            ddl.SelectedIndex = -1;
            var featuredChosenItem = ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER_3));
            if (featuredChosenItem != null) featuredChosenItem.Selected = true;

            if (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY) ddl.Enabled = false;
            if (GetIntState(CATEGORY_ID_FILTER) != AppConstant.DEFAULT_CATEGORY_FILTER) ddl.Enabled = true;
        }

        protected void gvFeaturedBrands_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "savePriority":
                    int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int categoryFeaturedBrandId = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        int priority = Convert.ToInt32(((TextBox)row.FindControl("txtFeaturedBrandPriority")).Text.Trim());

                        CategoryService.UpdateCategoryFeaturedBrandForPriorityById(categoryFeaturedBrandId, priority);

                        LoadFeaturedBrands(categoryId);
                        hfCurrentPanel.Value = "featBrands";
                        enbNotice.Message = "Priority for featured brand was successfully updated.";
                    }
                    break;
                default:
                    break;
            }
        }

        protected string FindFeaturedBrandPriority(int brandId, int categoryId)
        {
            CategoryFeaturedBrand matchingFeaturedBrand = new CategoryFeaturedBrand();
            Category category = CategoryService.GetActiveCategory(categoryId);
            if (category != null)
                matchingFeaturedBrand = category.FeaturedBrands.Where(x => x.BrandId == brandId && x.CategoryId == categoryId).FirstOrDefault();
            return (matchingFeaturedBrand != null) ? matchingFeaturedBrand.Priority.ToString() : string.Empty;
        }

        private void LoadFeaturedBrands(int categoryId)
        {
            //if (Page.IsPostBack
            //    && HasState(CATEGORY_ID)
            //    && GetIntState(CATEGORY_ID) != AppConstant.DEFAULT_CATEGORY)
            //{
            string name = null;
            int[] brandIds = null;
            bool? isCategoryFeaturedBrand = null;
            BrandSortingType orderBy = BrandSortingType.IdDesc;

            if (HasState(NAME_FILTER_3)) name = GetStringState(NAME_FILTER_3);
            if (HasState(BRAND_ID_FILTER))
            {
                string value = GetStringState(BRAND_ID_FILTER);
                int temp;
                brandIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (GetStringState(CHOSEN_FILTER_3) == YES) isCategoryFeaturedBrand = true;
            if (GetStringState(CHOSEN_FILTER_3) == NO) isCategoryFeaturedBrand = false;
            if (HasState("FeaturedBrand-OrderBy")) orderBy = (BrandSortingType)GetIntState("FeaturedBrand-OrderBy");

            var result = BrandService.GetPagedBrandOverviewModel(
                pageIndex: gvFeaturedBrands.CustomPageIndex,
                pageSize: gvFeaturedBrands.PageSize,
                brandIds: brandIds,
                name: name,
                isCategoryFeaturedBrand: isCategoryFeaturedBrand,
                categoryId: categoryId,
                orderBy: orderBy);

            if (result != null)
            {
                gvFeaturedBrands.DataSource = result.Items;
                gvFeaturedBrands.RecordCount = result.TotalCount;
                gvFeaturedBrands.CustomPageCount = result.TotalPages;
            }

            gvFeaturedBrands.DataBind();
            //}            
        }

        private void SetChosenFeaturedBrands(int brandId, bool chosen)
        {
            if (brandId != 0)
            {
                if ((chosen) && !ChosenBrandsFeatured.Contains(brandId))
                {
                    ChosenBrandsFeatured.Add(brandId);
                    NotChosenBrandsFeatured.Remove(brandId);
                }
                else if ((!chosen) && (ChosenBrandsFeatured.Contains(brandId)))
                {
                    ChosenBrandsFeatured.Remove(brandId);
                    NotChosenBrandsFeatured.Add(brandId);
                }
            }
        }

        private void SaveLastViewedBrand()
        {
            int brandId;

            for (int i = 0; i < gvFeaturedBrands.DataKeys.Count; i++)
            {
                CheckBox cb = gvFeaturedBrands.Rows[i].FindControl("cbFeaturedBrandChosen") as CheckBox;
                brandId = Convert.ToInt32(gvFeaturedBrands.DataKeys[i].Value);

                if (cb != null) SetChosenFeaturedBrands(brandId, cb.Checked);
            }
        }

        #endregion

        #region Category filters

        protected void gvFiltersProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFiltersProducts.CustomPageIndex = gvFiltersProducts.CustomPageIndex + e.NewPageIndex;

            if (gvFiltersProducts.CustomPageIndex < 0)
                gvFiltersProducts.CustomPageIndex = 0;

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadCategoryFilterProducts(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));
            
            hfCurrentPanel.Value = "filters";
        }

        protected void gvFiltersProducts_Sorting(object sender, GridViewSortEventArgs e)
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

            SetState("FiltersProduct-OrderBy", (int)orderBy);

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadCategoryFilterProducts(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvFiltersProducts_PreRender(object sender, EventArgs e)
        {
            if (gvFiltersProducts.TopPagerRow != null)
                gvFiltersProducts.TopPagerRow.Visible = true;
        }

        protected void gvFiltersProducts_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvFiltersProducts.TopPagerRow.FindControl("txtFiltersProductPageIndex")).Text.Trim()) - 1;

            if ((gvFiltersProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvFiltersProducts.CustomPageIndex = gotoIndex;

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadCategoryFilterProducts(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvFiltersProducts_lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER_3, ((TextBox)gvFiltersProducts.HeaderRow.FindControl("txtFiltersProductFilterId")).Text.Trim());
            SetState(NAME_FILTER_3, ((TextBox)gvFiltersProducts.HeaderRow.FindControl("txtFiltersProductFilterName")).Text.Trim());

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadCategoryFilterProducts(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvFiltersProducts_lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER_3);
            DisposeState(NAME_FILTER_3);

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadCategoryFilterProducts(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvFiltersProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    var productId = Convert.ToInt32(e.CommandArgument);
                    var categoryFilterId = Convert.ToInt32(hfCategoryFilterIdForProducts.Value);

                    CategoryService.DeleteCategoryFilterByProductIdList(categoryFilterId, new List<int> { productId });

                    int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

                    PopulateCategory(categoryId);

                    LoadCategoryFilterProducts(categoryId, categoryFilterId);
                    LoadNonAssignedProductsForFilter(categoryId, categoryFilterId);

                    enbNotice.Message = "Product category filter was removed successfully.";
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "filters";
        }

        protected void lbSaveCategoryFilter_Click(object sender, EventArgs e)
        {
            int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
            var filter = new CategoryFilter
            {
                CategoryId = categoryId,
                Type = txtCategoryFilterType.Text.Trim()
            };

            txtCategoryFilterType.Text = string.Empty;

            CategoryService.InsertCategoryFilter(filter);
            PopulateCategory(categoryId);

            hfCurrentPanel.Value = "filters";

            enbNotice.Message = "Category filter was saved successfully.";
        }

        protected void lbUpdateCategoryFilter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfCategoryFilterId.Value))
            {
                int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
                int filterId = 0;

                if (int.TryParse(hfCategoryFilterId.Value, out filterId))
                {
                    CategoryService.UpdateCategoryFilterType(txtEditCategoryFilterType.Text.Trim(), filterId);
                    enbNotice.Message = "Category filter was updated successfully.";
                    PopulateCategory(categoryId);
                    phEditCategoryFilter.Visible = false;
                    phNewCategoryFilter.Visible = true;
                }
            }

            hfCurrentPanel.Value = "filters";
        }

        protected void lbDeleteCategoryFilter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfCategoryFilterId.Value))
            {
                int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
                int filterId = 0;

                if (int.TryParse(hfCategoryFilterId.Value, out filterId))
                {
                    CategoryService.ProcessCategoryFilterRemoval(filterId);
                    enbNotice.Message = "Category filter was deleted successfully.";
                    PopulateCategory(categoryId);
                    phEditCategoryFilter.Visible = false;
                    phNewCategoryFilter.Visible = true;
                }
            }

            hfCurrentPanel.Value = "filters";
        }

        protected void rptFilters_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int id = 0;
            var categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
            switch (e.CommandName)
            {
                case "edit":
                    id = Convert.ToInt32(e.CommandArgument);
                    ltEditCategoryFilterTitle.Text = string.Format("Category Filter (ID: {0}", id);
                    hfCategoryFilterId.Value = id.ToString();
                    var filter = CategoryService.GetCategoryFilterById(id);
                    txtEditCategoryFilterType.Text = filter.Type;

                    phNewCategoryFilter.Visible = false;
                    phEditCategoryFilter.Visible = true;
                    break;
                case "remove":
                    id = Convert.ToInt32(e.CommandArgument);
                    CategoryService.ProcessCategoryFilterRemoval(id);

                    PopulateCategory(categoryId);
                    
                    phNewCategoryFilter.Visible = true;
                    phEditCategoryFilter.Visible = false;
                    phFilterProducts.Visible = false;
                    enbNotice.Message = "Category filter was deleted successfully.";
                    break;
                case "list":
                default:
                    id = Convert.ToInt32(e.CommandArgument);

                    ltProductFilterTitle.Text = string.Format("(Category Filter ID: {0})", id);
                    ltAssignCatagoryFilterTitle.Text = string.Format("(ID: {0})", id);

                    LoadCategoryFilterProducts(categoryId, id);
                    LoadNonAssignedProductsForFilter(categoryId, id);

                    hfCategoryFilterIdForProducts.Value = id.ToString();
                    phFilterProducts.Visible = true;
                    break;
            }

            hfCurrentPanel.Value = "filters";
        }

        protected void gvNotFilterProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNotFilterProducts.CustomPageIndex = gvNotFilterProducts.CustomPageIndex + e.NewPageIndex;

            if (gvNotFilterProducts.CustomPageIndex < 0)
                gvNotFilterProducts.CustomPageIndex = 0;

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadNonAssignedProductsForFilter(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvNotFilterProducts_Sorting(object sender, GridViewSortEventArgs e)
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

            SetState("NotFilterProducts-OrderBy", (int)orderBy);

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadNonAssignedProductsForFilter(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }
        
        protected void gvNotFilterProducts_PreRender(object sender, EventArgs e)
        {
            if (gvNotFilterProducts.TopPagerRow != null)
                gvNotFilterProducts.TopPagerRow.Visible = true;
        }
        
        protected void gvNotFilterProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "assign":                
                    var productId = Convert.ToInt32(e.CommandArgument);
                    var categoryFilterId = Convert.ToInt32(hfCategoryFilterIdForProducts.Value);
                    CategoryService.ProcessProductCategoryFilterInsertion(categoryFilterId, new List<int> { productId });
                    
                    int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

                    LoadCategoryFilterProducts(categoryId, categoryFilterId);
                    LoadNonAssignedProductsForFilter(categoryId, categoryFilterId);

                    enbNotice.Message = "Product category filter was assigned successfully.";
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "filters";
        }

        protected void gvNotFilterProducts_btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvNotFilterProducts.TopPagerRow.FindControl("gvNotFilterProducts_txtPageIndex")).Text.Trim()) - 1;

            if ((gvNotFilterProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvNotFilterProducts.CustomPageIndex = gotoIndex;

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadNonAssignedProductsForFilter(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvNotFilterProducts_lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER_4, ((TextBox)gvFiltersProducts.HeaderRow.FindControl("txtNotFilterProductId")).Text.Trim());
            SetState(NAME_FILTER_4, ((TextBox)gvFiltersProducts.HeaderRow.FindControl("txtNotFilterName")).Text.Trim());

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadNonAssignedProductsForFilter(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        protected void gvNotFilterProducts_lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER_4);
            DisposeState(NAME_FILTER_4);

            if (!string.IsNullOrEmpty(hfCategoryFilterIdForProducts.Value) && HasState(LAST_CHOSEN_CATEGORY))
                LoadNonAssignedProductsForFilter(GetIntState(LAST_CHOSEN_CATEGORY), Convert.ToInt32(hfCategoryFilterIdForProducts.Value));

            hfCurrentPanel.Value = "filters";
        }

        private void LoadCategoryFilterProducts(int categoryId, int categoryFilterId)
        {
            int[] productIds = null;
            string name = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState(NAME_FILTER_3)) name = GetStringState(NAME_FILTER_3);
            if (HasState(PRODUCT_ID_FILTER_3))
            {
                string value = GetStringState(PRODUCT_ID_FILTER_3);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState("FiltersProduct-OrderBy")) orderBy = (ProductSortingType)GetIntState("FiltersProduct-OrderBy");

            var result = ProductService.GetPagedProductOverviewModelsByCategoryFilter(
                categoryFilterId: categoryFilterId,
                pageIndex: gvFiltersProducts.CustomPageIndex,
                pageSize: gvFiltersProducts.PageSize,
                productIds: productIds,
                name: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvFiltersProducts.DataSource = result.Items;
                gvFiltersProducts.RecordCount = result.TotalCount;
                gvFiltersProducts.CustomPageCount = result.TotalPages;
            }

            gvFiltersProducts.DataBind();
        }

        private void LoadNonAssignedProductsForFilter(int categoryId, int categoryFilterId)
        {
            int[] categoryIds = new int[] { categoryId };
            string name = null;
            int[] productIds = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState(NAME_FILTER_4)) name = GetStringState(NAME_FILTER_4);
            if (HasState(PRODUCT_ID_FILTER_4))
            {
                string value = GetStringState(PRODUCT_ID_FILTER_4);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState("NotFilterProducts-OrderBy")) orderBy = (ProductSortingType)GetIntState("NotFilterProducts-OrderBy");

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvNotFilterProducts.CustomPageIndex,
                pageSize: gvNotFilterProducts.PageSize,
                categoryIds: categoryIds,
                productIds: productIds,
                keywords: name,
                notCategoryFilterId: categoryFilterId,
                orderBy: orderBy);

            if (result != null)
            {
                gvNotFilterProducts.DataSource = result.Items;
                gvNotFilterProducts.RecordCount = result.TotalCount;
                gvNotFilterProducts.CustomPageCount = result.TotalPages;
            }

            gvNotFilterProducts.DataBind();
        }

        #endregion

        #region Category What's New

        protected void lbSaveWhatsNew_Click(object sender, EventArgs e)
        {
            var categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

            var priority = 0;
            int.TryParse(txtWhatsNewPriority.Text.Trim(), out priority);

            var whatsNew = new CategoryWhatsNew
            {
                CategoryId = categoryId,
                Enabled = cbWhatsNewEnabled.Checked,
                HtmlContent = txtWhatsNewContent.Text.Trim(),
                StartDate = string.IsNullOrEmpty(txtWhatsNewDateFrom.Text.Trim()) ? default(DateTime?) : DateTime.ParseExact(txtWhatsNewDateFrom.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = string.IsNullOrEmpty(txtWhatsNewDateTo.Text.Trim()) ? default(DateTime?) : DateTime.ParseExact(txtWhatsNewDateTo.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Priority = priority
            };

            CategoryService.InsertCategoryWhatsNew(whatsNew);

            hfWhatsNewId.Value = string.Empty;
            txtWhatsNewContent.Text = string.Empty;
            txtWhatsNewDateFrom.Text = string.Empty;
            txtWhatsNewDateTo.Text = string.Empty;
            cbWhatsNewEnabled.Checked = false;
            txtWhatsNewPriority.Text = "0";

            lbUpdateWhatsNew.Visible = false;

            PopulateCategory(categoryId);
            
            hfCurrentPanel.Value = "whatsnew";
        }

        protected void lbUpdateWhatsNew_Click(object sender, EventArgs e)
        {
            var categoryId = GetIntState(LAST_CHOSEN_CATEGORY);

            var priority = 0;
            int.TryParse(txtWhatsNewPriority.Text.Trim(), out priority);

            var whatsNew = new CategoryWhatsNew
            {
                Id = Convert.ToInt32(hfWhatsNewId.Value),
                CategoryId = categoryId,
                Enabled = cbWhatsNewEnabled.Checked,
                HtmlContent = txtWhatsNewContent.Text.Trim(),
                StartDate = string.IsNullOrEmpty(txtWhatsNewDateFrom.Text.Trim()) ? default(DateTime?) : DateTime.ParseExact(txtWhatsNewDateFrom.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = string.IsNullOrEmpty(txtWhatsNewDateTo.Text.Trim()) ? default(DateTime?) : DateTime.ParseExact(txtWhatsNewDateTo.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Priority = priority
            };

            CategoryService.UpdateCategoryWhatsNew(whatsNew);

            ltWhatsNewTitle.Text = "New Item";
            hfWhatsNewId.Value = string.Empty;
            txtWhatsNewContent.Text = string.Empty;
            txtWhatsNewDateFrom.Text = string.Empty;
            txtWhatsNewDateTo.Text = string.Empty;
            cbWhatsNewEnabled.Checked = false;
            txtWhatsNewPriority.Text = "0";

            lbSaveWhatsNew.Visible = true;
            lbUpdateWhatsNew.Visible = false;
            
            PopulateCategory(categoryId);

            enbNotice.Message = "What's New item was updated successfully.";

            hfCurrentPanel.Value = "whatsnew";
        }

        protected void rptWhatsNewItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    CategoryService.DeleteCategoryWhatsNew(Convert.ToInt32(e.CommandArgument));
                    enbNotice.Message = "What's New item was deleted successfully.";
                    break;
                case "edit":
                    var whatsNew = CategoryService.GetCategoryWhatsNewById(Convert.ToInt32(e.CommandArgument));

                    if (whatsNew != null)
                    {
                        hfWhatsNewId.Value = whatsNew.Id.ToString();
                        ltWhatsNewTitle.Text = string.Format("What's New (ID : {0})", whatsNew.Id);
                        txtWhatsNewContent.Text = whatsNew.HtmlContent;
                        txtWhatsNewDateFrom.Text = whatsNew.StartDate.HasValue ? whatsNew.StartDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;
                        txtWhatsNewDateTo.Text = whatsNew.EndDate.HasValue ? whatsNew.EndDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;
                        cbWhatsNewEnabled.Checked = whatsNew.Enabled;
                        txtWhatsNewPriority.Text = whatsNew.Priority.ToString();
                        lbSaveWhatsNew.Visible = false;
                        lbUpdateWhatsNew.Visible = true;
                    }
                    else
                        enbNotice.Message = "Sorry, the what's new item was not found.";
                    break;
            }

            PopulateCategory(GetIntState(LAST_CHOSEN_CATEGORY));
            hfCurrentPanel.Value = "whatsnew";
        }

        #endregion

        #region Metadata

        protected void lbAddMeta_Click(object sender, EventArgs e)
        {
            var category = CategoryService.GetCategory(GetIntState(LAST_CHOSEN_CATEGORY));

            if (category == null)
            {
                enbNotice.Message = "Sorry, category could not be loaded.";
                return;
            }

            category.MetaTitle = txtMetaTitle.Text.Trim();
            category.MetaKeywords = txtMetaKeywords.Text.Trim();
            category.MetaDescription = txtMetaDescription.Text.Trim();
            category.SecondaryKeywords = txtSecondaryKeywords.Text.Trim();

            CategoryService.UpdateCategory(category);

            enbNotice.Message = "Category meta was updated successfully.";

            PopulateCategory(category.Id);
            hfCurrentPanel.Value = "meta";
        }
        
        #endregion

        #region Banners

        protected void gvBanners_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = GetIntState(LAST_CHOSEN_CATEGORY);
            
            switch (e.CommandName)
            {
                case "save":
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int mappingId = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                        int displayOrder = Convert.ToInt32(((TextBox)row.FindControl("txtBannerDisplayOrder")).Text.Trim());

                        CategoryService.UpdateCategoryLargeBannerForDisplayOrder(mappingId, displayOrder);

                        enbNotice.Message = "Display order was successfully updated.";
                        PopulateCategory(categoryId);
                    }
                    break;
                case "remove":
                    if (categoryId != AppConstant.DEFAULT_CATEGORY)
                    {
                        int largeBannerId = Convert.ToInt32(e.CommandArgument);
                        CategoryService.DeleteCategoryFromLargeBanner(categoryId, largeBannerId);                        
                        
                        enbNotice.Message = "Banner was successfully removed.";
                        PopulateCategory(categoryId);
                    }
                    break;
                default:
                    break;
            }

            hfCurrentPanel.Value = "banners";
        }

        #endregion

        protected bool ProductHasThisCategory(int productId, int categoryId, int categoryFilterId)
        {
            var found = CategoryService.ProductHasThisCategory(productId, categoryId, categoryFilterId);
            return found;
        }

        private int GenerateTreeLevel(int categoryId)
        {
            int treeLevel = 1;
            if (categoryId > 0)
            {
                int level = CategoryService.GetCategoryTreeLevel(categoryId);
                treeLevel = level + 1;
            }
            return treeLevel;
        }

        private void ResetScreen()
        {
            SetState("sortdirection", string.Empty);
            ClearUp();
            SetState(MODE, NEW);
            hfParent.Value = string.Empty;
            ltlParent.Text = string.Empty;
            gvProducts.DataBind();
            gvFeaturedProducts.DataBind();
            gvFeaturedBrands.DataBind();
        }

        private void ClearUp()
        {
            txtName.Text = string.Empty;
            txtDesc.Text = string.Empty;
            txtShortDesc.Text = string.Empty;
            txtMetaTitle.Text = string.Empty;
            txtMetaDescription.Text = string.Empty;
            txtMetaKeywords.Text = string.Empty;
            txtSecondaryKeywords.Text = string.Empty;
            txtH1Title.Text = string.Empty;
            txtUrlKey.Text = string.Empty;
            txtPriority.Text = "0";
            cbRemoveThumb.Checked = false;
            ddlColourScheme.SelectedIndex = -1;
            phFilters.Visible = false;
            phFeatureProducts.Visible = false;
            phFeatureProducts.Visible = false;
            phCategoryProducts.Visible = false;
            imgThumbnail.Src = string.Empty;
            ChosenProducts.Clear();
            NotChosenProducts.Clear();
            ChosenProductsFeatured.Clear();
            NotChosenProductsFeatured.Clear();
            ChosenBrandsFeatured.Clear();
            NotChosenBrandsFeatured.Clear();
        }

        private void PopulateCategory(int categoryId, bool firstTimeLoading = false)
        {
            var category = CategoryService.GetCategory(categoryId);

            if (category == null)
                Response.Redirect("/catalog/category_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CategoryNotFound);

            #region General information

            txtName.Text = Server.HtmlDecode(category.CategoryName);
            txtDesc.Text = category.Description;
            txtShortDesc.Text = category.ShortDescription;            
            txtH1Title.Text = Server.HtmlDecode(category.H1Title);
            txtUrlKey.Text = category.UrlRewrite;
            txtPriority.Text = Convert.ToString(category.Priority);

            ddlColourScheme.SelectedIndex = -1;
            if (ddlColourScheme.Items.FindByValue(category.ColourScheme) != null)
                ddlColourScheme.Items.FindByValue(category.ColourScheme).Selected = true;

            hfParent.Value = category.ParentId == 0 ? string.Empty : category.ParentId.ToString();
            cbVisible.Checked = category.Visible;

            // Only category level 1 could have category template
            phCategoryTemplate.Visible = category.TreeLevel == 1;

            if (category.ParentId != 0)
            {
                Category parentCategory = CategoryService.GetCategory(Convert.ToInt32(category.ParentId));
                if (parentCategory != null) ltlParent.Text = parentCategory.CategoryName;
            }
            
            if (category.ThumbnailFilename == string.Empty)
            {
                imgThumbnail.Visible = false;
                cbRemoveThumb.Visible = false;
                fuThumbnail.Visible = true;
            }
            else
            {
                imgThumbnail.Src = "/get_image_handler.aspx?type=category&img=" + category.ThumbnailFilename;
                imgThumbnail.Visible = true;
                cbRemoveThumb.Visible = true;
                fuThumbnail.Visible = false;
            }

            ddlCategoryTemplate.SelectedIndex = -1;
            var template = ddlCategoryTemplate.Items.FindByValue(category.CategoryTemplateId.ToString());
            if (template != null) template.Selected = true;

            #endregion

            #region Category products

            phCategoryProducts.Visible = category.TreeLevel == 3;
            // Only category level 3 could have assigned products
            if (category.TreeLevel == 3)
                LoadProducts(categoryId, firstTimeLoading);

            #endregion

            #region Category media

            rptImages.DataSource = category.CategoryMedias;
            rptImages.DataBind();

            #endregion

            #region Category filters

            // Filters are only for category level 3
            phFilters.Visible = category.TreeLevel == 3;
            if (category.TreeLevel == 3)
            {
                var filters = category.CategoryFilters;
                phCategoryFilterProducts.Visible = filters.Count > 0;

                rptFilters.DataSource = category.CategoryFilters;
                rptFilters.DataBind();
            }

            #endregion

            #region Featured items

            // Only category level 1 could have featured products
            // Only category level 2 could have featured items (product & brand)
            phFeatureProducts.Visible = category.TreeLevel == 1 || category.TreeLevel == 2;
            phFeatureBrands.Visible = category.TreeLevel == 2;

            if (category.TreeLevel == 1 || category.TreeLevel == 2)
            {
                var featuredItemType = Convert.ToInt32(ddlFeaturedItemTypeFilter.SelectedValue);
                LoadFeaturedProducts(gvFeaturedProducts, categoryId, featuredItemType: featuredItemType);
                LoadFeaturedProducts(gvNewFeaturedProducts, categoryId, notFeaturedItemType: featuredItemType);
            }

            if (category.TreeLevel == 2)
            {
                LoadFeaturedBrands(categoryId);
            }

            #endregion

            #region What's new items

            // Only category level 2 could have What's New items
            phWhatsNew.Visible = category.TreeLevel == 2;
            if (category.TreeLevel == 2)
            {
                rptWhatsNewItems.DataSource = category.CategoryWhatsNews;
                rptWhatsNewItems.DataBind();
            }

            #endregion

            #region Metadata

            txtMetaTitle.Text = category.MetaTitle;
            txtMetaDescription.Text = category.MetaDescription;
            txtMetaKeywords.Text = category.MetaKeywords;
            txtSecondaryKeywords.Text = category.SecondaryKeywords;
            
            #endregion

            #region Banners

            phBanners.Visible = category.TreeLevel == 1;
            if (category.TreeLevel == 1)
            {
                gvBanners.DataSource = category.Banners;
                gvBanners.DataBind();
            }

            #endregion

            SetState(CATEGORY_ID, categoryId);

            if (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY)
            {
                SetState(MODE, NEW);
                SetState(CHOSEN_FILTER, ANY);
                SetState(CHOSEN_FILTER_2, ANY);
                SetState(CHOSEN_FILTER_3, ANY);
            }
            else
            {
                SetState(MODE, EDIT);
                SetState(CHOSEN_FILTER, YES);
                if (category.FeaturedItems.Count > 0)
                    SetState(CHOSEN_FILTER_2, YES);
                else
                    SetState(CHOSEN_FILTER_2, ANY);
                if (category.FeaturedBrands.Count > 0)
                    SetState(CHOSEN_FILTER_3, YES);
                else
                    SetState(CHOSEN_FILTER_3, ANY);
            }

            hfCurrentPanel.Value = "general";
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
    }
}