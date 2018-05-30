using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Data class which is used to store Progress task related information.
    /// </summary>
    [DataContract(Name = "ProgressData",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class ProgressData
    {
        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        [DataMember]
        public int Percentage { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }
    }
}
