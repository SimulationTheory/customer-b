using Newtonsoft.Json;
using RestSharp;

namespace PSE.RestUtility.Core.Extensions
{
    public static class RestClientExtensions
    {
        public static T ExecuteRequest<T>(this IRestClient source, IRestRequest request)
        {
            var restResponse = source.Execute(request);
            var response = JsonConvert.DeserializeObject<T>(restResponse.Content);

            return response;
        }
    }
}