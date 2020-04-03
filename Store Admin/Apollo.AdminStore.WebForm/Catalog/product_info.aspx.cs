using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class product_info : BasePage, ICallbackEventHandler
    {
        private const int MAX_LENGTH_META_DESCRIPTION = 160;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(product_info).FullName);

        public IOrderService OrderService { get; set; }
        public IBrandService BrandService { get; set; }
        public ICategoryService CategoryService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public IProductService ProductService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public ImageUtility ImageUtility { get; set; }        
        public CurrencySettings CurrencySettings { get; set; }

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

            #region Tags

            ddlTag.DataTextField = "Name";
            ddlTag.DataValueField = "Id";
            ddlTag.DataSource = ProductService.GetTagList();
            ddlTag.DataBind();

            ddlTag.Attributes.Add("onchange", "show_processing('divTag');" + ClientScript.GetCallbackEventReference(this, "'tag_" + QueryProductId.ToString() + "_' + this.options[this.selectedIndex].value", "selectTag", "'context'"));

            #endregion

            #region Restricted Group

            ddlRestrictedGroups.DataSource = ProductService.GetRestrictedGroups();
            ddlRestrictedGroups.DataBind();

            #endregion

            #region Tax Category

            ddlTaxCategory.DataTextField = "Name";
            ddlTaxCategory.DataValueField = "Id";
            ddlTaxCategory.DataSource = ProductService.GetTaxCategories();
            ddlTaxCategory.DataBind();

            #endregion

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadProductInfo();
        }

        protected void rptPrices_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int productPriceId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case EDIT:
                    var product = ProductService.GetProductById(QueryProductId);
                    var productPrice = product.ProductPrices.Where(pp => pp.Id == productPriceId).FirstOrDefault();

                    hfProductPriceId.Value = e.CommandArgument.ToString();
                    ltlPriceTitle.Text = "Price ID: " + e.CommandArgument;
                    txtPrice.Text = productPrice.Price.ToString("#.##");

                    switch ((OptionType)product.OptionType)
                    {
                        case OptionType.None:
                            mvOption.ActiveViewIndex = 0;
                            break;
                        case OptionType.Size:
                            txtSize.Text = productPrice.Size;
                            mvOption.ActiveViewIndex = 1;
                            break;
                        case OptionType.Colour:
                            if (productPrice.ColourId.HasValue)
                            {
                                txtColourId.Text = productPrice.ColourId.ToString();
                                var colour = ProductService.GetColour(productPrice.ColourId.Value);

                                if (colour != null)
                                {
                                    ltlColourOptionName.Text = colour.Value;
                                    imgColourOption.ImageUrl = string.Format("/get_image_handler.aspx?type={0}&img={1}", "colour", colour.ThumbnailFilename);
                                }
                            }
                            
                            mvOption.ActiveViewIndex = 2;
                            break;
                        case OptionType.GiftCard:
                        default:
                            mvOption.ActiveViewIndex = 0;
                            break;
                    }

                    lbRemoveAssociatedImage.Visible = false;
                    lbChangeAssociatedImage.Visible = false;

                    if (productPrice.ProductMediaId.HasValue)
                    {
                        txtProductMediaIdForOption.Text = productPrice.ProductMediaId.Value.ToString();
                        var picture = ProductService.GetProductMedia(productPrice.ProductMediaId.Value);

                        if (picture != null)
                        {
                            imgAssociatedPicture.ImageUrl = string.Format("/get_image_handler.aspx?type={0}&img={1}", "media", picture.ThumbnailFilename);
                            lbChangeAssociatedImage.Visible = true;
                            lbRemoveAssociatedImage.Visible = true;
                        }
                        else
                        {
                            lbChangeAssociatedImage.Visible = true;
                        }
                    }
                    else
                        lbChangeAssociatedImage.Visible = true;

                    txtPriceCode.Text = productPrice.PriceCode;
                    txtStock.Text = productPrice.Stock.ToString();
                    txtBarcode.Text = productPrice.Barcode;
                    txtPrice.Text = productPrice.Price.ToString();
                    txtWeight.Text = productPrice.Weight.ToString();
                    cbPriceStatus.Checked = productPrice.Enabled;
                    txtAdditionalShippingCost.Text = productPrice.AdditionalShippingCost.ToString("0.00");
                    txtPriority.Text = productPrice.Priority.ToString();
                    txtMaximumAllowedPurchaseQuantity.Text = productPrice.MaximumAllowedPurchaseQuantity.HasValue ? productPrice.MaximumAllowedPurchaseQuantity.Value.ToString() : string.Empty;
                    cbDisableStockSync.Checked = productPrice.DisableStockSync;

                    lbDeletePrice.Visible = true;
                    lbSavePrice.Visible = true;
                    lbAddNewPrice.Visible = false;

                    enbNotice.Message = string.Empty;
                    break;

                case DELETE:
                    var lineItems = OrderService.GetLineItemsByProductPrice(productPriceId);
                    if (lineItems.Count > 0)
                    {
                        enbNotice.Message = "Sorry, the price could not be deleted as there are some associated line items.";
                    }
                    else
                    {
                        ProductService.DeleteProductPrice(productPriceId);
                        enbNotice.Message = "Price was deleted successfully.";
                    }
                    ProductService.DeleteProductPrice(productPriceId);
                    LoadProductInfo();
                    
                    break;
            }

            hfCurrentPanel.Value = "prices";
        }

        protected void rptImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = e.Item.FindControl("rbPrimary") as RadioButton;
                string mediaId = ((ProductMedia)e.Item.DataItem).Id.ToString();
                rb.Attributes.Add("onclick", "set_primary_image(this);" +
                                             "$('.image_box_" + mediaId + " label').html('<i class=\"fa fa-spinner fa-spin\"></i>');" +
                                             ClientScript.GetCallbackEventReference(this, "'image_" + QueryProductId.ToString() +
                                             "_" + mediaId + "'", "choose_image", "'" + mediaId + "'"));
            }
        }

        protected void rptImages_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case DELETE:
                    ProductService.DeleteProductMedia(Convert.ToInt32(e.CommandArgument));
                    break;
                case TOGGLE:
                    ProductService.ToggleProductMediaStatus(Convert.ToInt32(e.CommandArgument));
                    break;
            }

            LoadProductInfo();
            hfCurrentPanel.Value = "images";
        }

        protected void lbSavePrice_Click(object sender, EventArgs e)
        {
            var productPrice = ProductService.GetProductPrice(Convert.ToInt32(hfProductPriceId.Value));

            if (productPrice == null)
            {
                enbNotice.Message = "Price could not be loaded. Product Price ID = " + hfProductPriceId.Value + ".";
                return;
            }

            var product = ProductService.GetProductById(productPrice.ProductId);
            if (product == null)
            {
                enbNotice.Message = "Product could not be loaded. Product ID = " + productPrice.ProductId + ".";
                return;
            }

            productPrice.PriceCode = txtPriceCode.Text.Trim();
            productPrice.Price = Convert.ToDecimal(txtPrice.Text.Trim());
            productPrice.Weight = Convert.ToInt32(txtWeight.Text.Trim());
            productPrice.Stock = Convert.ToInt32(txtStock.Text.Trim());
            productPrice.Barcode = txtBarcode.Text.Trim();
            productPrice.Enabled = cbPriceStatus.Checked;
            productPrice.Priority = Convert.ToInt32(txtPriority.Text.Trim());
            string additionalShippingCost = txtAdditionalShippingCost.Text.Trim().Length > 0 ? txtAdditionalShippingCost.Text : "0";
            productPrice.AdditionalShippingCost = Convert.ToDecimal(additionalShippingCost);

            if (string.IsNullOrEmpty(txtMaximumAllowedPurchaseQuantity.Text.Trim()))
                productPrice.MaximumAllowedPurchaseQuantity = null;
            else
                productPrice.MaximumAllowedPurchaseQuantity = Convert.ToInt32(txtMaximumAllowedPurchaseQuantity.Text.Trim());

            switch ((OptionType)product.OptionType)
            {
                case OptionType.None:
                    break;
                case OptionType.Size:
                    // Size option section
                    string size = txtSize.Text.Trim();
                    productPrice.Size = size;
                    break;
                case OptionType.Colour:
                    // Colour option section
                    int colourId = Convert.ToInt32(txtColourId.Text);
                    productPrice.ColourId = colourId;
                    break;
                case OptionType.GiftCard:
                    break;
                default:
                    break;
            }

            productPrice.DisableStockSync = cbDisableStockSync.Checked;

            if (!string.IsNullOrEmpty(txtProductMediaIdForOption.Text.Trim()) && Convert.ToInt32(txtProductMediaIdForOption.Text.Trim()) > 0)
            {
                productPrice.ProductMediaId = Convert.ToInt32(txtProductMediaIdForOption.Text.Trim());
            }
            else
            {
                productPrice.ProductMediaId = null;
            }
            
            productPrice.UpdatedOnDate = DateTime.Now;
            ProductService.UpdateProductPrice(productPrice);

            enbNotice.Message = "Price was updated successfully.";

            ClearPriceInfo();
            LoadProductInfo();
            hfCurrentPanel.Value = "prices";
        }

        protected void lbDeletePrice_Click(object sender, EventArgs e)
        {
            int productPriceId = Convert.ToInt32(hfProductPriceId.Value);
            var lineItems = OrderService.GetLineItemsByProductPrice(productPriceId);
            if (lineItems.Count > 0)
            {
                enbNotice.Message = "Sorry, the price could not be deleted as there are some associated line items.";
            }
            else
            {
                ProductService.DeleteProductPrice(productPriceId);
                enbNotice.Message = "Price was deleted successfully.";
            }
            
            ClearPriceInfo();
            LoadProductInfo();
            hfCurrentPanel.Value = "prices";
        }

        protected void lbAddNewPrice_Click(object sender, EventArgs e)
        {
            var productId = QueryProductId;
            string amazonSKU = string.Empty;
            string additionalShippingCost = string.Empty;
            int colourId = 0;
            int.TryParse(txtColourId.Text.Trim(), out colourId);

            var newPrice = new ProductPrice
            {
                ProductId = productId,
                PriceCode = txtPriceCode.Text.Trim(),
                Price = Convert.ToDecimal(txtPrice.Text.Trim()),
                Weight = Convert.ToInt32(txtWeight.Text.Trim()),
                AdditionalShippingCost = Convert.ToDecimal(txtAdditionalShippingCost.Text.Trim()),
                Stock = Convert.ToInt32(txtStock.Text.Trim()),
                Enabled = cbPriceStatus.Checked,
                Barcode = txtBarcode.Text.Trim(),
                Size = txtSize.Text.Trim(),
                ColourId = colourId,
                MaximumAllowedPurchaseQuantity = string.IsNullOrEmpty(txtMaximumAllowedPurchaseQuantity.Text.Trim()) ? default(int?) : Convert.ToInt32(txtMaximumAllowedPurchaseQuantity.Text.Trim()),
                DisableStockSync = cbDisableStockSync.Checked
            };

            ProductService.InsertProductPrice(newPrice);

            ClearPriceInfo();
            LoadProductInfo();
            enbNotice.Message = "Price was added successfully.";
            hfCurrentPanel.Value = "prices";
        }

        protected void lbSaveImage_Click(object sender, EventArgs e)
        {
            ProductMedia media = new ProductMedia();
            string message = ImageUtility.UploadProductImage(fuImage.PostedFile,
                                                             txtUrlKey.Text,
                                                             QueryProductId,
                                                             false,
                                                             out media);

            if (message == string.Empty && media != null)
            {
                ProductService.InsertProductMedia(media);
            }

            ClearImageInfo();
            LoadProductInfo();
            hfCurrentPanel.Value = "images";

            if (message == string.Empty)
                enbNotice.Message = "Image was updated successfully.";
            else
                enbNotice.Message = "Sorry, image was FAILED to update. " + message;
        }

        protected void lbSave300Image_Click(object sender, EventArgs e)
        {
            ProductMedia media = new ProductMedia();
            string message = ImageUtility.UploadProduct300Image(fuMainImage.PostedFile,
                                                                fuThumbImage.PostedFile,
                                                                txtUrlKey.Text,
                                                                QueryProductId,
                                                                false,
                                                                out media);

            if (message == string.Empty && media != null)
            {
                ProductService.InsertProductMedia(media);
            }

            ClearImageInfo();
            LoadProductInfo();
            hfCurrentPanel.Value = "images";

            if (message == string.Empty)
                enbNotice.Message = "Image was updated successfully.";
            else
                enbNotice.Message = "Sorry, image was FAILED to update. " + message;
        }

        protected void lbUploadColour_Click(object sender, EventArgs e)
        {
            string mainFilename = string.Empty;
            string thumbFilename = string.Empty;

            bool success = ImageUtility.UploadColourImage(fuColourImage.PostedFile, Server.HtmlEncode(txtColourName.Text), out mainFilename, out thumbFilename);

            if (success)
            {
                Colour colour = new Colour();
                colour.Value = Server.HtmlEncode(txtColourName.Text.Trim());
                colour.ColourFilename = mainFilename;
                colour.ThumbnailFilename = thumbFilename;

                colour.Id = ProductService.InsertColour(colour);

                enbNotice.Message = "Colour was added successfully.";

                imgColourOption.ImageUrl = "/get_image_handler.aspx?type=colour&img=" + colour.ColourFilename;
                ltlColourOptionName.Text = colour.Value;
                txtColourId.Text = colour.Id.ToString();

                mvOption.ActiveViewIndex = 2;
            }
            else
            {
                enbNotice.Message = "Colour was FAILED to add.";
            }

            hfCurrentPanel.Value = "prices";
        }

        protected void lbEditUpdateColour_Click(object sender, EventArgs e)
        {
            int colourId = Convert.ToInt32(txtColourId.Text);
            Colour colour = ProductService.GetColour(colourId);

            if (colour != null)
            {
                colour.Value = txtEditColourName.Text;

                if (fuEditColour.HasFile)
                {
                    string mainFilename = string.Empty;
                    string thumbFilename = string.Empty;

                    ImageUtility.UploadColourImage(fuEditColour.PostedFile, colour.Value, out mainFilename, out thumbFilename);

                    colour.ColourFilename = mainFilename;
                    colour.ThumbnailFilename = thumbFilename;
                }

                ProductService.UpdateColour(colour);

                imgColourOption.ImageUrl = "/get_image_handler.aspx?type=colour&img=" + colour.ColourFilename;
                ltlColourOptionName.Text = colour.Value;

                mvOption.ActiveViewIndex = 2;

                ltlPriceTitle.Text = "Price ID: " + hfProductPriceId.Value;

                int productId = QueryProductId;
                var product = ProductService.GetProductById(productId);
                rptPrices.DataSource = product.ProductPrices;
                rptPrices.DataBind();

                lbDeletePrice.Visible = true;
                lbSavePrice.Visible = true;
                lbAddNewPrice.Visible = false;

                hfCurrentPanel.Value = "prices";

                enbNotice.Message = "Colour was updated successfully.";
            }
        }

        protected void lbRemoveAssociatedImage_Click(object sender, EventArgs e)
        {
            txtProductMediaIdForOption.Text = string.Empty;
            imgAssociatedPicture.ImageUrl = null;
            lbRemoveAssociatedImage.Visible = false;

            hfCurrentPanel.Value = "prices";
        }

        protected void lbChangeAssociatedImage_Click(object sender, EventArgs e)
        {
            mvAssociatedImage.ActiveViewIndex = 1;

            rptRelatedImages.DataSource = ProductService.GetProductMediaByProductId(QueryProductId);
            rptRelatedImages.DataBind();

            hfCurrentPanel.Value = "prices";
        }

        protected void rptRelatedImages_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int productMediaId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "choose":
                    txtProductMediaIdForOption.Text = productMediaId.ToString();
                    var picture = ProductService.GetProductMedia(productMediaId);

                    if (picture != null)
                    {
                        imgAssociatedPicture.ImageUrl = string.Format("/get_image_handler.aspx?type={0}&img={1}", "media", picture.ThumbnailFilename);
                    }

                    lbRemoveAssociatedImage.Visible = true;

                    mvAssociatedImage.ActiveViewIndex = 0;

                    break;
            }

            hfCurrentPanel.Value = "prices";
        }

        protected void lbCancelAssociatedImage_Click(object sender, EventArgs e)
        {
            mvAssociatedImage.ActiveViewIndex = 0;
            hfCurrentPanel.Value = "prices";
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            ClearPriceInfo();
            ClearImageInfo();
            enbNotice.Message = string.Empty;

            LoadProductInfo();
            hfCurrentPanel.Value = "general";
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            var deleted = ProductService.ProcessProductDeletion(QueryProductId);

            if (deleted)
                Response.Redirect("/catalog/product_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductDeleted);

            enbNotice.Message = "This product could not be deleted as it is in order line. It's recommended to disable it.";

            hfCurrentPanel.Value = "general";
        }
        
        protected void lbSearchNewCategory_Click(object sender, EventArgs e)
        {
            int categoryId = 0;
            int.TryParse(hfCategory.Value, out categoryId);
            var treeList = CategoryService.GetTreeList(categoryId);
            ectCategory.FindSelectedNode(AppConstant.DEFAULT_CATEGORY, treeList);
            ectCategory.Visible = true;

            lbSearchNewCategory.Visible = false;

            hfCurrentPanel.Value = "category";
        }

        protected void lbSearchNewGoogleTaxonomy_Click(object sender, EventArgs e)
        {
            int googleTaxonomyId = 0;
            int.TryParse(hfGoogleTaxonomyId.Value, out googleTaxonomyId);
            var treeList = CategoryService.GetGoogleTaxonomyTreeList(googleTaxonomyId);
            egtGoogle.FindSelectedNode(0, treeList);
            egtGoogle.Visible = true;

            lbSearchNewGoogleTaxonomy.Visible = false;
            lbCancelGoogleTaxonomy.Visible = true;

            hfCurrentPanel.Value = "google";
        }

        protected void lbAddNewCategory_Click(object sender, EventArgs e)
        {
            int productId = QueryProductId;

            if (hfCategory.Value != string.Empty)
            {
                int categoryId = Convert.ToInt32(hfCategory.Value);
                CategoryService.ProcessCategoryAssignmentForProduct(categoryId, productId);

                enbNotice.Message = "Category was added successfully.";
            }

            lbSearchNewCategory.Visible = true;
            lbAddNewCategory.Visible = false;
            lbCancelCategory.Visible = false;

            hfCategory.Value = string.Empty;
            ltlCategory.Text = string.Empty;

            LoadProductInfo();
            hfCurrentPanel.Value = "category";
        }

        protected void lbCancelCategory_Click(object sender, EventArgs e)
        {
            lbSearchNewCategory.Visible = true;
            lbAddNewCategory.Visible = false;
            lbCancelCategory.Visible = false;

            hfCategory.Value = string.Empty;
            ltlCategory.Text = string.Empty;

            LoadProductInfo();
            hfCurrentPanel.Value = "category";
        }

        protected void lbCancelGoogleTaxonomy_Click(object sender, EventArgs e)
        {
            ltlNewGoogleTaxonomy.Text = string.Empty;
            hfNewGoogleTaxonomyId.Value = string.Empty;

            egtGoogle.Clear();

            lbUpdateNewGoogleTaxonomy.Visible = false;
            lbCancelGoogleTaxonomy.Visible = false;
            lbSearchNewGoogleTaxonomy.Visible = true;
            egtGoogle.Visible = false;

            LoadProductInfo();

            hfCurrentPanel.Value = "google";
        }

        protected void lbSaveTag_Click(object sender, EventArgs e)
        {
            int productId = QueryProductId;
            int tagId = Convert.ToInt32(ddlTag.SelectedValue);
            string content;

            if (ftbTagContent.Visible)
            {
                content = AdminStoreUtility.CleanFtbOutput(ftbTagContent.Text.Trim());
            }
            else
            {
                content = txtTagContent.Text.Trim();
            }

            if (content == string.Empty)
            {
                ProductService.DeleteProductTag(productId, tagId);

                enbNotice.Message = "Tag was deleted successfully.";
            }
            else
            {
                var product = ProductService.GetProductById(productId);
                var productTag = product.ProductTags.Where(pt => pt.TagId == tagId).FirstOrDefault();

                if (productTag != null)
                {
                    productTag.Value = content;
                    ProductService.UpdateProductTag(productTag);

                    enbNotice.Message = "Tag was updated successfully.";
                }
                else
                {
                    var newProductTag = new ProductTag
                    {
                        ProductId = productId,
                        TagId = tagId,
                        Value = content
                    };

                    ProductService.InsertProductTag(newProductTag);

                    enbNotice.Message = "Tag was added successfully.";
                }
            }

            ddlTag.SelectedIndex = 0;
            LoadProductInfo();
            hfCurrentPanel.Value = "tags";
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            string message = UpdateProduct();

            if (string.IsNullOrEmpty(message))
            {
                enbNotice.Message = "Product was updated successfully.";
            }
            else
            {
                ErrorSummary.AddError(message, "vgProduct", this.Page);
            }

            LoadProductInfo();
            ClearPriceInfo();
            ClearImageInfo();
            hfCurrentPanel.Value = "general";
        }

        protected void lbEditColourId_Click(object sender, EventArgs e)
        {
            mvOption.ActiveViewIndex = 3;

            var colourId = 0;
            Colour colour = null;
            if (int.TryParse(txtColourId.Text, out colourId) && colourId != 0)
            {
                colour = ProductService.GetColour(Convert.ToInt32(txtColourId.Text));
            }

            if (colour != null)
            {
                imgEditColourImg.ImageUrl = "/get_image_handler.aspx?type=colour&img=" + colour.ColourFilename;
                txtEditColourName.Text = colour.Value;
                txtColourId.Text = colour.Id.ToString();
                phEditColourPanel.Visible = true;
                phNewColour.Visible = false;
            }
            else
            {
                phNewColour.Visible = true;
            }

            LoadColours();
            hfCurrentPanel.Value = "prices";
        }

        protected void lbRemoveCategory_Click(object sender, EventArgs e)
        {
            int productId = QueryProductId;
            int categoryId = 0;

            var categories = CategoryService.GetCategoriesByProductId(productId);

            if (categories.Count <= 1)
            {
                enbNotice.Message = "Sorry, you must have at least one category.";
            }
            else if (int.TryParse(ddlCategorySelection.SelectedValue, out categoryId))
            {
                CategoryService.DeleteCategoryFromProduct(categoryId, productId);
                enbNotice.Message = "Category was deleted successfully.";
                LoadProductInfo();
            }
            else
                enbNotice.Message = "Sorry, there is no category to be deleted.";

            hfCurrentPanel.Value = "category";
        }

        protected void lbRemoveGoogleTaxonomy_Click(object sender, EventArgs e)
        {
            ProductService.UpdateProductGoogleTaxonomy(QueryProductId, 0);

            LoadProductInfo();

            enbNotice.Message = "Google Taxonomy was removed successfully.";

            hfCurrentPanel.Value = "google";
        }

        protected void lbUpdateNewGoogleTaxonomy_Click(object sender, EventArgs e)
        {
            var googleTaxonomyId = 0;

            if (int.TryParse(hfNewGoogleTaxonomyId.Value, out googleTaxonomyId))
            {
                ProductService.UpdateProductGoogleTaxonomy(QueryProductId, googleTaxonomyId);

                lbUpdateNewGoogleTaxonomy.Visible = false;
                lbCancelGoogleTaxonomy.Visible = false;
                lbSearchNewGoogleTaxonomy.Visible = true;

                ltlNewGoogleTaxonomy.Text = string.Empty;
                hfNewGoogleTaxonomyId.Value = string.Empty;

                LoadProductInfo();
                
                enbNotice.Message = "Google Taxonomy was updated successfully.";
            }
            
            hfCurrentPanel.Value = "google";
        }

        protected void ectCategory_TreeChanged(string categoryName, int categoryId)
        {
            LoadProductInfo();
            hfCurrentPanel.Value = "category";
        }

        protected void egtGoogle_TreeChanged(string taxonomyName, int taxonomyId)
        {
            LoadProductInfo();
            hfCurrentPanel.Value = "google";
        }

        protected void ectCategory_TreeNodeSelected(string categoryName, int categoryId)
        {
            ltlCategory.Text = categoryName;
            hfCategory.Value = categoryId.ToString();
            ectCategory.Clear();

            lbAddNewCategory.Visible = true;
            lbCancelCategory.Visible = true;
            ectCategory.Visible = false;

            LoadProductInfo();

            hfCurrentPanel.Value = "category";
        }

        protected void egtGoogle_TreeNodeSelected(string taxonomyName, int taxonomyId)
        {
            var googleTaxonomy = CategoryService.GetGoogleTaxonomyTreeListWithName(taxonomyId);
            ltlNewGoogleTaxonomy.Text = string.Join(" &gt; ", googleTaxonomy.Select(x => x.Value).ToArray()) + "<br/>";
            hfNewGoogleTaxonomyId.Value = taxonomyId.ToString();
            egtGoogle.Clear();

            lbUpdateNewGoogleTaxonomy.Visible = true;
            lbCancelGoogleTaxonomy.Visible = true;
            egtGoogle.Visible = false;

            LoadProductInfo();

            hfCurrentPanel.Value = "google";
        }

        protected void lbGo_Click(object sender, EventArgs e)
        {
            Response.Redirect("/catalog/product_info.aspx?productid=" + txtGoProductId.Text.Trim());
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Product | CacheEntityKey.Brand);

            if (result)
                enbNotice.Message = "All products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected string GeneratePriceOption(int productPriceId)
        {
            Product product = ProductService.GetProductById(QueryProductId);
            ProductPrice option = ProductService.GetProductPrice(productPriceId);
            StringBuilder sb = new StringBuilder();

            switch ((OptionType)product.OptionType)
            {
                case OptionType.Size:
                    sb.AppendFormat("Size: {0}", option.Size);
                    break;
                case OptionType.Colour:
                    if (option.ColourId.HasValue)
                    {
                        var colour = ProductService.GetColour(option.ColourId.Value);

                        if (colour != null)
                            sb.AppendFormat("<img alt='{0}' src='/get_image_handler.aspx?type={2}&img={1}'/><br/>{0}", colour.Value, colour.ColourFilename, "colour");
                        else
                        {
                            _logger.ErrorFormat("Colour could not be loaded. Colour ID={{{0}}}", option.ColourId);                            
                        }
                    }
                    break;
                case OptionType.None:
                case OptionType.GiftCard:                
                default:
                    break;
            }

            if (option.ProductMediaId.HasValue)
            {
                var picture = ProductService.GetProductMedia(option.ProductMediaId.Value);

                if (picture != null)
                {
                    sb.AppendFormat("<br/><br/>Associated image<br/><img src='/get_image_handler.aspx?type={1}&img={0}'/>", picture.ThumbnailFilename, "media");                    
                }
            }

            return sb.ToString();
        }
        
        protected void lbAddMeta_Click(object sender, EventArgs e)
        {
            var product = ProductService.GetProductById(QueryProductId);

            if (product == null)
            {
                enbNotice.Message = "Sorry, product could not be loaded.";
                return;
            }

            product.MetaTitle = txtMetaTitle.Text.Trim();
            product.MetaDescription = txtMetaDescription.Text.Trim();

            if (string.IsNullOrEmpty(product.MetaDescription))
                product.MetaDescription = AdminStoreUtility.TruncateString(MAX_LENGTH_META_DESCRIPTION, product.Description);

            product.MetaKeywords = txtMetaKeywords.Text.Trim();
            product.SecondaryKeywords = txtSecondaryKeywords.Text.Trim();

            ProductService.UpdateProduct(product);

            enbNotice.Message = "Product meta was updated successfully.";

            LoadProductInfo();
            hfCurrentPanel.Value = "meta";
        }
        
        protected void lbSearchColour_Click(object sender, EventArgs e)
        {
            SetState(COLOUR_ID_FILTER, ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourId")).Text.Trim());
            SetState(COLOUR_VALUE_FILTER, ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourName")).Text.Trim());
            SetState("COLOUR_BRAND_NAME_FILTER", ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourBrand")).Text.Trim());

            LoadColours();
        }

        protected void lbResetFilterColour_Click(object sender, EventArgs e)
        {
            DisposeState(COLOUR_ID_FILTER);
            DisposeState(COLOUR_VALUE_FILTER);
            DisposeState("COLOUR_BRAND_NAME_FILTER");

            LoadColours();
        }

        protected void gvBrandColours_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBrandColours.CustomPageIndex = gvBrandColours.CustomPageIndex + e.NewPageIndex;

            if (gvBrandColours.CustomPageIndex < 0)
                gvBrandColours.CustomPageIndex = 0;

            LoadColours();
        }

        protected void gvBrandColours_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ColourSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = ColourSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ColourSortingType.IdAsc;
                    break;
                case "Value":
                    orderBy = ColourSortingType.ValueDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ColourSortingType.ValueAsc;
                    break;
                case "BrandName":
                    orderBy = ColourSortingType.BrandNameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ColourSortingType.BrandNameAsc;
                    break;
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadColours();
        }

        protected void gvBrandColours_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "select":
                    int colourId = Convert.ToInt32(e.CommandArgument);
                    var colour = ProductService.GetColour(colourId);

                    txtColourId.Text = colourId.ToString();
                    imgColourOption.ImageUrl = "/get_image_handler.aspx?type=colour&img=" + colour.ColourFilename;
                    mvOption.ActiveViewIndex = 2;
                    break;
                default:
                    break;
            }
        }

        protected void gvBrandColours_PreRender(object sender, EventArgs e)
        {
            if (gvBrandColours.TopPagerRow != null)
            {
                gvBrandColours.TopPagerRow.Visible = true;
                ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourId")).Text = GetStringState(COLOUR_ID_FILTER);
                ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourName")).Text = GetStringState(COLOUR_VALUE_FILTER);
                ((TextBox)gvBrandColours.HeaderRow.FindControl("txtFilterColourBrand")).Text = GetStringState("COLOUR_BRAND_NAME_FILTER");
            }
        }

        protected void btnColourGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvBrandColours.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvBrandColours.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvBrandColours.CustomPageIndex = gotoIndex;

            LoadColours();
        }
        
        protected void lbSaveDescription_Click(object sender, EventArgs e)
        {
            int productId = QueryProductId;
            var product = ProductService.GetProductById(productId);

            if (product == null)
                Response.Redirect("/catalog/product_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductNotFound);

            if (ftbDesc.Visible)
                product.Description = AdminStoreUtility.CleanFtbOutput(ftbDesc.Xhtml);
            else
                product.Description = txtDescription.Text.Trim();

            //product.Description = ltDescription.Text;

            ProductService.UpdateProduct(product);
            LoadProductInfo();
            hfCurrentPanel.Value = "desc";

            enbNotice.Message = "Description was updated successfully.";
        }

        protected void lbSaveRating_Click(object sender, EventArgs e)
        {
            int productId = QueryProductId;
            var product = ProductService.GetProductById(productId);

            if (product == null)
                Response.Redirect("/catalog/product_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductNotFound);

            int ApolloRating = 0;

            if (int.TryParse(txtApolloRating.Text.Trim(), out ApolloRating))
            {
                product.Rating = ApolloRating;
                ProductService.UpdateProduct(product);
                LoadProductInfo();

                enbNotice.Message = "Rank rating was updated successfully.";
            }
            else
                enbNotice.Message = "Sorry, invalid Apollo rating detected. Please enter integer only.";
            
            hfCurrentPanel.Value = "ratings";
        }

        protected void lbAssignRestrictedGroup_Click(object sender, EventArgs e)
        {
            var id = QueryProductId;
            var product = ProductService.GetProductById(id);
            var restrictedGroupId = Convert.ToInt32(ddlRestrictedGroups.SelectedValue);

            if (product.RestrictedGroups.Where(x => x.Id == restrictedGroupId).Count() > 0)
            {
                enbNotice.Message = "Sorry, this restricted group has already been assigned.";
            }
            else
            {
                var productRestrictedGroup = new RestrictedGroupMapping
                {
                    ProductId = id,
                    RestrictedGroupId = restrictedGroupId
                };

                ProductService.InsertProductRestrictedGroup(productRestrictedGroup);
                enbNotice.Message = "Restricted group has been successfully assigned to this product.";

                LoadProductInfo();
            }

            hfCurrentPanel.Value = "restriction";
        }

        protected void rptAssignedRestrictedGroups_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    var id = QueryProductId;
                    ProductService.DeleteProductRestrictedGroup(id, Convert.ToInt32(e.CommandArgument));
                    enbNotice.Message = "Restricted group has been successfully removed.";
                    break;
            }

            LoadProductInfo();
            hfCurrentPanel.Value = "restriction";
        }

        protected void lbUpdateGoogleShoppingLabels_Click(object sender, EventArgs e)
        {
            var customLabelGroup = new ProductGoogleCustomLabelGroupMapping
            {
                ProductId = QueryProductId,
                CustomLabel1 = txtCustomLabel1.Text.Trim(),
                CustomLabel2 = txtCustomLabel2.Text.Trim(),
                CustomLabel3 = txtCustomLabel3.Text.Trim(),
                CustomLabel4 = txtCustomLabel4.Text.Trim(),
                CustomLabel5 = txtCustomLabel5.Text.Trim(),
                Value1 = txtValue1.Text.Trim(),
                Value2 = txtValue2.Text.Trim(),
                Value3 = txtValue3.Text.Trim(),
                Value4 = txtValue4.Text.Trim(),
                Value5 = txtValue5.Text.Trim(),
            };
            
            ProductService.SaveProductGoogleCustomLabelGroup(customLabelGroup);
            LoadProductInfo();

            enbNotice.Message = "Google Shopping custom labels were updated successfully.";

            hfCurrentPanel.Value = "google";
        }

        private void LoadColours()
        {
            int[] colourIds = null;
            string name = null;
            string brandName = null;
            ColourSortingType orderBy = ColourSortingType.IdAsc;

            if (HasState(COLOUR_ID_FILTER))
            {
                string value = GetStringState(COLOUR_ID_FILTER);
                int temp;
                colourIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(COLOUR_VALUE_FILTER)) name = GetStringState(COLOUR_VALUE_FILTER);
            if (HasState("COLOUR_BRAND_NAME_FILTER")) brandName = GetStringState("COLOUR_BRAND_NAME_FILTER");

            if (HasState("OrderBy")) orderBy = (ColourSortingType)GetIntState("OrderBy");

            var result = ProductService.GetColourLoadPaged(
                pageIndex: gvBrandColours.CustomPageIndex,
                pageSize: gvBrandColours.PageSize,
                colourIds: colourIds,
                name: name,
                brand: brandName,
                orderBy: orderBy);

            if (result != null)
            {
                gvBrandColours.DataSource = result.Items;
                gvBrandColours.RecordCount = result.TotalCount;
                gvBrandColours.CustomPageCount = result.TotalPages;
            }

            gvBrandColours.DataBind();

            //if (gvBrandColours.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private string UpdateProduct()
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
                message = "Url Key is not unique.";
            }
            else
            {
                var product = ProductService.GetProductById(productId);

                if (ftbDesc.Visible)
                    product.Description = AdminStoreUtility.CleanFtbOutput(ftbDesc.Xhtml);
                else
                    product.Description = txtDescription.Text.Trim();

                product.Rating = string.IsNullOrEmpty(txtApolloRating.Text.Trim()) ? 0 : Convert.ToInt32(txtApolloRating.Text.Trim());
                product.Name = Server.HtmlEncode(txtName.Text.Trim());
                product.ProductCode = txtProductCode.Text.Trim();
                product.H1Title = Server.HtmlEncode(txtH1Title.Text.Trim());
                product.Enabled = ddlStatus.SelectedValue == ENABLED ? true : false;
                product.DeliveryId = Convert.ToInt32(ddlDelivery.SelectedValue);
                product.HasFreeWrapping = cbFreeWrapped.Checked;
                product.IsPharmaceutical = cbIsPharm.Checked;
                product.BrandId = Convert.ToInt32(ddlBrand.SelectedValue);
                product.UrlRewrite = txtUrlKey.Text.Trim();
                product.MetaTitle = txtMetaTitle.Text.Trim();
                product.MetaDescription = txtMetaDescription.Text.Trim();

                if (string.IsNullOrEmpty(product.MetaDescription))
                    product.MetaDescription = AdminStoreUtility
                        .TruncateString(MAX_LENGTH_META_DESCRIPTION,
                                        AdminStoreUtility.CleanHtml(product.Description,
                                                                    product.Description.Length));

                product.MetaKeywords = txtMetaKeywords.Text.Trim();
                product.SecondaryKeywords = txtSecondaryKeywords.Text.Trim();
                product.Discontinued = cbDiscontinued.Checked;
                product.OpenForOffer = cbOpenForOffer.Checked;
                product.EnforceStockCount = cbEnforceStockCount.Checked;
                product.IsGoogleProductSearchDisabled = cbGoogleProductSearchDisabled.Checked;
                product.ShowPreOrderButton = cbDisplayPreOrder.Checked;
                product.OptionType = Convert.ToInt32(ddlOptionType.SelectedValue);
                product.ProductMark = txtProductMark.Text.Trim();
                product.ProductMarkType = Convert.ToInt32(ddlProductMarks.SelectedValue);
                if (!string.IsNullOrEmpty(txtProductMarkExpiryDate.Text.Trim()))
                    product.ProductMarkExpiryDate = DateTime.ParseExact(txtProductMarkExpiryDate.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                else
                    product.ProductMarkExpiryDate = null;
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

                //product.Description = ltDescription.Text;
                product.UpdatedOnDate = DateTime.Now;

                ProductService.UpdateProduct(product);
            }

            return message;
        }

        private void ClearPriceInfo()
        {
            lbAddNewPrice.Visible = true;
            lbSavePrice.Visible = false;
            lbDeletePrice.Visible = false;
            ltlPriceTitle.Text = NEW_PRICE;
            mvOption.ActiveViewIndex = 0;
            hfProductPriceId.Value = string.Empty;
            txtPrice.Text = string.Empty;
            imgColourOption.ImageUrl = string.Empty;
            ltlColourOptionName.Text = string.Empty;
            txtColourId.Text = string.Empty;
            txtSize.Text = string.Empty;
            txtStock.Text = string.Empty;
            txtBarcode.Text = string.Empty;
            txtWeight.Text = string.Empty;
            phEditColourPanel.Visible = false;
            mvOption.ActiveViewIndex = 0;
            cbPriceStatus.Checked = false;
            txtPriceCode.Text = string.Empty;
            txtAdditionalShippingCost.Text = "0";
            txtPriority.Text = "0";
            txtMaximumAllowedPurchaseQuantity.Text = string.Empty;
            cbDisableStockSync.Checked = false;
            txtProductMediaIdForOption.Text = string.Empty;
            lbRemoveAssociatedImage.Visible = false;
            lbChangeAssociatedImage.Visible = false;
            imgAssociatedPicture.ImageUrl = null;
        }

        private void ClearImageInfo()
        {
            ltlImageTitle.Text = NEW_IMAGE;
            //cbPrimary.Checked = false;
        }

        private void LoadProductInfo()
        {
            int productId = QueryProductId;
            var product = ProductService.GetProductById(productId);

            if (product == null)
                Response.Redirect("/catalog/product_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductNotFound);

            #region Next and Prev

            // Reads Next and Previous ProductId's
            var prevNext = ProductService.GetPrevNextProductId(productId);

            if (prevNext[0] > 0)
                hlPrev.NavigateUrl = "/catalog/product_info.aspx?productid=" + prevNext[0].ToString();
            else
                hlPrev.Visible = false;

            if (prevNext[1] > 0)
                hlNext.NavigateUrl = "/catalog/product_info.aspx?productid=" + prevNext[1].ToString();
            else
                hlNext.Visible = false;

            #endregion

            #region General

            ltlTitle.Text = string.Format("{0} (ID: {1}) {2} {3}",
                product.Name,
                product.Id,
                string.Format("<a href='{0}' target='_blank'><i class='fa fa-external-link' aria-hidden='true'></i></a>", AdminStoreUtility.GetProductUrl(product.UrlRewrite)),
                product.Enabled ? null : "<i class='fa fa-eye-slash' aria-hidden='true' title='offline'></i>");
            txtName.Text = Server.HtmlDecode(product.Name);
            txtProductCode.Text = product.ProductCode;
            txtH1Title.Text = product.H1Title;

            ddlStatus.SelectedIndex = -1;
            var status = ddlStatus.Items.FindByValue(product.Enabled ? ENABLED : DISABLED);
            if (status != null) status.Selected = true;

            ddlDelivery.Items.FindByValue(product.DeliveryId.ToString()).Selected = true;
            cbFreeWrapped.Checked = product.HasFreeWrapping;
            cbIsPharm.Checked = product.IsPharmaceutical;
            cbDisplayPreOrder.Checked = product.ShowPreOrderButton;

            ddlBrand.SelectedIndex = -1;
            ListItem foundBrand = ddlBrand.Items.FindByValue(product.BrandId.ToString());
            Brand brand = null;
            if (foundBrand != null)
            {
                foundBrand.Selected = true;
                brand = BrandService.GetBrandById(product.BrandId);
            }
            else
            {
                phGeneralAlert.Visible = true;
                enbNotice.AppendMessage = "Brand ID " + product.BrandId.ToString() + " was not found.";
            }

            txtUrlKey.Text = product.UrlRewrite;
            cbOpenForOffer.Checked = product.OpenForOffer;

            ddlTaxCategory.SelectedIndex = -1;
            if (product.TaxCategory != null)
            {
                var foundTax = ddlTaxCategory.Items.FindByValue(product.TaxCategory.Id.ToString());
                if (foundTax != null)
                    foundTax.Selected = true;
            }

            cbEnforceStockCount.Checked = product.EnforceStockCount;
            ltlBrandStock.Text = string.Empty;
            if (brand != null && brand.EnforceStockCount == true)
            {
                ltlBrandStock.Text = "<p class='hint'>(enforce stock count is enabled on brand too)</p>";
            }

            cbGoogleProductSearchDisabled.Checked = product.IsGoogleProductSearchDisabled;
            txtStepQuantity.Text = product.StepQuantity.ToString();
            cbDiscontinued.Checked = product.Discontinued;

            ddlOptionType.SelectedIndex = -1;
            var optionType = ddlOptionType.Items.FindByValue(product.OptionType.ToString());
            if (optionType != null) optionType.Selected = true;

            txtProductMark.Text = product.ProductMark;
            var markType = ddlProductMarks.Items.FindByValue(product.ProductMarkType.ToString());
            if (markType != null) markType.Selected = true;

            txtProductMarkExpiryDate.Text = product.ProductMarkExpiryDate.HasValue ? product.ProductMarkExpiryDate.Value.ToString(AppConstant.DATE_FORM1) : string.Empty;

            cbIsPhoneOrder.Checked = product.IsPhoneOrder;

            cbVisibleIndividually.Checked = product.VisibleIndividually;

            #endregion

            #region Rank rating

            hfCustomerRating.Value = GetProductCustomerRating(product.ProductReviews);
            hfApolloRating.Value = product.Rating.ToString();

            var popularity = ProductService.CalculateProductPopularity(product.Id, 90);
            hfPopularity.Value = Convert.ToInt32(popularity).ToString();

            txtMetaKeywords.Text = product.MetaKeywords;
            txtSecondaryKeywords.Text = product.SecondaryKeywords;

            #endregion

            #region Prices

            ltlPriceTitle.Text = NEW_PRICE;
            rptPrices.DataSource = product.ProductPrices;
            rptPrices.DataBind();

            if (rptPrices.Items.Count == 0)
            {
                phPricesAlert.Visible = true;
                enbNotice.AppendMessage = "Price was not assigned to this product.";
            }
            else
                phPricesAlert.Visible = false;

            lbDeletePrice.Visible = false;
            lbSavePrice.Visible = false;

            switch ((OptionType)product.OptionType)
            {
                case OptionType.None:
                    mvOption.ActiveViewIndex = 0;
                    break;
                case OptionType.Size:
                    mvOption.ActiveViewIndex = 1;
                    break;
                case OptionType.Colour:
                    mvOption.ActiveViewIndex = 2;
                    break;
                case OptionType.GiftCard:
                    mvOption.ActiveViewIndex = 0;
                    break;
                default:
                    mvOption.ActiveViewIndex = 0;
                    break;
            }

            #endregion

            #region Meta information

            txtMetaTitle.Text = product.MetaTitle;
            txtMetaDescription.Text = product.MetaDescription;
            txtMetaKeywords.Text = product.MetaKeywords;
            txtSecondaryKeywords.Text = product.SecondaryKeywords;

            #endregion

            #region Images

            ltlImageTitle.Text = NEW_IMAGE;
            rptImages.DataSource = product.ProductMedias;
            rptImages.DataBind();

            if (rptImages.Items.Count == 0)
            {
                phImagesAlert.Visible = true;
                enbNotice.AppendMessage = "Image was not uploaded to this product.";
            }
            else
                phImagesAlert.Visible = false;

            #endregion

            #region Description

            if (IsFireFoxBrowser)
            {
                ftbDesc.Visible = false;
                ftbTagContent.Visible = false;
                txtDescription.Text = product.Description;
            }
            else
            {
                txtDescription.Visible = false;
                txtTagContent.Visible = false;
                ftbDesc.Text = product.Description;
            }

            //ltDescription.Text = product.Description;

            if (string.IsNullOrEmpty(product.Description.Trim()))
            {
                phDescriptionAlert.Visible = true;
                enbNotice.AppendMessage = "There is no description on this product.";
            }
            else
                phDescriptionAlert.Visible = false;

            #endregion

            #region Category

            var assignedCategory = CategoryService.GetCategoriesByProductId(product.Id);
            var listItems = new List<ListItem>();
            foreach (var item in assignedCategory)
            {
                StringBuilder sb = new StringBuilder();
                var tree = CategoryService.GetTreeList(item.Id);

                foreach (var leaf in tree)
                {
                    var leafCategory = CategoryService.GetCategory(leaf);
                    if (sb.ToString() != string.Empty) sb.Append(" > ");
                    if (leafCategory != null) sb.Append(Server.HtmlDecode(leafCategory.CategoryName) + (leafCategory.Visible ? null : " (hidden)"));
                }

                listItems.Add(new ListItem(Server.HtmlDecode(sb.ToString()), item.Id.ToString()));
            }

            ddlCategorySelection.Items.Clear();
            ddlCategorySelection.Items.AddRange(listItems.ToArray());
            ddlCategorySelection.DataBind();
            ddlCategorySelection.Visible = ddlCategorySelection.Items.Count != 0;
            phCategoryAlert.Visible = ddlCategorySelection.Items.Count == 0;

            if (ddlCategorySelection.Items.Count == 0)
                enbNotice.AppendMessage = "Category was not assigned to this product.";

            #endregion

            #region Tags

            ProductTag productTag = product.ProductTags.Where(pt => pt.TagId == Convert.ToInt32(ddlTag.Items[0].Value)).FirstOrDefault();
            if (productTag != null)
            {
                if (IsFireFoxBrowser)
                {
                    ftbTagContent.Visible = false;
                    txtTagContent.Text = productTag.Value;
                }
                else
                {
                    txtTagContent.Visible = false;
                    ftbTagContent.Text = productTag.Value;
                }
            }

            #endregion

            #region Restricted groups

            rptAssignedRestrictedGroups.DataSource = product.RestrictedGroups;
            rptAssignedRestrictedGroups.DataBind();

            #endregion

            #region Google Taxonomy

            ltlGoogleTaxonomy.Text = string.Empty;
            hfGoogleTaxonomyId.Value = string.Empty;
            lbRemoveGoogleTaxonomy.Visible = product.GoogleTaxonomyId > 0;

            if (product.GoogleTaxonomyId == 0)
            {
                phGoogleAlert.Visible = true;
                enbNotice.AppendMessage = "Please assign Google taxonomy to this product.";
            }
            else
            {
                hfGoogleTaxonomyId.Value = product.GoogleTaxonomyId.ToString();
                var googleTaxonomy = CategoryService.GetGoogleTaxonomyTreeListWithName(product.GoogleTaxonomyId);
                ltlGoogleTaxonomy.Text = string.Join(" &gt; ", googleTaxonomy.Select(x => x.Value).ToArray()) + "<br/>";
            }

            #endregion

            #region Google Shopping Custom Labels

            if (product.ProductGoogleCustomLabelGroup == null && phGoogleAlert.Visible == false) phGoogleAlert.Visible = true;

            if (product.ProductGoogleCustomLabelGroup != null)
            {
                txtCustomLabel1.Text = product.ProductGoogleCustomLabelGroup.CustomLabel1;
                txtCustomLabel2.Text = product.ProductGoogleCustomLabelGroup.CustomLabel2;
                txtCustomLabel3.Text = product.ProductGoogleCustomLabelGroup.CustomLabel3;
                txtCustomLabel4.Text = product.ProductGoogleCustomLabelGroup.CustomLabel4;
                txtCustomLabel5.Text = product.ProductGoogleCustomLabelGroup.CustomLabel5;

                txtValue1.Text = product.ProductGoogleCustomLabelGroup.Value1;
                txtValue2.Text = product.ProductGoogleCustomLabelGroup.Value2;
                txtValue3.Text = product.ProductGoogleCustomLabelGroup.Value3;
                txtValue4.Text = product.ProductGoogleCustomLabelGroup.Value4;
                txtValue5.Text = product.ProductGoogleCustomLabelGroup.Value5;
            }

            #endregion
        }

        private string GetProductCustomerRating(IList<ProductReview> reviews)
        {
            if (reviews == null) return "0";
            if (reviews.Count == 0) return "0";

            int productReviewRating = 0;
            int count = 0;
            foreach (var review in reviews)
            {
                if (review.Approved)
                {
                    productReviewRating += review.Score;
                    count++;
                }
            }
            return count == 0 ? "0" : Convert.ToString((productReviewRating * 20) / count);
        }

        #region ICallbackEventHandler Members

        private string value;

        public string GetCallbackResult()
        {
            return value;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            const int PRODUCT_ID = 1;
            const int PRODUCT_MEDIA_ID = 2;
            const string ERROR = "Error";
            const string UPDATED = "Updated";

            string[] args = eventArgument.Split(new char[] { '_' });

            if (args.Length > 1)
            {
                switch (args[0])
                {
                    // image_<productId>_<mediaId>
                    case IMAGE:
                        try
                        {
                            ProductService.UpdateProductPrimaryImage(Convert.ToInt32(args[PRODUCT_ID]), Convert.ToInt32(args[PRODUCT_MEDIA_ID]));
                            value = UPDATED;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(string.Format("Failed to set primary image. Product ID={{{0}}}, Product Media ID={{{1}}}", args[PRODUCT_ID], args[PRODUCT_MEDIA_ID]), ex);
                            value = ERROR;
                        }

                        break;
                    // tag_<productId>_<selected_tag_id>
                    case TAG:
                        int productId = QueryProductId;
                        int tagId = Convert.ToInt32(args[2]);

                        try
                        {
                            var product = ProductService.GetProductById(productId);
                            var productTag = product.ProductTags.Where(pt => pt.TagId == tagId).FirstOrDefault();

                            value = productTag != null ? productTag.Value : string.Empty;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(string.Format("Failed to get tag content. Product ID={{{0}}}, Tag ID={{{1}}}", productId, tagId), ex);
                            value = ERROR;
                        }

                        break;

                    case STOCK:
                        try
                        {
                            var branchId = Convert.ToInt32(args[1]);
                            value = string.Empty;

                            if (value == string.Empty)
                                value = "n/a";
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(string.Format("Failed to get stock level. Branch ID={{{0}}}, Barcode={{{1}}}", args[1], args[2]), ex);
                            value = ERROR;
                        }

                        break;

                }
            }
        }

        #endregion
    }
}