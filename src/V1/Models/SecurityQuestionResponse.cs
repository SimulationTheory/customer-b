using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    public class SecurityQuestionResponse: IAPIResponse
    {
        public int Sequence { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }        
    }    
}
