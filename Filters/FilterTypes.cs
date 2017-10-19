using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.Filters
{
    /// <summary>
    /// Enum which holds linq filter logical operator types.
    /// </summary>
    public enum FilterLogicalOperatorTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// The and
        /// </summary>
        [EnumDescription("And")]
        And,
        /// <summary>
        /// The or
        /// </summary>
        [EnumDescription("Or")]
        Or
    }

    /// <summary>
    /// Enum which holds linq filter expression comparer operator types.
    /// </summary>
    public enum FilterCompareOperatorTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// The equal to
        /// </summary>
        [EnumDescription("Equal To")]
        EqualTo,
        /// <summary>
        /// The not equal to
        /// </summary>
        [EnumDescription("Not Equal To")]
        NotEqualTo,
        /// <summary>
        /// The less than
        /// </summary>
        [EnumDescription("Less Than")]
        LessThan,
        /// <summary>
        /// The less than or equal to
        /// </summary>
        [EnumDescription("Less Than Or Equal To")]
        LessThanOrEqualTo,
        /// <summary>
        /// The greater than
        /// </summary>
        [EnumDescription("Greater Than")]
        GreaterThan,
        /// <summary>
        /// The greater than or equal to
        /// </summary>
        [EnumDescription("Greater Than Or Equal To")]
        GreaterThanOrEqualTo,
        /// <summary>
        /// The starts with
        /// </summary>
        [EnumDescription("Starts With")]
        StartsWith,
        /// <summary>
        /// The ends with
        /// </summary>
        [EnumDescription("Ends With")]
        EndsWith,
        /// <summary>
        /// The contains
        /// </summary>
        [EnumDescription("Contains")]
        Contains,
        /// <summary>
        /// The does not contain
        /// </summary>
        [EnumDescription("Does Not Contain")]
        DoesNotContain,
        /// <summary>
        /// The is null
        /// </summary>
        [EnumDescription("Is Null")]
        IsNull,
        /// <summary>
        /// The is not null
        /// </summary>
        [EnumDescription("Is Not Null")]
        IsNotNull,
        /// <summary>
        /// The is empty
        /// </summary>
        [EnumDescription("Is Empty")]
        IsEmpty,
        /// <summary>
        /// The is not empty
        /// </summary>
        [EnumDescription("Is Not Empty")]
        IsNotEmpty
    }

    /// <summary>
    /// Enum which holds binding list changed notification types.
    /// </summary>
    public enum ListChangedNotifyTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
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
        /// The updated
        /// </summary>
        [EnumDescription("Updated")]
        Updated,
    }
}