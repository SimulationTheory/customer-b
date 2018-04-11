using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class PostCreateUserSecurityQuestionsRequest : IAPIRequest
    {
        public IEnumerable<CreateUpdateUserSecurityQuestionModel> Request { get; set; }
    }
}
