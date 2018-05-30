using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Ducksoft.SOA.Common.WcfClientHelpers
{
    /// <summary>
    /// Provides a wrapper around a WCF service client.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <remarks>
    /// The wrapper can be safely used with a <see langword="using"/> block.  WCF exceptions
    /// result in the client being aborted if necessary.
    /// </remarks>
    public class ServiceClientWrapper<TChannel> : ClientBase<TChannel>, IDisposable
        where TChannel : class
    {
        #region Construction

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        public ServiceClientWrapper()
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="endpointConfigName">The endpoint name.</param>
        public ServiceClientWrapper(string endpointConfigName)
            : base(endpointConfigName)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="endpoint">The service endpoint.</param>
        public ServiceClientWrapper(ServiceEndpoint endpoint)
            : base(endpoint)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpoint">The service endpoint.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance, ServiceEndpoint endpoint)
            : base(callbackInstance, endpoint)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance, string endpointConfigName)
            : base(callbackInstance, endpointConfigName)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(string endpointConfigName, EndpointAddress remoteAddress)
            : base(endpointConfigName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(string endpointConfigName, string remoteAddress)
            : base(endpointConfigName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="binding">The binding to use.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="binding">The binding to use.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance, Binding binding,
            EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance, string endpointConfigName,
            EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ServiceClientWrapper{T}" /> class.
        /// </summary>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        public ServiceClientWrapper(InstanceContext callbackInstance, string endpointConfigName,
            string remoteAddress)
            : base(callbackInstance, endpointConfigName, remoteAddress)
        {
        }
        #endregion

        /// <summary>
        /// Gets the wrapped client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TChannel Client
        {
            get
            {
                return (Channel);
            }
        }

        /// <summary>
        /// Closes the client if it is open.
        /// </summary>
        /// <remarks>
        /// This method properly closes the client even in the face of aborted connections.
        /// </remarks>
        public new void Close()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>
        /// Implicit conversion to <typeparamref name="TChannel" />.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>
        /// The underlying client.
        /// </returns>
        public static implicit operator TChannel(ServiceClientWrapper<TChannel> value)
        {
            return (value.Client);
        }

        #region Protected Members

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> if disposing.</param>
        /// <remarks>
        /// The default implementation closes the client if it is not already closed.  If the client is
        /// in an invalid state then the connection is aborted.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (CommunicationState.Closed != State)
                {
                    base.Close();
                }
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
