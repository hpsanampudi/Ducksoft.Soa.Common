using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.ODataContracts
{
    /// <summary>
    /// Data class which stores the WCF data service JSON metadata related information.
    /// </summary>
    [DataContract(Name = "__metadata",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class MetaData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
