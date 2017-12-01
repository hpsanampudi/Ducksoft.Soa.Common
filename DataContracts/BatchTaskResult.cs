using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Data class which is used to store async task result related information.
    /// </summary>
    [DataContract(Name = "BatchTaskResult",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class BatchTaskResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [DataMember]
        public AggregateException Exception { get; set; }
    }
}
