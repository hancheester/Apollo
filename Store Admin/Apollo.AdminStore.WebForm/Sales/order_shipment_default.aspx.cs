using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_shipment_default : BasePage
    {
        public IOrderService OrderService { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            if (QueryOrderId <= 0)
                Response.Redirect("/sales/order_default.aspx");
            else
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", QueryOrderId.ToString());
                LoadShipments();
            }

            base.OnInit(e);
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(CARRIER_FILTER);
            DisposeState(TRACKING_REF_FILTER);
            
            LoadShipments();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(CARRIER_FILTER, ((TextBox)gvShipments.HeaderRow.FindControl("txtFilterCarrier")).Text.Trim());
            SetState(TRACKING_REF_FILTER, ((TextBox)gvShipments.HeaderRow.FindControl("txtFilterTrackingRef")).Text.Trim());
            
            LoadShipments();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvShipments.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvShipments.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvShipments.CustomPageIndex = gotoIndex;

            LoadShipments();
        }

        protected void gvShipments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvShipments.CustomPageIndex = gvShipments.CustomPageIndex + e.NewPageIndex;

            if (gvShipments.CustomPageIndex < 0)
                gvShipments.CustomPageIndex = 0;

            LoadShipments();
        }

        protected void gvShipments_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = OrderShipmentSortingType.OrderIdDesc;

            switch (e.SortExpression)
            {
                default:
                case "OrderId":
                    orderBy = OrderShipmentSortingType.OrderIdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderShipmentSortingType.OrderIdDesc;
                    break;
                case "Carrier":
                    orderBy = OrderShipmentSortingType.ShippingOptionNameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderShipmentSortingType.ShippingOptionNameAsc;
                    break;
                case "TrackingRef":
                    orderBy = OrderShipmentSortingType.TrackingRefDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderShipmentSortingType.TrackingRefDesc;
                    break;
            }
            
            SetState("OrderBy", (int)orderBy);            
            LoadShipments();
        }

        protected void gvShipments_PreRender(object sender, EventArgs e)
        {
            if (gvShipments.TopPagerRow != null)
            {
                gvShipments.TopPagerRow.Visible = true;                
                ((TextBox)gvShipments.HeaderRow.FindControl("txtFilterCarrier")).Text = GetStringState(CARRIER_FILTER);
                ((TextBox)gvShipments.HeaderRow.FindControl("txtFilterTrackingRef")).Text = GetStringState(TRACKING_REF_FILTER);                
            }
        }
        
        private void LoadShipments()
        {         
            string trackingRef = null;
            string shippingName = null;
            OrderShipmentSortingType orderBy = OrderShipmentSortingType.OrderIdDesc;
            
            if (HasState("OrderBy")) orderBy = (OrderShipmentSortingType)GetIntState("OrderBy");
            if (HasState(TRACKING_REF_FILTER)) trackingRef = GetStringState(TRACKING_REF_FILTER);
            if (HasState(CARRIER_FILTER)) shippingName = GetStringState(CARRIER_FILTER);

            var result = OrderService.GetPagedOrderShipmentOverviewModel(
                pageIndex: gvShipments.CustomPageIndex,
                pageSize: gvShipments.PageSize,
                orderIds: new int[] { QueryOrderId },
                shippingName: shippingName,
                trackingRef: trackingRef,
                orderBy: orderBy);

            if (result != null)
            {
                gvShipments.DataSource = result.Items;
                gvShipments.RecordCount = result.TotalCount;
                gvShipments.CustomPageCount = result.TotalPages;
            }

            gvShipments.DataBind();

            if (gvShipments.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}