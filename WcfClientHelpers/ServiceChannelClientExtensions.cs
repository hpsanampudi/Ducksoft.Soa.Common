using System;
using System.Reactive.Disposables;

namespace Ducksoft.SOA.Common.WcfClientHelpers
{
    /// <summary>
    /// Static class which provides WCF service client related extension methods.
    /// </summary>
    public static class ServiceChannelClientExtensions
    {
        /// <summary>
        /// Opens the persistent channel.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IDisposable OpenPersistentChannel<T>(this T source)
            where T : IPersistentChannel
        {
            if (null == source) return (Disposable.Empty);

            source.Open();
            return (Disposable.Create(source.Close));
        }
    }
}
