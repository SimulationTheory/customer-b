using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class CustomerCredentials
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
