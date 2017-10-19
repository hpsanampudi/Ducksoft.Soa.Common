using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.ObjectComparer
{
    /// <summary>
    /// Enum which is used to show object compare status types.
    /// </summary>
    public enum ObjectCompareStatusTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// The equal
        /// </summary>
        [EnumDescription("Equal")]
        Equal,
        /// <summary>
        /// The added
        /// </summary>
        [EnumDescription("Added")]
        Added,
        /// <summary>
        /// The deleted
        /// </summary>
        [EnumDescription("Deleted")]
        Deleted,
        /// <summary>
        /// The modified at both, Used in vertical display mode only.
        /// </summary>
        [EnumDescription("Modified At Both")]
        ModifiedAtBoth,
        /// <summary>
        /// The modified at source, Used in horizontal display mode only.
        /// </summary>
        [EnumDescription("Modified At Source")]
        ModifiedAtSource,
        /// <summary>
        /// The modified at target, Used in horizontal display mode only.
        /// </summary>
        [EnumDescription("Modified At Target")]
        ModifiedAtTarget,
    }

    /// <summary>
    /// Enum which is used to show object compare result display types.
    /// </summary>
    public enum ObjectComparDisplayTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// The vertical
        /// </summary>
        [EnumDescription("Vertical")]
        Vertical,
        /// <summary>
        /// The horizontal
        /// </summary>
        [EnumDescription("Horizontal")]
        Horizontal
    }
}
