using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    ///  Class for comparing object's properties.    
    /// </summary>
    public class PropertyComparer<T> : Comparer<T>
    {
        private readonly IComparer _comparer;
        private readonly ListSortDirection _direction;
        private readonly PropertyDescriptor _prop;
        private readonly bool _useToString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyComparer{T}"/> class.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="direction">The direction.</param>
        /// <exception cref="System.MissingMemberException"></exception>
        public PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (!prop.ComponentType.IsAssignableFrom(typeof(T)))
            {
                throw new MissingMemberException(typeof(T).Name, prop.Name);
            }

            var errMessage =
                "Cannot use PropertyComparer unless it can be compared by IComparable or ToString";

            Debug.Assert(CanSort(prop.PropertyType), errMessage);
            _prop = prop;
            _direction = direction;

            if (CanSortWithIComparable(prop.PropertyType))
            {
                var property = typeof(Comparer<>).MakeGenericType(new[] { prop.PropertyType })
                    .GetProperty(nameof(Comparer.Default));

                _comparer = (IComparer)property.GetValue(null, null);
                _useToString = false;
            }
            else
            {
                Debug.Assert(CanSortWithToString(prop.PropertyType), errMessage);
                _comparer = StringComparer.CurrentCultureIgnoreCase;
                _useToString = true;
            }
        }

        /// <summary>
        ///     Compares two instances of items in the list.
        /// </summary>
        /// <param name="left"> The left item to compare. </param>
        /// <param name="right"> The right item to compare. </param>
        /// <returns> </returns>
        public override int Compare(T left, T right)
        {
            var leftValue = _prop.GetValue(left);
            var rightValue = _prop.GetValue(right);

            if (_useToString)
            {
                leftValue = leftValue?.ToString();
                rightValue = rightValue?.ToString();
            }

            return (_direction == ListSortDirection.Ascending ?
                _comparer.Compare(leftValue, rightValue) :
                _comparer.Compare(rightValue, leftValue));
        }

        /// <summary>
        ///     Determines whether this instance can sort for the specified type.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        ///     <c>true</c> if this instance can sort for the specified type; otherwise, <c>false</c> .
        /// </returns>
        public static bool CanSort(Type type)
        {
            return (CanSortWithToString(type) || CanSortWithIComparable(type));
        }

        /// <summary>
        ///     Determines whether this instance can sort for the specified type using IComparable.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        ///     <c>true</c> if this instance can sort for the specified type; otherwise, <c>false</c> .
        /// </returns>
        private static bool CanSortWithIComparable(Type type)
        {
            return ((type.GetInterface(nameof(IComparable)) != null) || (type.IsNullableType()));
        }

        /// <summary>
        ///     Determines whether this instance can sort for the specified type using ToString.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        ///     <c>true</c> if this instance can sort for the specified type; otherwise, <c>false</c> .
        /// </returns>
        private static bool CanSortWithToString(Type type)
        {
            return (type.Equals(typeof(XNode)) || type.IsSubclassOf(typeof(XNode)));
        }
    }
}
