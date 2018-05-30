using Ducksoft.SOA.Common.Contracts;
using Ducksoft.SOA.Common.WcfClients;
using Ninject.Modules;

namespace Ducksoft.SOA.Common.Infrastructure
{
    /// <summary>
    /// Class which is used to configure bindings related to dependency injection through NInject.
    /// </summary>
    public class LoggingModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<ILoggingService>().To<LoggingServiceClient>().InSingletonScope()
                .WithConstructorArgument("Logging");
        }
    }
}
