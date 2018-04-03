using Ducksoft.Soa.Common.Utilities;
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

    /// <summary>
    /// Class which is used to store type of SOA layer from where log message is initally sent.
    /// </summary>
    [DataContract(Name = "AppLayerTypes",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public enum AppLayerTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        [EnumMember(Value = "None")]
        None = -1,
        /// <summary>
        /// The presentation layer
        /// </summary>
        [EnumDescription("UI")]
        [EnumMember(Value = "UI")]
        UI,
        /// <summary>
        /// The adaptor
        /// </summary>
        [EnumDescription("Adaptors")]
        [EnumMember(Value = "Adaptors")]
        Adaptor,
        /// <summary>
        /// The business layer
        /// </summary>
        [EnumDescription("BL")]
        [EnumMember(Value = "BL")]
        BL,
        /// <summary>
        /// The data access layer
        /// </summary>
        [EnumDescription("DAL")]
        [EnumMember(Value = "DAL")]
        DAL,
        /// <summary>
        /// The common utilities layer
        /// </summary>
        [EnumDescription("Common")]
        [EnumMember(Value = "Common")]
        Common
    }
}
