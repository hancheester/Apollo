namespace Apollo.Core.Model
{
    public static partial class SettingKey
    {
        // Email Templates
        public static string AccountRegisterEmailPath { get { return "AccountRegisterEmailPath"; } }
        public static string AccountRegisterWithPasswordEmailPath { get { return "AccountRegisterWithPasswordEmailPath"; } }
        public static string DespatchConfirmationEmailPath { get { return "DespatchConfirmationEmailPath"; } }
        public static string NewUsernameEmailPath { get { return "NewUsernameEmailPath"; } }
        public static string OrderConfirmationEmailPath { get { return "OrderConfirmationEmailPath"; } }
        public static string PasswordRetrievalEmailPath { get { return "PasswordRetrievalEmailPath"; } }
        public static string PaymentInvoiceEmailPath { get { return "PaymentInvoiceEmailPath"; } }
        public static string PaymentInvoiceConfirmationEmailPath { get { return "PaymentInvoiceConfirmationEmailPath"; } }

        // SagePay URLs
        public static string SagePay3DSecureCallbackURL { get { return "SagePay3DSecureCallbackURL"; } }
        public static string SagePayPaymentGatewayURL { get { return "SagePayPaymentGatewayURL"; } }
        public static string SagePayRegisterRefundURL { get { return "SagePayRegisterRefundURL"; } }
        public static string SagePayRegisterAbortURL { get { return "SagePayRegisterAbortURL"; } }
        public static string SagePayRegisterReleaseURL { get { return "SagePayRegisterReleaseURL"; } }
        public static string SagePayRegisterRepeatURL { get { return "SagePayRegisterRepeatURL"; } }
        public static string SagePayReportingAdminAPIULR { get { return "SagePayReportingAdminAPIURL"; } }
        public static string SagePayVPSProtocol { get { return "SagePayVPSProtocol"; } }
        public static string SagePayVendor { get { return "SagePayVendor"; } }
        public static string SagePayWebUser { get { return "SagePayWebUser"; } }
        public static string SagePayWebPwd { get { return "SagePayWebPwd"; } }

        // Media 
        public static string BrandMediaPath { get { return "BrandMediaPath"; } }
        public static string ProductColourPath { get { return "ProductColourPath"; } }
        public static string ProductMediaPath { get { return "ProductMediaPath"; } }
        public static string CategoryMediaPath { get { return "CategoryMediaPath"; } }
        public static string OfferMediaPath { get { return "OfferMediaPath"; } }
        public static string MediumBannerPath { get { return "MediumBannerPath"; } }
        public static string MiniBannerPath { get { return "MiniBannerPath"; } }
        public static string LargeBannerPath { get { return "LargeBannerPath"; } }
        public static string OfferBannerPath { get { return "OfferBannerPath"; } }

        // NHunspell Resource Files
        public static string NHunspellResourceAffFilePath { get { return "NHunspellResourceAffFilePath"; } }
        public static string NHunspellResourceDictFilePath { get { return "NHunspellResourceDictFilePath"; } }

        // Affiliate Windows
        public static string AwMerchantId { get { return "AwMerchantId"; } }
        public static string AwTestMode { get { return "AwTestMode"; } }

        // Prescription
        public static string NHSPrescriptionProductId { get { return "NHSPrescriptionProductId"; } }
        public static string NHSNoticeForEmail { get { return "NHSNoticeForEmail"; } }
        public static string NHSPrescriptionPrice { get { return "NHSPrescriptionPrice"; } } 

        public static string LargeLogoURL { get { return "LargeLogoURL"; } }
        public static string CompanyName { get { return "CompanyName"; } }
        public static string StoreFrontUrl { get { return "StoreFrontUrl"; } }
        public static string StoreFrontSecuredUrl { get { return "StoreFrontSecuredURL"; } }
        public static string DefaultCountryId { get { return "DefaultCountryId"; } }
        public static string MaxPharmProduct { get { return "MaxPharmProduct"; } }
        public static string DefaultShippingCountryCode { get { return "DefaultShippingCountryCode"; } }
        public static string DefaultCurrencyCode { get { return "DefaultCurrencyCode"; } }
        public static string NoImageFile { get { return "NoImageFile"; } }
        public static string TermURL { get { return "TermURL"; } }
        public static string VAT { get { return "VAT"; } }
        public static string LoyaltyRate { get { return "LoyaltyRate"; } }
        public static string SecretKey { get { return "SecretKey"; } }
        public static string IVKey { get { return "IVKey"; } }
        public static string InvoicePrefix { get { return "InvoicePrefix"; } }
        public static string PetterBranchId { get { return "PetterBranchId"; } }
        public static string WarehouseBranchId { get { return "WarehouseBranchId"; } }
        public static string PharmaciaBranchId { get { return "PharmaciaBranchId"; } }
        public static string VectorSyncCaller { get { return "VectorSyncCaller"; } }
        public static string VectorStockSyncXmlPath { get { return "VectorStockSyncXmlPath"; } }
        public static string CustomerRole { get { return "CustomerRole"; } }
        public static string Token { get { return "Token"; } }
        public static string StoreFrontRefreshCacheURL { get { return "StoreFrontRefreshCacheURL"; } }
        public static string StoreFrontToken { get { return "StoreFrontToken"; } }
        public static string CacheEnabled { get { return "CacheEnabled"; } }
        public static string ExchangeRateProviderURL { get { return "ExchangeRateProviderURL"; } }
        public static string ExchangeRateFactor { get { return "ExchangeRateFactor"; } }
        public static string DisableSSL { get { return "DisableSSL"; } }
        public static string PhoneOrderMessage {  get { return "PhoneOrderMessage"; } }
        public static string ContactUsEmail { get { return "ContactUsEmail"; } }
    }
}
