using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class CreateUpdateUserSecurityQuestionResponseModel
    {
        public CreateUpdateUserSecurityQuestionsRequestModel Request { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
