using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public string BulkProductFileLocalPath { get; set; }
        public string SitemapFileLocalPath { get; set; }
    }
}
