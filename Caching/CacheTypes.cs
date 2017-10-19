using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.Caching
{
    /// <summary>
    /// Stores the type of cache expirations types
    /// </summary>
    public enum CacheExpirationTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// Cache will expire the entry after a given time interval.
        /// </summary>
        [EnumDescription("Absolute")]
        Absolute,
        /// <summary>
        /// Cache will expire the entry if it hasn't been accessed in given time interval.
        /// i.e., If the cache object is used in within given interval then same interval will extend.
        /// </summary>
        [EnumDescription("Sliding")]
        Sliding,
        /// <summary>
        /// The hybrid combination of both absolute and sliding.
        /// </summary>
        [EnumDescription("Absolute And Sliding (Hybrid)")]
        AbsoluteAndSliding
    }
}
