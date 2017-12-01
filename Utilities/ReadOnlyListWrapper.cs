using System.Collections;
using System.Collections.Generic;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Generic class which used to hold readonly list object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
    public sealed class ReadOnlyListWrapper<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// The source
        /// </summary>
        public readonly IList<T> Source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyListWrapper{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ReadOnlyListWrapper(IList<T> source)
        {
            Source = source;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return (Source.Count);
            }
        }

        /// <summary>
        /// Gets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="T"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return (Source[index]);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (Source.GetEnumerator());
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }
    }
}
