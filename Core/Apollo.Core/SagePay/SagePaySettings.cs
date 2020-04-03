using Apollo.Core.Configuration;

namespace Apollo.Core.SagePay
{
    public class SagePaySettings : ISettings
    {
        public string SagePay3DSecureCallbackLink { get; set; }
        public string SagePayPaymentGatewayLink { get; set; }
        public string SagePayRegisterRefundLink { get; set; }
        public string SagePayRegisterAbortLink { get; set; }
        public string SagePayRegisterReleaseLink { get; set; }
        public string SagePayRegisterRepeatLink { get; set; }
        public string SagePayReportingAdminAPILink { get; set; }
        public string SagePayVPSProtocol { get; set; }
        public string SagePayVendor { get; set; }
        public string SagePayWebUser { get; set; }
        public string SagePayWebPwd { get; set; }
    }
}
