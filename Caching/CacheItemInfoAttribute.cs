using System;

namespace Ducksoft.Soa.Common.Caching
{
    /// <summary>
    /// Attribute class for storing cache item related information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheItemInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the cache interval in minutes.
        /// </summary>
        /// <value>
        /// The cache interval in minutes.
        /// </value>
        public int Interval { get; private set; }

        /// <summary>
        /// Gets the type of the expiration.
        /// </summary>
        /// <value>
        /// The type of the expiration.
        /// </value>
        public CacheExpirationTypes ExpirationType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemInfoAttribute"/> class.
        /// with default cache expiration type obsolute.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheInterval">The cache interval.</param>
        public CacheItemInfoAttribute(string cacheKey, int cacheInterval)
            : this(cacheKey, cacheInterval, CacheExpirationTypes.Absolute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemInfoAttribute" /> class.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheInterval">The cache interval in minutes.</param>
        /// <param name="cacheExpirationType">Type of the cache expiration.</param>
        public CacheItemInfoAttribute(string cacheKey, int cacheInterval,
            CacheExpirationTypes cacheExpirationType)
        {
            Key = cacheKey;
            Interval = cacheInterval;
            ExpirationType = cacheExpirationType;
        }
    }
}
