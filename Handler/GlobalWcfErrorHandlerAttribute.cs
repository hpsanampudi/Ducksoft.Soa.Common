using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Infrastructure;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Ducksoft.Soa.Common.Handlers
{
    /// <summary>
    /// Class which is used to handle any unhandled exceptions occurs in WCF service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GlobalWcfErrorHandlerAttribute : Attribute, IErrorHandler, IContractBehavior
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalWcfErrorHandlerAttribute"/> class.
        /// </summary>
        public GlobalWcfErrorHandlerAttribute()
        {
            logger = LoggingServiceHelper.AddOrGetLoggingService;
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>
        /// true if Windows Communication Foundation (WCF) should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single" />; otherwise, false. The default is false.
        /// </returns>
        public bool HandleError(Exception error)
        {
            if (error is FaultException) return false; //WCF will handle this

            return true;
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1" /> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception" /> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message" /> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException) return; // WCF will handle this

            //Hp --> BugFix: When unhandled exception occurs it is not throwing custom fault object.
            var myFaultException = new CustomFault("Unhandled service exception!", error);
            myFaultException.TicketNumber = logger.Error(myFaultException);

            var messageFault = MessageFault.CreateFault(
                new FaultCode("Unhandled service exception!"),
                new FaultReason(error.Message),
                myFaultException,
                new NetDataContractSerializer());

            fault = Message.CreateMessage(version, messageFault, null);
        }

        /// <summary>
        /// Configures any binding elements to support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract description to modify.</param>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description for which the extension is intended.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientRuntime">The client runtime.</param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description to be modified.</param>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="dispatchRuntime">The dispatch runtime that controls service execution.</param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(this);
        }

        /// <summary>
        /// Implement to confirm that the contract and endpoint can support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract to validate.</param>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
    }
}
