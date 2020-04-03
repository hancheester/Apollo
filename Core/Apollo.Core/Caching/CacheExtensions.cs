using Apollo.Core.Model;
using System;

namespace Apollo.Core.Caching
{
    public static class CacheExtensions
    {
        private static readonly object syncObject = new object();

        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> load)
        {
            lock (syncObject)
            {
                if (cacheManager.IsSet(key))
                    return cacheManager.Get<T>(key);

                var result = load();

                //Do not add if result is null.                
                if (result != null) cacheManager.Set(key, result);

                return result;
            }
        }

        public static T GetWithExpiry<T>(this ICacheManager cacheManager, string key, Func<T> load)
            where T : ICacheExpirySupported, new()
        {
            lock (syncObject)
            {
                if (cacheManager.IsSet(key))
                    return cacheManager.Get<T>(key);

                var result = load();

                //Do not add if result is null.                
                if (result != null)
                {
                    DateTimeOffset? obsoluteExpiration = null;

                    if (result.CacheExpiryDate.HasValue)
                        obsoluteExpiration = new DateTimeOffset(result.CacheExpiryDate.Value);

                    cacheManager.Set(key, result, obsoluteExpiration);
                }
                return result;
            }
        }

        public static T GetWithExpiry<T>(this ICacheManager cacheManager, string key, Func<T> load, bool expiredEndOfDay = false)
        {
            lock (syncObject)
            {
                if (cacheManager.IsSet(key))
                    return cacheManager.Get<T>(key);

                var result = load();

                //Do not add if result is null.                
                if (result != null)
                {
                    var obsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(24D));

                    if (expiredEndOfDay)
                    {
                        var expiryDate = DateTime.Now.AddDays(1);
                        obsoluteExpiration = new DateTime(expiryDate.Year, expiryDate.Month, expiryDate.Day, 0, 0, 0);
                    }

                    cacheManager.Set(key, result, obsoluteExpiration);
                }
                return result;
            }
        }

        public static T GetWithExpiry<T>(this ICacheManager cacheManager, string key, Func<T> load, DateTimeOffset? obsoluteExpiration = null)
        {
            lock (syncObject)
            {
                if (cacheManager.IsSet(key))
                    return cacheManager.Get<T>(key);

                var result = load();

                //Do not add if result is null.                
                if (result != null)
                {
                    cacheManager.Set(key, result, obsoluteExpiration);
                }
                return result;
            }
        }
    }
}
