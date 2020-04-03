using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_featureditem_info : BasePage
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
            if (!IsPostBack)
                LoadItemInfo();            
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
                ProductOverviewModel product = e.Row.DataItem as ProductOverviewModel;
                CheckBox cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (ChosenProducts.Contains(product.Id))
                    cb.Checked = true;
            }
        }

        protected void gvProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            ProductSortingType orderBy = ProductSortingType.IdAsc;

            switch(e.SortExpression)
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
            {
                gvProducts.TopPagerRow.Visible = true;
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
            }
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
            SetState(CHOSEN_FILTER, (GetIntState(CATEGORY_ID) == AppConstant.DEFAULT_CATEGORY ? ANY : YES));

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
                        ltlProductName.Text = string.Format("<a href='/catalog/product_info.aspx?productid={0}' title='{1}'>{1}</a>", product.Id.ToString(), product.Name);
                        txtProductId.Text = product.Id.ToString();
                    }
                    mvProducts.ActiveViewIndex = 0;
                    break;
            }
        }

        protected void lbEditProductId_Click(object sender, EventArgs e)
        {
            mvProducts.ActiveViewIndex = 1;

            if (mvProducts.ActiveViewIndex == 1)
                LoadProducts();
        }

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            mvProducts.ActiveViewIndex = 0;
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            CampaignService.DeleteProductGroupMapping(QueryId);                   
            Response.Redirect("/marketing/cms_featureditem_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.FeaturedItemDeleted);
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var item = CampaignService.GetProductGroupMappingById(QueryId);
            
            item.ProductId = Convert.ToInt32(txtProductId.Text);
            item.Priority = Convert.ToInt32(txtPriority.Text.Trim());
            item.ProductGroupId = Convert.ToInt32(ddlGroups.SelectedValue);

            CampaignService.UpdateProductGroupMapping(item);
            
            enbNotice.Message = "Item was updated successfully.";
        }

        private void LoadItemInfo()
        {
            var item = CampaignService.GetProductGroupMappingById(QueryId);

            if (item == null)
                Response.Redirect("/marketing/cms_featureditem_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.FeaturedItemNotFound);

            var product = ProductService.GetProductOverviewModelById(item.ProductId);
            ltlTitle.Text = string.Format("{0} (ID: {1})", product.Name, product.Id);
            ltlProductName.Text = string.Format("<a href='/catalog/product_info.aspx?productid={0}' title='{1}'>{1}</a>", product.Id.ToString(), product.Name);
            txtProductId.Text = item.ProductId.ToString();
            var group = ddlGroups.Items.FindByValue(item.ProductGroupId.ToString());
            if (group != null) group.Selected = true;
            txtPriority.Text = item.Priority.ToString();            
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