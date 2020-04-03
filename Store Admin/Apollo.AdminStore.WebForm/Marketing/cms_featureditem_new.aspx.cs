using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_featureditem_new : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        public IProductService ProductService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            var groups = CampaignService.GetProductGroups();
            ddlGroups.DataTextField = "Name";
            ddlGroups.DataValueField = "Id";
            ddlGroups.DataSource = groups;
            ddlGroups.DataBind();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var product = e.Row.DataItem as ProductOverviewModel;
                var cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (ChosenProducts.Contains(product.Id))
                    cb.Checked = true;
            }
        }

        protected void gvProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            ProductSortingType orderBy = ProductSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Name":
                    orderBy = ProductSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.NameAsc;
                    break;
                case "Id":
                default:
                    orderBy = ProductSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductSortingType.IdAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadProducts();
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

            LoadProducts();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            SetState(CHOSEN_FILTER, (GetIntState(CATEGORY_ID) == 0 ? ANY : YES));

            LoadProducts();
        }
        
        protected void lbSave_Click(object sender, EventArgs e)
        {
            var newItem = new ProductGroupMapping
            {
                ProductId = Convert.ToInt32(txtProductId.Text.Trim()),
                ProductGroupId = Convert.ToInt32(ddlGroups.SelectedValue),
                Priority = Convert.ToInt32(txtPriority.Text.Trim())                
            };

            int id = CampaignService.InsertProductGroupMapping(newItem);

            Response.Redirect("/marketing/cms_featureditem_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.FeaturedItemCreated + "&" + QueryKey.ID + "=" + id);
        }

        protected void lbEditProductId_Click(object sender, EventArgs e)
        {
            mvProducts.ActiveViewIndex = 1;

            if (mvProducts.ActiveViewIndex == 1)
                LoadProducts();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DisposeState(NAME_FILTER);
            DisposeState(PRODUCT_ID_FILTER);

            LoadProducts();
        }

        protected void gvProducts_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "select":
                    txtProductId.Text = e.CommandArgument.ToString();

                    var product = ProductService.GetProductOverviewModelById(Convert.ToInt32(txtProductId.Text));

                    if (product != null)
                    {
                        ltlProductId.Text = product.Name;
                        txtProductId.Text = product.Id.ToString();
                    }

                    mvProducts.ActiveViewIndex = 0;

                    break;
            }
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
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
            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState("OrderBy")) orderBy = (ProductSortingType)GetIntState("OrderBy");

            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
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

            if (gvProducts.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}