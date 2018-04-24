using PSE.RestUtility.Core.Models;
using RestSharp;

namespace PSE.RestUtility.Core.Interfaces
{
    public interface IRestUtility
    {
        IRestClient GetRestClient(string url = null, bool forceRemoteCertValidation = true);
        TokenCookies GetMcfCookies(string jwtToken, string channel);
    }
}
