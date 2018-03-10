using PSE.WebAPI.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class SecurityQuestionResponse: IAPIResponse
    {
        [Required]
        public int Sequence { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Answer { get; set; }        
    }    
}
