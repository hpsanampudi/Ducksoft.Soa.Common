using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to create WCF rest service client using HttpClient.
    /// </summary>
    /// <seealso cref="Ducksoft.Soa.Common.RestClientHelpers.ServiceRestFactory{TClient}" />
    public class ServiceHttpClient : ServiceRestFactory<HttpClient>
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected override HttpClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHttpClient" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        public ServiceHttpClient(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace) : base(svcBaseUrl, restMsgFormat, defaultNamespace)
        {
            ConfigureClient();
        }

        /// <summary>
        /// Configures the client settings.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <exception cref="ExceptionBase"></exception>
        public override void ConfigureClient(HttpClient client = null)
        {
            if (null == client)
            {
                Client = new HttpClient();
                var mediaType = default(MediaTypeWithQualityHeaderValue);
                switch (RestMsgFormat)
                {
                    case WebMessageFormat.Xml:
                        {
                            mediaType = new MediaTypeWithQualityHeaderValue("application/xml");
                        }
                        break;

                    case WebMessageFormat.Json:
                        {
                            mediaType = new MediaTypeWithQualityHeaderValue("application/json");
                        }
                        break;

                    default:
                        {
                            var errMessage = $"Rest message format {RestMsgFormat} is not handled!";
                            throw (new ExceptionBase(errMessage));
                        }
                }

                Func<string, StringWithQualityHeaderValue> ParseQualityHeader =
                    (input) => StringWithQualityHeaderValue.Parse(input);

                Client.BaseAddress = new Uri(SvcBaseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(mediaType);
                Client.DefaultRequestHeaders.AcceptEncoding.Add(ParseQualityHeader("gzip"));
                Client.DefaultRequestHeaders.AcceptEncoding.Add(ParseQualityHeader("defalte"));
                Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
                    new ProductHeaderValue("DSOA_ServiceHttpClient", "1.0")));
            }
            else
            {
                Client = client;
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The operation contract path.</param>
        /// <returns></returns>
        public override TResponse GetData<TResponse>(string contractOrApiPath)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () => await GetDataAsync<TResponse>(contractOrApiPath)).Result);
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
                var response = await Client.GetAsync(contractOrApiPath, cancelToken);
                EnsureValidResponse(response, cancelToken);

                //Hp --> Note: HttpClient is not deserializing the response properly.
                //result = await response.Content.ReadAsAsync<TResponse>(cancelToken);
                //So, instead of using HttpClient deserializing mechanism use rest sharp deserializer.
                var rawData = await response.Content.ReadAsStringAsync();
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
        /// Gets the data list.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="contractOrApiPath">The contract or API path.</param>
        /// <returns></returns>        
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () =>
            await GetDataListAsync<TResponse>(contractOrApiPath)).Result);
        }

        /// <summary>
        /// Gets the data list asynchronous.
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
                var response = await Client.GetAsync(contractOrApiPath, cancelToken);
                EnsureValidResponse(response, cancelToken);

                //Hp --> Note: HttpClient is not deserializing the response properly.
                //result = await response.Content.ReadAsAsync<TResponse>(cancelToken);
                //So, instead of using HttpClient deserializing mechanism use rest sharp deserializer.
                var rawData = await response.Content.ReadAsStringAsync();
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
        public override void PostData(string contractOrApiPath)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            Task.Run(async () => await PostDataAsync(contractOrApiPath)).Wait();
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
                //Hp --> Note: Here http content is null as we are not passing request object.
                var response = await Client.PostAsync(contractOrApiPath, null, cancelToken);
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
        public override TResponse PostData<TResponse>(string contractOrApiPath)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () => await PostDataAsync<TResponse>(contractOrApiPath)).Result);
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
                //Hp --> Note: Here http content is null as we are not passing request object.
                var response = await Client.PostAsync(contractOrApiPath, null, cancelToken);
                EnsureValidResponse(response, cancelToken);

                //Hp --> Note: HttpClient is not deserializing the response properly.
                //result = await response.Content.ReadAsAsync<TResponse>(cancelToken);
                //So, instead of using HttpClient deserializing mechanism use rest sharp deserializer.
                var rawData = await response.Content.ReadAsStringAsync();
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
        public override TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "")
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () => await PostDataAsync<TRequest, TResponse>(
                contractOrApiPath, requestObject, requestObjNamespace)).Result);
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
                var response = await Client.PostAsync(
                    contractOrApiPath, requestObject, Formatter, cancelToken);

                EnsureValidResponse(response, cancelToken);

                //Hp --> Note: HttpClient is not deserializing the response properly.
                //result = await response.Content.ReadAsAsync<TResponse>(cancelToken);
                //So, instead of using HttpClient deserializing mechanism use rest sharp deserializer.
                var rawData = await response.Content.ReadAsStringAsync();
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
            if (null != Client)
            {
                Client.Dispose();
            }
        }

        /// <summary>
        /// Ensures the valid response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancelToken">The cancel token.</param>
        protected virtual void EnsureValidResponse(HttpResponseMessage response,
                    CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(response, () => response);
            CheckForCancellationRequest(cancelToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Gets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual MediaTypeFormatter Formatter
        {
            get
            {
                var serializer = default(MediaTypeFormatter);
                switch (RestMsgFormat)
                {
                    case WebMessageFormat.Json:
                        {
                            serializer = new JsonMediaTypeFormatter();
                        }
                        break;

                    case WebMessageFormat.Xml:
                        {
                            serializer = new XmlMediaTypeFormatter();
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
        }

        /// <summary>
        /// Converts to form url encoded content.
        /// </summary>
        /// <param name="requestContent">Content of the request.</param>
        /// <returns></returns>
        private FormUrlEncodedContent ConvertTo(IDictionary<string, string> requestContent)
        {
            ErrorBase.CheckArgIsNull(requestContent, () => requestContent);
            return (new FormUrlEncodedContent(requestContent));
        }

        /// <summary>
        /// Converts to multi part form data content.
        /// </summary>
        /// <param name="requestContent">Content of the request.</param>
        /// <returns></returns>
        private MultipartFormDataContent ConvertTo(IDictionary<string, HttpContent> requestContent)
        {
            ErrorBase.CheckArgIsNull(requestContent, () => requestContent);

            var delimeter = new string[] { ";" };
            var content = new MultipartFormDataContent();
            requestContent.ToList().ForEach(item =>
            {
                var keys = item.Key.Split(delimeter, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (1 == keys.Count)
                {
                    content.Add(item.Value, keys[0]);
                }
                else if (2 == keys.Count)
                {
                    content.Add(item.Value, keys[0], keys[1]);
                }
                else
                {
                    var errMessage = $"The given httpContent key {item.Key} is invalid!";
                    throw (new ExceptionBase(errMessage));
                }
            });

            return (content);
        }
    }
}
