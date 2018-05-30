using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.Utilities;
using Nelibur.ObjectMapper;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Models
{
    /// <summary>
    /// Singleton class which is used to map entity to DTO (or) vice versa while performing CRUD operations
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <typeparam name="TAudit">The type of the audit.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.EFHelpers.Interfaces.IMapEntityModel{TEntities, TAudit}" />
    /// <seealso cref="Ducksoft.Soa.Common.EFHelpers.Interfaces.IMapEntityModel" />
    public sealed class CrudEntityModel<TEntities, TAudit> : IMapEntityModel<TEntities, TAudit>
        where TEntities : DataServiceContext
        where TAudit : struct
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is 
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<CrudEntityModel<TEntities, TAudit>> instance =
            new Lazy<CrudEntityModel<TEntities, TAudit>>(() =>
            new CrudEntityModel<TEntities, TAudit>());

        /// <summary>
        /// Gets the instance of the singleton object: CrudEntityModel.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CrudEntityModel<TEntities, TAudit> Instance
        {
            get { return (instance.Value); }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [Inject]
        public ILoggingService Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudEntityModel" /> class.
        /// </summary>
        private CrudEntityModel()
        {
        }

        #region Interface: IMapEntityModel implementation
        /// <summary>
        /// Creates the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToCreate">The object to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TPKey Create<TDTO, TEntity, TPKey>(TDTO objectToCreate,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(TPKey);
            try
            {
                if (typeof(TDTO).Implements<IAuditColumns<TAudit>>())
                {
                    var record = (IAuditColumns<TAudit>)objectToCreate;
                    record.InsertDate = DateTime.Now;
                }

                var recordToCreate = TinyMapper.Map<TEntity>(objectToCreate);
                var repository = Repository<TEntities, TEntity>.Instance;

                result = repository.CreateRecord<TPKey>(recordToCreate, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while adding the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Gets the page data.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public PaginationData<TDTO> GetPageData<TDTO, TEntity>(
            IList<QueryOption> queryOptions = null, IDataServiceQuery<TEntity> query = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = new PaginationData<TDTO>();
            try
            {
                var repository = Repository<TEntities, TEntity>.Instance;
                var response = repository.GetPaginationData(queryOptions, query,
                    isAddOrAppendDeleteFilter, cancelToken);

                result = new PaginationData<TDTO>
                {
                    TotalItems = response?.TotalItems ?? 0,
                    PageData = response?.PageData?
                    .Select(I => TinyMapper.Map<TDTO>(I)).ToList() ?? new List<TDTO>()
                };
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while reading the {typeof(TEntity).FullName} page records.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public IEnumerable<TDTO> GetAll<TDTO, TEntity>(IDataServiceQuery<TEntity> query = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = new List<TDTO>();
            try
            {
                var repository = Repository<TEntities, TEntity>.Instance;
                var response = repository.GetAllRecords(query, isAddOrAppendDeleteFilter,
                    cancelToken);

                result = response?.Select(I => TinyMapper.Map<TDTO>(I)).ToList() ?? result;
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while reading the {typeof(TEntity).FullName} records.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Gets the single or default record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="query">The query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TDTO GetSingleOrDefault<TDTO, TEntity>(string odataFilterExpression,
            IDataServiceQuery<TEntity> query = null, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(TDTO);
            try
            {
                var repository = Repository<TEntities, TEntity>.Instance;
                var record = repository.GetSingleOrDefault(odataFilterExpression, query,
                    isAddOrAppendDeleteFilter, cancelToken);

                result = (record != null) ? TinyMapper.Map<TDTO>(record) : result;
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while reading the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToUpdate">The object to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TPKey Update<TDTO, TEntity, TPKey>(TDTO objectToUpdate, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(TPKey);
            try
            {
                if (typeof(TDTO).Implements<IAuditColumns<TAudit>>())
                {
                    var record = (IAuditColumns<TAudit>)objectToUpdate;
                    record.UpdateDate = DateTime.Now;
                }

                var recordToUpdate = TinyMapper.Map<TEntity>(objectToUpdate);
                var repository = Repository<TEntities, TEntity>.Instance;

                result = repository.UpdateRecord<TPKey>(recordToUpdate, isTracked,
                    isAddOrAppendDeleteFilter, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while updating the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToDelete">The object to delete.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TPKey Delete<TDTO, TEntity, TPKey>(TDTO objectToDelete, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(TPKey);
            try
            {
                //Hp --> Logic: Do not delete physical record, always update audit column DeleteDate            
                if (typeof(TDTO).Implements<IAuditColumns<TAudit>>())
                {
                    var record = (IAuditColumns<TAudit>)objectToDelete;
                    record.DeleteDate = DateTime.Now;
                }

                //Hp --> Logic: Map database record with DTO.
                var recordToDelete = TinyMapper.Map<TEntity>(objectToDelete);
                var repository = Repository<TEntities, TEntity>.Instance;

                result = repository.UpdateRecord<TPKey>(recordToDelete, isTracked,
                    isAddOrAppendDeleteFilter, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while deleting the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Deletes the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TPKey Delete<TEntity, TPKey>(string odataFilterExpression, TAudit userId,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            var result = default(TPKey);
            try
            {
                //Hp --> Get the record to delete from database based on filter query.
                var repository = Repository<TEntities, TEntity>.Instance;
                var recordToDelete = repository.GetSingleOrDefault(odataFilterExpression,
                    isAddOrAppendDeleteFilter: isAddOrAppendDeleteFilter, cancelToken: cancelToken);

                //Hp --> Logic: Do not delete physical record, always update audit column DeleteDate                  
                if (recordToDelete.IsHavingAuditColumns(AuditColumnTypes.Delete))
                {
                    recordToDelete.SetPropertyValue("DeleteBy", userId);
                    recordToDelete.SetPropertyValue("DeleteDate", DateTime.Now);
                }

                result = repository.UpdateRecord<TPKey>(recordToDelete, true,
                    isAddOrAppendDeleteFilter, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while deleting the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Purges the specified object.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="objectToPurge">The object to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public bool Purge<TDTO, TEntity, TPKey>(TDTO objectToPurge, bool isTracked = false,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var isSuccess = false;
            try
            {
                //Hp --> Logic: Map database record with DTO.
                var recordToPurge = TinyMapper.Map<TEntity>(objectToPurge);
                var repository = Repository<TEntities, TEntity>.Instance;

                //Hp --> Logic: Delete the physical record permanently 
                isSuccess = repository.PurgeRecord<TPKey>(recordToPurge, isTracked,
                    isAddOrAppendDeleteFilter, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while purging the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (isSuccess);
        }

        /// <summary>
        /// Purge the database record by given OData filter expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public bool Purge<TEntity, TPKey>(string odataFilterExpression,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            var isSuccess = false;
            try
            {
                //Hp --> Logic: Delete the physical record permanently 
                var repository = Repository<TEntities, TEntity>.Instance;
                isSuccess = repository.PurgeRecord<TPKey>(odataFilterExpression,
                    isAddOrAppendDeleteFilter, cancelToken);
            }
            catch (FaultException<CustomFault> ex)
            {
                var fault = ex.Detail;
                fault.IsNotifyUser = true;
                Logger.Error(fault);
            }
            catch (Exception ex)
            {
                var custMessage =
                    $"An error occurred while purging the {typeof(TEntity).FullName} record.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (isSuccess);
        }
        #endregion
    }
}
