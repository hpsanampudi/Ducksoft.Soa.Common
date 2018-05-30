using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.SOA.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface for data service query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="System.Linq.IQueryable{TEntity}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{TEntity}" />
    /// <seealso cref="System.Linq.IQueryable" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public interface IDataServiceQuery<TEntity> : IQueryable<TEntity>, IEnumerable<TEntity>, IQueryable, IEnumerable
    {
        /// <summary>
        /// Expands the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A new query that includes the requested $expand query option appended to the URI of the supplied query.</returns>
        IDataServiceQuery<TEntity> Expand(string path);

        /// <summary>
        /// Expands the specified navigation property accessor.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="navigationPropertyAccessor">The navigation property accessor.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> Expand<TTarget>(Expression<Func<TEntity, TTarget>> navigationPropertyAccessor);

        /// <summary>
        /// Includes the total count.
        /// </summary>
        /// <returns>A new DataServiceQuery`1 object that has the inline count option set.</returns>
        IDataServiceQuery<TEntity> IncludeTotalCount();

        /// <summary>
        /// Adds the query option.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new query that includes the requested query option appended to the URI of the supplied query</returns>
        IDataServiceQuery<TEntity> AddQueryOption(string name, object value);

        /// <summary>
        /// Gets the request URI.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        Uri RequestUri { get; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> Execute();

        /// <summary>
        /// Filters the specified query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Skips the specified number of elements in sequence.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> Skip(int count);

        /// <summary>
        /// Takes the specified number of elements.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> Top(int count);

        /// <summary>
        /// Orders the given query key by ascending order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> predicate);

        /// <summary>
        /// Orders the given query key by descending order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IDataServiceQuery<TEntity> OrderByDescending<TKey>(
            Expression<Func<TEntity, TKey>> predicate);

    }
}
