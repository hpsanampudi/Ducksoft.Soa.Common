using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Class which is used store dynamic linq query related information.
    /// </summary>
    [DataContract(Name = "DynamicLinqFilter",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class DynamicLinqFilter
    {
        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        /// <value>
        /// The filter expression.
        /// </value>
        [DataMember]
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the filter parameters.
        /// </summary>
        /// <value>
        /// The filter parameters.
        /// </value>
        [DataMember]
        public object[] Parameters { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        [DataMember]
        public int Index { get; set; }
    }
}
