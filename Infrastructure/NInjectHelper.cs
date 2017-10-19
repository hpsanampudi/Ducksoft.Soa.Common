using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Ducksoft.Soa.Common.Infrastructure
{
    /// <summary>
    /// Singleton class, which is used to used to load ninject bindings.
    /// </summary>
    public sealed class NInjectHelper : IDisposable
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is 
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<NInjectHelper> instance =
            new Lazy<NInjectHelper>(() => new NInjectHelper());

        /// <summary>
        /// Gets the instance of the singleton object: IKernel.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static NInjectHelper Instance
        {
            get { return (instance.Value); }
        }

        /// <summary>
        /// The ninject kernel instance
        /// </summary>
        private readonly IKernel kernelInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="NInjectHelper"/> class from being created.
        /// </summary>
        private NInjectHelper()
        {
            kernelInstance = new StandardKernel();
        }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <returns></returns>
        public IKernel GetKernel()
        {
            return (kernelInstance);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            return ((T)kernelInstance.GetService(typeof(T)));
        }

        /// <summary>
        /// Loads the modules.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void LoadModules(IEnumerable<INinjectModule> modules)
        {
            if ((null != modules) && (modules.Any()))
            {
                kernelInstance.Load(modules);
            }
        }

        /// <summary>
        /// Automatically loads the calling assembly related ninject module.
        /// </summary>
        /// <returns></returns>
        public void LoadAssembly()
        {
            LoadModules(Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(T => T.GetInterfaces().Any(I => (I == typeof(INinjectModule))))
                .Select(T => (INinjectModule)Activator.CreateInstance(T)));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            kernelInstance.Dispose();
        }
    }
}
