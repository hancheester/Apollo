using System;
using System.Collections.Generic;

namespace Apollo.Core.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        void Set(string key, object data);
        void Set(string key, object data, DateTimeOffset? absoluteExpiration = null);
        bool IsSet(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);
        void Clear();        
        void RemoveCacheStartsWith(string key);
        bool TryGetCache(string key, out object value);
        IDictionary<string, string> GetPerformanceData();
        IList<string> GetCacheKeys();
    }
}
