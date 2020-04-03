using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Security
{
    public class SecuritySettings : ISettings
    {
        public string SecretKey { get; set; }
        public string IVKey { get; set; }
        public bool DisableSSL { get; set; }
    }
}
