using Ducksoft.Soa.Common.Utilities;
using System.IO;

namespace Ducksoft.Soa.Common.EFHelpers.ConnectedService
{
    /// <summary>
    /// Class which stores connected Odata service related generic methods.
    /// </summary>
    public static class ConnectedServiceHelper
    {
        /// <summary>
        /// Gets the data service URL.
        /// </summary>
        /// <param name="connectSvcPath">The connect SVC path.</param>
        /// <returns>
        /// Uri.
        /// </returns>
        public static string GetDataSvcUrl(string connectSvcPath)
        {
            //Hp --> Logic: Read connected service url from json file.
            var jsonStr = string.Join(string.Empty, File.ReadAllLines(connectSvcPath));
            var jsonObject = Utility.DeserializeFromJson<JsonConnectedService>(jsonStr);
            var dataSvcUrl = jsonObject?.ExtendedData?.Endpoint ?? string.Empty;

            //Hp --> BigFix: The request URI is not valid. The segment '$metadata' must be the last
            //segment in the URI
            return (dataSvcUrl.Replace("$metadata", string.Empty));
        }
    }
}
