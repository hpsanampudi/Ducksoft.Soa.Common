using System;
using System.Threading;

namespace Ducksoft.Soa.Common.WcfClientHelpers
{
    /// <summary>
    /// Provides abstract base class for wrapping WCF channels.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    public abstract class ServiceChannelClient<T> : IPersistentChannel where T : class
    {
        /// <summary>
        /// The channel
        /// </summary>
        private ServiceClientWrapper<T> channel;

        /// <summary>
        /// Determines if the channel is already open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has open channel; otherwise, <c>false</c>.
        /// </value>
        protected bool HasOpenChannel
        {
            get
            {
                return (null != channel);
            }
        }

        /// <summary>
        /// Creates an instance of the client wrapper.
        /// </summary>
        /// <returns>
        /// The client wrapper.
        /// </returns>
        protected virtual ServiceClientWrapper<T> CreateInstance()
        {
            return (ServiceClientFactory.CreateAndWrap<T>());
        }

        /// <summary>
        /// Invokes a method on the channel.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        protected virtual void InvokeMethod(Action<T> action)
        {
            if (HasOpenChannel)
            {
                action(channel.Client);
            }
            else
            {
                ServiceClientFactory.InvokeMethod(action, CreateInstance);
            }
        }

        /// <summary>
        /// Invokes a method on the channel.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action to perform.</param>
        /// <returns></returns>
        protected virtual TResult InvokeMethod<TResult>(Func<T, TResult> action)
        {
            return (HasOpenChannel ? action(channel.Client) :
                ServiceClientFactory.InvokeMethod(action, CreateInstance));
        }

        /// <summary>
        /// Opens the persistent channel if it is not open.
        /// </summary>
        protected virtual void OpenChannel()
        {
            if (null == channel)
            {
                var myChannel = CreateInstance();
                var oldChannel = Interlocked.CompareExchange(ref channel, myChannel, null);
                if (null != oldChannel)
                {
                    myChannel.Close();
                }
            }
        }

        /// <summary>
        /// Closes the persistent channel if it is open.
        /// </summary>
        protected virtual void CloseChannel()
        {
            var myChannel = Interlocked.Exchange(ref channel, null);
            if (null != myChannel)
            {
                myChannel.Close();
            }
        }

        #region Interface : IPersistentChannel implementation
        /// <summary>
        /// Closes the channel.
        /// </summary>
        void IPersistentChannel.Close()
        {
            CloseChannel();
        }

        /// <summary>
        /// Opens the channel.
        /// </summary>
        void IPersistentChannel.Open()
        {
            OpenChannel();
        }
        #endregion
    }
}
