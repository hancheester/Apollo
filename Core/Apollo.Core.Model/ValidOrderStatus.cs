namespace Apollo.Core.Model
{
    public class ValidOrderStatus
    {
        public static string[] VALID_STATUSES = new string[] { OrderStatusCode.ORDER_PLACED,
                                                               OrderStatusCode.PARTIAL_SHIPPING,
                                                               OrderStatusCode.SHIPPING,
                                                               OrderStatusCode.STOCK_WARNING,
                                                               OrderStatusCode.INVOICED,
                                                               OrderStatusCode.ON_HOLD };

        public static string[] VALID_CUSTOMER_STATUSES = new string[] { OrderStatusCode.CANCELLED,
                                                                        OrderStatusCode.INVOICED,
                                                                        OrderStatusCode.ON_HOLD,
                                                                        OrderStatusCode.ORDER_PLACED,
                                                                        OrderStatusCode.PARTIAL_SHIPPING,
                                                                        OrderStatusCode.SCHEDULED_FOR_CANCEL,
                                                                        OrderStatusCode.SHIPPING,
                                                                        OrderStatusCode.STOCK_WARNING,
                                                                        OrderStatusCode.AWAITING_REPLY };
    }
}
