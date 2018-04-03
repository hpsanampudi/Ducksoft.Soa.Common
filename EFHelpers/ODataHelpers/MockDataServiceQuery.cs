using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.Soa.Common.EFHelpers.ODataHelpers
{
    /// <summary>
    /// Class for mocking of data service query.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class MockDataServiceQuery<TElement> : IDataServiceQuery<TElement>
    {
        IQueryable<TElement> query;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDataServiceQuery{TElement}"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public MockDataServiceQuery(IQueryable<TElement> query)
        {
            this.query = query;
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        public Type ElementType => typeof(TElement);

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        public Expression Expression => query.Expression;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public IQueryProvider Provider => query.Provider;

        /// <summary>
        /// Adds the query option.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A new query that includes the requested query option appended to the URI of the supplied query
        /// </returns>
        public IDataServiceQuery<TElement> AddQueryOption(string name, object value) => this;

        /// <summary>
        /// Expands the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// A new query that includes the requested $expand query option appended to the URI of the supplied query.
        /// </returns>
        public IDataServiceQuery<TElement> Expand(string path) => this;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator() => query.GetEnumerator();

        /// <summary>
        /// Includes the total count.
        /// </summary>
        /// <returns>
        /// A new DataServiceQuery`1 object that has the inline count option set.
        /// </returns>
        public IDataServiceQuery<TElement> IncludeTotalCount() => this;

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();
    }
}
