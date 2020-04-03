namespace Apollo.Core.Model
{
    public struct OrderIssueCode
    {
        public const string SYSTEM_CHECK_FAILED = "IC3";
        public const string FAILED_TO_CHARGE = "IC13";
        public const string OTC_ORDER = "IC39";
        public const string OTC_ORDER_AND_SYSTEM_CHECK_FAILED = "IC42";
    }
        
    public struct OrderStatusCode
    {
        public const string ORDER_PLACED = "OP";
        public const string PARTIAL_SHIPPING = "PS";
        public const string SHIPPING = "S";
        public const string SHIPPING_NO_EARNED_POINT = "S_NO_POINT";
        public const string STOCK_WARNING = "SW";
        public const string SCHEDULED_FOR_CANCEL = "SC";
        public const string CANCELLED = "C";
        public const string INVOICED = "I";
        public const string PENDING = "P";
        public const string DISCARDED = "D";
        public const string ON_HOLD = "OH";
        public const string AWAITING_COMPLETION = "AC";
        public const string AWAITING_REPLY = "AR";
    }
    
    public struct LineStatusCode
    {
        public const string PENDING = "P";
        public const string ORDERED = "O";
        public const string GOODS_ALLOCATED = "GA";
        public const string PICK_IN_PROGRESS = "PP";
        public const string STOCK_WARNING = "SW";
        public const string CANCELLED = "C";
        public const string DESPATCHED = "D";
        public const string PARTIAL_SHIPPING = "PS";
        public const string AWAITING_STOCK = "AS";
        public const string SCHEDULED_FOR_CANCEL = "SC";
        public const string ON_HOLD = "OH";
        //public const string IN_TRANSIT = "IT";
    }

    public struct PaymentType
    {
        public const string GOOGLE_CHECKOUT = "Google Checkout";
        public const string SAGEPAY = "SagePay";
        public const string WORLDPAY = "WorldPay";
        public const string PAID_BY_PHONE = "Paid by phone";
    }
}