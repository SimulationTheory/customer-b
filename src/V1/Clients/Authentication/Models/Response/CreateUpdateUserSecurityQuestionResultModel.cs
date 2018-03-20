namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class CreateUpdateUserSecurityQuestionResultModel
    {
        public CreateUpdateUserSecurityQuestionModel Request { get; set; }
        public ValidationResultModel BusinessValidationResult { get; set; }
    }
}