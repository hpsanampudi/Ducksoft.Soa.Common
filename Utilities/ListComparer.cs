using System.Collections.Generic;
using System.Linq;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Generic class which used to compare two list objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{System.Collections.Generic.List{T}}" />
    public class ListComparer<T> : IEqualityComparer<List<T>>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is ignore sequence.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore sequence; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreSequence { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListComparer{T}" /> class.
        /// </summary>
        /// <param name="isIgnoreSequence">if set to <c>true</c> [is ignore sequence].</param>
        public ListComparer(bool isIgnoreSequence = false)
        {
            IsIgnoreSequence = isIgnoreSequence;
        }

        #region Interface: IEqualityComparer implementation
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(List<T> x, List<T> y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            //Check whether the properties are equal.
            var isEqual = false;
            var comparer = Utility.GetEqualityComparer<T>();
            if (IsIgnoreSequence)
            {
                isEqual = !((x.Except(y, comparer).Any()) || (y.Except(x, comparer).Any()));
            }
            else
            {
                isEqual = x.SequenceEqual(y, comparer);
            }

            return (isEqual);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(List<T> obj)
        {
            //Check whether the object is null
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return (GetHashCode());
        }
        #endregion
    }
}
