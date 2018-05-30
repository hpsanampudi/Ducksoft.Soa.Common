using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ducksoft.SOA.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client using web request.
    /// </summary>
    /// <seealso cref="Ducksoft.SOA.Common.RestClientHelpers.ServiceRestFactory{TClient}" />
    public class ServiceWebRequest : ServiceRestFactory<WebRequest>
    {
        protected override WebRequest DefaultClient
        {
            get
            {
                //Hp --> Note: Always ignore user provided value since As we need to create 
                //new instance on each request call.
                return (default(WebRequest));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceWebRequest" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public ServiceWebRequest(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace, ServiceAuthTypes authType)
            : base(svcBaseUrl, restMsgFormat, defaultNamespace, authType)
        {
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="webMethod">The web method.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        protected WebRequest InitializeClient(string contractOrApiPath, string webMethod = "GET",
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes))
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNullOrDefault(webMethod, () => webMethod);

            var client = ConfigureClient();
            if (client == null)
            {
                //Hp --> Logic: Always create new instance on each request.
                client = WebRequest.Create($"{SvcBaseUrl}{contractOrApiPath}");
                client.Method = webMethod;
                switch (RestMsgFormat)
                {
                    case WebMessageFormat.Xml:
                        {
                            client.ContentType = "application/xml; charset=utf-8";
                        }
                        break;

                    case WebMessageFormat.Json:
                        {
                            client.ContentType = "application/json; charset=utf-8";
                        }
                        break;

                    default:
                        {
                            var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                            throw (new ExceptionBase(errMessage));
                        }
                }
            }

            if (AuthType == ServiceAuthTypes.OAuth2)
            {
                var tokenResponse = GetOAuth2Token(dateFormat, strategyType);
                if (!tokenResponse?.IsError ?? false)
                {
                    client.Headers.Add(HttpRequestHeader.Authorization,
                        $"{tokenResponse.TokenType}{tokenResponse.AccessToken}");
                }
            }

            return (client);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <param name="dateFormat">The date format.</param>
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
                var client = InitializeClient(contractOrApiPath, dateFormat: dateFormat,
                    strategyType: strategyType);

                var response = client.GetResponse() as HttpWebResponse;
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType);
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
                var client = InitializeClient(contractOrApiPath, dateFormat: dateFormat,
                    strategyType: strategyType);

                var response = await GetResponseAsync(client, dateFormat, strategyType, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType, cancelToken);
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
                var client = InitializeClient(contractOrApiPath, dateFormat: dateFormat,
                    strategyType: strategyType);

                var response = client.GetResponse() as HttpWebResponse;
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<List<TResponse>>(rawData, dateFormat, strategyType);
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
                var client = InitializeClient(contractOrApiPath, dateFormat: dateFormat,
                    strategyType: strategyType);

                var response = await GetResponseAsync(client, dateFormat, strategyType, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<List<TResponse>>(
                    rawData, dateFormat, strategyType, cancelToken);
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
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);

                //Hp --> Note: As we are not passing request object set content length as zero.
                client.ContentLength = 0;

                var response = client.GetResponse() as HttpWebResponse;
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
        public override async Task PostDataAsync(string contractOrApiPath,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            bool isIgnoreError = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var cancelToken = CancelTokenSource.Token;

            try
            {
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);

                //Hp --> Note: As we are not passing request object set content length as zero.
                client.ContentLength = 0;

                var response = await GetResponseAsync(client, dateFormat, strategyType, cancelToken);
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
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);

                //Hp --> Note: As we are not passing request object set content length as zero.
                client.ContentLength = 0;

                var response = client.GetResponse() as HttpWebResponse;
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType);
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
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);

                //Hp --> Note: As we are not passing request object set content length as zero.
                client.ContentLength = 0;

                var response = await GetResponseAsync(client, dateFormat, strategyType, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType, cancelToken);
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
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                //Hp --> Note: HttpWebRequest is not serializing the request properly.
                //So, instead of using HttpWebRequest serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace, dateFormat,
                    strategyType);

                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);

                client.ContentLength = Encoding.UTF8.GetByteCount(rawRequestData);
                using (Stream stream = client.GetRequestStream())
                {
                    stream.Write(requestInBytes, 0, (int)client.ContentLength);
                }

                var response = client.GetResponse() as HttpWebResponse;
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType);
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
                var client = InitializeClient(contractOrApiPath, "POST", dateFormat, strategyType);
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                //Hp --> Note: HttpWebRequest is not serializing the request properly.
                //So, instead of using HttpWebRequest serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace, dateFormat,
                    strategyType, cancelToken);

                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);

                client.ContentLength = Encoding.UTF8.GetByteCount(rawRequestData);
                using (Stream stream = client.GetRequestStream())
                {
                    stream.Write(requestInBytes, 0, (int)client.ContentLength);
                }

                var response = await GetResponseAsync(client, dateFormat, strategyType, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

                //Hp --> Note: HttpWebResponse is not deserializing the response properly.
                //using (var respStream = new MemoryStream(response))
                //{
                //    var serializer = GetSerializer<TResponse>();
                //    result = (TResponse)serializer.ReadObject(respStream);
                //}
                //So, instead of using HttpWebResponse deserializing mechanism use rest sharp deserializer.
                var rawData = string.Empty;
                using (var respStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(respStream, Encoding.UTF8);
                    rawData = reader.ReadToEnd();
                }

                result = DeserializeFrom<TResponse>(rawData, dateFormat, strategyType, cancelToken);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ExceptionBase(errMessage));
            }

            return (result);
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual XmlObjectSerializer GetXmlSerializer<TResponse>(
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes))
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
        /// <param name="client">The client.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <param name="strategyType">Type of the strategy.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        protected async Task<HttpWebResponse> GetResponseAsync(WebRequest client,
            string dateFormat = null, JsonStrategyTypes strategyType = default(JsonStrategyTypes),
            CancellationToken cancelToken = default(CancellationToken))
        {
            CheckForCancellationRequest(cancelToken);
            if (null == client)
            {
                var errMessage = $"[Method]: {nameof(GetResponseAsync)}" +
                    $"{Environment.NewLine}[Reason]: {nameof(client)} instance is null!";

                throw (new NullReferenceException(errMessage));
            }

            using (cancelToken.Register(client.Abort))
            {
                return (await client.GetResponseAsync() as HttpWebResponse);
            }
        }

        /// <summary>
        /// Ensures the valid response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        protected virtual void EnsureValidResponse(HttpWebResponse response,
            CancellationToken cancelToken = default(CancellationToken))
        {
            CheckForCancellationRequest(cancelToken);
            if (HttpStatusCode.OK != response.StatusCode)
            {
                var statusCodeTitle = nameof(response.StatusCode).SplitCamelCase();
                var statusDescTitle = nameof(response.StatusDescription).SplitCamelCase();
                var errMessage = $"{statusCodeTitle}: {response.StatusCode}" +
                    $"{Environment.NewLine}{statusDescTitle}: {response.StatusDescription}";

                throw (new ExceptionBase(errMessage));
            }
        }
    }
}
