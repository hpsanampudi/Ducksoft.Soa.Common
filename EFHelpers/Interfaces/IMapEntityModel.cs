using Ducksoft.Soa.Common.DataContracts;
using System.Collections.Generic;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to map entity to DTO (or) vice versa while performing CRUD operations
    /// </summary>
    public interface IMapEntityModel
    {
        /// <summary>
        /// Creates the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="objectToCreate">The object to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int Create<TDTO, TEntity>(TDTO objectToCreate,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="TDTO">The type of the dto.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        PaginationData<TDTO> GetAll<TDTO, TEntity>(IList<QueryOption> queryOptions = null,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <typeparam name="TDTO">The type of the dto.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        TDTO GetSingleOrDefault<TDTO, TEntity>(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Updates the specified object to update.
        /// </summary>
        /// <typeparam name="TDTO">The type of the dto.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="objectToUpdate">The object to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int Update<TDTO, TEntity>(TDTO objectToUpdate, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Deletes the specified object to delete.
        /// </summary>
        /// <typeparam name="TDTO">The type of the dto.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="objectToDelete">The object to delete.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int Delete<TDTO, TEntity>(TDTO objectToDelete, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Deletes the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        int Delete<TEntity>(string odataFilterExpression, int userId,
            CancellationToken cancelToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Purges the specified object to purge.
        /// </summary>
        /// <typeparam name="TDTO">The type of the dto.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="objectToPurge">The object to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool Purge<TDTO, TEntity>(TDTO objectToPurge, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TDTO : class;

        /// <summary>
        /// Purge the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        bool Purge<TEntity>(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        IDbRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}