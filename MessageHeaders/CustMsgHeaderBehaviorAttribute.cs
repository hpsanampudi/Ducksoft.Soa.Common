using Ducksoft.SOA.Common.Contracts;
using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Ducksoft.SOA.Common.MessageHeaders
{
    /// <summary>
    /// Class which is used to bind custom message header information to each incoming wcf request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustMsgHeaderBehaviorAttribute : Attribute, IDispatchMessageInspector,
        IClientMessageInspector, IEndpointBehavior, IServiceBehavior
    {
        /// <summary>
        /// The custom HTTP headers
        /// </summary>
        private readonly IDictionary<string, string> customHttpHeaders;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustMsgHeaderBehaviorAttribute"/> class.
        /// </summary>
        public CustMsgHeaderBehaviorAttribute() : this(new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustMsgHeaderBehaviorAttribute" /> class.
        /// </summary>
        /// <param name="customHttpHeaders">The custom HTTP headers.</param>
        public CustMsgHeaderBehaviorAttribute(IDictionary<string, string> customHttpHeaders)
        {
            this.customHttpHeaders = customHttpHeaders;
            logger = LoggingServiceHelper.AddOrGetLoggingService;
        }

        #region IDispatchMessageInspector

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)" /> method.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel,
            InstanceContext instanceContext)
        {
            //Retrieve Inbound Object from Request
            try
            {
                var bindingName =
                    OperationContext.Current.EndpointDispatcher.ChannelDispatcher.BindingName;

                bool isRestRequest = ((!string.IsNullOrWhiteSpace(bindingName)) &&
                    (bindingName.Contains("WebHttpBinding")));

                //Hp --> Logic: If service request is REST call then read custom data from 
                //http request header.
                if (isRestRequest)
                {
                    var httpMsgPropertyName = HttpRequestMessageProperty.Name;

                    //Hp --> Logic: Making sure we have a HttpRequestMessageProperty                    
                    if (request.Properties.ContainsKey(httpMsgPropertyName))
                    {
                        var httpRequestProperty =
                            request.Properties[httpMsgPropertyName] as HttpRequestMessageProperty;

                        var myCustHdrKeys =
                            httpRequestProperty.Headers.AllKeys.Where(K => K.StartsWith("custom-",
                            StringComparison.CurrentCultureIgnoreCase));

                        var incomingMsgProperties =
                            OperationContext.Current.IncomingMessageProperties;

                        foreach (var key in myCustHdrKeys)
                        {
                            var values = httpRequestProperty.Headers.GetValues(key);
                            if (incomingMsgProperties.ContainsKey(key))
                            {
                                incomingMsgProperties[key] = values;
                            }
                            else
                            {
                                incomingMsgProperties.Add(key, values);
                            }
                        }
                    }
                }
                else
                {
                    //Hp --> Logic: If service request is SOAP call then read custom data from 
                    //message header.
                    var incomingMsgProperties = OperationContext.Current.IncomingMessageProperties;
                    var myCustMsgHdrs = request.Headers.Where(K => K.Name.StartsWith("custom-",
                        StringComparison.CurrentCultureIgnoreCase));

                    foreach (var item in myCustMsgHdrs)
                    {
                        object value = null;
                        if (string.Equals("viaCode", item.Namespace))
                        {
                            value = request.Headers.GetHeader<CustomMsgHeader>(
                                item.Name, item.Namespace);
                        }
                        else if (string.Equals("viaConfig", item.Namespace))
                        {
                            value = request.Headers.GetHeader<string>(item.Name, item.Namespace);
                        }

                        if (null != value)
                        {
                            if (incomingMsgProperties.ContainsKey(item.Name))
                            {
                                incomingMsgProperties[item.Name] = value;
                            }
                            else
                            {
                                //CustomHeaderServerContextExtension.Current.CustomHeader = header;
                                incomingMsgProperties.Add(item.Name, value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#else
                logger.Warning(new CustomFault(exception: ex));
#endif
            }

            return null;
        }

        /// <summary>
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)" /> method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //No need to do anything else
        }

        #endregion

        #region IClientMessageInspector

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {
                //Hp --> Logic : If service request is SOAP call then read custom header data from
                //web.config file
                foreach (var item in customHttpHeaders)
                {
                    var typedConfigHeader = new MessageHeader<string>(item.Value);
                    var untypedConfigHeader =
                        typedConfigHeader.GetUntypedHeader(item.Key, "viaConfig");

                    request.Headers.Add(untypedConfigHeader);
                }

                //Hp --> Logic: Add if any custom header information is defined via code.                
                var typedCodeHeader =
                    new MessageHeader<CustomMsgHeader>(CustomMsgHeaderBuilder.Instance.HeaderInfo);

                var untypedCodeHeader = typedCodeHeader.GetUntypedHeader("custom-header", "viaCode");
                request.Headers.Add(untypedCodeHeader);
            }
            catch (Exception ex)
            {
                logger.Error(new CustomFault(exception: ex));
            }

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //No need to do anything else
        }

        #endregion

        #region IEndpointBehavior

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (null == channelDispatcher)
            {
                return;
            }

            foreach (var ed in channelDispatcher.Endpoints)
            {
                ed.DispatchRuntime.MessageInspectors.Add(
                    new CustMsgHeaderBehaviorAttribute(customHttpHeaders));
            }
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(
                new CustMsgHeaderBehaviorAttribute(customHttpHeaders));
        }

        #endregion

        #region IServiceBehaviour

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Adds the binding parameters.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="bindingParameters">The binding parameters.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
            foreach (var eDispatcher in serviceHostBase.ChannelDispatchers
                .Cast<ChannelDispatcher>().SelectMany(cDispatcher => cDispatcher.Endpoints))
            {
                eDispatcher.DispatchRuntime.MessageInspectors.Add(
                    new CustMsgHeaderBehaviorAttribute(customHttpHeaders));
            }
        }

        #endregion

    }
}
