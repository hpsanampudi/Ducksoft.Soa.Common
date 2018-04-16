using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.Infrastructure;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Models
{
    /// <summary>
    /// Abstract class which is used to query CRUD operations of given entity set data.
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.EFHelpers.Interfaces.IRepository{TEntities, TEntity}" />
    public abstract class RepositoryBase<TEntities, TEntity> : IRepository<TEntities, TEntity>
        where TEntities : DataServiceContext
        where TEntity : class
    {
        /// <summary>
        /// The loader
        /// </summary>
        protected readonly IEntityLoader<TEntities> Loader;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILoggingService Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntities, TEntity}" /> class.
        /// </summary>
        /// <param name="loader">The loader.</param>
        /// <param name="logger">The logger.</param>
        public RepositoryBase(IEntityLoader<TEntities> loader = null, ILoggingService logger = null)
        {
            Loader = loader ?? NInjectHelper.Instance.GetInstance<IEntityLoader<TEntities>>();
            Logger = logger ?? LoggingServiceHelper.AddOrGetLoggingService;
        }

        #region Interface: IRepository<TEntities, TEntity> implementation
        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="recordToCreate">The record to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual TResult CreateRecord<TResult>(TEntity recordToCreate,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return (Loader.AddRecord<TEntity, TResult>(recordToCreate, cancelToken: cancelToken));
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual PaginationData<TEntity> GetPaginationData(IList<QueryOption> queryOptions,
            IDataServiceQuery<TEntity> query = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var baseQuery = query ?? Loader.CreateBaseQuery<TEntity>();
            return (Loader.GetPaginationData(baseQuery, queryOptions, isAddOrAppendDeleteFilter,
                cancelToken));
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAllRecords(IDataServiceQuery<TEntity> query = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var baseQuery = query ?? Loader.CreateBaseQuery<TEntity>();
            return (Loader.ExecuteODataQuery(baseQuery,
                isAddOrAppendDeleteFilter: isAddOrAppendDeleteFilter, cancelToken: cancelToken));
        }

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual TEntity GetSingleOrDefault(string odataFilterExpression,
            IDataServiceQuery<TEntity> query = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var baseQuery = query ?? Loader.CreateBaseQuery<TEntity>();
            var filterQuery = Loader.LoadFilterQueryOption(baseQuery, odataFilterExpression,
                isAddOrAppendDeleteFilter);

            return (Loader.GetSingleOrDefault(filterQuery, cancelToken));
        }

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="recordToUpdate">The record to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual TResult UpdateRecord<TResult>(TEntity recordToUpdate, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return (Loader.UpdateRecord<TEntity, TResult>(recordToUpdate, isTracked: isTracked,
                isAddOrAppendDeleteFilter: isAddOrAppendDeleteFilter, cancelToken: cancelToken));
        }

        /// <summary>
        /// Purges the record.
        /// </summary>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="recordToPurge">The record to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual bool PurgeRecord<TPKey>(TEntity recordToPurge, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return (Loader.PurgeRecord<TEntity, TPKey>(recordToPurge, isTracked: isTracked,
                isAddOrAppendDeleteFilter: isAddOrAppendDeleteFilter, cancelToken: cancelToken));
        }

        /// <summary>
        /// Purges the record.
        /// </summary>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual bool PurgeRecord<TPKey>(string odataFilterExpression,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return (Loader.PurgeRecord<TEntity, TPKey>(odataFilterExpression,
                isAddOrAppendDeleteFilter, cancelToken));
        }
        #endregion
    }
}
