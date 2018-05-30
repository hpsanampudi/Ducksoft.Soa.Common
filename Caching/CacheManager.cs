using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace Ducksoft.SOA.Common.Caching
{
    /// <summary>
    /// Singleton class, which is used to manage reading and writing of data to cache.
    /// </summary>
    public sealed class CacheManager
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is 
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<CacheManager> instance =
            new Lazy<CacheManager>(() => new CacheManager());

        /// <summary>
        /// Gets the instance of the singleton object: CacheManager.
        /// </summary>
        public static CacheManager Instance
        {
            get { return (instance.Value); }
        }

        /// <summary>
        /// Gets the cache keys.
        /// </summary>
        /// <value>
        /// The cache keys.
        /// </value>
        public IList<string> CacheKeys { get; private set; }

        /// <summary>
        /// The cache manager object
        /// </summary>
        private readonly ObjectCache cacheMgrObject;

        /// <summary>
        /// Prevents a default instance of the <see cref="CacheManager"/> class from being created.
        /// </summary>
        private CacheManager()
        {
            cacheMgrObject = MemoryCache.Default;
            CacheKeys = new List<string>();
        }

        /// <summary>
        /// Adds or get existing cache object by expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The cache object expression.</param>
        /// <returns></returns>
        public T AddOrGetExisting<T>(Expression<Func<T>> expression) where T : class
        {
            var methodName = Utility.GetMemberName(expression);
            var srcType = Utility.GetDeclaringType(expression.Body);
            var itemInfo = Utility.GetMethodAttribute<CacheItemInfoAttribute>(srcType, methodName);
            var cacheKey = ((itemInfo.Key.Contains('<')) && (itemInfo.Key.Contains('>'))) ?
                GetDynamicCacheKey(expression.Body, itemInfo.Key) : itemInfo.Key.Trim();

            if ((!cacheMgrObject.Contains(cacheKey)) && (!CacheKeys.Contains(cacheKey)))
            {
                CacheKeys.Add(cacheKey);
            }

            var newValue = new Lazy<T>(expression.Compile());
            var oldValue = cacheMgrObject.AddOrGetExisting(cacheKey, newValue,
                GetCachePolicy(itemInfo)) as Lazy<T>;

            return ((oldValue ?? newValue).Value);
        }

        /// <summary>
        /// Gets the dynamic cache key.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="cacheItemKey">The cache item key.</param>
        /// <returns></returns>
        private static string GetDynamicCacheKey(Expression expression, string cacheItemKey)
        {
            var methodArgs = Utility.GetMethodArguments(expression);
            const string delimiter = "_";

            return (string.Join(delimiter, cacheItemKey.Split(
                new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(subKey => subKey.Trim())
                .Select(subKey =>
                {
                    var dynArg = string.Empty;
                    if (subKey.Contains('<') && subKey.Contains('>'))
                    {
                        var argName = subKey.TrimStart('<').TrimEnd('>').Trim();
                        dynArg = methodArgs.Where(arg => (argName == arg.Name))
                            .Select(arg => string.Join(":", new string[]
                                {
                                    arg.Name,
                                    arg.Value.ToString()
                                }))
                            .SingleOrDefault();

                    }
                    return (string.IsNullOrEmpty(dynArg) ? subKey : dynArg);
                })));
        }

        /// <summary>
        /// Deletes the specified cache object by expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The cache object expression.</param>
        public void Delete<T>(Expression<Func<T>> expression) where T : class
        {
            var methodName = Utility.GetMemberName(expression);
            var srcType = Utility.GetDeclaringType(expression.Body);
            var itemInfo = Utility.GetMethodAttribute<CacheItemInfoAttribute>(srcType, methodName);
            var cacheKey = ((itemInfo.Key.Contains('<')) && (itemInfo.Key.Contains('>'))) ?
                GetDynamicCacheKey(expression.Body, itemInfo.Key) : itemInfo.Key.Trim();

            Delete(cacheKey);
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        public void DeleteAll()
        {
            CacheKeys.ToList().ForEach(Delete);
        }

        /// <summary>
        /// Deletes the specified cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        public void Delete(string cacheKey)
        {
            if ((cacheMgrObject.Contains(cacheKey)) && (CacheKeys.Contains(cacheKey)))
            {
                cacheMgrObject.Remove(cacheKey);
                CacheKeys.Remove(cacheKey);

                //Hp --> Logic: Free unreferenced memory
                GC.Collect();
            }
        }

        /// <summary>
        /// Gets the cache policy.
        /// </summary>
        /// <param name="cacheItemInfo">The interval.</param>
        /// <returns></returns>
        private static CacheItemPolicy GetCachePolicy(CacheItemInfoAttribute cacheItemInfo)
        {
            var policy = new CacheItemPolicy();
            switch (cacheItemInfo.ExpirationType)
            {
                case CacheExpirationTypes.Absolute:
                    {
                        policy.AbsoluteExpiration =
                            DateTimeOffset.Now.AddMinutes(cacheItemInfo.Interval);
                    }
                    break;

                case CacheExpirationTypes.Sliding:
                    {
                        policy.SlidingExpiration = new TimeSpan(0, cacheItemInfo.Interval, 0);
                    }
                    break;

                default:
                    {
                        ErrorBase.Require(false, string.Format(
                            "The given cache expiration type {0} is not handled programatically!",
                            Utility.GetEnumDescription(cacheItemInfo.ExpirationType)));
                    }
                    break;
            }

            return (policy);
        }
    }
}
