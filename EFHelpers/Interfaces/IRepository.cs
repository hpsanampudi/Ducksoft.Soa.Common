using Ducksoft.Soa.Common.DataContracts;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to query CRUD operations of entity set data.
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntities, TEntity>
        where TEntities : DataServiceContext
        where TEntity : class
    {
        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="recordToCreate">The record to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TResult CreateRecord<TResult>(TEntity recordToCreate,
            CancellationToken cancelToken = default(CancellationToken))
            where TResult : struct;

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetPaginationData(IList<QueryOption> queryOptions,
            IDataServiceQuery<TEntity> query = null,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAllRecords(IDataServiceQuery<TEntity> query = null,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TEntity GetSingleOrDefault(string odataFilterExpression,
            IDataServiceQuery<TEntity> query = null,
            CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="recordToUpdate">The record to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TResult UpdateRecord<TResult>(TEntity recordToUpdate, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TResult : struct;

        /// <summary>
        /// Purges the record.
        /// </summary>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="recordToPurge">The record to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool PurgeRecord<TPKey>(TEntity recordToPurge, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
             where TPKey : struct;

        /// <summary>
        /// Purge the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool PurgeRecord<TPKey>(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken))
            where TPKey : struct;
    }
}
