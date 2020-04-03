using Apollo.Core.Configuration;

namespace Apollo.Core.Domain
{
    public class StoreInformationSettings : ISettings
    {
        public string StoreFrontLink { get; set; }
        public string StoreFrontSecuredLink { get; set; }
        public string CompanyName { get; set; }
        public string TermURL { get; set; }
        public bool DisplayEuCookieLawWarning { get; set; }
    }
}
