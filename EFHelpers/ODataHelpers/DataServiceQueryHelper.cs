using Ducksoft.SOA.Common.EFHelpers.Interfaces;
using Ducksoft.SOA.Common.Utilities;
using System.Data.Services.Client;
using System.Linq;

namespace Ducksoft.SOA.Common.EFHelpers.ODataHelpers
{
    /// <summary>
    /// Static helper class for data service query to support mocking.
    /// </summary>
    public static class DataServiceQueryHelper
    {
        /// <summary>
        /// Wraps the data service query to support mocking.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static IDataServiceQuery<TEntity> Wrap<TEntity>(this IQueryable<TEntity> query)
            where TEntity : class
        {
            ErrorBase.CheckArgIsNull(query, nameof(query));
            var wrapper = default(IDataServiceQuery<TEntity>);

            var baseQuery = query as DataServiceQuery<TEntity>;
            if (baseQuery != null)
            {
                wrapper = new DataServiceQueryWrapper<TEntity>(baseQuery);
            }
            else
            {
                wrapper = new MockDataServiceQuery<TEntity>(query);
            }

            return (wrapper);
        }
    }
}
