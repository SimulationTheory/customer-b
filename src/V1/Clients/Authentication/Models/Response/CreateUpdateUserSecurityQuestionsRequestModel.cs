namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class CreateUpdateUserSecurityQuestionsRequestModel
    {
        public short Sequence { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}