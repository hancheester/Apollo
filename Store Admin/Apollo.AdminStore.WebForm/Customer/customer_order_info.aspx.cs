using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_order_info : BasePage
    {
        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
       
        protected override void OnInit(EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId <= 0)
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountIdInvalid);

            Account account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);
                LoadOrders();
            }
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);
            
            base.OnInit(e);
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(ORDER_ID_FILTER); ;
            DisposeState(FROM_ORDER_PLACED_DATE_FILTER);
            DisposeState(TO_ORDER_PLACED_DATE_FILTER);
            DisposeState(ORDER_STATUS_FILTER);

            LoadOrders();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ID_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(FROM_ORDER_PLACED_DATE_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_ORDER_PLACED_DATE_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtToDate")).Text.Trim());
            
            DropDownList ddl = (DropDownList)gvOrders.HeaderRow.FindControl("ddlStatus");
            SetState(ORDER_STATUS_FILTER, ddl.SelectedIndex != 0 ? ddl.SelectedValue.ToString() : string.Empty);

            LoadOrders();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvOrders.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvOrders.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvOrders.CustomPageIndex = gotoIndex;

            LoadOrders();
        }

        protected void ddlStatus_Init(object sender, EventArgs e)
        {
            var list = OrderService.GetOrderStatusList().OrderBy(x => x.Status).ToList();
            list.Insert(0, new OrderStatus { Status = AppConstant.DEFAULT_SELECT });

            DropDownList ddl = (DropDownList)sender;
            ddl.DataSource = list;
            ddl.DataBind();
        }

        protected void gvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrders.CustomPageIndex = gvOrders.CustomPageIndex + e.NewPageIndex;

            if (gvOrders.CustomPageIndex < 0)
                gvOrders.CustomPageIndex = 0;

            LoadOrders();
        }

        protected void gvOrders_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = OrderSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = OrderSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.IdAsc;
                    break;
                case "OrderPlaced":
                    orderBy = OrderSortingType.OrderPlacedDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.OrderPlacedAsc;
                    break;
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadOrders();
        }

        protected void gvOrders_PreRender(object sender, EventArgs e)
        {
            if (gvOrders.TopPagerRow != null)
            {
                gvOrders.TopPagerRow.Visible = true;
                ((TextBox)gvOrders.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(ORDER_ID_FILTER);
                ((TextBox)gvOrders.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_ORDER_PLACED_DATE_FILTER);
                ((TextBox)gvOrders.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_ORDER_PLACED_DATE_FILTER);

                DropDownList ddl = (DropDownList)gvOrders.HeaderRow.FindControl("ddlStatus");
                if (GetStringState(ORDER_STATUS_FILTER) != string.Empty) ddl.Items.FindByValue(GetStringState(ORDER_STATUS_FILTER)).Selected = true;
            }
        }

        private void LoadOrders()
        {
            int[] orderIds = null;
            int[] userIds = null;
            string[] statusCodes = null;
            string fromOrderPlacedOn = null;
            string toOrderPlacedOn = null;
            OrderSortingType orderBy = OrderSortingType.IdDesc;

            if (HasState("OrderBy")) orderBy = (OrderSortingType)GetIntState("OrderBy");
            if (HasState(ORDER_ID_FILTER))
            {
                string value = GetStringState(ORDER_ID_FILTER);
                int temp;
                orderIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(FROM_ORDER_PLACED_DATE_FILTER)) fromOrderPlacedOn = DateTime.ParseExact(GetStringState(FROM_ORDER_PLACED_DATE_FILTER), AppConstant.DATE_FORM1, CultureInfo.InvariantCulture).ToString(AppConstant.DATE_FORM2);
            if (HasState(TO_ORDER_PLACED_DATE_FILTER)) toOrderPlacedOn = DateTime.ParseExact(GetStringState(TO_ORDER_PLACED_DATE_FILTER), AppConstant.DATE_FORM1, CultureInfo.InvariantCulture).AddDays(1D).ToString(AppConstant.DATE_FORM2);
            if (HasState(ORDER_STATUS_FILTER)) statusCodes = new string[] { GetStringState(ORDER_STATUS_FILTER) };
            userIds = new int[] { QueryUserId };

            var result = OrderService.GetPagedOrderOverviewModel(
                pageIndex: gvOrders.CustomPageIndex,
                pageSize: gvOrders.PageSize,
                orderIds: orderIds,
                profileIds: userIds,
                fromOrderPlacedOn: fromOrderPlacedOn,
                toOrderPlacedOn: toOrderPlacedOn,
                statusCodes: statusCodes,
                orderBy: orderBy);

            if (result != null)
            {
                gvOrders.DataSource = result.Items;
                gvOrders.RecordCount = result.TotalCount;
                gvOrders.CustomPageCount = result.TotalPages;
            }

            gvOrders.DataBind();

            if (gvOrders.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadOrders();
        }        
    }
}