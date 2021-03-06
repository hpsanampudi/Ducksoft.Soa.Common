﻿using Ducksoft.SOA.Common.EFHelpers.Interfaces;
using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.SOA.Common.EFHelpers.ODataHelpers
{
    /// <summary>
    /// Class for mocking of data service query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Ducksoft.SOA.Common.EFHelpers.Interfaces.IDataServiceQuery{TEntity}" />
    public class MockDataServiceQuery<TEntity> : IDataServiceQuery<TEntity>
    {
        /// <summary>
        /// The query
        /// </summary>
        private IQueryable<TEntity> query;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDataServiceQuery{TElement}"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public MockDataServiceQuery(IQueryable<TEntity> query)
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
        public Uri RequestUri => new Uri(ElementType.Name, UriKind.RelativeOrAbsolute);

        /// <summary>
        /// Adds the query option.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A new query that includes the requested query option appended to the URI of the supplied query
        /// </returns>
        public IDataServiceQuery<TEntity> AddQueryOption(string name, object value)
        {
            switch (name.ToLowerInvariant())
            {
                case "$filter":
                    {
                        //TODO: Hp -->  Needs to implement filter parser logic.
                    }
                    break;

                case "$skip":
                    {
                        query = query.Skip((int)value);
                    }
                    break;

                case "$top":
                    {
                        query = query.Take((int)value);
                    }
                    break;

                case "$orderby":
                    {
                        var orderCriteria = value as string;
                        var expressions = orderCriteria.Split(new string[] { " " },
                            StringSplitOptions.RemoveEmptyEntries).ToList();

                        var sortDirection = ListSortDirection.Ascending;
                        if (expressions.Count > 1)
                        {
                            var sortOrder = expressions.LastOrDefault();
                            sortDirection = sortOrder.IsEqualTo("desc") ?
                                ListSortDirection.Descending : ListSortDirection.Ascending;
                        }

                        var sortColumns = expressions.FirstOrDefault().Split(new string[] { "," },
                            StringSplitOptions.RemoveEmptyEntries);

                        foreach (var column in sortColumns)
                        {
                            if (column.Contains("."))
                            {
                                query = query.SortBy(column, sortDirection);
                            }
                            else
                            {
                                query = query.SortBy(sortDirection, column);
                            }
                        }
                    }
                    break;

                default:
                    {
                        //Hp --> Do nothing
                    }
                    break;
            }

            return (this);
        }

        /// <summary>
        /// Expands the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// A new query that includes the requested $expand query option appended to the URI of the supplied query.
        /// </returns>
        public IDataServiceQuery<TEntity> Expand(string path) => this;

        /// <summary>
        /// Expands the specified navigation property accessor.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="navigationPropertyAccessor">The navigation property accessor.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Expand<TTarget>(
            Expression<Func<TEntity, TTarget>> navigationPropertyAccessor) => this;

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
        public IDataServiceQuery<TEntity> IncludeTotalCount() => this;

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> Execute() => query.AsEnumerable();

        /// <summary>
        /// Filters the specified query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            query = query.Where(predicate);
            return (this);
        }

        /// <summary>
        /// Skips the specified number of elements in sequence.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Skip(int count)
        {
            query = query.Skip(count);
            return (this);
        }

        /// <summary>
        /// Takes the specified number of elements.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> Top(int count)
        {
            query = query.Take(count);
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
            query = query.OrderBy(predicate);
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
            query = query.OrderByDescending(predicate);
            return (this);
        }
    }
}
