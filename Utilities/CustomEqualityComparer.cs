using System;
using System.Collections.Generic;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Generic class which used to compare two objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public Func<T, object> Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEqualityComparer{T}" /> class.
        /// </summary>
        public CustomEqualityComparer() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public CustomEqualityComparer(Func<T, object> expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(T x, T y)
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

            var source = Expression?.Invoke(x) ?? x;
            var target = Expression?.Invoke(y) ?? y;
            return (source.Equals(target));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(T obj)
        {
            //Check whether the object is null
            if (ReferenceEquals(obj, null))
            {
                return (0);
            }

            return ((Expression?.Invoke(obj) ?? obj).GetHashCode());
        }
    }
}
