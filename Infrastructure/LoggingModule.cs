using Ninject.Modules;
using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.WcfClients;

namespace Ducksoft.Soa.Common.Infrastructure
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
