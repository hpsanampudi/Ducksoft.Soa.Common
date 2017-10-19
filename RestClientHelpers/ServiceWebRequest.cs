using Ducksoft.Soa.Common.Utilities;
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

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client using web request.
    /// </summary>
    /// <seealso cref="Ducksoft.Soa.Common.RestClientHelpers.ServiceRestFactory{TClient}" />
    public class ServiceWebRequest : ServiceRestFactory<WebRequest>
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected override WebRequest Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceWebRequest" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        public ServiceWebRequest(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace) : base(svcBaseUrl, restMsgFormat, defaultNamespace)
        {
            ConfigureClient();
        }

        /// <summary>
        /// Configures the client settings.
        /// </summary>
        /// <param name="client">The client.</param>        
        public override void ConfigureClient(WebRequest client = null)
        {
            //Hp --> Note: Always ignore user provided value since As we need to create 
            //new instance on each request call.
            Client = null;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse GetData<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                InitializeClient(contractOrApiPath);
                var response = Client.GetResponse() as HttpWebResponse;
                EnsureValidResponse(response);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> GetDataAsync<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = TokenSource.Token;

            try
            {
                InitializeClient(contractOrApiPath);
                var response = await GetResponseAsync(cancelToken);
                EnsureValidResponse(response, cancelToken);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();

            try
            {
                InitializeClient(contractOrApiPath);
                var response = Client.GetResponse() as HttpWebResponse;
                EnsureValidResponse(response);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<List<TResponse>> GetDataListAsync<TResponse>(
            string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();
            var cancelToken = TokenSource.Token;

            try
            {
                InitializeClient(contractOrApiPath);
                var response = await GetResponseAsync(cancelToken);
                EnsureValidResponse(response, cancelToken);

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
        /// <exception cref="ExceptionBase"></exception>
        public override void PostData(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            try
            {
                InitializeClient(contractOrApiPath, "POST");

                //Hp --> Note: As we are not passing request object set content length as zero.
                Client.ContentLength = 0;

                var response = Client.GetResponse() as HttpWebResponse;
                EnsureValidResponse(response);
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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task PostDataAsync(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var cancelToken = TokenSource.Token;

            try
            {
                InitializeClient(contractOrApiPath, "POST");

                //Hp --> Note: As we are not passing request object set content length as zero.
                Client.ContentLength = 0;

                var response = await GetResponseAsync(cancelToken);
                EnsureValidResponse(response, cancelToken);
                await Task.CompletedTask;
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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                InitializeClient(contractOrApiPath, "POST");

                //Hp --> Note: As we are not passing request object set content length as zero.
                Client.ContentLength = 0;

                var response = Client.GetResponse() as HttpWebResponse;
                EnsureValidResponse(response);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);
            var cancelToken = TokenSource.Token;

            try
            {
                InitializeClient(contractOrApiPath, "POST");

                //Hp --> Note: As we are not passing request object set content length as zero.
                Client.ContentLength = 0;

                var response = await GetResponseAsync(cancelToken);
                EnsureValidResponse(response, cancelToken);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "")
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);
            var result = default(TResponse);

            try
            {
                InitializeClient(contractOrApiPath, "POST");
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                //Hp --> Note: HttpWebRequest is not serializing the request properly.
                //So, instead of using HttpWebRequest serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace);
                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);

                Client.ContentLength = Encoding.UTF8.GetByteCount(rawRequestData);
                using (Stream stream = Client.GetRequestStream())
                {
                    stream.Write(requestInBytes, 0, (int)Client.ContentLength);
                }

                var response = Client.GetResponse() as HttpWebResponse;
                EnsureValidResponse(response);

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
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        public override async Task<TResponse> PostDataAsync<TRequest, TResponse>(
            string contractOrApiPath, TRequest requestObject, string requestObjNamespace = "")
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNull(requestObject, () => requestObject);
            var result = default(TResponse);
            var cancelToken = TokenSource.Token;

            try
            {
                InitializeClient(contractOrApiPath, "POST");
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                //Hp --> Note: HttpWebRequest is not serializing the request properly.
                //So, instead of using HttpWebRequest serializing mechanism use rest sharp serializer.
                var rawRequestData = SerializeTo(requestObject, requestObjNamespace, cancelToken);
                var requestInBytes = Encoding.UTF8.GetBytes(rawRequestData);

                Client.ContentLength = Encoding.UTF8.GetByteCount(rawRequestData);
                using (Stream stream = Client.GetRequestStream())
                {
                    stream.Write(requestInBytes, 0, (int)Client.ContentLength);
                }

                var response = await GetResponseAsync(cancelToken);
                EnsureValidResponse(response, cancelToken);

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
        /// Initializes the client.
        /// </summary>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <param name="webMethod">The web method.</param>
        /// <exception cref="ExceptionBase"></exception>
        protected void InitializeClient(string contractOrApiPath, string webMethod = "GET")
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            ErrorBase.CheckArgIsNullOrDefault(webMethod, () => webMethod);

            //Hp --> Logic: Always create new instance on each request.
            Client = WebRequest.Create($"{SvcBaseUrl}{contractOrApiPath}");
            Client.Method = webMethod;
            switch (RestMsgFormat)
            {
                case WebMessageFormat.Xml:
                    {
                        Client.ContentType = "application/xml; charset=utf-8";
                    }
                    break;

                case WebMessageFormat.Json:
                    {
                        Client.ContentType = "application/json; charset=utf-8";
                    }
                    break;

                default:
                    {
                        var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                        throw (new ExceptionBase(errMessage));
                    }
            }
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
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns></returns>
        protected async Task<HttpWebResponse> GetResponseAsync(
            CancellationToken cancelToken = default(CancellationToken))
        {
            CheckForCancellationRequest(cancelToken);
            if (null == Client)
            {
                var errMessage = $"[Method]: {nameof(GetResponseAsync)}" +
                    $"{Environment.NewLine}[Reason]: {nameof(Client)} instance is null!";

                throw (new NullReferenceException(errMessage));
            }

            using (cancelToken.Register(Client.Abort))
            {
                return (await Client.GetResponseAsync() as HttpWebResponse);
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
