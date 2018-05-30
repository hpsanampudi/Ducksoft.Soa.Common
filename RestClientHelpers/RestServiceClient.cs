using Ducksoft.SOA.Common.DataContracts;
using System;
using System.ServiceModel.Web;

namespace Ducksoft.SOA.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client.
    /// </summary>
    /// <typeparam name="TClient">The type of the client.</typeparam>
    public abstract class RestServiceClient<TClient> where TClient : class
    {
        /// <summary>
        /// The rest factory
        /// </summary>
        protected readonly ServiceRestFactory<TClient> RestFactory;

        /// <summary>
        /// Occurs when [register client].
        /// </summary>
        public event Func<TClient> RegisterClient;

        /// <summary>
        /// Occurs when [register OAuth2 token request].
        /// </summary>
        public event Func<OAuth2TokenRequest> RegisterOAuth2TokenRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceClient{TClient}" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The rest message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public RestServiceClient(string svcBaseUrl,
            WebMessageFormat restMsgFormat = WebMessageFormat.Json, string defaultNamespace = "",
            ServiceAuthTypes authType = ServiceAuthTypes.None)
        {
            RestFactory = ServiceRestFactory<TClient>.Create(
                svcBaseUrl, restMsgFormat, defaultNamespace, authType);

            RestFactory.OnRaiseOAuth2TokenRequest += () => RegisterOAuth2TokenRequest?.Invoke();
            RestFactory.OnInitClient += () => RegisterClient?.Invoke();
        }

        #region Interface: IDisposable implementation
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (null != RestFactory)
            {
                RestFactory.Dispose();
            }
        }
        #endregion
    }
}
