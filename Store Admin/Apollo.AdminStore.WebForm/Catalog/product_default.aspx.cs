using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class product_default : BasePage
    {
        public IProductService ProductService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (QueryProductName != string.Empty)
                    SetState(PRODUCT_NAME_FILTER, QueryProductName.ToString());

                LoadProducts();
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

            SetState("OrderBy", (int)orderBy);
            LoadProducts();
        }

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
            {
                gvProducts.TopPagerRow.Visible = true;
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);                
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(PRODUCT_NAME_FILTER);
                ((DropDownList)gvProducts.HeaderRow.FindControl("ddlStatus")).Items.FindByValue(GetStringState(STATUS_FILTER)).Selected = true;
                ((DropDownList)gvProducts.HeaderRow.FindControl("ddlDiscontinued")).Items.FindByValue(GetStringState(DISCONTINUED_FILTER)).Selected = true;
            }

            var products = gvProducts.DataSource as Product[];

            if (products != null)
            {
                for (int i = 0; i < gvProducts.Rows.Count; i++)
                {
                    CheckBox cb = gvProducts.Rows[i].FindControl("cbChosen") as CheckBox;

                    if (ChosenProducts.Contains(products[i].Id))
                        cb.Checked = true;

                    SetChosenProducts(products[i].Id, cb.Checked);
                }
            }

            if (gvProducts.Rows.Count == 1 && (int)gvProducts.DataKeys[0].Value == 0 && gvProducts.TopPagerRow != null)
            {
                gvProducts.TopPagerRow.FindControl(PH_RECORD_FOUND).Visible = false;
                gvProducts.TopPagerRow.FindControl(PH_RECORD_NOT_FOUND).Visible = true;
            }
        }

        protected void lbResetProductFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(PRODUCT_NAME_FILTER);
            SetState(STATUS_FILTER, ANY);
            SetState(DISCONTINUED_FILTER, ANY);

            gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void lbSearchProduct_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(PRODUCT_NAME_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState(STATUS_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlStatus")).SelectedValue);
            SetState(DISCONTINUED_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlDiscontinued")).SelectedValue);

            gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void btnProductGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Product | CacheEntityKey.Brand);

            if (result)
                enbNotice.Message = "All product related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
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

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            bool? enabled = null;
            bool? discontinued = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(PRODUCT_NAME_FILTER)) name = GetStringState(PRODUCT_NAME_FILTER);
            if (GetStringState(STATUS_FILTER) == ENABLED) enabled = true;
            if (GetStringState(STATUS_FILTER) == DISABLED) enabled = false;
            if (GetStringState(DISCONTINUED_FILTER) == ENABLED) discontinued = true;
            if (GetStringState(DISCONTINUED_FILTER) == DISABLED) discontinued = false;
            if (HasState("OrderBy")) orderBy = (ProductSortingType)GetIntState("OrderBy");

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                productIds: productIds,
                keywords: name,
                enabled: enabled,
                discontinued: discontinued,
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

        protected void lbSubmit_Click(object sender, EventArgs e)
        {
            SaveLastViewedProduct();
            switch (ddlActions.SelectedValue)
            {
                case CHANGE_STATUS:                    
                    bool status = ddlActionStatus.SelectedValue == ENABLED ? true : false;
                    ProductService.UpdateProductStatusByProductIdList(ChosenProducts, status);

                    enbNotice.Message = "Product status was updated successfully.";
                    break;

                case CHANGE_DISCONTINUED:                    
                    bool discontinued = ddlActionDiscontinued.SelectedValue == ENABLED ? true : false;
                    ProductService.UpdateProductDiscontinuedStatusByProductIdList(ChosenProducts.ToArray(), discontinued);

                    enbNotice.Message = "Product discontinued status was updated successfully.";
                    break;

            }

            ddlActions.SelectedIndex = -1;

            ChosenProducts.Clear();
            NotChosenProducts.Clear();
            LoadProducts();
        }

        protected void ddlActions_PreRender(object sender, EventArgs e)
        {
            DropDownList dll = sender as DropDownList;
            dll.Attributes.Add("onchange", "check_actions(this);");
        }
    }
}