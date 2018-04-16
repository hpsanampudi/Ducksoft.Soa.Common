using Ducksoft.Soa.Common.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to load Entities with user provided connection string information.
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    public interface IEntityLoader<TEntities> where TEntities : DataServiceContext
    {
        /// <summary>
        /// Gets the data service client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        TEntities DataSvcClient { get; }

        /// <summary>
        /// Gets the data SVC URL.
        /// </summary>
        /// <value>
        /// The data SVC URL.
        /// </value>
        Uri DataSvcUrl { get; }

        /// <summary>
        /// Gets the connection information.
        /// </summary>
        /// <value>
        /// The connection information.
        /// </value>
        DbConnectionInfo ConnectionInfo { get; }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAllRecords<TEntity>(IDataServiceQuery<TEntity> query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Gets all records while calling a store procedure returning complex data type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll<TEntity>(string query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TEntity GetSingleOrDefault<TEntity>(IDataServiceQuery<TEntity> query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Executes the OData query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        IEnumerable<TEntity> ExecuteODataQuery<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, IList<QueryOption> queryOptions = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Loads the query options.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> LoadQueryOptions<TEntity>(IDataServiceQuery<TEntity> baseQuery,
            IList<QueryOption> queryOptions = null, bool isAddOrAppendDeleteFilter = true)
            where TEntity : class;

        /// <summary>
        /// Loads the filter query option.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> LoadFilterQueryOption<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, string odataFilterExpression = "",
            bool isAddOrAppendDeleteFilter = true)
            where TEntity : class;

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetPaginationData<TEntity>(IDataServiceQuery<TEntity> baseQuery,
            int? pageIndex = default(int?), int? pageSize = default(int?),
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetPaginationData<TEntity>(IDataServiceQuery<TEntity> baseQuery,
            IList<QueryOption> queryOptions = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Gets the total records count.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        long GetTotalRecordsCount<TEntity>(IDataServiceQuery<TEntity> baseQuery,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Adds the record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}"></exception>
        TPKey AddRecord<TEntity, TPKey>(TEntity record, Func<TEntity, TPKey> primaryKey = null,
            TPKey defaultValue = default(TPKey),
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct;

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <exception cref="FaultException{CustomFault}"></exception>
        TPKey UpdateRecord<TEntity, TPKey>(TEntity record, Func<TEntity, TPKey> primaryKey = null,
            TPKey defaultValue = default(TPKey), bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct;

        /// <summary>
        /// Purges (or) deletes the record permanently.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <exception cref="FaultException{CustomFault}"></exception>
        bool PurgeRecord<TEntity, TPKey>(TEntity record, Func<TEntity, TPKey> primaryKey = null,
            bool isTracked = false, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct;

        /// <summary>
        /// Purges (or) deletes the record permanently.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}"></exception>
        bool PurgeRecord<TEntity, TPKey>(string odataFilterExpression,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct;

        /// <summary>
        /// Creates the base query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> CreateBaseQuery<TEntity>(string entitySetName = "")
            where TEntity : class;

        /// <summary>
        /// Wraps the data service query to support mocking.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> WrapQuery<TEntity>(IQueryable<TEntity> query)
            where TEntity : class;
    }
}
