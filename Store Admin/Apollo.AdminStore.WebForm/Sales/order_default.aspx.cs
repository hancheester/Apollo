using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_default : BasePage
    {
        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected bool DisplayAdvancedSearch { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            enbNotice.Message = string.Empty;

            if (!Page.IsPostBack)
            {
                if (QueryOrderIssueCode != string.Empty)
                    SetState(ISSUE_FILTER, QueryOrderIssueCode.ToUpper());

                if (QueryOrderStatusCode != string.Empty)
                    SetState(ORDER_STATUS_FILTER, QueryOrderStatusCode.ToUpper());

                if (QueryAddress != string.Empty)
                    SetState(ORDER_ADDRESS_FILTER, QueryAddress.ToString());
                
                LoadOrders();
            }
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(ORDER_ID_FILTER); ;
            DisposeState(FROM_ORDER_PLACED_DATE_FILTER);
            DisposeState(TO_ORDER_PLACED_DATE_FILTER);
            DisposeState(FROM_LAST_ACTIVITY_DATE_FILTER);
            DisposeState(TO_LAST_ACTIVITY_DATE_FILTER);
            DisposeState(EMAIL_FILTER);
            DisposeState(ORDER_STATUS_FILTER);
            DisposeState(ISSUE_FILTER);
            DisposeState(SHIPPING_COUNTRY_ID_FILTER);
            DisposeState(SHIPPING_NAME_FILTER);
            DisposeState(ORDER_ADDRESS_FILTER);

            cgOrders.CustomPageIndex = 0;

            LoadOrders();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ID_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(FROM_ORDER_PLACED_DATE_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_ORDER_PLACED_DATE_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtToDate")).Text.Trim());
            SetState(FROM_LAST_ACTIVITY_DATE_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtFromActivityDate")).Text.Trim());
            SetState(TO_LAST_ACTIVITY_DATE_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtToActivityDate")).Text.Trim());
            SetState(EMAIL_FILTER, ((TextBox)cgOrders.HeaderRow.FindControl("txtEmail")).Text.Trim());
            
            DropDownList ddlCountries = (DropDownList)cgOrders.HeaderRow.FindControl("ddlCountries");
            SetState(SHIPPING_COUNTRY_ID_FILTER, ddlCountries.SelectedIndex != 0 ? ddlCountries.SelectedValue.ToString() : string.Empty);

            DropDownList ddlStatus = (DropDownList)cgOrders.HeaderRow.FindControl("ddlStatus");
            SetState(ORDER_STATUS_FILTER, ddlStatus.SelectedIndex != 0 ? ddlStatus.SelectedValue.ToString() : string.Empty);

            DropDownList ddlDelivery = (DropDownList)cgOrders.HeaderRow.FindControl("ddlDelivery");
            SetState(SHIPPING_NAME_FILTER, ddlDelivery.SelectedIndex != 0 ? ddlDelivery.SelectedValue.ToString() : string.Empty);

            DropDownList ddlIssue = (DropDownList)cgOrders.HeaderRow.FindControl("ddlIssue");
            SetState(ISSUE_FILTER, ddlIssue.SelectedIndex != 0 ? ddlIssue.SelectedValue.ToString() : string.Empty);

            cgOrders.CustomPageIndex = 0;
            
            LoadOrders();
        }

        protected void lbAdvancedSearch_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ADDRESS_FILTER, txtAddress.Text.Trim());
            DisplayAdvancedSearch = true;
            LoadOrders();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)cgOrders.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((cgOrders.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                cgOrders.CustomPageIndex = gotoIndex;

            LoadOrders();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }
        
        protected void ddlStatus_Init(object sender, EventArgs e)
        {
            var list = OrderService.GetOrderStatusList().OrderBy(x => x.Status).ToList();
            list.Insert(0, new OrderStatus { Status = AppConstant.DEFAULT_SELECT });

            DropDownList ddl = (DropDownList)sender;
            ddl.DataSource = list;
            ddl.DataBind();
        }

        protected void ddlCountries_Init(object sender, EventArgs e)
        {
            var list = ShippingService.GetCountries().ToList();
            list.Insert(0, new Country { Name = AppConstant.DEFAULT_SELECT });

            DropDownList ddl = (DropDownList)sender;
            ddl.DataSource = list;
            ddl.DataBind();
        }

        protected void ddlIssue_Init(object sender, EventArgs e)
        {
            var list = OrderService.GetOrderIssueList().OrderBy(x => x.Issue).ToList();
            list.Insert(0, new OrderIssue { Issue = AppConstant.DEFAULT_SELECT });
            DropDownList ddl = (DropDownList)sender;            
            ddl.DataSource = list;
            ddl.DataBind();
        }
        
        protected void ddlDelivery_Init(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl = AdminStoreUtility.GenerateDeliveryList(ddl);
        }
        
        protected string GetTrafficLightColour(object objLastAlert, object objStatusCode, object objOrderId)
        {
            string statusCode = (string)objStatusCode;
            int orderId = (int)objOrderId;

            if (statusCode == OrderStatusCode.STOCK_WARNING)
            {
                var lines = OrderService.GetLineItemOverviewModelListByOrderId(orderId);

                bool found = lines.Any(l => l.StatusCode == LineStatusCode.STOCK_WARNING || l.StatusCode == LineStatusCode.AWAITING_STOCK);

                if (found)
                {
                    DateTime lastAlert = (DateTime)objLastAlert;

                    if (lastAlert.DayOfWeek == DayOfWeek.Saturday)
                        lastAlert = lastAlert.AddDays(2);
                    else if (lastAlert.DayOfWeek == DayOfWeek.Sunday)
                        lastAlert = lastAlert.AddDays(1);

                    TimeSpan span = DateTime.Now - lastAlert;

                    if (span.TotalDays >= AppConstant.RED_ALERT_DUE)
                        return "red";

                    if (span.TotalDays >= AppConstant.ORANGE_ALERT_DUE)
                        return "orange";
                }
            }

            return string.Empty;
        }
        
        protected void btnExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveLastViewedOrder();

                byte[] bytes = OrderService.ExportOrdersCsv(ChosenOrders);

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("orders_{0:ddMMyyyyHHmmss}.csv", DateTime.Now));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {
                enbNotice.Message = "Sorry, csv file was failed to export. " + ex.Message;
            }
        }
        
        protected void cgOrders_PreRender(object sender, EventArgs e)
        {
            if (cgOrders.TopPagerRow != null)
            {
                cgOrders.TopPagerRow.Visible = true;
                ((TextBox)cgOrders.HeaderRow.FindControl("txtFilterId")).Text = GetStringState("OrderId");
                ((TextBox)cgOrders.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_ORDER_PLACED_DATE_FILTER);
                ((TextBox)cgOrders.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_ORDER_PLACED_DATE_FILTER);
                ((TextBox)cgOrders.HeaderRow.FindControl("txtFromActivityDate")).Text = GetStringState(FROM_LAST_ACTIVITY_DATE_FILTER);
                ((TextBox)cgOrders.HeaderRow.FindControl("txtToActivityDate")).Text = GetStringState(TO_LAST_ACTIVITY_DATE_FILTER);
                ((TextBox)cgOrders.HeaderRow.FindControl("txtEmail")).Text = GetStringState(EMAIL_FILTER);
                
                DropDownList ddlCountries = (DropDownList)cgOrders.HeaderRow.FindControl("ddlCountries");
                if (GetStringState(SHIPPING_COUNTRY_ID_FILTER) != string.Empty) ddlCountries.Items.FindByValue(GetStringState(SHIPPING_COUNTRY_ID_FILTER)).Selected = true;

                DropDownList ddlStatus = (DropDownList)cgOrders.HeaderRow.FindControl("ddlStatus");
                if (GetStringState(ORDER_STATUS_FILTER) != string.Empty) ddlStatus.Items.FindByValue(GetStringState(ORDER_STATUS_FILTER)).Selected = true;

                DropDownList ddlIssue = (DropDownList)cgOrders.HeaderRow.FindControl("ddlIssue");
                if (GetStringState(ISSUE_FILTER) != string.Empty) ddlIssue.Items.FindByValue(GetStringState(ISSUE_FILTER)).Selected = true;

                DropDownList ddlDelivery = (DropDownList)cgOrders.HeaderRow.FindControl("ddlDelivery");
                if (GetStringState(SHIPPING_NAME_FILTER) != string.Empty) ddlDelivery.Items.FindByValue(GetStringState(SHIPPING_NAME_FILTER)).Selected = true;                               
            }

            var orders = cgOrders.DataSource as OrderOverviewModel[];
            if (orders != null)
            {
                for (int i = 0; i < cgOrders.Rows.Count; i++)
                {
                    CheckBox cb = cgOrders.Rows[i].FindControl("cbChosen") as CheckBox;

                    if (ChosenOrders.Contains(orders[i].Id))
                        cb.Checked = true;

                    SetChosenOrders(orders[i].Id, cb.Checked);
                }
            }
        }

        protected void cgOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedOrder();

            cgOrders.CustomPageIndex = cgOrders.CustomPageIndex + e.NewPageIndex;
            if (cgOrders.CustomPageIndex < 0) cgOrders.CustomPageIndex = 0;

            LoadOrders();
        }
        
        protected void cgOrders_Sorting(object sender, GridViewSortEventArgs e)
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
                case "LastActivityDate":
                    orderBy = OrderSortingType.LastActivityDateDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.LastActivityDateAsc;
                    break;
                case "Email":
                    orderBy = OrderSortingType.EmailDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.EmailAsc;
                    break;
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadOrders();            
        }

        private void LoadOrders()
        {
            int[] orderIds = null;
            string emails = null;
            string fromOrderPlacedOn = null;
            string toOrderPlacedOn = null;
            string fromLastActivityDate = null;
            string toLastActivityDate = null;
            string[] statusCodes = null;
            string issueCode = null;
            string address = null;
            int shippingCountryId = 0;
            string shippingName = null;
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
            
            if (HasState(FROM_ORDER_PLACED_DATE_FILTER)) fromOrderPlacedOn = GetStringState(FROM_ORDER_PLACED_DATE_FILTER);
            if (HasState(TO_ORDER_PLACED_DATE_FILTER)) toOrderPlacedOn = GetStringState(TO_ORDER_PLACED_DATE_FILTER);
            if (HasState(FROM_LAST_ACTIVITY_DATE_FILTER)) fromLastActivityDate = GetStringState(FROM_LAST_ACTIVITY_DATE_FILTER);
            if (HasState(TO_LAST_ACTIVITY_DATE_FILTER)) toLastActivityDate = GetStringState(TO_LAST_ACTIVITY_DATE_FILTER);
            if (HasState(EMAIL_FILTER)) emails = GetStringState(EMAIL_FILTER);
            if (HasState(SHIPPING_NAME_FILTER)) shippingName = GetStringState(SHIPPING_NAME_FILTER);
            if (HasState(SHIPPING_COUNTRY_ID_FILTER)) shippingCountryId = GetIntState(SHIPPING_COUNTRY_ID_FILTER);
            if (HasState(ORDER_STATUS_FILTER)) statusCodes = new string[] { GetStringState(ORDER_STATUS_FILTER) };
            if (HasState(ISSUE_FILTER)) issueCode = GetStringState(ISSUE_FILTER);
            if (HasState(ORDER_ADDRESS_FILTER)) address = GetStringState(ORDER_ADDRESS_FILTER);

            var result = OrderService.GetPagedOrderOverviewModel(
                pageIndex: cgOrders.CustomPageIndex,
                pageSize: cgOrders.PageSize,
                orderIds: orderIds,
                fromOrderPlacedOn: fromOrderPlacedOn,
                toOrderPlacedOn: toOrderPlacedOn,
                fromLastActivity: fromLastActivityDate,
                toLastActivity: toLastActivityDate,
                emails: emails,
                shippingOptionName: shippingName,
                shippingCountryId: shippingCountryId,
                statusCodes: statusCodes,
                issueCode: issueCode,
                address: address,
                orderBy: orderBy);

            if (result != null)
            {
                cgOrders.DataSource = result.Items;
                cgOrders.RecordCount = result.TotalCount;
                cgOrders.CustomPageCount = result.TotalPages;
            }

            cgOrders.DataBind();

            if (cgOrders.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private void SaveLastViewedOrder()
        {
            int orderId;

            for (int i = 0; i < cgOrders.DataKeys.Count; i++)
            {
                CheckBox cb = cgOrders.Rows[i].FindControl("cbChosen") as CheckBox;
                orderId = Convert.ToInt32(cgOrders.DataKeys[i].Value);

                if (cb != null) SetChosenOrders(orderId, cb.Checked);
            }
        }

        private void SetChosenOrders(int orderId, bool chosen)
        {
            if (orderId != 0)
            {
                if ((chosen) && !ChosenOrders.Contains(orderId))
                {
                    ChosenOrders.Add(orderId);
                    NotChosenOrders.Remove(orderId);
                }
                else if ((!chosen) && (ChosenOrders.Contains(orderId)))
                {
                    ChosenOrders.Remove(orderId);
                    NotChosenOrders.Add(orderId);
                }
            }
        }

        
    }
}