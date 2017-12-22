using Ducksoft.Soa.Common.DataContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Interface which is used to create WCF rest factory instance.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IServiceRestFactory : IDisposable
    {
        /// <summary>
        /// Gets the web request and response message format.
        /// </summary>
        /// <value>
        /// The web request and response message format.
        /// </value>
        WebMessageFormat RestMsgFormat { get; }

        /// <summary>
        /// Gets the service base URL.
        /// </summary>
        /// <value>
        /// The service base URL.
        /// </value>
        string SvcBaseUrl { get; }

        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        /// <value>
        /// The default namespace.
        /// </value>
        string DefaultNamespace { get; }

        /// <summary>
        /// Gets the type of the authentication.
        /// </summary>
        /// <value>
        /// The type of the authentication.
        /// </value>
        ServiceAuthTypes AuthType { get; }

        /// <summary>
        /// Occurs when [on raise OAuth2 token request].
        /// </summary>
        event Func<OAuth2TokenRequest> OnRaiseOAuth2TokenRequest;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        TResponse GetData<TResponse>(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        List<TResponse> GetDataList<TResponse>(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data list asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        Task<List<TResponse>> GetDataListAsync<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        void PostData(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        Task PostDataAsync(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        TResponse PostData<TResponse>(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        TResponse PostData<TRequest, TResponse>(string contractOrApiPath, TRequest requestObject,
            string requestObjNamespace = "", string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        Task<TResponse> PostDataAsync<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false);
    }
}