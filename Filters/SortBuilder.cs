using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    /// Class which is used to build Linq sort expression. 
    /// </summary>
    [DataContract(Name = "SortBuilder",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class SortBuilder
    {
        /// <summary>
        /// Gets or sets the sort columns.
        /// </summary>
        /// <value>
        /// The sort columns.
        /// </value>
        [DataMember]
        public Dictionary<string, ListSortDirection> SortColumns { get; set; }
    }
}
