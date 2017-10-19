using RestSharp;
using RestSharp.Serializers;
using Ducksoft.Soa.Common.Utilities;
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
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected override RestClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRSharpClient" /> class.
        /// </summary>
        /// <param name="svcBaseUrl">The service base URL.</param>
        /// <param name="restMsgFormat">The web request and response message format.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        public ServiceRSharpClient(string svcBaseUrl, WebMessageFormat restMsgFormat,
            string defaultNamespace) : base(svcBaseUrl, restMsgFormat, defaultNamespace)
        {
            ConfigureClient();
        }

        /// <summary>
        /// Configures the client settings.
        /// </summary>
        /// <param name="client">The client.</param>
        public override void ConfigureClient(RestClient client = null)
        {
            if (null == client)
            {
                Client = new RestClient(SvcBaseUrl);
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
        /// <exception cref="ExceptionBase"></exception>
        public override TResponse GetData<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = default(TResponse);

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var response = Client.Execute(request);
                EnsureValidResponse(response);
                result = DeserializeFrom<TResponse>(response);
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
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var response = await Client.ExecuteTaskAsync(request, cancelToken);
                EnsureValidResponse(response, cancelToken);
                result = DeserializeFrom<TResponse>(response, cancelToken);
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
        /// <exception cref="ExceptionBase"></exception>
        public override List<TResponse> GetDataList<TResponse>(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);
            var result = new List<TResponse>();

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET);
                var response = Client.Execute<List<TResponse>>(request);
                EnsureValidResponse(response);
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
                var request = GetDefaultRestRequest(contractOrApiPath, Method.GET);
                var response = await Client.ExecuteTaskAsync<List<TResponse>>(request, cancelToken);
                EnsureValidResponse(response, cancelToken);
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
        /// <exception cref="ExceptionBase"></exception>
        public override void PostData(string contractOrApiPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            try
            {
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var response = Client.Execute(request);
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
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);
                var response = await Client.ExecuteTaskAsync(request, cancelToken);
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
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var response = Client.Execute(request);
                EnsureValidResponse(response);
                result = DeserializeFrom<TResponse>(response);
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
                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);
                var response = await Client.ExecuteTaskAsync(request, cancelToken);
                EnsureValidResponse(response, cancelToken);
                result = DeserializeFrom<TResponse>(response, cancelToken);
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
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);
                request.AddBody(requestObject, requestObjNamespace);

                //Hp --> Note: For rest sharp Execute<T> method, T should be new() instance type.
                //So, We can't use TResponse here. The alternate way is get IRestResponse and
                //deserialize it based on response content type.
                var response = Client.Execute(request);
                EnsureValidResponse(response);
                result = DeserializeFrom<TResponse>(response);
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
                requestObjNamespace =
                    ((requestObjNamespace ?? DefaultNamespace) ?? string.Empty).Trim();

                var request = GetDefaultRestRequest(contractOrApiPath, Method.POST);
                request.AddBody(requestObject, requestObjNamespace);

                var response = await Client.ExecuteTaskAsync(request, cancelToken);
                EnsureValidResponse(response, cancelToken);
                result = DeserializeFrom<TResponse>(response, cancelToken);
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
        /// <returns></returns>
        protected IRestRequest GetDefaultRestRequest(string contractOrApiPath, Method webMethodType)
        {
            ErrorBase.CheckArgIsNullOrDefault(contractOrApiPath, () => contractOrApiPath);

            var request = new RestRequest(contractOrApiPath, webMethodType);
            request.RequestFormat = RestDataFormat;
            request.DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            GetSerializer(request);

            return (request);
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        protected virtual ISerializer GetSerializer(IRestRequest request)
        {
            ErrorBase.CheckArgIsNull(request, () => request);
            var serializer = default(ISerializer);

            switch (request.RequestFormat)
            {
                case DataFormat.Json:
                    {
                        serializer = request.JsonSerializer;
                        serializer.ContentType = "application/json; charset=utf-8";
                    }
                    break;

                case DataFormat.Xml:
                    {
                        serializer = request.XmlSerializer;
                        serializer.ContentType = "application/xml; charset=utf-8";
                    }
                    break;

                default:
                    {
                        var errMessage = $"Request format {request.RequestFormat} is not handled!";
                        throw (new ExceptionBase(errMessage));
                    }
            }

            //Hp --> Note: For serialization we need to set date format explicity.
            serializer.DateFormat = "yyyy-MM-ddTHH:mm:ssZ";

            return (serializer);
        }
    }
}
