using Apollo.Core.Configuration;

namespace Apollo.Core.Caching
{
    public class CacheSettings : ISettings
    {
        public string StoreFrontRefreshCacheLink { get; set; }
        public string StoreFrontGetPerfDataLink { get; set; }
        public string StoreFrontGetCacheKeysLink { get; set; }
        public string StoreFrontToken { get; set; }
    }
}
