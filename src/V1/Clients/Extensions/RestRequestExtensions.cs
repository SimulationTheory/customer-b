using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Clients.Extensions
{
    public static class RestRequestExtensions
    {
        /// <summary>
        /// Configures the authentication header to use the Java Web Token
        /// </summary>
        /// <param name="request">The request to update</param>
        /// <param name="jwtAccessToken">Access token from sign up</param>
        /// <returns>The updated request object</returns>
        public static IRestRequest SetJwtAuthorization(this IRestRequest request, string jwtAccessToken)
        {
            if (!string.IsNullOrEmpty(jwtAccessToken))
            {
                request.AddParameter("Authorization", jwtAccessToken, ParameterType.HttpHeader);
            }

            return request;
        }
    }
}
