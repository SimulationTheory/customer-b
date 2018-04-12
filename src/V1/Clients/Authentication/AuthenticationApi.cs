using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.Customer.V1.Clients.Extensions;
using RestSharp;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Authentication
{
    /// <summary>
    /// Makes calls to the Authentication endpoints
    /// </summary>
    public class AuthenticationApi : ClientProxy.ClientProxy, IAuthenticationApi
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="coreOptions"></param>
        public AuthenticationApi(ICoreOptions coreOptions) : base(coreOptions)
        {
        }

        /// <summary>
        /// Calls Account exists APi
        /// </summary>
        /// <param name="bpId"></param>
        /// <returns></returns>

        public Task<IRestResponse<AccountExistsResponse>> GetAccountExists(long bpId)
        {
            var request = new RestRequest($"/v{API_VERSION}/authentication/mypse-account-exists/{bpId}");
            return ExecuteAsync<AccountExistsResponse>(request);
        }

        /// <summary>
        /// Calls Usrename exist API in Authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<IRestResponse<ExistsResponse>> GetUserNameExists(string userName)
        {
            var request = new RestRequest($"/v{API_VERSION}/authentication/user-name-exists/{userName}");
            return ExecuteAsync<ExistsResponse>(request);
        }

        /// <summary>
        /// Calls the Signup Api in authentication repo to create users in Cognito as well as Cassandra
        /// </summary>
        /// <param name="profileInfo">contains information needed to sign the customer up</param>
        /// <returns></returns>
        public Task<IRestResponse<OkResult>> SignUpCustomer(WebProfile profileInfo)
        {
            var signUpInfo = new SignupRequest
            {
                BPId = profileInfo.BPId,
                Username = profileInfo.CustomerCredentials.UserName,
                Password = profileInfo.CustomerCredentials.Password,
                Email = profileInfo.Email
            };

            return SignUpCustomer(signUpInfo);
        }

        /// <summary>
        /// Calls the Signup Api in authentication repo to create users in Cognito as well as Cassandra
        /// </summary>
        /// <param name="signUpInfo">contains information needed to sign the customer up</param>
        /// <returns></returns>
        public async Task<IRestResponse<OkResult>> SignUpCustomer(SignupRequest signUpInfo)
        {
            var request = new RestRequest($"/v{API_VERSION}/authentication/signup", Method.POST);

            var body = JsonConvert.SerializeObject(signUpInfo);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var resp = await ExecuteAsync<OkResult>(request);
            return resp;
        }

        /// <summary>
        /// Calls the Signup Api in authentication repo to create users in Cognito as well as Cassandra
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IRestResponse<SignInResponse>> GetJwtToken(string username, string password)
        {
            var request = new RestRequest($"/v{API_VERSION}/authentication/signin", Method.POST);

            var requestBody = new SignInRequest
            {
                Username = username,
                Password = password,

            };
            request.AddHeader("ContentType", "application/json");
            request.AddHeader("Accept", "application/json");
            var body = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var resp = await ExecuteAsync<SignInResponse>(request);
            return resp;
        }

        /// <summary>
        /// Calls the Signup Api in authentication repo to save user selected security questions
        /// /authentication/security-question/user
        /// </summary>
        /// <param name="profileInfo"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<IRestResponse<PostCreateUserSecurityQuestionsResponse>> SaveSecurityQuestions(WebProfile profileInfo, string jwtToken)
        {

            var request = new RestRequest("/v1.0/authentication/security-question/user", Method.POST);
            request.SetJwtAuthorization(jwtToken);

            request.AddHeader("ContentType", "application/json");
            request.AddHeader("Accept", "application/json");

            var req1 = new List<CreateUpdateUserSecurityQuestionModel>();
            profileInfo.SecurityQuestionResponses.ToList().ForEach(s => req1.Add(new CreateUpdateUserSecurityQuestionModel() { Sequence = s.Sequence, Question = s.Question, Answer = s.Answer }));
            var requestBody = new PostCreateUserSecurityQuestionsRequest()
            {
                Request = req1
            };
            var body = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var resp = await ExecuteAsync<PostCreateUserSecurityQuestionsResponse>(request);
            return resp;
        }
    }
}
