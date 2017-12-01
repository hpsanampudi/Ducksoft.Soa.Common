using Ducksoft.Soa.Common.DataContracts;
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
        /// Gets the default client.
        /// </summary>
        /// <value>
        /// The default client.
        /// </value>
        /// <exception cref="ExceptionBase"></exception>
        protected override HttpClient DefaultClient
        {
            get
            {
                var client = new HttpClient();
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

                client.BaseAddress = new Uri(SvcBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(mediaType);
                client.DefaultRequestHeaders.AcceptEncoding.Add(ParseQualityHeader("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(ParseQualityHeader("defalte"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
                    new ProductHeaderValue("SOA_ServiceHttpClient", "1.0")));

                return (client);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHttpClient" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="authType">Type of the authentication.</param>
        public ServiceHttpClient(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace, ServiceAuthTypes authType)
            : base(svcBaseUrl, restMsgFormat, defaultNamespace, authType)
        {
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <returns></returns>
        protected HttpClient InitializeClient()
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        tokenResponse.TokenType, tokenResponse.AccessToken);
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
        public override TResponse GetData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () =>
            await GetDataAsync<TResponse>(contractOrApiPath, isIgnoreError)).Result);
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
                var client = InitializeClient();
                var response = await client.GetAsync(contractOrApiPath, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () =>
            await GetDataListAsync<TResponse>(contractOrApiPath, isIgnoreError)).Result);
        }

        /// <summary>
        /// Gets the data list asynchronous.
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
                var client = InitializeClient();
                var response = await client.GetAsync(contractOrApiPath, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        public override void PostData(string contractOrApiPath, bool isIgnoreError = false)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            Task.Run(async () => await PostDataAsync(contractOrApiPath, isIgnoreError)).Wait();
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
                //Hp --> Note: Here http content is null as we are not passing request object.
                var client = InitializeClient();
                var response = await client.PostAsync(contractOrApiPath, null, cancelToken);
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
        public override TResponse PostData<TResponse>(string contractOrApiPath,
            bool isIgnoreError = false)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () =>
            await PostDataAsync<TResponse>(contractOrApiPath, isIgnoreError)).Result);
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
                //Hp --> Note: Here http content is null as we are not passing request object.
                var client = InitializeClient();
                var response = await client.PostAsync(contractOrApiPath, null, cancelToken);
                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

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
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <returns></returns>
        public override TResponse PostData<TRequest, TResponse>(string contractOrApiPath,
            TRequest requestObject, string requestObjNamespace = "", bool isIgnoreError = false)
        {
            //Hp --> Logic: Http client always executes request asynchronously.
            return (Task.Run(async () => await PostDataAsync<TRequest, TResponse>(
                contractOrApiPath, requestObject, requestObjNamespace, isIgnoreError)).Result);
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
                var client = InitializeClient();
                var response = await client.PostAsync(
                    contractOrApiPath, requestObject, Formatter, cancelToken);

                if (!isIgnoreError)
                {
                    EnsureValidResponse(response, cancelToken);
                }

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
