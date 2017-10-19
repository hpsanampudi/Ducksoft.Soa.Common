using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Ducksoft.Soa.Common.Contracts;
using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Infrastructure;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ducksoft.Soa.Logging.EFHelpers
{
    /// <summary>
    /// Static helper class whcih is used to perform SQl database operations.
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="sqlConnectionInfo">The SQL connection information.</param>
        /// <param name="scriptFilePaths">The script file paths.</param>
        /// <param name="filterServerInstances">The filter server instances.</param>
        /// <param name="logger">The logger.</param>
        public static void RunScript(DbConnectionInfo sqlConnectionInfo,
            IList<string> scriptFilePaths, IList<string> filterServerInstances = null,
            ILoggingService logger = null)
        {
            ErrorBase.CheckArgIsNullOrDefault(sqlConnectionInfo, () => sqlConnectionInfo);
            ErrorBase.CheckArgIsNullOrDefault(scriptFilePaths, () => scriptFilePaths);

            var myLogger = logger ?? NInjectHelper.Instance.GetInstance<ILoggingService>();
            try
            {
                if ((string.IsNullOrEmpty(sqlConnectionInfo.ProviderName)) ||
                    (string.IsNullOrEmpty(sqlConnectionInfo.ServerName)) ||
                    (string.IsNullOrEmpty(sqlConnectionInfo.DbName)))
                {
                    myLogger.Error(new CustomFault(
                        string.Format(CultureInfo.CurrentUICulture,
                        "Either ProviderName:{0}, ServerName:{1}, DbName:{2} is null (or) Empty",
                        sqlConnectionInfo.ProviderName, sqlConnectionInfo.ServerName,
                        sqlConnectionInfo.DbName), isNotifyUser: true));

                    return;

                }

                if (null == filterServerInstances)
                {
                    filterServerInstances = new List<string> { "SQLSERVER01" };
                }

                //Hp --> Logic: Check whether we are connecting to production envinorment Database?
                if (filterServerInstances.Contains(sqlConnectionInfo.ServerName))
                {
                    myLogger.Warning(new CustomFault(
                        string.Format(CultureInfo.CurrentUICulture,
                        "User is not allowed to run scripts on production envinorment: {0}",
                        sqlConnectionInfo.ServerName), isNotifyUser: true));

                    return;
                }

                var connectionStr = sqlConnectionInfo.GetSqlConnectionStr();
                myLogger.Information(new CustomFault($"Connecting to {connectionStr}..."));

                scriptFilePaths.Where(filePath =>
                    ((File.Exists(filePath)) && (Path.GetExtension(filePath).IsEqualTo(".sql"))))
                    .ToList().ForEach(file =>
                    {
                        var script = File.ReadAllText(file);
                        using (var conn = new SqlConnection(connectionStr))
                        {
                            var server = new Server(new ServerConnection(conn));
                            server.ConnectionContext.ExecuteNonQuery(script);
                        }
                    });
            }
            catch (Exception ex)
            {
                myLogger.Error(new CustomFault(exception: ex, isNotifyUser: true));
            }
        }
    }
}
