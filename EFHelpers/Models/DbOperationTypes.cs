using Ducksoft.SOA.Common.Utilities;

namespace Ducksoft.SOA.Common.EFHelpers.Models
{
    /// <summary>
    /// Enum which stores database CRUD operation types
    /// </summary>
    public enum DbOperationTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// The create
        /// </summary>
        [EnumDescription("Create")]
        Create,
        /// <summary>
        /// The read
        /// </summary>
        [EnumDescription("Read")]
        Read,
        /// <summary>
        /// The update
        /// </summary>
        [EnumDescription("Update")]
        Update,
        /// <summary>
        /// The delete
        /// </summary>
        [EnumDescription("Delete")]
        Delete
    }
}
