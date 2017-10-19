using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Helpers;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Data.Entity;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Ducksoft.Soa.Common.EFHelpers.Models
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
                    args.Exception = new DataServiceException(400, custMessage);
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