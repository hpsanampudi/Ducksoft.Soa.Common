using Ducksoft.SOA.Common.Utilities;
using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Enum which stores the information related to user selected three state boolean value.
    /// </summary>
    [DataContract(Name = "ThreeStateOptionTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum ThreeStateOptionTypes
    {
        /// <summary>
        /// The none means equivalent to boolean value null
        /// </summary>
        [EnumMember(Value = null)]
        [EnumDescription(null)]
        None = -1,
        /// <summary>
        /// The no means equivalent to boolean value false
        /// </summary>
        [EnumMember(Value = "false")]
        [EnumDescription("false")]
        No,
        /// <summary>
        /// The yes means equivalent to boolean value true
        /// </summary>
        [EnumMember(Value = "true")]
        [EnumDescription("true")]
        Yes
    }
}
