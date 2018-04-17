using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service.Enums;
using RestSharp;

namespace PSE.Customer.V1.Clients.Authentication.Interfaces
{
    public interface IAuthenticationApi : IClientProxy
    {
        Task<IRestResponse<AccountExistsResponse>> GetAccountExists(long bpId);

        Task<IRestResponse<ExistsResponse>> GetUserNameExists(string userName);

        Task<IRestResponse<OkResult>> SignUpCustomer(WebProfile profileInfo);

        Task<IRestResponse<OkResult>> SignUpCustomer(SignupRequest signUpInfo);

        Task<IRestResponse<SignInResponse>> GetJwtToken(string username, string password);

        Task<IRestResponse<PostCreateUserSecurityQuestionsResponse>> SaveSecurityQuestions(WebProfile profileInfo, string jwtToken);
        Task<IRestResponse<PutSyncUserEmailResponse>> SyncUserEmail(string jwt, RequestChannelEnum requestChannel);
    }
}
