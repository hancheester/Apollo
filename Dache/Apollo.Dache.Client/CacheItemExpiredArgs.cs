using System;

namespace Apollo.Dache.Client
{
    /// <summary>
    /// Socket error args.
    /// </summary>
    public class CacheItemExpiredArgs : EventArgs
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="cacheKey">The expired cache key.</param>
        internal CacheItemExpiredArgs(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        /// <summary>
        /// The cache key.
        /// </summary>
        public string CacheKey { get; private set; }
    }
}
