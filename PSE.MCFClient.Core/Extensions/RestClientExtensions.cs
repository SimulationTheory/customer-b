using System.Collections.Generic;
using Newtonsoft.Json;
using PSE.MCFClient.Core.Interfaces;
using PSE.MCFClient.Core.Models;
using RestSharp;

namespace PSE.MCFClient.Core.Extensions
{
    public static class RestClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static T ExecuteMcfRequest<T>(this IRestClient source, IRestRequest request) where T : IMcfResponse
        {
            var restResponse = source.Execute(request);
            var response = JsonConvert.DeserializeObject<McfResponse<T>>(restResponse.Content);

            return response.Result;
        }
    }
}