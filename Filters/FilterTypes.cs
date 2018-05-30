using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    /// Enum which holds linq filter logical operator types.
    /// </summary>
    [DataContract(Name = "FilterLogicalOperatorTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum FilterLogicalOperatorTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumMember]
        None = -1,
        /// <summary>
        /// The and
        /// </summary>
        [EnumMember]
        And,
        /// <summary>
        /// The or
        /// </summary>
        [EnumMember]
        Or,
    }

    /// <summary>
    /// Enum which holds linq filter expression comparer operator types.
    /// </summary>
    [DataContract(Name = "FilterCompareOperatorTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum FilterCompareOperatorTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumMember]
        None = -1,
        /// <summary>
        /// The equal to
        /// </summary>
        [EnumMember]
        EqualTo,
        /// <summary>
        /// The not equal to
        /// </summary>
        [EnumMember]
        NotEqualTo,
        /// <summary>
        /// The less than
        /// </summary>
        [EnumMember]
        LessThan,
        /// <summary>
        /// The less than or equal to
        /// </summary>
        [EnumMember]
        LessThanOrEqualTo,
        /// <summary>
        /// The greater than
        /// </summary>
        [EnumMember]
        GreaterThan,
        /// <summary>
        /// The greater than or equal to
        /// </summary>
        [EnumMember]
        GreaterThanOrEqualTo,
        /// <summary>
        /// The starts with
        /// </summary>
        [EnumMember]
        StartsWith,
        /// <summary>
        /// The ends with
        /// </summary>
        [EnumMember]
        EndsWith,
        /// <summary>
        /// The contains
        /// </summary>
        [EnumMember]
        Contains,
        /// <summary>
        /// The does not contain
        /// </summary>
        [EnumMember]
        NotContains,
        /// <summary>
        /// The is null
        /// </summary>
        [EnumMember]
        IsNull,
        /// <summary>
        /// The is not null
        /// </summary>
        [EnumMember]
        IsNotNull,
        /// <summary>
        /// The is empty
        /// </summary>
        [EnumMember]
        IsEmpty,
        /// <summary>
        /// The is not empty
        /// </summary>
        [EnumMember]
        IsNotEmpty
    }

    /// <summary>
    /// Enum which holds binding list changed notification types.
    /// </summary>
    [DataContract(Name = "ListChangedNotifyTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum ListChangedNotifyTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumMember]
        None = -1,
        /// <summary>
        /// The added
        /// </summary>
        [EnumMember]
        Added,
        /// <summary>
        /// The deleted
        /// </summary>
        [EnumMember]
        Deleted,
        /// <summary>
        /// The updated
        /// </summary>
        [EnumMember]
        Updated,
    }
}