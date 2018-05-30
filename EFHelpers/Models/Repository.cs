using System;
using System.Data.Services.Client;

namespace Ducksoft.SOA.Common.EFHelpers.Models
{
    /// <summary>
    /// Singleton Class which is used to query given entity set data through WCF data service.
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Ducksoft.SOA.Common.EFHelpers.Models.RepositoryBase{TEntities, TEntity}" />
    public sealed class Repository<TEntities, TEntity> : RepositoryBase<TEntities, TEntity>
        where TEntities : DataServiceContext
        where TEntity : class
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<Repository<TEntities, TEntity>> instance =
            new Lazy<Repository<TEntities, TEntity>>(() => new Repository<TEntities, TEntity>());

        /// <summary>
        /// Gets the instance of the singleton object: <see cref="Repository{TEntities, TEntity}"/>.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Repository<TEntities, TEntity> Instance
        {
            get { return (instance.Value); }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Repository{TEntities, TEntity}"/> class from being created.
        /// </summary>
        private Repository() : base()
        {
        }
    }
}
