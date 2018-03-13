using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using PSE.Caching.Core.Redis;
using PSE.MCFClient.Core.Extensions;
using PSE.MCFClient.Core.Interfaces;
using PSE.MCFClient.Core.Models;
using RestSharp;
using StackExchange.Redis;

namespace PSE.MCFClient.Core
{
    public class McfClient : IMcfClient
    {
        private readonly Uri _baseUri;
        private readonly string _loadBalancerUrl;
        private readonly IDatabase _redis;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="loadBalancerUrl"></param>
        /// <param name="options"></param>
        /// <param name="db"></param>
        public McfClient(string baseUri, string loadBalancerUrl, RedisOptions options, int db = -1)
        {
            _baseUri = new Uri(baseUri);
            _loadBalancerUrl = loadBalancerUrl;
            var connectionMultiplexer = ConnectionMultiplexer.Connect(options.Configuration);
            _redis = connectionMultiplexer.GetDatabase(db);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="jwt"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IRestResponse ExecuteMcfRequestViaJwt(Method method, string requestPath, string jwt, IEnumerable<QueryParameter> parameters = null)
        {
            var cookies = GetMcfCookies(jwt).Result;
            var client = GetClient();
            var request = GetRestRequest(method, requestPath, parameters, cookies);
            var response = client.Execute(request);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="jwt"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteMcfRequestViaJwt<T>(Method method, string requestPath, string jwt, IEnumerable<QueryParameter> parameters = null)
        {
            var result = ExecuteMcfRequestViaJwt(method, requestPath, jwt, parameters);
            var response = JsonConvert.DeserializeObject<T>(result.Content);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="cookies"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IRestResponse ExecuteMcfRequestViaCookies(Method method, string requestPath, IEnumerable<Cookie> cookies, IEnumerable<QueryParameter> parameters = null)
        {
            var client = GetClient();
            var request = GetRestRequest(method, requestPath, parameters, cookies);
            var response = client.Execute(request);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestPath"></param>
        /// <param name="parameters"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public RestRequest GetRestRequest(Method method, string requestPath, IEnumerable<QueryParameter> parameters = null, IEnumerable<Cookie> cookies = null)
        {
            var request = new RestRequest(requestPath);
            request.Method = method;            
            request.AddHeader("Accept", "application/json");

            if (cookies != null)
            {
                request.AddMcfCookies(cookies);
            }
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddQueryParameter(parameter.Name, parameter.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RestClient GetClient()
        {
            var restClient = new RestClient(_baseUri);
            restClient.RemoteCertificateValidationCallback = ValidateServerCertificate;

            return restClient;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public TokenCookies GetMcfCookies(string jwtToken)
        {
            TokenCookies cookies = null;
            RedisKey key = $"authentication:{jwtToken}";
            var cookiesJson = _redis.StringGet(key);

            if (cookiesJson.IsNullOrEmpty)
            {
                var client = new RestClient(_loadBalancerUrl);
                var request = new RestRequest("/v1.0/authentication/mcf-token", Method.GET);
                request.AddHeader("Authorization", jwtToken);

                var result = client.Execute(request);

                cookiesJson = result.Content;

            }
            cookies = JsonConvert.DeserializeObject<TokenCookies>(cookiesJson);
            

            return cookies;
        }

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion
    }
}