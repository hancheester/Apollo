using Apollo.Dache.Client;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Apollo.Core.Caching
{
    public class DacheCacheManager : ICacheManager
    {
        // The lock used to ensure state
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private CacheClient _cache;

        public DacheCacheManager()
        {
            _cache = new CacheClient();
            _cache.HostDisconnected += _cache_HostDisconnected;
        }
        
        public void Clear()
        {
            _cache.Clear();
        }

        public virtual T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public virtual bool IsSet(string key)
        {
            return _cache.IsSet(key);
        }

        public virtual void Remove(string key)
        {
            _cache.Remove(key);
        }

        public virtual void RemoveByPattern(string pattern)
        {
            _cache.RemoveByPattern(pattern);
        }

        public virtual void RemoveCacheStartsWith(string key)
        {
            _cache.RemoveCacheStartsWith(key);
        }

        public virtual void Set(string key, object data)
        {
            Set(key, data, absoluteExpiration: null);
        }

        public virtual void Set(string key, object data, DateTimeOffset? absoluteExpiration = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (data == null) return;
            if (IsSet(key)) Remove(key);

            _cache.AddOrUpdate(key, data, absoluteExpiration: absoluteExpiration);
        }

        public virtual bool TryGetCache(string key, out object value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            value = null;

            lock (_cache)
            {
                return _cache.TryGet<object>(key, out value);
            }
        }

        public IDictionary<string, string> GetPerformanceData()
        {
            return _cache.GetPerformanceData();
        }

        public IList<string> GetCacheKeys()
        {
            return _cache.GetCacheKeys();
        }

        private void _cache_HostDisconnected(object sender, EventArgs e)
        {
            _lock.ExitWriteLock();

            try
            {
                _cache = new CacheClient();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }        
    }
}
