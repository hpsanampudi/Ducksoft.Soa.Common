using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Ducksoft.SOA.Common.MessageHeaders
{
    /// <summary>
    /// Singleton class, which is used to used to build custom message header.
    /// </summary>
    public sealed class CustomMsgHeaderBuilder
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is 
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<CustomMsgHeaderBuilder> instance =
            new Lazy<CustomMsgHeaderBuilder>(() => new CustomMsgHeaderBuilder());

        /// <summary>
        /// Gets the instance of the singleton object: CustHdrMsgBuilder.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CustomMsgHeaderBuilder Instance
        {
            get { return (instance.Value); }
        }

        /// <summary>
        /// The header information
        /// </summary>
        private CustomMsgHeader headerInfo;

        /// <summary>
        /// Gets the custom header information.
        /// </summary>
        /// <value>
        /// The custom header information.
        /// </value>
        public CustomMsgHeader HeaderInfo
        {
            get
            {
                try
                {
                    headerInfo = OperationContext.Current.IncomingMessageProperties
                    .FirstOrDefault(f => f.Key == "custom-header").Value as CustomMsgHeader;
                }
                catch
                {
                    //Hp --> Logic: Do nothing
                }

                return (headerInfo);
            }
            set
            {
                headerInfo = (null != value) ? new CustomMsgHeader(value) : default(CustomMsgHeader);
            }
        }

        /// <summary>
        /// Gets all custom message headers related information.
        /// </summary>
        /// <value>
        /// All custom message headers related information.
        /// </value>
        public IDictionary<string, object> AllCustomHeaders
        {
            get
            {
                var incomingMsgProperties = OperationContext.Current.IncomingMessageProperties;
                return ((null != incomingMsgProperties) ?
                    (incomingMsgProperties.Where(C => C.Key.StartsWith(
                        "custom-", StringComparison.CurrentCultureIgnoreCase))
                        .ToDictionary(K => K.Key, V => V.Value)) : null);
            }
        }

        /// <summary>
        /// Initializes the <see cref="CustomMsgHeaderBuilder"/> class.
        /// </summary>
        private CustomMsgHeaderBuilder()
        {
            headerInfo = default(CustomMsgHeader);
        }

        /// <summary>
        /// Gets the connection information.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        public DbConnectionInfo GetConnectionInfo(string dbName)
        {
            ErrorBase.CheckArgIsNullOrDefault(dbName, () => dbName);

            //Hp --> Logic: Always take current header information from incomming message header.
            //Also store into local variable to avoid multiple calls on "HeaderInfo" property during
            //validation.
            var myHdrInfo = HeaderInfo;
            return (((null != myHdrInfo) && (null != myHdrInfo.ConnectionInfoList) &&
                (myHdrInfo.ConnectionInfoList.ContainsKey(dbName))) ?
                myHdrInfo.ConnectionInfoList[dbName] : null);
        }
    }
}
