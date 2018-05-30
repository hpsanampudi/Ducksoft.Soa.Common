using Ducksoft.SOA.Common.RestClientConverters;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Ducksoft.SOA.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to configure custom web http behavior under WCF client config file.
    /// </summary>
    /// <seealso cref="System.ServiceModel.Description.WebHttpBehavior" />
    public class CustomWebHttpBehavior : WebHttpBehavior
    {
        /// <summary>
        /// Gets the query string converter.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <returns></returns>
        protected override QueryStringConverter GetQueryStringConverter(
            OperationDescription operationDescription)
        {
            return (new CustomQueryStringConverter());
        }
    }
}
