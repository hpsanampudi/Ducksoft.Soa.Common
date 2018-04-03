using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which is used to convert given generic list data into pagination set.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [DataContract(Name = "PaginationDataOfType{0}",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class PaginationData<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        /// <value>
        /// The total items.
        /// </value>
        [DataMember]
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the page data.
        /// </summary>
        /// <value>
        /// The page data.
        /// </value>
        [DataMember]
        public List<TEntity> PageData { get; set; }
    }
}
