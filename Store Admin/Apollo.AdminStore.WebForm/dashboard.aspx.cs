using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm
{
    public partial class dashboard : BasePage, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        public IReportService ReportService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IncompleteOrders();
                RegisteredCustomers(isPageIndexChanging:true);
            }
        }

        protected string GetFormattedPrice(decimal value)
        {
            return AdminStoreUtility.GetFormattedPrice(value, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity);
        }

        private void IncompleteOrders()
        {
            rpIncompleteOrders.DataSource = ReportService.GetIncompleteOrders();
            rpIncompleteOrders.DataBind();
        }

        private void RegisteredCustomers(bool isPageIndexChanging)
        {
            rpRegistrationActivity.DataSource = ReportService.GetRegisteredCustomersReport();
            rpRegistrationActivity.DataBind();
        }
        
        #region ICallbackEventHandler Members

        string message;

        public string GetCallbackResult()
        {
            return message;
        }
        private DateTime ParseDate(string strDate)
        {
            DateTime chosenDate = new DateTime();
            DateTime.TryParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chosenDate);

            return chosenDate;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            string type = eventArgument;
            string param = string.Empty;

            if (type.Split('|').Length > 1)
            {
                param = type.Split('|')[1];
                type = type.Split('|')[0];
            }
            
            switch (type)
            {
                case "charge_failed":
                    int failedChargeCount = OrderService.GetOrderCountByOrderStatusAndIssueCode(
                        OrderStatusCode.ON_HOLD,
                        OrderIssueCode.FAILED_TO_CHARGE);

                    message = failedChargeCount.ToString();
                    break;
                case "otc_order_system_check":
                    int otcOrderSystemCheckCount = OrderService.GetOrderCountByOrderStatusAndIssueCode(
                        OrderStatusCode.ON_HOLD,
                        OrderIssueCode.OTC_ORDER_AND_SYSTEM_CHECK_FAILED);

                    message = otcOrderSystemCheckCount.ToString();
                    break;
                case "otc_order":
                    int otcOrderCount = OrderService.GetOrderCountByOrderStatusAndIssueCode(
                        OrderStatusCode.ON_HOLD,
                        OrderIssueCode.OTC_ORDER);

                    message = otcOrderCount.ToString();
                    break;
                case "system_check":
                    int systemCheckCount = OrderService.GetOrderCountByOrderStatusAndIssueCode(
                        OrderStatusCode.ON_HOLD,
                        OrderIssueCode.SYSTEM_CHECK_FAILED);

                    message = systemCheckCount.ToString();
                    break;
                case "order_placed":
                    int orderPlacedCount = OrderService.GetOrderCountByOrderStatus(OrderStatusCode.ORDER_PLACED);
                    message = orderPlacedCount.ToString();
                    break;

                case "cancel":
                    int cancelCount = OrderService.GetRefundCountByTypeAndStatus(true, true);
                    message = cancelCount.ToString();
                    break;

                case "refund":
                    int refundCount = OrderService.GetRefundCountByTypeAndStatus(false, true);
                    message = refundCount.ToString();
                    break;

                case "line_items":
                    int lineItemcount = 0;
                    string inLineStatusXml = AdminStoreUtility.BuildXmlString("status", new string[] { LineStatusCode.ORDERED, LineStatusCode.PENDING });
                    string inOrderStatusXml = AdminStoreUtility.BuildXmlString("status", new string[] { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING });

                    DataTable pendingOrderTable = OrderService.GetOrderCountGroupByDate(inLineStatusXml, inOrderStatusXml);

                    for (int i = 0; i < pendingOrderTable.Rows.Count; i++)
                        lineItemcount += Convert.ToInt32(pendingOrderTable.Rows[i][1]);

                    message = lineItemcount.ToString();
                    break;

                case "picking":
                    int pickingItemCount = 0;
                    
                    message = pickingItemCount.ToString();
                    break;

                case "packing":
                    int packingCount = OrderService.GetOrderCountForPacking();
                    message = packingCount.ToString();
                    break;

                case "despatch":
                    int despatchCount = OrderService.GetOrderCountByOrderStatus(OrderStatusCode.SHIPPING);
                    message = despatchCount.ToString();
                    break;

                case "complete":
                    int completeCount = OrderService.GetOrderCountByOrderStatus(OrderStatusCode.INVOICED);
                    message = completeCount.ToString();
                    break;

                case "awaiting_reply":
                    int awaitingReplyCount = OrderService.GetOrderCountByOrderStatus(OrderStatusCode.AWAITING_REPLY);
                    message = awaitingReplyCount.ToString();
                    break;

                case "cancel_value_chart":
                    DateTime chosenCancelDate = ParseDate(param);
                    string inOrderStatusXmlForCancelValueChart = AdminStoreUtility.BuildXmlString("status", new string[] { OrderStatusCode.CANCELLED, OrderStatusCode.SCHEDULED_FOR_CANCEL });

                    int count1 = ReportService.GetOrderCountSumByDate(inOrderStatusXmlForCancelValueChart,
                                                                   false, 1, 0, 1, 0, 1, 0,
                                                                   0, chosenCancelDate.Day, 0, chosenCancelDate.Month, 0, chosenCancelDate.Year);

                    message = count1.ToString();

                    break;

                case "cancel_other_value_chart":
                    DateTime chosenCancelOtherDate = ParseDate(param);
                    string inOrderStatusXmlForCancelOtherValueChart = AdminStoreUtility.BuildXmlString("status",
                                                                        new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                   OrderStatusCode.PARTIAL_SHIPPING,
                                                                                   OrderStatusCode.SHIPPING,
                                                                                   OrderStatusCode.STOCK_WARNING,
                                                                                   OrderStatusCode.INVOICED,
                                                                                   OrderStatusCode.ON_HOLD });

                    int count2 = ReportService.GetOrderCountSumByDate(inOrderStatusXmlForCancelOtherValueChart,
                                                                   false, 1, 0, 1, 0, 1, 0,
                                                                   0, chosenCancelOtherDate.Day, 0, chosenCancelOtherDate.Month, 0, chosenCancelOtherDate.Year);

                    message = count2.ToString();

                    break;

                case "refund_value_chart":
                    int refundValueChartCount = OrderService.GetRefundCountByTypeAndStatus(false, true, param, param);
                    message = refundValueChartCount.ToString();
                    break;

                case "refund_other_value_chart":
                    DateTime chosenRefundOtherDate = ParseDate(param);
                    string inOrderStatusXmlForRefundOtherValueChart = AdminStoreUtility.BuildXmlString("status",
                                                                        new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                   OrderStatusCode.PARTIAL_SHIPPING,
                                                                                   OrderStatusCode.SHIPPING,
                                                                                   OrderStatusCode.STOCK_WARNING,
                                                                                   OrderStatusCode.INVOICED,
                                                                                   OrderStatusCode.ON_HOLD });

                    int count3 = ReportService.GetOrderCountSumByDate(inOrderStatusXmlForRefundOtherValueChart,
                                                                   false, 1, 0, 1, 0, 1, 0,
                                                                   0, chosenRefundOtherDate.Day, 0, chosenRefundOtherDate.Month, 0, chosenRefundOtherDate.Year);

                    message = count3.ToString();

                    break;

                case "as_value_chart":
                    var asList = OrderService.GetLineItemListByStatusCodeAndDate(DateTime.Now.AddYears(-3), DateTime.MaxValue,
                                                                               new string[] { LineStatusCode.AWAITING_STOCK },
                                                                               new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                              OrderStatusCode.PARTIAL_SHIPPING });

                    message = asList.Count.ToString();
                    break;

                case "sw_value_chart":
                    var swList = OrderService.GetLineItemListByStatusCodeAndDate(DateTime.Now.AddYears(-3), DateTime.MaxValue,
                                                                               new string[] { LineStatusCode.GOODS_ALLOCATED },
                                                                               new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                              OrderStatusCode.PARTIAL_SHIPPING });

                    message = swList.Count.ToString();
                    break;

                case "ga_value_chart":
                    var gaList = OrderService.GetLineItemListByStatusCodeAndDate(DateTime.Now.AddYears(-3), DateTime.MaxValue,
                                                                               new string[] { LineStatusCode.GOODS_ALLOCATED },
                                                                               new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                              OrderStatusCode.PARTIAL_SHIPPING });

                    message = gaList.Count.ToString();
                    break;

                case "pp_value_chart":
                    var ppList = OrderService.GetLineItemListByStatusCodeAndDate(DateTime.Now.AddYears(-3), DateTime.MaxValue,
                                                                               new string[] { LineStatusCode.PICK_IN_PROGRESS },
                                                                               new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                              OrderStatusCode.PARTIAL_SHIPPING });
                    message = ppList.Count.ToString();
                    break;

                case "o_value_chart":
                    var oList = OrderService.GetLineItemListByStatusCodeAndDate(DateTime.Now.AddYears(-3), DateTime.MaxValue,
                                                                               new string[] { LineStatusCode.ORDERED },
                                                                               new string[] { OrderStatusCode.ORDER_PLACED,
                                                                                              OrderStatusCode.PARTIAL_SHIPPING });

                    message = oList.Count.ToString();
                    break;

                case "shipment_value_chart":
                    DateTime chosenDate = new DateTime();
                    DateTime.TryParseExact(param, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out chosenDate);

                    StringBuilder sb = new StringBuilder();

                    // Loop each day
                    for (int i = -3; i < 4; i++)
                    {
                        DateTime loopDate = chosenDate.AddDays(i);
                        string dateInString = loopDate.ToString("dd/MM/yyyy");

                        sb.Append(dateInString).Append(",");

                        // Get rate for same day
                        sb.Append(OrderService.GetShipmentRate(dateInString, 0, 3)).Append(",");

                        // Get rate for > 3 days
                        sb.Append(OrderService.GetShipmentRate(dateInString, 3, 7)).Append(",");

                        // Get rate for > 1 week
                        sb.Append(OrderService.GetShipmentRate(dateInString, 7, 14)).Append(",");

                        // Get rate for > 2 weeks
                        sb.Append(OrderService.GetShipmentRate(dateInString, 14, 999)).Append(",");
                    }

                    sb = sb.Remove(sb.Length - 1, 1);

                    message = sb.ToString();
                    break;
            }
        }

        #endregion
    }
}