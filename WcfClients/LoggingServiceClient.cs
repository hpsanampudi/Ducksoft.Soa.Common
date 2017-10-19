using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.WcfClientHelpers;
using System;

namespace Ducksoft.Soa.Common.WcfClients
{
    /// <summary>
    /// Class which is used to create wcf logging service client.
    /// </summary>
    public class LoggingServiceClient : ServiceChannelClient<ILoggingService>, ILoggingService
    {
        /// <summary>
        /// Gets the name of the end point.
        /// </summary>
        /// <value>
        /// The name of the end point.
        /// </value>
        public string EndPointName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingServiceClient"/> class.
        /// </summary>
        /// <param name="endpointConfigName">Name of the endpoint configuration.</param>
        public LoggingServiceClient(string endpointConfigName)
        {
            EndPointName = endpointConfigName;
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        protected override ServiceClientWrapper<ILoggingService> CreateInstance()
        {
            return (ServiceClientFactory.CreateAndWrap<ILoggingService>(EndPointName));
        }

        /// <summary>
        /// Debugs the specified fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public Guid Debug(CustomFault fault)
        {
            return (InvokeMethod(client => client.Debug(fault)));
        }

        /// <summary>
        /// Errors the specified fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public Guid Error(CustomFault fault)
        {
            return (InvokeMethod(client => client.Error(fault)));
        }

        /// <summary>
        /// Fatals the specified fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public Guid Fatal(CustomFault fault)
        {
            return (InvokeMethod(client => client.Fatal(fault)));
        }

        /// <summary>
        /// Warnings the specified fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public Guid Warning(CustomFault fault)
        {
            return (InvokeMethod(client => client.Warning(fault)));
        }

        /// <summary>
        /// Informations the specified fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public Guid Information(CustomFault fault)
        {
            return (InvokeMethod(client => client.Information(fault)));
        }
    }
}
