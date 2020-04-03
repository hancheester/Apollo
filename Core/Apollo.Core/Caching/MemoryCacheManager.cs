using Apollo.Core.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Apollo.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager
    {
        private readonly CachePerformanceDataManager _performanceDataManager;        
        
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
        
        public event EventHandler CacheRemoved;

        public MemoryCacheManager(CachePerformanceDataManager performanceDataManager)
        {
            if (performanceDataManager == null) throw new ArgumentNullException("performanceDataManager");
            this._performanceDataManager = performanceDataManager;
        }

        public virtual T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            _performanceDataManager.IncrementGetsPerSecond();
            return (T)Cache[key];
        }
        
        public virtual void Set(string key, object data)
        {
            Set(key, data, ObjectCache.InfiniteAbsoluteExpiration);
        }

        public virtual void Set(string key, object data, DateTimeOffset? absoluteExpiration = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (data == null) return;
            if (IsSet(key)) Remove(key);

            CacheItemPolicy policy = new CacheItemPolicy();

            if (absoluteExpiration.HasValue)
                policy.AbsoluteExpiration = absoluteExpiration.Value;

            Cache.Add(new CacheItem(key, data), policy);

            _performanceDataManager.IncrementAddsPerSecond();
        }

        public virtual bool IsSet(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return (Cache.Contains(key));
        }
        
        public virtual void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            _performanceDataManager.IncrementRemovesPerSecond();
            Cache.Remove(key);
        }
        
        public virtual void RemoveByPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentNullException("pattern");

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<string>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }
        
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
        
        public void RemoveCacheStartsWith(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            List<string> keys = Cache.Where(c => c.Key.StartsWith(key)).Select(c => c.Key).ToList();
            keys.ForEach((k) => Cache.Remove(k));
        }

        public bool TryGetCache(string key, out object value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            _performanceDataManager.IncrementGetsPerSecond();

            value = null;

            lock (Cache)
            {
                if (Cache.Contains(key))
                {
                    value = Cache[key];
                    return true;
                }

                return false;
            }
        }
        
        public IDictionary<string, string> GetPerformanceData()
        {
            var data = new Dictionary<string, string>();
            data.Add("Number of cached objects", Cache.Count().ToString());
            data.Add("Total requests per second", _performanceDataManager.TotalRequestsPerSecond.ToString());
            data.Add("Adds per second", _performanceDataManager.AddsPerSecond.ToString());
            data.Add("Gets per second", _performanceDataManager.GetsPerSecond.ToString());
            data.Add("Removes per second", _performanceDataManager.RemovesPerSecond.ToString());
            
            return data;
        }

        public IList<string> GetCacheKeys()
        {
            return Cache.Select(x => x.Key).ToList();
        }
    }
}
