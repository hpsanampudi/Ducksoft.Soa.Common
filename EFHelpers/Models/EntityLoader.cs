using Ninject;
using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Linq;
using System.Threading;
using System.Web;

namespace Ducksoft.Soa.Common.EFHelpers.Models
{
    /// <summary>
    /// Class which is used to load Entities with user provided connection string information.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityLoader<TEntity> : IEntityLoader where TEntity : DataServiceContext
    {
        /// <summary>
        /// Gets the data service client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected virtual TEntity DataSvcClient { get; private set; }

        /// <summary>
        /// Gets the data service URL.
        /// </summary>
        /// <value>
        /// The data service URL.
        /// </value>
        public Uri DataSvcUrl { get; private set; }

        /// <summary>
        /// Gets the connection information.
        /// </summary>
        /// <value>
        /// The connection information.
        /// </value>
        public DbConnectionInfo ConnectionInfo { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [Inject]
        public ILoggingService Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLoader{TEntity}"/> class.
        /// </summary>
        public EntityLoader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLoader{TEntity}"/> class.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        public EntityLoader(Uri dataServiceUrl)
            : this(dataServiceUrl, null)
        {
            //Hp --> Logic: If connectionInfo is null then it takes value mentioned inside 
            //corrsponding web.config file.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLoader{TEntity}"/> class.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        /// <param name="connectionInfo">The connection information.</param>
        public EntityLoader(Uri dataServiceUrl, DbConnectionInfo connectionInfo)
        {
            Create(dataServiceUrl, connectionInfo);
        }

        /// <summary>
        /// Creates the entities with specified data service URL and db connection string.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        /// <param name="connectionInfo">The connection information.</param>
        protected void Create(Uri dataServiceUrl, DbConnectionInfo connectionInfo = null)
        {
            ErrorBase.CheckArgIsNull(dataServiceUrl, () => dataServiceUrl);
            DataSvcUrl = dataServiceUrl;
            ConnectionInfo = connectionInfo;

            //Hp --> BugFix: Failed to redirect to user provided db connection.
            //Note: Store the result in local variable and don't set it to DataSvcClient property
            //object because there is a chance it can overrriden at BL.
            var client = new DataServiceContext(dataServiceUrl) as TEntity;

            //Hp --> Logic: If TEntity is derived from DataServiceContext then we need to use 
            //reflection to initialize the client object.
            if (null == client)
            {
                DataSvcClient = (TEntity)Activator.CreateInstance(typeof(TEntity),
                    new object[]
                    {
                        dataServiceUrl
                    });
            }
            else
            {
                DataSvcClient = client;
            }

            //Hp --> BugFix: View data is not getting latest changes from DB.
            DataSvcClient.MergeOption = MergeOption.OverwriteChanges;

            EventHandler<SendingRequest2EventArgs> handler = (sender, eArgs) =>
            {
                if (null == connectionInfo) return;

                //Hp --> Logic: Redirect the EF database connection to user provided value.
                foreach (var item in connectionInfo.ToDictonary())
                {
                    eArgs.RequestMessage.SetHeader(item.Key, item.Value);
                }
            };

            //Hp --> Logic: Always supress the event before calling.
            DataSvcClient.SendingRequest2 -= handler;
            DataSvcClient.SendingRequest2 += handler;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected virtual IEnumerable<QueryOperationResponse<T>> GetAll<T>(
            DataServiceQuery<T> query, CancellationToken cancelToken = default(CancellationToken))
            where T : class
        {
            // Hp --> Logic: In order to get all records we need to loop untill data service 
            //continuation token is null.
            DataServiceQueryContinuation<T> token = null;
            var response = query.Execute() as QueryOperationResponse<T>;
            do
            {
                if ((default(CancellationToken) != cancelToken) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                // If nextLink is not null, then there is a new page to load. 
                if (null != token)
                {
                    // Load the new page from the next link URI.
                    response = DataSvcClient.Execute(token);
                }

                //Hp --> Logic: To improve the performance yield the return value.
                if (null != response) yield return (response);
            } while ((null != response) && (null != (token = response.GetContinuation())));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected virtual IQueryable<T> GetAll<T>(string query,
            CancellationToken cancelToken = default(CancellationToken))
            where T : class
        {
            ErrorBase.CheckArgIsNullOrDefault(query, () => query);

            if ((default(CancellationToken) != cancelToken) &&
                    (cancelToken.IsCancellationRequested))
            {
                cancelToken.ThrowIfCancellationRequested();
            }

            var queryUri = new Uri(query, UriKind.RelativeOrAbsolute);
            //Hp --> BugFix: Cannot materialize a collection of a primitives or complex 
            //without the type 'T' being a collection.
            //Comments: When calling a store procedure returning complex data type then pass
            //other parameters of Execute method as shown below:
            return (DataSvcClient.Execute<T>(queryUri, "GET", false).AsQueryable());
        }

        /// <summary>
        /// Executes the OData query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        protected virtual IEnumerable<QueryOperationResponse<T>> ExecuteODataQuery<T>(
            DataServiceQuery<T> baseQuery, IList<QueryOption> queryOptions = null,
            CancellationToken cancelToken = default(CancellationToken)) where T : class
        {
            return (GetAll<T>(LoadQueryOptions(baseQuery, queryOptions), cancelToken));
        }

        /// <summary>
        /// Loads the query options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        protected virtual DataServiceQuery<T> LoadQueryOptions<T>(DataServiceQuery<T> baseQuery,
            IList<QueryOption> queryOptions = null) where T : class
        {
            var query = baseQuery;
            if (!queryOptions?.Any() ?? true)
            {
                return (query);
            }

            queryOptions.ToList().ForEach(item =>
            {
                query = query.AddQueryOption(item.Option, item.Query);
            });

            return (query);
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public PaginationData<T> GetPaginationData<T>(DataServiceQuery<T> baseQuery,
            int? pageIndex = default(int?), int? pageSize = default(int?),
            CancellationToken cancelToken = default(CancellationToken)) where T : class
        {
            if (null == baseQuery)
            {
                return (null);
            }

            var qryOptions = new List<QueryOption>();
            if ((null != pageIndex) && (null != pageSize))
            {
                qryOptions.AddRange(new List<QueryOption>
                {
                    new QueryOption
                    {
                        Option = "$skip",
                        Query = (pageIndex * pageSize).ToString()
                    },
                    new QueryOption
                    {
                        Option = "$top",
                        Query = pageSize.ToString()
                    }
                });
            }

            var response = ExecuteODataQuery(baseQuery, qryOptions, cancelToken);
            return (new PaginationData<T>
            {
                TotalItems = response.FirstOrDefault().TotalCount,
                PageData = response.SelectMany(item => item)
            });
        }

        /// <summary>
        /// Gets the total records count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public long GetTotalRecordsCount<T>(DataServiceQuery<T> baseQuery,
            CancellationToken cancelToken = default(CancellationToken))
            where T : class
        {
            var url = baseQuery.RequestUri.Query;
            var isExists = HttpUtility.ParseQueryString(url).AllKeys.Contains("$inlinecount");
            if (!isExists)
            {
                baseQuery = LoadQueryOptions(baseQuery, new List<QueryOption>
                {
                    new QueryOption
                    {
                        Option = "$inlinecount",
                        Query = "allpages"
                    }
                });
            }

            if ((default(CancellationToken) != cancelToken) &&
                    (cancelToken.IsCancellationRequested))
            {
                cancelToken.ThrowIfCancellationRequested();
            }

            var response = baseQuery.Execute() as QueryOperationResponse<T>;
            return (response.TotalCount);
        }

        /// <summary>
        /// Adds the record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record">The record.</param>
        /// <param name="target">The target.</param>
        /// <param name="onSuccessWriteToLog">if set to <c>true</c> [on success write to log].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public int AddRecord<T>(T record, Action<T> target, bool onSuccessWriteToLog = false)
            where T : class
        {
            ErrorBase.CheckArgIsNull(record, () => record);
            ErrorBase.CheckArgIsNull(target, () => target);

            int result = -1;
            var entityTypeName = typeof(T).FullName;

            try
            {
                target.Invoke(record);
                var dbResponse = DataSvcClient.SaveChanges();

                // Enumerate the returned responses. 
                foreach (ChangeOperationResponse change in dbResponse)
                {
                    // Get the descriptor for the entity.
                    EntityDescriptor descriptor = change.Descriptor as EntityDescriptor;
                    var addedEntity = descriptor?.Entity as T;
                    if (addedEntity == null)
                    {
                        return (result);
                    }

                    result = GetPrimaryKeyValue(addedEntity);
                    if (onSuccessWriteToLog)
                    {
                        var infoMessage =
                            $"Successfully, added record with ID {result} in {entityTypeName}.";

                        Logger.Information(new CustomFault(infoMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                var custMessage = $"An error occurred while adding the record in {entityTypeName}.";
                throw (new Exception(custMessage, ex));
            }
            finally
            {
                //Hp --> BugFix: The context is already tracking a different entity with the same resource Uri.
                //Reason: This error occurs when we try to add an entity to the Data Service Context more than once. 
                //Fix: We must detach the entity before attaching it again.
                DataSvcClient.Detach(record);
            }

            return result;
        }

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record">The record.</param>
        /// <param name="onSuccessWriteToLog">if set to <c>true</c> [on success write to log].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public int UpdateRecord<T>(T record, bool onSuccessWriteToLog = false) where T : class
        {
            ErrorBase.CheckArgIsNull(record, () => record);

            var result = -1;
            var entityTypeName = typeof(T).FullName;

            try
            {
                //Hp --> Note: Before updating the record the context must track the entity.
                //We can either query the entity and modify the corresponding feilds (or)
                //directly call context AttachTo method.
                //DataSvcClient.AttachTo("BasicDetail", record, "*");
                DataSvcClient.UpdateObject(record);
                var dbResponse = DataSvcClient.SaveChanges();

                // Enumerate the returned responses. 
                foreach (ChangeOperationResponse change in dbResponse)
                {
                    // Get the descriptor for the entity.
                    EntityDescriptor descriptor = change.Descriptor as EntityDescriptor;
                    var updatedEntity = descriptor?.Entity as T;
                    if (updatedEntity == null)
                    {
                        return (result);
                    }

                    result = GetPrimaryKeyValue(updatedEntity);
                    if (onSuccessWriteToLog)
                    {
                        var infoMessage =
                            $"Successfully, updated record with ID {result} in {entityTypeName}.";

                        Logger.Information(new CustomFault(infoMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                var id = GetPrimaryKeyValue(record);
                var custMessage =
                    $"An error occurred while updating the record with ID {id} in {entityTypeName}.";

                throw (new Exception(custMessage, ex));
            }
            finally
            {
                //Hp --> BugFix: The context is already tracking a different entity with the same resource Uri.
                //Reason: This error occurs when we try to add an entity to the Data Service Context more than once. 
                //Fix: We must detach the entity before attaching it again.
                DataSvcClient.Detach(record);
            }

            return (result);
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record">The record.</param>
        /// <param name="onSuccessWriteToLog">if set to <c>true</c> [on success write to log].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public int DeleteRecord<T>(T record, bool onSuccessWriteToLog = false) where T : class
        {
            ErrorBase.CheckArgIsNull(record, () => record);

            var result = -1;
            var entityTypeName = typeof(T).FullName;

            try
            {
                //Hp --> Note: Before deleting the record the context must track the entity.
                //We can either query the entity and modify the corresponding feilds (or)
                //directly call context AttachTo method.
                //DataSvcClient.AttachTo("BasicDetail", record, "*");
                DataSvcClient.DeleteObject(record);
                var dbResponse = DataSvcClient.SaveChanges();

                // Enumerate the returned responses. 
                foreach (ChangeOperationResponse change in dbResponse)
                {
                    // Get the descriptor for the entity.
                    EntityDescriptor descriptor = change.Descriptor as EntityDescriptor;
                    var deletedEntity = descriptor?.Entity as T;
                    if (deletedEntity == null)
                    {
                        return (result);
                    }

                    result = GetPrimaryKeyValue(deletedEntity);
                    if (onSuccessWriteToLog)
                    {
                        var infoMessage =
                            $"Successfully, deleted record with ID {result} in {entityTypeName}.";

                        Logger.Information(new CustomFault(infoMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                var id = GetPrimaryKeyValue(record);
                var custMessage =
                    $"An error occurred while deleting the record with ID {id} in {entityTypeName}.";

                throw (new Exception(custMessage, ex));
            }
            finally
            {
                //Hp --> BugFix: The context is already tracking a different entity with the same resource Uri.
                //Reason: This error occurs when we try to add an entity to the Data Service Context more than once. 
                //Fix: We must detach the entity before attaching it again.
                DataSvcClient.Detach(record);
            }

            return (result);
        }

        /// <summary>
        /// Gets the primary key value dynamically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcEntity">The source entity.</param>
        /// <returns></returns>
        private int GetPrimaryKeyValue<T>(T srcEntity) where T : class
        {
            // Find primary key names based on data service key attribute.
            var myDsKeyAttr = typeof(T).GetCustomAttributes(
                typeof(DataServiceKeyAttribute), true).FirstOrDefault(k =>
                    (0 != ((DataServiceKeyAttribute)k).KeyNames.Count)) as DataServiceKeyAttribute;

            //TODO: Hp --> Needs to implement logic it entity is having multiple Pkeys and datatype
            //other than integer.
            var pKey = myDsKeyAttr.KeyNames.Single();
            return (Utility.ToInt(srcEntity.GetPropertyValue(pKey)));
        }
    }
}
