namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class CreateUpdateUserSecurityQuestionResponseModel
    {
        public CreateUpdateUserSecurityQuestionsRequestModel Request { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
