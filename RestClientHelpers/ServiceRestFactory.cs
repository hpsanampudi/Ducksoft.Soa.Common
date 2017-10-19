using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// The token source
        /// </summary>
        public readonly CancellationTokenSource TokenSource;

        /// <summary>
        /// The register cancel token
        /// </summary>
        protected CancellationTokenRegistration registerCancelToken;

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected abstract TClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRestFactory{TClient}" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        public ServiceRestFactory(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace)
        {
            SvcBaseUrl = svcBaseUrl;
            RestMsgFormat = restMsgFormat;
            DefaultNamespace = defaultNamespace;

            TokenSource = new CancellationTokenSource();
            var cancelToken = TokenSource.Token;
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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public static ServiceRestFactory<TClient> Create(string svcBaseUrl,
            WebMessageFormat restMsgFormat = WebMessageFormat.Json,
            string defaultNamespace = "")
        {
            var restFactory = default(ServiceRestFactory<TClient>);
            var clientType = typeof(TClient);
            if (typeof(RestClient) == clientType)
            {
                restFactory = new ServiceRSharpClient(svcBaseUrl, restMsgFormat, defaultNamespace)
                    as ServiceRestFactory<TClient>;
            }
            else if (typeof(HttpClient) == clientType)
            {
                restFactory = new ServiceHttpClient(svcBaseUrl, restMsgFormat, defaultNamespace)
                    as ServiceRestFactory<TClient>;
            }
            else if (typeof(WebClient) == clientType)
            {
                restFactory = new ServiceWebClient(svcBaseUrl, restMsgFormat, defaultNamespace)
                    as ServiceRestFactory<TClient>;
            }
            else if (typeof(WebRequest) == clientType)
            {
                restFactory = new ServiceWebRequest(svcBaseUrl, restMsgFormat, defaultNamespace)
                    as ServiceRestFactory<TClient>;
            }
            else
            {
                var errMessage = $"The given rest client type {clientType} is not handled!";
                throw (new ExceptionBase(errMessage));
            }

            return (restFactory);
        }

        /// <summary>
        /// Configures the client settings.
        /// </summary>
        /// <param name="client">The client.</param>
        public abstract void ConfigureClient(TClient client = null);

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <returns></returns>
        public abstract TResponse GetData<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract List<TResponse> GetDataList<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Gets the data list asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract Task<List<TResponse>> GetDataListAsync<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        public abstract void PostData(string contractOrApiPath);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract Task PostDataAsync(string contractOrApiPath);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract TResponse PostData<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>
        public abstract Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath);

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <returns></returns>
        public abstract TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "");

        /// <summary>
        /// Posts the data asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="requestObject">The request object.</param>
        /// <param name="requestObjNamespace">The request object namespace.</param>
        /// <returns></returns>
        public abstract Task<TResponse> PostDataAsync<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "");

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

            if (null != TokenSource)
            {
                TokenSource.Dispose();
            }
        }
        #endregion

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
