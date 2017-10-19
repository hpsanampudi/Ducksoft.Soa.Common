using Ducksoft.Soa.Common.DataContracts;
using System;

namespace Ducksoft.Soa.Common.EFHelpers.Interfaces
{
    /// <summary>
    /// Interface which is used to load Entities with user provided connection string information.
    /// </summary>
    public interface IEntityLoader
    {
        /// <summary>
        /// Gets the data SVC URL.
        /// </summary>
        /// <value>
        /// The data SVC URL.
        /// </value>
        Uri DataSvcUrl { get; }

        /// <summary>
        /// Gets the connection information.
        /// </summary>
        /// <value>
        /// The connection information.
        /// </value>
        DbConnectionInfo ConnectionInfo { get; }
    }
}
