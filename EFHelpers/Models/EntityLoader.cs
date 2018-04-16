using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.EFHelpers.Interfaces;
using Ducksoft.Soa.Common.EFHelpers.ODataHelpers;
using Ducksoft.Soa.Common.Filters;
using Ducksoft.Soa.Common.Utilities;
using Nelibur.ObjectMapper;
using Nelibur.ObjectMapper.Bindings;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading;
using System.Web;

namespace Ducksoft.Soa.Common.EFHelpers.Models
{
    /// <summary>
    /// Class which is used to load Entities with user provided connection string information.
    /// </summary>
    /// <typeparam name="TEntities">The type of the entities.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.EFHelpers.Interfaces.IEntityLoader{TEntities}" />
    public class EntityLoader<TEntities> : IEntityLoader<TEntities>
        where TEntities : DataServiceContext
    {
        /// <summary>
        /// Gets the data service client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public virtual TEntities DataSvcClient { get; private set; }

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
        /// Initializes a new instance of the <see cref="EntityLoader{TEntities}"/> class.
        /// </summary>
        public EntityLoader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLoader{TEntities}"/> class.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        public EntityLoader(Uri dataServiceUrl)
            : this(dataServiceUrl, null)
        {
            //Hp --> Logic: If connectionInfo is null then it takes value mentioned inside 
            //corrsponding web.config file.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLoader{TEntities}" /> class.
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
            var client = new DataServiceContext(dataServiceUrl) as TEntities;

            //Hp --> Logic: If TEntities is derived from DataServiceContext then we need to use 
            //reflection to initialize the client object.
            if (null == client)
            {
                DataSvcClient = (TEntities)Activator.CreateInstance(typeof(TEntities),
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
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}">
        /// </exception>
        protected virtual IEnumerable<QueryOperationResponse<TEntity>> GetAll<TEntity>(
            IDataServiceQuery<TEntity> query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            var errMessage = string.Empty;
            var fault = default(CustomFault);
            var response = default(QueryOperationResponse<TEntity>);

            try
            {
                response = query?.Execute() as QueryOperationResponse<TEntity>;
            }
            catch (Exception ex)
            {
                fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }

            // Hp --> Logic: In order to get all records we need to loop untill data service 
            //continuation token is null.
            DataServiceQueryContinuation<TEntity> token = null;
            do
            {
                try
                {
                    if ((cancelToken != default(CancellationToken)) &&
                        (cancelToken.IsCancellationRequested))
                    {
                        cancelToken.ThrowIfCancellationRequested();
                    }

                    // If nextLink is not null, then there is a new page to load. 
                    if (token != null)
                    {
                        // Load the new page from the next link URI.
                        response = DataSvcClient.Execute(token);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    errMessage = $"User canceled while reading the " +
                        $"{typeof(TEntity).FullName} records.";

                    fault = new CustomFault(errMessage, ex);
                    break;
                }
                catch (Exception ex)
                {
                    //Hp --> Logic: It can be DataServiceClientException (or) any error.
                    errMessage = $"An error occurred while reading the " +
                        $"{typeof(TEntity).FullName} records.";

                    fault = new CustomFault(errMessage, ex);
                    break;
                }

                //Hp --> Logic: To improve the performance yield the return value.
                if (response != null)
                {
                    yield return (response);
                }

            } while ((token = response?.GetContinuation()) != null);

            //Hp --> Logic: If any fault occurs then throw fault exception.
            if (fault != null)
            {
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAllRecords<TEntity>(
            IDataServiceQuery<TEntity> query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class => GetAll(query, cancelToken).SelectMany(R => R);

        /// <summary>
        /// Gets all records while calling a store procedure returning complex data type
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query url calling store procedure.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}"></exception>
        public virtual IQueryable<TEntity> GetAll<TEntity>(string query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            ErrorBase.CheckArgIsNullOrDefault(query, () => query);
            var response = default(IQueryable<TEntity>);

            try
            {
                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                var queryUri = new Uri(query, UriKind.RelativeOrAbsolute);
                //Hp --> BugFix: Cannot materialize a collection of a primitives or complex 
                //without the type 'T' being a collection.
                //Comments: When calling a store procedure returning complex data type then pass
                //other parameters of Execute method as shown below:
                response = DataSvcClient.Execute<TEntity>(queryUri, "GET", false).AsQueryable();
            }
            catch (OperationCanceledException ex)
            {
                var errMessage = $"User canceled while reading the " +
                    $"{typeof(TEntity).FullName} records.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var errMessage = $"An error occurred while reading the " +
                    $"{typeof(TEntity).FullName} records.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }

            return (response);
        }

        /// <summary>
        /// Gets the single or default.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}"></exception>
        public virtual TEntity GetSingleOrDefault<TEntity>(IDataServiceQuery<TEntity> query,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            ErrorBase.CheckArgIsNullOrDefault(query, () => query);

            var result = default(TEntity);
            try
            {
                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                result = query.Execute().SingleOrDefault();
            }
            catch (OperationCanceledException ex)
            {
                var errMessage = $"User canceled while reading the " +
                    $"{typeof(TEntity).FullName} record.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var errMessage = $"An error occurred while reading the " +
                    $"{typeof(TEntity).FullName} record.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }

            return (result);
        }

        /// <summary>
        /// Executes the OData query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> ExecuteODataQuery<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, IList<QueryOption> queryOptions = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class =>
            GetAllRecords(LoadQueryOptions(baseQuery, queryOptions, isAddOrAppendDeleteFilter),
                cancelToken);

        /// <summary>
        /// Loads the query options.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public virtual IDataServiceQuery<TEntity> LoadQueryOptions<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, IList<QueryOption> queryOptions = null,
            bool isAddOrAppendDeleteFilter = true)
            where TEntity : class
        {
            var query = baseQuery;
            var requestUrl = baseQuery.RequestUri;
            var urlQuery = HttpUtility.ParseQueryString(requestUrl.Query);

            var urlQueryOptions = urlQuery.AllKeys.ToList();
            queryOptions = queryOptions?.ToList() ?? new List<QueryOption>();
            if (urlQueryOptions.Any(U => queryOptions.Any(Q => Q.Option.IsEqualTo(U))))
            {
                var errorMessage =
                    $"Base query url already contains one (or) more user provided query option key!";

                throw (new ExceptionBase(errorMessage));
            }

            if (isAddOrAppendDeleteFilter)
            {
                var comparer = Utility.GetEqualityComparer<string>();
                var isHavingUrlFilter = urlQuery.AllKeys.Contains("$filter", comparer);
                if (isHavingUrlFilter)
                {
                    var urlFilterExpr = urlQuery["$filter"];
                    var deleteExpression = "DeleteDate eq null";
                    var strComparer = StringComparison.CurrentCultureIgnoreCase;
                    if (!urlFilterExpr.Contains(deleteExpression, strComparer))
                    {
                        query = AddOrAppendDeleteFilter<TEntity>(requestUrl);
                    }
                }
                else
                {
                    //Hp --> Logic: If delete filter does not exists then create.
                    queryOptions = AddOrAppendDeleteFilter(queryOptions);
                }
            }

            queryOptions.ToList().ForEach(item =>
            {
                query = query.AddQueryOption(item.Option, item.Query);
            });

            return (query);
        }

        /// <summary>
        /// Loads the filter query option.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="odataFilterExpression">The odata filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <returns></returns>
        public virtual IDataServiceQuery<TEntity> LoadFilterQueryOption<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, string odataFilterExpression = "",
            bool isAddOrAppendDeleteFilter = true)
            where TEntity : class
        {
            var queryOptions = new List<QueryOption>
            {
                new QueryOption
                {
                    Option = "$filter",
                    Query = odataFilterExpression?.Trim() ?? string.Empty
                }
            };

            return (LoadQueryOptions(baseQuery, queryOptions, isAddOrAppendDeleteFilter));
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public PaginationData<TEntity> GetPaginationData<TEntity>(
            IDataServiceQuery<TEntity> baseQuery,
            int? pageIndex = default(int?), int? pageSize = default(int?),
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            if (baseQuery == null)
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

            return (GetPaginationData(baseQuery, qryOptions, isAddOrAppendDeleteFilter,
                cancelToken));
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        public PaginationData<TEntity> GetPaginationData<TEntity>(
            IDataServiceQuery<TEntity> baseQuery, IList<QueryOption> queryOptions = null,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            if (baseQuery == null)
            {
                return (null);
            }

            var queryToExecute = LoadQueryOptions(
                baseQuery, queryOptions, isAddOrAppendDeleteFilter);

            return (new PaginationData<TEntity>
            {
                TotalItems = GetTotalRecordsCount(
                    queryToExecute, isAddOrAppendDeleteFilter, cancelToken),

                PageData = GetAllRecords(queryToExecute, cancelToken).ToList()
            });
        }

        /// <summary>
        /// Gets the total records count.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseQuery">The base query.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}">
        /// </exception>
        public long GetTotalRecordsCount<TEntity>(IDataServiceQuery<TEntity> baseQuery,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
        {
            long totalRecords = -1;

            try
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
                    }, isAddOrAppendDeleteFilter);
                }

                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                var response = baseQuery.Execute() as QueryOperationResponse<TEntity>;
                totalRecords = response.TotalCount;
            }
            catch (OperationCanceledException ex)
            {
                var errMessage = $"User canceled while reading the " +
                    $"{typeof(TEntity).FullName} total records.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var errMessage = $"An error occurred while reading the " +
                    $"{typeof(TEntity).FullName} total records.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }

            return (totalRecords);
        }

        /// <summary>
        /// Adds the record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}"></exception>
        public virtual TPKey AddRecord<TEntity, TPKey>(TEntity record,
            Func<TEntity, TPKey> primaryKey = null,
            TPKey defaultValue = default(TPKey),
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct
        {
            ErrorBase.CheckArgIsNull(record, () => record);

            var result = defaultValue;
            Func<TEntity, TPKey> GetPrimaryKeyValue = (R) =>
            primaryKey?.Invoke(R) ?? GetPrimaryKeyInfo(R, defaultValue).Value;

            try
            {
                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                var entitySetName = GetEntitySetName<TEntity>();
                DataSvcClient.AddObject(entitySetName, record);
                var dbResponse = DataSvcClient.SaveChanges();

                // Enumerate the returned responses. 
                foreach (ChangeOperationResponse change in dbResponse)
                {
                    // Get the descriptor for the entity.
                    EntityDescriptor descriptor = change.Descriptor as EntityDescriptor;
                    var addedEntity = (TEntity)descriptor?.Entity ?? null;
                    if (addedEntity == null)
                    {
                        continue;
                    }

                    result = GetPrimaryKeyValue(addedEntity);
                    break;
                }
            }
            catch (OperationCanceledException ex)
            {
                var errMessage = $"User canceled while adding the " +
                    $"{typeof(TEntity).FullName} record.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var errMessage = $"An error occurred while adding the " +
                    $"{typeof(TEntity).FullName} record.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            finally
            {
                DataSvcClient.Detach(record);
            }

            return (result);
        }

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <exception cref="FaultException{CustomFault}">
        /// </exception>
        public virtual TPKey UpdateRecord<TEntity, TPKey>(TEntity record,
            Func<TEntity, TPKey> primaryKey = null, TPKey defaultValue = default(TPKey),
            bool isTracked = false, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct
        {
            ErrorBase.CheckArgIsNull(record, () => record);

            Func<TEntity, TPKey> GetPrimaryKeyValue = (R) =>
            primaryKey?.Invoke(R) ?? GetPrimaryKeyInfo(R, defaultValue).Value;

            var result = defaultValue;
            var dbRecord = default(TEntity);

            try
            {
                // Hp --> Logic: The Context is not track user passed record entity, so we need 
                //to get the latest database record based on primary key and map the user supplied values.
                if (!isTracked)
                {
                    var odataFilter = GetODataPKExpression(record);
                    var baseQuery = CreateBaseQuery<TEntity>();
                    var query = LoadFilterQueryOption(
                        baseQuery, odataFilter, isAddOrAppendDeleteFilter);

                    dbRecord = GetSingleOrDefault(query, cancelToken);
                    if (dbRecord == null)
                    {
                        var errMessage =
                            $"Failed to get database record with PK filter: {odataFilter}";

                        throw (new ExceptionBase(errMessage));
                    }

                    //Hp --> Logic: Map the source record values into database record.
                    TinyMapper.Bind(GetIgnoreConfig<TEntity, TEntity>());
                    dbRecord = TinyMapper.Map(record, dbRecord);
                }
                else
                {
                    dbRecord = record;
                }

                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                //Hp --> Note: Before updating the record the context must track the entity.
                //We can either query the entity and modify the corresponding feilds (or)
                //directly call context AttachTo method.
                //DataSvcClient.AttachTo("BasicDetail", record, "*");
                DataSvcClient.UpdateObject(dbRecord);
                var dbResponse = DataSvcClient.SaveChanges();

                // Enumerate the returned responses. 
                foreach (ChangeOperationResponse change in dbResponse)
                {
                    // Get the descriptor for the entity.
                    EntityDescriptor descriptor = change.Descriptor as EntityDescriptor;
                    var updatedEntity = (TEntity)descriptor?.Entity ?? null;
                    if (updatedEntity == null)
                    {
                        continue;
                    }

                    result = GetPrimaryKeyValue(updatedEntity);
                    break;
                }
            }
            catch (OperationCanceledException ex)
            {
                var pKey = GetPrimaryKeyValue(record);
                var errMessage = $"User canceled while updating the {typeof(TEntity).FullName} " +
                    $"record \"{pKey}\".";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var pKey = GetPrimaryKeyValue(record);
                var errMessage = $"An error occurred while updating the " +
                    $"{typeof(TEntity).FullName} record \"{pKey}\".";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            finally
            {
                if (dbRecord != null)
                {
                    DataSvcClient.Detach(dbRecord);
                }
            }

            return (result);
        }

        /// <summary>
        /// Purges (or) deletes the record permanently.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="record">The record.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="isTracked">if set to <c>true</c> [is tracked entity].</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <exception cref="FaultException{CustomFault}">
        /// </exception>
        public virtual bool PurgeRecord<TEntity, TPKey>(TEntity record,
            Func<TEntity, TPKey> primaryKey = null,
            bool isTracked = false, bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct
        {
            ErrorBase.CheckArgIsNull(record, () => record);

            Func<TEntity, TPKey> GetPrimaryKeyValue = (R) =>
            primaryKey?.Invoke(R) ?? GetPrimaryKeyInfo<TEntity, TPKey>(R).Value;

            var isSuccess = false;
            var dbRecord = default(TEntity);

            try
            {
                // Hp --> Logic: The Context is not track user passed record entity, so we need 
                //to get the latest database record based on primary key and map the user supplied values.
                if (!isTracked)
                {
                    var odataFilter = GetODataPKExpression(record);
                    var baseQuery = CreateBaseQuery<TEntity>();
                    var query = LoadFilterQueryOption(
                        baseQuery, odataFilter, isAddOrAppendDeleteFilter);

                    dbRecord = GetSingleOrDefault(query, cancelToken);
                    if (dbRecord == null)
                    {
                        var errMessage =
                            $"Failed to get database record with PK filter: {odataFilter}";

                        throw (new ExceptionBase(errMessage));
                    }

                    //Hp --> Logic: Map the source record values into database record.
                    TinyMapper.Bind(GetIgnoreConfig<TEntity, TEntity>());
                    dbRecord = TinyMapper.Map(record, dbRecord);
                }
                else
                {
                    dbRecord = record;
                }

                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                //Hp --> Note: Before updating the record the context must track the entity.
                //We can either query the entity and modify the corresponding feilds (or)
                //directly call context AttachTo method.
                //DataSvcClient.AttachTo("BasicDetail", record, "*");
                DataSvcClient.DeleteObject(record);
                DataSvcClient.SaveChanges();
                isSuccess = true;
            }
            catch (OperationCanceledException ex)
            {
                var pKey = GetPrimaryKeyValue(record);
                var errMessage = $"User canceled while purging the " +
                    $"{typeof(TEntity).FullName} record \"{pKey}\".";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var pKey = GetPrimaryKeyValue(record);
                var errMessage = $"An error occurred while purging the " +
                    $"{typeof(TEntity).FullName} record \"{pKey}\".";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            finally
            {
                if (dbRecord != null)
                {
                    DataSvcClient.Detach(dbRecord);
                }
            }

            return (isSuccess);
        }

        /// <summary>
        /// Purges (or) deletes the record permanently.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPKey">The type of the primary key.</typeparam>
        /// <param name="odataFilterExpression">The OData filter expression.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="FaultException{CustomFault}">
        /// </exception>
        public virtual bool PurgeRecord<TEntity, TPKey>(string odataFilterExpression,
            bool isAddOrAppendDeleteFilter = true,
            CancellationToken cancelToken = default(CancellationToken))
            where TEntity : class
            where TPKey : struct
        {
            ErrorBase.CheckArgIsNullOrDefault(odataFilterExpression, () => odataFilterExpression);

            var result = false;
            try
            {
                var baseQuery = CreateBaseQuery<TEntity>();
                var query = LoadFilterQueryOption(
                    baseQuery, odataFilterExpression, isAddOrAppendDeleteFilter);

                if ((cancelToken != default(CancellationToken)) &&
                    (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                var record = query.Execute().SingleOrDefault();
                result = PurgeRecord<TEntity, TPKey>(record, null, true, isAddOrAppendDeleteFilter,
                    cancelToken);
            }
            catch (OperationCanceledException ex)
            {
                var errMessage = $"User canceled while purging the {typeof(TEntity).FullName} " +
                    $"record with filter {odataFilterExpression}.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }
            catch (Exception ex)
            {
                //Hp --> Logic: It can be DataServiceClientException (or) any error.
                var errMessage = $"An error occurred while purging the " +
                    $"{typeof(TEntity).FullName} record with filter {odataFilterExpression}.";

                var fault = new CustomFault(errMessage, ex);
                throw (new FaultException<CustomFault>(fault, fault.Reason));
            }

            return (result);
        }

        /// <summary>
        /// Gets the primary key information.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="srcEntity">The source entity.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected KeyValuePair<string, TResult> GetPrimaryKeyInfo<TEntity, TResult>(
            TEntity srcEntity, TResult defaultValue = default(TResult))
            where TEntity : class
            where TResult : struct
        {
            // Find primary key names based on data service key attribute.
            var myDsKeyAttr = typeof(TEntity).GetCustomAttributes(
                typeof(DataServiceKeyAttribute), true).FirstOrDefault(k =>
                    (((DataServiceKeyAttribute)k).KeyNames.Count != 0)) as DataServiceKeyAttribute;

            //TODO: Hp --> Needs to implement logic it entity is having multiple Pkeys.
            var pKey = myDsKeyAttr.KeyNames.FirstOrDefault();
            var pKeyValue = (pKey != null) ?
                (TResult)srcEntity.GetPropertyValue(pKey) : defaultValue;

            return (new KeyValuePair<string, TResult>(pKey, pKeyValue));
        }

        /// <summary>
        /// Gets the OData primary key expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="srcEntity">The source entity.</param>
        /// <returns></returns>
        public virtual string GetODataPKExpression<TEntity>(TEntity srcEntity)
            where TEntity : class
        {
            //Hp --> Logic: Find primary key names based on data service key attribute.
            var myDsKeyAttr = typeof(TEntity).GetCustomAttributes(
                typeof(DataServiceKeyAttribute), true).FirstOrDefault(k =>
                    (0 != ((DataServiceKeyAttribute)k).KeyNames.Count)) as DataServiceKeyAttribute;

            var group = new FilterGroup();
            group.OperatorType = FilterLogicalOperatorTypes.And;
            group.Filters = myDsKeyAttr.KeyNames
                .Select(pk => new FilterBuilder
                {
                    PropertyName = pk,
                    OperatorType = FilterCompareOperatorTypes.EqualTo,
                    Value = srcEntity.GetPropertyValue(pk)
                })
                .ToList();

            return (group.ToString());
        }

        /// <summary>
        /// Gets the primary key based predicate expression dynamically.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="srcEntity">The source entity.</param>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, bool>> GetPKExpression<TEntity>(
            TEntity srcEntity)
            where TEntity : class
        {
            //Hp --> Logic: Find primary key names based on data service key attribute.
            var myDsKeyAttr = typeof(TEntity).GetCustomAttributes(
                typeof(DataServiceKeyAttribute), true).FirstOrDefault(k =>
                    (0 != ((DataServiceKeyAttribute)k).KeyNames.Count)) as DataServiceKeyAttribute;

            //Hp --> Logic: Create entity => portion of lambda expression
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression body = null;

            foreach (var key in myDsKeyAttr.KeyNames)
            {
                //Hp --> Logic: Create entity.Id portion of lambda expression
                var property = Expression.Property(parameter, key);

                //Hp --> Logic: Create 'id' portion of lambda expression
                var id = srcEntity.GetType().GetProperty(key).GetValue(srcEntity, null);
                var equalsTo = Expression.Constant(id);

                //Hp --> Logic: Create entity.Id == 'id' portion of lambda expression
                var equality = Expression.Equal(property, equalsTo);

                //TODO: Hp --> Needs to check whether below line of code works for multiple Pkeys.
                body = (null != body) ? Expression.AndAlso(body, equality) : equality;
            }

            //Hp --> Logic: Finally create entire expression: entity => entity.Id == 'id'
            var result = Expression.Lambda<Func<TEntity, bool>>(body, new[] { parameter });
            return (result);
        }

        /// <summary>
        /// Adds or append delete filter.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        protected IList<QueryOption> AddOrAppendDeleteFilter(IList<QueryOption> queryOptions)
        {
            var myQueryOptions = queryOptions ?? new List<QueryOption>();
            var filterOption = "$filter";
            var deleteExpression = "DeleteDate eq null";
            var filter = myQueryOptions.SingleOrDefault(Q => Q.Option.IsEqualTo(filterOption));
            if (filter == null)
            {
                myQueryOptions.Add(new QueryOption
                {
                    Option = filterOption,
                    Query = $"({deleteExpression})"
                });
            }
            else
            {
                var comparer = StringComparison.CurrentCultureIgnoreCase;
                if (!filter.Query.Contains(deleteExpression, comparer))
                {
                    myQueryOptions.Remove(filter);
                    filter.Query += $" and ({deleteExpression})";
                    myQueryOptions.Add(filter);
                }
            }

            return (myQueryOptions);
        }

        /// <summary>
        /// Adds the or append delete filter.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="isAddOrAppendDeleteFilter">if set to <c>true</c> [is add or append delete filter].</param>
        /// <returns></returns>
        protected IDataServiceQuery<TEntity> AddOrAppendDeleteFilter<TEntity>(Uri requestUrl,
            bool isAddOrAppendDeleteFilter = true)
            where TEntity : class
        {
            ErrorBase.CheckArgIsNull(requestUrl, nameof(requestUrl));
            var deleteExpression = (isAddOrAppendDeleteFilter) ?
                "DeleteDate eq null" : string.Empty;

            //Hp --> Logic: The last segment in request url is entity set name.
            var entitySetName = string.Empty;
            var entitySegmentName = requestUrl.Segments.LastOrDefault();
            if (string.IsNullOrWhiteSpace(entitySegmentName))
            {
                entitySetName = GetEntitySetName<TEntity>();
            }
            else
            {
                entitySetName = entitySegmentName.TrimEnd(')').TrimEnd('(');
            }

            //Hp --> Logic: As we can edit the request url filter query parameter,
            //create new base query and copy all odata query options from request url
            //by editing the filter expression.
            var query = CreateBaseQuery<TEntity>(entitySetName);
            Func<string, string, bool> IsAppend = (source, target) =>
            {
                var isAppend = true;
                var strComparer = StringComparison.CurrentCultureIgnoreCase;
                if ((string.IsNullOrWhiteSpace(target)) || (source.Contains(target, strComparer)))
                {
                    isAppend = false;
                    return (isAppend);
                }

                return (isAppend);
            };

            var filterOption = "$filter";
            var urlQuery = HttpUtility.ParseQueryString(requestUrl.Query);
            foreach (var key in urlQuery.AllKeys)
            {
                var myValue = urlQuery[key];
                if (key.IsEqualTo(filterOption))
                {
                    if (IsAppend(myValue, deleteExpression))
                    {
                        myValue += $" and ({deleteExpression})";
                    }
                }

                query = query.AddQueryOption(key, myValue);
            }

            //Hp --> If there is no filter in request iel query then add user supplied values.
            var comparer = Utility.GetEqualityComparer<string>();
            var isHavingUrlFilter = urlQuery.AllKeys.Contains(filterOption, comparer);
            if ((!isHavingUrlFilter) && (!string.IsNullOrWhiteSpace(deleteExpression)))
            {
                var filterValue = $"({deleteExpression})";
                query = query.AddQueryOption(filterOption, filterValue);
            }

            return (query);
        }


        /// <summary>
        /// Gets the name of the entity set.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="isPluralize">if set to <c>true</c> [is pluralize].</param>
        /// <returns></returns>
        protected string GetEntitySetName<TEntity>(bool isPluralize = false)
            where TEntity : class
        {
            var entitySetName = string.Empty;
            if (!isPluralize)
            {
                var entitySetAttr = typeof(TEntity).GetClassAttribute<EntitySetAttribute>();
                entitySetName = entitySetAttr?.EntitySet ?? string.Empty;
            }
            else
            {
                //TODO: Hp --> Entity framework mismatch version error is comming need to check.
                //Hp --> Logic: To get name of the entity set in pluralization format.
                //var service =
                //    DbConfiguration.DependencyResolver.GetService<IPluralizationService>();

                //entitySetName = service.Pluralize(typeof(T).Name);
            }

            return (entitySetName);
        }

        /// <summary>
        /// Creates the base query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> CreateBaseQuery<TEntity>(string entitySetName = "")
            where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(entitySetName))
            {
                entitySetName = GetEntitySetName<TEntity>();
            }

            IQueryable<TEntity> baseQuery = DataSvcClient.CreateQuery<TEntity>(entitySetName);
            return (baseQuery.Wrap());
        }

        /// <summary>
        /// Wraps the data service query to support mocking.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IDataServiceQuery<TEntity> WrapQuery<TEntity>(IQueryable<TEntity> query)
            where TEntity : class => query.Wrap();

        /// <summary>
        /// Gets the ignore configuration.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="predefinedProperties">The predefined properties.</param>
        /// <returns></returns>
        protected Action<IBindingConfig<TSource, TTarget>> GetIgnoreConfig<TSource, TTarget>(
            IEnumerable<string> predefinedProperties = null) => (config) =>
            {
                predefinedProperties = predefinedProperties ?? new List<string>();
                foreach (var propName in predefinedProperties)
                {
                    config.Ignore(Utility.GetExpression<TSource>(propName));
                }

                var complexProperties = Utility.GetAllComplexProperties<TSource>().Select(P => P.Name);
                foreach (var propName in complexProperties)
                {
                    config.Ignore(Utility.GetExpression<TSource>(propName));
                }
            };
    }
}
