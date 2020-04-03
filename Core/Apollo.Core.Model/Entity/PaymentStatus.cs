namespace Apollo.Core.Model.Entity
{
    public class PaymentStatus
    {
        private int _orderId;

        private string _issuerUrl;

        private string _paReq;

        private string _mD;

        private int _userId;

        private string _checkoutErr;

        private string _status;

        private bool _processStatus;

        private bool _has3DSecure;

        private bool _redirectedToContainer;

        private int _paymentEmailInvoiceId;

        private int _checkoutOrderId;

        public int CheckoutOrderId
        {
            get { return _checkoutOrderId; }
            set { _checkoutOrderId = value; }
        }

        public int PaymentEmailInvoiceId
        {
            get { return _paymentEmailInvoiceId; }
            set { _paymentEmailInvoiceId = value; }
        }
        
        public bool RedirectedToContainer
        {
            get { return _redirectedToContainer; }
            set { _redirectedToContainer = value; }
        }

        public bool Has3DSecure
        {
            get { return _has3DSecure; }
            set { _has3DSecure = value; }
        }

        public bool ProcessStatus
        {
            get { return _processStatus; }
            set { _processStatus = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string CheckoutErr
        {
            get { return _checkoutErr; }
            set { _checkoutErr = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        
        public string MD
        {
            get { return _mD; }
            set { _mD = value; }
        }

        public string PaReq
        {
            get { return _paReq; }
            set { _paReq = value; }
        }

        public string IssuerUrl
        {
            get { return _issuerUrl; }
            set { _issuerUrl = value; }
        }

        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
    }

    public class PaymentInputs
    {
        private string _userName;

        private Card _card;

        private string _userHostAddress;

        private string _userAgent;

        private string _currencyCode;

        private ShippingOption _shippingOption;

        private int _allocatedPoint;

        private decimal _pointValue;

        private string _promoCode;

        private decimal _discountValue;

        private decimal _exchangeRate;

        private decimal _totalAmount;

        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
        }


        public decimal ExchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; }
        }

        public decimal DiscountValue
        {
            get { return _discountValue; }
            set { _discountValue = value; }
        }

        public string PromoCode
        {
            get { return _promoCode; }
            set { _promoCode = value; }
        }
        public decimal PointValue
        {
            get { return _pointValue; }
            set { _pointValue = value; }
        }
        public int AllocatedPoint
        {
            get { return _allocatedPoint; }
            set { _allocatedPoint = value; }
        }

        public ShippingOption ShippingOption
        {
            get { return _shippingOption; }
            set { _shippingOption = value; }
        }

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }

        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        public string UserHostAddress
        {
            get { return _userHostAddress; }
            set { _userHostAddress = value; }
        }

        public Card Card
        {
            get { return _card; }
            set { _card = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }

    public class UpdatedStatus
    {
        private int _orderId;

        private string _redirectionStatus;

        private string _redirectionMessage;

        private int _userId;

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private Order _orderInfo;

        public Order OrderInfo
        {
            get { return _orderInfo; }
            set { _orderInfo = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string RedirectionMessage
        {
            get { return _redirectionMessage; }
            set { _redirectionMessage = value; }
        }

        public string RedirectionStatus
        {
            get { return _redirectionStatus; }
            set { _redirectionStatus = value; }
        }

        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
    }

    //public enum CurrencyType
    //{
    //    HtmlEntity,
    //    Symbol,
    //    Code
    //}
}
