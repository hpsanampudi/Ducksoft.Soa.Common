using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Utilities;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Abstract class which is used to create WCF rest service factory instance.
    /// </summary>
    /// <typeparam name="TClient">The type of the client.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.RestClientHelpers.IServiceRestFactory" />
    public abstract class ServiceRestFactory<TClient> : IServiceRestFactory where TClient : class
    {
        /// <summary>
        /// Gets the web request and response message format.
        /// </summary>
        /// <value>
        /// The web request and response message format.
        /// </value>
        public WebMessageFormat RestMsgFormat { get; private set; }

        /// <summary>
        /// Gets the service base URL.
        /// </summary>
        /// <value>
        /// The service base URL.
        /// </value>
        public string SvcBaseUrl { get; private set; }

        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        /// <value>
        /// The default namespace.
        /// </value>
        public string DefaultNamespace { get; private set; }

        /// <summary>
        /// Gets the type of the authentication.
        /// </summary>
        /// <value>
        /// The type of the authentication.
        /// </value>
        public ServiceAuthTypes AuthType { get; private set; }

        /// <summary>
        /// Occurs when [on raise OAuth2 token request].
        /// </summary>
        public event Func<OAuth2TokenRequest> OnRaiseOAuth2TokenRequest;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        public readonly CancellationTokenSource CancelTokenSource;

        /// <summary>
        /// The register cancel token
        /// </summary>
        protected CancellationTokenRegistration registerCancelToken;

        /// <summary>
        /// Gets the current OAuth2 token.
        /// </summary>
        /// <value>
        /// The current OAuth2 token.
        /// </value>
        protected OAuth2TokenResponse CurrentOAuth2Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRestFactory{TClient}" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public ServiceRestFactory(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace, ServiceAuthTypes authType)
        {
            SvcBaseUrl = svcBaseUrl;
            RestMsgFormat = restMsgFormat;
            DefaultNamespace = defaultNamespace;
            AuthType = authType;

            CancelTokenSource = new CancellationTokenSource();
            var cancelToken = CancelTokenSource.Token;
            registerCancelToken = cancelToken.Register(() =>
            {
                Debug.WriteLine($"User canceled the asyncronous task");
                //cancelToken.ThrowIfCancellationRequested();
            });
        }

        /// <summary>
        /// Creates the specified service base URL.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public static ServiceRestFactory<TClient> Create(string svcBaseUrl,
            WebMessageFormat restMsgFormat = WebMessageFormat.Json,
            string defaultNamespace = "", ServiceAuthTypes authType = ServiceAuthTypes.None)
        {
            var restFactory = default(ServiceRestFactory<TClient>);
            var clientType = typeof(TClient);
            if (typeof(RestClient) == clientType)
            {
                restFactory = new ServiceRSharpClient(svcBaseUrl, restMsgFormat,
                    defaultNamespace, authType) as ServiceRestFactory<TClient>;
            }
            else if (typeof(HttpClient) == clientType)
            {
                restFactory = new ServiceHttpClient(svcBaseUrl, restMsgFormat,
                    defaultNamespace, authType) as ServiceRestFactory<TClient>;
            }
            else if (typeof(WebClient) == clientType)
            {
                restFactory = new ServiceWebClient(svcBaseUrl, restMsgFormat,
                    defaultNamespace, authType) as ServiceRestFactory<TClient>;
            }
            else if (typeof(WebRequest) == clientType)
            {
                restFactory = new ServiceWebRequest(svcBaseUrl, restMsgFormat,
                    defaultNamespace, authType) as ServiceRestFactory<TClient>;
            }
            else
            {
                var errMessage = $"The given rest client type {clientType} is not handled!";
                throw (new ExceptionBase(errMessage));
            }

            return (restFactory);
        }

        /// <summary>
        /// Gets the default client.
        /// </summary>
        /// <value>
        /// The default client.
        /// </value>
        protected abstract TClient DefaultClient { get; }

        /// <summary>
        /// Occurs when [on initialize client].
        /// </summary>
        public event Func<TClient> OnInitClient;

        /// <summary>
        /// Configures the client.
        /// </summary>
        /// <returns></returns>
        protected virtual TClient ConfigureClient()
        {
            var handler = OnInitClient;
            return (handler?.Invoke() ?? DefaultClient);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract TResponse GetData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract List<TResponse> GetDataList<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Gets the data list asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract Task<List<TResponse>> GetDataListAsync<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        public abstract void PostData(string contractOrApiPath, bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract Task PostDataAsync(string contractOrApiPath, bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract TResponse PostData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", bool isIgnoreError = false);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public abstract Task<TResponse> PostDataAsync<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", bool isIgnoreError = false);

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

            if (null != registerCancelToken)
            {
                registerCancelToken.Dispose();
            }

            if (null != CancelTokenSource)
            {
                CancelTokenSource.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// Gets the OAuth2 token.
        /// </summary>
        /// <param name="isDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual OAuth2TokenResponse GetOAuth2Token(bool isDisplayError = true)
        {
            if (AuthType != ServiceAuthTypes.OAuth2)
            {
                var errMessage = $"Invalid token authorization type: \"{nameof(AuthType)}\"";
                throw (new ExceptionBase(errMessage));
            }

            var handler = OnRaiseOAuth2TokenRequest;
            var tokenRequest = handler?.Invoke();
            if (tokenRequest == null)
            {
                var errMessage = $"Object {nameof(tokenRequest)} instance is null!";
                throw (new ExceptionBase(errMessage));
            }

            if ((tokenRequest.Equals(CurrentOAuth2Token?.TokenRequest)) &&
                (!CurrentOAuth2Token?.IsTokenExpired ?? false))
            {
                return (CurrentOAuth2Token);
            }

            var client = new RestClient(tokenRequest.TokenUrl);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!tokenRequest.Headers.ContainsKey("content-type"))
            {
                tokenRequest.Headers.Add("content-type", "application/x-www-form-urlencoded");
            }

            foreach (var item in tokenRequest.Headers)
            {
                request.AddHeader(item.Key, item.Value);
            }

            var contentType = tokenRequest.Headers["content-type"];
            var bodyParmeters = string.Join("&",
                tokenRequest.BodyParameters.Select(item => $"{item.Key}={item.Value}"));

            request.AddParameter(contentType, bodyParmeters, ParameterType.RequestBody);

            var response = client.Execute(request);
            if ((isDisplayError) &&
                ((response.StatusCode != HttpStatusCode.OK) || (response.ErrorException != null)))
            {
                var errMessage =
                        $"{response.StatusDescription}{Environment.NewLine}{response.Content}";

                throw (new ExceptionBase(errMessage, response.ErrorException));
            }

            var tokenResponse = DeserializeFrom<OAuth2TokenResponse>(response);
            tokenResponse.StatusCode = response.StatusCode;
            tokenResponse.Raw = response.Content;
            tokenResponse.TokenRequest = tokenRequest;
            CurrentOAuth2Token = tokenResponse;

            return (tokenResponse);
        }

        /// <summary>
        /// Gets the json date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        protected long ToJsonDate(string dateTime)
        {
            ErrorBase.CheckArgIsNullOrDefault(dateTime, () => dateTime);

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = DateTime.Parse(dateTime);
            return ((currentDate - unixEpoch).Ticks / TimeSpan.TicksPerMillisecond);
        }

        /// <summary>
        /// Deserializes from given json/xml string.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="jsonOrXmlStr">The json or XML string.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected virtual TResponse DeserializeFrom<TResponse>(string jsonOrXmlStr,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNullOrDefault(jsonOrXmlStr, () => jsonOrXmlStr);
            return (DeserializeFrom<TResponse>(
                new RestResponse { Content = jsonOrXmlStr }, cancelToken));
        }

        /// <summary>
        /// Deserializes from given rest response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected TResponse DeserializeFrom<TResponse>(IRestResponse response,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(response, () => response);

            var result = default(TResponse);
            var deserializer = default(IDeserializer);
            if (response.Content.IsJson())
            {
                deserializer = new JsonDeserializer();
            }
            else
            {
                //Hp --> Note: For rest sharp Deserialize<T> method, T should be new() instance type.
                //so, for string data type which is immutable consider DotNetXmlDeserializer.
                if (typeof(TResponse) == typeof(string))
                {
                    deserializer = new DotNetXmlDeserializer();
                }
                else
                {
                    deserializer = new XmlDeserializer();
                }
            }

            CheckForCancellationRequest(cancelToken);
            result = deserializer.Deserialize<TResponse>(response);
            return (result);
        }

        /// <summary>
        /// Serializes to xml (or) json string.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase">
        /// </exception>
        protected string SerializeTo<TRequest>(TRequest requestObject,
            string requestObjNamespace = "",
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);

            var rawData = string.Empty;
            var serializer = default(ISerializer);
            switch (RestMsgFormat)
            {
                case WebMessageFormat.Xml:
                    {
                        serializer = new XmlSerializer();
                        serializer.ContentType = "application/xml; charset=utf-8";
                    }
                    break;

                case WebMessageFormat.Json:
                    {
                        serializer = new JsonSerializer();
                        serializer.ContentType = "application/json; charset=utf-8";
                    }
                    break;

                default:
                    {
                        var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                        throw (new ExceptionBase(errMessage));
                    }
            }


            //Hp --> Note: For serialization we need to set date format explicity.
            serializer.DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            serializer.Namespace = (requestObjNamespace ?? string.Empty).Trim();

            CheckForCancellationRequest(cancelToken);
            rawData = serializer.Serialize(requestObject);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                var errMessage = $"[Method]: {nameof(SerializeTo)}" +
                    $"{Environment.NewLine}[Reason]: Failed to serialize {nameof(requestObject)}!";

                throw (new ExceptionBase(errMessage));
            }

            return (rawData);
        }

        /// <summary>
        /// Checks for cancellation request.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        protected void CheckForCancellationRequest(CancellationToken cancelToken)
        {
            if ((null == cancelToken) || (default(CancellationToken) == cancelToken))
            {
                return;
            }

            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
        }
    }
}
