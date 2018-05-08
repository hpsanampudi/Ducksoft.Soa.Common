namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Enum which stores the information related to user selected three state boolean value.
    /// </summary>
    public enum ThreeStateOptionTypes
    {
        /// <summary>
        /// The none means equivalent to boolean value null
        /// </summary>
        [EnumDescription(null)]
        None = -1,
        /// <summary>
        /// The no means equivalent to boolean value false
        /// </summary>
        [EnumDescription("false")]
        No,
        /// <summary>
        /// The yes means equivalent to boolean value true
        /// </summary>
        [EnumDescription("true")]
        Yes
    }
}
