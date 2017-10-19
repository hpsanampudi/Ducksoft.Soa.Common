using Ducksoft.Soa.Common.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which stores custom header related information passed during WCF request.
    /// </summary>
    [DataContract(Name = "CustomMsgHeader",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class CustomMsgHeader
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        /// <value>
        /// The name of the domain.
        /// </value>
        [DataMember]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        /// <value>
        /// The name of the machine.
        /// </value>
        [DataMember]
        public int MachineName { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        [DataMember]
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [DataMember]
        public int ClientId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the connection string information.
        /// </summary>
        /// <value>
        /// The connection string information.
        /// </value>
        [DataMember]
        public IDictionary<string, DbConnectionInfo> ConnectionInfoList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMsgHeader"/> class.
        /// </summary>
        public CustomMsgHeader()
        {
        }

        /// <summary>
        /// Copy constructor: Initializes a new instance of the <see cref="CustomMsgHeader"/> class.
        /// </summary>
        /// <param name="custMsgHdrInfo">The customer MSG HDR information.</param>
        public CustomMsgHeader(CustomMsgHeader custMsgHdrInfo)
        {
            ErrorBase.CheckArgIsNull(custMsgHdrInfo, () => custMsgHdrInfo);
            AppName = custMsgHdrInfo.AppName;
            ClientId = custMsgHdrInfo.ClientId;
            CompanyId = custMsgHdrInfo.CompanyId;
            DomainName = custMsgHdrInfo.DomainName;
            MachineName = custMsgHdrInfo.MachineName;
            UserId = custMsgHdrInfo.UserId;
            UserName = custMsgHdrInfo.UserName;

            //Hp --> Logic: Perform deep copy
            ConnectionInfoList = (null == custMsgHdrInfo.ConnectionInfoList) ? null :
                custMsgHdrInfo.ConnectionInfoList.ToDictionary(src => src.Key, src => src.Value);
        }
    }
}
