using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class product_price_default : BasePage, ICallbackEventHandler
    {
        public IProductService ProductService { get; set; }
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadPrices();
        }

        protected void lbUpdateSelected_Click(object sender, EventArgs e)
        {
            SaveLastViewedPrices();
            
            ProductService.UpdateProductPriceOverviewModels(ChosenPrices);
            enbNotice.Message = "Prices were updated successfully";
            ChosenPrices.Clear();
            NotChosenPrices.Clear();
            LoadPrices();
        }

        protected void lbResetPriceFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            SetState(STATUS_FILTER, ANY);
            DisposeState(BARCODE_FILTER);
            
            enbNotice.Message = string.Empty;
            LoadPrices();
        }

        protected void lbSearchPrice_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterProductId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState(STATUS_FILTER, ((DropDownList)gvPrices.HeaderRow.FindControl("ddlStatus")).SelectedValue);
            SetState(BARCODE_FILTER, ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterBarcode")).Text.Trim());
            
            enbNotice.Message = string.Empty;
            LoadPrices();
        }

        protected void btnPriceGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvPrices.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvPrices.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvPrices.CustomPageIndex = gotoIndex;

            enbNotice.Message = string.Empty;
            LoadPrices();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }

        protected void gvPrices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedPrices();

            gvPrices.CustomPageIndex = gvPrices.CustomPageIndex + e.NewPageIndex;

            if (gvPrices.CustomPageIndex < 0)
                gvPrices.CustomPageIndex = 0;

            LoadPrices();
        }

        protected void gvPrices_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ProductPriceSortingType.ProductIdAsc;

            switch (e.SortExpression)
            {
                case "ProductId":
                    orderBy = ProductPriceSortingType.ProductIdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductPriceSortingType.ProductIdAsc;
                    break;
                case "Name":
                    orderBy = ProductPriceSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductPriceSortingType.NameAsc;
                    break;
                case "Weight":
                    orderBy = ProductPriceSortingType.WeightDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductPriceSortingType.WeightAsc;
                    break;
                case "Price":
                    orderBy = ProductPriceSortingType.PriceDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductPriceSortingType.PriceAsc;
                    break;
                case "Stock":
                    orderBy = ProductPriceSortingType.StockDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductPriceSortingType.StockAsc;
                    break;
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadPrices();
        }

        protected void gvPrices_PreRender(object sender, EventArgs e)
        {
            if (gvPrices.TopPagerRow != null)
            {
                gvPrices.TopPagerRow.Visible = true;
                ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterProductId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
                ((DropDownList)gvPrices.HeaderRow.FindControl("ddlStatus")).Items.FindByValue(GetStringState(STATUS_FILTER)).Selected = true;
                ((TextBox)gvPrices.HeaderRow.FindControl("txtFilterBarcode")).Text = GetStringState(BARCODE_FILTER);
            }

            for (int i = 0; i < gvPrices.Rows.Count; i++)
            {
                CheckBox cb = gvPrices.Rows[i].FindControl("cbChosen") as CheckBox;
                ProductPriceOverviewModel price = BuildPrice(Convert.ToInt32(gvPrices.DataKeys[i].Value), gvPrices.Rows[i]);

                if (cb != null) SetChosenPrices(price, cb.Checked);
            }

            if (gvPrices.Rows.Count == 1 && (int)gvPrices.DataKeys[0].Value == 0 && gvPrices.TopPagerRow != null)
            {
                gvPrices.TopPagerRow.FindControl(PH_RECORD_FOUND).Visible = false;
                gvPrices.TopPagerRow.FindControl(PH_RECORD_NOT_FOUND).Visible = true;
            }
        }

        protected void gvPrices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var price = (ProductPriceOverviewModel)e.Row.DataItem;
                ProductPriceOverviewModel chosenPrice = null;

                if (ExistInChosenPrices(price))
                {
                    chosenPrice = ChosenPrices.Find(delegate (ProductPriceOverviewModel item)
                    {
                        return item.Id == price.Id;
                    });
                }

                CheckBox cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (chosenPrice != null)
                {
                    TextBox txtWeight = e.Row.FindControl("txtWeight") as TextBox;
                    TextBox txtPrice = e.Row.FindControl("txtPrice") as TextBox;
                    TextBox txtStock = e.Row.FindControl("txtStock") as TextBox;
                    TextBox txtBarcode = e.Row.FindControl("txtBarcode") as TextBox;
                    
                    cb.Checked = true;

                    txtWeight.Text = chosenPrice.Weight.ToString();
                    txtPrice.Text = AdminStoreUtility.GetFormattedPrice(chosenPrice.Price, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.None);
                    txtStock.Text = chosenPrice.Stock.ToString();
                    txtBarcode.Text = chosenPrice.Barcode;
                }
            }
        }
        
        private void LoadPrices()
        {
            int[] productIds = null;
            string name = null;
            bool? productEnabled = null;
            string barcode = null;
            ProductPriceSortingType orderBy = ProductPriceSortingType.ProductIdDesc;

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
            if (GetStringState(STATUS_FILTER) == ENABLED) productEnabled = true;
            if (GetStringState(STATUS_FILTER) == DISABLED) productEnabled = false;
            if (HasState(BARCODE_FILTER)) barcode = GetStringState(BARCODE_FILTER);
            if (HasState("OrderBy")) orderBy = (ProductPriceSortingType)GetIntState("OrderBy");

            var result = ProductService.GetPagedProductPriceOverviewModels(
                pageIndex: gvPrices.CustomPageIndex,
                pageSize: gvPrices.PageSize,
                productIds: productIds,
                name: name,
                barcode: barcode,
                productEnabled: productEnabled,
                orderBy: orderBy);

            if (result != null)
            {
                gvPrices.DataSource = result.Items;
                gvPrices.RecordCount = result.TotalCount;
                gvPrices.CustomPageCount = result.TotalPages;
            }

            gvPrices.DataBind();

            if (gvPrices.Rows.Count <= 0) enbNotice.Message = "No records found.";            
        }

        private void SaveLastViewedPrices()
        {
            for (int i = 0; i < gvPrices.DataKeys.Count; i++)
            {
                CheckBox cb = gvPrices.Rows[i].FindControl("cbChosen") as CheckBox;
                var productPriceId = Convert.ToInt32(gvPrices.DataKeys[i].Value);
                var price = BuildPrice(productPriceId, gvPrices.Rows[i]);
                
                if (cb != null) SetChosenPrices(price, cb.Checked);
            }
        }

        private void SetChosenPrices(ProductPriceOverviewModel price, bool chosen)
        {
            if (price != null)
            {
                if (chosen)
                {
                    if (ExistInChosenPrices(price))
                    {
                        ChosenPrices.RemoveAll(delegate (ProductPriceOverviewModel arg)
                        {
                            return arg.Id == price.Id;
                        });
                    }

                    ChosenPrices.Add(price);
                    NotChosenPrices.RemoveAll(delegate (ProductPriceOverviewModel arg)
                    {
                        return arg.Id == price.Id;
                    });
                }
                else
                {
                    ChosenPrices.RemoveAll(delegate (ProductPriceOverviewModel arg)
                    {
                        return arg.Id == price.Id;
                    });

                    if (ExistInNotChosenPrices(price))
                    {
                        NotChosenPrices.RemoveAll(delegate (ProductPriceOverviewModel arg)
                        {
                            return arg.Id == price.Id;
                        });
                    }

                    NotChosenPrices.Add(price);
                }
            }
        }

        private bool ExistInChosenPrices(ProductPriceOverviewModel price)
        {
            bool exist = false;

            for (int j = 0; j < ChosenPrices.Count; j++)
            {
                exist = IsEqual(ChosenPrices[j], price);

                if (!exist) exist = ChosenPrices[j].Id == price.Id;

                if (exist) break;
            }

            return exist;
        }

        private bool ExistInNotChosenPrices(ProductPriceOverviewModel price)
        {
            bool exist = false;

            for (int j = 0; j < NotChosenPrices.Count; j++)
            {
                exist = IsEqual(NotChosenPrices[j], price);

                if (!exist) exist = NotChosenPrices[j].Id == price.Id;

                if (exist) break;
            }

            return exist;
        }

        private bool IsEqual(ProductPriceOverviewModel item1, ProductPriceOverviewModel item2)
        {
            PropertyInfo[] properties1 = item1.GetType().GetProperties();
            PropertyInfo[] properties2 = item2.GetType().GetProperties();
            object value1;
            object value2;

            Type type = item1.GetType();
            bool isEqual = false;

            for (int i = 0; i < properties1.Length; i++)
            {
                value1 = properties1[i].GetValue(item1, null);
                value2 = properties2[i].GetValue(item2, null);

                if (value1 != null && value2 != null)
                    isEqual = value1.Equals(value2);

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        private ProductPriceOverviewModel BuildPrice(int productPriceId, GridViewRow row)
        {
            var price = ProductService.GetProductPriceOverviewModel(productPriceId);

            if (price != null)
            {
                // Fetch weight
                TextBox txtWeight = row.FindControl("txtWeight") as TextBox;
                if (!string.IsNullOrEmpty(txtWeight.Text.Trim()))
                    price.Weight = Convert.ToInt32(txtWeight.Text.Trim());

                // Fetch price
                TextBox txtPrice = row.FindControl("txtPrice") as TextBox;
                if (!string.IsNullOrEmpty(txtPrice.Text.Trim()))
                    price.Price = Convert.ToDecimal(txtPrice.Text.Trim());

                // Fetch stock
                TextBox txtStock = row.FindControl("txtStock") as TextBox;
                if (!string.IsNullOrEmpty(txtStock.Text.Trim()))
                    price.Stock = Convert.ToInt32(txtStock.Text.Trim());

                // Fetch Barcode
                TextBox txtBarcode = row.FindControl("txtBarcode") as TextBox;
                price.Barcode = txtBarcode.Text.Trim();
                
                return price;
            }

            return null;
        }

        #region ICallbackEventHandler Members

        private string message;
        private const char splitter = '_';

        public string GetCallbackResult()
        {
            return message;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(splitter);
            var branchId = Convert.ToInt32(args[0]);
            var message = string.Empty;

            if (message == string.Empty)
                message = "n/a";
        }

        #endregion
    }
}