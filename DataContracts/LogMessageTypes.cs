using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which is used to store type of log message related information.
    /// </summary>
    [DataContract(Name = "LogMessageTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum LogMessageTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumMember(Value = "None")]
        None = -1,
        /// <summary>
        /// The debug
        /// </summary>
        [EnumMember(Value = "Debug")]
        Debug,
        /// <summary>
        /// The error
        /// </summary>
        [EnumMember(Value = "Error")]
        Error,
        /// <summary>
        /// The fatal
        /// </summary>
        [EnumMember(Value = "Fatal")]
        Fatal,
        /// <summary>
        /// The warning
        /// </summary>
        [EnumMember(Value = "Warning")]
        Warning,
        /// <summary>
        /// The information
        /// </summary>
        [EnumMember(Value = "Information")]
        Information,
    }
}
