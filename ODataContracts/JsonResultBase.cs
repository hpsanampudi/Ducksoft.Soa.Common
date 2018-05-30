using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.ODataContracts
{
    /// <summary>
    /// Data class which stores the WCF data service JSON Result base information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "JsonResultBaseOfType{0}",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class JsonResultBase<T> where T : ContentBase
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [DataMember(Name = "d")]
        public ComposedResult<T> Result { get; set; }
    }
}
