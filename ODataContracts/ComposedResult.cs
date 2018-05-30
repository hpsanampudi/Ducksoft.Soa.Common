using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.ODataContracts
{
    /// <summary>
    /// Data class which stores the WCF data service composed JSON result related information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "ComposedResultOfType{0}",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class ComposedResult<T> where T : ContentBase
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        [DataMember(Name = "results")]
        public IEnumerable<T> Entities { get; set; }

        /// <summary>
        /// Gets or sets the next link URI.
        /// </summary>
        /// <value>
        /// The next link URI.
        /// </value>
        [DataMember(Name = "__next")]
        public string NextLinkUri { get; set; }
    }
}
