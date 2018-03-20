using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class PostCreateUserSecurityQuestionsRequest : IAPIRequest
    {
        public IEnumerable<CreateUpdateUserSecurityQuestionModel> Request { get; set; }
    }
}
