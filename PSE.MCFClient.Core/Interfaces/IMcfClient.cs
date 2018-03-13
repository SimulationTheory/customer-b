using System.Collections.Generic;
using System.Net;
using PSE.MCFClient.Core.Models;
using RestSharp;

namespace PSE.MCFClient.Core.Interfaces
{
    public interface IMcfClient
    {
        IRestResponse ExecuteMcfRequestViaJwt(Method method, string requestPath, string jwt, IEnumerable<QueryParameter> parameters = null);
        T ExecuteMcfRequestViaJwt<T>(Method method, string requestPath, string jwt, IEnumerable<QueryParameter> parameters = null);
        IRestResponse ExecuteMcfRequestViaCookies(Method method, string requestPath, IEnumerable<Cookie> cookies, IEnumerable<QueryParameter> parameters = null);
        RestRequest GetRestRequest(Method method, string requestPath, IEnumerable<QueryParameter> parameters = null, IEnumerable<Cookie> cookies = null);
        RestClient GetClient();
    }
}
