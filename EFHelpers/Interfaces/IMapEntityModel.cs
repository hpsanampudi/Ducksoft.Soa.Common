﻿using Ducksoft.SOA.Common.DataContracts;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Threading;

namespace Ducksoft.SOA.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to map entity to DTO (or) vice versa while performing CRUD operations
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <typeparam name="TAudit">The type of the audit.</typeparam>
    public interface IMapEntityModel<TEntities, TAudit>
        where TEntities : DataServiceContext
        where TAudit : struct
    {
        /// <summary>
        /// Creates the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToCreate">The object to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TPKey Create<TDTO, TEntity, TPKey>(TDTO objectToCreate,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Gets the page data.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TDTO> GetPageData<TDTO, TEntity>(IList<QueryOption> queryOptions = null,
            IDataServiceQuery<TEntity> query = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        IEnumerable<TDTO> GetAll<TDTO, TEntity>(IDataServiceQuery<TEntity> query = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Gets the single or default record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TDTO GetSingleOrDefault<TDTO, TEntity>(string odataFilterExpression,
            IDataServiceQuery<TEntity> query = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToUpdate">The object to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TPKey Update<TDTO, TEntity, TPKey>(TDTO objectToUpdate, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToDelete">The object to delete.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TPKey Delete<TDTO, TEntity, TPKey>(TDTO objectToDelete, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Deletes the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TPKey Delete<TEntity, TPKey>(string odataFilterExpression, TAudit userId,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;

        /// <summary>
        /// Purges the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToPurge">The object to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool Purge<TDTO, TEntity, TPKey>(TDTO objectToPurge, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class;

        /// <summary>
        /// Purge the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool Purge<TEntity, TPKey>(string odataFilterExpression,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class;
    }
}