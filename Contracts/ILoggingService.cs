using Ducksoft.SOA.Common.DataContracts;
using System;
using System.ServiceModel;

namespace Ducksoft.SOA.Common.Contracts
{
    /// <summary>
    /// Interface in which logging WCF service contracts exposed.
    /// </summary>
    [ServiceContract(Namespace = "http://ducksoftware.co.uk/SOA/WCF/Contracts",
        SessionMode = SessionMode.Allowed)]
    public interface ILoggingService
    {
        /// <summary>
        /// Debugs the specified message and exception.
        /// </summary>
        /// <param name="fault">The fault.</param>
        [OperationContract]
        Guid Debug(CustomFault fault);

        /// <summary>
        /// Errors the specified message and exception.
        /// </summary>
        /// <param name="fault">The fault.</param>
        [OperationContract]
        Guid Error(CustomFault fault);

        /// <summary>
        /// Fatals the specified message and exception.
        /// </summary>
        /// <param name="fault">The fault.</param>
        [OperationContract]
        Guid Fatal(CustomFault fault);

        /// <summary>
        /// Warnings the specified message and exception.
        /// </summary>
        /// <param name="fault">The fault.</param>
        [OperationContract]
        Guid Warning(CustomFault fault);

        /// <summary>
        /// Informations the specified message and exception.
        /// </summary>
        /// <param name="fault">The fault.</param>
        [OperationContract]
        Guid Information(CustomFault fault);
    }
}
