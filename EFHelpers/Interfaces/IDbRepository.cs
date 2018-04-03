using Ducksoft.Soa.Common.DataContracts;
using System.Collections.Generic;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to query CRUD operations of entity set data.
    /// </summary>
    public interface IDbRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordToCreate">The record to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int CreateRecord(TEntity recordToCreate,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetAllRecords(IList<QueryOption> queryOptions,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TEntity GetSingleOrDefault(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <param name="recordToUpdate">The record to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int UpdateRecord(TEntity recordToUpdate, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Purges the record.
        /// </summary>
        /// <param name="recordToPurge">The record to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool PurgeRecord(TEntity recordToPurge, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Purge the database record by given OData filter expression.
        /// </summary>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool PurgeRecord(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken));
    }
}
