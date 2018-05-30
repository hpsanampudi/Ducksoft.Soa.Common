using Ducksoft.SOA.Common.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.SOA.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to query entity set data through given WCF data service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IQueryableRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the name of the entity set.
        /// </summary>
        /// <value>
        /// The name of the entity set.
        /// </value>
        string EntitySetName { get; }

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <value>
        /// The entity set.
        /// </value>
        DataServiceQuery<TEntity> EntitySet { get; }

        /// <summary>
        /// Gets the rows count.
        /// </summary>
        /// <value>
        /// The rows count.
        /// </value>
        int RowsCount { get; }

        /// <summary>
        /// Gets all records.
        /// Note: If server side pagination is set then it will return records based on that limit.
        /// Also, If records to retreive is larger then it may throw low memory exception.
        /// </summary>
        /// <param name="isPaginationSet">if set to <c>true</c> [is pagination set].</param>
        /// <returns></returns>        
        IEnumerable<IQueryable<TEntity>> GetAll(bool isPaginationSet);

        /// <summary>
        /// Gets all records based on given query options.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        IEnumerable<IQueryable<TEntity>> GetAll(IList<QueryOption> queryOptions);

        /// <summary>
        /// Gets all records based on given predicate expression.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IEnumerable<IQueryable<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetPaginationData(int pageNumber, int pageSize);

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        PaginationData<TEntity> GetPaginationData(string expression, int skip, int take);

        /// <summary>
        /// Searches the by.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<TEntity> SearchBy(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Searches the by.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        IQueryable<TEntity> SearchBy(string expression);

        /// <summary>
        /// Finds the first.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        TEntity FindFirst(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the pk expression.
        /// </summary>
        /// <param name="srcEntity">The source entity.</param>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> GetPKExpression(TEntity srcEntity);

        /// <summary>
        /// Adds the new entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        void AddNew(TEntity entity, bool commit = true);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        void Insert(TEntity entity, bool commit = true);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        void Update(TEntity entity, bool commit = true);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        void Delete(TEntity entity, bool commit = true);

        /// <summary>
        /// Commits this instance changes.
        /// </summary>
        void Commit();
    }
}
