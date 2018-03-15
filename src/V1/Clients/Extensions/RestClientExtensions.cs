using System;
using System.Threading.Tasks;
using RestSharp;

namespace PSE.Customer.V1.Clients.Extensions
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse> ExecuteTaskAsync(this RestClient client, RestRequest request)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

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
    }
}
