using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.Infrastructure;
using Ducksoft.Soa.Common.Utilities;
using Nelibur.ObjectMapper;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Ducksoft.Soa.Common.EFHelpers.Models
{
    /// <summary>
    /// Singleton class which is used to map entity to DTO (or) vice versa while performing CRUD operations
    /// </summary>
    /// <seealso cref="CKA.SOA.DAL.Common.Interfaces.IMapEntityModel" />
    public class CrudEntityModel : IMapEntityModel
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is 
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<CrudEntityModel> instance =
            new Lazy<CrudEntityModel>(() => new CrudEntityModel());

        /// <summary>
        /// Gets the instance of the singleton object: CrudEntityModel.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CrudEntityModel Instance
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
        /// <typeparam name="TAudit">The type of the audit.</typeparam>
        /// <param name="objectToCreate">The object to create.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public int Create<TDTO, TEntity, TAudit>(TDTO objectToCreate,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
            where TAudit : struct
        {
            var result = -1;
            try
            {
                if (typeof(TDTO).Implements<IAuditColumns<TAudit>>())
                {
                    var record = (IAuditColumns<TAudit>)objectToCreate;
                    record.InsertDate = DateTime.Now;
                }

                var recordToCreate = TinyMapper.Map<TEntity>(objectToCreate);
                var repository = GetRepository<TEntity>();

                result = repository.CreateRecord(recordToCreate, cancelToken);
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
        /// Gets all.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public PaginationData<TDTO> GetAll<TDTO, TEntity>(IList<QueryOption> queryOptions = null,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(PaginationData<TDTO>);
            try
            {
                var repository = GetRepository<TEntity>();
                var response = repository.GetAllRecords(queryOptions, cancelToken);

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
                    $"An error occurred while reading the {typeof(TEntity).FullName} records.";

                var fault = new CustomFault(custMessage, ex, isNotifyUser: true);
                Logger.Error(fault);
            }

            return (result);
        }

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public TDTO GetSingleOrDefault<TDTO, TEntity>(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var result = default(TDTO);
            try
            {
                var repository = GetRepository<TEntity>();
                var record = repository.GetSingleOrDefault(odataFilterExpression, cancelToken);

                result = (record != null) ? TinyMapper.Map<TDTO>(record) : null;
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
        /// Updates the specified object to update.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TAudit">The type of the audit.</typeparam>
        /// <param name="objectToUpdate">The object to update.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public int Update<TDTO, TEntity, TAudit>(TDTO objectToUpdate, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
            where TAudit : struct
        {
            var result = -1;
            try
            {
                if (typeof(TDTO).Implements<IAuditColumns<TAudit>>())
                {
                    var record = (IAuditColumns<TAudit>)objectToUpdate;
                    record.UpdateDate = DateTime.Now;
                }

                var recordToUpdate = TinyMapper.Map<TEntity>(objectToUpdate);
                var repository = GetRepository<TEntity>();

                result = repository.UpdateRecord(recordToUpdate, isTracked, cancelToken);
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
        /// Deletes the specified object to delete.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TAudit">The type of the audit.</typeparam>
        /// <param name="objectToDelete">The object to delete.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public int Delete<TDTO, TEntity, TAudit>(TDTO objectToDelete, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
            where TAudit : struct
        {
            var result = -1;
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
                var repository = GetRepository<TEntity>();

                result = repository.UpdateRecord(recordToDelete, isTracked, cancelToken);
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
        /// <typeparam name="TAudit">The type of the audit.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public int Delete<TEntity, TAudit>(string odataFilterExpression, TAudit userId,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TAudit : struct
        {
            var result = -1;
            try
            {
                //Hp --> Get the record to delete from database based on filter query.
                var repository = GetRepository<TEntity>();
                var recordToDelete = repository.GetSingleOrDefault(odataFilterExpression,
                    cancelToken);

                //Hp --> Logic: Do not delete physical record, always update audit column DeleteDate                  
                if (recordToDelete.IsHavingAuditColumns(AuditColumnTypes.Delete))
                {
                    recordToDelete.SetPropertyValue("DeleteBy", userId);
                    recordToDelete.SetPropertyValue("DeleteDate", DateTime.Now);
                }

                result = repository.UpdateRecord(recordToDelete, true, cancelToken);
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
        /// Deletes the specified object to purge.
        /// </summary>
        /// <typeparam name="TDTO">The type of the map.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="objectToPurge">The object to purge.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public bool Purge<TDTO, TEntity>(TDTO objectToPurge, bool isTracked = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TDTO : class
            where TEntity : class
        {
            var isSuccess = false;
            try
            {
                //Hp --> Logic: Map database record with DTO.
                var recordToPurge = TinyMapper.Map<TEntity>(objectToPurge);
                var repository = GetRepository<TEntity>();

                //Hp --> Logic: Delete the physical record permanently 
                isSuccess = repository.PurgeRecord(recordToPurge, isTracked, cancelToken);
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
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public bool Purge<TEntity>(string odataFilterExpression,
            CancellationToken cancelToken = default(CancellationToken)) where TEntity : class
        {
            var isSuccess = false;
            try
            {
                //Hp --> Logic: Delete the physical record permanently 
                var repository = GetRepository<TEntity>();
                isSuccess = repository.PurgeRecord(odataFilterExpression, cancelToken);
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
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IDbRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return (NInjectHelper.Instance.GetInstance<IDbRepository<TEntity>>());
        }
        #endregion
    }
}
