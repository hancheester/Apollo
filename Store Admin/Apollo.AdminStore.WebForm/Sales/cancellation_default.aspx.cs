using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class cancellation_default : BasePage
    {
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadRefunds();            
        }

        protected void lbResetRefundFilter_Click(object sender, EventArgs e)
        {
            DisposeState(ORDER_ID_FILTER);
            DisposeState(FROM_DATE_FILTER);
            DisposeState(TO_DATE_FILTER);
            SetState(STATUS_CODE_FILTER, string.Empty);

            LoadRefunds();
        }

        protected void lbSearchRefund_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ID_FILTER, ((TextBox)gvRefunds.HeaderRow.FindControl("txtFilterOrderId")).Text.Trim());            
            SetState(FROM_DATE_FILTER, ((TextBox)gvRefunds.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_DATE_FILTER, ((TextBox)gvRefunds.HeaderRow.FindControl("txtToDate")).Text.Trim());
            SetState(STATUS_CODE_FILTER, ((DropDownList)gvRefunds.HeaderRow.FindControl("ddlStatus")).SelectedValue);

            LoadRefunds();
        }

        protected void btnRefundGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvRefunds.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvRefunds.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvRefunds.CustomPageIndex = gotoIndex;

            LoadRefunds();
        }

        protected void gvRefunds_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRefunds.CustomPageIndex = gvRefunds.CustomPageIndex + e.NewPageIndex;

            if (gvRefunds.CustomPageIndex < 0)
                gvRefunds.CustomPageIndex = 0;

            LoadRefunds();
        }

        protected void gvRefunds_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = RefundSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "OrderId":
                    orderBy = RefundSortingType.OrderIdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = RefundSortingType.OrderIdAsc;
                    break;
                case "DateStamp":
                    orderBy = RefundSortingType.DateStampDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = RefundSortingType.DateStampAsc;
                    break;
                case "IsCompleted":
                    orderBy = RefundSortingType.IsCompletedDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = RefundSortingType.IsCompletedAsc;
                    break;
                case "Id":
                default:
                    orderBy = RefundSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = RefundSortingType.IdAsc;
                    break;
            }
            SetState("OrderBy", (int)orderBy);
            LoadRefunds();
        }

        protected void gvRefunds_PreRender(object sender, EventArgs e)
        {
            if (gvRefunds.TopPagerRow != null)
            {
                gvRefunds.TopPagerRow.Visible = true;
                ((TextBox)gvRefunds.HeaderRow.FindControl("txtFilterOrderId")).Text = GetStringState(ORDER_ID_FILTER);
                ((TextBox)gvRefunds.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_DATE_FILTER);
                ((TextBox)gvRefunds.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_DATE_FILTER);
                ((DropDownList)gvRefunds.HeaderRow.FindControl("ddlStatus")).Items.FindByValue(GetStringState(STATUS_CODE_FILTER)).Selected = true;
            }
        }

        protected string GetShippingCountryImage(int orderId)
        {
            var shipping = OrderService.GetShippingOverviewModelByOrderId(orderId);

            if (shipping != null)
            {
                return AdminStoreUtility.GetShippingCountryImage(shipping.ShippingCountryId);
            }
            return string.Empty;
        }

        private void LoadRefunds()
        {
            int[] orderIds = null;
            bool isCompleted = false;
            string fromDate = null;
            string toDate = null;
            RefundSortingType orderBy = RefundSortingType.IdDesc;

            if (HasState("OrderBy")) orderBy = (RefundSortingType)GetIntState("OrderBy");
            if (HasState(ORDER_ID_FILTER))
            {
                string value = GetStringState(ORDER_ID_FILTER);
                int temp;
                orderIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(FROM_DATE_FILTER)) fromDate = GetStringState(FROM_DATE_FILTER);
            if (HasState(TO_DATE_FILTER)) toDate = GetStringState(TO_DATE_FILTER);
            if (HasState(STATUS_CODE_FILTER) && GetStringState(STATUS_CODE_FILTER) != string.Empty)
            {
                string status = GetStringState(STATUS_CODE_FILTER);

                if (status == AppConstant.ZERO)
                    isCompleted = false;
                else if (status == AppConstant.ONE)
                    isCompleted = true;
            }

            var result = OrderService.GetRefundLoadPaged(
                pageIndex: gvRefunds.CustomPageIndex,
                pageSize: gvRefunds.PageSize,
                orderIds: orderIds,
                isCancellation: true,
                isCompleted: isCompleted,
                fromDateStamp: fromDate,
                toDateStamp: toDate,
                orderBy: orderBy);

            if (result != null)
            {
                gvRefunds.DataSource = result.Items;
                gvRefunds.RecordCount = result.TotalCount;
                gvRefunds.CustomPageCount = result.TotalPages;
            }

            gvRefunds.DataBind();

            if (gvRefunds.Rows.Count <= 0) enbNotice.Message = "No records found.";           
        }
    }
}