using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    public class SecurityQuestionResponse: IAPIResponse
    {
        public string Id { get; set; }
        public string Answer { get; set; }
    }    
}
