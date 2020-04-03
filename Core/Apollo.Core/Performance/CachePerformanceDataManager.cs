using System;
using System.Threading;

namespace Apollo.Core.Performance
{
    public class CachePerformanceDataManager : IDisposable
    {
        private readonly Timer _perSecondTimer = null;

        private int _addsPerSecond = 0;
        private int _getsPerSecond = 0;
        private int _removesPerSecond = 0;
        
        /// <summary>
        /// The cache memory usage in megabytes.
        /// </summary>
        public virtual int CacheMemoryUsageMb { get; set; }

        /// <summary>
        /// The cache memory usage limit in megabytes.
        /// </summary>
        public virtual int CacheMemoryUsageLimitMb { get; set; }

        /// <summary>
        /// The cache memory usage percent.
        /// </summary>
        public virtual int CacheMemoryUsagePercent { get; set; }

        /// <summary>
        /// The adds per second.
        /// </summary>
        public int AddsPerSecond
        {
            get
            {
                return _addsPerSecond;
            }
        }

        /// <summary>
        /// Increments the adds per second.
        /// </summary>
        public virtual void IncrementAddsPerSecond()
        {
            Interlocked.Increment(ref _addsPerSecond);
        }

        /// <summary>
        /// The gets per second.
        /// </summary>
        public int GetsPerSecond
        {
            get
            {
                return _getsPerSecond;
            }
        }

        /// <summary>
        /// Increments the gets per second.
        /// </summary>
        public virtual void IncrementGetsPerSecond()
        {
            Interlocked.Increment(ref _getsPerSecond);
        }

        /// <summary>
        /// The removes per second.
        /// </summary>
        public int RemovesPerSecond
        {
            get
            {
                return _removesPerSecond;
            }
        }

        /// <summary>
        /// Increments the removes per second.
        /// </summary>
        public virtual void IncrementRemovesPerSecond()
        {
            Interlocked.Increment(ref _removesPerSecond);
        }

        /// <summary>
        /// The total requests per second.
        /// </summary>
        public int TotalRequestsPerSecond
        {
            get
            {
                return _addsPerSecond + _getsPerSecond + _removesPerSecond;
            }
        }

        public CachePerformanceDataManager()
        {
            // Configure per second timer to fire every 1000 ms starting 1000ms from now
            _perSecondTimer = new Timer(PerSecondOperations, null, 1000, 1000);
        }
        
        private void PerSecondOperations(object state)
        {
            lock (_perSecondTimer)
            {
                Interlocked.Exchange(ref _addsPerSecond, 0);
                Interlocked.Exchange(ref _getsPerSecond, 0);
                Interlocked.Exchange(ref _removesPerSecond, 0);
            }
        }

        public void Dispose()
        {
            _perSecondTimer.Dispose();
        }
    }
}
