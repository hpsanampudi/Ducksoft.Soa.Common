using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.EFHelpers.Models;
using Ninject.Modules;

namespace Ducksoft.Soa.Common.Infrastructure
{
    /// <summary>
    /// Class which is used to configure bindings related to dependency injection through NInject.
    /// </summary>
    public class MapEntityModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<IMapEntityModel>().ToConstant(CrudEntityModel.Instance);
        }
    }
}
