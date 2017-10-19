using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Dynamic.Extensions
{
    /// <summary>
    /// Static class which holds Dynamic linq query extension methods.
    /// http://weblogs.asp.net/scottgu/dynamic-linq-part-1-using-the-linq-dynamic-query-library
    /// </summary>
    public static class DynamicLinqExtensions
    {
        /// <summary>
        /// GroupBy many allows you to define a list of subgroups in a single call.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="groupSelectors">The group selectors.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example: customers.GroupByMany("Country", "City")
        /// </remarks>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements, params string[] groupSelectors)
        {
            var selectors = new List<Func<TElement, object>>(groupSelectors.Length);
            foreach (var selector in groupSelectors)
            {
                var expression =
                    DynamicExpression.ParseLambda(typeof(TElement), typeof(object), selector);

                selectors.Add((Func<TElement, object>)expression.Compile());
            }

            return (elements.GroupByMany(selectors.ToArray()));
        }

        /// <summary>
        /// GroupBy many allows you to define a list of subgroups in a single call.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="groupSelectors">The group selectors.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example: customers.GroupByMany(c => c.Country, c => c.City)
        /// </remarks>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements, params Func<TElement, object>[] groupSelectors)
        {
            if (0 < groupSelectors.Length)
            {
                var selector = groupSelectors.First();
                //Hp --> Logic : Reduce the list recursively until zero
                var nextSelectors = groupSelectors.Skip<Func<TElement, object>>(1).ToArray();

                return (elements.GroupBy(selector).Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        }));
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Class which store linq groupby results
    /// </summary>
    public class GroupResult
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public object Key { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable Items { get; set; }

        /// <summary>
        /// Gets or sets the sub groups.
        /// </summary>
        /// <value>
        /// The sub groups.
        /// </value>
        public IEnumerable<GroupResult> SubGroups { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() { return string.Format("{0} ({1})", Key, Count); }
    }
}
