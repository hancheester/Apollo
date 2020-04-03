namespace Apollo.Core.Model
{
    public struct SagePayDirectFormName
    {
        public const string VPSPROTOCOL = "VPSProtocol";
        public const string TXTYPE = "TxType";
        public const string VENDOR = "Vendor";
        public const string VENDOR_TXCODE = "VendorTxCode";
        public const string VPSTXID = "VPSTxId";
        public const string REFERRER_ID = "ReferrerID";
        public const string AMOUNT = "Amount";
        public const string CURRENCY = "Currency";
        public const string DESCRIPTION = "Description";
        public const string CARD_HOLDER = "CardHolder";
        public const string CARD_NUMBER = "CardNumber";
        public const string START_DATE = "StartDate";
        public const string EXPIRY_DATE = "ExpiryDate";
        public const string ISSUE_NUMBER = "IssueNumber";
        public const string CV2 = "CV2";
        public const string CARD_TYPE = "CardType";
        public const string BILLING_SURNAME = "BillingSurname";
        public const string BILLING_FIRSTNAMES = "BillingFirstnames";
        public const string BILLING_ADDRESS1 = "BillingAddress1";
        public const string BILLING_ADDRESS2 = "BillingAddress2";
        public const string BILLING_CITY = "BillingCity";
        public const string BILLING_POSTCODE = "BillingPostCode";
        public const string BILLING_COUNTRY = "BillingCountry";
        public const string BILLING_STATE = "BillingState";
        public const string BILLING_PHONE = "BillingPhone";
        public const string DELIVERY_SURNAME = "DeliverySurname";
        public const string DELIVERY_FIRSTNAMES = "DeliveryFirstnames";
        public const string DELIVERY_ADDRESS1 = "DeliveryAddress1";
        public const string DELIVERY_ADDRESS2 = "DeliveryAddress2";
        public const string DELIVERY_CITY = "DeliveryCity";
        public const string DELIVERY_POSTCODE = "DeliveryPostCode";
        public const string DELIVERY_COUNTRY = "DeliveryCountry";
        public const string DELIVERY_STATE = "DeliveryState";
        public const string DELIVERY_PHONE = "DeliveryPhone";
        public const string CUSTOMER_EMAIL = "CustomerEMail";
        public const string BASKET = "Basket";
        public const string GIFT_AID = "GiftAidPayment";
        public const string APPLY_AVS_CV2 = "ApplyAVSCV2";
        public const string CLIENT_IP_ADDRESS = "ClientIPAddress";
        public const string APPLY_3DSECURE = "Apply3DSecure";
        public const string ACCOUNT_TYPE = "AccountType";
        public const string STATUS = "Status";
        public const string STATUS_DETAIL = "StatusDetail";
        public const string MD = "MD";
        public const string ACSURL = "ACSURL";
        public const string PAREQ = "PAReq";
        public const string PARES = "PARes";
        public const string AVSCV2 = "AVSCV2";
        public const string ADDRESS_RESULT = "AddressResult";
        public const string POSTCODE_RESULT = "PostCodeResult";
        public const string CV2RESULT = "CV2Result";
        public const string THREE_SECURE_STATUS = "3DSecureStatus";
        public const string CAVV = "CAVV";
        public const string RELATED_VPSTXID = "RelatedVPSTxId";
        public const string RELATED_VENDOR_TXCODE = "RelatedVendorTxCode";
        public const string RELATED_SECURITY_KEY = "RelatedSecurityKey";
        public const string RELATED_TX_AUTHNO = "RelatedTxAuthNo";
        public const string SECURITY_KEY = "SecurityKey";
        public const string TX_AUTHNO = "TxAuthNo";
        public const string RELEASE_AMOUNT = "ReleaseAmount";
    }

    public struct SagePayDirectStatus
    {
        public const string OK = "OK";
        public const string MALFORMED = "MALFORMED";
        public const string INVALID = "INVALID";
        public const string NOTAUTHED = "NOTAUTHED";
        public const string REJECTED = "REJECTED";
        public const string THREE_D_AUTH = "3DAUTH";
        public const string PPREDIRECT = "PPREDIRECT";
        public const string AUTHENTICATED = "AUTHENTICATED";
        public const string REGISTERED = "REGISTERED";
        public const string ERROR = "ERROR";
    }

    public struct SagePayAccountTxType
    {
        public const string PAYMENT = "PAYMENT";
        public const string DEFERRED = "DEFERRED";
        public const string AUTHENTICATE = "AUTHENTICATE";
        public const string RELEASE = "RELEASE";
        public const string ABORT = "ABORT";
        public const string REFUND = "REFUND";
        public const string REPEAT = "REPEAT";
        public const string VOID = "VOID";
        public const string MANUAl = "MANUAL";
        public const string DIRECTREFUND = "DIRECTREFUND";
        public const string AUTHORISE = "AUTHORISE";
        public const string CANCEL = "CANCEL";
    }

    public struct SagePayProgressStatus
    {
        public const string NAME_VALUES_GENERATED = "Name value string generated";
        public const string REQUEST_SENT = "Request sent";
        public const string RESPONSE_RECEIVED = "Response received";
        public const string ERROR_OCCURRED = "Payment error occurred";
        public const string ACKNOWLEDGMENT = "Acknowledgment";
        public const string REDIRECT_TO_3D_SECURE = "Redirect to 3D Secure";
        public const string VOID_DATABASE_RECORD_CREATED = "VOID database record created";
        public const string CANCEL_DATABASE_RECORD_CREATED = "CANCEL database record created";
        public const string RELEASE_DATABASE_RECORD_CREATED = "RELEASE database record created";
        public const string REFUND_DATABASE_RECORD_CREATED = "REFUND database record created";
        public const string AUTHORISE_DATABASE_RECORD_CREATED = "AUTHORISE database record created";
        public const string REPEAT_DATABASE_RECORD_CREATED = "REPEAT database record created";
        public const string ABORT_DATABASE_RECORD_CREATED = "ABORT database record created";
        public const string REGISTER_TRANSACTION_PAYMENT = "Register transaction PAYMENT";
        public const string REGISTER_TRANSACTION_DEFERRED = "Register transaction DEFERRED";
        public const string REGISTER_TRANSACTION_AUTHENTICATE = "Register transaction AUTHENTICATE";
        public const string REGISTER_TRANSACTION_REFUND = "Register transaction REFUND";
        public const string REGISTER_TRANSACTION_VOID = "Register transaction VOID";
        public const string REGISTER_TRANSACTION_AUTHORISE = "Register transaction AUTHORISE";
        public const string REGISTER_TRANSACTION_CANCEL = "Register transaction CANCEL";
        public const string REGISTER_TRANSACTION_RELEASE = "Register transaction RELEASE";
        public const string REGISTER_TRANSACTION_ABORT = "Register transaction ABORT";
        public const string REGISTER_TRANSACTION_REPEAT = "Register transaction REPEAT";
    }

    public struct SagePayMessage
    {
        public const string PROTOCOL_NOT_MATCHED_MESSAGE = "Cannot match the protocol version.";
        public const string TRANSACTION_NOT_FOUND_MESSAGE = "Unable to find the transaction in our database.";
        public const string SECURITY_KEY_NOT_FOUND_MESSAGE = "Unable to find security key.";
        public const string DEFAULT_ERROR_MESSAGE = "There was an error processing your credit card. Please check your name, credit card number, and card expiration date for correctness. Remember, these must match the card exactly. ";
        public const string NOTAUTHORISED_ERROR_MESSAGE = "The transaction was not authorised by the bank.";
        public const string NOTAUTHENTICATED_ERROR_MESSAGE = "The transaction was not authenticated.";
        public const string REJECTED_ERROR_MESSAGE = "The transaction was failed by your 3D-Secure or AVS/CV2 rule-bases.";
        public const string DEFAULT_TRANSACTION_ERROR_MESSAGE = "There was an error processing your transaction. Please look at related transaction logs for more information. ";
    }

    public struct SagePayAVSCV2
    {
        public const string ALL_MATCH = "ALL MATCH";
        public const string SECURITY_CODE_MATCH_ONLY = "SECURITY CODE MATCH ONLY";
        public const string ADDRESS_MATCH_ONLY = "ADDRESS MATCH ONLY";
        public const string NO_DATA_MATCHES = "NO DATA MATCHES";
        public const string DATA_NOT_CHECKED = "DATA NOT CHECKED";
    }

    public struct SagePay3DSecureStatus
    {
        public const string OK = "OK";
        public const string NOAUTH = "NOAUTH";
        public const string CANTAUTH = "CANTAUTH";
        public const string NOTAUTHED = "NOTAUTHED";
        public const string ATTEMPTONLY = "ATTEMPTONLY";
        public const string NOTCHECKED = "NOTCHECKED";
        public const string INCOMPLETE = "INCOMPLETE";
        public const string MALFORMED = "MALFORMED";
        public const string INVALID = "INVALID";
        public const string ERROR = "ERROR";
    }
}
