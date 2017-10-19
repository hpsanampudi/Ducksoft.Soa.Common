using System.Collections.Generic;

namespace Ducksoft.Soa.Common.ObjectComparer
{
    /// <summary>
    /// Interface which is used to store object comparer model data.
    /// </summary>
    /// <typeparam name="TObjectType">The type of the object type.</typeparam>
    /// <typeparam name="TCompareObjType">The type of the compare object type.</typeparam>
    public interface IObjectComparerModel<TObjectType, TCompareObjType>
    {
        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        /// <value>
        /// The name of the primary key.
        /// </value>
        string PrimaryKeyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the compare property.
        /// </summary>
        /// <value>
        /// The name of the compare property.
        /// </value>
        string ComparePropName { get; set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        IEnumerable<TObjectType> Source { get; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        IEnumerable<TObjectType> Target { get; }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        IList<TCompareObjType> Results { get; }

        /// <summary>
        /// Gets the display type.
        /// </summary>
        /// <value>
        /// The display type.
        /// </value>
        ObjectComparDisplayTypes DisplayType { get; }

        /// <summary>
        /// Executes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        IEnumerable<TCompareObjType> Execute(
            IEnumerable<TObjectType> source, IEnumerable<TObjectType> target);
    }
}
