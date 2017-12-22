using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Utilities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client using Rest sharp.
    /// </summary>
    /// <seealso cref="Ducksoft.Soa.Common.RestClientHelpers.ServiceRestFactory{TClient}" />
    public class ServiceRSharpClient : ServiceRestFactory<RestClient>
    {
        /// <summary>
        /// Gets the default client.
        /// </summary>
        /// <value>
        /// The default client.
        /// </value>
        protected override RestClient DefaultClient
        {
            get
            {
                return (new RestClient(SvcBaseUrl));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRSharpClient" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public ServiceRSharpClient(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace, ServiceAuthTypes authType)
            : base(svcBaseUrl, restMsgFormat, defaultNamespace, authType)
        {
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <returns></returns>
        protected RestClient InitializeClient() => ConfigureClient();

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse GetData<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET, dateFormat,
                    strategyType);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var client = InitializeClient();
                var response = client.Execute(request);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET, dateFormat,
                    strategyType);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var client = InitializeClient();
                var response = await client.ExecuteTaskAsync(request, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET, dateFormat,
                    strategyType);

                var client = InitializeClient();
                var response = client.Execute<List<TResponse>>(request);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                result = response.Data;
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the data list asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<List<TResponse>> GetDataListAsync<TResponse>(
            string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes), bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET, dateFormat,
                    strategyType);

                var client = InitializeClient();
                var response = await client.ExecuteTaskAsync<List<TResponse>>(request, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                result = response.Data;
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <exception cref="ExceptionBase"></exception>
        public override void PostData(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes), bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var client = InitializeClient();
                var response = client.Execute(request);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }
        }

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task PostDataAsync(string contractOrApiPath, string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes), bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                var client = InitializeClient();
                var response = await client.ExecuteTaskAsync(request, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                await Utility.CompletedTask;
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }
        }

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var client = InitializeClient();
                var response = client.Execute(request);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                var client = InitializeClient();
                var response = await client.ExecuteTaskAsync(request, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

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
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", string dateFormat = null,
            JsonStrategyTypes strategyType = default(JsonStrategyTypes), bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);
            var result = default(TResponse);

            try
            {
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                request.AddBody(requestObject, requestObjNamespace);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var client = InitializeClient();
                var response = client.Execute(request);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

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
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TRequest, TResponse>(
            string contractOrApiPath, TRequest requestObject, string requestObjNamespace = "",
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);
            var result = default(TResponse);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST, dateFormat,
                    strategyType);

                request.AddBody(requestObject, requestObjNamespace);

                var client = InitializeClient();
                var response = await client.ExecuteTaskAsync(request, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                result = DeserializeFrom<TResponse>(response, dateFormat, strategyType, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Ensures the valid response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual void EnsureValidResponse(IRestResponse response,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(response, () => response);

            CheckForCancellationRequest(cancelToken);
            if ((HttpStatusCode.OK != response.StatusCode) || (null != response.ErrorException))
            {
                var errMessage =
                $"{response.StatusDescription}{Environment.NewLine}{response.Content}";

                throw (new ExceptionBase(errMessage, response.ErrorException));
            }
        }

        /// <summary>
        /// Gets the rest data format.
        /// </summary>
        /// <value>
        /// The rest data format.
        /// </value>
        /// <exception cref="ExceptionBase"></exception>
        protected DataFormat RestDataFormat
        {
            get
            {
                var requestFormat = default(DataFormat);
                switch (RestMsgFormat)
                {
                    case WebMessageFormat.Xml:
                        {
                            requestFormat = DataFormat.Xml;
                        }
                        break;

                    case WebMessageFormat.Json:
                        {
                            requestFormat = DataFormat.Json;
                        }
                        break;

                    default:
                        {
                            var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                            throw (new ExceptionBase(errMessage));
                        }
                }

                return (requestFormat);
            }
        }

        /// <summary>
        /// Gets the default rest request.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="webMethodType">Type of the web method.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <returns></returns>
        protected IRestRequest GetDefaultRestRequest(string contractOrApiPath, Method webMethodType,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes))
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            var request = new RestRequest(contractOrApiPath, webMethodType);
            request.RequestFormat = RestDataFormat;

            //Hp --> Logic: If request date format is empty then reset to null.
            request.DateFormat = string.IsNullOrWhiteSpace(dateFormat) ? null : dateFormat;

            //Hp --> Logic: Update request related serializer (Json/Xml) properties.
            ConfigureRSharpSerializer(request, dateFormat, strategyType);

            if (AuthType == ServiceAuthTypes.OAuth2)
            {
                var tokenResponse = GetOAuth2Token(dateFormat, strategyType);
                if (!tokenResponse?.IsError ?? false)
                {
                    request.AddHeader("cache-control", "no-cache");
                    request.AddParameter("authorization",
                        $"{tokenResponse.TokenType} {tokenResponse.AccessToken}",
                        ParameterType.HttpHeader);
                }
            }

            return (request);
        }
    }
}
