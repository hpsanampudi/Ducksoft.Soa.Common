using Ducksoft.Soa.Common.Utilities;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Ducksoft.Soa.Common.WcfClientHelpers
{
    /// <summary>
    /// Factory for creating WCF service clients.
    /// </summary>
    public static class ServiceClientFactory
    {
        #region CreateAndWrap

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>()
            where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>());
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            string endpointConfigName) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(endpointConfigName));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="endpoint">The service endpoint.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            ServiceEndpoint endpoint) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(endpoint));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpoint">The service endpoint.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance, ServiceEndpoint endpoint) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance, endpoint));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance, string endpointConfigName) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance, endpointConfigName));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            string endpointConfigName, EndpointAddress remoteAddress) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(endpointConfigName, remoteAddress));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            string endpointConfigName, string remoteAddress) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(endpointConfigName, remoteAddress));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="binding">The binding to use.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            Binding binding, EndpointAddress remoteAddress) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(binding, remoteAddress));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="binding">The binding to use.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance, binding, remoteAddress));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance, string endpointConfigName,
            EndpointAddress remoteAddress) where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance, endpointConfigName,
                remoteAddress));
        }

        /// <summary>
        /// Creates a service client instance and wraps it.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="callbackInstance">The instance callback.</param>
        /// <param name="endpointConfigName">The endpoint name.</param>
        /// <param name="remoteAddress">The endpoint address.</param>
        /// <returns>
        /// The wrapped instance.
        /// </returns>
        public static ServiceClientWrapper<TChannel> CreateAndWrap<TChannel>(
            InstanceContext callbackInstance, string endpointConfigName, string remoteAddress)
            where TChannel : class
        {
            return (new ServiceClientWrapper<TChannel>(callbackInstance, endpointConfigName,
                remoteAddress));
        }
        #endregion

        #region InvokeMethod

        /// <summary>
        /// Invokes a service method.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="invocation">The function to execute.</param>
        /// <exception cref="System.ArgumentNullException">invocation</exception>
        /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// This method creates a service client, invokes a method and closes the client.
        /// It is useful for invoking a single method without the boiler plate code.
        /// Do not use this method if multiple methods need to be called on the same service instance.
        /// </remarks>
        public static void InvokeMethod<TChannel>(Action<TChannel> invocation)
            where TChannel : class
        {
            ErrorBase.CheckArgIsNull(invocation, () => invocation);
            using (var proxy = CreateAndWrap<TChannel>())
            {
                invocation(proxy.Client);
            }
        }

        /// <summary>
        /// Invokes a service method.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <param name="invocation">The function to execute.</param>
        /// <param name="initializer">The initializer to create the proxy.
        /// If this is <see langword="null" /> then the default initializer is used.</param>
        /// <exception cref="System.ArgumentNullException">invocation</exception>
        /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// This method creates a service client, invokes a method and closes the client.
        /// It is useful for invoking a single method without the boiler plate code.
        /// Do not use this method if multiple methods need to be called on the same service instance.
        /// </remarks>
        public static void InvokeMethod<TChannel>(Action<TChannel> invocation,
            Func<ServiceClientWrapper<TChannel>> initializer) where TChannel : class
        {
            ErrorBase.CheckArgIsNull(invocation, () => invocation);
            var init = initializer ?? (CreateAndWrap<TChannel>);
            using (var proxy = init())
            {
                invocation(proxy.Client);
            }
        }

        /// <summary>
        /// Invokes a service method.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <typeparam name="TResult">The return obj from the method.</typeparam>
        /// <param name="invocation">The function to execute.</param>
        /// <returns>
        /// The result of the method invocation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">invocation</exception>
        /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// This method creates a service client, invokes a method and closes the client.
        /// It is useful for invoking a single method without the boiler plate code.
        /// Do not use this method if multiple methods need to be called
        /// on the same service instance.
        /// </remarks>
        public static TResult InvokeMethod<TChannel, TResult>(Func<TChannel, TResult> invocation)
            where TChannel : class
        {
            ErrorBase.CheckArgIsNull(invocation, () => invocation);
            using (var proxy = CreateAndWrap<TChannel>())
            {
                return invocation(proxy.Client);
            }
        }

        /// <summary>
        /// Invokes a service method.
        /// </summary>
        /// <typeparam name="TChannel">The service interface.</typeparam>
        /// <typeparam name="TResult">The return obj from the method.</typeparam>
        /// <param name="invocation">The function to execute.</param>
        /// <param name="initializer">The initializer to create the proxy.
        /// If <see langword="null" /> then the default initializer is used.</param>
        /// <returns>
        /// The result of the method invocation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">invocation</exception>
        /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" />.</exception>
        /// <remarks>
        /// This method creates a service client, invokes a method and closes the client.
        /// It is useful for invoking a single method without the boiler plate code.
        /// Do not use this method if multiple methods need to be called on the same service instance.
        /// </remarks>
        public static TResult InvokeMethod<TChannel, TResult>(Func<TChannel, TResult> invocation,
            Func<ServiceClientWrapper<TChannel>> initializer) where TChannel : class
        {
            ErrorBase.CheckArgIsNull(invocation, () => invocation);
            var init = initializer ?? (CreateAndWrap<TChannel>);
            using (var proxy = init())
            {
                return invocation(proxy.Client);
            }
        }
        #endregion
    }
}
