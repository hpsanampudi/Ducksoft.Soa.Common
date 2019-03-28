using Ducksoft.SOA.Common.Contracts;
using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Helpers;
using Ducksoft.SOA.Common.Utilities;
using System;
using System.Data.Entity;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Ducksoft.SOA.Common.EFHelpers.Models
{
    /// <summary>
    /// Base class which is used to create entity framework model object through WCF data services.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JSONPSupportBehavior]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    public class EFDataServiceBase<T> : EntityFrameworkDataService<T> where T : DbContext
    {
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <value>
        /// The database connection string.
        /// </value>
        public string DbConnectionStr { get; private set; }

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFDataServiceBase{T}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EFDataServiceBase(ILoggingService logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Initializes the service.
        /// Note: This method is called only once to initialize service-wide policies.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            //Hp --> Allow full access rights on all entity sets
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetEntitySetPageSize("*", 1000);
            config.DataServiceBehavior.AcceptAnyAllRequests = true;
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.DataServiceBehavior.AcceptReplaceFunctionInQuery = true;
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;

            // Hp --> To report more information in the error response messages for troubleshooting
            config.UseVerboseErrors = true;
        }

        /// <summary>
        /// Creates the data source.
        /// </summary>
        /// <returns></returns>
        protected override T CreateDataSource()
        {
            if (WebOperationContext.Current == null) return (null);

            T dataSource;
            var headers = WebOperationContext.Current.IncomingRequest.Headers;
            var hdrCollection = headers.AllKeys.ToDictionary(key => key, key => headers.Get(key));

            var connectionInfo = new DbConnectionInfo(hdrCollection);
            var efConnection = connectionInfo?.GetEFConnectionStr<T>();
            if (!string.IsNullOrWhiteSpace(efConnection))
            {
                //Hp --> Logic: Redirect the EF database connection to user provided value.
                DbConnectionStr = efConnection;
                dataSource = (T)Activator.CreateInstance(typeof(T), new object[] { efConnection });
            }
            else
            {
                dataSource = base.CreateDataSource();
                DbConnectionStr = dataSource.Database.Connection.ConnectionString;
            }

            //Hp --> Note: The default timeout for executing any SQL command is 30 seconds.
            //But sometimes in QTAC application, it will take more time than default.
            //So, increase command timeout to 5 minutes.
            if (null != dataSource)
            {
                dataSource.Database.CommandTimeout = (5 * 60); //Value in Seconds
            }

            return (dataSource);
        }
        
        /// <summary>
        /// Called when [before save changes].
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <exception cref="DataServiceException"></exception>
        protected void OnBeforeSaveChanges(object entity, UpdateOperations operationType,
            string entitySetName = "")
        {
            ErrorBase.CheckArgIsNull(entity, nameof(entity));

            switch (operationType)
            {
                case UpdateOperations.Change:
                    {
                        //Hp --> Logic: Updates only the modified feild values of given entity in database.
                        var dbEntityEntry = CurrentDataSource.Entry(entity);

                        //Hp --> Logic: Check whether given entity is modified (or) not?
                        //If not then just return.
                        if (dbEntityEntry.State != EntityState.Modified)
                        {
                            return;
                        }

                        //Hp --> Logic: Filter only modified field values.
                        foreach (string propName in dbEntityEntry.OriginalValues.PropertyNames)
                        {
                            var dbPropEntry = dbEntityEntry.Property(propName);
                            var originalValue = dbPropEntry.OriginalValue;
                            var modifiedValue = dbPropEntry.CurrentValue;

                            //Hp --> Logic: Check whether the property value is same as orginal value.
                            //If Yes, then reset the modified status as false otherwise true.
                            if (dbPropEntry.IsModified && (originalValue?.Equals(modifiedValue) ??
                                modifiedValue?.Equals(originalValue) ?? true))
                            {
                                dbPropEntry.IsModified = false;
                            }
                        }
                    }
                    break;

                case UpdateOperations.Delete:
                    {
                        if (!entity.IsHavingAuditColumns(AuditColumnTypes.Delete))
                        {
                            //Hp --> Logic: User is not allowed to delete database record permanently.
                            //Instead update the feild DeleteBy and DeleteDate.
                            var statusCode = (int)HttpStatusCode.BadRequest;
                            var errMessage = $"User is not allowed to delete {entitySetName} " +
                                $"database record permanently.";

                            throw new DataServiceException(statusCode, errMessage);
                        }

                        //TODO: Hp --> How to get name of the user who perform this action?
                        //Hp --> Logic: Update deleted date timestamp.
                        entity.SetPropertyValue("DeleteDate", DateTime.Now);
                    }
                    break;

                case UpdateOperations.None:
                case UpdateOperations.Add:
                    {
                        //Hp --> Do nothing, trigger default behaviour
                    }
                    break;
            }
        }        

        /// <summary>
        /// Called when [start processing request].
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            ErrorBase.CheckArgIsNull(args, () => args);
            //var identity = HttpContext.Current.User.Identity;
            //if ((null == identity) || (!identity.IsAuthenticated))
            //{
            //    throw new DataServiceException(401, "Unauthorized");
            //}

            base.OnStartProcessingRequest(args);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void HandleException(HandleExceptionArgs args)
        {
            ErrorBase.CheckArgIsNull(args, () => args);

            //Hp --> Logic: It will be more usefull when we write inner exception message also to
            //listners since it holds the actual error message when any database query fails.
            var custMessage = string.Join(Environment.NewLine, args.Exception.Messages());

            // Handle exceptions raised in service operations. 
            if ((args.Exception is TargetInvocationException)
                && (null != args.Exception.InnerException))
            {
                var innerException = args.Exception.InnerException;
                if (innerException is DataServiceException)
                {
                    // Unpack the DataServiceException.
                    args.Exception = new DataServiceException(custMessage, innerException);
                }
                else
                {
                    // Return a new DataServiceException as "400: bad request."
                    var statusCode = (int)HttpStatusCode.BadRequest;
                    args.Exception = new DataServiceException(statusCode, custMessage);
                }
            }
            else
            {
                //Hp --> Note: Always raise an data service exception.
                args.Exception = new DataServiceException(custMessage, args.Exception);
            }

            logger.Error(new CustomFault(exception: args.Exception));
            base.HandleException(args);
        }
    }
}
