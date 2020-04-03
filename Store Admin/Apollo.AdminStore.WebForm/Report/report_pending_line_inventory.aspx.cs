using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Report
{
    public partial class report_pending_line_inventory : BasePage
    {
        public IOrderService OrderService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadPendingLines();
        }

        protected void lbDownloadPendingItems_Click(object sender, EventArgs e)
        {
            byte[] bytes = OrderService.PrintInventoryPendingLinePdf();
            
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("inventory_pending_{0:ddMMyyyyHHmmss}.pdf", DateTime.Now));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(bytes);
            Response.End();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);

            LoadPendingLines();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvResults.HeaderRow.FindControl("txtFilterProductId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvResults.HeaderRow.FindControl("txtFilterName")).Text.Trim());

            LoadPendingLines();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvResults.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvResults.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvResults.CustomPageIndex = gotoIndex;

            LoadPendingLines();
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.CustomPageIndex = gvResults.CustomPageIndex + e.NewPageIndex;

            if (gvResults.CustomPageIndex < 0)
                gvResults.CustomPageIndex = 0;

            LoadPendingLines();
        }

        protected void gvResults_PreRender(object sender, EventArgs e)
        {
            if (gvResults.TopPagerRow != null)
            {
                gvResults.TopPagerRow.Visible = true;
                ((TextBox)gvResults.HeaderRow.FindControl("txtFilterProductId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvResults.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);                
            }            
        }

        protected string GetOrderList(object relatedOrder)
        {
            var orderList = relatedOrder as IList<int>;
            var sb = new StringBuilder();

            if (orderList != null && orderList.Count > 0)
            {
                sb.Append("<table class='table table-bordered'><tr><td>");

                for (int j = 1; j <= orderList.Count; j++)
                {
                    if (j % 4 == 0)
                    {
                        sb.AppendFormat("<a class='label label-success hidden-link' href='/sales/order_info.aspx?orderid={0}' target='_blank'>{0}</a><br/>", orderList[j - 1]);
                    }
                    else
                    {
                        sb.AppendFormat("<a class='label label-success hidden-link' href='/sales/order_info.aspx?orderid={0}' target='_blank'>{0}</a>&nbsp;", orderList[j - 1]);
                    }
                }

                sb.Append("</td></tr></table>");
            }
            
            return sb.ToString();
        }

        private void LoadPendingLines()
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
            
            var result = OrderService.GetInventoryPendingLinesLoadPaged(
                pageIndex: gvResults.CustomPageIndex,
                pageSize: gvResults.PageSize,
                productIds: productIds,
                name: name);

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