using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace PSE.RestUtility.Core.Extensions
{

    public static class RestRequestExtensions
    {
        public static IRestRequest AddCookies(this IRestRequest source, IEnumerable<Cookie> cookies)
        {
            foreach (var cookie in cookies)
            {
                source.AddCookie(cookie.Name, cookie.Value);
            }

            return source;
        }

        public static IRestRequest AddBasicCredentials(this IRestRequest source, string userName, string password)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password)));
            source.AddHeader("Authorization", $"Basic {credentials}");

            return source;
        }

        public static IRestRequest AddJsonBody<T>(this IRestRequest source, T body)
        {
            var json = JsonConvert.SerializeObject(body);
            source.AddParameter("application/json", json, ParameterType.RequestBody);

            return source;
        }
    }
}