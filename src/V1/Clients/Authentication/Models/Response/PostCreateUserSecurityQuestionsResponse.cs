using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class PostCreateUserSecurityQuestionsResponse : IAPIResponse
    {
        public IEnumerable<CreateUpdateUserSecurityQuestionResultModel> Response { get; set; }
    }
}
