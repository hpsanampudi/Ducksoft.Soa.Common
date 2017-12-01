using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client using web client.
    /// </summary>
    /// <seealso cref="Ducksoft.Soa.Common.RestClientHelpers.ServiceRestFactory{TClient}" />
    public class ServiceWebClient : ServiceRestFactory<WebClient>
    {
        /// <summary>
        /// Gets the default client.
        /// </summary>
        /// <value>
        /// The default client.
        /// </value>
        /// <exception cref="ExceptionBase"></exception>
        protected override WebClient DefaultClient
        {
            get
            {
                var client = new WebClient
                {
                    BaseAddress = SvcBaseUrl,
                    Encoding = Encoding.UTF8
                };

                var contentType = string.Empty;
                switch (RestMsgFormat)
                {
                    case WebMessageFormat.Xml:
                        {
                            contentType = "application/xml";
                        }
                        break;

                    case WebMessageFormat.Json:
                        {
                            contentType = "application/json";
                        }
                        break;

                    default:
                        {
                            var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                            throw (new ExceptionBase(errMessage));
                        }
                }

                client.Headers[HttpRequestHeader.ContentType] = contentType;
                return (client);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceWebClient" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public ServiceWebClient(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace, ServiceAuthTypes authType)
            : base(svcBaseUrl, restMsgFormat, defaultNamespace, authType)
        {
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <returns></returns>
        protected WebClient InitializeClient()
        {
            var client = ConfigureClient();
            if (client == null)
            {
                return (client);
            }

            if (AuthType == ServiceAuthTypes.OAuth2)
            {
                var tokenResponse = GetOAuth2Token();
                if (!tokenResponse?.IsError ?? false)
                {
                    client.Headers[HttpRequestHeader.Authorization] =
                        $"{tokenResponse.TokenType} {tokenResponse.AccessToken}";
                }
            }

            return (client);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse GetData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                var client = InitializeClient();
                var response = client.DownloadData(contractOrApiPath);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData);
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var response = await DownloadDataTaskAsync(contractOrApiPath, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the list data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();

            try
            {
                var client = InitializeClient();
                var response = client.DownloadData(contractOrApiPath);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<JsonResultBase<TResponse>>();
                //    var x = serializer.ReadObject(respStream) as JsonResultBase<TResponse>;
                //    result = x.Result.MyContents;
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<List<TResponse>>(rawData);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the list data asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<List<TResponse>> GetDataListAsync<TResponse>(
            string contractOrApiPath, bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var response = await DownloadDataTaskAsync(contractOrApiPath, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<JsonResultBase<TResponse>>();
                //    var x = serializer.ReadObject(respStream) as JsonResultBase<TResponse>;
                //    result = x.Result.MyContents;
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<List<TResponse>>(rawData, cancelToken);
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <exception cref="ExceptionBase"></exception>
        public override void PostData(string contractOrApiPath, bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            try
            {
                //Hp --> Note: As we are not passing request object set with new instance.
                var client = InitializeClient();
                var response = client.UploadData(contractOrApiPath, "POST", new byte[] { });
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task PostDataAsync(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var response =
                    await UploadDataTaskAsync(contractOrApiPath, cancelToken: cancelToken);

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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                //Hp --> Note: As we are not passing request object set with new instance.
                var client = InitializeClient();
                var response = client.UploadData(contractOrApiPath, "POST", new byte[] { });
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData);
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var response =
                    await UploadDataTaskAsync(contractOrApiPath, cancelToken: cancelToken);

                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData, cancelToken);
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);
            var result = default(TResponse);

            try
            {
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                //Hp --> Note: WebClient is not serializing the request properly.
                //So, instead of using WebClient serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace);
                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);

                var client = InitializeClient();
                var response = client.UploadData(contractOrApiPath, "POST", requestInBytes);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData);
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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TRequest, TResponse>(
            string contractOrApiPath, TRequest requestObject, string requestObjNamespace = "",
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

                //Hp --> Note: WebClient is not serializing the request properly.
                //So, instead of using WebClient serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace, cancelToken);
                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);
                var response = await UploadDataTaskAsync(contractOrApiPath,
                    requestObjInBytes: requestInBytes, cancelToken: cancelToken);

                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: WebClient is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using WebClient deserializing mechanism use rest sharp deserializer.
                var rawData = Encoding.UTF8.GetString(response);
                result = DeserializeFrom<TResponse>(rawData, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual XmlObjectSerializer GetSerializer<TResponse>()
        {
            var serializer = default(XmlObjectSerializer);

            switch (RestMsgFormat)
            {
                case WebMessageFormat.Json:
                    {
                        serializer = new DataContractJsonSerializer(typeof(TResponse));
                    }
                    break;

                case WebMessageFormat.Xml:
                    {
                        serializer = new DataContractSerializer(typeof(TResponse));
                    }
                    break;

                default:
                    {
                        var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                        throw (new ExceptionBase(errMessage));
                    }
            }

            return (serializer);
        }

        /// <summary>
        /// Downloads the data task asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected async Task<byte[]> DownloadDataTaskAsync(string contractOrApiPath,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            CheckForCancellationRequest(cancelToken);

            var client = InitializeClient();
            using (cancelToken.Register(client.CancelAsync))
            {
                return (await client.DownloadDataTaskAsync(contractOrApiPath));
            }
        }

        /// <summary>
        /// Uploads the data task asynchronous.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="webMethod">The web method.</param>
        /// <param name="requestObjInBytes">The request object bytes.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected async Task<byte[]> UploadDataTaskAsync(string contractOrApiPath,
            string webMethod = "POST", byte[] requestObjInBytes = default(byte[]),
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNullOrDefault(webMethod, () => webMethod);

            CheckForCancellationRequest(cancelToken);

            //Hp --> Logic: As we are not passing request object set with new instance.
            if (null == requestObjInBytes)
            {
                requestObjInBytes = new byte[] { };
            }

            var client = InitializeClient();
            using (cancelToken.Register(client.CancelAsync))
            {
                return (await client.UploadDataTaskAsync(
                    contractOrApiPath, webMethod, requestObjInBytes));
            }
        }

        /// <summary>
        /// Ensures the valid response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        protected virtual void EnsureValidResponse(byte[] response,
            CancellationToken cancelToken = default(CancellationToken))
        {
            CheckForCancellationRequest(cancelToken);
            if (null == response)
            {
                var errMessage = $"[Method]: {nameof(EnsureValidResponse)}" +
                    $"{Environment.NewLine}[Reason]: {nameof(response)} object is null";
                throw (new NullReferenceException(errMessage));
            }
        }
    }
}
