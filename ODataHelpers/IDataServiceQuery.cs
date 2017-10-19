using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ducksoft.Soa.Common.ODataHelpers
{
    /// <summary>
    /// Interface for data service query.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <seealso cref="System.Linq.IQueryable{TElement}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{TElement}" />
    /// <seealso cref="System.Linq.IQueryable" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public interface IDataServiceQuery<TElement> : IQueryable<TElement>, IEnumerable<TElement>, IQueryable, IEnumerable
    {
        /// <summary>
        /// Expands the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A new query that includes the requested $expand query option appended to the URI of the supplied query.</returns>
        IDataServiceQuery<TElement> Expand(string path);

        /// <summary>
        /// Includes the total count.
        /// </summary>
        /// <returns>A new DataServiceQuery`1 object that has the inline count option set.</returns>
        IDataServiceQuery<TElement> IncludeTotalCount();

        /// <summary>
        /// Adds the query option.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new query that includes the requested query option appended to the URI of the supplied query</returns>
        IDataServiceQuery<TElement> AddQueryOption(string name, object value);
    }
}
