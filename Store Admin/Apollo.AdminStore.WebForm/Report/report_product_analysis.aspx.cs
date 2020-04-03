using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Report
{
    public partial class report_product_analysis : BasePage
    {
        public IReportService ReportService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtToDate.Text = DateTime.Today.ToString(AppConstant.DATE_FORM1);
                txtFromDate.Text = DateTime.Today.AddDays(-6).ToString(AppConstant.DATE_FORM1);

                LoadProducts();
            }            
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);

            LoadProducts();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvResults.HeaderRow.FindControl("txtFilterProductId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvResults.HeaderRow.FindControl("txtFilterName")).Text.Trim());

            LoadProducts();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvResults.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvResults.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvResults.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.CustomPageIndex = gvResults.CustomPageIndex + e.NewPageIndex;

            if (gvResults.CustomPageIndex < 0)
                gvResults.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvResults_PreRender(object sender, EventArgs e)
        {
            if (gvResults.TopPagerRow != null)
            {
                gvResults.TopPagerRow.Visible = true;
            }

            ((TextBox)gvResults.HeaderRow.FindControl("txtFilterProductId")).Text = GetStringState(PRODUCT_ID_FILTER);
            ((TextBox)gvResults.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
        }

        protected string BuildTopCountries(object item)
        {
            var topCountries = item as Dictionary<string, int>;
            var sb = new StringBuilder();

            if (topCountries != null && topCountries.Count > 0)
            {
                foreach (var country in topCountries)
                {
                    sb.AppendFormat("{0}({1}) ", AdminStoreUtility.GetShippingCountryImage(country.Key), country.Value);
                }                
            }

            return sb.ToString();
        }

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;

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

            string fromDate = null;
            string toDate = null;

            if (txtFromDate.Text.Trim().Length != 0)
            {
                fromDate = txtFromDate.Text.Trim();
            }

            if (txtToDate.Text.Trim().Length != 0)
            {
                toDate = txtToDate.Text.Trim();
            }

            var result = ReportService.GetProductAnalysis(
                pageIndex: gvResults.CustomPageIndex,
                pageSize: gvResults.PageSize,
                productIds: productIds,
                name: name,
                fromDateStamp: fromDate,
                toDateStamp: toDate);

            if (result != null)
            {
                gvResults.DataSource = result.Items;
                gvResults.RecordCount = result.TotalCount;
                gvResults.CustomPageCount = result.TotalPages;
            }

            gvResults.DataBind();

            if (gvResults.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}