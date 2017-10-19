using Ninject.Modules;
using Ducksoft.Soa.Common.Contracts;
using System.Collections.Generic;

namespace Ducksoft.Soa.Common.Infrastructure
{
    /// <summary>
    /// Static logging helper class which is used to load logging service in IOC container.
    /// </summary>
    public static class LoggingServiceHelper
    {
        /// <summary>
        /// Adds or get logging service to IOC container.
        /// </summary>
        /// <returns></returns>
        public static ILoggingService AddOrGetLoggingService
        {
            get
            {
                ILoggingService logger = null;
                try
                {
                    logger = NInjectHelper.Instance.GetInstance<ILoggingService>();
                }
                catch
                {
                    //Hp --> Logic: If logging service instance is not yet loaded in IOC container,
                    //then load it here.
                    NInjectHelper.Instance.LoadModules(new List<INinjectModule>
                {
                    new LoggingModule(),
                });

                    logger = NInjectHelper.Instance.GetInstance<ILoggingService>();
                }

                return (logger);
            }
        }
    }
}
