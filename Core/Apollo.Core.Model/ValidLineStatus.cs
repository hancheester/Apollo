namespace Apollo.Core.Model
{
    public class ValidLineStatus
    {
        public static string[] VALID_LINE_STATUSES = new string[] { LineStatusCode.ORDERED,
                                                                    LineStatusCode.GOODS_ALLOCATED,
                                                                    LineStatusCode.PARTIAL_SHIPPING,
                                                                    LineStatusCode.PICK_IN_PROGRESS,
                                                                    LineStatusCode.STOCK_WARNING,
                                                                    LineStatusCode.AWAITING_STOCK,
                                                                    LineStatusCode.DESPATCHED };
    }
}
