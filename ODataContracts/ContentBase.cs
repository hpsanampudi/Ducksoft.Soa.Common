using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.ODataContracts
{
    /// <summary>
    /// Data class which stores the WCF data service JSON content related information.
    /// </summary>
    [DataContract(Name = "ContentBase",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class ContentBase
    {
        /// <summary>
        /// Gets or sets the meta data.
        /// </summary>
        /// <value>
        /// The meta data.
        /// </value>
        [DataMember(Name = "__metadata")]
        public MetaData MetaData { get; set; }
    }
}
