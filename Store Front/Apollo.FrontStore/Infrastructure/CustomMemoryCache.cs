using Apollo.Core.Caching;
using System;
using System.Runtime.Caching;

namespace Apollo.FrontStore.Infrastructure
{
    public class CustomMemoryCache : MemoryCache
    {
        private readonly ICacheManager _cacheManager;

        public CustomMemoryCache(string name, ICacheManager cacheManager)
         : base(name)
        {
            _cacheManager = cacheManager;
        }
        
        public override bool Add(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            var newKey = string.Format(CacheKey.CHILD_ACTION_CACHE_ID_KEY, key);
            _cacheManager.Set(newKey, value);

            return true;
        }

        public override object Get(string key, string regionName = null)
        {
            var newKey = string.Format(CacheKey.CHILD_ACTION_CACHE_ID_KEY, key);
            return _cacheManager.Get<object>(newKey);
        }
    }
}