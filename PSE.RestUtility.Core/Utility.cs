using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PSE.Caching.Core.Redis;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Models;
using RestSharp;
using StackExchange.Redis;

namespace PSE.RestUtility.Core
{
    public class Utility : IRestUtility
    {
        private readonly string _loadBalancerUrl;
        private readonly IDatabase _redis;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadBalancerUrl"></param>
        /// <param name="options"></param>
        /// <param name="db"></param>
        public Utility(string loadBalancerUrl, RedisOptions options, int db = -1)
        {
            _loadBalancerUrl = loadBalancerUrl;
            var connectionMultiplexer = ConnectionMultiplexer.Connect(options.Configuration);
            _redis = connectionMultiplexer.GetDatabase(db);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="forceRemoteCertValidation"></param>
        /// <returns></returns>
        public IRestClient GetRestClient(string url, bool forceRemoteCertValidation = true)
        {
            var restClient = new RestClient(url);

            if (forceRemoteCertValidation)
            {
                restClient.RemoteCertificateValidationCallback = ValidateServerCertificate;
            }

            return restClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public TokenCookies GetMcfCookies(string jwtToken, string channel)
        {
            TokenCookies cookies = null;

            RedisKey key = $"authentication:{jwtToken}";
            var cookiesJson = _redis.StringGet(key);

            if (cookiesJson.IsNullOrEmpty)
            {
                var client = GetRestClient(_loadBalancerUrl, true);
                var request = new RestRequest("/v1.0/authentication/mcf-token", Method.GET);
                request.AddHeader("Authorization", jwtToken);
                request.AddHeader("request-channel", channel);
                var result = client.Execute(request);

                cookiesJson = result.Content;

            }

            cookies = JsonConvert.DeserializeObject<TokenCookies>(cookiesJson);

            return cookies;
        }

        public async Task<TokenCookies> GetMcfCookiesByBpAsync(long bpNumber)
        {
            TokenCookies cookies = null;

            RedisKey key = $"authentication:mcf:nojwt:{bpNumber}";
            var cookiesJson = _redis.StringGet(key);

            if (cookiesJson.IsNullOrEmpty)
            {
                var client = GetRestClient(_loadBalancerUrl, true);
                var request = new RestRequest($"/v1.0/authentication/mcf-token/{bpNumber}", Method.GET);

                var result = await client.ExecuteTaskAsync(request);

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