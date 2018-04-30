using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.Soa.Common.EFHelpers.ODataHelpers
{
    /// <summary>
    /// Wrapper class for data service query
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.EFHelpers.Interfaces.IDataServiceQuery{TEntity}" />
    public class DataServiceQueryWrapper<TEntity> : IDataServiceQuery<TEntity>
    {
        /// <summary>
        /// The query
        /// </summary>
        private DataServiceQuery<TEntity> query;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceQueryWrapper{TElement}" /> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="isIncludeTotalCount">if set to <c>true</c> [is include total count].</param>
        public DataServiceQueryWrapper(DataServiceQuery<TEntity> query)
        {
            this.query = query;
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        public Type ElementType => typeof(TEntity);

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        public Expression Expression => query.Expression;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public IQueryProvider Provider => query.Provider;

        /// <summary>
        /// Gets the request URI.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        public Uri RequestUri => query.RequestUri;

        /// <summary>
        /// Adds the query option.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A new query that includes the requested query option appended to the URI of the supplied query
        /// </returns>
        public IDataServiceQuery<TEntity> AddQueryOption(string name, object value) =>
            new DataServiceQueryWrapper<TEntity>(query.AddQueryOption(name, value));

        /// <summary>
        /// Expands the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// A new query that includes the requested $expand query option appended to the URI of the supplied query.
        /// </returns>
        public IDataServiceQuery<TEntity> Expand(string path) =>
            new DataServiceQueryWrapper<TEntity>(query.Expand(path));

        /// <summary>
        /// Expands the specified navigation property accessor.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="navigationPropertyAccessor">The navigation property accessor.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Expand<TTarget>(
            Expression<Func<TEntity, TTarget>> navigationPropertyAccessor) =>
            new DataServiceQueryWrapper<TEntity>(query.Expand(navigationPropertyAccessor));

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator() => query.GetEnumerator();

        /// <summary>
        /// Includes the total count.
        /// </summary>
        /// <returns>
        /// A new DataServiceQuery`1 object that has the inline count option set.
        /// </returns>
        public IDataServiceQuery<TEntity> IncludeTotalCount() =>
            new DataServiceQueryWrapper<TEntity>(query.IncludeTotalCount());

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> Execute() => query.Execute();

        /// <summary>
        /// Filters the specified query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            query = query.Where(predicate) as DataServiceQuery<TEntity>;
            return (this);
        }

        /// <summary>
        /// Skips the specified number of elements in sequence.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Skip(int count)
        {
            query = query.Skip(count) as DataServiceQuery<TEntity>;
            return (this);
        }

        /// <summary>
        /// Takes the specified number of elements.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Top(int count)
        {
            query = query.Take(count) as DataServiceQuery<TEntity>;
            return (this);
        }

        /// <summary>
        /// Orders the given query key by ascending order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> predicate)
        {
            query = query.OrderBy(predicate) as DataServiceQuery<TEntity>;
            return (this);
        }

        /// <summary>
        /// Orders the given query key either by descending order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> OrderByDescending<TKey>(
            Expression<Func<TEntity, TKey>> predicate)
        {
            query = query.OrderByDescending(predicate) as DataServiceQuery<TEntity>;
            return (this);
        }
    }
}
