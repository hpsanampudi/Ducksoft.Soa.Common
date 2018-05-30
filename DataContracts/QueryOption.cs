using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Data class which is used to store WCF data service query option related information.
    /// </summary>
    [DataContract(Name = "QueryOption",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class QueryOption
    {
        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        /// <value>
        /// The option.
        /// </value>
        [DataMember]
        public string Option { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return (string.Join("=", new string[] { Option, Query }));
        }
    }
}
