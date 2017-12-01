using System.Collections.Generic;
using System.Linq;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Generic class which used to compare two dictionary objects.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{System.Collections.Generic.Dictionary{TKey, TValue}}" />
    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is ignore sequence.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore sequence; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreSequence { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryComparer" /> class.
        /// </summary>
        /// <param name="isIgnoreSequence">if set to <c>true</c> [is ignore sequence].</param>
        public DictionaryComparer(bool isIgnoreSequence = false)
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
        public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
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
            var keyComparer = new ListComparer<TKey>(IsIgnoreSequence);
            if (!keyComparer.Equals(x.Keys.ToList(), y.Keys.ToList()))
            {
                return false;
            }

            //Hp --> Logic: Prepare target values based on user provided ignore sequence.
            var valueComparer = new ListComparer<TValue>();
            var targetValues = IsIgnoreSequence ? x.Select(item => y[item.Key]) : y.Values;
            var isEqual = valueComparer.Equals(x.Values.ToList(), targetValues.ToList());
            return (isEqual);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(Dictionary<TKey, TValue> obj)
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
