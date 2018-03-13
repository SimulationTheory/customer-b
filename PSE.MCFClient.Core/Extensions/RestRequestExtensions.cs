using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using RestSharp;

namespace PSE.MCFClient.Core.Extensions
{

    public static class RestRequestExtensions
    {
        public static IRestRequest AddMcfCookies(this IRestRequest source, IEnumerable<Cookie> cookies)
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
    }
}