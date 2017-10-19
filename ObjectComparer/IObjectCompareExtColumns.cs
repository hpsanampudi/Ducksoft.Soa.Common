namespace Ducksoft.Soa.Common.ObjectComparer
{
    /// <summary>
    /// Interface which is used to provide comparer object extension columns.
    /// </summary>
    /// <typeparam name="TComparePropType">The type of the compare property type.</typeparam>
    public interface IObjectCompareExtColumns<TComparePropType>
    {
        /// <summary>
        /// Gets the target value.
        /// </summary>
        /// <value>
        /// The target value.
        /// </value>
        TComparePropType TargetValue { get; }

        /// <summary>
        /// Gets the type of the status.
        /// </summary>
        /// <value>
        /// The type of the status.
        /// </value>
        ObjectCompareStatusTypes StatusType { get; }

        /// <summary>
        /// Gets the display type.
        /// </summary>
        /// <value>
        /// The display type.
        /// </value>
        ObjectComparDisplayTypes DisplayType { get; }
    }
}
