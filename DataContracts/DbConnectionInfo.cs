using Ducksoft.Soa.Common.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which stores database connection string related information.
    /// </summary>
    [DataContract(Name = "DbConnectionInfo",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class DbConnectionInfo
    {
        /// <summary>
        /// The provider name
        /// </summary>
        private string providerName;

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        [DataMember]
        public string ProviderName
        {
            get { return (providerName); }
            set
            {
                providerName =
                    string.IsNullOrWhiteSpace(value) ? "System.Data.SqlClient" : value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        [DataMember]
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        [DataMember]
        public string DbName { get; set; }

        /// <summary>
        /// Gets or sets the name of the edmx model.
        /// </summary>
        /// <value>
        /// The name of the edmx model.
        /// </value>
        [DataMember]
        public string EdmxModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the redirect connect string.
        /// </summary>
        /// <value>
        /// The name of the redirect connect string.
        /// </value>
        [DataMember]
        public string ConnectStrName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionInfo" /> class.
        /// </summary>
        public DbConnectionInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionInfo"/> class.
        /// </summary>
        /// <param name="custHdrCollection">The customer header collection.</param>
        public DbConnectionInfo(IDictionary<string, string> custHdrCollection)
        {
            if (null == custHdrCollection) return;

            var result = custHdrCollection.Keys.Intersect(ToDictonary().Keys)
                .ToDictionary(k => k, k => custHdrCollection[k]);

            foreach (var item in result)
            {
                if (nameof(ProviderName) == item.Key)
                {
                    ProviderName = item.Value;
                }
                else if (nameof(ServerName) == item.Key)
                {
                    ServerName = item.Value;
                }
                else if (nameof(DbName) == item.Key)
                {
                    DbName = item.Value;
                }
                else if (nameof(EdmxModelName) == item.Key)
                {
                    EdmxModelName = item.Value;
                }
                else if (nameof(ConnectStrName) == item.Key)
                {
                    ConnectStrName = item.Value;
                }
            }
        }

        /// <summary>
        /// To the dictonary.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> ToDictonary()
        {
            return (new Dictionary<string, string>
            {
                { nameof(ProviderName), ProviderName },
                { nameof(ServerName), ServerName },
                { nameof(DbName), DbName },
                { nameof(EdmxModelName), EdmxModelName },
                { nameof(ConnectStrName), ConnectStrName }
            });
        }

        /// <summary>
        /// Gets the SQL connection string.
        /// </summary>
        /// <returns></returns>
        public string GetSqlConnectionStr()
        {
            var sqlConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectStrName))
            {
                sqlConnection =
                    ConfigurationManager.ConnectionStrings[ConnectStrName].ConnectionString;

                return (sqlConnection);
            }

            //Hp --> Logic: Skip RedirectConnectStr from dictonary and check it has valid data?
            var isInvalid = ToDictonary()
                .Where(I => !I.Key.IsEqualTo(nameof(ConnectStrName)))
                .Any(I => string.IsNullOrWhiteSpace(I.Value));

            if (isInvalid)
            {
                sqlConnection = string.Empty;
            }
            else
            {
                new SqlConnectionStringBuilder()
                {
                    DataSource = ServerName,
                    InitialCatalog = DbName,
                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true
                }.ToString();
            }

            return (sqlConnection);
        }

        /// <summary>
        /// Gets the EF connection string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetEFConnectionStr<T>() where T : DbContext
        {
            var efConnection = string.Empty;
            var sqlConnection = GetSqlConnectionStr();
            if (string.IsNullOrWhiteSpace(sqlConnection))
            {
                return (efConnection);
            }

            if (!string.IsNullOrWhiteSpace(ConnectStrName))
            {
                efConnection = new EntityConnectionStringBuilder(sqlConnection).ToString();
            }
            else
            {
                var entityType = typeof(T);
                var assemblyFullName = entityType.Assembly.GetName().FullName;

                var metaDataStr = (string.IsNullOrWhiteSpace(EdmxModelName)) ?
                    @"res://*" :
                    string.Format(@"res://{0}/{1}.csdl| res://{0}/{1}.ssdl| res://{0}/{1}.msl",
                    assemblyFullName, EdmxModelName);

                efConnection = new EntityConnectionStringBuilder
                {
                    Provider = ProviderName,
                    ProviderConnectionString = sqlConnection,
                    Metadata = metaDataStr
                }
                .ToString();
            }

            return (efConnection);
        }
    }
}
