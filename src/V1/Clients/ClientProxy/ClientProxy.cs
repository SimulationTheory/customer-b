using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.ClientProxy
{
    public class ClientProxy : IClientProxy
    {
        protected const string API_VERSION = "1.0";
        protected readonly ICoreOptions _coreOptions;
        protected IApiUser _apiUser;

        /// <summary>
        /// Constructor for dependency injection
        /// </summary>
        /// <param name="coreOptions">used to get load config info such as the balancer address</param>
        public ClientProxy(ICoreOptions coreOptions)
        {
            _coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
        }

        /// <summary>
        /// Executes the client call asynchronously, typically to another microservice.
        /// Use this overload when a return type is expected.
        /// </summary>
        /// <param name="request">Request to execute asynchronously</param>
        /// <returns>An awaitable task, which returns the specified type along with the response information</returns>
        public Task<IRestResponse> ExecuteAsync(IRestRequest request)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(_coreOptions.Configuration.LoadBalancerUrl)
            };

            // Make call to endpoint in Task thread
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException != null)
                {
                    taskCompletionSource.TrySetException(response.ErrorException);
                }
                else
                {
                    taskCompletionSource.TrySetResult(response);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the client call asynchronously, typically to another microservice
        /// </summary>
        /// <typeparam name="TResponseType">Type of object expected to be returned by API endpoint</typeparam>
        /// <param name="request">Request to execute asynchronously</param>
        /// <returns>An awaitable task, which returns the specified type along with the response information</returns>
        public Task<IRestResponse<TResponseType>> ExecuteAsync<TResponseType>(IRestRequest request)
            where TResponseType : new()
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(_coreOptions.Configuration.LoadBalancerUrl)
            };

            // Make call to endpoint in Task thread
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<TResponseType>>();
            client.ExecuteAsync<TResponseType>(request, response =>
            {
                if (response.ErrorException != null)
                {
                    taskCompletionSource.TrySetException(response.ErrorException);
                }
                else
                {
                    // If successful, response.Data should an object of type T
                    // Other information about the call (such as the http response status code) is also returned
                    taskCompletionSource.TrySetResult(response);
                }
            });

            return taskCompletionSource.Task;
        }
    }
}
