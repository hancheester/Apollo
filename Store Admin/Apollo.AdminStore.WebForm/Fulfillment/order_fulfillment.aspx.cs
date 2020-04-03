using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.FulFillment
{
    public partial class order_fulfillment : BasePage, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string notice = string.Empty;

            if (!IsPostBack)
            {
                LoadOrders();
            }

            enbMsg.Message = notice;
        }

        private void LoadOrders()
        {
            int[] orderIds = null;
            string fromOrderPlacedOn = null;
            string toOrderPlacedOn = null;
            int shippingCountryId = 0;
            string shippingName = null;
            OrderSortingType orderBy = OrderSortingType.IdAsc;
            
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
            if (HasState(SHIPPING_NAME_FILTER)) shippingName = GetStringState(SHIPPING_NAME_FILTER);
            if (HasState(SHIPPING_COUNTRY_ID_FILTER)) shippingCountryId = GetIntState(SHIPPING_COUNTRY_ID_FILTER);
            if (HasState("OrderBy")) orderBy = (OrderSortingType)GetIntState("OrderBy");

            var result = OrderService.GetPagedOrderOverviewModel(
                pageIndex: gvOrders.CustomPageIndex,
                pageSize: gvOrders.PageSize,
                orderIds: orderIds,
                fromOrderPlacedOn: fromOrderPlacedOn,
                toOrderPlacedOn: toOrderPlacedOn,
                shippingOptionName: shippingName,
                shippingCountryId: shippingCountryId,
                readyForPacking: true,
                orderBy: orderBy);

            if (result != null)
            {
                gvOrders.DataSource = result.Items;
                gvOrders.RecordCount = result.TotalCount;
                gvOrders.CustomPageCount = result.TotalPages;
            }

            gvOrders.DataBind();

            if (gvOrders.Rows.Count <= 0) enbMsg.Message = "No records found.";
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvOrders.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvOrders.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvOrders.CustomPageIndex = gotoIndex;

            LoadOrders();
        }

        protected void lbGo_Click(object sender, EventArgs e)
        {
            Response.Redirect("/fulfillment/order_packing_info.aspx?orderid=" + txtOrderId.Text.Trim());
        }

        protected void gvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedOrder();

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
                case "LastActivityDate":
                    orderBy = OrderSortingType.LastActivityDateDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.LastActivityDateAsc;
                    break;
                case "Id":
                default:
                    orderBy = OrderSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderSortingType.IdAsc;
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

                DropDownList ddlDelivery = (DropDownList)gvOrders.HeaderRow.FindControl("ddlDelivery");
                if (GetStringState(SHIPPING_NAME_FILTER) != string.Empty) ddlDelivery.Items.FindByValue(GetStringState(SHIPPING_NAME_FILTER)).Selected = true;

                DropDownList ddlCountries = (DropDownList)gvOrders.HeaderRow.FindControl("ddlCountries");
                if (GetStringState(SHIPPING_COUNTRY_ID_FILTER) != string.Empty) ddlCountries.Items.FindByValue(GetStringState(SHIPPING_COUNTRY_ID_FILTER)).Selected = true;
            }

            var orders = gvOrders.DataSource as OrderOverviewModel[];

            for (int i = 0; i < gvOrders.Rows.Count; i++)
            {
                CheckBox cb = gvOrders.Rows[i].FindControl("cbChosen") as CheckBox;

                if (ChosenOrders.Contains(orders[i].Id))
                    cb.Checked = true;

                SetChosenOrders(orders[i].Id, cb.Checked);
            }
        }

        private void SaveLastViewedOrder()
        {
            int orderId;

            for (int i = 0; i < gvOrders.DataKeys.Count; i++)
            {
                CheckBox cb = gvOrders.Rows[i].FindControl("cbChosen") as CheckBox;
                orderId = Convert.ToInt32(gvOrders.DataKeys[i].Value);

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

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(ORDER_ID_FILTER);
            DisposeState(FROM_ORDER_PLACED_DATE_FILTER);
            DisposeState(TO_ORDER_PLACED_DATE_FILTER);
            DisposeState(SHIPPING_NAME_FILTER);
            DisposeState(SHIPPING_COUNTRY_ID_FILTER);

            LoadOrders();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ID_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(FROM_ORDER_PLACED_DATE_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_ORDER_PLACED_DATE_FILTER, ((TextBox)gvOrders.HeaderRow.FindControl("txtToDate")).Text.Trim());

            DropDownList ddlDelivery = (DropDownList)gvOrders.HeaderRow.FindControl("ddlDelivery");
            SetState(SHIPPING_NAME_FILTER, ddlDelivery.SelectedIndex != 0 ? ddlDelivery.SelectedValue.ToString() : string.Empty);

            DropDownList ddlCountries = (DropDownList)gvOrders.HeaderRow.FindControl("ddlCountries");
            SetState(SHIPPING_COUNTRY_ID_FILTER, ddlCountries.SelectedIndex != 0 ? ddlCountries.SelectedValue.ToString() : string.Empty);

            LoadOrders();
        }

        protected void lbPrint_Click(object sender, EventArgs e)
        {
            SaveLastViewedOrder();

            if (ChosenOrders.Count > 0)
            {
                rptOrders.DataSource = ChosenOrders;
                rptOrders.DataBind();

                mvScreen.ActiveViewIndex = 1;
            }
            else
            {
                enbInfo.Message = "Sorry, there were no selection.";
                LoadOrders();
            }
        }

        protected void lbExportToCsv_Click(object sender, EventArgs e)
        {
            SaveLastViewedOrder();

            // The following table is format for csv while being generated.
            //Field Names	                                Order	Format	
            //Recipient/ Tracked Returns	                1		
            //Recipient/ Tracked Returns Address line 1	    2		
            //Recipient/ Tracked Returns Post Town	        3		
            //Reference	                                    4		            
            //Weight (kgs)	                                5	    1.200 KG
            //Service Format                                6       Parcel
            //Recipient/ Tracked Returns Postcode           7
            //Service Code                                  8       CRL24

            try
            {
                string fileName = DateTime.Now.ToString("ddMMyyyyhhmmss");
                string filePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
                bool fileStatus = File.Exists(filePath);

                if (fileStatus == false)
                {
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    fs.Close();
                    //using (StreamWriter fileHeader = File.AppendText(filePath))
                    //{
                    //    fileHeader.WriteLine("Service Reference,Service,Recipient/ Tracked Returns,Recipient/ Tracked Returns Address line 1,Recipient/ Tracked Returns Post Town,Recipient/ Tracked Returns Country Code,Reference,Items,Weight (g)");
                    //}

                    GenerateCsvData(ChosenOrders, filePath);
                }
                else
                {
                    GenerateCsvData(ChosenOrders, filePath);
                }

                HttpResponse response = HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".csv");
                response.TransmitFile(filePath);
                response.Flush();

                if (File.Exists(filePath)) File.Delete(filePath);

                response.End();
            }
            catch (Exception ex)
            {
                enbInfo.Message = ex.Message;
            }

            //if (ChosenOrders.Count > 0)
            //{
            //    rptOrders.DataSource = OrderProxy.GetOrders(ChosenOrders);
            //    rptOrders.DataBind();

            //    mvScreen.ActiveViewIndex = 1;
            //}
            //else
            //{
            //    enbInfo.Message = "Sorry, there were no selection.";
            //    LoadOrders(true);
            //}
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Literal ltlOrderId = e.Item.FindControl("ltlOrderId") as Literal;
            Repeater rptItems = e.Item.FindControl("rptItems") as Repeater;

            try
            {
                if (ltlOrderId != null)
                {
                    rptItems.DataSource = OrderService.GetLineItemOverviewModelListByOrderId(Convert.ToInt32(ltlOrderId.Text));
                    rptItems.DataBind();
                }
            }
            finally
            {
                ltlOrderId = null;
                rptItems = null;
            }
        }
        
        protected void ddlDelivery_Init(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl = AdminStoreUtility.GenerateDeliveryList(ddl);
        }

        protected void ddlCountries_Init(object sender, EventArgs e)
        {
            var list = ShippingService.GetActiveCountries().ToList();
            list.Insert(0, new Country { Name = AppConstant.DEFAULT_SELECT });

            DropDownList ddl = (DropDownList)sender;
            ddl.DataValueField = "Id";
            ddl.DataTextField = "Name";
            ddl.DataSource = list;
            ddl.DataBind();
        }

        protected string GetShippingOptionNameByOrderId(int orderId)
        {
            var name = OrderService.GetShippingOptionNameByOrderId(orderId);

            if (!string.IsNullOrEmpty(name)) return name;
            return string.Empty;
        }

        private void GenerateCsvData(IList<int> orderIds, string filePath)
        {
            const string DEFAULT_SERVICE_FORMAT = "Parcel";
            const string DEFAULT_SERVICE_CODE = "CRL24";
            
            using (StreamWriter sw = File.AppendText(filePath))
            {
                foreach (var id in orderIds)
                {
                    var order = OrderService.GetCompleteOrderById(id);

                    string name = order.ShipTo;
                    string addressLine1 = order.ShippingAddressLine1 + " " + order.ShippingAddressLine2;
                    string city = order.ShippingCity;
                    string postcode = order.ShippingPostCode;
                    string reference = id.ToString();
                    decimal weightInKG = order.TotalItemWeight / 1000M;

                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                        name,
                        addressLine1,
                        city,                        
                        reference,
                        weightInKG,
                        DEFAULT_SERVICE_FORMAT,
                        postcode,
                        DEFAULT_SERVICE_CODE));
                }

                sw.Close();
            }
        }

        #region ICallbackEventHandler Members

        private string _alert = string.Empty;

        string ICallbackEventHandler.GetCallbackResult()
        {
            return _alert;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            int count = 0;
            if (Session["Count"] == null)
            {
                count = OrderService.GetOrderCountForSpecialDelivery();
                Session["Count"] = count;
            }
            else
                count = Convert.ToInt32(Session["Count"]);

            if (count > 0)
                _alert = "Alert! Next day delivery order received.";
            else
                _alert = string.Empty;
        }

        #endregion
    }
}