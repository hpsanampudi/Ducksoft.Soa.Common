using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ninject.Modules;
using System.Collections.Generic;

namespace Ducksoft.Soa.Common.Infrastructure
{
    /// <summary>
    /// Static entity model mapper helper class which is used to map entity models in IOC container.
    /// </summary>
    public static class MapEntityModelHelper
    {
        /// <summary>
        /// Adds or get entity model mapper to IOC container.
        /// </summary>
        /// <returns></returns>
        public static IMapEntityModel AddOrGetMapEntityModel
        {
            get
            {
                IMapEntityModel mapper = null;
                try
                {
                    mapper = NInjectHelper.Instance.GetInstance<IMapEntityModel>();
                }
                catch
                {
                    //Hp --> Logic: If logging service instance is not yet loaded in IOC container,
                    //then load it here.
                    NInjectHelper.Instance.LoadModules(new List<INinjectModule>
                    {
                        new MapEntityModule(),
                    });

                    mapper = NInjectHelper.Instance.GetInstance<IMapEntityModel>();
                }

                return (mapper);
            }
        }
    }
}
