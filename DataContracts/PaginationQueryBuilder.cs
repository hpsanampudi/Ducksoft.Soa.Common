using Ducksoft.SOA.Common.Filters;
using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Class which is used to store pagination query related information.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [DataContract(Name = "PaginationQueryBuilder",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class PaginationQueryBuilder
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        [DataMember]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        [DataMember]
        public FilterGroup Filters { get; set; }

        /// <summary>
        /// Gets or sets the sorts.
        /// </summary>
        /// <value>
        /// The sorts.
        /// </value>
        [DataMember]
        public SortBuilder Sorts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationQueryBuilder"/> class.
        /// </summary>
        public PaginationQueryBuilder()
        {
            Filters = new FilterGroup();
            Sorts = new SortBuilder();
        }
    }
}
